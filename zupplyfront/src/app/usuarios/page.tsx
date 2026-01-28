"use client";

import { useEffect, useState } from "react";
import { apiRequest } from "@/lib/api";
import { useAuth } from "@/components/AuthProvider";
import { Protected } from "@/components/Protected";
import { DevBlockHeader } from "@/components/DevBlockHeader";
import { ROLE_LABELS, type Role } from "@/lib/permissions";

// ═══════════════════════════════════════════════════════════════════════════
// TIPOS
// ═══════════════════════════════════════════════════════════════════════════

type User = {
  id: string;
  name: string;
  email: string;
  phoneNumber?: string;
  isActive: boolean;
  lastLoginAt?: string;
  roles: string[];
  createdAt: string;
  updatedAt: string;
};

type RoleDto = {
  id: string;
  code: string;
  name: string;
  description: string;
  hierarchyLevel: number;
  isActive: boolean;
  isSystemRole: boolean;
};

export default function UsuariosPage() {
  const { user } = useAuth();
  const [users, setUsers] = useState<User[]>([]);
  const [roles, setRoles] = useState<RoleDto[]>([]);
  const [form, setForm] = useState({ name: "", email: "", password: "" });
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(true);
  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const [selectedRoles, setSelectedRoles] = useState<string[]>([]);
  const [showRoleModal, setShowRoleModal] = useState(false);

  const loadUsers = async () => {
    setLoading(true);
    try {
      const data = await apiRequest<User[]>(
        "/api/users",
        undefined,
        user.token
      );
      setUsers(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error cargando usuarios");
    } finally {
      setLoading(false);
    }
  };

  const loadRoles = async () => {
    try {
      const data = await apiRequest<RoleDto[]>(
        "/api/roles/active",
        undefined,
        user.token
      );
      setRoles(data);
    } catch {
      // Los roles pueden no estar disponibles si no tiene permisos
    }
  };

  useEffect(() => {
    loadUsers();
    loadRoles();
  }, []);

  const handleCreate = async (event: React.FormEvent) => {
    event.preventDefault();
    setError("");

    try {
      await apiRequest<User>(
        "/api/users",
        {
          method: "POST",
          body: JSON.stringify(form),
        },
        user.token
      );
      setForm({ name: "", email: "", password: "" });
      await loadUsers();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error creando usuario");
    }
  };

  const handleToggleActive = async (targetUser: User) => {
    try {
      const endpoint = targetUser.isActive
        ? `/api/users/${targetUser.id}/deactivate`
        : `/api/users/${targetUser.id}/activate`;

      await apiRequest(
        endpoint,
        { method: "POST" },
        user.token
      );
      await loadUsers();
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Error actualizando usuario"
      );
    }
  };

  const openRoleModal = (targetUser: User) => {
    setSelectedUser(targetUser);
    setSelectedRoles(targetUser.roles);
    setShowRoleModal(true);
  };

  const handleAssignRoles = async () => {
    if (!selectedUser) return;

    try {
      await apiRequest(
        `/api/users/${selectedUser.id}/roles`,
        {
          method: "POST",
          body: JSON.stringify({ roleCodes: selectedRoles }),
        },
        user.token
      );
      setShowRoleModal(false);
      setSelectedUser(null);
      await loadUsers();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error asignando roles");
    }
  };

  const toggleRole = (roleCode: string) => {
    setSelectedRoles((prev) =>
      prev.includes(roleCode)
        ? prev.filter((r) => r !== roleCode)
        : [...prev, roleCode]
    );
  };

  return (
    <Protected permission="users.read">
      <div className="space-y-6">
        <header>
          <h2 className="text-xl font-semibold text-zinc-900">Usuarios</h2>
          <p className="text-sm text-zinc-500">
            Gestiona usuarios del sistema con control de roles y permisos.
          </p>
        </header>

        {/* Formulario de creación */}
        <Protected permission="users.create">
          <form
            onSubmit={handleCreate}
            className="dev-block-container grid gap-3 rounded-xl bg-white p-4 shadow-sm md:grid-cols-4"
          >
            <DevBlockHeader label="bruma" />
            <input
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
              placeholder="Nombre"
              value={form.name}
              onChange={(event) =>
                setForm((prev) => ({ ...prev, name: event.target.value }))
              }
              required
            />
            <input
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
              placeholder="Correo"
              type="email"
              value={form.email}
              onChange={(event) =>
                setForm((prev) => ({ ...prev, email: event.target.value }))
              }
              required
            />
            <input
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
              placeholder="Contraseña"
              type="password"
              value={form.password}
              onChange={(event) =>
                setForm((prev) => ({ ...prev, password: event.target.value }))
              }
              required
            />
            <div>
              <button className="btn-primary">Crear usuario</button>
            </div>
          </form>
        </Protected>

        {error && (
          <div className="rounded-md border border-rose-200 bg-rose-50 px-4 py-3 text-sm text-rose-700">
            {error}
            <button
              onClick={() => setError("")}
              className="ml-2 text-rose-500 hover:text-rose-700"
            >
              ✕
            </button>
          </div>
        )}

        {/* Tabla de usuarios */}
        <div className="dev-block-container overflow-hidden rounded-xl border border-zinc-200 bg-white">
          <DevBlockHeader label="sello" />
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead className="bg-zinc-50 text-left text-xs uppercase text-zinc-500">
                <tr>
                  <th className="px-4 py-3">Usuario</th>
                  <th className="px-4 py-3">Roles</th>
                  <th className="px-4 py-3">Estado</th>
                  <th className="px-4 py-3">Último acceso</th>
                  <th className="px-4 py-3">Acciones</th>
                </tr>
              </thead>
              <tbody>
                {loading ? (
                  <tr>
                    <td
                      colSpan={5}
                      className="px-4 py-6 text-center text-sm text-zinc-500"
                    >
                      Cargando usuarios...
                    </td>
                  </tr>
                ) : users.length === 0 ? (
                  <tr>
                    <td
                      colSpan={5}
                      className="px-4 py-6 text-center text-sm text-zinc-500"
                    >
                      No hay usuarios registrados.
                    </td>
                  </tr>
                ) : (
                  users.map((userRow) => (
                    <tr key={userRow.id} className="border-t border-zinc-100">
                      <td className="px-4 py-3">
                        <div>
                          <p className="font-medium text-zinc-900">
                            {userRow.name}
                          </p>
                          <p className="text-xs text-zinc-500">
                            {userRow.email}
                          </p>
                        </div>
                      </td>
                      <td className="px-4 py-3">
                        <div className="flex flex-wrap gap-1">
                          {!userRow.roles || userRow.roles.length === 0 ? (
                            <span className="text-xs text-zinc-400">
                              Sin roles
                            </span>
                          ) : (
                            userRow.roles.map((role) => (
                              <span
                                key={role}
                                className="rounded-full bg-emerald-100 px-2 py-0.5 text-xs text-emerald-700"
                              >
                                {ROLE_LABELS[role as Role] ?? role}
                              </span>
                            ))
                          )}
                        </div>
                      </td>
                      <td className="px-4 py-3">
                        <span
                          className={`rounded-full px-2 py-0.5 text-xs ${userRow.isActive
                            ? "bg-emerald-100 text-emerald-700"
                            : "bg-zinc-100 text-zinc-500"
                            }`}
                        >
                          {userRow.isActive ? "Activo" : "Inactivo"}
                        </span>
                      </td>
                      <td className="px-4 py-3 text-xs text-zinc-500">
                        {userRow.lastLoginAt
                          ? new Date(userRow.lastLoginAt).toLocaleDateString(
                            "es",
                            {
                              day: "2-digit",
                              month: "short",
                              hour: "2-digit",
                              minute: "2-digit",
                            }
                          )
                          : "Nunca"}
                      </td>
                      <td className="px-4 py-3">
                        <div className="flex gap-2">
                          <Protected permission="users.roles.manage" fallback={null}>
                            <button
                              onClick={() => openRoleModal(userRow)}
                              className="text-xs text-blue-600 hover:underline"
                            >
                              Roles
                            </button>
                          </Protected>
                          <Protected permission="users.update" fallback={null}>
                            <button
                              onClick={() => handleToggleActive(userRow)}
                              className={`text-xs ${userRow.isActive
                                ? "text-amber-600"
                                : "text-emerald-600"
                                } hover:underline`}
                            >
                              {userRow.isActive ? "Desactivar" : "Activar"}
                            </button>
                          </Protected>
                        </div>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>

        {/* Modal de asignación de roles */}
        {showRoleModal && selectedUser && (
          <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
            <div className="w-full max-w-md rounded-xl bg-white p-6 shadow-xl">
              <h3 className="text-lg font-semibold text-zinc-900">
                Asignar roles a {selectedUser.name}
              </h3>
              <p className="mt-1 text-sm text-zinc-500">
                Selecciona los roles que deseas asignar a este usuario.
              </p>

              <div className="mt-4 space-y-2">
                {roles.map((role) => (
                  <label
                    key={role.code}
                    className="flex cursor-pointer items-center gap-3 rounded-lg border border-zinc-200 p-3 hover:bg-zinc-50"
                  >
                    <input
                      type="checkbox"
                      checked={selectedRoles.includes(role.code)}
                      onChange={() => toggleRole(role.code)}
                      className="h-4 w-4 rounded border-zinc-300"
                    />
                    <div>
                      <p className="text-sm font-medium text-zinc-900">
                        {role.name}
                      </p>
                      <p className="text-xs text-zinc-500">{role.description}</p>
                    </div>
                    {role.isSystemRole && (
                      <span className="ml-auto rounded bg-zinc-100 px-2 py-0.5 text-[10px] text-zinc-500">
                        Sistema
                      </span>
                    )}
                  </label>
                ))}
              </div>

              <div className="mt-6 flex justify-end gap-2">
                <button
                  onClick={() => setShowRoleModal(false)}
                  className="rounded-lg border border-zinc-200 px-4 py-2 text-sm text-zinc-700 hover:bg-zinc-50"
                >
                  Cancelar
                </button>
                <button onClick={handleAssignRoles} className="btn-primary">
                  Guardar
                </button>
              </div>
            </div>
          </div>
        )}
      </div>
    </Protected>
  );
}
