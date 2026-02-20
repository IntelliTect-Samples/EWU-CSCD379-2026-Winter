<script setup>
import { ref, onMounted } from 'vue'
import { getProducts } from '~/services/api'

const products = ref([])

onMounted(async () => {
  products.value = await getProducts()
})
</script>

<template>
  <div>
    <h1>Our Cakes</h1>

    <div v-for="p in products" :key="p.id">
      <h3>{{ p.name }}</h3>
      <p>${{ p.price }}</p>
      <img v-if="p.imageUrl" :src="p.imageUrl" width="150" />
    </div>
  </div>
</template>