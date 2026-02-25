import { defineNuxtRouteMiddleware, navigateTo } from 'nuxt/app'
import { useApi } from '../composables/api'

export default defineNuxtRouteMiddleware(() => {
  const { token } = useApi()
  if (!token.value) return navigateTo('/login')
})