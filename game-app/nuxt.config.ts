// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  app:{
    head: {
      link: [
        { rel: 'icon', type: 'image/png', href: '/favicon.png' }
      ]
    }
  },
  ssr: false,
  runtimeConfig: {
    public: {
      api: 'https://grid-snap-api-a7c2b6b9dygdc3gt.eastus2-01.azurewebsites.net/api/score'
    }
  },
  nitro: {
    preset: 'azure-swa'
  },
  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },
  css: [
    '@/assets/css/main.css',
    '@/assets/css/game.css',
    '@/assets/css/index.css',
    '@/assets/css/results.css',
  ],
  build: {
    transpile: ['vuetify'],
  },
  vite: {
    define: {
      'process.env.DEBUG': 'false',
    },
  },
})
