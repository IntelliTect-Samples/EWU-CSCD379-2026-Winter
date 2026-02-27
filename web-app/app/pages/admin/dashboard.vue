<template>
  <div class="page">
    <h1>Admin Dashboard</h1>

    <div class="grid">
      <div class="card">
        <NuxtLink to="/admin/products">
          <button>Manage Products</button>
        </NuxtLink>
      </div>

      <div class="card">
        <NuxtLink to="/admin/orders">
          <button>Manage Orders</button>
        </NuxtLink>
      </div>
    </div>

    <div style="margin-top: 24px;">
      <button class="btn-danger" @click="handleLogout">
        Logout
      </button>
    </div>
  </div>
</template>

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
  router.push('/admin/login')
}
</script>

