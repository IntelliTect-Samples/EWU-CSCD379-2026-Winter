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
  <div class="products-page">
    <h1>Our Cakes</h1>
    <div class="grid">
      <div class="card" v-for="p in products" :key="p.id">
        <div class="img-wrap">
          <img v-if="p.imageUrl" :src="p.imageUrl" :alt="p.name" />
          <div v-else class="placeholder">No Image</div>
        </div>
        <div class="card-body">
          <h3>{{ p.name }}</h3>
          <h6 class="product-description">
            {{ p.description }}
          </h6>
          <p class="price">${{ p.price }}</p>
          <button class="order" @click="addToCart(p)">Place Order</button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.products-page { padding:28px; font-family: 'Helvetica Neue', Arial, sans-serif; }
.grid { display:grid; grid-template-columns: repeat(auto-fill,minmax(220px,1fr)); gap:18px; margin-top:18px; }
.card { background:white; border-radius:10px; padding:12px; box-shadow:0 6px 18px rgba(105,82,114,0.06); display:flex; flex-direction:column; align-items:center; }
.img-wrap { width:100%; height:150px; display:flex; align-items:center; justify-content:center; overflow:hidden; border-radius:8px; background:#fff7fb; }
.img-wrap img { width:100%; height:100%; object-fit:cover; }
.placeholder { color:#9b8a9b; }
.card-body { text-align:center; margin-top:10px; width:100%; }
.price { color:#a33b6a; font-weight:700; margin:6px 0; }
.order { background:#ff7faa; color:white; border:none; padding:8px 12px; border-radius:8px; cursor:pointer; font-weight:700; }
</style>
