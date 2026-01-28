"use client";

import { useEffect, useState, useRef } from "react";
import { apiRequest } from "@/lib/api";
import { useAuth } from "@/components/AuthProvider";
import { Protected } from "@/components/Protected";
import { DevBlockHeader } from "@/components/DevBlockHeader";

type Stock = {
  productId: string;
  quantity: number;
  createdAt: string;
  updatedAt: string;
};

type Product = {
  id: string;
  name: string;
  description: string;
  price: number;
  supplierProductCode: number;
  internalProductCode: number;
  salePercentage: number;
  consumptionTaxPercentage: number;
  wholesaleSalePercentage: number;
  specialSalePercentage: number;
  iva: number;
  createdAt: string;
};

type StockWithProduct = Stock & {
  productName?: string;
};

export default function InventarioPage() {
  const { user } = useAuth();
  const [stocks, setStocks] = useState<StockWithProduct[]>([]);
  const [products, setProducts] = useState<Product[]>([]);
  const [productId, setProductId] = useState("");
  const [productName, setProductName] = useState("");
  const [currentStock, setCurrentStock] = useState<number | null>(null);
  const [quantity, setQuantity] = useState("");
  const [action, setAction] = useState<"increase" | "decrease">("increase");
  const [error, setError] = useState("");
  const [productModalOpen, setProductModalOpen] = useState(false);
  const [productSearch, setProductSearch] = useState("");

  const productNameInputRef = useRef<HTMLInputElement>(null);
  const searchInputRef = useRef<HTMLInputElement>(null);

  const loadStocks = async () => {
    const data = await apiRequest<Stock[]>(
      "/api/inventory/stocks",
      undefined,
      user.token
    );

    // Fetch product names for each stock
    const stocksWithNames = await Promise.all(
      data.map(async (stock) => {
        try {
          const product = await apiRequest<Product>(
            `/api/catalog/products/${stock.productId}`,
            undefined,
            user.token
          );
          return { ...stock, productName: product.name };
        } catch {
          return { ...stock, productName: "Producto desconocido" };
        }
      })
    );

    setStocks(stocksWithNames);
  };

  const loadProducts = async (search?: string) => {
    const query = search ? `?search=${encodeURIComponent(search)}` : "";
    const data = await apiRequest<Product[]>(
      `/api/catalog/products${query}`,
      undefined,
      user.token
    );
    setProducts(data);
  };

  useEffect(() => {
    loadStocks().catch((err) =>
      setError(err instanceof Error ? err.message : "Error cargando stock")
    );
  }, []);

  useEffect(() => {
    if (!productModalOpen) {
      return;
    }

    const handler = setTimeout(() => {
      loadProducts(productSearch).catch((err) =>
        setError(
          err instanceof Error ? err.message : "Error filtrando productos"
        )
      );
    }, 250);

    return () => clearTimeout(handler);
  }, [productSearch, productModalOpen]);

  useEffect(() => {
    if (productModalOpen && searchInputRef.current) {
      searchInputRef.current.focus();
    }
  }, [productModalOpen]);

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
      setProductName("");
      setCurrentStock(null);
      setQuantity("");
      await loadStocks();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error ajustando stock");
    }
  };

  const selectProduct = async (product: Product) => {
    setProductId(product.id);
    setProductName(product.name);
    setProductModalOpen(false);
    setProductSearch("");

    // Fetch current stock
    try {
      const stockData = await apiRequest<Stock>(
        `/api/inventory/stocks/${product.id}`,
        undefined,
        user.token
      );
      setCurrentStock(stockData.quantity);
    } catch {
      setCurrentStock(0);
    }

    setTimeout(() => productNameInputRef.current?.focus(), 0);
  };

  return (
    <Protected module="inventario">
      <div className="space-y-6">
        <header>
          <h2 className="text-xl font-semibold text-zinc-900">Inventario</h2>
          <p className="text-sm text-zinc-500">
            Consulta stocks y ajusta cantidades.
          </p>
        </header>

        <form
          onSubmit={handleAdjust}
          className="dev-block-container grid gap-3 rounded-xl bg-white p-4 shadow-sm md:grid-cols-4"
        >
          <DevBlockHeader label="salvia" />

          <div className="flex flex-col gap-1">
            <label className="text-[10px] font-bold uppercase tracking-wider text-zinc-500 ml-1">
              Producto
            </label>
            <input
              ref={productNameInputRef}
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
              placeholder="Nombre del producto"
              value={productName}
              onChange={(event) => {
                setProductName(event.target.value);
                if (!event.target.value) {
                  setCurrentStock(null);
                  setProductId("");
                }
              }}
              onKeyDown={(event) => {
                if (event.key === "Enter") {
                  event.preventDefault();
                  const term = event.currentTarget.value.trim();
                  setProductSearch(term);
                  setProductModalOpen(true);
                  if (term) {
                    loadProducts(term).catch(() => { });
                  } else {
                    loadProducts().catch(() => { });
                  }
                }
              }}
              required
            />
          </div>

          <div className="flex flex-col gap-1">
            <label className="text-[10px] font-bold uppercase tracking-wider text-zinc-500 ml-1">
              Stock Actual
            </label>
            <input
              className="rounded-md border border-zinc-200 bg-zinc-50 px-3 py-2 text-sm text-zinc-500 cursor-not-allowed"
              value={currentStock !== null ? currentStock : ""}
              placeholder="-"
              readOnly
              tabIndex={-1}
            />
          </div>

          <div className="flex flex-col gap-1">
            <label className="text-[10px] font-bold uppercase tracking-wider text-zinc-500 ml-1">
              Cantidad
            </label>
            <input
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
              placeholder="0"
              type="number"
              value={quantity}
              onChange={(event) => setQuantity(event.target.value)}
              required
            />
          </div>

          <div className="flex flex-col gap-1">
            <label className="text-[10px] font-bold uppercase tracking-wider text-zinc-500 ml-1">
              Acción
            </label>
            <select
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
              value={action}
              onChange={(event) =>
                setAction(event.target.value as "increase" | "decrease")
              }
            >
              <option value="increase">Aumentar</option>
              <option value="decrease">Disminuir</option>
            </select>
          </div>

          <div className="md:col-span-4">
            <button className="btn-primary">
              Aplicar ajuste
            </button>
          </div>
          {error && (
            <p className="md:col-span-4 text-sm text-rose-600">{error}</p>
          )}
        </form>

        <div className="dev-block-container overflow-hidden rounded-xl border border-zinc-200 bg-white">
          <DevBlockHeader label="trigo" />
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
                  <td className="px-4 py-3">{stock.productName || stock.productId}</td>
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

        {productModalOpen && (
          <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
            <div className="dev-block-container w-full max-w-3xl rounded-xl bg-white p-4 shadow-lg">
              <DevBlockHeader label="roble" />
              <div className="flex items-center justify-between">
                <h3 className="text-sm font-semibold text-zinc-800">
                  Buscar producto
                </h3>
                <button
                  onClick={() => {
                    setProductModalOpen(false);
                    setTimeout(() => productNameInputRef.current?.focus(), 0);
                  }}
                  className="btn-secondary text-xs"
                >
                  Cerrar
                </button>
              </div>

              <div className="mt-3">
                <input
                  ref={searchInputRef}
                  className="w-full rounded-md border border-zinc-200 px-3 py-2 text-sm"
                  placeholder="Busca por nombre, códigos o descripción"
                  value={productSearch}
                  onChange={(event) => setProductSearch(event.target.value)}
                  onKeyDown={(event) => {
                    if (event.key === "Escape") {
                      setProductModalOpen(false);
                      setTimeout(() => productNameInputRef.current?.focus(), 0);
                    }
                  }}
                />
              </div>

              <div className="mt-4 max-h-64 overflow-auto rounded-md border border-zinc-100">
                {products.map((product) => (
                  <button
                    key={product.id}
                    onClick={() => selectProduct(product)}
                    className="flex w-full flex-col gap-1 border-b border-zinc-100 px-3 py-3 text-left text-sm hover:bg-zinc-50"
                  >
                    <span className="font-medium text-zinc-900">
                      {product.name}
                    </span>
                    <span className="text-xs text-zinc-500">
                      Cod. interno: {product.internalProductCode} · Cod. prov:{" "}
                      {product.supplierProductCode}
                    </span>
                    <span className="text-xs text-zinc-400">
                      {product.description}
                    </span>
                  </button>
                ))}
                {products.length === 0 && (
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
