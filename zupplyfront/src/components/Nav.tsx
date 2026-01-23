"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useAuth } from "@/components/AuthProvider";
import type { Permission } from "@/lib/permissions";

type NavItem = {
  label: string;
  href: string;
  permission?: Permission;
};

const NAV_ITEMS: NavItem[] = [
  { label: "Dashboard", href: "/" },
  { label: "Catálogo", href: "/catalogo", permission: "catalogo" },
  { label: "Inventario", href: "/inventario", permission: "inventario" },
  { label: "Compras", href: "/compras", permission: "compras" },
  { label: "Proveedores", href: "/proveedores", permission: "compras" },
  { label: "Ventas", href: "/ventas", permission: "ventas" },
  { label: "Clientes", href: "/clientes", permission: "clientes" },
  { label: "Usuarios", href: "/usuarios", permission: "usuarios" },
];

type NavProps = {
  onNavigate?: () => void;
};

export function Nav({ onNavigate }: NavProps) {
  const pathname = usePathname();
  const { permissions, isAuthenticated, logout, user } = useAuth();

  return (
    <aside className="flex h-full w-full flex-col gap-6 border-r border-zinc-200 bg-white p-6">
      <div>
        <h1 className="text-lg font-semibold text-zinc-900">Zupply</h1>
        <p className="text-xs text-zinc-500">Panel de microservicios</p>
      </div>

      <nav className="flex flex-col gap-1 text-sm">
        {NAV_ITEMS.filter((item) =>
          item.permission ? permissions.includes(item.permission) : true
        ).map((item) => {
          const active = pathname === item.href;
          return (
            <Link
              key={item.href}
              href={item.href}
              onClick={onNavigate}
              className={`rounded-md px-3 py-2 ${
                active
                  ? "bg-zinc-900 text-white"
                  : "text-zinc-700 hover:bg-zinc-100"
              }`}
            >
              {item.label}
            </Link>
          );
        })}
      </nav>

      <div className="mt-auto rounded-lg border border-zinc-200 p-3 text-xs text-zinc-600">
        {isAuthenticated ? (
          <div className="flex flex-col gap-2">
            <span>
              Sesión: <strong>{user.email ?? "usuario"}</strong>
            </span>
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
    </aside>
  );
}
