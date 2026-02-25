<template>
  <v-app>
    <v-navigation-drawer v-model="drawer" width="280">
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
      <v-chip v-if="isAuthed" size="small" variant="tonal" prepend-icon="mdi-account">
        Signed in
      </v-chip>
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
import { useApi } from '../composables/api'
import { useAuthService } from '../composables/auth'

const drawer = ref(false)

const { token } = useApi()
const auth = useAuthService()
const isAuthed = computed(() => !!token.value)

function logout() {
  auth.logout()
  navigateTo('/')
}
</script>