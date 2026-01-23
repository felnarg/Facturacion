export type Role =
  | "admin"
  | "catalogo"
  | "inventario"
  | "compras"
  | "ventas"
  | "clientes"
  | "usuarios"
  | "auditor";

export type Permission =
  | "catalogo"
  | "inventario"
  | "compras"
  | "ventas"
  | "clientes"
  | "usuarios";

export const ROLE_PERMISSIONS: Record<Role, Permission[]> = {
  admin: ["catalogo", "inventario", "compras", "ventas", "clientes", "usuarios"],
  catalogo: ["catalogo"],
  inventario: ["inventario", "catalogo"],
  compras: ["compras", "inventario", "catalogo"],
  ventas: ["ventas", "catalogo", "inventario", "clientes"],
  clientes: ["clientes"],
  usuarios: ["usuarios"],
  auditor: ["catalogo", "inventario", "compras", "ventas", "clientes", "usuarios"],
};
