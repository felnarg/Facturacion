"use client";

import Link from "next/link";

type ModuleCardProps = {
  title: string;
  description: string;
  href: string;
};

export function ModuleCard({ title, description, href }: ModuleCardProps) {
  return (
    <Link
      href={href}
      className="rounded-xl border border-zinc-200 bg-white p-4 shadow-sm transition hover:-translate-y-0.5 hover:shadow-md"
    >
      <h3 className="text-base font-semibold text-zinc-900">{title}</h3>
      <p className="mt-1 text-sm text-zinc-600">{description}</p>
    </Link>
  );
}
