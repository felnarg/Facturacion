"use client";

import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";
import type { AuthUser } from "@/lib/auth";
import {
  clearAuth,
  getRole,
  getToken,
  parseJwt,
  setRole,
  setToken,
} from "@/lib/auth";
import type { Permission, Role } from "@/lib/permissions";
import { ROLE_PERMISSIONS } from "@/lib/permissions";

type AuthContextValue = {
  user: AuthUser;
  permissions: Permission[];
  isAuthenticated: boolean;
  login: (token: string, role: Role) => void;
  logout: () => void;
  setUserRole: (role: Role) => void;
};

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<AuthUser>({});

  useEffect(() => {
    const token = getToken();
    const role = getRole();
    const parsed = parseJwt(token);
    setUser({
      ...parsed,
      token,
      role,
    });
  }, []);

  const login = useCallback((token: string, role: Role) => {
    setToken(token);
    setRole(role);
    const parsed = parseJwt(token);
    setUser({
      ...parsed,
      token,
      role,
    });
  }, []);

  const logout = useCallback(() => {
    clearAuth();
    setUser({});
  }, []);

  const setUserRole = useCallback((role: Role) => {
    setRole(role);
    setUser((prev) => ({
      ...prev,
      role,
    }));
  }, []);

  const permissions = useMemo<Permission[]>(() => {
    if (!user.role) {
      return [];
    }

    return ROLE_PERMISSIONS[user.role] ?? [];
  }, [user.role]);

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      permissions,
      isAuthenticated: Boolean(user.token),
      login,
      logout,
      setUserRole,
    }),
    [login, logout, permissions, setUserRole, user]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth debe usarse dentro de AuthProvider");
  }

  return context;
}
