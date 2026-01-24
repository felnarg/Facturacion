"use client";

import { useEffect, useState } from "react";
import { apiRequest } from "@/lib/api";
import { useAuth } from "@/components/AuthProvider";
import { Protected } from "@/components/Protected";
import { DevBlockHeader } from "@/components/DevBlockHeader";

type Product = {
  id: string;
  name: string;
  description: string;
  price: number;
  stock: number;
  sku: string;
  supplierProductCode: number;
  internalProductCode: number;
  salePercentage: number;
  createdAt: string;
  updatedAt: string;
};

const emptyForm = {
  name: "",
  description: "",
  price: "",
  stock: "",
  sku: "",
  supplierProductCode: "",
  internalProductCode: "",
  salePercentage: "",
};

export default function CatalogoPage() {
  const { user } = useAuth();
  const [products, setProducts] = useState<Product[]>([]);
  const [form, setForm] = useState({ ...emptyForm });
  const [editing, setEditing] = useState<Product | null>(null);
  const [error, setError] = useState("");
  const [searchModalOpen, setSearchModalOpen] = useState(false);
  const [searchTerm, setSearchTerm] = useState("");
  const [searchResults, setSearchResults] = useState<Product[]>([]);

  const loadProducts = async () => {
    const data = await apiRequest<Product[]>(
      "/api/catalog/products",
      undefined,
      user.token
    );
    setProducts(data);
  };

  const searchProducts = async (term: string) => {
    if (!term.trim()) {
      setSearchResults([]);
      return;
    }

    const data = await apiRequest<Product[]>(
      `/api/catalog/products?search=${encodeURIComponent(term)}`,
      undefined,
      user.token
    );
    setSearchResults(data);
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
      supplierProductCode: Number(form.supplierProductCode),
      internalProductCode: Number(form.internalProductCode),
      salePercentage: Number(form.salePercentage),
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
      supplierProductCode: String(product.supplierProductCode),
      internalProductCode: String(product.internalProductCode),
      salePercentage: String(product.salePercentage),
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
          className="dev-block-container grid gap-3 rounded-xl bg-white p-4 shadow-sm md:grid-cols-2"
        >
          <DevBlockHeader label="tomillo" />
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
            placeholder="% Venta"
            type="number"
            min={1}
            max={100}
            value={form.salePercentage}
            onChange={(event) =>
              setForm((prev) => ({ ...prev, salePercentage: event.target.value }))
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
          <input
            className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
            placeholder="Código proveedor"
            type="number"
            value={form.supplierProductCode}
            onChange={(event) =>
              setForm((prev) => ({
                ...prev,
                supplierProductCode: event.target.value,
              }))
            }
            required
          />
          <input
            className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
            placeholder="Código interno"
            type="number"
            value={form.internalProductCode}
            onChange={(event) =>
              setForm((prev) => ({
                ...prev,
                internalProductCode: event.target.value,
              }))
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
            onKeyDown={(event) => {
              if (event.key === "Enter" && !event.shiftKey) {
                event.preventDefault();
                setSearchTerm(form.description);
                setSearchModalOpen(true);
                searchProducts(form.description).catch(() => {});
              }
            }}
          />
          <div className="md:col-span-2 flex items-center gap-3">
            <button type="submit" className="btn-primary">
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

        <div className="dev-block-container overflow-hidden rounded-xl border border-zinc-200 bg-white">
          <DevBlockHeader label="nuez" />
          <table className="w-full text-sm">
            <thead className="bg-zinc-50 text-left text-xs uppercase text-zinc-500">
              <tr>
                <th className="px-4 py-3">Nombre</th>
                <th className="px-4 py-3">SKU</th>
                <th className="px-4 py-3">Cod. Prov</th>
                <th className="px-4 py-3">Cod. Interno</th>
                <th className="px-4 py-3">Precio</th>
                <th className="px-4 py-3">% Venta</th>
                <th className="px-4 py-3">Stock</th>
                <th className="px-4 py-3"></th>
              </tr>
            </thead>
            <tbody>
              {products.map((product) => (
                <tr key={product.id} className="border-t border-zinc-100">
                  <td className="px-4 py-3">{product.name}</td>
                  <td className="px-4 py-3">{product.sku}</td>
                  <td className="px-4 py-3">{product.supplierProductCode}</td>
                  <td className="px-4 py-3">{product.internalProductCode}</td>
                  <td className="px-4 py-3">{product.price}</td>
                  <td className="px-4 py-3">{product.salePercentage}</td>
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
                    colSpan={8}
                    className="px-4 py-6 text-center text-sm text-zinc-500"
                  >
                    No hay productos registrados.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>

        {searchModalOpen && (
          <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
            <div className="dev-block-container w-full max-w-3xl rounded-xl bg-white p-4 shadow-lg">
              <DevBlockHeader label="ambar" />
              <div className="flex items-center justify-between">
                <h3 className="text-sm font-semibold text-zinc-800">
                  Buscar producto
                </h3>
                <button
                  onClick={() => setSearchModalOpen(false)}
                  className="btn-secondary text-xs"
                >
                  Cerrar
                </button>
              </div>

              <div className="mt-3">
                <input
                  className="w-full rounded-md border border-zinc-200 px-3 py-2 text-sm"
                  placeholder="Busca por nombre, SKU, códigos o descripción"
                  value={searchTerm}
                  onChange={(event) => {
                    const value = event.target.value;
                    setSearchTerm(value);
                    searchProducts(value).catch(() => {});
                  }}
                />
              </div>

              <div className="mt-4 max-h-72 overflow-auto rounded-md border border-zinc-100">
                {searchResults.map((product) => (
                  <button
                    key={product.id}
                    onClick={() => {
                      setForm({
                        name: product.name,
                        description: product.description,
                        price: String(product.price),
                        salePercentage: String(product.salePercentage),
                        stock: String(product.stock),
                        sku: product.sku,
                        supplierProductCode: String(product.supplierProductCode),
                        internalProductCode: String(product.internalProductCode),
                      });
                      setEditing(product);
                      setSearchModalOpen(false);
                    }}
                    className="flex w-full flex-col gap-1 border-b border-zinc-100 px-3 py-3 text-left text-sm hover:bg-zinc-50"
                  >
                    <span className="font-medium text-zinc-900">
                      {product.name} ({product.sku})
                    </span>
                    <span className="text-xs text-zinc-500">
                      Prov: {product.supplierProductCode} · Interno:{" "}
                      {product.internalProductCode}
                    </span>
                    <span className="text-xs text-zinc-400">
                      {product.description}
                    </span>
                  </button>
                ))}
                {searchResults.length === 0 && (
                  <div className="px-3 py-6 text-center text-xs text-zinc-500">
                    No hay resultados.
                  </div>
                )}
              </div>
            </div>
          </div>
        )}
      </div>
    </Protected>
  );
}
