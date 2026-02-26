<template>
  <div>
    <div class="d-flex align-center justify-space-between mb-4 flex-wrap ga-2">
      <div>
        <div class="text-h5 font-weight-medium">My Requests</div>
        <div class="text-body-2 text-medium-emphasis">Create and track your work orders</div>
      </div>
      <v-chip size="small" variant="tonal" prepend-icon="mdi-lock">
        Authenticated
      </v-chip>
    </div>

    <v-row>
      <v-col cols="12" md="7">
        <v-card>
          <v-card-title class="d-flex align-center justify-space-between">
            <div class="text-subtitle-1">My Work Orders</div>
            <v-btn variant="tonal" prepend-icon="mdi-refresh" :loading="loading" @click="refresh">
              Refresh
            </v-btn>
          </v-card-title>
          <v-divider />

          <v-card-text v-if="!loading && mine.length === 0">
            <v-alert type="info" variant="tonal" border="start" title="No requests yet">
              Create your first work order to get started.
            </v-alert>
          </v-card-text>

          <v-data-table
            v-else
            :items="mine"
            :headers="headers"
            density="comfortable"
            :loading="loading"
            :items-per-page="10"
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
          </v-data-table>
        </v-card>
      </v-col>

      <v-col cols="12" md="5">
        <v-card>
          <v-card-title class="text-subtitle-1">Create Work Order</v-card-title>
          <v-divider />
          <v-card-text class="d-flex flex-column ga-3">
            <v-text-field v-model="title" label="Title" variant="outlined" />
            <v-textarea v-model="description" label="Description" variant="outlined" />
            <v-select v-model="priority" :items="priorities" label="Priority" variant="outlined" />

            <v-btn
              color="primary"
              variant="flat"
              block
              :loading="creating"
              :disabled="!title.trim()"
              @click="create"
              prepend-icon="mdi-plus"
            >
              Create
            </v-btn>

            <div class="text-caption text-medium-emphasis">
              New requests will appear on the public board and update live.
            </div>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <v-snackbar v-model="snack.show" :color="snack.color" timeout="2500">
      {{ snack.text }}
    </v-snackbar>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useWorkOrdersService } from '../composables/workorders'

definePageMeta({ middleware: ['auth'] })

type WorkOrder = {
  id: number
  title: string
  status: string
  priority: string
}

const headers = [
  { title: 'ID', key: 'id' },
  { title: 'Title', key: 'title' },
  { title: 'Status', key: 'status' },
  { title: 'Priority', key: 'priority' }
]

const { getMine, create: createWO } = useWorkOrdersService()

const mine = ref<WorkOrder[]>([])
const loading = ref(false)
const creating = ref(false)

const title = ref('')
const description = ref('')
const priority = ref('Medium')
const priorities = ['Low', 'Medium', 'High', 'Urgent']

const snack = ref<{ show: boolean; text: string; color: string }>({
  show: false,
  text: '',
  color: 'info'
})

function toast(text: string, color = 'info') {
  snack.value = { show: true, text, color }
}

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

async function refresh() {
  loading.value = true
  try {
    mine.value = (await getMine()) as WorkOrder[]
  } catch {
    toast('Unable to load your requests. Check login/API.', 'error')
  } finally {
    loading.value = false
  }
}

onMounted(refresh)

async function create() {
  creating.value = true
  try {
    await createWO({
      title: title.value,
      description: description.value,
      priority: priority.value
    })

    title.value = ''
    description.value = ''
    priority.value = 'Medium'

    await refresh()
    toast('Work order created.', 'success')
  } catch {
    toast('Create failed.', 'error')
  } finally {
    creating.value = false
  }
}
</script>