import { useRuntimeConfig, useState } from 'nuxt/app'

export function useApi() {
  const config = useRuntimeConfig()
  const token = useState<string | null>('token', () => null)
  const roles = useState<string[]>('roles', () => [])

  const apiFetch = $fetch.create({
    baseURL: config.public.apiBase as string,
    onRequest({ options }) {
      if (!token.value) return

      const h =
        options.headers instanceof Headers
          ? options.headers
          : new Headers(options.headers as HeadersInit | undefined)

      h.set('Authorization', `Bearer ${token.value}`)

      // Some ofetch typings mark headers readonly-ish; force assignment safely
      ;(options as any).headers = h
    }
  })

  return { apiFetch, token, roles }
}