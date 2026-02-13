// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },
  build: {
    transpile: ['vuetify'],
  },
  modules: ["vuetify-nuxt-module"],
  vuetify: {
    locale: {
      locale: 'en',
      fallback: 'en',
    },
  },
  runtimeConfig: {
    public: {
      apiBase: 'http://localhost:5115'
    }
  }
})
