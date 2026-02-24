<template>
  <div>
    <ClientOnly>
      <PetalBackground />
    </ClientOnly>

    <v-container class="py-12 position-relative" style="z-index: 1;">
      <v-row class="mb-10" align="end">
        <v-col cols="12" md="7">
          <p class="mgmt-subtitle mb-2">Internal Administration</p>
          <h1 class="mgmt-display-title">Studio Management</h1>
        </v-col>
        <v-col cols="12" md="5" class="text-md-right">
          <v-btn 
            variant="text" 
            prepend-icon="mdi-account-circle-outline" 
            class="editorial-text mr-4"
            style="text-transform: none"
          >
            {{ username }} Admin
          </v-btn>
          <v-btn 
            @click="handleLogout"
            color="#d18b99" 
            variant="outlined" 
            rounded="xl"
            class="px-6"
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
                + Add New Stem
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
  token: String
})

// Send events back to login.vue
const emit = defineEmits(['logout', 'session-expired'])

const config = useRuntimeConfig()
const inventory = ref([])
const showAddDialog = ref(false)
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