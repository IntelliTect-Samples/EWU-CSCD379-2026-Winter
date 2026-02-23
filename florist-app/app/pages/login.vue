<template>
  <div class="admin-page-wrapper">
    <ClientOnly>
      <PetalBackground />
    </ClientOnly>
    <v-container class="py-16 position-relative" style="z-index: 1;">
      
      <v-row v-if="!isAuthenticated" justify="center" align="center" style="min-height: 60vh;">
        <v-col cols="12" md="5" lg="4">
          <v-card class="glass-card pa-10 text-center" elevation="0">
            <h1 class="display-main mb-2">Garden Portal</h1>
            
            <v-text-field
              v-model="loginForm.username"
              label="Username"
              variant="outlined"
              color="#B64995"
              class="mb-4 boutique-input"
            ></v-text-field>

            <v-text-field
              v-model="loginForm.password"
              label="Password"
              type="password"
              variant="outlined"
              color="#B64995"
              class="mb-8 boutique-input"
            ></v-text-field>

            <v-alert v-if="loginError" color="#d18b99" variant="tonal" icon="mdi-flower-pollen" class="mb-6 text-left">
              The garden gate remains locked. Please check your credentials.
            </v-alert>

            <v-btn
              block
              size="x-large"
              color="#2D5A27"
              variant="flat"
              rounded="xl"
              class="letter-spacing-2"
              @click="handleLogin"
            >
              Enter Portal
            </v-btn>
          </v-card>
        </v-col>
      </v-row>

      <div v-else class="fade-in">
        <v-row align="end" class="mb-12">
          <v-col cols="12" md="8">
            <h1 class="display-main">Collection Management</h1>
            <p class="brand-ethos">
              CURRENT ACCESS: <span class="text-green-darken-3 font-weight-bold">{{ userRole }}</span>
            </p>
          </v-col>
          <v-col cols="12" md="4" class="text-md-right">
            <v-btn variant="text" color="grey-darken-1" @click="logout" class="mr-4">Sign Out</v-btn>
            <v-btn 
              v-if="userRole === 'ADMIN'" 
              color="#2D5A27" 
              variant="flat" 
              rounded="xl" 
              prepend-icon="mdi-plus"
              class="px-6"
              @click="showAddDialog = true"
            >
              Add New Stem
            </v-btn>
          </v-col>
        </v-row>

        <v-dialog v-model="showAddDialog" max-width="500px">
          <v-card class="glass-card pa-8">
            <h2 class="display-main mb-4">New Arrangement</h2>
            <v-text-field v-model="newBouquet.name" label="Name" variant="outlined" color="#B64995"></v-text-field>
            <v-text-field v-model="newBouquet.price" label="Price" type="number" variant="outlined" color="#B64995"></v-text-field>
            <v-select v-model="newBouquet.season" :items="['Spring', 'Summer', 'Autumn', 'Winter']" :rules="seasonRules" label="Select Season" required variant="outlined" color="#B64995"></v-select>
            <v-text-field v-model="newBouquet.imageUrl" label="Image URL" variant="outlined" color="#B64995"></v-text-field>
            
            <v-btn block color="#2D5A27" size="large" rounded="xl" class="text-white mt-4" @click="saveNewBouquet">
              Save to Collection
            </v-btn>
          </v-card>
        </v-dialog>

        <v-row>
          <v-col v-for="item in products" :key="item.id" cols="12">
            <v-card class="glass-card mb-4 pa-4 d-flex align-center" elevation="0">
              <v-avatar size="60" class="mr-6 rounded-lg">
                <v-img :src="item.imageUrl" cover></v-img>
              </v-avatar>
              
              <div class="flex-grow-1">
                <h3 class="staff-name" style="font-size: 1.2rem;">{{ item.name }}</h3>
                <span class="brand-ethos" style="font-size: 0.6rem;">{{ item.season }} COLLECTION</span>
              </div>

              <div class="text-right px-6">
                <p class="staff-name" style="font-size: 1.2rem;">${{ item.price }}</p>
              </div>

              <div v-if="userRole === 'ADMIN'" class="admin-actions ml-4">
                <v-btn icon="mdi-pencil-outline" variant="text" color="#2D5A27"></v-btn>
                <v-btn icon="mdi-trash-can-outline" variant="text" color="#d18b99" @click="deleteStem(item.id)"></v-btn>
              </div>
            </v-card>
          </v-col>
        </v-row>
      </div>
    </v-container>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import '~/assets/css/login.css'

const router = useRouter()
const config = useRuntimeConfig()
const isAuthenticated = ref(false)
const userRole = ref('') 
const loginError = ref(false)
const loginForm = ref({ username: '', password: '' })

const tokenCookie = useCookie('auth_token', { maxAge: 60 * 60 * 3 }) // 3 hours

const products = ref([])

const showAddDialog = ref(false)

const newBouquet = ref({ 
  name: '', 
  price: 0, 
  season: '', 
  imageUrl: '' 
})

const seasonRules = [v => !!v || 'You must choose a season for this arrangement']

const saveNewBouquet = async () => {
  try {
    await $fetch(`${config.public.apiBase}/bouquets`, {
      method: 'POST',
      body: newBouquet.value,
      headers: {
        Authorization: `Bearer ${tokenCookie.value}`
      }
    })
    
    showAddDialog.value = false
    await fetchManagementProducts()
    
    // Reset the form for next time
    newBouquet.value = { name: '', price: 0, season: 'Spring', imageUrl: '' }
  } catch (err) {
    console.error("The garden rejected the new stem:", err)
    alert("Error saving the bouquet. Ensure you are logged in as Admin.")
  }
}

const handleLogin = async () => {
  loginError.value = false

  try {
    const response = await $fetch(`${config.public.apiBase}/auth/login`, {
      method: 'POST',
      body: loginForm.value
    })

    if (response && response.token) {
      tokenCookie.value = response.token 
      isAuthenticated.value = true
      userRole.value = response.role ? response.role.toUpperCase() : 'STAFF'
      await fetchManagementProducts()
    }
  } catch (err) {
    loginError.value = true
  }
}

const fetchManagementProducts = async () => {
  try {
    const data = await $fetch(`${config.public.apiBase}/bouquets`, {
      headers: {
        Authorization: `Bearer ${tokenCookie.value}`
      }
    })
    products.value = data
  } catch (err) {
    console.error("Could not load the stems:", err)
  }
}

const deleteStem = async (id) => {
  if (!confirm("Are you sure you want to remove this arrangement from the collection?")) return

  try {
    await $fetch(`${config.public.apiBase}/bouquets/${id}`, {
      method: 'DELETE',
      headers: {
        Authorization: `Bearer ${tokenCookie.value}`
      }
    })
    // Refresh the list after deletion
    await fetchManagementProducts()
  } catch (err) {
    console.error("Could not prune the stem:", err)
    alert("Permission denied. Only Admins can delete items.")
  }
}

onMounted(async () => {
  if (tokenCookie.value) {
    try {
      // You could create a /auth/me endpoint that returns user info based on token
      // For now, we'll just assume the token is valid for the UI
      isAuthenticated.value = true
      // You might want to store the role in a cookie too so it persists on refresh
    } catch (e) {
      tokenCookie.value = null
    }
  }
})

const logout = () => {
  tokenCookie.value = null
  isAuthenticated.value = false
  userRole.value = ''
  loginForm.value = { username: '', password: '' }
  router.push('/login')
}
</script>