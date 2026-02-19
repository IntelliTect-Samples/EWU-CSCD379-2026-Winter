<template>
  <v-app>
    <ClientOnly>
      <PetalBackground />
    </ClientOnly>
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

        <v-list class="menu-glass mt-2" min-width="220">
          <v-list-item 
            to="/shop" 
            prepend-icon="mdi-flower" 
            title="Shop" 
            class="menu-item" 
          />

          <v-list-item 
            to="/cart" 
            prepend-icon="mdi-basket-outline" 
            title="Cart"
            class="menu-item"
          >
            <template v-slot:append>
              <v-badge
                color="#B64995"
                :content="cart.length"
                :model-value="cart.length > 0"
                inline
              >
              </v-badge>
            </template>
          </v-list-item>

          <v-list-item 
            to="/login" 
            prepend-icon="mdi-flower-tulip" 
            title="Garden Portal" 
            class="menu-item" 
          />
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

const { cart, cartTotal } = useCart()
const loginDialog = ref(false)
const route = useRoute()

const currentRouteName = computed(() => {
  switch (route.path) {
    case '/shop':
      return 'Shop'
    case '/login':
      return 'Garden Portal'
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