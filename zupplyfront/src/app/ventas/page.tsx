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
  const [items, setItems] = useState<SaleItem[]>([]);

  // Form State
  const [productName, setProductName] = useState("");
  const [quantity, setQuantity] = useState("");
  const [unitValue, setUnitValue] = useState(0);
  const [total, setTotal] = useState(0);
  const [saleType, setSaleType] = useState<'detal' | 'mayor' | 'especial'>('detal');
  const [paymentMethod, setPaymentMethod] = useState<'efectivo' | 'credito' | 'datafono'>('efectivo');
  const [amountTendered, setAmountTendered] = useState("");
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);

  // Search State
  const [products, setProducts] = useState<Product[]>([]);
  const [isProductModalOpen, setIsProductModalOpen] = useState(false);
  const [isPaymentModalOpen, setIsPaymentModalOpen] = useState(false);
  const [productSearch, setProductSearch] = useState("");

  const [error, setError] = useState("");

  // Refs
  const quantityInputRef = useRef<HTMLInputElement>(null);
  const productInputRef = useRef<HTMLInputElement>(null);
  const searchInputRef = useRef<HTMLInputElement>(null);
  const paymentInputRef = useRef<HTMLInputElement>(null);



  // Search Modal Effect
  useEffect(() => {
    if (isProductModalOpen) {
      searchInputRef.current?.focus();
      loadProducts(productSearch).catch(() => { });
    }
  }, [isProductModalOpen, productSearch]);

  // Payment Modal Focus Effect
  useEffect(() => {
    if (isPaymentModalOpen) {
      setTimeout(() => paymentInputRef.current?.focus(), 0);
    }
  }, [isPaymentModalOpen]);

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
    if (e.key === "Enter") {
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

  const grandTotal = items.reduce((sum, item) => sum + item.total, 0);

  // Rounding Helper (Updated to Ceiling)
  const roundToNearest50 = (value: number) => {
    return Math.ceil(value / 50) * 50;
  };

  const roundedTotal = roundToNearest50(grandTotal);

  const loadProducts = async (search?: string) => {
    const query = search ? `?search=${encodeURIComponent(search)}` : "";
    const data = await apiRequest<Product[]>(
      `/api/catalog/products${query}`,
      undefined,
      user.token
    );
    setProducts(data);
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
    setAmountTendered(""); // Reset tendered amount? Optional, maybe keep it. Resetting for now.
    setSelectedProduct(null);
    setUnitValue(0);
    setTotal(0);
    setSaleType('detal');

    // Focus back to product input
    setTimeout(() => productInputRef.current?.focus(), 0);
  };

  const handleQuantityKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === "Enter" || e.key === "Tab") {
      e.preventDefault();
      addItem();
    }
  };

  const handleCreateSale = () => {
    if (items.length === 0) return;
    setError("");
    setIsPaymentModalOpen(true);
  };

  const confirmSale = async () => {
    if (Number(amountTendered) < roundedTotal) {
      // Prevent completing if payment is insufficient
      return;
    }
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
      setIsPaymentModalOpen(false);
      setAmountTendered("");

      // Optional: Show success message or toast
      // await loadSales(); // Removed per previous step but usually re-fetching is good practice or just rely on local state if avoiding history table
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error creando venta");
      setIsPaymentModalOpen(false);
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
            {/* Row 1: Type of Sale & Payment Method (Global) & Total */}
            <div className="flex w-full items-end justify-between gap-4">
              <div className="flex w-full md:w-1/2 gap-4">
                <div className="w-1/2">
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
                <div className="w-1/2">
                  <label className="mb-1 block text-xs font-medium text-zinc-500">
                    Método Pago
                  </label>
                  <select
                    className="w-full rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
                    value={paymentMethod}
                    onChange={(e) => setPaymentMethod(e.target.value as 'efectivo' | 'credito' | 'datafono')}
                  >
                    <option value="efectivo">Efectivo</option>
                    <option value="credito">Crédito</option>
                    <option value="datafono">Datáfono</option>
                  </select>
                </div>
              </div>

              {/* Grand Total Display */}
              <div className="flex flex-col items-end mr-50">
                <span className="text-xs font-medium uppercase text-zinc-500 tracking-wider">Total Venta</span>
                <span className="text-3xl font-bold text-zinc-900 drop-shadow-sm">
                  ${grandTotal.toLocaleString('es-CO', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}
                </span>
              </div>
            </div>


            {/* Row 2: Item Inputs */}
            {/* Row 2: Item Inputs */}
            <ResizableInputGroup
              columns={[
                {
                  id: "product",
                  label: "Producto",
                  initialWidth: 650,
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
                  <th className="px-3 py-2">Subtotal</th>
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

        {/* Payment Confirmation Modal */}
        {isPaymentModalOpen && (
          <div className="fixed inset-0 z-[60] flex items-center justify-center bg-black/40 p-4 backdrop-blur-sm">
            <div className="w-full max-w-md rounded-xl bg-white p-6 shadow-2xl ring-1 ring-zinc-900/5">
              <h3 className="mb-4 text-lg font-bold text-zinc-900">
                Confirmar Venta
              </h3>

              <div className="space-y-4">
                <div className="flex justify-between border-b border-zinc-100 pb-2">
                  <span className="text-zinc-500">Total a Pagar (Redondeado)</span>
                  <span className="text-xl font-bold text-zinc-900">
                    ${roundedTotal.toLocaleString('es-CO', { minimumFractionDigits: 0, maximumFractionDigits: 0 })}
                  </span>
                </div>

                <div>
                  <label className="mb-1 block text-sm font-medium text-zinc-700">
                    Paga con:
                  </label>
                  <input
                    ref={paymentInputRef}
                    className="w-full rounded-md border border-zinc-300 px-3 py-2 text-lg font-medium text-zinc-900 focus:border-zinc-500 focus:outline-none focus:ring-1 focus:ring-zinc-500"
                    type="number"
                    placeholder="Ingrese valor"
                    value={amountTendered}
                    onChange={(e) => setAmountTendered(e.target.value)}
                    onKeyDown={(e) => {
                      if (e.key === "Enter") {
                        if (Number(amountTendered) >= roundedTotal) {
                          confirmSale();
                        }
                      }
                    }}
                  />
                </div>

                <div className="flex justify-between border-t border-zinc-100 pt-3">
                  <span className="text-zinc-500">Cambio / Devuelta</span>
                  <span className={`text-xl font-bold ${(Number(amountTendered) - roundedTotal) < 0 ? 'text-rose-600' : 'text-emerald-600'}`}>
                    ${(Number(amountTendered) - roundedTotal).toLocaleString('es-CO', { minimumFractionDigits: 0, maximumFractionDigits: 0 })}
                  </span>
                </div>
              </div>

              <div className="mt-6 flex gap-3">
                <button
                  onClick={() => setIsPaymentModalOpen(false)}
                  className="w-full rounded-md border border-zinc-300 bg-white px-4 py-2 text-sm font-medium text-zinc-700 hover:bg-zinc-50 focus:outline-none focus:ring-2 focus:ring-zinc-500/20"
                >
                  Cancelar
                </button>
                <button
                  onClick={confirmSale}
                  disabled={Number(amountTendered) < roundedTotal}
                  className={`w-full rounded-md px-4 py-2 text-sm font-medium text-white focus:outline-none focus:ring-2 focus:ring-zinc-900/20 ${Number(amountTendered) < roundedTotal ? 'bg-zinc-400 cursor-not-allowed' : 'bg-zinc-900 hover:bg-zinc-800'}`}
                >
                  Aceptar
                </button>
              </div>
            </div>
          </div>
        )}
      </div>
    </Protected>
  );
}
