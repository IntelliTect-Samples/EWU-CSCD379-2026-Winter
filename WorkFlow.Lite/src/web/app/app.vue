<template>
  <v-app>
    <v-navigation-drawer v-model="drawer" temporary width="280">
      <v-list density="comfortable" nav>
        <v-list-item title="WorkFlow Lite" subtitle="Work orders & approvals" class="mb-2" />
        <v-divider class="mb-2" />
        <v-list-item to="/" prepend-icon="mdi-view-dashboard" title="Public Board" />
        <v-list-item to="/my-requests" prepend-icon="mdi-clipboard-text" title="My Requests" />
        <v-list-item to="/admin" prepend-icon="mdi-shield-account" title="Admin" />
      </v-list>

      <template #append>
        <v-divider />
        <div class="pa-3">
          <v-btn v-if="isAuthed" block variant="tonal" prepend-icon="mdi-logout" @click="logout">
            Logout
          </v-btn>
          <v-btn v-else block variant="tonal" prepend-icon="mdi-login" to="/login">
            Login
          </v-btn>
        </div>
      </template>
    </v-navigation-drawer>

    <v-app-bar density="comfortable">
      <v-app-bar-nav-icon @click="drawer = !drawer" />
      <v-toolbar-title>WorkFlow Lite</v-toolbar-title>

      <v-spacer />

      <!-- Desktop quick nav (still works on mobile) -->
      <v-btn to="/" variant="text">Board</v-btn>
      <v-btn to="/my-requests" variant="text">My Requests</v-btn>
      <v-btn to="/admin" variant="text">Admin</v-btn>

      <v-divider vertical class="mx-2" />

      <v-btn v-if="isAuthed" variant="text" @click="logout">Logout</v-btn>
      <v-btn v-else to="/login" variant="text">Login</v-btn>
    </v-app-bar>

    <v-main>
      <v-container class="py-6">
        <NuxtPage />
      </v-container>
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { navigateTo } from 'nuxt/app'
import { useApi } from './composables/api'
import { useAuthService } from './composables/auth'

const drawer = ref(false)

const { token } = useApi()
const auth = useAuthService()
const isAuthed = computed(() => !!token.value)

function logout() {
  auth.logout()
  navigateTo('/')
}
</script>