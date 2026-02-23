import { useState } from "#imports";

interface AuthUser {
  id: string;
  email: string;
  roles: string[];
}

export function useAuth() {
  const token = useState<string | null>("auth_token", () => null);
  const user = useState<AuthUser | null>("auth_user", () => null);

  function setToken(newToken: string | null) {
    token.value = newToken;
    if (import.meta.client) {
      if (newToken) {
        localStorage.setItem("auth_token", newToken);
      } else {
        localStorage.removeItem("auth_token");
      }
    }
  }

  /** Restore token from localStorage (call once on client mount) */
  function restoreToken() {
    if (import.meta.client && !token.value) {
      const saved = localStorage.getItem("auth_token");
      if (saved) {
        token.value = saved;
      }
    }
  }

  /** Fetch the current user's info from the API using the stored token */
  async function fetchUser() {
    if (!token.value) {
      user.value = null;
      return;
    }
    try {
      const config = useRuntimeConfig();
      const apiBase = config.public.apiBase as string;
      const info = await $fetch<AuthUser>(`${apiBase}/auth/me`, {
        headers: { Authorization: `Bearer ${token.value}` },
      });
      user.value = info;
    } catch {
      // Token is invalid or expired – clear everything
      user.value = null;
      setToken(null);
    }
  }

  async function login(email: string, password: string) {
    const config = useRuntimeConfig();
    const apiBase = config.public.apiBase as string;
    // Identity API expects JSON body
    const res = await $fetch<{ accessToken: string; refreshToken: string }>(
      `${apiBase.replace("/api", "")}/login`,
      {
        method: "POST",
        body: { email, password },
      },
    );
    if (!res.accessToken) throw new Error("No token returned");
    setToken(res.accessToken);
    await fetchUser();
    return res;
  }

  function logout() {
    setToken(null);
    user.value = null;
  }

  /** Check if the current user has a specific role */
  function hasRole(role: string): boolean {
    return user.value?.roles?.includes(role) ?? false;
  }

  return {
    token,
    user,
    login,
    logout,
    setToken,
    restoreToken,
    fetchUser,
    hasRole,
  };
}
