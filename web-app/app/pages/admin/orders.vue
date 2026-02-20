<script setup>
import { ref, onMounted } from 'vue'

const orders = ref([])

onMounted(async () => {
  const res = await fetch("http://localhost:5000/api/orders")
  orders.value = await res.json()
})
</script>

<template>
  <div>
    <h1>Manage Orders</h1>

    <div v-for="o in orders" :key="o.id">
      <p>{{ o.customerName }} - {{ o.status }}</p>
    </div>
  </div>
</template>