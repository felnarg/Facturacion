"use client";

import { useEffect, useState } from "react";
import { apiRequest } from "@/lib/api";
import { useAuth } from "@/components/AuthProvider";
import { Protected } from "@/components/Protected";
import { DevBlockHeader } from "@/components/DevBlockHeader";
import { ResizableInputGroup } from "@/components/ResizableInputGroup";

type Purchase = {
  id: string;
  productId: string;
  internalProductCode: number;
  productName: string;
  supplierId: string;
  quantity: number;
  supplierName: string;
  supplierInvoiceNumber: string;
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
};

type PurchaseItem = {
  productId: string;
  internalProductCode: number;
  productName: string;
  quantity: number;
  cost: number;
  salePercent: number;
  originalSalePercent: number;
};

export default function ComprasPage() {
  const { user } = useAuth();
  const [purchases, setPurchases] = useState<Purchase[]>([]);
  const [suppliers, setSuppliers] = useState<Supplier[]>([]);
  const [products, setProducts] = useState<Product[]>([]);
  const [items, setItems] = useState<PurchaseItem[]>([]);
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);
  const [form, setForm] = useState({
    productId: "",
    internalProductCode: "",
    productName: "",
    quantity: "",
    cost: "",
    salePercent: "",
    originalSalePercent: "",
    consumptionTax: "",
    wholesaleSale: "",
    specialSale: "",
    stock: "",
    iva: "19",
    productCreatedAt: "",
    supplierId: "",
    supplierName: "",
    supplierInvoiceNumber: "",
  });
  const [productModalOpen, setProductModalOpen] = useState(false);
  const [productSearch, setProductSearch] = useState("");
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

  const loadProducts = async (search?: string) => {
    const query = search ? `?search=${encodeURIComponent(search)}` : "";
    const data = await apiRequest<Product[]>(
      `/api/catalog/products${query}`,
      undefined,
      user.token
    );
    setProducts(data);
    return data;
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

  const handleCreate = async (event: React.FormEvent) => {
    event.preventDefault();
    setError("");

    try {
      if (items.length === 0) {
        setError("Debes agregar al menos un producto.");
        return;
      }

      const invalidItem = items.find((item) => item.internalProductCode <= 0);
      if (invalidItem) {
        setError("Revisa el código interno de los productos agregados.");
        return;
      }

      for (const item of items) {
        await apiRequest<Purchase>(
          "/api/purchases",
          {
            method: "POST",
            body: JSON.stringify({
              productId: item.productId,
              internalProductCode: item.internalProductCode,
              productName: item.productName,
              quantity: item.quantity,
              supplierId: form.supplierId,
              supplierInvoiceNumber: form.supplierInvoiceNumber,
              salePercentage: item.salePercent,
              originalSalePercentage: item.originalSalePercent,
            }),
          },
          user.token
        );
      }
      setForm({
        productId: "",
        internalProductCode: "",
        productName: "",
        quantity: "",
        cost: "",
        salePercent: "",
        originalSalePercent: "",
        consumptionTax: "",
        wholesaleSale: "",
        specialSale: "",
        stock: "",
        iva: "19",
        productCreatedAt: "",
        supplierId: "",
        supplierName: "",
        supplierInvoiceNumber: "",
      });
      setItems([]);
      setSelectedProduct(null);
      await loadPurchases();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error creando compra");
    }
  };

  const handleAddItem = async () => {
    if (!form.productId || !form.internalProductCode || !form.productName) {
      setError("Debes seleccionar un producto válido.");
      return;
    }

    const internalProductCode = Number(form.internalProductCode);
    if (!internalProductCode || internalProductCode <= 0) {
      setError("El código interno es obligatorio.");
      return;
    }

    const quantity = Number(form.quantity);
    const cost = Number(form.cost);

    const salePercent = Number(form.salePercent);
    const originalSalePercent = Number(form.originalSalePercent);

    if (
      !quantity ||
      quantity <= 0 ||
      !cost ||
      cost <= 0
    ) {
      setError("Cantidad y costo deben ser válidos.");
      return;
    }

    setItems((prev) => [
      ...prev,
      {
        productId: form.productId,
        internalProductCode,
        productName: form.productName,
        quantity,
        cost,
        salePercent: salePercent || 0,
        originalSalePercent:
          originalSalePercent && originalSalePercent > 0
            ? originalSalePercent
            : salePercent,
      },
    ]);

    setForm((prev) => ({
      ...prev,
      productId: "",
      internalProductCode: "",
      productName: "",
      quantity: "",
      cost: "",
      salePercent: "",
      originalSalePercent: "",
      consumptionTax: "",
      wholesaleSale: "",
      specialSale: "",
      stock: "",
      iva: "19",
      productCreatedAt: "",
    }));
    setSelectedProduct(null);
  };

  const markReceived = async (id: string) => {
    await apiRequest(
      `/api/purchases/${id}/receive`,
      { method: "PUT" },
      user.token
    );
    await loadPurchases();
  };

  const updateProductInBackend = async () => {
    if (!selectedProduct) return;
    try {
      const payload = {
        name: selectedProduct.name,
        description: selectedProduct.description,
        price: selectedProduct.price,
        stock: Number(form.stock),
        supplierProductCode: selectedProduct.supplierProductCode,
        internalProductCode: selectedProduct.internalProductCode,
        salePercentage: Number(form.salePercent),
        consumptionTaxPercentage: Number(form.consumptionTax),
        wholesaleSalePercentage: Number(form.wholesaleSale),
        specialSalePercentage: Number(form.specialSale),
        iva: Number(form.iva),
      };

      await apiRequest(
        `/api/catalog/products/${selectedProduct.id}`,
        {
          method: "PUT",
          body: JSON.stringify(payload),
        },
        user.token
      );

      // Update our local selectedProduct state just in case
      setSelectedProduct({
        ...selectedProduct,
        stock: payload.stock,
        salePercentage: payload.salePercentage,
        consumptionTaxPercentage: payload.consumptionTaxPercentage,
        wholesaleSalePercentage: payload.wholesaleSalePercentage,
        specialSalePercentage: payload.specialSalePercentage,
        iva: payload.iva
      });

    } catch (err) {
      setError(err instanceof Error ? err.message : "Error actualizando producto");
    }
  };

  const handleKeyDownUpdate = (e: React.KeyboardEvent) => {
    if (e.key === "Enter" || e.key === "Tab") {
      void updateProductInBackend();
    }
  };

  const fillProductForm = (product: Product) => {
    setSelectedProduct(product);
    setForm((prev) => ({
      ...prev,
      productId: product.id,
      productName: product.name,
      salePercent: String(product.salePercentage ?? ""),
      originalSalePercent: String(product.salePercentage ?? ""),
      consumptionTax: String(product.consumptionTaxPercentage ?? ""),
      wholesaleSale: String(product.wholesaleSalePercentage ?? ""),
      specialSale: String(product.specialSalePercentage ?? ""),
      stock: String(product.stock ?? "0"),
      iva: String(product.iva ?? "19"),
      productCreatedAt: product.createdAt ? new Date(product.createdAt).toISOString().split('T')[0] : "",
    }));
  };

  const currentIva = Number(form.iva) / 100;

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
          className="dev-block-container flex flex-col gap-3 rounded-xl bg-white p-4 shadow-sm"
        >
          <DevBlockHeader label="mango" />
          <div className="grid w-full gap-3 md:grid-cols-2">
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
                className="btn-secondary text-xs"
              >
                Seleccionar
              </button>
            </div>
            <div className="flex items-center gap-2">
              <input
                className="w-full rounded-md border border-zinc-200 px-3 py-2 text-sm"
                placeholder="Número de factura"
                value={form.supplierInvoiceNumber}
                onChange={(event) =>
                  setForm((prev) => ({
                    ...prev,
                    supplierInvoiceNumber: event.target.value,
                  }))
                }
                required
              />
            </div>
          </div>

          <ResizableInputGroup
            columns={[
              {
                id: "code",
                label: "Código interno",
                initialWidth: 180,
                content: (
                  <input
                    className="w-full rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
                    placeholder="Código"
                    value={form.internalProductCode ?? ""}
                    onChange={(event) =>
                      setForm((prev) => ({
                        ...prev,
                        internalProductCode: event.target.value,
                      }))
                    }
                    onKeyDown={async (event) => {
                      if (event.key === "Enter") {
                        event.preventDefault();
                        const term = event.currentTarget.value.trim();
                        if (!term) {
                          setProductSearch("");
                          setProductModalOpen(true);
                          loadProducts().catch(() => { });
                          return;
                        }
                        const result = await loadProducts(term);
                        const exact = result.find(
                          (product) => String(product.internalProductCode) === term
                        );
                        if (exact) {
                          fillProductForm(exact);
                        } else {
                          setProductSearch(term);
                          setProductModalOpen(true);
                          loadProducts(term).catch(() => { });
                        }
                      }
                    }}
                  />
                ),
              },
              {
                id: "name",
                label: "Nombre producto",
                initialWidth: 220,
                content: (
                  <input
                    className="w-full rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
                    placeholder="Nombre"
                    value={form.productName ?? ""}
                    onChange={(event) =>
                      setForm((prev) => ({
                        ...prev,
                        productName: event.target.value,
                      }))
                    }
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
                  />
                ),
              },
              {
                id: "qty",
                label: "Cantidad",
                initialWidth: 120,
                content: (
                  <input
                    className="w-full rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
                    placeholder="0"
                    type="number"
                    value={form.quantity ?? ""}
                    onChange={(event) =>
                      setForm((prev) => ({
                        ...prev,
                        quantity: event.target.value,
                      }))
                    }
                  />
                ),
              },
              {
                id: "cost",
                label: "Costo",
                initialWidth: 140,
                content: (
                  <input
                    className="w-full rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
                    placeholder="0.00"
                    type="number"
                    value={form.cost ?? ""}
                    onChange={(event) =>
                      setForm((prev) => ({ ...prev, cost: event.target.value }))
                    }
                    onKeyDown={(event) => {
                      if (event.key === "Enter") {
                        event.preventDefault();
                        void handleAddItem();
                      }
                    }}
                  />
                ),
              },
              {
                id: "val",
                label: "Valor + IVA",
                initialWidth: 140,
                content: (
                  <input
                    className="w-full rounded-md border border-zinc-200 bg-zinc-50 px-3 py-2 text-sm text-zinc-500 cursor-not-allowed"
                    value={
                      form.cost
                        ? (Number(form.cost) * (1 + currentIva)).toFixed(2)
                        : ""
                    }
                    readOnly
                    tabIndex={-1}
                  />
                ),
              },
              {
                id: "total",
                label: "Total",
                initialWidth: 160,
                content: (
                  <input
                    className="w-full rounded-md border border-zinc-200 bg-zinc-50 px-3 py-2 text-sm text-zinc-500 cursor-not-allowed"
                    value={
                      form.cost && form.quantity
                        ? (
                          Number(form.cost) *
                          (1 + currentIva) *
                          Number(form.quantity)
                        ).toFixed(2)
                        : ""
                    }
                    readOnly
                    tabIndex={-1}
                  />
                ),
              },
            ]}
          />

          <div className="grid grid-cols-1 md:grid-cols-4 gap-3">
            <div className="flex flex-col gap-1">
              <label className="text-xs font-semibold text-zinc-500 uppercase">Stock</label>
              <input
                className="w-full rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
                type="number"
                min={0}
                value={form.stock}
                onChange={(e) => setForm(prev => ({ ...prev, stock: e.target.value }))}
                onBlur={() => void updateProductInBackend()}
                onKeyDown={handleKeyDownUpdate}
              />
            </div>
            <div className="flex flex-col gap-1">
              <label className="text-xs font-semibold text-zinc-500 uppercase">% Venta</label>
              <input
                className="w-full rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
                type="number"
                min={0}
                max={100}
                value={form.salePercent}
                onChange={(e) => setForm(prev => ({ ...prev, salePercent: e.target.value }))}
                onBlur={() => void updateProductInBackend()}
                onKeyDown={handleKeyDownUpdate}
              />
            </div>
            <div className="flex flex-col gap-1">
              <label className="text-xs font-semibold text-zinc-500 uppercase">% Venta Mayor</label>
              <input
                className="w-full rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
                type="number"
                min={0}
                max={100}
                value={form.wholesaleSale}
                onChange={(e) => setForm(prev => ({ ...prev, wholesaleSale: e.target.value }))}
                onBlur={() => void updateProductInBackend()}
                onKeyDown={handleKeyDownUpdate}
              />
            </div>
            <div className="flex flex-col gap-1">
              <label className="text-xs font-semibold text-zinc-500 uppercase">% Venta Especial</label>
              <input
                className="w-full rounded-md border border-zinc-200 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-zinc-900/10"
                type="number"
                min={0}
                max={100}
                value={form.specialSale}
                onChange={(e) => setForm(prev => ({ ...prev, specialSale: e.target.value }))}
                onBlur={() => void updateProductInBackend()}
                onKeyDown={handleKeyDownUpdate}
              />
            </div>
          </div>

          <div className="md:col-span-3">
            <button className="btn-primary" disabled={items.length === 0}>
              Registrar compra
            </button>
            <button
              type="button"
              onClick={() => void handleAddItem()}
              className="btn-secondary ml-3"
            >
              Agregar producto
            </button>
          </div>
          {error && (
            <p className="md:col-span-3 text-sm text-rose-600">{error}</p>
          )}
        </form>

        <div className="dev-block-container rounded-xl border border-zinc-200 bg-white p-4">
          <DevBlockHeader label="kiwi" />
          <h3 className="text-sm font-semibold text-zinc-700">
            Productos en la compra
          </h3>
          <div className="mt-3 max-h-[520px] overflow-y-auto">
            <table className="w-full min-w-[900px] text-sm">
              <thead className="sticky top-0 bg-zinc-50 text-left text-xs uppercase text-zinc-500">
                <tr>
                  <th className="px-3 py-2">Producto</th>
                  <th className="px-3 py-2">Cod. Interno</th>
                  <th className="px-3 py-2">Cantidad</th>
                  <th className="px-3 py-2">Costo</th>
                  <th className="px-3 py-2">Valor</th>
                  <th className="px-3 py-2">Total</th>
                  <th className="px-3 py-2">% Venta</th>
                </tr>
              </thead>
              <tbody>
                {items.map((item, index) => {
                  const valor = item.cost * (1 + currentIva);
                  const total = valor * item.quantity;
                  const hasInvalidCode = item.internalProductCode <= 0;
                  return (
                    <tr
                      key={`${item.productId}-${index}`}
                      className={`border-t text-zinc-800 ${hasInvalidCode ? "bg-rose-50 text-rose-700" : ""
                        }`}
                    >
                      <td className="px-3 py-2">{item.productName}</td>
                      <td className="px-3 py-2">
                        {item.internalProductCode}
                        {hasInvalidCode && (
                          <span className="ml-2 text-xs text-rose-600">
                            Código inválido
                          </span>
                        )}
                      </td>
                      <td className="px-3 py-2">{item.quantity}</td>
                      <td className="px-3 py-2">{item.cost.toFixed(2)}</td>
                      <td className="px-3 py-2">{valor.toFixed(2)}</td>
                      <td className="px-3 py-2">{total.toFixed(2)}</td>
                      <td className="px-3 py-2">{item.salePercent}%</td>
                    </tr>
                  );
                })}
                {items.length === 0 && (
                  <tr>
                    <td
                      colSpan={7}
                      className="px-3 py-6 text-center text-xs text-zinc-500"
                    >
                      Aún no has agregado productos.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>

        <div className="dev-block-container overflow-hidden rounded-xl border border-zinc-200 bg-white">
          <DevBlockHeader label="cacao" />
          <table className="w-full text-sm">
            <thead className="bg-zinc-50 text-left text-xs uppercase text-zinc-500">
              <tr>
                <th className="px-4 py-3">Producto</th>
                <th className="px-4 py-3">Cod. Interno</th>
                <th className="px-4 py-3">Cantidad</th>
                <th className="px-4 py-3">Proveedor</th>
                <th className="px-4 py-3">Factura</th>
                <th className="px-4 py-3">Estado</th>
                <th className="px-4 py-3"></th>
              </tr>
            </thead>
            <tbody>
              {purchases.map((purchase) => (
                <tr key={purchase.id} className="border-t border-zinc-100">
                  <td className="px-4 py-3">{purchase.productName}</td>
                  <td className="px-4 py-3">{purchase.internalProductCode}</td>
                  <td className="px-4 py-3">{purchase.quantity}</td>
                  <td className="px-4 py-3">{purchase.supplierName}</td>
                  <td className="px-4 py-3">{purchase.supplierInvoiceNumber}</td>
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
            <div className="dev-block-container w-full max-w-2xl rounded-xl bg-white p-4 shadow-lg">
              <DevBlockHeader label="olivo" />
              <div className="flex items-center justify-between">
                <h3 className="text-sm font-semibold text-zinc-800">
                  Seleccionar proveedor
                </h3>
                <button
                  onClick={() => setSupplierModalOpen(false)}
                  className="btn-secondary text-xs"
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

        {productModalOpen && (
          <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
            <div className="dev-block-container w-full max-w-3xl rounded-xl bg-white p-4 shadow-lg">
              <DevBlockHeader label="cedro" />
              <div className="flex items-center justify-between">
                <h3 className="text-sm font-semibold text-zinc-800">
                  Buscar producto
                </h3>
                <button
                  onClick={() => setProductModalOpen(false)}
                  className="btn-secondary text-xs"
                >
                  Cerrar
                </button>
              </div>

              <div className="mt-3">
                <input
                  className="w-full rounded-md border border-zinc-200 px-3 py-2 text-sm"
                  placeholder="Busca por nombre, códigos o descripción"
                  value={productSearch}
                  onChange={(event) => setProductSearch(event.target.value)}
                />
              </div>

              <div className="mt-4 max-h-64 overflow-auto rounded-md border border-zinc-100">
                {products.map((product) => (
                  <button
                    key={product.id}
                    onClick={() => {
                      fillProductForm(product);
                      setProductModalOpen(false);
                    }}
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
