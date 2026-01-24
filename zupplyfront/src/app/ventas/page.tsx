"use client";

import { useEffect, useState } from "react";
import { apiRequest } from "@/lib/api";
import { useAuth } from "@/components/AuthProvider";
import { Protected } from "@/components/Protected";
import { DevBlockHeader } from "@/components/DevBlockHeader";

type SaleItem = {
  productId: string;
  quantity: number;
};

type Sale = {
  id: string;
  createdAt: string;
  items: SaleItem[];
};

export default function VentasPage() {
  const { user } = useAuth();
  const [sales, setSales] = useState<Sale[]>([]);
  const [items, setItems] = useState<SaleItem[]>([]);
  const [productId, setProductId] = useState("");
  const [quantity, setQuantity] = useState("");
  const [error, setError] = useState("");

  const loadSales = async () => {
    const data = await apiRequest<Sale[]>(
      "/api/sales",
      undefined,
      user.token
    );
    setSales(data);
  };

  useEffect(() => {
    loadSales().catch((err) =>
      setError(err instanceof Error ? err.message : "Error cargando ventas")
    );
  }, []);

  const addItem = () => {
    if (!productId || !quantity) {
      return;
    }

    setItems((prev) => [
      ...prev,
      { productId, quantity: Number(quantity) },
    ]);
    setProductId("");
    setQuantity("");
  };

  const handleCreateSale = async () => {
    setError("");
    try {
      await apiRequest<Sale>(
        "/api/sales",
        {
          method: "POST",
          body: JSON.stringify({ items }),
        },
        user.token
      );
      setItems([]);
      await loadSales();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error creando venta");
    }
  };

  return (
    <Protected permission="ventas">
      <div className="space-y-6">
        <header>
          <h2 className="text-xl font-semibold text-zinc-900">Ventas</h2>
          <p className="text-sm text-zinc-500">
            Registra ventas y revisa el historial.
          </p>
        </header>

        <div className="dev-block-container rounded-xl bg-white p-4 shadow-sm">
          <DevBlockHeader label="lima" />
          <h3 className="text-sm font-semibold text-zinc-700">
            Crear nueva venta
          </h3>
          <div className="mt-3 grid gap-3 md:grid-cols-3">
            <input
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
              placeholder="ProductId"
              value={productId}
              onChange={(event) => setProductId(event.target.value)}
            />
            <input
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
              placeholder="Cantidad"
              type="number"
              value={quantity}
              onChange={(event) => setQuantity(event.target.value)}
            />
            <button onClick={addItem} className="btn-secondary">
              Agregar item
            </button>
          </div>

          <div className="mt-4 space-y-2 text-sm">
            {items.map((item, index) => (
              <div
                key={`${item.productId}-${index}`}
                className="flex items-center justify-between rounded-md bg-zinc-50 px-3 py-2"
              >
                <span>{item.productId}</span>
                <span className="text-zinc-500">x {item.quantity}</span>
              </div>
            ))}
            {items.length === 0 && (
              <p className="text-zinc-500">No hay items agregados.</p>
            )}
          </div>

          <button
            onClick={handleCreateSale}
            className="btn-primary mt-4"
            disabled={items.length === 0}
          >
            Registrar venta
          </button>
          {error && <p className="mt-2 text-sm text-rose-600">{error}</p>}
        </div>

        <div className="dev-block-container overflow-hidden rounded-xl border border-zinc-200 bg-white">
          <DevBlockHeader label="pizarra" />
          <table className="w-full text-sm">
            <thead className="bg-zinc-50 text-left text-xs uppercase text-zinc-500">
              <tr>
                <th className="px-4 py-3">Venta</th>
                <th className="px-4 py-3">Fecha</th>
                <th className="px-4 py-3">Items</th>
              </tr>
            </thead>
            <tbody>
              {sales.map((sale) => (
                <tr key={sale.id} className="border-t border-zinc-100">
                  <td className="px-4 py-3">{sale.id}</td>
                  <td className="px-4 py-3">
                    {new Date(sale.createdAt).toLocaleString()}
                  </td>
                  <td className="px-4 py-3">
                    {sale.items.map((item) => (
                      <div key={`${sale.id}-${item.productId}`}>
                        {item.productId} x {item.quantity}
                      </div>
                    ))}
                  </td>
                </tr>
              ))}
              {sales.length === 0 && (
                <tr>
                  <td
                    colSpan={3}
                    className="px-4 py-6 text-center text-sm text-zinc-500"
                  >
                    No hay ventas registradas.
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
