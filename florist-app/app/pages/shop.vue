<template>
  <div>
    <ClientOnly>
      <PetalBackground />
    </ClientOnly>
    <v-container class="py-16 position-relative" style="z-index: 1;">
      <header class="shop-header mb-16 text-center">
        <h2 class="category-title">The Collection</h2>
        <p class="category-subtitle">botanicals for every season</p>
        
        <div class="filter-bar d-flex justify-center ga-8 mt-8">
          <span 
            v-for="cat in ['All', 'Spring', 'Summer', 'Autumn', 'Winter']" 
            :key="cat"
            class="filter-item"
            :class="{ active: activeFilter === cat }"
            @click="activeFilter = cat"
          >
            {{ cat }}
          </span>
        </div>
      </header>

      <v-row v-if="pending">
        <v-col v-for="n in 6" :key="n" cols="12" sm="6" md="4">
          <v-skeleton-loader type="card, text" class="bg-transparent"></v-skeleton-loader>
        </v-col>
      </v-row>

      <v-row v-else>
        <v-col v-for="product in filteredProducts" :key="product.id" cols="12" sm="6" md="4">
          <v-card flat class="product-card d-flex flex-column h-100">
            <div class="image-container mb-4">
              <v-img
                :src="resolveImageUrl(product.imageUrl)"
                cover
                aspect-ratio="0.75"
                class="product-image"
              >
                <template v-slot:placeholder>
                  <v-row class="fill-height ma-0" align="center" justify="center">
                    <v-progress-circular indeterminate color="pink-lighten-4"></v-progress-circular>
                  </v-row>
                </template>
              </v-img>
            </div>
            
            <div class="product-info px-4">
              <h3 class="product-name text-center mb-2">{{ product.name }}</h3>
            </div>

            <v-spacer></v-spacer>

            <v-card-actions class="px-4 pb-4 pt-0 d-flex justify-space-between align-center">
              <span class="product-price text-subtitle-1 font-weight-bold">
                {{ formatPrice(product.price) }}
              </span>
              
              <v-btn
                color="pink-lighten-4"
                variant="flat"
                prepend-icon="mdi-cart-plus"
                @click="addToCart(product)"
                class="add-cart-btn"
              >
                Add
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-col>
      </v-row>

      <v-row v-if="!pending && filteredProducts.length === 0" class="justify-center py-10">
        <p class="text-grey-darken-1 italic">No arrangements found for this season.</p>
      </v-row>
    </v-container>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import '~/assets/css/shop.css'
const config = useRuntimeConfig()
const { addToCart } = useCart() 

const activeFilter = ref('All')
const products = ref([])
const pending = ref(true)

const resolveImageUrl = (path) => {
  if (path.startsWith('http')) return path
  if (path.startsWith('/images/')) {
    return path
  }

  if (path.startsWith('/uploads/')) {
    const apiRoot = config.public.apiBase.replace(/\/api$/, '')
    return `${apiRoot}${path}`
  }

  return path
}

const fetchProducts = async () => {
  pending.value = true
  try {
    const data = await $fetch(`${config.public.apiBase}/bouquets`)
    products.value = data
  } catch (error) {
    console.error("Database connection error:", error)
  } finally {
    pending.value = false
  }
}

onMounted(() => {
  fetchProducts()
})

const filteredProducts = computed(() => {
  if (activeFilter.value === 'All') return products.value
  return products.value.filter(p => p.season === activeFilter.value)
})

const formatPrice = (value) => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
  }).format(value)
}
</script>