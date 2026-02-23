import { useAuth } from "./useAuth";

export const useApi = () => {
  const config = useRuntimeConfig();
  const apiBase = config.public.apiBase as string;
  const { token } = useAuth();

  const apiFetch = <T>(url: string, options?: any) => {
    const headers: Record<string, string> = {};

    // Merge any existing headers
    if (options?.headers) {
      Object.assign(headers, options.headers);
    }

    // Attach bearer token when available
    if (token.value) {
      headers["Authorization"] = `Bearer ${token.value}`;
    }

    return $fetch<T>(`${apiBase}${url}`, {
      ...options,
      headers,
    });
  };

  return { apiFetch, apiBase };
};
