export default defineNuxtRouteMiddleware((to) => {
  // Skip on server (localStorage only exists in browser)
  if (import.meta.server) return

  // Allow access to public admin pages (like login)
  if (to.meta.public) {
    return
  }

  const isAdmin = localStorage.getItem('isAdmin')

  if (!isAdmin) {
    return navigateTo('/admin/login')
  }
})