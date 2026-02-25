function parseRolesFromJwt(token: string): string[] {
  const payload = JSON.parse(atob(token.split('.')[1]))
  const role = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
  if (!role) return []
  return Array.isArray(role) ? role : [role]
}

export function useAuthService() {
  const { apiFetch, token, roles } = useApi()

  return {
    async register(email: string, password: string) {
      await apiFetch('/api/auth/register', { method: 'POST', body: { email, password } })
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