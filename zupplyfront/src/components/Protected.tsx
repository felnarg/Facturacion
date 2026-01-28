"use client";

import { useAuth } from "@/components/AuthProvider";
import type { Permission } from "@/lib/permissions";

type ProtectedProps = {
  /** Permiso específico requerido para ver el contenido */
  permission?: Permission;
  /** Módulo de la aplicación para verificar acceso */
  module?: string;
  /** Contenido a mostrar si tiene permisos */
  children: React.ReactNode;
  /** Contenido alternativo si no tiene permisos (opcional) */
  fallback?: React.ReactNode;
};

/**
 * Componente que protege contenido basado en autenticación y permisos
 */
export function Protected({
  permission,
  module,
  children,
  fallback,
}: ProtectedProps) {
  const { isAuthenticated, hasPermission, canAccessModule } = useAuth();

  if (!isAuthenticated) {
    return (
      fallback ?? (
        <div className="rounded-lg border border-dashed border-zinc-300 p-4 text-sm text-zinc-600">
          Debes iniciar sesión para ver esta sección.
        </div>
      )
    );
  }

  // Verificar permiso específico
  if (permission && !hasPermission(permission)) {
    return (
      fallback ?? (
        <div className="rounded-lg border border-dashed border-amber-300 bg-amber-50 p-4 text-sm text-amber-800">
          No tienes permisos para acceder a esta funcionalidad.
        </div>
      )
    );
  }

  // Verificar acceso al módulo
  if (module && !canAccessModule(module)) {
    return (
      fallback ?? (
        <div className="rounded-lg border border-dashed border-amber-300 bg-amber-50 p-4 text-sm text-amber-800">
          No tienes permisos para acceder a este módulo.
        </div>
      )
    );
  }

  return <>{children}</>;
}
