import { useAuth } from '~/composables/useAuth'

export default defineNuxtRouteMiddleware((to) => {
  if (process.server) return

  const { isAuthenticated, isAdmin } = useAuth()

  if (to.meta.public) return

  if (!isAuthenticated.value) {
    return navigateTo('/admin/login')
  }

  if (!isAdmin.value) {
    return navigateTo('/')
  }
})