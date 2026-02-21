<script setup>
import { ref, onMounted } from 'vue'
import { getProducts } from '~/services/api'

const products = ref([])

onMounted(async () => {
  products.value = await getProducts()
})

const addToCart = (product) => {
  const cart = JSON.parse(localStorage.getItem('cart') || '[]')

  const existing = cart.find(item => item.id === product.id)

  if (existing) {
    existing.quantity += 1
  } else {
    cart.push({ ...product, quantity: 1 })
  }

  localStorage.setItem('cart', JSON.stringify(cart))

  alert("Added to cart!")
}
</script>

<template>
  <div>
    <h1>Our Cakes</h1>

    <div v-for="p in products" :key="p.id">
      <h3>{{ p.name }}</h3>
      <p>${{ p.price }}</p>
      <img v-if="p.imageUrl" :src="p.imageUrl" width="150" />

      <!-- NEW BUTTON -->
      <button @click="addToCart(p)">Place Order</button>
    </div>
  </div>
</template>