using Microsoft.EntityFrameworkCore;
using Usuarios.Domain.Entities;
using Usuarios.Domain.Repositories;
using Usuarios.Infrastructure.Data;

namespace Usuarios.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly UsuariosDbContext _dbContext;

    public UserRepository(UsuariosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<User?> GetByIdWithRolesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Include(user => user.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<User?> GetByIdWithRolesAndPermissionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Include(user => user.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(role => role.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return await _dbContext.Users.FirstOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);
    }

    public async Task<User?> GetByEmailWithRolesAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return await _dbContext.Users
            .Include(user => user.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);
    }

    public async Task<User?> GetByEmailWithRolesAndPermissionsAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return await _dbContext.Users
            .Include(user => user.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(role => role.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.AsNoTracking()
            .Include(user => user.UserRoles)
                .ThenInclude(ur => ur.Role)
            .OrderBy(user => user.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAllWithRolesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.AsNoTracking()
            .Include(user => user.UserRoles)
                .ThenInclude(ur => ur.Role)
            .OrderBy(user => user.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.AsNoTracking()
            .Include(user => user.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(user => user.IsActive)
            .OrderBy(user => user.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.AsNoTracking()
            .Include(user => user.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(user => user.UserRoles.Any(ur => ur.RoleId == roleId))
            .OrderBy(user => user.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        // No llamamos a Update explícitamente porque confiamos en el Change Tracker de EF Core
        // al trabajar con entidades cargadas en el mismo contexto.
        // Llamar a Update forzaría el estado Modified en todo el grafo, lo cual puede causar
        // problemas de concurrencia o actualizaciones innecesarias.
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        // Las relaciones UserRole se eliminan en cascada por la configuración de EF
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SyncUserRolesAsync(Guid userId, IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default)
    {
        var roleIdSet = roleIds.ToHashSet();

        // Obtener roles actuales del usuario directamente de la tabla UserRoles
        var currentUserRoles = await _dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .ToListAsync(cancellationToken);

        // Identificar roles a eliminar
        var rolesToRemove = currentUserRoles
            .Where(ur => !roleIdSet.Contains(ur.RoleId))
            .ToList();

        // Identificar roles a agregar
        var existingRoleIds = currentUserRoles.Select(ur => ur.RoleId).ToHashSet();
        var rolesToAdd = roleIdSet
            .Where(roleId => !existingRoleIds.Contains(roleId))
            .Select(roleId => new UserRole(userId, roleId))
            .ToList();

        // Aplicar cambios directamente en el DbSet
        if (rolesToRemove.Count > 0)
        {
            _dbContext.UserRoles.RemoveRange(rolesToRemove);
        }

        if (rolesToAdd.Count > 0)
        {
            _dbContext.UserRoles.AddRange(rolesToAdd);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return await _dbContext.Users.AnyAsync(user => user.Email == normalizedEmail, cancellationToken);
    }
}

