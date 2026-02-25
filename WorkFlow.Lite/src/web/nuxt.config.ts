import { defineNuxtConfig } from 'nuxt/config'

export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },
  ssr: false,
  css: ['vuetify/styles', '@mdi/font/css/materialdesignicons.css'],
  build: { transpile: ['vuetify'] },
  runtimeConfig: {
    public: {
      apiBase: (process.env.NUXT_PUBLIC_API_BASE as string) || 'https://localhost:5001'
    }
  }
})