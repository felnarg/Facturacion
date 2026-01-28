using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Usuarios.Domain.Constants;
using Usuarios.Domain.Entities;
using Usuarios.Infrastructure.Data;

namespace Usuarios.Infrastructure.Seeding;

/// <summary>
/// Servicio para inicializar datos base del sistema RBAC:
/// - Permisos predefinidos
/// - Roles predefinidos con sus permisos asignados
/// </summary>
public sealed class RbacSeeder
{
    private readonly UsuariosDbContext _dbContext;
    private readonly ILogger<RbacSeeder> _logger;

    public RbacSeeder(UsuariosDbContext dbContext, ILogger<RbacSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedPermissionsAsync(cancellationToken);
        await SeedRolesAsync(cancellationToken);
        await SeedRolePermissionsAsync(cancellationToken);
        await SeedDefaultAdminAsync(cancellationToken);
    }

    /// <summary>
    /// Crea un usuario administrador por defecto si no existe ningún usuario con rol superadmin
    /// </summary>
    private async Task SeedDefaultAdminAsync(CancellationToken cancellationToken)
    {
        // Verificar si ya existe un superadmin
        var superAdminRole = await _dbContext.Roles
            .FirstOrDefaultAsync(r => r.Code == RoleCodes.SuperAdmin, cancellationToken);
        
        if (superAdminRole is null)
        {
            _logger.LogWarning("No se encontró el rol superadmin");
            return;
        }

        var existingSuperAdmin = await _dbContext.UserRoles
            .Include(ur => ur.User)
            .Where(ur => ur.RoleId == superAdminRole.Id && (ur.ExpiresAt == null || ur.ExpiresAt > DateTime.UtcNow))
            .FirstOrDefaultAsync(cancellationToken);

        if (existingSuperAdmin is not null)
        {
            _logger.LogInformation("Ya existe un usuario superadmin: {Email}", existingSuperAdmin.User.Email);
            return;
        }

        // Crear usuario admin por defecto
        const string defaultEmail = "admin@zupply.com";
        const string defaultPassword = "Admin123!";
        
        // Verificar si ya existe el email
        var existingUser = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == defaultEmail, cancellationToken);

        if (existingUser is not null)
        {
            // Asignar rol superadmin al usuario existente
            var userRole = new UserRole(existingUser.Id, superAdminRole.Id);
            _dbContext.UserRoles.Add(userRole);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Se asignó rol superadmin al usuario existente: {Email}", defaultEmail);
            return;
        }

        // Crear nuevo usuario
        var passwordHash = ComputeHash(defaultPassword);
        var adminUser = new User("Administrador", defaultEmail, passwordHash);
        
        // Agregar rol superadmin
        var adminUserRole = new UserRole(adminUser.Id, superAdminRole.Id);
        adminUser.AddRole(adminUserRole);
        
        _dbContext.Users.Add(adminUser);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("═════════════════════════════════════════════════════════════");
        _logger.LogInformation("✓ Usuario administrador creado:");
        _logger.LogInformation("  Email: {Email}", defaultEmail);
        _logger.LogInformation("  Password: {Password}", defaultPassword);
        _logger.LogInformation("  ¡IMPORTANTE: Cambia esta contraseña después del primer login!");
        _logger.LogInformation("═════════════════════════════════════════════════════════════");
    }

    private static string ComputeHash(string input)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }

    private async Task SeedPermissionsAsync(CancellationToken cancellationToken)
    {
        var existingCodes = await _dbContext.Permissions
            .Select(p => p.Code)
            .ToListAsync(cancellationToken);

        var permissionsToAdd = Permissions.GetAll()
            .Where(p => !existingCodes.Contains(p.Code))
            .Select(p => new Permission(p.Code, p.Name, p.Description, p.Module))
            .ToList();

        if (permissionsToAdd.Any())
        {
            _dbContext.Permissions.AddRange(permissionsToAdd);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Se crearon {Count} permisos nuevos", permissionsToAdd.Count);
        }
    }

    private async Task SeedRolesAsync(CancellationToken cancellationToken)
    {
        var existingCodes = await _dbContext.Roles
            .Select(r => r.Code)
            .ToListAsync(cancellationToken);

        var rolesToAdd = RoleCodes.GetAll()
            .Where(r => !existingCodes.Contains(r.Code))
            .Select(r => new Role(r.Code, r.Name, r.Description, r.HierarchyLevel, r.IsSystemRole))
            .ToList();

        if (rolesToAdd.Any())
        {
            _dbContext.Roles.AddRange(rolesToAdd);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Se crearon {Count} roles nuevos", rolesToAdd.Count);
        }
    }

    private async Task SeedRolePermissionsAsync(CancellationToken cancellationToken)
    {
        // Obtener todos los roles y permisos
        var roles = await _dbContext.Roles
            .Include(r => r.RolePermissions)
            .ToDictionaryAsync(r => r.Code, cancellationToken);

        var permissions = await _dbContext.Permissions
            .ToDictionaryAsync(p => p.Code, cancellationToken);

        var rolePermissionsToAdd = new List<RolePermission>();

        // ═══════════════════════════════════════════════════════════════════════════
        // SUPERADMIN - Todos los permisos
        // ═══════════════════════════════════════════════════════════════════════════
        if (roles.TryGetValue(RoleCodes.SuperAdmin, out var superAdmin))
        {
            var allPermissionCodes = permissions.Keys.ToList();
            AddPermissionsToRole(superAdmin, allPermissionCodes, permissions, rolePermissionsToAdd);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // ADMINISTRADOR DE TIENDA
        // ═══════════════════════════════════════════════════════════════════════════
        if (roles.TryGetValue(RoleCodes.StoreManager, out var storeManager))
        {
            var managerPermissions = new[]
            {
                Permissions.UsersRead, Permissions.UsersCreate, Permissions.UsersUpdate,
                Permissions.UsersRolesManage, Permissions.RolesRead,
                Permissions.CatalogRead, Permissions.CatalogManage,
                Permissions.InventoryStockRead, Permissions.InventoryStockUpdate, Permissions.InventoryPricesEdit,
                Permissions.OrdersCreate, Permissions.OrdersReadAll, Permissions.OrdersStatusUpdate, 
                Permissions.OrdersRefund, Permissions.OrdersCancel, Permissions.OrdersDiscountApply,
                Permissions.PosPaymentProcess, Permissions.PosCashierClose, Permissions.PosReceiptPrint, Permissions.PosCashierReadAll,
                Permissions.ReportsSales, Permissions.ReportsInventory,
                Permissions.SettingsStore
            };
            AddPermissionsToRole(storeManager, managerPermissions, permissions, rolePermissionsToAdd);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // GESTOR DE INVENTARIO
        // ═══════════════════════════════════════════════════════════════════════════
        if (roles.TryGetValue(RoleCodes.InventoryManager, out var inventoryManager))
        {
            var inventoryPermissions = new[]
            {
                Permissions.CatalogRead, Permissions.CatalogManage,
                Permissions.InventoryStockRead, Permissions.InventoryStockUpdate, 
                Permissions.InventoryPricesEdit, Permissions.InventorySuppliersManage, Permissions.InventoryCategoriesManage,
                Permissions.ReportsInventory
            };
            AddPermissionsToRole(inventoryManager, inventoryPermissions, permissions, rolePermissionsToAdd);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // SUPERVISOR DE CAJAS
        // ═══════════════════════════════════════════════════════════════════════════
        if (roles.TryGetValue(RoleCodes.CashierSupervisor, out var supervisor))
        {
            var supervisorPermissions = new[]
            {
                Permissions.CatalogRead,
                Permissions.OrdersCreate, Permissions.OrdersReadAll, Permissions.OrdersStatusUpdate,
                Permissions.OrdersRefund, Permissions.OrdersCancel, Permissions.OrdersDiscountApply,
                Permissions.PosPaymentProcess, Permissions.PosCashierClose, Permissions.PosReceiptPrint, Permissions.PosCashierReadAll,
                Permissions.InventoryStockRead
            };
            AddPermissionsToRole(supervisor, supervisorPermissions, permissions, rolePermissionsToAdd);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // CAJERO
        // ═══════════════════════════════════════════════════════════════════════════
        if (roles.TryGetValue(RoleCodes.Cashier, out var cashier))
        {
            var cashierPermissions = new[]
            {
                Permissions.CatalogRead,
                Permissions.OrdersCreate, Permissions.OrdersReadOwn,
                Permissions.PosPaymentProcess, Permissions.PosReceiptPrint,
                Permissions.InventoryStockRead
            };
            AddPermissionsToRole(cashier, cashierPermissions, permissions, rolePermissionsToAdd);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // PICKER (Recopilador)
        // ═══════════════════════════════════════════════════════════════════════════
        if (roles.TryGetValue(RoleCodes.Picker, out var picker))
        {
            var pickerPermissions = new[]
            {
                Permissions.CatalogRead,
                Permissions.PickerOrdersRead, Permissions.PickerOrdersUpdate,
                Permissions.InventoryStockRead
            };
            AddPermissionsToRole(picker, pickerPermissions, permissions, rolePermissionsToAdd);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // REPARTIDOR (Delivery)
        // ═══════════════════════════════════════════════════════════════════════════
        if (roles.TryGetValue(RoleCodes.DeliveryDriver, out var delivery))
        {
            var deliveryPermissions = new[]
            {
                Permissions.DeliveryAddressRead, Permissions.DeliveryStatusUpdate
            };
            AddPermissionsToRole(delivery, deliveryPermissions, permissions, rolePermissionsToAdd);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // CLIENTE
        // ═══════════════════════════════════════════════════════════════════════════
        if (roles.TryGetValue(RoleCodes.Customer, out var customer))
        {
            var customerPermissions = new[]
            {
                Permissions.CatalogRead,
                Permissions.OrdersCreate, Permissions.OrdersReadOwn
            };
            AddPermissionsToRole(customer, customerPermissions, permissions, rolePermissionsToAdd);
        }

        // Agregar todas las relaciones nuevas
        if (rolePermissionsToAdd.Any())
        {
            _dbContext.RolePermissions.AddRange(rolePermissionsToAdd);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Se asignaron {Count} permisos a roles", rolePermissionsToAdd.Count);
        }
    }

    private static void AddPermissionsToRole(
        Role role,
        IEnumerable<string> permissionCodes,
        Dictionary<string, Permission> allPermissions,
        List<RolePermission> rolePermissionsToAdd)
    {
        var existingPermissionIds = role.RolePermissions.Select(rp => rp.PermissionId).ToHashSet();

        foreach (var code in permissionCodes)
        {
            if (allPermissions.TryGetValue(code, out var permission) && 
                !existingPermissionIds.Contains(permission.Id))
            {
                rolePermissionsToAdd.Add(new RolePermission(role.Id, permission.Id));
            }
        }
    }
}

/// <summary>
/// Extensión para registrar el seeder como servicio
/// </summary>
public static class RbacSeederExtensions
{
    public static IServiceCollection AddRbacSeeder(this IServiceCollection services)
    {
        services.AddScoped<RbacSeeder>();
        return services;
    }

    public static async Task SeedRbacDataAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<RbacSeeder>();
        await seeder.SeedAsync();
    }
}
