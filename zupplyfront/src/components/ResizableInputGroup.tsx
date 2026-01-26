"use client";

import { useEffect, useRef, useState } from "react";

type Column = {
    id: string;
    label: string;
    initialWidth: number;
    minWidth?: number;
    content: React.ReactNode;
};

export function ResizableInputGroup({ columns: initialColumns }: { columns: Column[] }) {
    const [widths, setWidths] = useState<number[]>(
        initialColumns.map((c) => c.initialWidth)
    );
    const [isResizing, setIsResizing] = useState(false);
    const activeIndex = useRef<number | null>(null);
    const startX = useRef<number>(0);
    const startWidth = useRef<number>(0);

    useEffect(() => {
        const handleMouseMove = (e: MouseEvent) => {
            if (activeIndex.current === null) return;

            const diff = e.clientX - startX.current;
            const newWidth = Math.max(
                initialColumns[activeIndex.current].minWidth || 50,
                startWidth.current + diff
            );

            setWidths((prev) => {
                const next = [...prev];
                next[activeIndex.current!] = newWidth;
                return next;
            });
        };

        const handleMouseUp = () => {
            if (activeIndex.current !== null) {
                setIsResizing(false);
                activeIndex.current = null;
                document.body.style.cursor = "default";
            }
        };

        if (isResizing) {
            document.addEventListener("mousemove", handleMouseMove);
            document.addEventListener("mouseup", handleMouseUp);
        }

        return () => {
            document.removeEventListener("mousemove", handleMouseMove);
            document.removeEventListener("mouseup", handleMouseUp);
        };
    }, [isResizing, initialColumns]);

    const handleMouseDown = (index: number, e: React.MouseEvent) => {
        e.preventDefault();
        activeIndex.current = index;
        startX.current = e.clientX;
        startWidth.current = widths[index];
        setIsResizing(true);
        document.body.style.cursor = "col-resize";
    };

    return (
        <div className="w-full overflow-x-auto pb-4 scrollbar-thin">
            <div className="inline-block min-w-full border border-zinc-200 rounded-lg overflow-hidden bg-white shadow-sm">
                {/* Header Row */}
                <div className="flex bg-zinc-50 border-b border-zinc-200">
                    {initialColumns.map((col, titleIndex) => (
                        <div
                            key={col.id}
                            style={{ width: widths[titleIndex], flexShrink: 0 }}
                            className="relative flex items-center px-3 py-2 text-xs font-semibold uppercase text-zinc-500 border-r border-zinc-200 last:border-r-0"
                        >
                            <span className="truncate">{col.label}</span>
                            {/* Resize Handle */}
                            <div
                                onMouseDown={(e) => handleMouseDown(titleIndex, e)}
                                className="absolute right-0 top-0 bottom-0 w-4 cursor-col-resize hover:bg-blue-400/20 active:bg-blue-400/40 transition-colors z-10 flex justify-center items-center group"
                            >
                                <div className="w-[1px] h-4 bg-zinc-300 group-hover:bg-blue-400" />
                            </div>
                        </div>
                    ))}
                </div>

                {/* Content Row */}
                <div className="flex bg-white">
                    {initialColumns.map((col, contentIndex) => (
                        <div
                            key={`${col.id}-content`}
                            style={{ width: widths[contentIndex], flexShrink: 0 }}
                            className="relative p-2 border-r border-zinc-200 last:border-r-0"
                        >
                            {col.content}
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
}
