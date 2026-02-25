<template>
  <v-app>
    <v-app-bar title="WorkFlow Lite" density="comfortable">
      <v-spacer />
      <v-btn to="/" variant="text">Board</v-btn>
      <v-btn to="/my-requests" variant="text">My Requests</v-btn>
      <v-btn to="/admin" variant="text">Admin</v-btn>
      <v-divider vertical class="mx-2" />
      <v-btn v-if="isAuthed" variant="text" @click="logout">Logout</v-btn>
      <v-btn v-else to="/login" variant="text">Login</v-btn>
    </v-app-bar>

    <v-main>
      <NuxtPage />
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
const { token } = useApi()
const auth = useAuthService()

const isAuthed = computed(() => !!token.value)

function logout() {
  auth.logout()
  navigateTo('/')
}
</script>
