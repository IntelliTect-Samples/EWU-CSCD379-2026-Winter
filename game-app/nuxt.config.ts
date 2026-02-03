// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },
  css: [
    '@/assets/css/main.css',
    '@/assets/css/game.css',
    '@/assets/css/index.css',
    '@/assets/css/results.css',
  ],
})
