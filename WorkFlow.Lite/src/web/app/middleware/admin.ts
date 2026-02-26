import { defineNuxtRouteMiddleware, navigateTo } from 'nuxt/app'
import { useApi } from '../composables/api'

export default defineNuxtRouteMiddleware(() => {
  const { token, roles } = useApi()
  if (!token.value) return navigateTo('/login')

  const roleList = Array.isArray(roles.value) ? roles.value : []
  if (!roleList.includes('Admin')) return navigateTo('/')
})