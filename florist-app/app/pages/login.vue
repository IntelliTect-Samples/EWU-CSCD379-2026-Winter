<template>
  <div class="admin-page-wrapper">
    <ClientOnly>
      <PetalBackground />
    </ClientOnly>
    <v-container class="position-relative py-16" style="z-index: 1">
      
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
            >
              Add New Stem
            </v-btn>
          </v-col>
        </v-row>

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
                <v-btn icon="mdi-trash-can-outline" variant="text" color="#d18b99"></v-btn>
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
const config = useRuntimeConfig() // Accesses the NUXT_PUBLIC_API_BASE we set in Azure

const isAuthenticated = ref(false)
const userRole = ref('') 
const loginError = ref(false)
const loginForm = ref({ username: '', password: '' })

// This will hold our JWT token in the browser
const tokenCookie = useCookie('auth_token', { maxAge: 60 * 60 * 3 }) // 3 hours

const products = ref([
  { id: 1, name: "Spring Awakening", price: 185.00, season: "Spring", imageUrl: "images/spring-flowers.jpg" },
  { id: 2, name: "Autumn Glow", price: 210.00, season: "Autumn", imageUrl: "images/fall-flowers.jpg" }
])

const handleLogin = async () => {
  loginError.value = false

  try {
    // 1. CALL YOUR REAL .NET API
    // The endpoint matches your AuthController [HttpPost("login")]
    const { data, error } = await $fetch(`${config.public.apiBase}/auth/login`, {
      method: 'POST',
      body: loginForm.value
    })

    if (data) {
      // 2. SUCCESS! Save the token
      tokenCookie.value = data // The API returns the JWT string
      isAuthenticated.value = true
      
      // 3. SET ROLE (Assuming your API returns role or you decode it)
      // For now, let's peek at the username to set the UI role
      // In a real app, you'd decode the JWT to get the 'role' claim
      userRole.value = loginForm.value.username.toUpperCase() === 'ADMIN' ? 'ADMIN' : 'STAFF'
      
      console.log("Logged in successfully!")
    }
  } catch (err) {
    // 4. FAIL! Show the "Garden gate locked" alert
    console.error("Login failed:", err)
    loginError.value = true
  }
}

const logout = () => {
  tokenCookie.value = null // Clear the cookie
  isAuthenticated.value = false
  userRole.value = ''
  loginForm.value = { username: '', password: '' }
  router.push('/login')
}
</script>