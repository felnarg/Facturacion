import { useEffect, useCallback, RefObject } from "react";

interface UseModalKeyboardOptions {
    isOpen: boolean;
    onClose: () => void;
    returnFocusRef?: RefObject<HTMLElement | null>;
}

/**
 * Hook para manejar teclado en modales:
 * - Escape cierra el modal
 * - Restaura el foco al elemento anterior
 */
export function useModalKeyboard({
    isOpen,
    onClose,
    returnFocusRef,
}: UseModalKeyboardOptions) {
    const handleKeyDown = useCallback(
        (event: KeyboardEvent) => {
            if (event.key === "Escape") {
                event.preventDefault();
                event.stopPropagation();
                onClose();

                // Restaurar foco después de cerrar
                setTimeout(() => {
                    returnFocusRef?.current?.focus();
                }, 0);
            }
        },
        [onClose, returnFocusRef]
    );

    useEffect(() => {
        if (!isOpen) return;

        // Agregar listener al documento para capturar Escape en cualquier lugar del modal
        document.addEventListener("keydown", handleKeyDown);

        return () => {
            document.removeEventListener("keydown", handleKeyDown);
        };
    }, [isOpen, handleKeyDown]);
}

/**
 * Maneja navegación con Tab en listas de resultados
 */
export function useListKeyboardNavigation(
    items: unknown[],
    selectedIndex: number,
    setSelectedIndex: (index: number) => void,
    onSelect: (index: number) => void
) {
    const handleKeyDown = useCallback(
        (event: React.KeyboardEvent) => {
            if (items.length === 0) return;

            switch (event.key) {
                case "ArrowDown":
                case "Tab":
                    if (!event.shiftKey) {
                        event.preventDefault();
                        setSelectedIndex(
                            selectedIndex < items.length - 1 ? selectedIndex + 1 : 0
                        );
                    }
                    break;
                case "ArrowUp":
                case "Tab":
                    if (event.shiftKey || event.key === "ArrowUp") {
                        event.preventDefault();
                        setSelectedIndex(
                            selectedIndex > 0 ? selectedIndex - 1 : items.length - 1
                        );
                    }
                    break;
                case "Enter":
                    event.preventDefault();
                    if (selectedIndex >= 0 && selectedIndex < items.length) {
                        onSelect(selectedIndex);
                    }
                    break;
            }
        },
        [items.length, selectedIndex, setSelectedIndex, onSelect]
    );

    return { handleKeyDown, selectedIndex };
}
