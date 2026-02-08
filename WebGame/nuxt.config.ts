// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: "2025-07-15",
  devtools: { enabled: true },
  app: {
    head: {
      link: [
        { rel: "icon", type: "image/png", href: "/images/Animations/IdleFrames/character_2.png" },
      ],
    },
  },
  build: {
    transpile: ["vuetify"],
  },
  css: ["vuetify/styles"],
});
