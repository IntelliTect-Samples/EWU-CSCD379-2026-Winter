// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  ssr: false, 
  css: ['~/assets/main.css'],
  modules: ['@pinia/nuxt'],
  nitro: { preset: 'static' },
  runtimeConfig: {
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE || "http://localhost:5237"
    }
  }
})
