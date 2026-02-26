import { ref, computed, readonly } from 'vue'

const accessToken = ref('')
const email = ref('')
const roles = ref<string[]>([])

export const useAuth = () => {
  const config = useRuntimeConfig()
  const apiBase = config.public.apiBase as string

  const isAuthenticated = computed(() => !!accessToken.value)
  const isAdmin = computed(() => roles.value.includes('Admin'))

  // -------------------------
  // Restore from localStorage
  // -------------------------
  const restore = () => {
    if (process.server) return
    accessToken.value = localStorage.getItem('accessToken') || ''
    email.value = localStorage.getItem('email') || ''
    const savedRoles = localStorage.getItem('roles')
    roles.value = savedRoles ? JSON.parse(savedRoles) : []
  }

  const persist = () => {
    if (process.server) return
    localStorage.setItem('accessToken', accessToken.value)
    localStorage.setItem('email', email.value)
    localStorage.setItem('roles', JSON.stringify(roles.value))
  }

  // -------------------------
  // LOGIN
  // -------------------------
  const login = async (emailInput: string, password: string) => {
    const res = await fetch(`${apiBase}/api/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email: emailInput, password }),
    })

    if (!res.ok) {
      const text = await res.text()
      throw new Error(text || 'Login failed')
    }

    const data = await res.json()

    accessToken.value = data.accessToken
    email.value = emailInput
    roles.value = data.roles || []

    persist()
  }

  // -------------------------
  // LOGOUT
  // -------------------------
  const logout = () => {
    accessToken.value = ''
    email.value = ''
    roles.value = []

    if (!process.server) {
      localStorage.removeItem('accessToken')
      localStorage.removeItem('email')
      localStorage.removeItem('roles')
    }
  }

  // -------------------------
  // GET AUTH HEADER
  // -------------------------
  const getAuthHeaders = () => {
    if (!accessToken.value) return {}
    return {
      Authorization: `Bearer ${accessToken.value}`,
    }
  }

  if (!process.server && !accessToken.value) {
    restore()
  }

  return {
    accessToken: readonly(accessToken),
    email: readonly(email),
    roles: readonly(roles),
    isAuthenticated,
    isAdmin,
    login,
    logout,
    getAuthHeaders,
  }
}