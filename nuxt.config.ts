export default defineNuxtConfig({
  ssr: false,
  app: {
    head: {
      link: [
        { 
          rel: 'stylesheet', 
          href: 'https://fonts.googleapis.com/css2?family=Libre+Franklin:wght@700&family=Grand+Hotel&display=swap'
        }
      ]
    }
  },

  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },
  modules: [
    'vuetify-nuxt-module'
  ],
  vuetify: {
    moduleOptions: {
      /* styles config */
    },
    vuetifyOptions: {
      theme: {
        themes: {
          myCustomTheme: {
            colors: {
              correct: '#6aaa64',
              present: '#c9b458',
              absent: '#787c7e',
              primary: '#6aaa64'
            }
          },
        },
      },
    },
  },
  future: {
    compatibilityVersion: 4,
  },
})