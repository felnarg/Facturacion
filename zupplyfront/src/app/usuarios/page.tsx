"use client";

import { useEffect, useState } from "react";
import { apiRequest } from "@/lib/api";
import { useAuth } from "@/components/AuthProvider";
import { Protected } from "@/components/Protected";

type User = {
  id: string;
  name: string;
  email: string;
  createdAt: string;
  updatedAt: string;
};

export default function UsuariosPage() {
  const { user } = useAuth();
  const [users, setUsers] = useState<User[]>([]);
  const [form, setForm] = useState({ name: "", email: "", password: "" });
  const [error, setError] = useState("");

  const loadUsers = async () => {
    const data = await apiRequest<User[]>(
      "/api/users",
      undefined,
      user.token
    );
    setUsers(data);
  };

  useEffect(() => {
    loadUsers().catch((err) =>
      setError(err instanceof Error ? err.message : "Error cargando usuarios")
    );
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

  return (
    <Protected permission="usuarios">
      <div className="space-y-6">
        <header>
          <h2 className="text-xl font-semibold text-zinc-900">Usuarios</h2>
          <p className="text-sm text-zinc-500">Gestiona usuarios del IAM.</p>
        </header>

        <form
          onSubmit={handleCreate}
          className="grid gap-3 rounded-xl bg-white p-4 shadow-sm md:grid-cols-3"
        >
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
          <div className="md:col-span-3">
            <button className="rounded-md bg-zinc-900 px-4 py-2 text-sm text-white">
              Crear usuario
            </button>
          </div>
          {error && (
            <p className="md:col-span-3 text-sm text-rose-600">{error}</p>
          )}
        </form>

        <div className="overflow-hidden rounded-xl border border-zinc-200 bg-white">
          <table className="w-full text-sm">
            <thead className="bg-zinc-50 text-left text-xs uppercase text-zinc-500">
              <tr>
                <th className="px-4 py-3">Nombre</th>
                <th className="px-4 py-3">Correo</th>
              </tr>
            </thead>
            <tbody>
              {users.map((userRow) => (
                <tr key={userRow.id} className="border-t border-zinc-100">
                  <td className="px-4 py-3">{userRow.name}</td>
                  <td className="px-4 py-3">{userRow.email}</td>
                </tr>
              ))}
              {users.length === 0 && (
                <tr>
                  <td
                    colSpan={2}
                    className="px-4 py-6 text-center text-sm text-zinc-500"
                  >
                    No hay usuarios registrados.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>
    </Protected>
  );
}
