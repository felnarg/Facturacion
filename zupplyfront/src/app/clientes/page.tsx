"use client";

import { useEffect, useState, useRef } from "react";
import { apiRequest } from "@/lib/api";
import { useAuth } from "@/components/AuthProvider";
import { Protected } from "@/components/Protected";
import { DevBlockHeader } from "@/components/DevBlockHeader";
import { useModalKeyboard } from "@/hooks/useModalKeyboard";

type Customer = {
  id: string;
  name: string;
  email: string;
  city: string;
  phone: string;
  address: string;
  type: "Natural" | "Juridica";
  identificationType: "CC" | "NIT";
  identificationNumber: string;
  isCreditApproved: boolean;
  approvedCreditLimit: number;
  approvedPaymentTermDays: number;
  points: number;
  createdAt: string;
  updatedAt: string;
};

export default function ClientesPage() {
  const { user } = useAuth();
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [customerModalOpen, setCustomerModalOpen] = useState(false);
  const [customerSearch, setCustomerSearch] = useState("");
  const [selectedCustomerIndex, setSelectedCustomerIndex] = useState(-1);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [form, setForm] = useState({
    name: "",
    email: "",
    city: "",
    phone: "",
    address: "",
    type: "Natural" as Customer["type"],
    identificationType: "CC" as Customer["identificationType"],
    identificationNumber: "",
    isCreditApproved: false,
    approvedCreditLimit: "",
    approvedPaymentTermDays: "",
  });
  const [error, setError] = useState("");
  const nameInputRef = useRef<HTMLInputElement>(null);

  const loadCustomers = async (term: string) => {
    const query = term.trim()
      ? `?search=${encodeURIComponent(term.trim())}`
      : "";
    const data = await apiRequest<Customer[]>(
      `/api/customers${query}`,
      undefined,
      user.token
    );
    setCustomers(data);
  };

  useEffect(() => {
    if (!customerModalOpen) return;

    loadCustomers("").catch((err) =>
      setError(err instanceof Error ? err.message : "Error cargando clientes")
    );
  }, [customerModalOpen]);

  useEffect(() => {
    if (!customerModalOpen) return;

    const timeout = setTimeout(() => {
      loadCustomers(customerSearch).catch((err) =>
        setError(
          err instanceof Error ? err.message : "Error cargando clientes"
        )
      );
    }, 250);

    return () => clearTimeout(timeout);
  }, [customerSearch, customerModalOpen]);

  useModalKeyboard({
    isOpen: customerModalOpen,
    onClose: () => {
      setCustomerModalOpen(false);
      setCustomerSearch("");
      setSelectedCustomerIndex(-1);
    },
    returnFocusRef: nameInputRef,
  });

  const resetForm = () => {
    setForm({
      name: "",
      email: "",
      city: "",
      phone: "",
      address: "",
      type: "Natural",
      identificationType: "CC",
      identificationNumber: "",
      isCreditApproved: false,
      approvedCreditLimit: "",
      approvedPaymentTermDays: "",
    });
    setEditingId(null);
  };

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError("");

    try {
      const payload = {
        ...form,
        approvedCreditLimit: form.isCreditApproved
          ? Number(form.approvedCreditLimit || 0)
          : 0,
        approvedPaymentTermDays: form.isCreditApproved
          ? Number(form.approvedPaymentTermDays || 0)
          : 0,
      };

      if (editingId) {
        await apiRequest<Customer>(
          `/api/customers/${editingId}`,
          {
            method: "PUT",
            body: JSON.stringify(payload),
          },
          user.token
        );
      } else {
        await apiRequest<Customer>(
          "/api/customers",
          {
            method: "POST",
            body: JSON.stringify(payload),
          },
          user.token
        );
      }
      resetForm();
      await loadCustomers(customerSearch);
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : editingId
            ? "Error actualizando cliente"
            : "Error creando cliente"
      );
    }
  };

  const handleEdit = (customer: Customer) => {
    setEditingId(customer.id);
    setForm({
      name: customer.name,
      email: customer.email,
      city: customer.city,
      phone: customer.phone,
      address: customer.address,
      type: customer.type,
      identificationType: customer.identificationType,
      identificationNumber: customer.identificationNumber,
      isCreditApproved: customer.isCreditApproved ?? false,
      approvedCreditLimit: String(customer.approvedCreditLimit ?? 0),
      approvedPaymentTermDays: String(customer.approvedPaymentTermDays ?? 0),
    });
  };

  const handleDelete = async (id: string) => {
    const confirmed = window.confirm(
      "¿Deseas eliminar este cliente? Esta acción no se puede deshacer."
    );
    if (!confirmed) return;

    setError("");
    try {
      await apiRequest<void>(
        `/api/customers/${id}`,
        { method: "DELETE" },
        user.token
      );
      if (editingId === id) {
        resetForm();
      }
      await loadCustomers(customerSearch);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error eliminando cliente");
    }
  };

  return (
    <Protected module="clientes">
      <div className="space-y-6">
        <header>
          <h2 className="text-xl font-semibold text-zinc-900">Clientes</h2>
          <p className="text-sm text-zinc-500">
            Gestiona los clientes y sus datos básicos.
          </p>
        </header>

        <form
          onSubmit={handleSubmit}
          className="dev-block-container grid gap-3 rounded-xl bg-white p-4 shadow-sm md:grid-cols-3"
        >
          <DevBlockHeader label="perla" />
          <div className="space-y-1">
            <label className="text-xs text-zinc-500" htmlFor="customer-name">
              Nombre
            </label>
            <input
              id="customer-name"
              ref={nameInputRef}
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
              placeholder="Nombre del cliente"
              value={form.name}
              onChange={(event) =>
                setForm((prev) => ({ ...prev, name: event.target.value }))
              }
              onKeyDown={(event) => {
                if (event.key === "Enter") {
                  event.preventDefault();
                  setCustomerSearch(event.currentTarget.value);
                  setCustomerModalOpen(true);
                  setSelectedCustomerIndex(-1);
                }
              }}
              required
            />
          </div>
          <div className="space-y-1">
            <label className="text-xs text-zinc-500" htmlFor="customer-email">
              Correo
            </label>
            <input
              id="customer-email"
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
              placeholder="Correo"
              value={form.email}
              onChange={(event) =>
                setForm((prev) => ({ ...prev, email: event.target.value }))
              }
              required
            />
          </div>
          <div className="space-y-1">
            <label className="text-xs text-zinc-500" htmlFor="customer-city">
              Ciudad
            </label>
            <input
              id="customer-city"
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
              placeholder="Ciudad"
              value={form.city}
              onChange={(event) =>
                setForm((prev) => ({ ...prev, city: event.target.value }))
              }
              required
            />
          </div>
          <div className="space-y-1">
            <label className="text-xs text-zinc-500" htmlFor="customer-phone">
              Teléfono
            </label>
            <input
              id="customer-phone"
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
              placeholder="Teléfono"
              value={form.phone}
              onChange={(event) =>
                setForm((prev) => ({ ...prev, phone: event.target.value }))
              }
              required
            />
          </div>
          <div className="space-y-1 md:col-span-2">
            <label className="text-xs text-zinc-500" htmlFor="customer-address">
              Dirección
            </label>
            <input
              id="customer-address"
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
              placeholder="Dirección"
              value={form.address}
              onChange={(event) =>
                setForm((prev) => ({ ...prev, address: event.target.value }))
              }
              required
            />
          </div>
          <div className="space-y-1">
            <label className="text-xs text-zinc-500" htmlFor="customer-type">
              Tipo de cliente
            </label>
            <select
              id="customer-type"
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
              value={form.type}
              onChange={(event) =>
                setForm((prev) => ({
                  ...prev,
                  type: event.target.value as Customer["type"],
                }))
              }
            >
              <option value="Natural">Persona natural</option>
              <option value="Juridica">Persona jurídica</option>
            </select>
          </div>
          <div className="space-y-1">
            <label className="text-xs text-zinc-500" htmlFor="customer-id-type">
              Tipo de identificación
            </label>
            <select
              id="customer-id-type"
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
              value={form.identificationType}
              onChange={(event) =>
                setForm((prev) => ({
                  ...prev,
                  identificationType:
                    event.target.value as Customer["identificationType"],
                }))
              }
            >
              <option value="CC">CC</option>
              <option value="NIT">NIT</option>
            </select>
          </div>
          <div className="space-y-1">
            <label className="text-xs text-zinc-500" htmlFor="customer-id">
              Número de identificación
            </label>
            <input
              id="customer-id"
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
              placeholder="Número de identificación"
              value={form.identificationNumber}
              onChange={(event) =>
                setForm((prev) => ({
                  ...prev,
                  identificationNumber: event.target.value,
                }))
              }
              required
            />
          </div>
          <div className="space-y-1">
            <label className="text-xs text-zinc-500" htmlFor="customer-credit">
              Crédito aprobado
            </label>
            <select
              id="customer-credit"
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
              value={form.isCreditApproved ? "true" : "false"}
              onChange={(event) =>
                setForm((prev) => ({
                  ...prev,
                  isCreditApproved: event.target.value === "true",
                  approvedCreditLimit:
                    event.target.value === "true" ? prev.approvedCreditLimit : "",
                  approvedPaymentTermDays:
                    event.target.value === "true"
                      ? prev.approvedPaymentTermDays
                      : "",
                }))
              }
            >
              <option value="false">No aprobado</option>
              <option value="true">Aprobado</option>
            </select>
          </div>
          {form.isCreditApproved && (
            <>
              <div className="space-y-1">
                <label
                  className="text-xs text-zinc-500"
                  htmlFor="customer-credit-limit"
                >
                  Cupo aprobado
                </label>
                <input
                  id="customer-credit-limit"
                  className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
                  placeholder="Cupo aprobado"
                  type="number"
                  min="0"
                  value={form.approvedCreditLimit}
                  onChange={(event) =>
                    setForm((prev) => ({
                      ...prev,
                      approvedCreditLimit: event.target.value,
                    }))
                  }
                  required
                />
              </div>
              <div className="space-y-1">
                <label
                  className="text-xs text-zinc-500"
                  htmlFor="customer-credit-days"
                >
                  Días permitidos
                </label>
                <input
                  id="customer-credit-days"
                  className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
                  placeholder="Días permitidos"
                  type="number"
                  min="1"
                  value={form.approvedPaymentTermDays}
                  onChange={(event) =>
                    setForm((prev) => ({
                      ...prev,
                      approvedPaymentTermDays: event.target.value,
                    }))
                  }
                  required
                />
              </div>
            </>
          )}
          <div className="flex flex-wrap gap-2 md:col-span-3">
            <button className="btn-primary">
              {editingId ? "Guardar cambios" : "Crear cliente"}
            </button>
            {editingId && (
              <>
                <button
                  type="button"
                  onClick={() => handleDelete(editingId)}
                  className="btn-secondary"
                >
                  Eliminar
                </button>
              <button
                type="button"
                onClick={resetForm}
                className="btn-secondary"
              >
                Cancelar
              </button>
              </>
            )}
          </div>
          {error && (
            <p className="md:col-span-3 text-sm text-rose-600">{error}</p>
          )}
        </form>
        {customerModalOpen && (
          <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
            <div className="dev-block-container w-full max-w-3xl rounded-xl bg-white p-4 shadow-lg">
              <DevBlockHeader label="pampa" />
              <div className="flex items-center justify-between">
                <h3 className="text-sm font-semibold text-zinc-800">
                  Buscar cliente
                </h3>
                <button
                  onClick={() => setCustomerModalOpen(false)}
                  className="btn-secondary text-xs"
                >
                  Cerrar
                </button>
              </div>

              <div className="mt-3">
                <input
                  className="w-full rounded-md border border-zinc-200 px-3 py-2 text-sm"
                  placeholder="Busca por nombre, correo, ciudad, teléfono o CC/NIT"
                  value={customerSearch}
                  autoFocus
                  onChange={(event) => {
                    setCustomerSearch(event.target.value);
                    setSelectedCustomerIndex(-1);
                  }}
                  onKeyDown={(event) => {
                    if (
                      event.key === "ArrowDown" ||
                      (event.key === "Tab" && !event.shiftKey)
                    ) {
                      event.preventDefault();
                      setSelectedCustomerIndex((prev) =>
                        prev < customers.length - 1 ? prev + 1 : 0
                      );
                    } else if (
                      event.key === "ArrowUp" ||
                      (event.key === "Tab" && event.shiftKey)
                    ) {
                      event.preventDefault();
                      setSelectedCustomerIndex((prev) =>
                        prev > 0 ? prev - 1 : customers.length - 1
                      );
                    } else if (
                      event.key === "Enter" &&
                      selectedCustomerIndex >= 0
                    ) {
                      event.preventDefault();
                      const customer = customers[selectedCustomerIndex];
                      if (customer) {
                        handleEdit(customer);
                        setCustomerModalOpen(false);
                        setSelectedCustomerIndex(-1);
                        setTimeout(() => nameInputRef.current?.focus(), 0);
                      }
                    }
                  }}
                />
              </div>

              <div className="mt-4 max-h-72 overflow-auto rounded-md border border-zinc-100">
                {customers.map((customer, index) => (
                  <button
                    key={customer.id}
                    onClick={() => {
                      handleEdit(customer);
                      setCustomerModalOpen(false);
                      setSelectedCustomerIndex(-1);
                      setTimeout(() => nameInputRef.current?.focus(), 0);
                    }}
                    className={`flex w-full flex-col gap-1 border-b border-zinc-100 px-3 py-3 text-left text-sm hover:bg-zinc-50 ${index === selectedCustomerIndex ? "bg-zinc-100" : ""}`}
                  >
                    <span className="font-medium text-zinc-900">
                      {customer.name}
                    </span>
                    <span className="text-xs text-zinc-500">
                      {customer.identificationType} {customer.identificationNumber} ·{" "}
                      {customer.city} · {customer.phone}
                    </span>
                    <span className="text-xs text-zinc-400">
                      {customer.email} · {customer.address}
                    </span>
                  </button>
                ))}
                {customers.length === 0 && (
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
