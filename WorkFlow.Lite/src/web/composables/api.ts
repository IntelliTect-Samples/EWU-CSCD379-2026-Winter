export function useApi() {
  const config = useRuntimeConfig()
  const token = useState<string | null>('token', () => null)
  const roles = useState<string[]>('roles', () => [])

  const apiFetch = $fetch.create({
    baseURL: config.public.apiBase,
    onRequest({ options }) {
      if (!token.value) return

      const h = options.headers instanceof Headers
        ? options.headers
        : new Headers(options.headers as HeadersInit | undefined)

      h.set('Authorization', `Bearer ${token.value}`)
      options.headers = h
    }
  })

  return { apiFetch, token, roles }
}