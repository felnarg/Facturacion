"use client";

import { useEffect, useState } from "react";
import { apiRequest } from "@/lib/api";
import { useAuth } from "@/components/AuthProvider";
import { Protected } from "@/components/Protected";

type Stock = {
  productId: string;
  quantity: number;
  createdAt: string;
  updatedAt: string;
};

export default function InventarioPage() {
  const { user } = useAuth();
  const [stocks, setStocks] = useState<Stock[]>([]);
  const [productId, setProductId] = useState("");
  const [quantity, setQuantity] = useState("");
  const [action, setAction] = useState<"increase" | "decrease">("increase");
  const [error, setError] = useState("");

  const loadStocks = async () => {
    const data = await apiRequest<Stock[]>(
      "/api/inventory/stocks",
      undefined,
      user.token
    );
    setStocks(data);
  };

  useEffect(() => {
    loadStocks().catch((err) =>
      setError(err instanceof Error ? err.message : "Error cargando stock")
    );
  }, []);

  const handleAdjust = async (event: React.FormEvent) => {
    event.preventDefault();
    setError("");

    try {
      await apiRequest<Stock>(
        `/api/inventory/stocks/${productId}/${action}`,
        {
          method: "PUT",
          body: JSON.stringify({ quantity: Number(quantity) }),
        },
        user.token
      );
      setProductId("");
      setQuantity("");
      await loadStocks();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error ajustando stock");
    }
  };

  return (
    <Protected permission="inventario">
      <div className="space-y-6">
        <header>
          <h2 className="text-xl font-semibold text-zinc-900">Inventario</h2>
          <p className="text-sm text-zinc-500">
            Consulta stocks y ajusta cantidades.
          </p>
        </header>

        <form
          onSubmit={handleAdjust}
          className="grid gap-3 rounded-xl bg-white p-4 shadow-sm md:grid-cols-3"
        >
          <input
            className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
            placeholder="ProductId"
            value={productId}
            onChange={(event) => setProductId(event.target.value)}
            required
          />
          <input
            className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
            placeholder="Cantidad"
            type="number"
            value={quantity}
            onChange={(event) => setQuantity(event.target.value)}
            required
          />
          <select
            className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
            value={action}
            onChange={(event) =>
              setAction(event.target.value as "increase" | "decrease")
            }
          >
            <option value="increase">Aumentar</option>
            <option value="decrease">Disminuir</option>
          </select>
          <div className="md:col-span-3">
            <button className="rounded-md bg-zinc-900 px-4 py-2 text-sm text-white">
              Aplicar ajuste
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
                <th className="px-4 py-3">Actualizado</th>
              </tr>
            </thead>
            <tbody>
              {stocks.map((stock) => (
                <tr key={stock.productId} className="border-t border-zinc-100">
                  <td className="px-4 py-3">{stock.productId}</td>
                  <td className="px-4 py-3">{stock.quantity}</td>
                  <td className="px-4 py-3">
                    {new Date(stock.updatedAt).toLocaleString()}
                  </td>
                </tr>
              ))}
              {stocks.length === 0 && (
                <tr>
                  <td
                    colSpan={3}
                    className="px-4 py-6 text-center text-sm text-zinc-500"
                  >
                    No hay stocks registrados.
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
