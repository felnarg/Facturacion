"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { apiRequest } from "@/lib/api";
import { useAuth } from "@/components/AuthProvider";
import { DevBlockHeader } from "@/components/DevBlockHeader";
import type { AuthResponse } from "@/lib/auth";
import { ROLE_LABELS } from "@/lib/permissions";

export default function LoginPage() {
  const router = useRouter();
  const { login, isAuthenticated, user } = useAuth();
  const [mode, setMode] = useState<"login" | "register">("login");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string>("");
  const [form, setForm] = useState({
    name: "",
    email: "",
    password: "",
  });

  // Si ya está autenticado, mostrar información del usuario
  if (isAuthenticated) {
    return (
      <div className="mx-auto flex min-h-[80vh] max-w-lg flex-col justify-center">
        <div className="dev-block-container rounded-xl bg-white p-6 shadow-sm">
          <DevBlockHeader label="aurora" />
          <h2 className="text-xl font-semibold text-zinc-900">
            Sesión activa
          </h2>
          <p className="mt-1 text-sm text-zinc-500">
            Ya tienes una sesión iniciada.
          </p>

          <div className="mt-6 space-y-4">
            <div className="rounded-lg bg-zinc-50 p-4">
              <p className="text-sm text-zinc-600">
                <span className="font-medium">Usuario:</span> {user.name}
              </p>
              <p className="text-sm text-zinc-600">
                <span className="font-medium">Email:</span> {user.email}
              </p>
              <div className="mt-2">
                <span className="text-sm font-medium text-zinc-600">Roles:</span>
                <div className="mt-1 flex flex-wrap gap-1">
                  {user.roles.map((role) => (
                    <span
                      key={role}
                      className="rounded-full bg-emerald-100 px-2 py-0.5 text-xs text-emerald-700"
                    >
                      {ROLE_LABELS[role] ?? role}
                    </span>
                  ))}
                </div>
              </div>
              <div className="mt-2">
                <span className="text-sm font-medium text-zinc-600">
                  Permisos ({user.permissions.length}):
                </span>
                <div className="mt-1 max-h-32 overflow-y-auto">
                  <div className="flex flex-wrap gap-1">
                    {user.permissions.slice(0, 10).map((perm) => (
                      <span
                        key={perm}
                        className="rounded bg-zinc-200 px-1.5 py-0.5 text-[10px] text-zinc-600"
                      >
                        {perm}
                      </span>
                    ))}
                    {user.permissions.length > 10 && (
                      <span className="text-xs text-zinc-500">
                        +{user.permissions.length - 10} más
                      </span>
                    )}
                  </div>
                </div>
              </div>
            </div>

            <div className="flex gap-2">
              <button
                onClick={() => router.push("/")}
                className="btn-primary flex-1"
              >
                Ir al inicio
              </button>
            </div>
          </div>
        </div>
      </div>
    );
  }

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

      // Ahora el login recibe la respuesta completa del servidor
      login(response);
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
      <div className="dev-block-container rounded-xl bg-white p-6 shadow-sm">
        <DevBlockHeader label="aurora" />
        <h2 className="text-xl font-semibold text-zinc-900">
          {mode === "login" ? "Iniciar sesión" : "Crear cuenta"}
        </h2>
        <p className="mt-1 text-sm text-zinc-500">
          Los roles y permisos se asignan desde el servidor.
        </p>

        <div className="mt-4 flex gap-2 text-xs">
          <button
            type="button"
            onClick={() => setMode("login")}
            className={`rounded-full px-3 py-1 ${mode === "login"
                ? "bg-zinc-900 text-white"
                : "bg-zinc-100 text-zinc-700"
              }`}
          >
            Login
          </button>
          <button
            type="button"
            onClick={() => setMode("register")}
            className={`rounded-full px-3 py-1 ${mode === "register"
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
