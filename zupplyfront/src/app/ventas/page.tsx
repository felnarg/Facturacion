"use client";

import { useEffect, useState, useRef } from "react";
import { apiRequest } from "@/lib/api";
import { useAuth } from "@/components/AuthProvider";
import { Protected } from "@/components/Protected";
import { DevBlockHeader } from "@/components/DevBlockHeader";
import { ResizableInputGroup } from "@/components/ResizableInputGroup";

type Product = {
  id: string;
  name: string;
  price: number;
  stock: number;
  salePercentage: number;
  wholesaleSalePercentage: number;
  specialSalePercentage: number;
  iva: number;
  internalProductCode: number;
  consumptionTaxPercentage?: number;
};

type SaleItem = {
  productId: string;
  productName: string;
  quantity: number;
  unitValue: number;
  total: number;
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

  // Form State
  const [productName, setProductName] = useState("");
  const [quantity, setQuantity] = useState("");
  const [unitValue, setUnitValue] = useState(0);
  const [total, setTotal] = useState(0);
  const [saleType, setSaleType] = useState<'detal' | 'mayor' | 'especial'>('detal');
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);

  // Search State
  const [products, setProducts] = useState<Product[]>([]);
  const [isProductModalOpen, setIsProductModalOpen] = useState(false);
  const [productSearch, setProductSearch] = useState("");

  const [error, setError] = useState("");

  // Refs
  const quantityInputRef = useRef<HTMLInputElement>(null);
  const productInputRef = useRef<HTMLInputElement>(null);
  const searchInputRef = useRef<HTMLInputElement>(null);

  const loadSales = async () => {
    const data = await apiRequest<Sale[]>(
      "/api/sales",
      undefined,
      user.token
    );
    setSales(data);
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
    loadSales().catch((err) =>
      setError(err instanceof Error ? err.message : "Error cargando ventas")
    );
  }, []);

  // Search Modal Effect
  useEffect(() => {
    if (isProductModalOpen) {
      searchInputRef.current?.focus();
      loadProducts(productSearch).catch(() => { });
    }
  }, [isProductModalOpen, productSearch]);

  // Price Calculation Logic
  useEffect(() => {
    if (!selectedProduct) {
      setUnitValue(0);
      setTotal(0);
      return;
    }

    const qty = Number(quantity);
    if (!qty || qty <= 0) {
      setUnitValue(0);
      setTotal(0);
      return;
    }

    // Determine margin based on saleType selector
    let margin = selectedProduct.salePercentage || 0;
    if (saleType === 'especial' && selectedProduct.specialSalePercentage > 0) {
      margin = selectedProduct.specialSalePercentage;
    } else if (saleType === 'mayor' && selectedProduct.wholesaleSalePercentage > 0) {
      margin = selectedProduct.wholesaleSalePercentage;
    }

    // Calculate Price: Cost * (1 + ConsumptionTax) * (1 + IVA) * (1 + Margin)
    const basePrice = selectedProduct.price;

    const consumptionTax = (selectedProduct.consumptionTaxPercentage ?? 0) / 100;
    const iva = (selectedProduct.iva ?? 19) / 100;
    const saleMargin = margin / 100;

    const priceWithTaxesAndMargin = basePrice * (1 + consumptionTax) * (1 + iva) * (1 + saleMargin);

    setUnitValue(priceWithTaxesAndMargin);
    setTotal(priceWithTaxesAndMargin * qty);


  }, [quantity, selectedProduct, saleType]);

  const handleSearchKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === "Enter" || e.key === "Tab") {
      e.preventDefault();
      setProductSearch(productName);
      setIsProductModalOpen(true);
    }
  };

  const handleSelectProduct = (product: Product) => {
    setSelectedProduct(product);
    setProductName(product.name);
    setQuantity("1"); // Default to 1
    setIsProductModalOpen(false);
    setSaleType('detal'); // Reset to default
    setTimeout(() => quantityInputRef.current?.focus(), 0);
  };

  const addItem = () => {
    if (!selectedProduct || !quantity || Number(quantity) <= 0) {
      return;
    }

    setItems((prev) => [
      ...prev,
      {
        productId: selectedProduct.id,
        productName: selectedProduct.name,
        quantity: Number(quantity),
        unitValue: unitValue,
        total: total,
      },
    ]);

    // Reset form
    setProductName("");
    setQuantity("");
    setSelectedProduct(null);
    setUnitValue(0);
    setTotal(0);
    setSaleType('detal');

    // Focus back to product input
    setTimeout(() => productInputRef.current?.focus(), 0);
  };

  const handleQuantityKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === "Enter") {
      e.preventDefault();
      addItem();
    }
  };

  const handleCreateSale = async () => {
    setError("");
    try {
      await apiRequest<Sale>(
        "/api/sales",
        {
          method: "POST",
          body: JSON.stringify({
            items: items.map(i => ({ productId: i.productId, quantity: i.quantity }))
          }),
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
      <div className="space-y-6 relative">
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

          <div className="mt-4 flex flex-col gap-4">
            {/* Row 1: Type of Sale (Global) */}
            <div className="w-full md:w-1/4">
              <label className="mb-1 block text-xs font-medium text-zinc-500">
                Tipo Venta
              </label>
              <select
                className="w-full rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
                value={saleType}
                onChange={(e) => setSaleType(e.target.value as 'detal' | 'mayor' | 'especial')}
              >
                <option value="detal">Detal</option>
                <option value="mayor">Mayor</option>
                <option value="especial">Especial</option>
              </select>
            </div>

            {/* Row 2: Item Inputs */}
            {/* Row 2: Item Inputs */}
            <ResizableInputGroup
              columns={[
                {
                  id: "product",
                  label: "Producto",
                  initialWidth: 220,
                  content: (
                    <input
                      ref={productInputRef}
                      className="w-full rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
                      placeholder="Buscar (Enter)"
                      value={productName}
                      onChange={(e) => setProductName(e.target.value)}
                      onKeyDown={handleSearchKeyDown}
                    />
                  ),
                },
                {
                  id: "qty",
                  label: "Cantidad",
                  initialWidth: 100,
                  content: (
                    <input
                      ref={quantityInputRef}
                      className="w-full rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
                      placeholder="0"
                      type="number"
                      min="1"
                      value={quantity}
                      onChange={(e) => setQuantity(e.target.value)}
                      onKeyDown={handleQuantityKeyDown}
                    />
                  ),
                },
                {
                  id: "unit",
                  label: "Valor Unitario",
                  initialWidth: 140,
                  content: (
                    <input
                      className="w-full rounded-md border border-zinc-200 bg-zinc-50 px-3 py-2 text-sm text-zinc-500 cursor-not-allowed"
                      value={unitValue ? unitValue.toFixed(2) : ""}
                      readOnly
                      tabIndex={-1}
                    />
                  ),
                },
                {
                  id: "total",
                  label: "Total",
                  initialWidth: 140,
                  content: (
                    <input
                      className="w-full rounded-md border border-zinc-200 bg-zinc-50 px-3 py-2 text-sm text-zinc-500 cursor-not-allowed"
                      value={total ? total.toFixed(2) : ""}
                      readOnly
                      tabIndex={-1}
                    />
                  ),
                },
              ]}
            />
          </div>

          {/* Items List */}
          <div className="mt-4 border rounded-md border-zinc-200 overflow-hidden">
            <table className="w-full text-sm">
              <thead className="bg-zinc-50 text-left text-xs uppercase text-zinc-500">
                <tr>
                  <th className="px-3 py-2">Producto</th>
                  <th className="px-3 py-2">Cant</th>
                  <th className="px-3 py-2">Unitario</th>
                  <th className="px-3 py-2">Total</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-zinc-100">
                {items.map((item, index) => (
                  <tr key={`${item.productId}-${index}`}>
                    <td className="px-3 py-2 font-medium text-zinc-900">{item.productName || item.productId}</td>
                    <td className="px-3 py-2 font-medium text-zinc-900">{item.quantity}</td>
                    <td className="px-3 py-2 font-medium text-zinc-900">{item.unitValue?.toFixed(2)}</td>
                    <td className="px-3 py-2 font-medium text-zinc-900">{item.total?.toFixed(2)}</td>
                  </tr>
                ))}
                {items.length === 0 && (
                  <tr>
                    <td colSpan={4} className="px-3 py-4 text-center text-zinc-500">
                      Agrega items a la venta.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
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
                <th className="px-4 py-3">Producto</th>
                <th className="px-4 py-3">Cantidad</th>
                <th className="px-4 py-3">Venta</th>
                <th className="px-4 py-3">Fecha</th>
              </tr>
            </thead>
            <tbody>
              {sales.flatMap((sale) =>
                sale.items.map((item, index) => (
                  <tr key={`${sale.id}-${index}`} className="border-t border-zinc-100">
                    <td className="px-4 py-3">
                      {item.productName || item.productId}
                    </td>
                    <td className="px-4 py-3">
                      {item.quantity}
                    </td>
                    <td className="px-4 py-3">
                      {sale.id.slice(0, 8)}...
                    </td>
                    <td className="px-4 py-3">
                      {new Date(sale.createdAt).toLocaleDateString()} {new Date(sale.createdAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                    </td>
                  </tr>
                ))
              )}
              {sales.length === 0 && (
                <tr>
                  <td
                    colSpan={4}
                    className="px-4 py-6 text-center text-sm text-zinc-500"
                  >
                    No hay ventas registradas.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>

        {/* Product Search Modal */}
        {isProductModalOpen && (
          <div className="fixed inset-0 z-50 flex items-start justify-center bg-black/20 p-4 pt-[10vh] backdrop-blur-sm">
            <div className="w-full max-w-lg rounded-xl bg-white p-4 shadow-xl ring-1 ring-zinc-900/5">
              <div className="mb-3">
                <input
                  ref={searchInputRef}
                  className="w-full rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
                  placeholder="Buscar producto..."
                  value={productSearch}
                  onChange={(e) => setProductSearch(e.target.value)}
                  onKeyDown={(e) => {
                    if (e.key === "Escape") {
                      setIsProductModalOpen(false);
                      productInputRef.current?.focus();
                    }
                  }}
                />
              </div>
              <div className="max-h-[300px] overflow-y-auto">
                {products.length === 0 ? (
                  <p className="p-2 text-center text-sm text-zinc-500">
                    No se encontraron productos.
                  </p>
                ) : (
                  <ul className="divide-y divide-zinc-100">
                    {products.map((product) => (
                      <li
                        key={product.id}
                        className="cursor-pointer px-3 py-2 hover:bg-zinc-50"
                        onClick={() => handleSelectProduct(product)}
                      >
                        <div className="font-medium text-zinc-900">
                          {product.name}
                        </div>
                        <div className="text-xs text-zinc-500">
                          Cod: {product.internalProductCode} | Stock: {product.stock}
                        </div>
                      </li>
                    ))}
                  </ul>
                )}
              </div>
            </div>
          </div>
        )}
      </div>
    </Protected>
  );
}
