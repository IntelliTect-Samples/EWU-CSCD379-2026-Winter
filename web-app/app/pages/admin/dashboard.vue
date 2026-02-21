<script setup>
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

onMounted(() => {
  const admin = localStorage.getItem("isAdmin")
  if (!admin) {
    router.push('/admin/login')
  }
})

const logout = () => {
  localStorage.removeItem("isAdmin")
  router.push('/')
}
</script>

<template>
  <div>
    <h1>Admin Dashboard</h1>

    <NuxtLink to="/admin/products">
      <button>Manage Products</button>
    </NuxtLink>

    <NuxtLink to="/admin/orders">
      <button>Manage Orders</button>
    </NuxtLink>

    <br /><br />

    <button @click="logout">Logout</button>
  </div>
</template>