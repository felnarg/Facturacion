"use client";

import { useEffect, useState } from "react";
import { Nav } from "@/components/Nav";

type AppShellProps = {
  children: React.ReactNode;
};

export function AppShell({ children }: AppShellProps) {
  const [open, setOpen] = useState(false);

  useEffect(() => {
    const saved = window.localStorage.getItem("zupply.sidebar.open");
    if (saved !== null) {
      setOpen(saved === "true");
      return;
    }
    setOpen(window.innerWidth >= 1024);
  }, []);

  return (
    <div className="min-h-screen bg-zinc-100">
      <header className="flex items-center justify-between border-b border-zinc-200 bg-white px-4 py-3">
        <button
          type="button"
          onClick={() =>
            setOpen((prev) => {
              const next = !prev;
              window.localStorage.setItem(
                "zupply.sidebar.open",
                String(next)
              );
              return next;
            })
          }
          className="btn-secondary"
          aria-label="Alternar menú"
          aria-expanded={open}
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
            strokeLinecap="round"
            strokeLinejoin="round"
            className="h-5 w-5"
          >
            <line x1="3" y1="6" x2="21" y2="6" />
            <line x1="3" y1="12" x2="21" y2="12" />
            <line x1="3" y1="18" x2="21" y2="18" />
          </svg>
        </button>
        <span className="text-sm font-semibold text-zinc-900">Zupply</span>
        <span className="h-6 w-6" />
      </header>

      <div className="flex min-h-screen">
        <div
          className={`fixed inset-y-0 left-0 z-40 w-64 transform border-r border-zinc-200 bg-white transition-transform duration-200 lg:static ${
            open ? "translate-x-0 lg:w-64" : "-translate-x-full lg:w-0"
          } ${open ? "" : "lg:overflow-hidden lg:border-r-0"}`}
        >
          <div className="flex items-center justify-between border-b border-zinc-100 px-4 py-3 lg:hidden">
            <span className="text-sm font-semibold text-zinc-900">Menú</span>
            <button
              type="button"
              onClick={() => setOpen(false)}
              className="btn-secondary"
              aria-label="Cerrar menú"
            >
              Cerrar
            </button>
          </div>
          <div className="h-full">
            <Nav onNavigate={() => setOpen(false)} />
          </div>
        </div>

        {open && (
          <div
            className="fixed inset-0 z-30 bg-black/40 lg:hidden"
            onClick={() => setOpen(false)}
          />
        )}

        <main className="flex-1 p-6">{children}</main>
      </div>
    </div>
  );
}
