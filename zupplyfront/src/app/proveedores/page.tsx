"use client";

import { useEffect, useState } from "react";
import { apiRequest } from "@/lib/api";
import { useAuth } from "@/components/AuthProvider";
import { Protected } from "@/components/Protected";

type Supplier = {
  id: string;
  name: string;
  contactName: string;
  phone: string;
  email: string;
  address: string;
};

const emptyForm = {
  name: "",
  contactName: "",
  phone: "",
  email: "",
  address: "",
};

export default function ProveedoresPage() {
  const { user } = useAuth();
  const [suppliers, setSuppliers] = useState<Supplier[]>([]);
  const [form, setForm] = useState({ ...emptyForm });
  const [editing, setEditing] = useState<Supplier | null>(null);
  const [search, setSearch] = useState("");
  const [error, setError] = useState("");

  const loadSuppliers = async (term?: string) => {
    const query = term ? `?search=${encodeURIComponent(term)}` : "";
    const data = await apiRequest<Supplier[]>(
      `/api/suppliers${query}`,
      undefined,
      user.token
    );
    setSuppliers(data);
  };

  useEffect(() => {
    loadSuppliers().catch((err) =>
      setError(
        err instanceof Error ? err.message : "Error cargando proveedores"
      )
    );
  }, []);

  useEffect(() => {
    const handler = setTimeout(() => {
      loadSuppliers(search).catch((err) =>
        setError(
          err instanceof Error ? err.message : "Error filtrando proveedores"
        )
      );
    }, 250);

    return () => clearTimeout(handler);
  }, [search]);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError("");

    const payload = {
      name: form.name,
      contactName: form.contactName,
      phone: form.phone,
      email: form.email,
      address: form.address,
    };

    try {
      if (editing) {
        await apiRequest<Supplier>(
          `/api/suppliers/${editing.id}`,
          {
            method: "PUT",
            body: JSON.stringify(payload),
          },
          user.token
        );
        setEditing(null);
      } else {
        await apiRequest<Supplier>(
          "/api/suppliers",
          {
            method: "POST",
            body: JSON.stringify(payload),
          },
          user.token
        );
      }

      setForm({ ...emptyForm });
      await loadSuppliers(search);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error guardando proveedor");
    }
  };

  const handleEdit = (supplier: Supplier) => {
    setEditing(supplier);
    setForm({
      name: supplier.name,
      contactName: supplier.contactName,
      phone: supplier.phone,
      email: supplier.email,
      address: supplier.address,
    });
  };

  const handleDelete = async (id: string) => {
    if (!confirm("¿Seguro que deseas eliminar este proveedor?")) {
      return;
    }

    await apiRequest(
      `/api/suppliers/${id}`,
      { method: "DELETE" },
      user.token
    );
    await loadSuppliers(search);
  };

  return (
    <Protected permission="compras">
      <div className="space-y-6">
        <header>
          <h2 className="text-xl font-semibold text-zinc-900">Proveedores</h2>
          <p className="text-sm text-zinc-500">
            Gestiona el catálogo de proveedores.
          </p>
        </header>

        <div className="rounded-xl bg-white p-4 shadow-sm">
          <input
            className="w-full rounded-md border border-zinc-200 px-3 py-2 text-sm"
            placeholder="Buscar por nombre, email, contacto o teléfono"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
          />
        </div>

        <form
          onSubmit={handleSubmit}
          className="grid gap-3 rounded-xl bg-white p-4 shadow-sm md:grid-cols-2"
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
            placeholder="Contacto"
            value={form.contactName}
            onChange={(event) =>
              setForm((prev) => ({ ...prev, contactName: event.target.value }))
            }
          />
          <input
            className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
            placeholder="Teléfono"
            value={form.phone}
            onChange={(event) =>
              setForm((prev) => ({ ...prev, phone: event.target.value }))
            }
          />
          <input
            className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
            placeholder="Email"
            value={form.email}
            onChange={(event) =>
              setForm((prev) => ({ ...prev, email: event.target.value }))
            }
          />
          <input
            className="md:col-span-2 rounded-md border border-zinc-200 px-3 py-2 text-sm"
            placeholder="Dirección"
            value={form.address}
            onChange={(event) =>
              setForm((prev) => ({ ...prev, address: event.target.value }))
            }
          />
          <div className="md:col-span-2 flex items-center gap-3">
            <button className="btn-primary">
              {editing ? "Actualizar" : "Crear"}
            </button>
            {editing && (
              <button
                type="button"
                onClick={() => {
                  setEditing(null);
                  setForm({ ...emptyForm });
                }}
                className="btn-secondary"
              >
                Cancelar
              </button>
            )}
          </div>
          {error && (
            <p className="md:col-span-2 text-sm text-rose-600">{error}</p>
          )}
        </form>

        <div className="overflow-hidden rounded-xl border border-zinc-200 bg-white">
          <table className="w-full text-sm">
            <thead className="bg-zinc-50 text-left text-xs uppercase text-zinc-500">
              <tr>
                <th className="px-4 py-3">Nombre</th>
                <th className="px-4 py-3">Contacto</th>
                <th className="px-4 py-3">Email</th>
                <th className="px-4 py-3">Teléfono</th>
                <th className="px-4 py-3"></th>
              </tr>
            </thead>
            <tbody>
              {suppliers.map((supplier) => (
                <tr key={supplier.id} className="border-t border-zinc-100">
                  <td className="px-4 py-3">{supplier.name}</td>
                  <td className="px-4 py-3">{supplier.contactName}</td>
                  <td className="px-4 py-3">{supplier.email}</td>
                  <td className="px-4 py-3">{supplier.phone}</td>
                  <td className="px-4 py-3 text-right">
                    <button
                      onClick={() => handleEdit(supplier)}
                      className="mr-2 text-xs text-blue-600"
                    >
                      Editar
                    </button>
                    <button
                      onClick={() => handleDelete(supplier.id)}
                      className="text-xs text-rose-600"
                    >
                      Eliminar
                    </button>
                  </td>
                </tr>
              ))}
              {suppliers.length === 0 && (
                <tr>
                  <td
                    colSpan={5}
                    className="px-4 py-6 text-center text-sm text-zinc-500"
                  >
                    No hay proveedores registrados.
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
