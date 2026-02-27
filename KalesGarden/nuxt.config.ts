// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: "2025-07-15",
  devtools: { enabled: true },

  css: ["@picocss/pico/css/pico.min.css", "~/assets/css/responsive.css"],

  runtimeConfig: {
    public: {
      apiBase: "https://kalesgardenapi.azurewebsites.net/api",
    },
  },
});
