<template>
  <v-container class="py-4">
    <h1 class="text-h5">Admin</h1>
    <p class="text-body-2">Only Admin role can see this page.</p>

    <v-card class="mt-3">
      <v-card-title>All Work Orders (raw)</v-card-title>
      <v-card-text>
        <pre style="white-space: pre-wrap">{{ data }}</pre>
      </v-card-text>
    </v-card>
  </v-container>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { navigateTo } from 'nuxt/app'
import { useApi } from '../composables/api'

const { token, roles, apiFetch } = useApi()
const data = ref<any>(null)

onMounted(async () => {
  if (!token.value) return navigateTo('/login')
  if (!roles.value.includes('Admin')) return navigateTo('/')

  data.value = await apiFetch('/api/admin/all')
})
</script>