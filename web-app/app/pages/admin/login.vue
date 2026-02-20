<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const email = ref('')
const password = ref('')
const router = useRouter()

const login = async () => {
  // Later connect to backend
  if (email.value && password.value) {
    localStorage.setItem("admin", "true")
    router.push('/admin/dashboard')
  }
}
</script>

<template>
  <div>
    <h1>Admin Login</h1>

    <input v-model="email" placeholder="Email" />
    <input v-model="password" type="password" placeholder="Password" />

    <button @click="login">Login</button>
  </div>
</template>