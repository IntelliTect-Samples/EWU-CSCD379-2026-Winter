/**
 * Composable that wraps fetch with the API base URL and
 * automatically attaches the Bearer token Authorization header
 * when the user is authenticated.
 */
export function useApiFetch() {
  const config = useRuntimeConfig()
  const apiBase = config.public.apiBase as string
  const { getAuthHeaders } = useAuth()

  /**
   * Perform an authenticated fetch against the API.
   * @param path  - API path (e.g. "/Stylist/List")
   * @param options - Standard RequestInit options (method, body, headers, etc.)
   */
  async function apiFetch<T = any>(
    path: string,
    options: RequestInit = {},
  ): Promise<{ data: T | null; response: Response }> {
    const authHeaders = getAuthHeaders()

    const mergedHeaders: Record<string, string> = {
      ...authHeaders,
      ...(options.headers as Record<string, string> ?? {}),
    }

    // Only set Content-Type for non-FormData bodies
    if (options.body && !(options.body instanceof FormData)) {
      mergedHeaders['Content-Type'] = mergedHeaders['Content-Type'] ?? 'application/json'
    }

    const response = await fetch(`${apiBase}${path}`, {
      ...options,
      headers: mergedHeaders,
    })

    let data: T | null = null
    if (response.ok) {
      const contentType = response.headers.get('content-type')
      if (contentType?.includes('application/json')) {
        data = await response.json()
      }
    }

    return { data, response }
  }

  return { apiFetch }
}
