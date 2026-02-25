<template>
  <div>
    <!-- Header -->
    <div class="d-flex align-center justify-space-between mb-4 flex-wrap ga-2">
      <div>
        <div class="text-h5 font-weight-medium">Public Status Board</div>
        <div class="text-body-2 text-medium-emphasis">
          Live view of recent work orders and status updates
        </div>
      </div>

      <div class="d-flex align-center ga-2">
        <v-chip v-if="liveConnected" size="small" color="success" variant="tonal" prepend-icon="mdi-wifi">
          Live
        </v-chip>
        <v-chip v-else size="small" color="warning" variant="tonal" prepend-icon="mdi-wifi-off">
          Offline
        </v-chip>

        <v-btn to="/my-requests" color="primary" variant="flat" prepend-icon="mdi-plus">
          New request
        </v-btn>
      </div>
    </div>

    <!-- KPI Cards -->
    <v-row class="mb-2">
      <v-col cols="12" sm="6" md="3">
        <v-card variant="tonal">
          <v-card-text>
            <div class="text-caption text-medium-emphasis">Total</div>
            <div class="text-h5 font-weight-medium">{{ kpi.total }}</div>
          </v-card-text>
        </v-card>
      </v-col>
      <v-col cols="12" sm="6" md="3">
        <v-card variant="tonal">
          <v-card-text>
            <div class="text-caption text-medium-emphasis">Open</div>
            <div class="text-h5 font-weight-medium">{{ kpi.open }}</div>
          </v-card-text>
        </v-card>
      </v-col>
      <v-col cols="12" sm="6" md="3">
        <v-card variant="tonal">
          <v-card-text>
            <div class="text-caption text-medium-emphasis">In Progress</div>
            <div class="text-h5 font-weight-medium">{{ kpi.inProgress }}</div>
          </v-card-text>
        </v-card>
      </v-col>
      <v-col cols="12" sm="6" md="3">
        <v-card variant="tonal">
          <v-card-text>
            <div class="text-caption text-medium-emphasis">Closed</div>
            <div class="text-h5 font-weight-medium">{{ kpi.closed }}</div>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <!-- Filters -->
    <v-card class="mb-3">
      <v-card-text class="d-flex flex-wrap ga-3 align-center">
        <v-text-field
          v-model="search"
          label="Search"
          prepend-inner-icon="mdi-magnify"
          density="comfortable"
          variant="outlined"
          hide-details
          style="min-width: 260px"
        />

        <v-select
          v-model="statusFilter"
          :items="statusOptions"
          label="Status"
          density="comfortable"
          variant="outlined"
          hide-details
          style="min-width: 200px"
        />

        <v-select
          v-model="priorityFilter"
          :items="priorityOptions"
          label="Priority"
          density="comfortable"
          variant="outlined"
          hide-details
          style="min-width: 200px"
        />

        <v-spacer />

        <v-btn variant="text" @click="resetFilters" prepend-icon="mdi-filter-remove">
          Reset
        </v-btn>
        <v-btn variant="tonal" @click="refresh" prepend-icon="mdi-refresh" :loading="loading">
          Refresh
        </v-btn>
      </v-card-text>
    </v-card>

    <!-- Table / Empty state -->
    <v-card>
      <v-card-title class="d-flex align-center justify-space-between">
        <div class="text-subtitle-1">Work Orders</div>
        <div class="text-caption text-medium-emphasis">
          Showing {{ filteredItems.length }} of {{ items.length }}
        </div>
      </v-card-title>

      <v-divider />

      <v-card-text v-if="!loading && filteredItems.length === 0">
        <v-alert type="info" variant="tonal" border="start" title="No work orders found">
          If this is a new environment, create your first request and it will show up here.
          <div class="mt-3">
            <v-btn to="/my-requests" color="primary" variant="flat" prepend-icon="mdi-plus">
              Create a work order
            </v-btn>
          </div>
        </v-alert>
      </v-card-text>

      <v-data-table
        v-else
        :items="filteredItems"
        :headers="headers"
        density="comfortable"
        :items-per-page="10"
        :loading="loading"
      >
        <template #item.status="{ value }">
          <v-chip size="small" variant="tonal" :color="statusColor(value)">
            {{ value }}
          </v-chip>
        </template>

        <template #item.priority="{ value }">
          <v-chip size="small" variant="tonal" :color="priorityColor(value)">
            {{ value }}
          </v-chip>
        </template>

        <template #item.createdAt="{ value }">
          <span class="text-body-2">{{ formatDate(value) }}</span>
        </template>
      </v-data-table>
    </v-card>

    <v-snackbar v-model="snack.show" :color="snack.color" timeout="2500">
      {{ snack.text }}
    </v-snackbar>
  </div>
</template>

<script setup lang="ts">
import * as signalR from '@microsoft/signalr'
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { useRuntimeConfig } from 'nuxt/app'
import { useWorkOrdersService } from '../composables/workorders'

type BoardItem = {
  id: number
  title: string
  status: string
  priority: string
  createdAt: string
}

const headers = [
  { title: 'ID', key: 'id' },
  { title: 'Title', key: 'title' },
  { title: 'Status', key: 'status' },
  { title: 'Priority', key: 'priority' },
  { title: 'Created', key: 'createdAt' }
]

const { getPublicBoard } = useWorkOrdersService()

const items = ref<BoardItem[]>([])
const loading = ref(false)

const search = ref('')
const statusFilter = ref<string | null>(null)
const priorityFilter = ref<string | null>(null)

const statusOptions = ['Open', 'In Progress', 'Closed']
const priorityOptions = ['Low', 'Medium', 'High', 'Urgent']

const snack = ref<{ show: boolean; text: string; color: string }>({
  show: false,
  text: '',
  color: 'info'
})

function toast(text: string, color = 'info') {
  snack.value = { show: true, text, color }
}

function resetFilters() {
  search.value = ''
  statusFilter.value = null
  priorityFilter.value = null
}

const filteredItems = computed(() => {
  const q = search.value.trim().toLowerCase()

  return items.value.filter((x) => {
    const matchesSearch =
      !q ||
      String(x.id).includes(q) ||
      (x.title ?? '').toLowerCase().includes(q) ||
      (x.status ?? '').toLowerCase().includes(q) ||
      (x.priority ?? '').toLowerCase().includes(q)

    const matchesStatus = !statusFilter.value || x.status === statusFilter.value
    const matchesPriority = !priorityFilter.value || x.priority === priorityFilter.value

    return matchesSearch && matchesStatus && matchesPriority
  })
})

const kpi = computed(() => {
  const total = items.value.length
  const open = items.value.filter((x) => x.status === 'Open').length
  const inProgress = items.value.filter((x) => x.status === 'In Progress').length
  const closed = items.value.filter((x) => x.status === 'Closed').length
  return { total, open, inProgress, closed }
})

function statusColor(v: string) {
  if (v === 'Open') return 'warning'
  if (v === 'In Progress') return 'info'
  if (v === 'Closed') return 'success'
  return 'secondary'
}

function priorityColor(v: string) {
  if (v === 'Urgent') return 'error'
  if (v === 'High') return 'warning'
  if (v === 'Medium') return 'info'
  if (v === 'Low') return 'success'
  return 'secondary'
}

function formatDate(value: any) {
  if (!value) return ''
  const d = new Date(value)
  if (Number.isNaN(d.getTime())) return String(value)
  return d.toLocaleString()
}

async function refresh() {
  loading.value = true
  try {
    items.value = (await getPublicBoard()) as BoardItem[]
  } catch {
    toast('Unable to load board. Check API connection.', 'error')
  } finally {
    loading.value = false
  }
}

let conn: signalR.HubConnection | null = null
const liveConnected = ref(false)

onMounted(async () => {
  await refresh()

  const config = useRuntimeConfig()
  conn = new signalR.HubConnectionBuilder()
    .withUrl(`${config.public.apiBase}/hubs/workorders`)
    .withAutomaticReconnect()
    .build()

  conn.onreconnected(() => {
    liveConnected.value = true
    toast('Reconnected. Live updates resumed.', 'success')
  })
  conn.onreconnecting(() => {
    liveConnected.value = false
    toast('Connection lost. Reconnecting…', 'warning')
  })
  conn.onclose(() => {
    liveConnected.value = false
  })

  conn.on('WorkOrderCreated', async () => {
    toast('New work order created. Refreshing…', 'info')
    await refresh()
  })

  conn.on('WorkOrderUpdated', async () => {
    toast('Work order updated. Refreshing…', 'info')
    await refresh()
  })

  try {
    await conn.start()
    liveConnected.value = true
  } catch {
    liveConnected.value = false
    // Don’t spam errors; app still works without realtime
  }
})

onUnmounted(async () => {
  if (conn) await conn.stop()
})
</script>