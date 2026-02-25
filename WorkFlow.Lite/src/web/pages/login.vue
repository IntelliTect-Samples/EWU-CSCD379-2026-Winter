<template>
  <div class="d-flex justify-center">
    <v-card class="pa-4" style="max-width: 520px; width: 100%">
      <div class="text-h5 font-weight-medium mb-1">Sign in</div>
      <div class="text-body-2 text-medium-emphasis mb-4">
        Use your WorkFlow Lite account to manage work orders.
      </div>

      <v-alert v-if="msg" class="mb-3" :type="msgType" variant="tonal" border="start">
        {{ msg }}
      </v-alert>

      <v-text-field v-model="email" label="Email" autocomplete="username" variant="outlined" />
      <v-text-field
        v-model="password"
        label="Password"
        autocomplete="current-password"
        type="password"
        variant="outlined"
      />

      <v-btn color="primary" variant="flat" block class="mt-2" :loading="loading" @click="doLogin">
        Sign in
      </v-btn>

      <v-btn variant="text" block class="mt-2" :loading="loading" @click="doRegister">
        Create an account
      </v-btn>

      <v-divider class="my-4" />

      <div class="text-caption text-medium-emphasis">
        Tip: After signing in, go to <b>My Requests</b> to create and track work orders.
      </div>
    </v-card>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { navigateTo } from 'nuxt/app'
import { useAuthService } from '../composables/auth'

const email = ref('')
const password = ref('')
const msg = ref('')
const msgType = ref<'error' | 'success' | 'info'>('info')
const loading = ref(false)

const auth = useAuthService()

async function doRegister() {
  msg.value = ''
  loading.value = true
  try {
    await auth.register(email.value, password.value)
    msgType.value = 'success'
    msg.value = 'Account created. Now sign in.'
  } catch {
    msgType.value = 'error'
    msg.value = 'Registration failed.'
  } finally {
    loading.value = false
  }
}

async function doLogin() {
  msg.value = ''
  loading.value = true
  try {
    await auth.login(email.value, password.value)
    await navigateTo('/my-requests')
  } catch {
    msgType.value = 'error'
    msg.value = 'Sign-in failed. Check your credentials.'
  } finally {
    loading.value = false
  }
}
</script>