<template>
  <div>
    <div class="d-flex align-center justify-space-between mb-4 flex-wrap ga-2">
      <div>
        <div class="text-h5 font-weight-medium">Admin</div>
        <div class="text-body-2 text-medium-emphasis">
          Manage work orders across the organization
        </div>
      </div>

      <div class="d-flex align-center ga-2">
        <v-chip size="small" color="primary" variant="tonal" prepend-icon="mdi-shield-account">
          Admin Only
        </v-chip>

        <v-btn variant="tonal" prepend-icon="mdi-refresh" :loading="loading" @click="refresh">
          Refresh
        </v-btn>

        <v-btn variant="flat" color="primary" prepend-icon="mdi-download" @click="exportCsv" :disabled="items.length === 0">
          Export CSV
        </v-btn>
      </div>
    </div>

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
          clearable
          style="min-width: 200px"
        />

        <v-select
          v-model="priorityFilter"
          :items="priorityOptions"
          label="Priority"
          density="comfortable"
          variant="outlined"
          hide-details
          clearable
          style="min-width: 200px"
        />

        <v-spacer />

        <v-btn variant="text" prepend-icon="mdi-filter-remove" @click="resetFilters">
          Reset
        </v-btn>
      </v-card-text>
    </v-card>

    <v-card>
      <v-card-title class="d-flex align-center justify-space-between">
        <div class="text-subtitle-1">All Work Orders</div>
        <div class="text-caption text-medium-emphasis">
          Showing {{ filteredItems.length }} of {{ items.length }}
        </div>
      </v-card-title>

      <v-divider />

      <v-card-text v-if="!loading && filteredItems.length === 0">
        <v-alert type="info" variant="tonal" border="start" title="No results">
          Try clearing filters or refresh.
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
import { computed, onMounted, ref } from 'vue'
import { useApi } from '../composables/api'

definePageMeta({ middleware: ['admin'] })

type AdminItem = {
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

const { apiFetch } = useApi()

const items = ref<AdminItem[]>([])
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
    items.value = (await apiFetch('/api/admin/all')) as AdminItem[]
  } catch {
    toast('Unable to load admin data. Check login/role/API.', 'error')
  } finally {
    loading.value = false
  }
}

function exportCsv() {
  // basic CSV export (awesome + very "LOB")
  const cols = ['id', 'title', 'status', 'priority', 'createdAt'] as const
  const headerLine = cols.join(',')

  const lines = items.value.map((x) =>
    cols
      .map((c) => {
        const val = (x as any)[c] ?? ''
        const s = String(val).replace(/"/g, '""')
        return `"${s}"`
      })
      .join(',')
  )

  const csv = [headerLine, ...lines].join('\n')
  const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' })
  const url = URL.createObjectURL(blob)

  const a = document.createElement('a')
  a.href = url
  a.download = `workflowlite-admin-export-${new Date().toISOString().slice(0, 10)}.csv`
  a.click()

  URL.revokeObjectURL(url)
  toast('CSV exported.', 'success')
}

onMounted(refresh)
</script>