export default defineNuxtConfig({
  future: {
    compatibilityVersion: 4,
  },
  ssr: false,
  srcDir: 'app/',
  dir: {
    public: 'public/'
  },
  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },
  build: {
    transpile: ['vuetify'],
  },
  app: {
    baseURL: '/',
    buildAssetsDir: '_nuxt',
    head: {
      title: 'Bloom & Stem',
      link: [
        { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' },
        { rel: 'apple-touch-icon', sizes: '180x180', href: '/apple-touch-icon.png' },
        { rel: 'icon', type: 'image/png', sizes: '96x96', href: '/favicon-96x96.png' },
        { rel: 'icon', type: 'image/png', sizes: '192x192', href: '/web-app-manifest-192x192.png' },
        { rel : 'icon', type: 'image/png', sizes: '512x512', href: '/web-app-manifest-512x512.png' },   
        { rel: 'icon', type: 'image/svg+xml', href: '/favicon.svg' },    
        { rel: 'manifest', href: '/site.webmanifest' } 
      ],
      meta: [
        { name: 'theme-color', content: '#2D5A27' }
      ]
    }
  },
  css: [
    'vuetify/lib/styles/main.sass',
    '@mdi/font/css/materialdesignicons.min.css',
    '@/assets/css/index.css',
    '@/assets/css/login.css',
    '@/assets/css/NavigationBar.css',
    '@/assets/css/PetalBackground.css',
    '@/assets/css/shop.css'
  ],
  runtimeConfig: {
    public: {
      apiBase: 'https://bs-botanicals-api-dtgjbhfwacheb7bb.eastus2-01.azurewebsites.net/api'
    }
  }
})
