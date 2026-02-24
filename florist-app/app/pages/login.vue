<template>
  <div class="admin-page-wrapper">
    <ClientOnly><PetalBackground /></ClientOnly>
    
    <v-container class="py-16 position-relative" style="z-index: 1;">
      <v-row v-if="!isAuthenticated" justify="center" align="center" style="min-height: 60vh;">
        <v-col cols="12" md="5" lg="4">
          <v-card class="glass-card pa-10 text-center" elevation="0">
            <h1 class="display-main mb-2">Garden Portal</h1>
            <v-form @submit.prevent="handleLogin">
              <v-text-field 
                autofocus
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
                The garden gate remains locked.
              </v-alert>

              <v-btn 
                type="submit" 
                block 
                size="x-large" 
                color="#2D5A27" 
                variant="flat" 
                rounded="xl"
              >
                Enter Portal
              </v-btn>
            </v-form>
          </v-card>
        </v-col>
      </v-row>

      <ManagementPortal 
        v-else 
        :userRole="userRole" 
        :username="userCookie"  :token="tokenCookie"
        @logout="logout"
        @session-expired="sessionExpired = true"
      />

      <v-dialog v-model="sessionExpired" persistent max-width="400">
        <v-card class="pa-8 text-center" style="background-color: #fdfaf8; border: 1px solid #e0e0e0; border-radius: 24px;">
          <h2 class="display-main mb-2">Session Wilted</h2>
          <p class="brand-ethos mb-6">Please login again.</p>
          <v-btn block color="#2D5A27" variant="flat" rounded="xl" @click="logout">Re-enter Portal</v-btn>
        </v-card>
      </v-dialog>
      
    </v-container>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import '~/assets/css/login.css'

const router = useRouter()
const config = useRuntimeConfig()
const isAuthenticated = ref(false)
const userRole = ref('') 
const loginError = ref(false)
const loginForm = ref({ Username: '', Password: '' })
const sessionExpired = ref(false)

const tokenCookie = useCookie('auth_token', { maxAge: 2100 }) 
const userCookie = useCookie('user_name', { maxAge: 2100 })
const roleCookie = useCookie('user_role', { maxAge: 2100 })

const handleLogin = async () => {
  loginError.value = false
  try {
    const response = await $fetch(`${config.public.apiBase}/auth/login`, {
      method: 'POST',
      body: loginForm.value,
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json' 
      }
    })
    if (response && response.token) {
      tokenCookie.value = response.token 
      roleCookie.value = response.role.toUpperCase() 
      userCookie.value = response.username || 'Admin'
      isAuthenticated.value = true
      userRole.value = roleCookie.value
    }
  } catch (err) {
    loginError.value = true
  }
}

const logout = () => {
  tokenCookie.value = null
  roleCookie.value = null
  isAuthenticated.value = false
  userRole.value = ''
  sessionExpired.value = false
  router.push('/login')
}

onMounted(() => {
  if (tokenCookie.value && roleCookie.value) {
    isAuthenticated.value = true
    userRole.value = roleCookie.value
  }
})
</script>