"use client";

import { useEffect, useState } from "react";
import { apiRequest } from "@/lib/api";
import { useAuth } from "@/components/AuthProvider";
import { Protected } from "@/components/Protected";

type Product = {
  id: string;
  name: string;
  description: string;
  price: number;
  stock: number;
  sku: string;
  createdAt: string;
  updatedAt: string;
};

const emptyForm = {
  name: "",
  description: "",
  price: "",
  stock: "",
  sku: "",
};

export default function CatalogoPage() {
  const { user } = useAuth();
  const [products, setProducts] = useState<Product[]>([]);
  const [form, setForm] = useState({ ...emptyForm });
  const [editing, setEditing] = useState<Product | null>(null);
  const [error, setError] = useState("");

  const loadProducts = async () => {
    const data = await apiRequest<Product[]>(
      "/api/catalog/products",
      undefined,
      user.token
    );
    setProducts(data);
  };

  useEffect(() => {
    loadProducts().catch((err) =>
      setError(err instanceof Error ? err.message : "Error cargando productos")
    );
  }, []);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError("");

    const payload = {
      name: form.name,
      description: form.description,
      price: Number(form.price),
      stock: Number(form.stock),
      sku: form.sku,
    };

    try {
      if (editing) {
        await apiRequest<Product>(
          `/api/catalog/products/${editing.id}`,
          {
            method: "PUT",
            body: JSON.stringify(payload),
          },
          user.token
        );
        setEditing(null);
      } else {
        await apiRequest<Product>(
          "/api/catalog/products",
          {
            method: "POST",
            body: JSON.stringify(payload),
          },
          user.token
        );
      }

      setForm({ ...emptyForm });
      await loadProducts();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error guardando producto");
    }
  };

  const handleEdit = (product: Product) => {
    setEditing(product);
    setForm({
      name: product.name,
      description: product.description,
      price: String(product.price),
      stock: String(product.stock),
      sku: product.sku,
    });
  };

  const handleDelete = async (id: string) => {
    if (!confirm("¿Seguro que deseas eliminar este producto?")) {
      return;
    }

    await apiRequest(
      `/api/catalog/products/${id}`,
      { method: "DELETE" },
      user.token
    );
    await loadProducts();
  };

  return (
    <Protected permission="catalogo">
      <div className="space-y-6">
        <header>
          <h2 className="text-xl font-semibold text-zinc-900">Catálogo</h2>
          <p className="text-sm text-zinc-500">
            Crear, editar y eliminar productos.
          </p>
        </header>

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
            placeholder="SKU"
            value={form.sku}
            onChange={(event) =>
              setForm((prev) => ({ ...prev, sku: event.target.value }))
            }
            required
          />
          <input
            className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
            placeholder="Precio"
            type="number"
            value={form.price}
            onChange={(event) =>
              setForm((prev) => ({ ...prev, price: event.target.value }))
            }
            required
          />
          <input
            className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
            placeholder="Stock"
            type="number"
            value={form.stock}
            onChange={(event) =>
              setForm((prev) => ({ ...prev, stock: event.target.value }))
            }
            required
          />
          <textarea
            className="md:col-span-2 rounded-md border border-zinc-200 px-3 py-2 text-sm"
            placeholder="Descripción"
            value={form.description}
            onChange={(event) =>
              setForm((prev) => ({ ...prev, description: event.target.value }))
            }
          />
          <div className="md:col-span-2 flex items-center gap-3">
            <button
              type="submit"
              className="rounded-md bg-zinc-900 px-4 py-2 text-sm text-white"
            >
              {editing ? "Actualizar" : "Crear"}
            </button>
            {editing && (
              <button
                type="button"
                onClick={() => {
                  setEditing(null);
                  setForm({ ...emptyForm });
                }}
                className="rounded-md border border-zinc-300 px-4 py-2 text-sm"
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
                <th className="px-4 py-3">SKU</th>
                <th className="px-4 py-3">Precio</th>
                <th className="px-4 py-3">Stock</th>
                <th className="px-4 py-3"></th>
              </tr>
            </thead>
            <tbody>
              {products.map((product) => (
                <tr key={product.id} className="border-t border-zinc-100">
                  <td className="px-4 py-3">{product.name}</td>
                  <td className="px-4 py-3">{product.sku}</td>
                  <td className="px-4 py-3">{product.price}</td>
                  <td className="px-4 py-3">{product.stock}</td>
                  <td className="px-4 py-3 text-right">
                    <button
                      className="mr-2 text-xs text-blue-600"
                      onClick={() => handleEdit(product)}
                    >
                      Editar
                    </button>
                    <button
                      className="text-xs text-rose-600"
                      onClick={() => handleDelete(product.id)}
                    >
                      Eliminar
                    </button>
                  </td>
                </tr>
              ))}
              {products.length === 0 && (
                <tr>
                  <td
                    colSpan={5}
                    className="px-4 py-6 text-center text-sm text-zinc-500"
                  >
                    No hay productos registrados.
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
