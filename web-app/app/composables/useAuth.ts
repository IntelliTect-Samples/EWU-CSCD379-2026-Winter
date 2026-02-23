import { ref, computed, readonly } from 'vue'

const accessToken = ref('')
const refreshToken = ref('')
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
    refreshToken.value = localStorage.getItem('refreshToken') || ''
    email.value = localStorage.getItem('email') || ''
    const savedRoles = localStorage.getItem('roles')
    roles.value = savedRoles ? JSON.parse(savedRoles) : []
  }

  const persist = () => {
    if (process.server) return
    localStorage.setItem('accessToken', accessToken.value)
    localStorage.setItem('refreshToken', refreshToken.value)
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
      const err = await res.json().catch(() => null)
      throw new Error(err?.message || 'Login failed')
    }

    const data = await res.json()

    accessToken.value = data.accessToken
    refreshToken.value = data.refreshToken
    email.value = emailInput
    roles.value = data.roles || []

    persist()
  }

  // -------------------------
  // REGISTER
  // -------------------------
  const register = async (emailInput: string, password: string) => {
    const res = await fetch(`${apiBase}/api/auth/register`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email: emailInput, password }),
    })

    const data = await res.json()

    if (!res.ok) {
        console.log(data)
        throw new Error(JSON.stringify(data))
    }

    return data
    }

  // -------------------------
  // LOGOUT
  // -------------------------
  const logout = () => {
    accessToken.value = ''
    refreshToken.value = ''
    email.value = ''
    roles.value = []

    if (!process.server) {
      localStorage.clear()
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
    refreshToken: readonly(refreshToken),
    email: readonly(email),
    roles: readonly(roles),
    isAuthenticated,
    isAdmin,
    login,
    register,
    logout,
    getAuthHeaders,
  }
}