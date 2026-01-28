namespace Usuarios.Domain.Constants;

/// <summary>
/// Códigos predefinidos de roles del sistema
/// </summary>
public static class RoleCodes
{
    // ═══════════════════════════════════════════════════════════════════════════
    // ROLES ADMINISTRATIVOS
    // ═══════════════════════════════════════════════════════════════════════════
    
    /// <summary>Dueño o administrador de TI - Control total</summary>
    public const string SuperAdmin = "superadmin";
    
    /// <summary>Gerente local de la sucursal</summary>
    public const string StoreManager = "store_manager";
    
    /// <summary>Encargado de bodega y catálogo</summary>
    public const string InventoryManager = "inventory_manager";
    
    // ═══════════════════════════════════════════════════════════════════════════
    // ROLES OPERATIVOS
    // ═══════════════════════════════════════════════════════════════════════════
    
    /// <summary>Usuario de punto de venta (POS)</summary>
    public const string Cashier = "cashier";
    
    /// <summary>Apoyo al personal de ventas</summary>
    public const string CashierSupervisor = "cashier_supervisor";
    
    /// <summary>Encargado de pedidos online (picker)</summary>
    public const string Picker = "picker";
    
    // ═══════════════════════════════════════════════════════════════════════════
    // ROLES DE CLIENTE Y EXTERNOS
    // ═══════════════════════════════════════════════════════════════════════════
    
    /// <summary>Usuario de la App/Web</summary>
    public const string Customer = "customer";
    
    /// <summary>Personal de entrega</summary>
    public const string DeliveryDriver = "delivery_driver";
    
    /// <summary>
    /// Obtiene información de todos los roles predefinidos
    /// </summary>
    public static IReadOnlyList<RoleInfo> GetAll() => new List<RoleInfo>
    {
        new(SuperAdmin, "Super Administrador", "Control total del sistema, gestión de TI y auditoría", 100, true),
        new(StoreManager, "Administrador de Tienda", "Gestión de empleados, reportes y parámetros de sucursal", 80, true),
        new(InventoryManager, "Gestor de Inventario", "Gestión de productos, proveedores y stock", 60, true),
        new(CashierSupervisor, "Supervisor de Cajas", "Anulación de facturas, devoluciones y cierres de caja", 50, true),
        new(Cashier, "Cajero", "Procesamiento de ventas y pagos en punto de venta", 30, true),
        new(Picker, "Recopilador", "Gestión de pedidos online y recolección de productos", 30, true),
        new(DeliveryDriver, "Repartidor", "Entrega de pedidos y actualización de estados", 20, true),
        new(Customer, "Cliente", "Acceso a catálogo, carrito y pedidos propios", 10, true),
    };
}

/// <summary>
/// Información detallada de un rol
/// </summary>
public record RoleInfo(string Code, string Name, string Description, int HierarchyLevel, bool IsSystemRole);
