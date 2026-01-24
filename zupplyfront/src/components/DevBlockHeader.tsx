"use client";

import { useDevMode } from "@/context/DevModeContext";

type DevBlockHeaderProps = {
  label: string;
};

export function DevBlockHeader({ label }: DevBlockHeaderProps) {
  const { isDevMode } = useDevMode();

  if (!isDevMode) {
    return null;
  }

  return (
    <div className="dev-block-header" aria-hidden="true">
      {label}
    </div>
  );
}
