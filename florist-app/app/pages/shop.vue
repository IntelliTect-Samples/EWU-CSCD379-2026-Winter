<template>
  <div>
    <PetalBackground />
    <v-container fluid class="shop-container px-md-16 py-10">
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
          <v-hover v-slot="{ isHovering, props }">
            <v-card v-bind="props" flat class="product-card">
              <div class="image-container mb-4">
                <v-img
                  :src="product.imageUrl"
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
                <div class="image-overlay" :class="{ 'show-overlay': isHovering }">
                  <v-btn variant="text" class="view-btn" @click="openDetails(product.id)">
                    View Details
                  </v-btn>
                </div>
              </div>
              
              <div class="product-info text-center">
                <h3 class="product-name">{{ product.name }}</h3>
                <p class="product-price">${{ product.price }}</p>
              </div>
            </v-card>
          </v-hover>
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

const activeFilter = ref('All')
const products = ref([])
const pending = ref(true)

// --- FORMATTER ---
// This turns 185 into $185.00 automatically
const formatPrice = (value) => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
  }).format(value)
}

// --- DATA FETCHING ---
const fetchProducts = async () => {
  pending.value = true
  try {
    // Simulating the "Azure Trip" delay
    await new Promise(resolve => setTimeout(resolve, 1000))
    
    products.value = [
      {
        id: 1,
        name: "Spring Awakening",
        price: 185.00,
        season: "Spring",
        imageUrl: "/images/spring-flowers.jpg"
      },
      {
        id: 2,
        name: "Summer Solstice",
        price: 155.00,
        season: "Summer",
        imageUrl: "/images/summer-flowers.jpg"
      },
      {
        id: 3,
        name: "Autumn Glow",
        price: 210.00,
        season: "Autumn",
        imageUrl: "/images/fall-flowers.jpg"
      },
      {
        id: 4,
        name: "Winter's Embrace",
        price: 195.00,
        season: "Winter",
        imageUrl: "/images/winter-flowers.jpg"
      }
    ]
  } catch (error) {
    console.error("Database connection error:", error)
  } finally {
    pending.value = false
  }
}

onMounted(() => {
  fetchProducts()
})

// --- FILTER LOGIC ---
const filteredProducts = computed(() => {
  if (activeFilter.value === 'All') return products.value
  return products.value.filter(p => p.season === activeFilter.value)
})

const openDetails = (id) => {
  console.log("Navigating to product:", id)
  // Logic for a future Detail view or Quick View popup
}
</script>