import { defineNuxtRouteMiddleware, navigateTo } from 'nuxt/app'
import { useApi } from '../composables/api'

export default defineNuxtRouteMiddleware(() => {
  const { token, roles } = useApi()
  if (!token.value) return navigateTo('/login')
  if (!roles.value.includes('Admin')) return navigateTo('/')
})