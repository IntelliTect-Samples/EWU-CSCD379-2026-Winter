<template>
  <div>
    <ClientOnly>
      <PetalBackground />
    </ClientOnly>
    <v-container class="position-relative" style="z-index: 1">
      <v-row justify="center" align="center" style="min-height: 40vh">
        <v-col cols="12" md="8" class="text-center">
          <h1 class="display-main mb-2">Bloom & Stem</h1>
          <p class="text-uppercase tracking-widest text-grey-darken-1 mb-6">
            Exquisite Botanicals • Expertly Curated • Hand-Delivered
          </p>
          <v-btn to="/shop" 
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

      <v-row class="editorial-row py-16" align="center">
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
          <h2 class="editorial-title mb-6">The Art of the Bloom</h2>
          <p class="editorial-text mb-6">
            We believe that every stem tells a story. At Bloom & Stem, we source the finest seasonal botanicals from local growers to create arrangements that are more than just decorative.
          </p>
        </v-col>
      </v-row>
    </v-container>
  </div>
</template>

<script setup>
import '~/assets/css/index.css'
</script>

