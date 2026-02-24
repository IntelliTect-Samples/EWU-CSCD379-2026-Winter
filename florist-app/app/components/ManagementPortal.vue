<template>
  <div>
    <ClientOnly>
      <PetalBackground />
    </ClientOnly>

    <v-container class="py-1 position-relative" style="z-index: 1;">
      <v-row class="mb-10" align="end">
        <v-col cols="12" md="8">
          <h1 class="mgmt-display-title">Management Dashboard</h1>
        </v-col>
        <v-col cols="12" md="4" class="d-flex justify-md-end align-center">
          <span class="editorial-text mr-6 d-none d-sm-inline" style="font-size: 0.9rem; opacity: 0.8;">
            <v-icon start size="small">mdi-account-circle-outline</v-icon>
            {{ username }} Admin
          </span>
          <v-btn 
            @click="handleLogout"
            color="#2D5A27" 
            variant="flat" 
            rounded="xl"
            class="px-8 shadow-btn"
            style="letter-spacing: 1px; text-transform: uppercase; font-size: 0.75rem; font-weight: 600;"
          >
            Sign Out
          </v-btn>
        </v-col>
      </v-row>

      <v-card class="mgmt-card pa-8">
        <v-tabs v-model="activeTab" color="#2D5A27" class="mb-8">
          <v-tab value="inventory">The Collection</v-tab>
          <v-tab value="employees">The Team</v-tab>
        </v-tabs>

        <v-window v-model="activeTab">
          <v-window-item value="inventory">
            <div class="d-flex justify-space-between align-center mb-8">
              <h2 class="display-main" style="font-size: 1.8rem;">Inventory</h2>
              <v-btn color="#2D5A27" rounded="xl" class="px-8 shadow-btn" @click="openAddDialog">
                + Add New Product
              </v-btn>
            </div>
            
            <v-table class="inventory-table">
              <thead>
                <tr>
                  <th>Botanical</th>
                  <th>Season</th>
                  <th>Pricing</th>
                  <th class="text-right">Manage</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="item in inventory" :key="item.id">
                  <td class="py-4">
                    <div class="d-flex align-center">
                      <v-avatar rounded="lg" size="48" class="mr-4">
                        <v-img :src="item.imageUrl" cover />
                      </v-avatar>
                      <span class="editorial-text" style="font-size: 1.1rem">{{ item.name }}</span>
                    </div>
                  </td>
                  <td><v-chip variant="outlined" color="#2D5A27" size="small">{{ item.season }}</v-chip></td>
                  <td class="price-text">${{ item.price }}</td>
                  <td class="text-right">
                    <v-btn icon="mdi-pencil-outline" variant="text" color="grey" class="action-btn"></v-btn>
                    <v-btn icon="mdi-delete-outline" variant="text" color="#d18b99" class="action-btn"></v-btn>
                  </td>
                </tr>
              </tbody>
            </v-table>
          </v-window-item>

          <v-window-item value="employees">
            <div class="d-flex justify-space-between align-center mb-8">
              <h2 class="display-main" style="font-size: 1.8rem;">Studio Team</h2>
              <v-btn color="#2D5A27" rounded="xl" class="px-8 shadow-btn" disabled>
                + Add Team Member
              </v-btn>
            </div>
            
            <v-alert type="info" variant="tonal" color="#2D5A27" icon="mdi-account-group-outline">
              Team management features are currently being cultivated. Check back soon!
            </v-alert>
          </v-window-item>
        </v-window>
      </v-card>
    </v-container>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import '~/assets/css/management.css'

// Receive data from login.vue
const props = defineProps({
  userRole: String,
  username: String,
  token: String
})

// Send events back to login.vue
const emit = defineEmits(['logout', 'session-expired'])

const config = useRuntimeConfig()
const inventory = ref([])
const showAddDialog = ref(false)
const activeTab = ref('inventory')
const isEditing = ref(false)
const currentEditId = ref(null)

const newBouquet = ref({ name: '', price: 0, season: '', imageUrl: '', inventoryCount: 0 })
const seasonRules = [v => !!v || 'The garden requires a season.']

// Actions
const openAddDialog = () => {
  isEditing.value = false
  currentEditId.value = null
  newBouquet.value = { name: '', price: 0, season: '', imageUrl: '', inventoryCount: 0 }
  showAddDialog.value = true
}

const openEditDialog = (item) => {
  isEditing.value = true
  currentEditId.value = item.id
  newBouquet.value = { ...item }
  showAddDialog.value = true
}

const fetchManagementProducts = async () => {
  try {
    const data = await $fetch(`${config.public.apiBase}/bouquets`, {
      headers: { Authorization: `Bearer ${props.token}` }
    })
    inventory.value = data
  } catch (err) {
    if (err.status === 401) emit('session-expired')
  }
}

const saveBouquet = async () => {
  try {
    const method = isEditing.value ? 'PUT' : 'POST'
    const url = isEditing.value 
      ? `${config.public.apiBase}/bouquets/${currentEditId.value}` 
      : `${config.public.apiBase}/bouquets`

    await $fetch(url, {
      method: method,
      body: newBouquet.value,
      headers: { Authorization: `Bearer ${props.token}` }
    })
    showAddDialog.value = false
    await fetchManagementProducts()
  } catch (err) {
    if (err.status === 401) emit('session-expired')
  }
}

const adjustStock = async (item, change) => {
  const newCount = item.inventoryCount + change
  if (newCount < 0) return
  try {
    await $fetch(`${config.public.apiBase}/bouquets/${item.id}/inventory`, {
      method: 'PATCH',
      body: newCount,
      headers: { 
        Authorization: `Bearer ${props.token}`,
        'Content-Type': 'application/json'
      }
    })
    item.inventoryCount = newCount
  } catch (err) {
    if (err.status === 401) emit('session-expired')
  }
}

const deleteStem = async (id) => {
  if (!confirm("Prune this arrangement?")) return
  try {
    await $fetch(`${config.public.apiBase}/bouquets/${id}`, {
      method: 'DELETE',
      headers: { Authorization: `Bearer ${props.token}` }
    })
    await fetchManagementProducts()
  } catch (err) {
    if (err.status === 401) emit('session-expired')
  }
}

const handleLogout = () => {
  emit('logout');
};

onMounted(() => {
  fetchManagementProducts()
})
</script>