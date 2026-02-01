// ═══════════════════════════════════════════════════════════════════════════
// ROLES DEL SISTEMA (sincronizados con backend)
// ═══════════════════════════════════════════════════════════════════════════

export type Role =
  | "superadmin"
  | "store_manager"
  | "inventory_manager"
  | "cashier_supervisor"
  | "cashier"
  | "picker"
  | "delivery_driver"
  | "customer";

export const ROLE_LABELS: Record<Role, string> = {
  superadmin: "Super Administrador",
  store_manager: "Administrador de Tienda",
  inventory_manager: "Gestor de Inventario",
  cashier_supervisor: "Supervisor de Cajas",
  cashier: "Cajero",
  picker: "Recopilador",
  delivery_driver: "Repartidor",
  customer: "Cliente",
};

// ═══════════════════════════════════════════════════════════════════════════
// PERMISOS DEL SISTEMA (sincronizados con backend)
// ═══════════════════════════════════════════════════════════════════════════

export type Permission =
  // Usuarios
  | "users.create"
  | "users.read"
  | "users.update"
  | "users.delete"
  | "users.roles.manage"
  // Roles
  | "roles.read"
  | "roles.manage"
  | "permissions.manage"
  // Catálogo
  | "catalog.read"
  | "catalog.manage"
  // Inventario
  | "inventory.stock.read"
  | "inventory.stock.update"
  | "inventory.prices.edit"
  | "inventory.suppliers.manage"
  | "inventory.categories.manage"
  // Pedidos
  | "orders.create"
  | "orders.read.all"
  | "orders.read.own"
  | "orders.status.update"
  | "orders.refund"
  | "orders.discount.apply"
  | "orders.cancel"
  // POS
  | "pos.payment.process"
  | "pos.cashier.close"
  | "pos.receipt.print"
  | "pos.cashier.read.all"
  // Picker/Delivery
  | "picker.orders.read"
  | "picker.orders.update"
  | "delivery.address.read"
  | "delivery.status.update"
  // Reportes
  | "reports.sales"
  | "reports.inventory"
  | "audit.logs.view"
  // Kardex
  | "kardex.read"
  | "kardex.manage"
  // Configuración
  | "settings.store"
  | "settings.system"
  | "settings.database";

// Agrupación de permisos por módulo de navegación
export const MODULE_PERMISSIONS: Record<string, Permission[]> = {
  catalogo: ["catalog.read", "catalog.manage"],
  inventario: [
    "inventory.stock.read",
    "inventory.stock.update",
    "inventory.prices.edit",
    "inventory.suppliers.manage",
    "inventory.categories.manage",
  ],
  compras: ["inventory.suppliers.manage", "inventory.stock.update"],
  ventas: [
    "orders.create",
    "orders.read.all",
    "orders.read.own",
    "orders.status.update",
    "pos.payment.process",
  ],
  clientes: ["orders.read.all", "orders.read.own"],
  kardex: ["kardex.read", "kardex.manage"],
  usuarios: [
    "users.create",
    "users.read",
    "users.update",
    "users.delete",
    "users.roles.manage",
    "roles.read",
    "roles.manage",
  ],
};

/**
 * Verifica si el usuario tiene al menos un permiso del módulo
 */
export function canAccessModule(
  module: string,
  userPermissions: Permission[]
): boolean {
  const requiredPermissions = MODULE_PERMISSIONS[module];
  if (!requiredPermissions) return false;
  return requiredPermissions.some((p) => userPermissions.includes(p));
}

/**
 * Verifica si el usuario tiene un permiso específico
 */
export function hasPermission(
  permission: Permission,
  userPermissions: Permission[]
): boolean {
  return userPermissions.includes(permission);
}
