export default defineNuxtRouteMiddleware(() => {
  const { token } = useApi()
  if (!token.value) return navigateTo('/login')
})