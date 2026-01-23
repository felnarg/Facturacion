"use client";

import { useEffect, useState } from "react";
import { apiRequest } from "@/lib/api";
import { useAuth } from "@/components/AuthProvider";
import { Protected } from "@/components/Protected";

type Purchase = {
  id: string;
  productId: string;
  supplierId: string;
  quantity: number;
  supplierName: string;
  status: string;
  createdAt: string;
  receivedAt?: string | null;
};

type Supplier = {
  id: string;
  name: string;
  contactName: string;
  phone: string;
  email: string;
  address: string;
};

export default function ComprasPage() {
  const { user } = useAuth();
  const [purchases, setPurchases] = useState<Purchase[]>([]);
  const [suppliers, setSuppliers] = useState<Supplier[]>([]);
  const [form, setForm] = useState({
    productId: "",
    quantity: "",
    supplierId: "",
    supplierName: "",
  });
  const [supplierSearch, setSupplierSearch] = useState("");
  const [supplierModalOpen, setSupplierModalOpen] = useState(false);
  const [error, setError] = useState("");

  const loadPurchases = async () => {
    const data = await apiRequest<Purchase[]>(
      "/api/purchases",
      undefined,
      user.token
    );
    setPurchases(data);
  };

  useEffect(() => {
    loadPurchases().catch((err) =>
      setError(err instanceof Error ? err.message : "Error cargando compras")
    );
  }, []);

  const loadSuppliers = async (search?: string) => {
    const query = search ? `?search=${encodeURIComponent(search)}` : "";
    const data = await apiRequest<Supplier[]>(
      `/api/suppliers${query}`,
      undefined,
      user.token
    );
    setSuppliers(data);
  };

  useEffect(() => {
    if (!supplierModalOpen) {
      return;
    }

    loadSuppliers().catch((err) =>
      setError(
        err instanceof Error ? err.message : "Error cargando proveedores"
      )
    );
  }, [supplierModalOpen]);

  useEffect(() => {
    if (!supplierModalOpen) {
      return;
    }

    const handler = setTimeout(() => {
      loadSuppliers(supplierSearch).catch((err) =>
        setError(
          err instanceof Error ? err.message : "Error filtrando proveedores"
        )
      );
    }, 250);

    return () => clearTimeout(handler);
  }, [supplierSearch, supplierModalOpen]);

  const handleCreate = async (event: React.FormEvent) => {
    event.preventDefault();
    setError("");

    try {
      await apiRequest<Purchase>(
        "/api/purchases",
        {
          method: "POST",
          body: JSON.stringify({
            productId: form.productId,
            quantity: Number(form.quantity),
            supplierId: form.supplierId,
          }),
        },
        user.token
      );
      setForm({
        productId: "",
        quantity: "",
        supplierId: "",
        supplierName: "",
      });
      await loadPurchases();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error creando compra");
    }
  };

  const markReceived = async (id: string) => {
    await apiRequest(
      `/api/purchases/${id}/receive`,
      { method: "PUT" },
      user.token
    );
    await loadPurchases();
  };

  return (
    <Protected permission="compras">
      <div className="space-y-6">
        <header>
          <h2 className="text-xl font-semibold text-zinc-900">Compras</h2>
          <p className="text-sm text-zinc-500">
            Registra compras y confirma recepciones.
          </p>
        </header>

        <form
          onSubmit={handleCreate}
          className="grid gap-3 rounded-xl bg-white p-4 shadow-sm md:grid-cols-3"
        >
          <input
            className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
            placeholder="ProductId"
            value={form.productId}
            onChange={(event) =>
              setForm((prev) => ({ ...prev, productId: event.target.value }))
            }
            required
          />
          <input
            className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
            placeholder="Cantidad"
            type="number"
            value={form.quantity}
            onChange={(event) =>
              setForm((prev) => ({ ...prev, quantity: event.target.value }))
            }
            required
          />
          <div className="flex items-center gap-2">
            <input
              className="w-full rounded-md border border-zinc-200 px-3 py-2 text-sm"
              placeholder="Proveedor"
              value={form.supplierName}
              readOnly
              required
            />
            <button
              type="button"
              onClick={() => setSupplierModalOpen(true)}
              className="rounded-md border border-zinc-300 px-3 py-2 text-xs"
            >
              Seleccionar
            </button>
          </div>
          <div className="md:col-span-3">
            <button className="rounded-md bg-zinc-900 px-4 py-2 text-sm text-white">
              Registrar compra
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
                <th className="px-4 py-3">Producto</th>
                <th className="px-4 py-3">Cantidad</th>
                <th className="px-4 py-3">Proveedor</th>
                <th className="px-4 py-3">Estado</th>
                <th className="px-4 py-3"></th>
              </tr>
            </thead>
            <tbody>
              {purchases.map((purchase) => (
                <tr key={purchase.id} className="border-t border-zinc-100">
                  <td className="px-4 py-3">{purchase.productId}</td>
                  <td className="px-4 py-3">{purchase.quantity}</td>
                  <td className="px-4 py-3">{purchase.supplierName}</td>
                  <td className="px-4 py-3">{purchase.status}</td>
                  <td className="px-4 py-3 text-right">
                    {purchase.status !== "Received" && (
                      <button
                        onClick={() => markReceived(purchase.id)}
                        className="text-xs text-blue-600"
                      >
                        Marcar recibido
                      </button>
                    )}
                  </td>
                </tr>
              ))}
              {purchases.length === 0 && (
                <tr>
                  <td
                    colSpan={5}
                    className="px-4 py-6 text-center text-sm text-zinc-500"
                  >
                    No hay compras registradas.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>

        {supplierModalOpen && (
          <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
            <div className="w-full max-w-2xl rounded-xl bg-white p-4 shadow-lg">
              <div className="flex items-center justify-between">
                <h3 className="text-sm font-semibold text-zinc-800">
                  Seleccionar proveedor
                </h3>
                <button
                  onClick={() => setSupplierModalOpen(false)}
                  className="text-xs text-zinc-500"
                >
                  Cerrar
                </button>
              </div>

              <div className="mt-3">
                <input
                  className="w-full rounded-md border border-zinc-200 px-3 py-2 text-sm"
                  placeholder="Buscar por nombre, contacto, email o teléfono"
                  value={supplierSearch}
                  onChange={(event) => setSupplierSearch(event.target.value)}
                />
              </div>

              <div className="mt-4 max-h-64 overflow-auto rounded-md border border-zinc-100">
                {suppliers.map((supplier) => (
                  <button
                    key={supplier.id}
                    onClick={() => {
                      setForm((prev) => ({
                        ...prev,
                        supplierId: supplier.id,
                        supplierName: supplier.name,
                      }));
                      setSupplierModalOpen(false);
                    }}
                    className="flex w-full flex-col gap-1 border-b border-zinc-100 px-3 py-3 text-left text-sm hover:bg-zinc-50"
                  >
                    <span className="font-medium text-zinc-900">
                      {supplier.name}
                    </span>
                    <span className="text-xs text-zinc-500">
                      {supplier.contactName} · {supplier.email} ·{" "}
                      {supplier.phone}
                    </span>
                    <span className="text-xs text-zinc-400">
                      {supplier.address}
                    </span>
                  </button>
                ))}
                {suppliers.length === 0 && (
                  <div className="px-3 py-6 text-center text-xs text-zinc-500">
                    No hay proveedores disponibles.
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
