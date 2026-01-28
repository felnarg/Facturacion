"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useAuth } from "@/components/AuthProvider";
import { ROLE_LABELS, type Role } from "@/lib/permissions";

type NavItem = {
  label: string;
  href: string;
  /** Módulo para verificar acceso (usa permisos del backend) */
  module?: string;
};

const NAV_ITEMS: NavItem[] = [
  { label: "Dashboard", href: "/" },
  { label: "Catálogo", href: "/catalogo", module: "catalogo" },
  { label: "Inventario", href: "/inventario", module: "inventario" },
  { label: "Compras", href: "/compras", module: "compras" },
  { label: "Proveedores", href: "/proveedores", module: "compras" },
  { label: "Ventas", href: "/ventas", module: "ventas" },
  { label: "Clientes", href: "/clientes", module: "clientes" },
  { label: "Usuarios", href: "/usuarios", module: "usuarios" },
];

type NavProps = {
  onNavigate?: () => void;
};

export function Nav({ onNavigate }: NavProps) {
  const pathname = usePathname();
  const { isAuthenticated, logout, user, canAccessModule } = useAuth();

  const visibleItems = NAV_ITEMS.filter((item) =>
    item.module ? canAccessModule(item.module) : true
  );

  return (
    <aside className="flex h-full w-full flex-col gap-6 border-r border-zinc-200 bg-white p-6">
      <div>
        <h1 className="text-lg font-semibold text-zinc-900">Zupply</h1>
        <p className="text-xs text-zinc-500">Panel de microservicios</p>
      </div>

      <nav className="flex flex-col gap-1 text-sm">
        {visibleItems.map((item) => {
          const active = pathname === item.href;
          return (
            <Link
              key={item.href}
              href={item.href}
              onClick={onNavigate}
              className={`rounded-md px-3 py-2 ${active
                  ? "bg-zinc-900 text-white"
                  : "text-zinc-700 hover:bg-zinc-100"
                }`}
            >
              {item.label}
            </Link>
          );
        })}
      </nav>

      <div className="mt-auto space-y-3">
        {isAuthenticated && user.roles.length > 0 && (
          <div className="rounded-lg border border-zinc-200 bg-zinc-50 p-3">
            <p className="text-[10px] font-medium uppercase text-zinc-400">
              Roles activos
            </p>
            <div className="mt-1 flex flex-wrap gap-1">
              {user.roles.map((role) => (
                <span
                  key={role}
                  className="rounded-full bg-emerald-100 px-2 py-0.5 text-[10px] text-emerald-700"
                >
                  {ROLE_LABELS[role as Role] ?? role}
                </span>
              ))}
            </div>
          </div>
        )}

        <div className="rounded-lg border border-zinc-200 p-3 text-xs text-zinc-600">
          {isAuthenticated ? (
            <div className="flex flex-col gap-2">
              <div>
                <p className="font-medium text-zinc-900">{user.name}</p>
                <p className="text-zinc-500">{user.email}</p>
              </div>
              <button
                onClick={() => {
                  logout();
                  onNavigate?.();
                }}
                className="rounded-md border border-zinc-300 px-2 py-1 text-zinc-700 hover:bg-zinc-100"
              >
                Cerrar sesión
              </button>
            </div>
          ) : (
            <Link
              href="/login"
              onClick={onNavigate}
              className="inline-flex items-center justify-center rounded-md border border-zinc-300 px-2 py-1 hover:bg-zinc-100"
            >
              Iniciar sesión
            </Link>
          )}
        </div>
      </div>
    </aside>
  );
}
