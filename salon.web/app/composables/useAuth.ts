interface TokenResponse {
  tokenType: string
  accessToken: string
  expiresIn: number
  refreshToken: string
}

interface MeResponse {
  email: string
  roles: string[]
}

interface AuthState {
  accessToken: string | null
  refreshToken: string | null
  email: string | null
  roles: string[]
}

export function useAuth() {
  const config = useRuntimeConfig()
  const apiBase = config.public.apiBase as string

  // Nuxt-managed shared state — survives navigation, no re-reads from localStorage
  const authState = useState<AuthState>('auth', () => ({
    accessToken: null,
    refreshToken: null,
    email: null,
    roles: [],
  }))

  const isLoading = useState<boolean>('auth-loading', () => false)
  const error = useState<string | null>('auth-error', () => null)

  const isAuthenticated = computed(() => !!authState.value.accessToken)
  const email = computed(() => authState.value.email)
  const roles = computed(() => authState.value.roles)

  // On first client load, restore tokens from localStorage (persistence across refreshes)
  if (import.meta.client && !authState.value.accessToken) {
    const storedToken = localStorage.getItem('auth_accessToken')
    const storedRefresh = localStorage.getItem('auth_refreshToken')
    const storedEmail = localStorage.getItem('auth_email')
    if (storedToken) {
      authState.value.accessToken = storedToken
      authState.value.refreshToken = storedRefresh
      authState.value.email = storedEmail
      // Fetch roles from server
      fetchMe()
    }
  }

  function persistToStorage() {
    if (!import.meta.client) return
    if (authState.value.accessToken) {
      localStorage.setItem('auth_accessToken', authState.value.accessToken)
      localStorage.setItem('auth_refreshToken', authState.value.refreshToken ?? '')
      localStorage.setItem('auth_email', authState.value.email ?? '')
    } else {
      localStorage.removeItem('auth_accessToken')
      localStorage.removeItem('auth_refreshToken')
      localStorage.removeItem('auth_email')
    }
  }

  function saveTokens(data: TokenResponse, userEmail: string) {
    authState.value.accessToken = data.accessToken
    authState.value.refreshToken = data.refreshToken
    authState.value.email = userEmail
    persistToStorage()
  }

  async function fetchMe() {
    if (!authState.value.accessToken) return
    try {
      const res = await fetch(`${apiBase}/auth/me`, {
        headers: { Authorization: `Bearer ${authState.value.accessToken}` },
      })
      if (res.ok) {
        const data: MeResponse = await res.json()
        authState.value.email = data.email
        authState.value.roles = data.roles
      } else {
        authState.value.roles = []
      }
    } catch {
      authState.value.roles = []
    }
  }

  function clearTokens() {
    authState.value.accessToken = null
    authState.value.refreshToken = null
    authState.value.email = null
    authState.value.roles = []
    persistToStorage()
  }

  async function login(userEmail: string, password: string): Promise<boolean> {
    isLoading.value = true
    error.value = null

    try {
      const response = await fetch(`${apiBase}/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email: userEmail, password }),
      })

      if (!response.ok) {
        const text = await response.text()
        error.value = text || 'Invalid email or password'
        return false
      }

      const data: TokenResponse = await response.json()
      saveTokens(data, userEmail)
      await fetchMe()
      return true
    } catch (e: any) {
      error.value = e.message || 'Login failed'
      return false
    } finally {
      isLoading.value = false
    }
  }

  async function register(userEmail: string, password: string): Promise<boolean> {
    isLoading.value = true
    error.value = null

    try {
      const response = await fetch(`${apiBase}/register`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email: userEmail, password }),
      })

      if (!response.ok) {
        const body = await response.json().catch(() => null)
        if (body?.errors) {
          const messages = Object.values(body.errors).flat()
          error.value = messages.join('. ') || 'Registration failed'
        } else {
          error.value = body?.title || 'Registration failed'
        }
        return false
      }

      // Auto-login after successful registration
      return await login(userEmail, password)
    } catch (e: any) {
      error.value = e.message || 'Registration failed'
      return false
    } finally {
      isLoading.value = false
    }
  }

  function logout() {
    clearTokens()
    error.value = null
  }

  async function refreshAccessToken(): Promise<boolean> {
    if (!authState.value.refreshToken) return false

    try {
      const response = await fetch(`${apiBase}/refresh`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ refreshToken: authState.value.refreshToken }),
      })

      if (!response.ok) {
        clearTokens()
        return false
      }

      const data: TokenResponse = await response.json()
      saveTokens(data, authState.value.email!)
      return true
    } catch {
      clearTokens()
      return false
    }
  }

  function getAuthHeaders(): Record<string, string> {
    if (!authState.value.accessToken) return {}
    return { Authorization: `Bearer ${authState.value.accessToken}` }
  }

  return {
    isAuthenticated,
    email,
    roles,
    isLoading,
    error,
    login,
    register,
    logout,
    refreshAccessToken,
    getAuthHeaders,
    fetchMe,
  }
}