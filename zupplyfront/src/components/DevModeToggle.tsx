"use client";

import { useDevMode } from "@/context/DevModeContext";
import { isDev } from "@/lib/env";
import { useEffect, useState } from "react";

export function DevModeToggle() {
    const { isDevMode, toggleDevMode } = useDevMode();
    const [mounted, setMounted] = useState(false);

    useEffect(() => {
        setMounted(true);
    }, []);

    // Only render in the actual development environment
    if (!isDev) {
        return null;
    }

    if (!mounted) return null;

    return (
        <div className="fixed top-24 right-6 z-[9999]">
            <button
                onClick={toggleDevMode}
                className={`group flex items-center gap-2.5 px-4 py-2.5 rounded-xl shadow-xl border backdrop-blur-md transition-all duration-300 hover:scale-105 active:scale-95 ${isDevMode
                        ? "bg-white/90 border-emerald-200 text-emerald-700 dark:bg-emerald-950/30 dark:border-emerald-800 dark:text-emerald-400"
                        : "bg-white/90 border-slate-200 text-slate-500 hover:text-slate-900 dark:bg-slate-900/80 dark:border-slate-800 dark:text-slate-400 dark:hover:text-slate-200"
                    }`}
            >
                <span className="relative flex h-2.5 w-2.5">
                    {isDevMode && (
                        <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-emerald-400 opacity-75"></span>
                    )}
                    <span
                        className={`relative inline-flex rounded-full h-2.5 w-2.5 ${isDevMode ? "bg-emerald-500" : "bg-slate-400"
                            }`}
                    ></span>
                </span>
                <span className="text-xs font-bold tracking-wider uppercase">
                    {isDevMode ? "Dev Mode" : "Production View"}
                </span>
            </button>
        </div>
    );
}
