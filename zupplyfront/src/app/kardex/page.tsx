"use client";

import { useEffect, useState } from "react";
import { apiRequest } from "@/lib/api";
import { useAuth } from "@/components/AuthProvider";
import { Protected } from "@/components/Protected";
import { DevBlockHeader } from "@/components/DevBlockHeader";

type CreditAccount = {
  id: string;
  customerName: string;
  identificationType: "CC" | "NIT";
  identificationNumber: string;
  creditLimit: number;
  paymentTermDays: number;
  currentBalance: number;
  availableCredit: number;
  createdAt: string;
  updatedAt: string;
};

type CreditMovement = {
  id: string;
  creditAccountId: string;
  saleId?: string | null;
  amount: number;
  dueDate: string;
  status: string;
  createdAt: string;
};

const emptyForm = {
  customerName: "",
  identificationType: "CC" as "CC" | "NIT",
  identificationNumber: "",
  creditLimit: "",
  paymentTermDays: "",
};

export default function KardexPage() {
  const { user } = useAuth();
  const [accounts, setAccounts] = useState<CreditAccount[]>([]);
  const [movements, setMovements] = useState<CreditMovement[]>([]);
  const [form, setForm] = useState({ ...emptyForm });
  const [editing, setEditing] = useState<CreditAccount | null>(null);
  const [selectedAccountId, setSelectedAccountId] = useState<string>("");
  const [saleAmount, setSaleAmount] = useState("");
  const [saleId, setSaleId] = useState("");
  const [search, setSearch] = useState("");
  const [error, setError] = useState("");

  const loadAccounts = async (term?: string) => {
    const query = term ? `?search=${encodeURIComponent(term)}` : "";
    const data = await apiRequest<CreditAccount[]>(
      `/api/kardex/credit-accounts${query}`,
      undefined,
      user.token
    );
    setAccounts(data);
  };

  const loadMovements = async (accountId: string) => {
    const data = await apiRequest<CreditMovement[]>(
      `/api/kardex/credit-accounts/${accountId}/movements`,
      undefined,
      user.token
    );
    setMovements(data);
  };

  useEffect(() => {
    loadAccounts().catch((err) =>
      setError(err instanceof Error ? err.message : "Error cargando créditos")
    );
  }, []);

  useEffect(() => {
    const handler = setTimeout(() => {
      loadAccounts(search).catch((err) =>
        setError(err instanceof Error ? err.message : "Error filtrando créditos")
      );
    }, 250);

    return () => clearTimeout(handler);
  }, [search]);

  useEffect(() => {
    if (!selectedAccountId) {
      setMovements([]);
      return;
    }
    loadMovements(selectedAccountId).catch((err) =>
      setError(err instanceof Error ? err.message : "Error cargando movimientos")
    );
  }, [selectedAccountId]);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError("");

    const payload = {
      customerName: form.customerName,
      identificationType: form.identificationType,
      identificationNumber: form.identificationNumber,
      creditLimit: Number(form.creditLimit),
      paymentTermDays: Number(form.paymentTermDays),
    };

    try {
      if (editing) {
        await apiRequest(
          `/api/kardex/credit-accounts/${editing.id}`,
          { method: "PUT", body: JSON.stringify(payload) },
          user.token
        );
        setEditing(null);
      } else {
        await apiRequest(
          "/api/kardex/credit-accounts",
          { method: "POST", body: JSON.stringify(payload) },
          user.token
        );
      }

      setForm({ ...emptyForm });
      await loadAccounts(search);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error guardando crédito");
    }
  };

  const handleEdit = (account: CreditAccount) => {
    setEditing(account);
    setForm({
      customerName: account.customerName,
      identificationType: account.identificationType,
      identificationNumber: account.identificationNumber,
      creditLimit: String(account.creditLimit),
      paymentTermDays: String(account.paymentTermDays),
    });
  };

  const handleDelete = async (id: string) => {
    if (!confirm("¿Seguro que deseas eliminar este crédito?")) {
      return;
    }

    await apiRequest(`/api/kardex/credit-accounts/${id}`, { method: "DELETE" }, user.token);
    await loadAccounts(search);
  };

  const handleRegisterCreditSale = async () => {
    if (!selectedAccountId) {
      setError("Debes seleccionar un cliente para registrar el crédito.");
      return;
    }

    if (!saleAmount || Number(saleAmount) <= 0) {
      setError("El monto del crédito debe ser válido.");
      return;
    }

    try {
      await apiRequest(
        `/api/kardex/credit-accounts/${selectedAccountId}/credit-sales`,
        {
          method: "POST",
          body: JSON.stringify({
            amount: Number(saleAmount),
            saleId: saleId ? saleId : null,
            occurredOn: new Date().toISOString(),
          }),
        },
        user.token
      );
      setSaleAmount("");
      setSaleId("");
      await loadAccounts(search);
      await loadMovements(selectedAccountId);
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Error registrando venta a crédito"
      );
    }
  };

  return (
    <Protected module="kardex">
      <div className="space-y-6">
        <header>
          <h2 className="text-xl font-semibold text-zinc-900">Kardex</h2>
          <p className="text-sm text-zinc-500">
            Gestiona cupos, periodos de pago y ventas a crédito.
          </p>
        </header>

        <div className="dev-block-container rounded-xl bg-white p-4 shadow-sm">
          <DevBlockHeader label="cobalto" />
          <input
            className="w-full rounded-md border border-zinc-200 px-3 py-2 text-sm"
            placeholder="Buscar por cliente o identificación"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
          />
        </div>

        <form
          onSubmit={handleSubmit}
          className="dev-block-container grid gap-3 rounded-xl bg-white p-4 shadow-sm md:grid-cols-2"
        >
          <DevBlockHeader label="lapis" />
          <input
            className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
            placeholder="Nombre cliente/empresa"
            value={form.customerName}
            onChange={(event) =>
              setForm((prev) => ({ ...prev, customerName: event.target.value }))
            }
            required
          />
          <select
            className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
            value={form.identificationType}
            onChange={(event) =>
              setForm((prev) => ({
                ...prev,
                identificationType: event.target.value as "CC" | "NIT",
              }))
            }
          >
            <option value="CC">CC</option>
            <option value="NIT">NIT</option>
          </select>
          <input
            className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
            placeholder="Número identificación"
            value={form.identificationNumber}
            onChange={(event) =>
              setForm((prev) => ({
                ...prev,
                identificationNumber: event.target.value,
              }))
            }
            required
          />
          <input
            className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
            placeholder="Cupo"
            type="number"
            value={form.creditLimit}
            onChange={(event) =>
              setForm((prev) => ({ ...prev, creditLimit: event.target.value }))
            }
            required
          />
          <input
            className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
            placeholder="Días de pago"
            type="number"
            value={form.paymentTermDays}
            onChange={(event) =>
              setForm((prev) => ({
                ...prev,
                paymentTermDays: event.target.value,
              }))
            }
            required
          />
          <div className="md:col-span-2 flex items-center gap-3">
            <button className="btn-primary">
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

        <div className="dev-block-container rounded-xl border border-zinc-200 bg-white p-4">
          <DevBlockHeader label="grafito" />
          <h3 className="text-sm font-semibold text-zinc-700">
            Registrar venta a crédito
          </h3>
          <div className="mt-3 grid gap-3 md:grid-cols-3">
            <select
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
              value={selectedAccountId}
              onChange={(event) => setSelectedAccountId(event.target.value)}
            >
              <option value="">Selecciona un cliente</option>
              {accounts.map((account) => (
                <option key={account.id} value={account.id}>
                  {account.customerName} ({account.identificationType}{" "}
                  {account.identificationNumber})
                </option>
              ))}
            </select>
            <input
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
              placeholder="Monto"
              type="number"
              value={saleAmount}
              onChange={(event) => setSaleAmount(event.target.value)}
            />
            <input
              className="rounded-md border border-zinc-200 px-3 py-2 text-sm"
              placeholder="Venta ID (opcional)"
              value={saleId}
              onChange={(event) => setSaleId(event.target.value)}
            />
          </div>
          <button onClick={handleRegisterCreditSale} className="btn-secondary mt-3">
            Registrar crédito
          </button>
        </div>

        <div className="dev-block-container overflow-hidden rounded-xl border border-zinc-200 bg-white">
          <DevBlockHeader label="tiza" />
          <table className="w-full text-sm">
            <thead className="bg-zinc-50 text-left text-xs uppercase text-zinc-500">
              <tr>
                <th className="px-4 py-3">Cliente</th>
                <th className="px-4 py-3">Identificación</th>
                <th className="px-4 py-3">Cupo</th>
                <th className="px-4 py-3">Saldo</th>
                <th className="px-4 py-3">Disponible</th>
                <th className="px-4 py-3">Plazo</th>
                <th className="px-4 py-3"></th>
              </tr>
            </thead>
            <tbody>
              {accounts.map((account) => (
                <tr key={account.id} className="border-t border-zinc-100">
                  <td className="px-4 py-3">{account.customerName}</td>
                  <td className="px-4 py-3">
                    {account.identificationType} {account.identificationNumber}
                  </td>
                  <td className="px-4 py-3">{account.creditLimit.toFixed(2)}</td>
                  <td className="px-4 py-3">{account.currentBalance.toFixed(2)}</td>
                  <td className="px-4 py-3">{account.availableCredit.toFixed(2)}</td>
                  <td className="px-4 py-3">{account.paymentTermDays} días</td>
                  <td className="px-4 py-3 text-right">
                    <button
                      onClick={() => {
                        handleEdit(account);
                        setSelectedAccountId(account.id);
                      }}
                      className="mr-2 text-xs text-blue-600"
                    >
                      Editar
                    </button>
                    <button
                      onClick={() => handleDelete(account.id)}
                      className="text-xs text-rose-600"
                    >
                      Eliminar
                    </button>
                  </td>
                </tr>
              ))}
              {accounts.length === 0 && (
                <tr>
                  <td
                    colSpan={7}
                    className="px-4 py-6 text-center text-sm text-zinc-500"
                  >
                    No hay créditos registrados.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>

        {selectedAccountId && (
          <div className="dev-block-container overflow-hidden rounded-xl border border-zinc-200 bg-white">
            <DevBlockHeader label="pizarra" />
            <table className="w-full text-sm">
              <thead className="bg-zinc-50 text-left text-xs uppercase text-zinc-500">
                <tr>
                  <th className="px-4 py-3">Monto</th>
                  <th className="px-4 py-3">Vence</th>
                  <th className="px-4 py-3">Estado</th>
                  <th className="px-4 py-3">Venta</th>
                </tr>
              </thead>
              <tbody>
                {movements.map((movement) => (
                  <tr key={movement.id} className="border-t border-zinc-100">
                    <td className="px-4 py-3">{movement.amount.toFixed(2)}</td>
                    <td className="px-4 py-3">
                      {new Date(movement.dueDate).toLocaleDateString()}
                    </td>
                    <td className="px-4 py-3">{movement.status}</td>
                    <td className="px-4 py-3">{movement.saleId ?? "-"}</td>
                  </tr>
                ))}
                {movements.length === 0 && (
                  <tr>
                    <td
                      colSpan={4}
                      className="px-4 py-6 text-center text-sm text-zinc-500"
                    >
                      No hay movimientos para este cliente.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </Protected>
  );
}
