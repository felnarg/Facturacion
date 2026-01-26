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
  supplierProductCode: number;
  internalProductCode: number;
  salePercentage: number;
  consumptionTaxPercentage: number;
  wholesaleSalePercentage: number;
  specialSalePercentage: number;
  iva: number;
  createdAt: string;
  updatedAt: string;
};

const emptyForm = {
  name: "",
  description: "",
  price: "0",
  stock: "0",
  supplierProductCode: "",
  internalProductCode: "",
  salePercentage: "30",
  consumptionTaxPercentage: "0",
  wholesaleSalePercentage: "25",
  specialSalePercentage: "20",
  iva: "19",
};


function safeNumber(value: string | number | undefined | null, defaultValue = 0): number {
  if (value === null || value === undefined || value === "") return defaultValue;
  const num = Number(value);
  return isNaN(num) ? defaultValue : num;
}

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

  // Global Enter listener
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      // If modal is open, let it behave normally (or handle search there)
      if (searchModalOpen) return;

      // Check if the key is Enter
      if (e.key === "Enter") {
        // We only want to open if the user isn't typing in a form field that might need Enter,
        // although the user requested "when I press enter, open search".
        // To prevent interfering with buttons or other Enter actions, we might check activeElement.
        // However, standard form 'Enter' usually submits. The user wants to OVERRIDE specific workflow. "despliegue un pop up"
        // Let's compromise: If focus is NOT on a button/input, OR if it IS but we want to intercept.
        // Given the request "presione enter se despliegue un pop up", it sounds like a hotkey.
        // I'll prevent default to avoid submission if it was going to happen, and open the modal.
        e.preventDefault();
        setSearchModalOpen(true);
      }
    };

    window.addEventListener("keydown", handleKeyDown);
    return () => window.removeEventListener("keydown", handleKeyDown);
  }, [searchModalOpen]);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError("");

    // Explicitly construct payload with safe number conversion
    // Matching CreateProductRequest.cs properties (camelCase for JSON)
    const payload = {
      name: form.name,
      description: form.description,
      price: safeNumber(form.price),
      stock: safeNumber(form.stock),
      supplierProductCode: safeNumber(form.supplierProductCode),
      internalProductCode: safeNumber(form.internalProductCode),
      salePercentage: safeNumber(form.salePercentage),
      consumptionTaxPercentage: safeNumber(form.consumptionTaxPercentage),
      wholesaleSalePercentage: safeNumber(form.wholesaleSalePercentage),
      specialSalePercentage: safeNumber(form.specialSalePercentage),
      iva: safeNumber(form.iva),
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
      supplierProductCode: String(product.supplierProductCode),
      internalProductCode: String(product.internalProductCode),
      salePercentage: String(product.salePercentage),
      consumptionTaxPercentage: String(product.consumptionTaxPercentage),
      wholesaleSalePercentage: String(product.wholesaleSalePercentage),
      specialSalePercentage: String(product.specialSalePercentage),
      iva: String(product.iva),
    });
  };

  const handleDelete = async (id: string) => {
    if (!confirm("¿Seguro que deseas eliminar este producto?")) {
      return;
    }

    try {
      await apiRequest(
        `/api/catalog/products/${id}`,
        { method: "DELETE" },
        user.token
      );
      // Reset form if we were editing the deleted item
      if (editing?.id === id) {
        setEditing(null);
        setForm({ ...emptyForm });
      }
      await loadProducts();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error eliminando producto");
    }
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

          {/* 1. Nombre */}
          <div className="flex flex-col gap-1">
            <label className="text-[10px] font-bold uppercase tracking-wider text-zinc-500 ml-1">Nombre</label>
            <input
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
              value={form.name}
              onChange={(event) =>
                setForm((prev) => ({ ...prev, name: event.target.value }))
              }
              required
            />
          </div>

          {/* 2. Codigo interno */}
          <div className="flex flex-col gap-1">
            <label className="text-[10px] font-bold uppercase tracking-wider text-zinc-500 ml-1">Código interno</label>
            <input
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
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
          </div>

          {/* 3. Codigo producto */}
          <div className="flex flex-col gap-1">
            <label className="text-[10px] font-bold uppercase tracking-wider text-zinc-500 ml-1">Código producto</label>
            <input
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
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
          </div>

          {/* 4. Stock */}
          <div className="flex flex-col gap-1">
            <label className="text-[10px] font-bold uppercase tracking-wider text-zinc-500 ml-1">Stock</label>
            <input
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
              type="number"
              value={form.stock}
              onChange={(event) =>
                setForm((prev) => ({ ...prev, stock: event.target.value }))
              }
              required
            />
          </div>

          {/* 5. Precio */}
          <div className="flex flex-col gap-1">
            <label className="text-[10px] font-bold uppercase tracking-wider text-zinc-500 ml-1">Precio</label>
            <input
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
              type="number"
              value={form.price}
              onChange={(event) =>
                setForm((prev) => ({ ...prev, price: event.target.value }))
              }
              required
            />
          </div>

          {/* 6. imp consumo */}
          <div className="flex flex-col gap-1">
            <label className="text-[10px] font-bold uppercase tracking-wider text-zinc-500 ml-1">% Imp. Consumo</label>
            <input
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
              type="number"
              min={0}
              max={100}
              value={form.consumptionTaxPercentage}
              onChange={(event) =>
                setForm((prev) => ({ ...prev, consumptionTaxPercentage: event.target.value }))
              }
              required
            />
          </div>

          {/* 7. iva */}
          <div className="flex flex-col gap-1">
            <label className="text-[10px] font-bold uppercase tracking-wider text-zinc-500 ml-1">% IVA</label>
            <input
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
              type="number"
              min={0}
              max={100}
              value={form.iva}
              onChange={(event) =>
                setForm((prev) => ({ ...prev, iva: event.target.value }))
              }
              required
            />
          </div>

          {/* 8. % venta */}
          <div className="flex flex-col gap-1">
            <label className="text-[10px] font-bold uppercase tracking-wider text-zinc-500 ml-1">% Venta</label>
            <input
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
              type="number"
              min={0}
              max={100}
              value={form.salePercentage}
              onChange={(event) =>
                setForm((prev) => ({ ...prev, salePercentage: event.target.value }))
              }
              required
            />
          </div>

          {/* 9. % venta mayor */}
          <div className="flex flex-col gap-1">
            <label className="text-[10px] font-bold uppercase tracking-wider text-zinc-500 ml-1">% Venta Mayor</label>
            <input
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
              type="number"
              min={0}
              max={100}
              value={form.wholesaleSalePercentage}
              onChange={(event) =>
                setForm((prev) => ({ ...prev, wholesaleSalePercentage: event.target.value }))
              }
              required
            />
          </div>

          {/* 10. % venta especial */}
          <div className="flex flex-col gap-1">
            <label className="text-[10px] font-bold uppercase tracking-wider text-zinc-500 ml-1">% Venta Especial</label>
            <input
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
              type="number"
              min={0}
              max={100}
              value={form.specialSalePercentage}
              onChange={(event) =>
                setForm((prev) => ({ ...prev, specialSalePercentage: event.target.value }))
              }
              required
            />
          </div>

          {/* 11. Descripción */}
          <div className="flex flex-col gap-1 md:col-span-2">
            <label className="text-[10px] font-bold uppercase tracking-wider text-zinc-500 ml-1">Descripción</label>
            <textarea
              className="w-full rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
              rows={3}
              value={form.description}
              onChange={(event) =>
                setForm((prev) => ({ ...prev, description: event.target.value }))
              }
            />
          </div>

          <div className="md:col-span-2 flex items-center gap-3">
            <button type="submit" className="btn-primary">
              {editing ? "Actualizar" : "Crear"}
            </button>
            {editing && (
              <>
                <button
                  type="button"
                  onClick={() => handleDelete(editing.id)}
                  className="rounded-md bg-rose-50 px-4 py-2 text-sm font-medium text-rose-600 hover:bg-rose-100"
                >
                  Eliminar
                </button>
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
              </>
            )}
          </div>
          {error && (
            <p className="md:col-span-2 text-sm text-rose-600">{error}</p>
          )}
        </form>



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
                  placeholder="Busca por nombre, códigos o descripción"
                  value={searchTerm}
                  autoFocus
                  onChange={(event) => {
                    const value = event.target.value;
                    setSearchTerm(value);
                    searchProducts(value).catch(() => { });
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
                        consumptionTaxPercentage: String(product.consumptionTaxPercentage),
                        wholesaleSalePercentage: String(product.wholesaleSalePercentage),
                        specialSalePercentage: String(product.specialSalePercentage),
                        iva: String(product.iva),
                        stock: String(product.stock),
                        supplierProductCode: String(product.supplierProductCode),
                        internalProductCode: String(product.internalProductCode),
                      });
                      setEditing(product);
                      setSearchModalOpen(false);
                      setSearchTerm("");
                      setSearchResults([]);
                    }}
                    className="flex w-full flex-col gap-1 border-b border-zinc-100 px-3 py-3 text-left text-sm hover:bg-zinc-50"
                  >
                    <span className="font-medium text-zinc-900">
                      {product.name}
                    </span>
                    <span className="text-xs text-zinc-500">
                      Prod: {product.supplierProductCode} · Interno:{" "}
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
