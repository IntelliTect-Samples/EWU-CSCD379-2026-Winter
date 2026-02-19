export default defineNuxtRouteMiddleware((to) => {
  // Skip auth check on the server — tokens live in localStorage which is
  // only available on the client. Without this guard, SSR always sees the
  // user as unauthenticated and redirects to /login.
  if (import.meta.server) return

  const { isAuthenticated } = useAuth()

  // Allow access to pages marked as public
  if (to.meta.public) {
    return
  }

  // Redirect unauthenticated users to login
  if (!isAuthenticated.value) {
    return navigateTo('/login')
  }
})
