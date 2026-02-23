<script setup>
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '~/composables/useAuth'

definePageMeta({
  middleware: 'admin'
})

const router = useRouter()
const { isAuthenticated, isAdmin, logout } = useAuth()

onMounted(() => {
  if (!isAuthenticated.value || !isAdmin.value) {
    router.push('/admin/login')
  }
})

const handleLogout = () => {
  logout()
  router.push('/')
}
</script>

<template>
  <div class="dashboard">
    <h1>Admin Dashboard</h1>

    <NuxtLink to="/admin/products">
      <button>Manage Products</button>
    </NuxtLink>

    <NuxtLink to="/admin/orders">
      <button>Manage Orders</button>
    </NuxtLink>

    <br /><br />

    <button @click="handleLogout">Logout</button>
  </div>
</template>