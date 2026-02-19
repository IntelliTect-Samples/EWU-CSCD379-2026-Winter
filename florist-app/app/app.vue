<template>
  <v-app>
    <PetalBackground />

    <v-app-bar 
      flat 
      class="px-md-12" 
      color="rgba(255, 255, 255, 0.8)" 
    >
      <v-app-bar-title class="logo-container">
        <span class="logo-text" @click="$router.push('/')">
          BLOOM & STEM
        </span>
      </v-app-bar-title>

      <v-spacer></v-spacer>

      <v-menu transition="slide-y-transition">
        <template v-slot:activator="{ props }">
          <v-btn
            icon="mdi-menu"
            variant="text"
            color="#2D5A27"
            v-bind="props"
            class="menu-icon-btn"
          >
          </v-btn>
        </template>

        <v-list class="menu-glass mt-2" min-width="200">
          <v-list-item to="/shop" prepend-icon="mdi-flower" title="Shop" class="menu-item" />
          <v-divider class="my-2"></v-divider>
          <v-list-item to="/staff" prepend-icon="mdi-flower-tulip" title="Staff Portal" />
          <v-list-item class="mt-2">
            <v-btn 
              block 
              flat 
              color="#2D5A27" 
              class="text-white rounded-pill"
              @click="loginDialog = true"
            >
              Login
            </v-btn>
          </v-list-item>
        </v-list>
      </v-menu>
    </v-app-bar>

    <v-footer app>
      <span class="text-caption">&copy; 2026 Bloom & Stem Florist</span>
    </v-footer>

    <v-main>
      <NuxtPage />
    </v-main>

    </v-app>
</template>

<script setup>
import { ref, computed } from 'vue'
import '~/assets/css/NavigationBar.css'

const loginDialog = ref(false)
const route = useRoute()

const currentRouteName = computed(() => {
  switch (route.path) {
    case '/shop':
      return 'Shop'
    case '/staff':
      return 'Staff Portal'
    default:
      return 'Page'
  }
})
</script>

<style>
[v-cloak] {
  display: none;
}

.v-application {
  opacity: 0;
  animation: fadeIn 0.4s forwards;
}

@keyframes fadeIn {
  to { opacity: 1; }
}
</style>