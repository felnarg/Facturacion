"use client";

import { useEffect, useState } from "react";
import { apiRequest } from "@/lib/api";
import { useAuth } from "@/components/AuthProvider";
import { Protected } from "@/components/Protected";
import { DevBlockHeader } from "@/components/DevBlockHeader";

type Customer = {
  id: string;
  name: string;
  email: string;
  points: number;
  createdAt: string;
  updatedAt: string;
};

export default function ClientesPage() {
  const { user } = useAuth();
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [form, setForm] = useState({ name: "", email: "" });
  const [error, setError] = useState("");

  const loadCustomers = async () => {
    const data = await apiRequest<Customer[]>(
      "/api/customers",
      undefined,
      user.token
    );
    setCustomers(data);
  };

  useEffect(() => {
    loadCustomers().catch((err) =>
      setError(err instanceof Error ? err.message : "Error cargando clientes")
    );
  }, []);

  const handleCreate = async (event: React.FormEvent) => {
    event.preventDefault();
    setError("");

    try {
      await apiRequest<Customer>(
        "/api/customers",
        {
          method: "POST",
          body: JSON.stringify(form),
        },
        user.token
      );
      setForm({ name: "", email: "" });
      await loadCustomers();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error creando cliente");
    }
  };

  return (
    <Protected permission="clientes">
      <div className="space-y-6">
        <header>
          <h2 className="text-xl font-semibold text-zinc-900">Clientes</h2>
          <p className="text-sm text-zinc-500">
            Gestiona los clientes y sus datos básicos.
          </p>
        </header>

        <form
          onSubmit={handleCreate}
          className="dev-block-container grid gap-3 rounded-xl bg-white p-4 shadow-sm md:grid-cols-2"
        >
          <DevBlockHeader label="perla" />
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
          <div className="md:col-span-2">
            <button className="btn-primary">
              Crear cliente
            </button>
          </div>
          {error && (
            <p className="md:col-span-2 text-sm text-rose-600">{error}</p>
          )}
        </form>

        <div className="dev-block-container overflow-hidden rounded-xl border border-zinc-200 bg-white">
          <DevBlockHeader label="pampa" />
          <table className="w-full text-sm">
            <thead className="bg-zinc-50 text-left text-xs uppercase text-zinc-500">
              <tr>
                <th className="px-4 py-3">Nombre</th>
                <th className="px-4 py-3">Correo</th>
                <th className="px-4 py-3">Puntos</th>
              </tr>
            </thead>
            <tbody>
              {customers.map((customer) => (
                <tr key={customer.id} className="border-t border-zinc-100">
                  <td className="px-4 py-3">{customer.name}</td>
                  <td className="px-4 py-3">{customer.email}</td>
                  <td className="px-4 py-3">{customer.points}</td>
                </tr>
              ))}
              {customers.length === 0 && (
                <tr>
                  <td
                    colSpan={3}
                    className="px-4 py-6 text-center text-sm text-zinc-500"
                  >
                    No hay clientes registrados.
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
