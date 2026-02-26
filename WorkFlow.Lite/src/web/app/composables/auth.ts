import { useApi } from './api'

function base64UrlDecode(input: string): string {
  const base64 = input.replace(/-/g, '+').replace(/_/g, '/')
  const padded = base64 + '='.repeat((4 - (base64.length % 4)) % 4)
  return atob(padded)
}

function parseRolesFromJwt(jwt: string): string[] {
  try {
    const payloadPart = jwt.split('.')[1]
    if (!payloadPart) return []

    const payload = JSON.parse(base64UrlDecode(payloadPart))

    const roleClaim =
      payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ??
      payload['role'] ??
      payload['roles']

    if (!roleClaim) return []

    return Array.isArray(roleClaim) ? roleClaim.map(String) : [String(roleClaim)]
  } catch {
    return []
  }
}

export function useAuthService() {
  const { apiFetch, setAuth } = useApi()

  return {
    async register(email: string, password: string) {
      await apiFetch('/api/auth/register', {
        method: 'POST',
        body: { email, password }
      })
    },

    async login(email: string, password: string) {
      const res = await apiFetch<{ token: string }>('/api/auth/login', {
        method: 'POST',
        body: { email, password }
      })

      const roles = parseRolesFromJwt(res.token)
      setAuth(res.token, roles)
    },

    logout() {
      setAuth(null, [])
    }
  }
}