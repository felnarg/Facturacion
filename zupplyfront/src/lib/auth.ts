import type { Permission, Role } from "./permissions";

// ═══════════════════════════════════════════════════════════════════════════
// TIPOS DE AUTENTICACIÓN
// ═══════════════════════════════════════════════════════════════════════════

/**
 * Respuesta del servidor al autenticar
 */
export type AuthResponse = {
  userId: string;
  email: string;
  name: string;
  token: string;
  roles: Role[];
  permissions: Permission[];
};

/**
 * Usuario autenticado en el frontend
 */
export type AuthUser = {
  id?: string;
  email?: string;
  name?: string;
  token?: string;
  roles: Role[];
  permissions: Permission[];
};

// ═══════════════════════════════════════════════════════════════════════════
// ALMACENAMIENTO LOCAL
// ═══════════════════════════════════════════════════════════════════════════

const TOKEN_KEY = "zupply_token";
const USER_KEY = "zupply_user";

export function setAuthData(response: AuthResponse) {
  localStorage.setItem(TOKEN_KEY, response.token);
  localStorage.setItem(
    USER_KEY,
    JSON.stringify({
      id: response.userId,
      email: response.email,
      name: response.name,
      roles: response.roles,
      permissions: response.permissions,
    })
  );
}

export function getToken(): string {
  if (typeof window === "undefined") return "";
  return localStorage.getItem(TOKEN_KEY) ?? "";
}

export function getStoredUser(): AuthUser {
  if (typeof window === "undefined") {
    return { roles: [], permissions: [] };
  }

  const stored = localStorage.getItem(USER_KEY);
  if (!stored) {
    return { roles: [], permissions: [] };
  }

  try {
    const parsed = JSON.parse(stored);
    return {
      id: parsed.id,
      email: parsed.email,
      name: parsed.name,
      roles: parsed.roles ?? [],
      permissions: parsed.permissions ?? [],
    };
  } catch {
    return { roles: [], permissions: [] };
  }
}

export function clearAuth() {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(USER_KEY);
}

/**
 * Parsea un token JWT para extraer información básica
 * (útil para verificar expiración o datos mínimos)
 */
export function parseJwt(token: string): Partial<AuthUser> {
  if (!token) {
    return {};
  }

  const parts = token.split(".");
  if (parts.length < 2) {
    return {};
  }

  try {
    const payload = JSON.parse(atob(parts[1]));
    return {
      id: payload.sub,
      email: payload.email,
      name: payload.name,
      // Los roles y permisos también vienen en el JWT
      roles: Array.isArray(payload.role) ? payload.role : payload.role ? [payload.role] : [],
      permissions: payload.permissions ?? [],
    };
  } catch {
    return {};
  }
}

/**
 * Verifica si el token ha expirado
 */
export function isTokenExpired(token: string): boolean {
  if (!token) return true;

  const parts = token.split(".");
  if (parts.length < 2) return true;

  try {
    const payload = JSON.parse(atob(parts[1]));
    if (!payload.exp) return false;
    return Date.now() >= payload.exp * 1000;
  } catch {
    return true;
  }
}
