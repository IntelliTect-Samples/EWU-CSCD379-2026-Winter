export default defineNuxtRouteMiddleware(() => {
  const { token, roles } = useApi()
  if (!token.value) return navigateTo('/login')
  if (!roles.value.includes('Admin')) return navigateTo('/')
})