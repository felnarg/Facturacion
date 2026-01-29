"use client";

import { useEffect, useState, useRef } from "react";
import { apiRequest } from "@/lib/api";
import { useAuth } from "@/components/AuthProvider";
import { Protected } from "@/components/Protected";
import { DevBlockHeader } from "@/components/DevBlockHeader";
import { ROLE_LABELS, type Role, type Permission } from "@/lib/permissions";
import { useModalKeyboard } from "@/hooks/useModalKeyboard";

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

type PermissionDto = {
  id: string;
  code: string;
  name: string;
  description: string;
  module: string;
};

type RoleDto = {
  id: string;
  code: string;
  name: string;
  description: string;
  hierarchyLevel: number;
  isActive: boolean;
  isSystemRole: boolean;
  permissions: PermissionDto[];
};

type PermissionsByModule = {
  module: string;
  permissions: PermissionDto[];
};

type Tab = "usuarios" | "roles" | "permisos";

// ═══════════════════════════════════════════════════════════════════════════
// COMPONENTE PRINCIPAL
// ═══════════════════════════════════════════════════════════════════════════

export default function UsuariosPage() {
  const { user, hasPermission } = useAuth();
  const [activeTab, setActiveTab] = useState<Tab>("usuarios");

  // Determinar qué tabs mostrar según permisos
  const tabs: { id: Tab; label: string; permission?: Permission }[] = [
    { id: "usuarios", label: "Usuarios" },
    { id: "roles", label: "Roles", permission: "roles.read" },
    { id: "permisos", label: "Permisos", permission: "roles.read" },
  ];

  const visibleTabs = tabs.filter(
    (tab) => !tab.permission || hasPermission(tab.permission)
  );

  return (
    <Protected permission="users.read">
      <div className="space-y-6">
        <header>
          <h2 className="text-xl font-semibold text-zinc-900">
            Gestión de Usuarios
          </h2>
          <p className="text-sm text-zinc-500">
            Administra usuarios, roles y permisos del sistema.
          </p>
        </header>

        {/* Tabs de navegación */}
        <div className="flex gap-1 rounded-lg bg-zinc-100 p-1">
          {visibleTabs.map((tab) => (
            <button
              key={tab.id}
              onClick={() => setActiveTab(tab.id)}
              className={`flex-1 rounded-md px-4 py-2 text-sm font-medium transition-all ${activeTab === tab.id
                ? "bg-white text-zinc-900 shadow-sm"
                : "text-zinc-600 hover:text-zinc-900"
                }`}
            >
              {tab.label}
            </button>
          ))}
        </div>

        {/* Contenido de cada tab */}
        {activeTab === "usuarios" && <UsersTabContent />}
        {activeTab === "roles" && <RolesTabContent />}
        {activeTab === "permisos" && <PermissionsTabContent />}
      </div>
    </Protected>
  );
}

// ═══════════════════════════════════════════════════════════════════════════
// TAB: USUARIOS
// ═══════════════════════════════════════════════════════════════════════════

function UsersTabContent() {
  const { user, hasPermission } = useAuth();
  const [users, setUsers] = useState<User[]>([]);
  const [roles, setRoles] = useState<RoleDto[]>([]);
  const [form, setForm] = useState({ name: "", email: "", password: "" });
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(true);
  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const [selectedRoles, setSelectedRoles] = useState<string[]>([]);
  const [showRoleModal, setShowRoleModal] = useState(false);
  const [showDeleteUserModal, setShowDeleteUserModal] = useState(false);

  // Ref for focus restoration
  const roleButtonRef = useRef<HTMLButtonElement>(null);

  // Keyboard handling for role modal
  useModalKeyboard({
    isOpen: showRoleModal,
    onClose: () => {
      setShowRoleModal(false);
      setSelectedUser(null);
    },
    returnFocusRef: roleButtonRef,
  });

  useModalKeyboard({
    isOpen: showDeleteUserModal,
    onClose: () => {
      setShowDeleteUserModal(false);
      setSelectedUser(null);
    },
  });

  const loadUsers = async () => {
    setLoading(true);
    try {
      const data = await apiRequest<User[]>("/api/users", undefined, user.token);
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

      await apiRequest(endpoint, { method: "POST" }, user.token);
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

  const openDeleteUserModal = (targetUser: User) => {
    setSelectedUser(targetUser);
    setShowDeleteUserModal(true);
  };

  const handleDeleteUser = async () => {
    if (!selectedUser) return;
    try {
      await apiRequest(
        `/api/users/${selectedUser.id}`,
        { method: "DELETE" },
        user.token
      );
      setShowDeleteUserModal(false);
      setSelectedUser(null);
      await loadUsers();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error eliminando usuario");
    }
  };

  return (
    <>
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
                        <p className="text-xs text-zinc-500">{userRow.email}</p>
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
                        <Protected permission="users.delete" fallback={null}>
                          <span className="text-zinc-300">|</span>
                          <button
                            onClick={() => openDeleteUserModal(userRow)}
                            className="text-xs text-rose-600 hover:text-rose-800 hover:underline"
                          >
                            Eliminar
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

            <div className="mt-4 max-h-64 space-y-2 overflow-y-auto">
              {roles.length === 0 ? (
                <p className="py-4 text-center text-sm text-zinc-500">
                  No hay roles disponibles o no tienes permiso para verlos.
                </p>
              ) : (
                roles.map((role) => (
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
                    <div className="flex-1">
                      <p className="text-sm font-medium text-zinc-900">
                        {role.name}
                      </p>
                      <p className="text-xs text-zinc-500">{role.description}</p>
                    </div>
                    {role.isSystemRole && (
                      <span className="rounded bg-zinc-100 px-2 py-0.5 text-[10px] text-zinc-500">
                        Sistema
                      </span>
                    )}
                  </label>
                ))
              )}
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
      {/* Modal Eliminar Usuario */}
      {showDeleteUserModal && selectedUser && (
        <div className="modal-backdrop">
          <div className="modal-content max-w-md">
            <h3 className="text-lg font-semibold text-zinc-900">
              Confirmar Eliminación
            </h3>
            <p className="mt-2 text-sm text-zinc-600">
              ¿Estás seguro que deseas eliminar al usuario <strong>{selectedUser.name}</strong>?
            </p>
            <p className="mt-1 text-xs text-zinc-500">
              Esta acción eliminará permanentemente al usuario y su acceso al sistema.
            </p>

            <div className="mt-6 flex justify-end gap-3">
              <button
                onClick={() => {
                  setShowDeleteUserModal(false);
                  setSelectedUser(null);
                }}
                className="rounded-lg border border-zinc-200 px-4 py-2 text-sm text-zinc-700 hover:bg-zinc-50"
              >
                Cancelar
              </button>
              <button
                onClick={handleDeleteUser}
                className="rounded-lg bg-rose-600 px-4 py-2 text-sm font-medium text-white hover:bg-rose-700"
              >
                Eliminar Usuario
              </button>
            </div>
          </div>
        </div>
      )}
    </>
  );
}

// ═══════════════════════════════════════════════════════════════════════════
// TAB: ROLES
// ═══════════════════════════════════════════════════════════════════════════

function RolesTabContent() {
  const { user, hasPermission } = useAuth();
  const [roles, setRoles] = useState<RoleDto[]>([]);
  const [allPermissions, setAllPermissions] = useState<PermissionsByModule[]>(
    []
  );
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // Estados para modales
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [showPermissionsModal, setShowPermissionsModal] = useState(false);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [selectedRole, setSelectedRole] = useState<RoleDto | null>(null);

  // Formulario de creación/edición
  const [formData, setFormData] = useState({
    code: "",
    name: "",
    description: "",
    hierarchyLevel: 50,
    permissionCodes: [] as string[],
  });

  // Hooks para cerrar modales con Escape
  useModalKeyboard({
    isOpen: showCreateModal,
    onClose: () => setShowCreateModal(false),
  });
  useModalKeyboard({
    isOpen: showEditModal,
    onClose: () => {
      setShowEditModal(false);
      setSelectedRole(null);
    },
  });
  useModalKeyboard({
    isOpen: showPermissionsModal,
    onClose: () => {
      setShowPermissionsModal(false);
      setSelectedRole(null);
    },
  });
  useModalKeyboard({
    isOpen: showDeleteModal,
    onClose: () => {
      setShowDeleteModal(false);
      setSelectedRole(null);
    },
  });

  const canManageRoles = hasPermission("roles.manage");
  const canManagePermissions = hasPermission("permissions.manage");

  const loadRoles = async () => {
    setLoading(true);
    try {
      const data = await apiRequest<RoleDto[]>(
        "/api/roles",
        undefined,
        user.token
      );
      setRoles(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error cargando roles");
    } finally {
      setLoading(false);
    }
  };

  const loadPermissions = async () => {
    try {
      const data = await apiRequest<PermissionsByModule[]>(
        "/api/permissions/grouped",
        undefined,
        user.token
      );
      setAllPermissions(data);
    } catch {
      // Silenciar error si no tiene permisos
    }
  };

  useEffect(() => {
    loadRoles();
    loadPermissions();
  }, []);

  const handleCreateRole = async () => {
    try {
      await apiRequest(
        "/api/roles",
        {
          method: "POST",
          body: JSON.stringify(formData),
        },
        user.token
      );
      setShowCreateModal(false);
      resetForm();
      await loadRoles();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error creando rol");
    }
  };

  const handleUpdateRole = async () => {
    if (!selectedRole) return;

    try {
      await apiRequest(
        `/api/roles/${selectedRole.id}`,
        {
          method: "PUT",
          body: JSON.stringify({
            name: formData.name,
            description: formData.description,
            hierarchyLevel: formData.hierarchyLevel,
          }),
        },
        user.token
      );
      setShowEditModal(false);
      setSelectedRole(null);
      resetForm();
      await loadRoles();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error actualizando rol");
    }
  };

  const handleToggleActive = async (role: RoleDto) => {
    try {
      const endpoint = role.isActive
        ? `/api/roles/${role.id}/deactivate`
        : `/api/roles/${role.id}/activate`;

      await apiRequest(endpoint, { method: "POST" }, user.token);
      await loadRoles();
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Error cambiando estado del rol"
      );
    }
  };

  const handleAssignPermissions = async () => {
    if (!selectedRole) return;

    try {
      await apiRequest(
        `/api/roles/${selectedRole.id}/permissions`,
        {
          method: "POST",
          body: JSON.stringify({ permissionCodes: formData.permissionCodes }),
        },
        user.token
      );
      setShowPermissionsModal(false);
      setSelectedRole(null);
      resetForm();
      await loadRoles();
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Error asignando permisos"
      );
    }
  };

  const openEditModal = (role: RoleDto) => {
    setSelectedRole(role);
    setFormData({
      code: role.code,
      name: role.name,
      description: role.description,
      hierarchyLevel: role.hierarchyLevel,
      permissionCodes: role.permissions.map((p) => p.code),
    });
    setShowEditModal(true);
  };

  const openPermissionsModal = (role: RoleDto) => {
    setSelectedRole(role);
    setFormData((prev) => ({
      ...prev,
      permissionCodes: role.permissions.map((p) => p.code),
    }));
    setShowPermissionsModal(true);
  };

  const resetForm = () => {
    setFormData({
      code: "",
      name: "",
      description: "",
      hierarchyLevel: 50,
      permissionCodes: [],
    });
  };

  const togglePermission = (code: string) => {
    setFormData((prev) => ({
      ...prev,
      permissionCodes: prev.permissionCodes.includes(code)
        ? prev.permissionCodes.filter((c) => c !== code)
        : [...prev.permissionCodes, code],
    }));
  };

  const openDeleteModal = (role: RoleDto) => {
    setSelectedRole(role);
    setShowDeleteModal(true);
  };

  const handleDeleteRole = async () => {
    if (!selectedRole) return;

    try {
      await apiRequest(
        `/api/roles/${selectedRole.id}`,
        { method: "DELETE" },
        user.token
      );
      setShowDeleteModal(false);
      setSelectedRole(null);
      await loadRoles();
    } catch (err) {
      // El error puede venir del backend (reglas de negocio)
      setError(
        err instanceof Error ? err.message : "Error eliminando el rol"
      );
      setShowDeleteModal(false); // Cerramos el modal para mostrar el error en la pantalla principal
    }
  };

  return (
    <Protected permission="roles.read">
      {/* Botón crear rol */}
      {canManageRoles && (
        <div className="flex justify-end">
          <button
            onClick={() => {
              resetForm();
              setShowCreateModal(true);
            }}
            className="btn-primary"
          >
            + Crear Rol
          </button>
        </div>
      )}

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

      {/* Tabla de roles */}
      <div className="overflow-hidden rounded-xl border border-zinc-200 bg-white">
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead className="bg-zinc-50 text-left text-xs uppercase text-zinc-500">
              <tr>
                <th className="px-4 py-3">Rol</th>
                <th className="px-4 py-3">Código</th>
                <th className="px-4 py-3">Nivel</th>
                <th className="px-4 py-3">Permisos</th>
                <th className="px-4 py-3">Estado</th>
                <th className="px-4 py-3">Acciones</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td
                    colSpan={6}
                    className="px-4 py-6 text-center text-sm text-zinc-500"
                  >
                    Cargando roles...
                  </td>
                </tr>
              ) : roles.length === 0 ? (
                <tr>
                  <td
                    colSpan={6}
                    className="px-4 py-6 text-center text-sm text-zinc-500"
                  >
                    No hay roles configurados.
                  </td>
                </tr>
              ) : (
                roles.map((role) => (
                  <tr key={role.id} className="border-t border-zinc-100">
                    <td className="px-4 py-3">
                      <div>
                        <p className="font-medium text-zinc-900">{role.name}</p>
                        <p className="text-xs text-zinc-500">
                          {role.description}
                        </p>
                      </div>
                    </td>
                    <td className="px-4 py-3">
                      <code className="rounded text-zinc-600 bg-zinc-100 px-1.5 py-0.5 text-xs">
                        {role.code}
                      </code>
                    </td>
                    <td className="px-4 py-3 text-center">
                      <span className="rounded-full bg-blue-100 px-2 py-0.5 text-xs text-blue-700">
                        {role.hierarchyLevel}
                      </span>
                    </td>
                    <td className="px-4 py-3">
                      <span className="text-xs text-zinc-600">
                        {role.permissions.length} permisos
                      </span>
                    </td>
                    <td className="px-4 py-3">
                      <div className="flex items-center gap-2">
                        <span
                          className={`rounded-full px-2 py-0.5 text-xs ${role.isActive
                            ? "bg-emerald-100 text-emerald-700"
                            : "bg-zinc-100 text-zinc-500"
                            }`}
                        >
                          {role.isActive ? "Activo" : "Inactivo"}
                        </span>
                        {role.isSystemRole && (
                          <span className="rounded bg-amber-100 px-1.5 py-0.5 text-[10px] text-amber-700">
                            Sistema
                          </span>
                        )}
                      </div>
                    </td>
                    <td className="px-4 py-3">
                      <div className="flex items-center gap-3">
                        {canManageRoles && !role.isSystemRole && (
                          <button
                            onClick={() => openEditModal(role)}
                            className="text-xs text-blue-600 hover:underline"
                          >
                            Editar
                          </button>
                        )}
                        {canManagePermissions && (
                          <button
                            onClick={() => openPermissionsModal(role)}
                            className="text-xs text-purple-600 hover:underline"
                          >
                            Permisos
                          </button>
                        )}

                        {(canManageRoles || canManagePermissions) && (
                          <span className="text-zinc-300">|</span>
                        )}

                        {canManageRoles && (
                          <button
                            onClick={() => handleToggleActive(role)}
                            className={`text-xs ${role.isActive
                              ? "text-amber-600"
                              : "text-emerald-600"
                              } hover:underline`}
                          >
                            {role.isActive ? "Desactivar" : "Activar"}
                          </button>
                        )}

                        {canManageRoles && !role.isSystemRole && (
                          <>
                            <span className="text-zinc-300">|</span>
                            <button
                              onClick={() => openDeleteModal(role)}
                              className="text-xs text-rose-600 hover:text-rose-800 hover:underline"
                            >
                              Eliminar
                            </button>
                          </>
                        )}
                      </div>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* Modal Crear Rol */}
      {showCreateModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
          <div className="w-full max-w-lg rounded-xl bg-white p-6 shadow-xl">
            <h3 className="text-lg font-semibold text-zinc-900">
              Crear Nuevo Rol
            </h3>

            <div className="mt-4 space-y-4">
              <div>
                <label className="block text-sm font-medium text-zinc-700">
                  Código
                </label>
                <input
                  type="text"
                  value={formData.code}
                  onChange={(e) =>
                    setFormData((prev) => ({ ...prev, code: e.target.value }))
                  }
                  className="mt-1 w-full rounded-md border border-zinc-200 px-3 py-2 text-sm"
                  placeholder="ej: vendedor_junior"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-zinc-700">
                  Nombre
                </label>
                <input
                  type="text"
                  value={formData.name}
                  onChange={(e) =>
                    setFormData((prev) => ({ ...prev, name: e.target.value }))
                  }
                  className="mt-1 w-full rounded-md border border-zinc-200 px-3 py-2 text-sm"
                  placeholder="ej: Vendedor Junior"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-zinc-700">
                  Descripción
                </label>
                <textarea
                  value={formData.description}
                  onChange={(e) =>
                    setFormData((prev) => ({
                      ...prev,
                      description: e.target.value,
                    }))
                  }
                  className="mt-1 w-full rounded-md border border-zinc-200 px-3 py-2 text-sm"
                  rows={2}
                  placeholder="Describe las responsabilidades de este rol..."
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-zinc-700">
                  Nivel de Jerarquía
                </label>
                <input
                  type="number"
                  value={formData.hierarchyLevel}
                  onChange={(e) =>
                    setFormData((prev) => ({
                      ...prev,
                      hierarchyLevel: parseInt(e.target.value) || 0,
                    }))
                  }
                  className="mt-1 w-full rounded-md border border-zinc-200 px-3 py-2 text-sm"
                  min={1}
                  max={100}
                />
                <p className="mt-1 text-xs text-zinc-500">
                  Mayor = más privilegios. Ej: SuperAdmin=100, Cliente=10
                </p>
              </div>
            </div>

            <div className="mt-6 flex justify-end gap-2">
              <button
                onClick={() => setShowCreateModal(false)}
                className="rounded-lg border border-zinc-200 px-4 py-2 text-sm text-zinc-700 hover:bg-zinc-50"
              >
                Cancelar
              </button>
              <button onClick={handleCreateRole} className="btn-primary">
                Crear Rol
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Modal Editar Rol */}
      {showEditModal && selectedRole && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
          <div className="w-full max-w-lg rounded-xl bg-white p-6 shadow-xl">
            <h3 className="text-lg font-semibold text-zinc-900">
              Editar Rol: {selectedRole.name}
            </h3>

            <div className="mt-4 space-y-4">
              <div>
                <label className="block text-sm font-medium text-zinc-700">
                  Nombre
                </label>
                <input
                  type="text"
                  value={formData.name}
                  onChange={(e) =>
                    setFormData((prev) => ({ ...prev, name: e.target.value }))
                  }
                  className="mt-1 w-full rounded-md border border-zinc-200 px-3 py-2 text-sm"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-zinc-700">
                  Descripción
                </label>
                <textarea
                  value={formData.description}
                  onChange={(e) =>
                    setFormData((prev) => ({
                      ...prev,
                      description: e.target.value,
                    }))
                  }
                  className="mt-1 w-full rounded-md border border-zinc-200 px-3 py-2 text-sm"
                  rows={2}
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-zinc-700">
                  Nivel de Jerarquía
                </label>
                <input
                  type="number"
                  value={formData.hierarchyLevel}
                  onChange={(e) =>
                    setFormData((prev) => ({
                      ...prev,
                      hierarchyLevel: parseInt(e.target.value) || 0,
                    }))
                  }
                  className="mt-1 w-full rounded-md border border-zinc-200 px-3 py-2 text-sm"
                  min={1}
                  max={100}
                />
              </div>
            </div>

            <div className="mt-6 flex justify-end gap-2">
              <button
                onClick={() => {
                  setShowEditModal(false);
                  setSelectedRole(null);
                }}
                className="rounded-lg border border-zinc-200 px-4 py-2 text-sm text-zinc-700 hover:bg-zinc-50"
              >
                Cancelar
              </button>
              <button onClick={handleUpdateRole} className="btn-primary">
                Guardar Cambios
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Modal Asignar Permisos */}
      {showPermissionsModal && selectedRole && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
          <div className="w-full max-w-2xl rounded-xl bg-white p-6 shadow-xl">
            <h3 className="text-lg font-semibold text-zinc-900">
              Permisos de: {selectedRole.name}
            </h3>
            <p className="mt-1 text-sm text-zinc-500">
              Selecciona los permisos que tendrá este rol.
            </p>

            <div className="mt-4 max-h-[400px] space-y-4 overflow-y-auto">
              {allPermissions.length === 0 ? (
                <p className="py-4 text-center text-sm text-zinc-500">
                  Cargando permisos...
                </p>
              ) : (
                allPermissions.map((group) => (
                  <div key={group.module}>
                    <h4 className="mb-2 text-xs font-semibold uppercase text-zinc-400">
                      {group.module}
                    </h4>
                    <div className="grid gap-2 md:grid-cols-2">
                      {group.permissions.map((perm) => (
                        <label
                          key={perm.code}
                          className="flex cursor-pointer items-center gap-2 rounded-lg border border-zinc-200 p-2 hover:bg-zinc-50"
                        >
                          <input
                            type="checkbox"
                            checked={formData.permissionCodes.includes(
                              perm.code
                            )}
                            onChange={() => togglePermission(perm.code)}
                            className="h-4 w-4 rounded border-zinc-300"
                          />
                          <div className="flex-1 min-w-0">
                            <p className="text-sm font-medium text-zinc-900 truncate">
                              {perm.name}
                            </p>
                            <p className="text-xs text-zinc-500 truncate">
                              {perm.code}
                            </p>
                          </div>
                        </label>
                      ))}
                    </div>
                  </div>
                ))
              )}
            </div>

            <div className="mt-6 flex items-center justify-between">
              <p className="text-sm text-zinc-600">
                {formData.permissionCodes.length} permisos seleccionados
              </p>
              <div className="flex gap-2">
                <button
                  onClick={() => {
                    setShowPermissionsModal(false);
                    setSelectedRole(null);
                  }}
                  className="rounded-lg border border-zinc-200 px-4 py-2 text-sm text-zinc-700 hover:bg-zinc-50"
                >
                  Cancelar
                </button>
                <button
                  onClick={handleAssignPermissions}
                  className="btn-primary"
                >
                  Guardar Permisos
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Modal Eliminar Rol */}
      {showDeleteModal && selectedRole && (
        <div className="modal-backdrop">
          <div className="modal-content max-w-md">
            <h3 className="text-lg font-semibold text-zinc-900">
              Confirmar Eliminación
            </h3>
            <p className="mt-2 text-sm text-zinc-600">
              ¿Estás seguro que deseas eliminar el rol <strong>{selectedRole.name}</strong>?
            </p>
            <p className="mt-1 text-xs text-zinc-500">
              Esta acción no se puede deshacer. No podrás eliminarlo si tiene usuarios asignados.
            </p>

            <div className="mt-6 flex justify-end gap-3">
              <button
                onClick={() => {
                  setShowDeleteModal(false);
                  setSelectedRole(null);
                }}
                className="rounded-lg border border-zinc-200 px-4 py-2 text-sm text-zinc-700 hover:bg-zinc-50"
              >
                Cancelar
              </button>
              <button
                onClick={handleDeleteRole}
                className="rounded-lg bg-rose-600 px-4 py-2 text-sm font-medium text-white hover:bg-rose-700"
              >
                Eliminar Rol
              </button>
            </div>
          </div>
        </div>
      )}
    </Protected>
  );
}

// ═══════════════════════════════════════════════════════════════════════════
// TAB: PERMISOS (solo lectura, vista de referencia)
// ═══════════════════════════════════════════════════════════════════════════

function PermissionsTabContent() {
  const { user } = useAuth();
  const [permissions, setPermissions] = useState<PermissionsByModule[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const loadPermissions = async () => {
      setLoading(true);
      try {
        const data = await apiRequest<PermissionsByModule[]>(
          "/api/permissions/grouped",
          undefined,
          user.token
        );
        setPermissions(data);
      } catch (err) {
        setError(
          err instanceof Error ? err.message : "Error cargando permisos"
        );
      } finally {
        setLoading(false);
      }
    };

    loadPermissions();
  }, []);

  return (
    <Protected permission="roles.read">
      <p className="text-sm text-zinc-500">
        Lista de todos los permisos disponibles en el sistema, agrupados por
        módulo.
      </p>

      {error && (
        <div className="rounded-md border border-rose-200 bg-rose-50 px-4 py-3 text-sm text-rose-700">
          {error}
        </div>
      )}

      {loading ? (
        <div className="py-8 text-center text-sm text-zinc-500">
          Cargando permisos...
        </div>
      ) : (
        <div className="space-y-6">
          {permissions.map((group) => (
            <div
              key={group.module}
              className="overflow-hidden rounded-xl border border-zinc-200 bg-white"
            >
              <div className="bg-gradient-to-r from-zinc-50 to-transparent px-4 py-3">
                <h3 className="text-sm font-semibold uppercase tracking-wide text-zinc-700">
                  {group.module}
                </h3>
              </div>
              <div className="divide-y divide-zinc-100">
                {group.permissions.map((perm) => (
                  <div
                    key={perm.id}
                    className="flex items-center justify-between px-4 py-3"
                  >
                    <div>
                      <p className="text-sm font-medium text-zinc-900">
                        {perm.name}
                      </p>
                      <p className="text-xs text-zinc-500">{perm.description}</p>
                    </div>
                    <code className="rounded bg-zinc-100 px-2 py-1 text-xs text-zinc-600">
                      {perm.code}
                    </code>
                  </div>
                ))}
              </div>
            </div>
          ))}
        </div>
      )}
    </Protected>
  );
}
