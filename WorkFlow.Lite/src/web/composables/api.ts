export function useApi() {
  const config = useRuntimeConfig()
  const token = useState<string | null>('token', () => null)
  const roles = useState<string[]>('roles', () => [])

  const apiFetch = $fetch.create({
    baseURL: config.public.apiBase,
    onRequest({ options }) {
      if (token.value) {
        options.headers = { ...(options.headers || {}), Authorization: `Bearer ${token.value}` }
      }
    }
  })

  return { apiFetch, token, roles }
}