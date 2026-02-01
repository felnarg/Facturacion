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
  city: string;
  phone: string;
  address: string;
  type: "Natural" | "Juridica";
  identificationType: "CC" | "NIT";
  identificationNumber: string;
  points: number;
  createdAt: string;
  updatedAt: string;
};

export default function ClientesPage() {
  const { user } = useAuth();
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [search, setSearch] = useState("");
  const [editingId, setEditingId] = useState<string | null>(null);
  const [form, setForm] = useState({
    name: "",
    email: "",
    city: "",
    phone: "",
    address: "",
    type: "Natural" as Customer["type"],
    identificationType: "CC" as Customer["identificationType"],
    identificationNumber: "",
  });
  const [error, setError] = useState("");

  const loadCustomers = async (term: string) => {
    const query = term.trim()
      ? `?search=${encodeURIComponent(term.trim())}`
      : "";
    const data = await apiRequest<Customer[]>(
      `/api/customers${query}`,
      undefined,
      user.token
    );
    setCustomers(data);
  };

  useEffect(() => {
    const timeout = setTimeout(() => {
      loadCustomers(search).catch((err) =>
        setError(
          err instanceof Error ? err.message : "Error cargando clientes"
        )
      );
    }, 300);

    return () => clearTimeout(timeout);
  }, [search]);

  const resetForm = () => {
    setForm({
      name: "",
      email: "",
      city: "",
      phone: "",
      address: "",
      type: "Natural",
      identificationType: "CC",
      identificationNumber: "",
    });
    setEditingId(null);
  };

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError("");

    try {
      if (editingId) {
        await apiRequest<Customer>(
          `/api/customers/${editingId}`,
          {
            method: "PUT",
            body: JSON.stringify(form),
          },
          user.token
        );
      } else {
        await apiRequest<Customer>(
          "/api/customers",
          {
            method: "POST",
            body: JSON.stringify(form),
          },
          user.token
        );
      }
      resetForm();
      await loadCustomers(search);
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : editingId
            ? "Error actualizando cliente"
            : "Error creando cliente"
      );
    }
  };

  const handleEdit = (customer: Customer) => {
    setEditingId(customer.id);
    setForm({
      name: customer.name,
      email: customer.email,
      city: customer.city,
      phone: customer.phone,
      address: customer.address,
      type: customer.type,
      identificationType: customer.identificationType,
      identificationNumber: customer.identificationNumber,
    });
  };

  const handleDelete = async (id: string) => {
    const confirmed = window.confirm(
      "¿Deseas eliminar este cliente? Esta acción no se puede deshacer."
    );
    if (!confirmed) return;

    setError("");
    try {
      await apiRequest<void>(
        `/api/customers/${id}`,
        { method: "DELETE" },
        user.token
      );
      if (editingId === id) {
        resetForm();
      }
      await loadCustomers(search);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error eliminando cliente");
    }
  };

  return (
    <Protected module="clientes">
      <div className="space-y-6">
        <header>
          <h2 className="text-xl font-semibold text-zinc-900">Clientes</h2>
          <p className="text-sm text-zinc-500">
            Gestiona los clientes y sus datos básicos.
          </p>
        </header>

        <form
          onSubmit={handleSubmit}
          className="dev-block-container grid gap-3 rounded-xl bg-white p-4 shadow-sm md:grid-cols-3"
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
          <input
            className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
            placeholder="Ciudad"
            value={form.city}
            onChange={(event) =>
              setForm((prev) => ({ ...prev, city: event.target.value }))
            }
            required
          />
          <input
            className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
            placeholder="Teléfono"
            value={form.phone}
            onChange={(event) =>
              setForm((prev) => ({ ...prev, phone: event.target.value }))
            }
            required
          />
          <input
            className="rounded-md border border-zinc-200 px-3 py-2 text-sm md:col-span-2"
            placeholder="Dirección"
            value={form.address}
            onChange={(event) =>
              setForm((prev) => ({ ...prev, address: event.target.value }))
            }
            required
          />
          <select
            className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
            value={form.type}
            onChange={(event) =>
              setForm((prev) => ({
                ...prev,
                type: event.target.value as Customer["type"],
              }))
            }
          >
            <option value="Natural">Persona natural</option>
            <option value="Juridica">Persona jurídica</option>
          </select>
          <select
            className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
            value={form.identificationType}
            onChange={(event) =>
              setForm((prev) => ({
                ...prev,
                identificationType: event.target.value as Customer["identificationType"],
              }))
            }
          >
            <option value="CC">CC</option>
            <option value="NIT">NIT</option>
          </select>
          <input
            className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
            placeholder="Número de identificación"
            value={form.identificationNumber}
            onChange={(event) =>
              setForm((prev) => ({
                ...prev,
                identificationNumber: event.target.value,
              }))
            }
            required
          />
          <div className="flex flex-wrap gap-2 md:col-span-3">
            <button className="btn-primary">
              {editingId ? "Guardar cambios" : "Crear cliente"}
            </button>
            {editingId && (
              <button
                type="button"
                onClick={resetForm}
                className="btn-secondary"
              >
                Cancelar
              </button>
            )}
          </div>
          {error && (
            <p className="md:col-span-3 text-sm text-rose-600">{error}</p>
          )}
        </form>

        <div className="dev-block-container overflow-hidden rounded-xl border border-zinc-200 bg-white">
          <DevBlockHeader label="pampa" />
          <div className="flex flex-wrap items-center gap-2 border-b border-zinc-100 p-4">
            <input
              className="min-w-[240px] rounded-md border border-zinc-200 px-3 py-2 text-sm"
              placeholder="Buscar por nombre, email, ciudad, teléfono o CC/NIT..."
              value={search}
              onChange={(event) => setSearch(event.target.value)}
            />
            {search && (
              <button
                type="button"
                onClick={() => setSearch("")}
                className="btn-secondary"
              >
                Limpiar
              </button>
            )}
          </div>
          <table className="w-full min-w-[1200px] text-sm">
            <thead className="bg-zinc-50 text-left text-xs uppercase text-zinc-500">
              <tr>
                <th className="px-4 py-3">Nombre</th>
                <th className="px-4 py-3">Tipo</th>
                <th className="px-4 py-3">CC/NIT</th>
                <th className="px-4 py-3">Ciudad</th>
                <th className="px-4 py-3">Teléfono</th>
                <th className="px-4 py-3">Dirección</th>
                <th className="px-4 py-3">Correo</th>
                <th className="px-4 py-3">Puntos</th>
                <th className="px-4 py-3">Acciones</th>
              </tr>
            </thead>
            <tbody>
              {customers.map((customer) => (
                <tr key={customer.id} className="border-t border-zinc-100">
                  <td className="px-4 py-3">{customer.name}</td>
                  <td className="px-4 py-3">
                    {customer.type === "Natural" ? "Natural" : "Jurídica"}
                  </td>
                  <td className="px-4 py-3">
                    {customer.identificationType} {customer.identificationNumber}
                  </td>
                  <td className="px-4 py-3">{customer.city}</td>
                  <td className="px-4 py-3">{customer.phone}</td>
                  <td className="px-4 py-3">{customer.address}</td>
                  <td className="px-4 py-3">{customer.email}</td>
                  <td className="px-4 py-3">{customer.points}</td>
                  <td className="px-4 py-3">
                    <div className="flex gap-2">
                      <button
                        type="button"
                        className="btn-secondary"
                        onClick={() => handleEdit(customer)}
                      >
                        Editar
                      </button>
                      <button
                        type="button"
                        className="btn-secondary"
                        onClick={() => handleDelete(customer.id)}
                      >
                        Eliminar
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
              {customers.length === 0 && (
                <tr>
                  <td
                    colSpan={9}
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
