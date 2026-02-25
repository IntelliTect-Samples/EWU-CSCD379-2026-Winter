<template>
  <v-container class="py-4">
    <v-row>
      <v-col cols="12" md="6">
        <h1 class="text-h5">My Requests</h1>
        <v-card class="mt-3">
          <v-data-table :items="mine" :headers="headers" density="compact" />
        </v-card>
      </v-col>

      <v-col cols="12" md="6">
        <h2 class="text-h6">Create Work Order</h2>
        <v-card class="mt-3">
          <v-card-text class="d-flex flex-column ga-3">
            <v-text-field v-model="title" label="Title" />
            <v-textarea v-model="description" label="Description" />
            <v-select v-model="priority" :items="priorities" label="Priority" />
            <v-btn @click="create" block>Create</v-btn>
            <div class="text-caption">{{ msg }}</div>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useWorkOrdersService } from '../composables/workorders'

definePageMeta({ middleware: ['auth'] })

const headers = [
  { title: 'ID', key: 'id' },
  { title: 'Title', key: 'title' },
  { title: 'Status', key: 'status' },
  { title: 'Priority', key: 'priority' }
]

const { getMine, create: createWO } = useWorkOrdersService()

const mine = ref<any[]>([])

const title = ref('')
const description = ref('')
const priority = ref('Medium')
const priorities = ['Low', 'Medium', 'High', 'Urgent']
const msg = ref('')

async function refresh() {
  mine.value = (await getMine()) as any[]
}

onMounted(refresh)

async function create() {
  msg.value = ''
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
    msg.value = 'Created!'
  } catch {
    msg.value = 'Create failed.'
  }
}
</script>