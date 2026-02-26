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
        <v-col cols="12" md="4" class="d-flex justify-space-between justify-md-end align-center">
          <span v-if="username" class="mgmt-user-text d-flex align-center" style="font-weight: 500; color: #2D5A27;">
            <v-icon start size="small">mdi-account-circle-outline</v-icon>
            {{ username }}
          </span>
          
          <v-btn 
            @click="handleLogout"
            color="#2D5A27" 
            rounded="xl" 
            class="px-6 shadow-btn logout-btn"
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
            <div class="inventory-header d-flex justify-space-between align-center mb-8">
              <h2 class="display-main" style="font-size: 1.8rem;">Inventory</h2>
              <v-btn 
                color="#2D5A27" 
                rounded="xl" 
                class="px-8 shadow-btn add-btn" 
                @click="openAddDialog"
              >
                + Add New Item
              </v-btn>
            </div>
            
            <v-table class="inventory-table">
              <thead>
                <tr>
                  <th>Botanical</th>
                  <th>Season</th>
                  <th>Pricing</th>
                  <th class="text-center">Manage</th> <th class="text-right">Stock Level</th>
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
                    <v-btn 
                      icon="mdi-pencil-outline" 
                      variant="text" 
                      color="grey" 
                      class="action-btn"
                      @click="openEditDialog(item)"
                    ></v-btn>

                    <v-btn 
                      icon="mdi-delete-outline" 
                      variant="text" 
                      color="#d18b99" 
                      class="action-btn"
                      @click="confirmDelete(item.id)"
                    ></v-btn>
                  </td>
                  <td class="text-center">
                    <div class="d-flex align-center justify-center">
                      <div class="d-flex align-center border rounded-pill px-2" style="border-color: #2D5A27 !important; height: 32px;">
                        <v-btn 
                          icon="mdi-minus" 
                          variant="text" 
                          density="comfortable" 
                          size="x-small" 
                          color="#2D5A27"
                          @click="adjustStock(item, -1)"
                          :disabled="item.inventoryCount <= 0"
                        ></v-btn>

                        <span class="mx-3 editorial-text" style="min-width: 20px; font-weight: 600;">
                          {{ item.inventoryCount }}
                        </span>

                        <v-btn 
                          icon="mdi-plus" 
                          variant="text" 
                          density="comfortable" 
                          size="x-small" 
                          color="#2D5A27"
                          @click="adjustStock(item, 1)"
                        ></v-btn>
                      </div>
                    </div>
                  </td>
                </tr>
              </tbody>
            </v-table>
          </v-window-item>

          <v-window-item value="employees">
            <div class="inventory-header d-flex justify-space-between align-center mb-8">
              <h2 class="display-main" style="font-size: 1.8rem;"> Current Staff</h2>
              <v-btn 
                color="#2D5A27" 
                rounded="xl" 
                class="px-8 shadow-btn add-btn" 
                disabled
              >
                + Add Member
              </v-btn>
            </div>
            
            <v-alert 
              type="info" 
              variant="tonal" 
              color="#2D5A27" 
              icon="mdi-account-group-outline"
              class="team-alert"
            >
              <div class="editorial-text" style="font-size: 1rem; line-height: 1.4;">
                Team management features are currently being cultivated. Check back soon!
              </div>
            </v-alert>
          </v-window-item>
        </v-window>
      </v-card>
    </v-container>
    <v-dialog v-model="showAddDialog" max-width="500px">
      <v-card class="pa-8 text-center" style="background-color: white; border-radius: 24px;">
        <v-card-title class="display-main px-0 mb-6" style="font-size: 1.8rem; font-weight: 600; color: #2D5A27;">{{ isEditing ? 'Edit Item' : 'New Item' }}</v-card-title>
        <v-card-text>
          <v-text-field v-model="newBouquet.name" label="Name" variant="outlined"></v-text-field>
          <v-select v-model="newBouquet.season" :items="['Spring', 'Summer', 'Autumn', 'Winter']" label="Season" variant="outlined"></v-select>
          <v-text-field v-model.number="newBouquet.price" label="Price" type="number" variant="outlined"></v-text-field>
          <v-text-field v-model="newBouquet.imageUrl" label="Image URL" variant="outlined"></v-text-field>
          <v-text-field 
            v-model.number="newBouquet.inventoryCount" 
            label="Initial Stock" 
            type="number" 
            variant="outlined" 
            color="#2D5A27"
          ></v-text-field>
        </v-card-text>

        <v-card-actions class="pb-8 justify-center px-4">
          <v-btn 
            color="#d18b99" 
            variant="flat" 
            rounded="xl" 
            class="px-6 dialog-btn" 
            @click="showAddDialog = false"
          >
            Cancel
          </v-btn>
          <v-btn 
            color="#2D5A27" 
            variant="flat" 
            rounded="xl" 
            class="px-6 dialog-btn" 
            @click="saveBouquet"
          >
            Save Changes
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <v-dialog v-model="showDeleteDialog" max-width="400px">
      <v-card class="pa-8 text-center" style="background-color: white; border-radius: 24px;">
        <h2 class="display-main mb-2" style="color: #2D5A27;">Delete Product</h2>
        <p class="brand-ethos mb-6">Are you sure you want to remove this product? This cannot be undone.</p>
        
        <v-card-actions class="px-0 d-flex flex-column gap-2">
          <v-btn color="#2D5A27" variant="flat" block rounded="xl" @click="showDeleteDialog = false">
            Keep in Collection
          </v-btn>
          <v-btn color="#d18b99" variant="outlined" block rounded="xl" @click="deleteStem(itemToDelete)">
            Confirm Deletion
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
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
const showDeleteDialog = ref(false)
const itemToDelete = ref(null)
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

const confirmDelete = (id) => {
  itemToDelete.value = id
  showDeleteDialog.value = true
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
  try {
    await $fetch(`${config.public.apiBase}/bouquets/${id}`, {
      method: 'DELETE',
      headers: { Authorization: `Bearer ${props.token}` }
    })
    showDeleteDialog.value = false
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