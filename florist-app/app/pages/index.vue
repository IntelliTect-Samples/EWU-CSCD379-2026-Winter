<template>
  <div>
    <ClientOnly>
      <PetalBackground />
    </ClientOnly>
    <v-container class="py-16 position-relative" style="z-index: 1;">
      <v-row justify="center" align="center" style="min-height: 40vh">
        <v-col cols="12" md="8" class="text-center">
          <h1 class="display-main mb-2">Bloom & Stem</h1>
          <p class="category-subtitle mb-6">
            Exquisite Botanicals • Expertly Curated • Hand-Delivered
          </p>
          <v-btn 
            to="/shop" 
            color="#2D5A27" 
            size="large" 
            variant="flat" 
            rounded="xl" 
            class="shop-cta-btn px-10"
          >
            Shop the Collection
          </v-btn>
        </v-col>
      </v-row>

      <v-row class="editorial-row pb-16" align="center">
        <v-col cols="12" md="6" class="pr-md-10">
          <v-img
            src="/images/main-flower-page.jpg"
            cover
            height="600"
            class="editorial-image"
            rounded="lg"
          ></v-img>
        </v-col>
        <v-col cols="12" md="5" class="offset-md-1 text-left">
          <h2 class="display-main mb-6" style="font-size: clamp(2rem, 5vw, 3.5rem);"
          >
            Every Bloom Tells a Story
          </h2>
          <p class="editorial-text mb-6">
            At Bloom & Stem, we source the finest seasonal botanicals from local growers to 
            create arrangements that are more than just decorative. 
            We believe every arrangement should tell a story, and that our customers should feel 
            the care and thoughtfulness that fits their unique style.
          </p>
        </v-col>
      </v-row>
    </v-container>
  </div>
</template>

<script setup>
import '~/assets/css/index.css'
</script>

