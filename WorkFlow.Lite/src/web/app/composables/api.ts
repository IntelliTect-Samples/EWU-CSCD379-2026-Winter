import { useRuntimeConfig, useState } from 'nuxt/app'

export function useApi() {
  const config = useRuntimeConfig()

  // Always initialize with the correct types
  const token = useState<string | null>('token', () => null)
  const roles = useState<string[]>('roles', () => [])

  const apiFetch = $fetch.create({
    baseURL: String(config.public.apiBase || ''),
    onRequest({ options }) {
      if (!token.value) return

      const h =
        options.headers instanceof Headers
          ? options.headers
          : new Headers(options.headers as HeadersInit | undefined)

      h.set('Authorization', `Bearer ${token.value}`)

      // Some ofetch typings can be picky; this avoids TS complaints
      ;(options as any).headers = h
    }
  })

  function setAuth(newToken: string | null, newRoles?: unknown) {
    token.value = newToken

    // Force roles to ALWAYS be a string[]
    if (Array.isArray(newRoles)) {
      roles.value = newRoles.map((r) => String(r))
    } else {
      roles.value = []
    }
  }

  return { apiFetch, token, roles, setAuth }
}