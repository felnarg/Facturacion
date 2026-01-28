"use client";

import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";
import type { AuthResponse, AuthUser } from "@/lib/auth";
import {
  clearAuth,
  getStoredUser,
  getToken,
  isTokenExpired,
  setAuthData,
} from "@/lib/auth";
import type { Permission } from "@/lib/permissions";
import { canAccessModule, hasPermission } from "@/lib/permissions";

type AuthContextValue = {
  user: AuthUser;
  permissions: Permission[];
  isAuthenticated: boolean;
  login: (response: AuthResponse) => void;
  logout: () => void;
  hasPermission: (permission: Permission) => boolean;
  canAccessModule: (module: string) => boolean;
};

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<AuthUser>({ roles: [], permissions: [] });
  const [isHydrated, setIsHydrated] = useState(false);

  useEffect(() => {
    const token = getToken();
    const storedUser = getStoredUser();

    // Verificar si el token ha expirado
    if (token && isTokenExpired(token)) {
      clearAuth();
      setUser({ roles: [], permissions: [] });
    } else {
      setUser({
        ...storedUser,
        token,
      });
    }
    setIsHydrated(true);
  }, []);

  const login = useCallback((response: AuthResponse) => {
    setAuthData(response);
    setUser({
      id: response.userId,
      email: response.email,
      name: response.name,
      token: response.token,
      roles: response.roles,
      permissions: response.permissions,
    });
  }, []);

  const logout = useCallback(() => {
    clearAuth();
    setUser({ roles: [], permissions: [] });
  }, []);

  const checkPermission = useCallback(
    (permission: Permission) => {
      return hasPermission(permission, user.permissions);
    },
    [user.permissions]
  );

  const checkModuleAccess = useCallback(
    (module: string) => {
      return canAccessModule(module, user.permissions);
    },
    [user.permissions]
  );

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      permissions: user.permissions,
      isAuthenticated: Boolean(user.token) && !isTokenExpired(user.token ?? ""),
      login,
      logout,
      hasPermission: checkPermission,
      canAccessModule: checkModuleAccess,
    }),
    [checkModuleAccess, checkPermission, login, logout, user]
  );

  // Evitar flash de contenido no autenticado durante la hidratación
  if (!isHydrated) {
    return null;
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth debe usarse dentro de AuthProvider");
  }

  return context;
}
