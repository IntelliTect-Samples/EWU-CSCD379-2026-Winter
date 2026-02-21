<script setup>
import { ref, onMounted } from 'vue'

const orders = ref([])

onMounted(async () => {
  const res = await fetch("http://localhost:5237/api/orders")
  orders.value = await res.json()
})
</script>

<template>
  <div>
    <h1>Manage Orders</h1>

    <div v-for="o in orders" :key="o.id" style="border:1px solid #ccc; padding:10px; margin-bottom:10px;">
      <p><strong>Name:</strong> {{ o.customerName }}</p>
      <p><strong>Email:</strong> {{ o.customerEmail }}</p>
      <p><strong>Phone:</strong> {{ o.customerPhone }}</p>
      <p><strong>Total:</strong> ${{ o.totalAmount }}</p>
      <p><strong>Status:</strong> {{ o.status }}</p>

      <h4>Items:</h4>
      <div v-for="item in o.orderItems" :key="item.id">
        <p>Product ID: {{ item.productId }} | Qty: {{ item.quantity }}</p>
      </div>
    </div>
  </div>
</template>