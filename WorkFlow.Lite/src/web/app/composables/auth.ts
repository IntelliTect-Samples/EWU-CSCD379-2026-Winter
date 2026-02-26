import { useApi } from './api'

function base64UrlDecode(input: string): string {
  // JWT uses base64url, not plain base64
  const base64 = input.replace(/-/g, '+').replace(/_/g, '/')
  const padded = base64 + '='.repeat((4 - (base64.length % 4)) % 4)
  return atob(padded)
}

function parseRolesFromJwt(jwt: string): string[] {
  try {
    const payloadPart = jwt.split('.')[1]
    if (!payloadPart) return []

    const payload = JSON.parse(base64UrlDecode(payloadPart))

    // ASP.NET emits roles as this claim type by default
    const roleClaim = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
    if (!roleClaim) return []

    return Array.isArray(roleClaim) ? roleClaim : [String(roleClaim)]
  } catch {
    return []
  }
}

export function useAuthService() {
  const { apiFetch, token, roles } = useApi()

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

      token.value = res.token
      roles.value = parseRolesFromJwt(res.token)
    },

    logout() {
      token.value = null
      roles.value = []
    }
  }
}