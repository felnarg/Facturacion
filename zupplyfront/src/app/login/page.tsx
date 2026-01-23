"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { apiRequest } from "@/lib/api";
import { useAuth } from "@/components/AuthProvider";
import type { Role } from "@/lib/permissions";

type AuthResponse = {
  token: string;
  email: string;
  userId: string;
};

const ROLES: Role[] = [
  "admin",
  "catalogo",
  "inventario",
  "compras",
  "ventas",
  "clientes",
  "usuarios",
  "auditor",
];

export default function LoginPage() {
  const router = useRouter();
  const { login } = useAuth();
  const [mode, setMode] = useState<"login" | "register">("login");
  const [loading, setLoading] = useState(false);
  const [role, setRole] = useState<Role>("admin");
  const [error, setError] = useState<string>("");
  const [form, setForm] = useState({
    name: "",
    email: "",
    password: "",
  });

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setLoading(true);
    setError("");

    try {
      const payload =
        mode === "register"
          ? {
              name: form.name,
              email: form.email,
              password: form.password,
            }
          : { email: form.email, password: form.password };

      const response = await apiRequest<AuthResponse>(
        `/api/auth/${mode === "register" ? "register" : "login"}`,
        {
          method: "POST",
          body: JSON.stringify(payload),
        }
      );

      login(response.token, role);
      router.push("/");
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : "No se pudo autenticar, intenta de nuevo."
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="mx-auto flex min-h-[80vh] max-w-lg flex-col justify-center">
      <div className="rounded-xl bg-white p-6 shadow-sm">
        <h2 className="text-xl font-semibold text-zinc-900">
          {mode === "login" ? "Iniciar sesión" : "Crear cuenta"}
        </h2>
        <p className="mt-1 text-sm text-zinc-500">
          Usa el IAM para obtener tu token y habilitar módulos.
        </p>

        <div className="mt-4 flex gap-2 text-xs">
          <button
            type="button"
            onClick={() => setMode("login")}
            className={`rounded-full px-3 py-1 ${
              mode === "login"
                ? "bg-zinc-900 text-white"
                : "bg-zinc-100 text-zinc-700"
            }`}
          >
            Login
          </button>
          <button
            type="button"
            onClick={() => setMode("register")}
            className={`rounded-full px-3 py-1 ${
              mode === "register"
                ? "bg-zinc-900 text-white"
                : "bg-zinc-100 text-zinc-700"
            }`}
          >
            Registro
          </button>
        </div>

        <form onSubmit={handleSubmit} className="mt-6 space-y-4">
          {mode === "register" && (
            <div>
              <label className="text-xs text-zinc-500">Nombre</label>
              <input
                value={form.name}
                onChange={(event) =>
                  setForm((prev) => ({ ...prev, name: event.target.value }))
                }
                className="mt-1 w-full rounded-md border border-zinc-200 px-3 py-2 text-sm"
                placeholder="Nombre completo"
                required
              />
            </div>
          )}

          <div>
            <label className="text-xs text-zinc-500">Correo</label>
            <input
              value={form.email}
              onChange={(event) =>
                setForm((prev) => ({ ...prev, email: event.target.value }))
              }
              className="mt-1 w-full rounded-md border border-zinc-200 px-3 py-2 text-sm"
              placeholder="correo@empresa.com"
              required
            />
          </div>

          <div>
            <label className="text-xs text-zinc-500">Contraseña</label>
            <input
              type="password"
              value={form.password}
              onChange={(event) =>
                setForm((prev) => ({ ...prev, password: event.target.value }))
              }
              className="mt-1 w-full rounded-md border border-zinc-200 px-3 py-2 text-sm"
              required
            />
          </div>

          <div>
            <label className="text-xs text-zinc-500">Rol (frontend)</label>
            <select
              value={role}
              onChange={(event) => setRole(event.target.value as Role)}
              className="mt-1 w-full rounded-md border border-zinc-200 px-3 py-2 text-sm"
            >
              {ROLES.map((roleOption) => (
                <option key={roleOption} value={roleOption}>
                  {roleOption}
                </option>
              ))}
            </select>
            <p className="mt-1 text-[11px] text-zinc-500">
              Este rol define qué módulos ves en la interfaz.
            </p>
          </div>

          {error && (
            <div className="rounded-md border border-rose-200 bg-rose-50 px-3 py-2 text-xs text-rose-700">
              {error}
            </div>
          )}

          <button
            type="submit"
            disabled={loading}
            className="btn-primary w-full"
          >
            {loading
              ? "Procesando..."
              : mode === "login"
              ? "Entrar"
              : "Crear cuenta"}
          </button>
        </form>
      </div>
    </div>
  );
}
