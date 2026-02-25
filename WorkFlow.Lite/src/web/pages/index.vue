<template>
  <v-container class="py-4">
    <v-row>
      <v-col cols="12" md="8">
        <h1 class="text-h5">Public Status Board</h1>
        <v-card class="mt-3">
          <v-data-table
  :items="items"
  :headers="headers"
  density="compact"
  :items-per-page="10"
  :loading="items.length === 0"
/>
        </v-card>
      </v-col>

      <v-col cols="12" md="4">
        <v-card>
          <v-card-title>Quick Links</v-card-title>
          <v-card-text class="d-flex flex-column ga-2">
            <v-btn to="/login" block>Login</v-btn>
            <v-btn to="/my-requests" variant="outlined" block>My Requests</v-btn>
            <v-btn to="/admin" variant="outlined" block>Admin</v-btn>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import * as signalR from '@microsoft/signalr'

const headers = [
  { title: 'ID', key: 'id' },
  { title: 'Title', key: 'title' },
  { title: 'Status', key: 'status' },
  { title: 'Priority', key: 'priority' },
  { title: 'Created', key: 'createdAt' }
]

const { getPublicBoard } = useWorkOrdersService()
const items = ref<any[]>([])

let conn: signalR.HubConnection | null = null

onMounted(async () => {
  // Initial load
  items.value = await getPublicBoard() as any[]

  // Setup SignalR
  const config = useRuntimeConfig()

  conn = new signalR.HubConnectionBuilder()
    .withUrl(`${config.public.apiBase}/hubs/workorders`)
    .withAutomaticReconnect()
    .build()

  conn.on('WorkOrderCreated', async () => {
    items.value = await getPublicBoard() as any[]
  })

  conn.on('WorkOrderUpdated', async () => {
    items.value = await getPublicBoard() as any[]
  })

  await conn.start()
})

// Clean up connection when leaving page
onUnmounted(async () => {
  if (conn) {
    await conn.stop()
  }
})
</script>