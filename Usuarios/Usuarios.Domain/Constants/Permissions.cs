namespace Usuarios.Domain.Constants;

/// <summary>
/// Constantes que definen todos los permisos disponibles en el sistema.
/// Estos valores se usan como Claims en el JWT y para validación de autorización.
/// </summary>
public static class Permissions
{
    // ═══════════════════════════════════════════════════════════════════════════
    // MÓDULO: GESTIÓN DE USUARIOS
    // ═══════════════════════════════════════════════════════════════════════════
    
    /// <summary>Crear nuevos empleados/usuarios</summary>
    public const string UsersCreate = "users.create";
    
    /// <summary>Ver lista de usuarios y sus roles</summary>
    public const string UsersRead = "users.read";
    
    /// <summary>Editar información de perfil o cambiar contraseñas</summary>
    public const string UsersUpdate = "users.update";
    
    /// <summary>Desactivar cuentas (baja lógica)</summary>
    public const string UsersDelete = "users.delete";
    
    /// <summary>Gestionar roles de usuarios</summary>
    public const string UsersRolesManage = "users.roles.manage";
    
    // ═══════════════════════════════════════════════════════════════════════════
    // MÓDULO: ROLES Y PERMISOS
    // ═══════════════════════════════════════════════════════════════════════════
    
    /// <summary>Ver roles y permisos del sistema</summary>
    public const string RolesRead = "roles.read";
    
    /// <summary>Crear y modificar roles</summary>
    public const string RolesManage = "roles.manage";
    
    /// <summary>Asignar permisos a roles</summary>
    public const string PermissionsManage = "permissions.manage";
    
    // ═══════════════════════════════════════════════════════════════════════════
    // MÓDULO: INVENTARIO Y PRODUCTOS (CATÁLOGO)
    // ═══════════════════════════════════════════════════════════════════════════
    
    /// <summary>Ver productos y precios (público)</summary>
    public const string CatalogRead = "catalog.read";
    
    /// <summary>Crear, editar y eliminar productos</summary>
    public const string CatalogManage = "catalog.manage";
    
    /// <summary>Ver cantidades de stock</summary>
    public const string InventoryStockRead = "inventory.stock.read";
    
    /// <summary>Ajustar cantidades de stock manualmente</summary>
    public const string InventoryStockUpdate = "inventory.stock.update";
    
    /// <summary>Cambiar precios y activar ofertas</summary>
    public const string InventoryPricesEdit = "inventory.prices.edit";
    
    /// <summary>Gestionar proveedores</summary>
    public const string InventorySuppliersManage = "inventory.suppliers.manage";
    
    /// <summary>Gestionar categorías de productos</summary>
    public const string InventoryCategoriesManage = "inventory.categories.manage";
    
    // ═══════════════════════════════════════════════════════════════════════════
    // MÓDULO: VENTAS Y PEDIDOS
    // ═══════════════════════════════════════════════════════════════════════════
    
    /// <summary>Crear pedidos/ventas (Cliente/Cajero)</summary>
    public const string OrdersCreate = "orders.create";
    
    /// <summary>Ver todos los pedidos de la tienda (Admin/Supervisor)</summary>
    public const string OrdersReadAll = "orders.read.all";
    
    /// <summary>Ver solo mis pedidos (Cliente)</summary>
    public const string OrdersReadOwn = "orders.read.own";
    
    /// <summary>Cambiar estado de pedidos (Pendiente → Enviado)</summary>
    public const string OrdersStatusUpdate = "orders.status.update";
    
    /// <summary>Procesar devoluciones de dinero</summary>
    public const string OrdersRefund = "orders.refund";
    
    /// <summary>Aplicar descuentos autorizados</summary>
    public const string OrdersDiscountApply = "orders.discount.apply";
    
    /// <summary>Anular facturas/pedidos</summary>
    public const string OrdersCancel = "orders.cancel";
    
    // ═══════════════════════════════════════════════════════════════════════════
    // MÓDULO: PUNTO DE VENTA (POS)
    // ═══════════════════════════════════════════════════════════════════════════
    
    /// <summary>Procesar pagos en caja</summary>
    public const string PosPaymentProcess = "pos.payment.process";
    
    /// <summary>Realizar cierre/arqueo de caja</summary>
    public const string PosCashierClose = "pos.cashier.close";
    
    /// <summary>Imprimir recibos y facturas</summary>
    public const string PosReceiptPrint = "pos.receipt.print";
    
    /// <summary>Ver corte de caja de todos los cajeros</summary>
    public const string PosCashierReadAll = "pos.cashier.read.all";
    
    // ═══════════════════════════════════════════════════════════════════════════
    // MÓDULO: DELIVERY / RECOLECCIÓN
    // ═══════════════════════════════════════════════════════════════════════════
    
    /// <summary>Ver lista de pedidos pendientes de recolección</summary>
    public const string PickerOrdersRead = "picker.orders.read";
    
    /// <summary>Marcar productos como recolectados</summary>
    public const string PickerOrdersUpdate = "picker.orders.update";
    
    /// <summary>Ver dirección de entrega</summary>
    public const string DeliveryAddressRead = "delivery.address.read";
    
    /// <summary>Actualizar estado de entrega (en camino, entregado)</summary>
    public const string DeliveryStatusUpdate = "delivery.status.update";
    
    // ═══════════════════════════════════════════════════════════════════════════
    // MÓDULO: AUDITORÍA Y REPORTES
    // ═══════════════════════════════════════════════════════════════════════════
    
    /// <summary>Ver gráficas de ingresos diarios/mensuales</summary>
    public const string ReportsSales = "reports.sales";
    
    /// <summary>Ver reporte de productos próximos a vencer o stock bajo</summary>
    public const string ReportsInventory = "reports.inventory";
    
    /// <summary>Ver logs de auditoría del sistema</summary>
    public const string AuditLogsView = "audit.logs.view";

    // ═══════════════════════════════════════════════════════════════════════════
    // MÓDULO: KARDEX
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>Ver cuentas y movimientos de crédito</summary>
    public const string KardexRead = "kardex.read";

    /// <summary>Gestionar cupos y créditos</summary>
    public const string KardexManage = "kardex.manage";
    
    // ═══════════════════════════════════════════════════════════════════════════
    // MÓDULO: CONFIGURACIÓN DEL SISTEMA
    // ═══════════════════════════════════════════════════════════════════════════
    
    /// <summary>Gestionar configuración de la tienda/sucursal</summary>
    public const string SettingsStore = "settings.store";
    
    /// <summary>Gestionar configuración del sistema</summary>
    public const string SettingsSystem = "settings.system";
    
    /// <summary>Acceso a base de datos y backups</summary>
    public const string SettingsDatabase = "settings.database";
    
    /// <summary>
    /// Obtiene todos los permisos disponibles en el sistema
    /// </summary>
    public static IReadOnlyList<PermissionInfo> GetAll() => new List<PermissionInfo>
    {
        // Usuarios
        new(UsersCreate, "Crear Usuarios", "Permite crear nuevos empleados y usuarios", "users"),
        new(UsersRead, "Ver Usuarios", "Permite ver la lista de usuarios y sus roles", "users"),
        new(UsersUpdate, "Editar Usuarios", "Permite editar información de perfil y contraseñas", "users"),
        new(UsersDelete, "Desactivar Usuarios", "Permite desactivar cuentas de usuario", "users"),
        new(UsersRolesManage, "Gestionar Roles de Usuario", "Permite asignar y remover roles de usuarios", "users"),
        
        // Roles y Permisos
        new(RolesRead, "Ver Roles", "Permite ver los roles y permisos del sistema", "roles"),
        new(RolesManage, "Gestionar Roles", "Permite crear y modificar roles", "roles"),
        new(PermissionsManage, "Gestionar Permisos", "Permite asignar permisos a roles", "permissions"),
        
        // Catálogo
        new(CatalogRead, "Ver Catálogo", "Permite ver productos y precios", "catalog"),
        new(CatalogManage, "Gestionar Catálogo", "Permite crear, editar y eliminar productos", "catalog"),
        
        // Inventario
        new(InventoryStockRead, "Ver Stock", "Permite ver cantidades de inventario", "inventory"),
        new(InventoryStockUpdate, "Actualizar Stock", "Permite ajustar cantidades de stock", "inventory"),
        new(InventoryPricesEdit, "Editar Precios", "Permite cambiar precios y activar ofertas", "inventory"),
        new(InventorySuppliersManage, "Gestionar Proveedores", "Permite administrar proveedores", "inventory"),
        new(InventoryCategoriesManage, "Gestionar Categorías", "Permite administrar categorías de productos", "inventory"),
        
        // Pedidos
        new(OrdersCreate, "Crear Pedidos", "Permite crear pedidos y ventas", "orders"),
        new(OrdersReadAll, "Ver Todos los Pedidos", "Permite ver todos los pedidos de la tienda", "orders"),
        new(OrdersReadOwn, "Ver Mis Pedidos", "Permite ver los pedidos propios", "orders"),
        new(OrdersStatusUpdate, "Actualizar Estado de Pedidos", "Permite cambiar el estado de pedidos", "orders"),
        new(OrdersRefund, "Procesar Devoluciones", "Permite procesar devoluciones de dinero", "orders"),
        new(OrdersDiscountApply, "Aplicar Descuentos", "Permite aplicar descuentos autorizados", "orders"),
        new(OrdersCancel, "Anular Pedidos", "Permite anular facturas y pedidos", "orders"),
        
        // POS
        new(PosPaymentProcess, "Procesar Pagos", "Permite procesar pagos en punto de venta", "pos"),
        new(PosCashierClose, "Cierre de Caja", "Permite realizar cierre y arqueo de caja", "pos"),
        new(PosReceiptPrint, "Imprimir Recibos", "Permite imprimir recibos y facturas", "pos"),
        new(PosCashierReadAll, "Ver Todas las Cajas", "Permite ver cortes de caja de todos los cajeros", "pos"),
        
        // Picker / Delivery
        new(PickerOrdersRead, "Ver Pedidos para Recolectar", "Permite ver pedidos pendientes de recolección", "picker"),
        new(PickerOrdersUpdate, "Actualizar Recolección", "Permite marcar productos como recolectados", "picker"),
        new(DeliveryAddressRead, "Ver Direcciones de Entrega", "Permite ver direcciones de entrega", "delivery"),
        new(DeliveryStatusUpdate, "Actualizar Estado de Entrega", "Permite actualizar estado de entregas", "delivery"),
        
        // Reportes
        new(ReportsSales, "Reportes de Ventas", "Permite ver gráficas de ingresos", "reports"),
        new(ReportsInventory, "Reportes de Inventario", "Permite ver reportes de stock y vencimientos", "reports"),
        new(AuditLogsView, "Ver Logs de Auditoría", "Permite ver registros de auditoría del sistema", "audit"),

        // Kardex
        new(KardexRead, "Ver Kardex", "Permite ver clientes, cupos y movimientos de crédito", "kardex"),
        new(KardexManage, "Gestionar Kardex", "Permite gestionar cupos y registrar créditos", "kardex"),
        
        // Configuración
        new(SettingsStore, "Configuración de Tienda", "Permite gestionar configuración de sucursal", "settings"),
        new(SettingsSystem, "Configuración del Sistema", "Permite gestionar configuración general", "settings"),
        new(SettingsDatabase, "Gestión de Base de Datos", "Permite acceso a backups y base de datos", "settings"),
    };
}

/// <summary>
/// Información detallada de un permiso
/// </summary>
public record PermissionInfo(string Code, string Name, string Description, string Module);
