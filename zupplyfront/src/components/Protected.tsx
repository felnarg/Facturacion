"use client";

import { useAuth } from "@/components/AuthProvider";
import type { Permission } from "@/lib/permissions";

type ProtectedProps = {
  permission?: Permission;
  children: React.ReactNode;
};

export function Protected({ permission, children }: ProtectedProps) {
  const { isAuthenticated, permissions } = useAuth();

  if (!isAuthenticated) {
    return (
      <div className="rounded-lg border border-dashed border-zinc-300 p-4 text-sm text-zinc-600">
        Debes iniciar sesión para ver esta sección.
      </div>
    );
  }

  if (permission && !permissions.includes(permission)) {
    return (
      <div className="rounded-lg border border-dashed border-amber-300 bg-amber-50 p-4 text-sm text-amber-800">
        No tienes permisos para acceder a esta funcionalidad.
      </div>
    );
  }

  return <>{children}</>;
}
