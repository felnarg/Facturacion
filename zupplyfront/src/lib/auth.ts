import type { Role } from "./permissions";

export type AuthUser = {
  id?: string;
  email?: string;
  token?: string;
  role?: Role;
};

const TOKEN_KEY = "zupply_token";
const ROLE_KEY = "zupply_role";

export function setToken(token: string) {
  localStorage.setItem(TOKEN_KEY, token);
}

export function getToken() {
  return localStorage.getItem(TOKEN_KEY) ?? "";
}

export function setRole(role: Role) {
  localStorage.setItem(ROLE_KEY, role);
}

export function getRole(): Role | undefined {
  const value = localStorage.getItem(ROLE_KEY);
  return value as Role | undefined;
}

export function clearAuth() {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(ROLE_KEY);
}

export function parseJwt(token: string): AuthUser {
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
    };
  } catch {
    return {};
  }
}
