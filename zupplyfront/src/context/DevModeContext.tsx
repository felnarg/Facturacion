"use client";

import React, { createContext, useContext, useEffect, useState } from "react";
import { isDev as isDevEnv } from "@/lib/env";

interface DevModeContextType {
  isDevMode: boolean;
  toggleDevMode: () => void;
}

const DevModeContext = createContext<DevModeContextType | undefined>(undefined);

export function DevModeProvider({ children }: { children: React.ReactNode }) {
  // Initialize state based on the actual environment variable.
  // If we are in dev environment, start as true.
  const [isDevMode, setIsDevMode] = useState(isDevEnv);

  // When isDevMode changes, update the body class for global styling changes (like borders)
  useEffect(() => {
    // Only apply class manipulation if we are technically in a dev environment
    // or if the user wants to force it.
    // Actually, if the context allows toggling, we should reflect it.
    if (isDevMode) {
      document.body.classList.add("dev-mode");
    } else {
      document.body.classList.remove("dev-mode");
    }
  }, [isDevMode]);

  const toggleDevMode = () => {
    setIsDevMode((prev) => !prev);
  };

  return (
    <DevModeContext.Provider value={{ isDevMode, toggleDevMode }}>
      {children}
    </DevModeContext.Provider>
  );
}

export function useDevMode() {
  const context = useContext(DevModeContext);
  if (context === undefined) {
    throw new Error("useDevMode must be used within a DevModeProvider");
  }
  return context;
}
