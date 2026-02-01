"use client";

import { ModuleCard } from "@/components/ModuleCard";
import { Protected } from "@/components/Protected";
import { useAuth } from "@/components/AuthProvider";
import { DevBlockHeader } from "@/components/DevBlockHeader";

export default function Home() {
  const { isAuthenticated, canAccessModule } = useAuth();

  const cards = [
    {
      key: "catalogo",
      title: "Catálogo",
      description: "Crear y administrar productos.",
      href: "/catalogo",
    },
    {
      key: "inventario",
      title: "Inventario",
      description: "Consultar y ajustar stocks.",
      href: "/inventario",
    },
    {
      key: "compras",
      title: "Compras",
      description: "Registrar compras y recepciones.",
      href: "/compras",
    },
    {
      key: "ventas",
      title: "Ventas",
      description: "Registrar ventas y ver historial.",
      href: "/ventas",
    },
    {
      key: "kardex",
      title: "Kardex",
      description: "Gestionar cupos y créditos por cliente.",
      href: "/kardex",
    },
    {
      key: "clientes",
      title: "Clientes",
      description: "Gestionar clientes y su historial.",
      href: "/clientes",
    },
    {
      key: "usuarios",
      title: "Usuarios",
      description: "Administración de usuarios IAM.",
      href: "/usuarios",
    },
  ];

  return (
    <div className="space-y-6">
      <header className="dev-block-container rounded-xl bg-white p-6 shadow-sm">
        <DevBlockHeader label="sol" />
        <h2 className="text-2xl font-semibold text-zinc-900">
          Panel Operativo Zupply
        </h2>
        <p className="mt-2 text-sm text-zinc-600">
          Administra los módulos según tu rol y permisos.
        </p>
        {!isAuthenticated && (
          <p className="mt-3 text-sm text-amber-700">
            Inicia sesión para habilitar más funcionalidades.
          </p>
        )}
      </header>

      <Protected>
        <section className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
          {cards
            .filter((card) => canAccessModule(card.key))
            .map((card) => (
              <ModuleCard
                key={card.key}
                title={card.title}
                description={card.description}
                href={card.href}
              />
            ))}
        </section>
      </Protected>
    </div>
  );
}
