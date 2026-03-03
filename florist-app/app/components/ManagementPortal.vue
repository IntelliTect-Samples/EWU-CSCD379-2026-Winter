<template>
  <div>
    <ClientOnly>
      <PetalBackground />
    </ClientOnly>

    <v-container class="py-1 position-relative" style="z-index: 1;">
      <v-row class="mb-10 align-center header-row-desktop">
        <v-col class="py-0">
          <h1 class="mgmt-display-title">Management Dashboard</h1>
        </v-col>

        <v-spacer class="hidden-sm-and-down"></v-spacer>

        <v-col cols="auto" class="d-flex align-center py-0">
          <span v-if="username" class="mgmt-user-text d-flex align-center me-6">
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
                  <th class="text-left">Botanical</th>
                  
                  <th class="text-left">Season</th>
                  
                  <th class="text-left">Pricing</th>
                  
                  <th class="text-right pr-12">Manage</th>
                  
                  <th class="text-center">Stock Level</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="item in inventory" :key="item.id">
                  <td class="text-left py-4">
                    <div class="d-flex align-center">
                      <v-avatar rounded="lg" size="48" class="mr-4">
                        <v-img :src="item.imageUrl?.startsWith('http') ? item.imageUrl : `${config.public.apiBase}${item.imageUrl}`" cover />
                      </v-avatar>
                      <span class="editorial-text" style="font-size: 1.1rem">{{ item.name }}</span>
                    </div>
                  </td>

                  <td class="text-left">
                    <v-chip variant="outlined" color="#2D5A27" size="small">{{ item.season }}</v-chip>
                  </td>

                  <td class="text-left price-text">
                    ${{ item.price }}
                  </td>

                  <td class="text-right pr-8">
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
          <div 
            class="drop-zone pa-6 mb-4 d-flex flex-column align-center justify-center"
            :class="{ 'drop-zone-active': isDragging }"
            @dragover.prevent="isDragging = true"
            @dragleave.prevent="isDragging = false"
            @drop.prevent="handleDrop"
            @click="$refs.fileInput.click()"
          >
            <v-icon size="40" color="#2D5A27" class="mb-2">
              {{ imageFile ? 'mdi-check-circle' : 'mdi-cloud-upload-outline' }}
            </v-icon>
            <p class="editorial-text mb-0" style="font-size: 0.9rem;">
              {{ imageFile ? imageFile.name : 'Drag photo here or click to browse' }}
            </p>
            <input type="file" ref="fileInput" class="d-none" accept="image/*" @change="handleFileSelect">
          </div>

          <v-img v-if="imagePreview" :src="imagePreview.startsWith('blob:') ? imagePreview : (imagePreview.startsWith('http') ? imagePreview : `${config.public.apiBase}${imagePreview}`)" height="120" cover class="rounded-lg mb-4 border"></v-img>

          <v-text-field v-model="newBouquet.name" label="Name" variant="outlined" color="#2D5A27"></v-text-field>
          <v-select v-model="newBouquet.season" :items="['Spring', 'Summer', 'Autumn', 'Winter']" label="Season" variant="outlined" color="#2D5A27"></v-select>
          <v-row>
            <v-col cols="6">
              <v-text-field v-model.number="newBouquet.price" label="Price" type="number" variant="outlined" color="#2D5A27"></v-text-field>
            </v-col>
            <v-col cols="6">
              <v-text-field v-model.number="newBouquet.inventoryCount" label="Stock" type="number" variant="outlined" color="#2D5A27"></v-text-field>
            </v-col>
          </v-row>
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
            :loading="isSaving"
          >
            {{ isEditing ? 'Save Changes' : 'Add to Collection' }}
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
const imageFile = ref(null)
const imagePreview = ref(null)
const isDragging = ref(false)
const isSaving = ref(false)

const newBouquet = ref({ name: '', price: 0, season: '', imageUrl: '', inventoryCount: 0 })

// Actions
const openAddDialog = () => {
  isEditing.value = false
  currentEditId.value = null
  imageFile.value = null
  imagePreview.value = null
  newBouquet.value = { name: '', price: 0, season: '', imageUrl: '', inventoryCount: 0 }
  showAddDialog.value = true
}

const openEditDialog = (item) => {
  isEditing.value = true
  currentEditId.value = item.id
  imageFile.value = null
  imagePreview.value = item.imageUrl
  newBouquet.value = { ...item }
  showAddDialog.value = true
}

const handleDrop = (e) => {
  isDragging.value = false
  const files = e.dataTransfer.files
  if (files[0]) processFile(files[0])
}

const handleFileSelect = (e) => {
  const files = e.target.files
  if (files[0]) processFile(files[0])
}

const processFile = (file) => {
  imageFile.value = file
  imagePreview.value = URL.createObjectURL(file)
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
  isSaving.value = true
  try {
    const formData = new FormData()
    
    formData.append('Name', newBouquet.value.name)
    formData.append('Price', Number(newBouquet.value.price))
    formData.append('Season', newBouquet.value.season)
    formData.append('InventoryCount', Number(newBouquet.value.inventoryCount))
    formData.append('IsAvailable', true)
    
    if (imageFile.value) {
      formData.append('ImageFile', imageFile.value)
    }

    const method = isEditing.value ? 'PUT' : 'POST'
    const url = isEditing.value 
      ? `${config.public.apiBase}/bouquets/${currentEditId.value}` 
      : `${config.public.apiBase}/bouquets`

    await $fetch(url, {
      method: method,
      body: formData,
      headers: { Authorization: `Bearer ${props.token}` }
    })

    showAddDialog.value = false
    await fetchManagementProducts()
  } catch (err) {
    console.error("Upload failed:", err.data)
    if (err.status === 401) emit('session-expired')
  } finally {
    isSaving.value = false
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