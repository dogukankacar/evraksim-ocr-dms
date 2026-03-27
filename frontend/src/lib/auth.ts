"use client";

import { AuthResponseDto, UserDto } from "@/types";

const TOKEN_KEY = "dms_token";
const USER_KEY = "dms_user";

export function getToken(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem(TOKEN_KEY);
}

export function setAuth(auth: AuthResponseDto): void {
  localStorage.setItem(TOKEN_KEY, auth.token);
  localStorage.setItem(
    USER_KEY,
    JSON.stringify({
      id: "",
      email: auth.email,
      fullName: auth.fullName,
      roles: auth.roles,
    })
  );
}

export function getUser(): UserDto | null {
  if (typeof window === "undefined") return null;
  const data = localStorage.getItem(USER_KEY);
  if (!data) return null;
  return JSON.parse(data);
}

export function clearAuth(): void {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(USER_KEY);
}

export function isAdmin(): boolean {
  const user = getUser();
  return user?.roles?.includes("Admin") ?? false;
}

export function isAuthenticated(): boolean {
  return !!getToken();
}
