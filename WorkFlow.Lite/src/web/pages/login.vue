<template>
  <v-container class="py-6" style="max-width: 480px">
    <h1 class="text-h5 mb-4">Login</h1>

    <v-card>
      <v-card-text class="d-flex flex-column ga-3">
        <v-text-field v-model="email" label="Email" />
        <v-text-field v-model="password" label="Password" type="password" />
        <v-btn @click="doLogin" block>Login</v-btn>
        <v-btn variant="outlined" @click="doRegister" block>Create account</v-btn>
        <div class="text-caption">{{ msg }}</div>
      </v-card-text>
    </v-card>
  </v-container>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { navigateTo } from 'nuxt/app'
import { useAuthService } from '../composables/auth'

const email = ref('')
const password = ref('')
const msg = ref('')
const auth = useAuthService()

async function doRegister() {
  msg.value = ''
  try {
    await auth.register(email.value, password.value)
    msg.value = 'Registered! Now click Login.'
  } catch {
    msg.value = 'Register failed.'
  }
}

async function doLogin() {
  msg.value = ''
  try {
    await auth.login(email.value, password.value)
    await navigateTo('/my-requests')
  } catch {
    msg.value = 'Login failed.'
  }
}
</script>