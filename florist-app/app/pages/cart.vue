<template>
    <ClientOnly>
      <PetalBackground />
    </ClientOnly>
  <v-container class="py-16 position-relative" style="z-index: 1;">
    <header class="text-center mb-12">
      <h1 class="cart-item-title">Your Bouquet</h1>
      <p class="brand-ethos">Review your selected stems</p>
    </header>

    <v-row justify="center">
      <v-col cols="12" md="8">
        <v-card v-if="cart.length > 0" class="glass-card pa-6" elevation="0">
          <v-list bg-color="transparent">
            <v-list-item v-for="item in cart" :key="item.cartId" class="mb-6">
              <template v-slot:prepend>
                <v-avatar size="100" rounded="lg" class="mr-4">
                  <v-img :src="item.imageUrl" cover></v-img>
                </v-avatar>
              </template>
              
              <v-list-item-title class="cart-item-title" style="font-size: 1.5rem;">
                {{ item.name }}
              </v-list-item-title>
              
              <v-list-item-subtitle class="cart-item-meta mt-1">
                {{ item.season }} Collection — ${{ item.price }}
              </v-list-item-subtitle>

              <template v-slot:append>
                <v-btn 
                  icon="mdi-close" 
                  variant="text" 
                  color="grey-lighten-1" 
                  @click="removeFromCart(item.cartId)"
                ></v-btn>
              </template>
            </v-list-item>
          </v-list>

          <v-divider class="my-6"></v-divider>

          <div class="d-flex justify-space-between align-center px-4">
            <div>
              <p class="cart-subtitle mb-0">Total Arrangement</p>
              <h2 class="cart-item-title" style="font-size: 2rem;">${{ cartTotal }}</h2>
            </div>
            <v-btn 
              color="#2D5A27" 
              size="x-large" 
              rounded="xl" 
              class="px-10 cart-checkout-btn"
              @click="checkout"
            >
              Checkout
            </v-btn>
          </div>
        </v-card>

        <v-card v-else class="pa-16 text-center" elevation="0">
          <p class="cart-item-meta mb-8">Your basket is currently empty.</p>
          <v-btn to="/shop" 
            color="#2D5A27" 
            size="large" 
            variant="flat" 
            rounded="xl" 
            class="cart-checkout-btn px-10"
          > 
            Return to Collection
          </v-btn>
        </v-card>
      </v-col>
    </v-row>
    <v-dialog v-model="checkoutComplete" max-width="500" persistent>
      <v-card class="pa-10 text-center"style="border-radius: 24px; background: rgba(255, 255, 255, 0.95); backdrop-filter: blur(10px);">
        <h2 class="cart-item-title text-h4 mb-4">A Garden Awaits</h2>
        <p class="cart-item-meta mb-8" style="font-size: 1.1rem; line-height: 1.6;">
          Thank you for your order!. Our florists will begin gathering your stems.
        </p>
        <v-btn 
          color="#2D5A27" 
          variant="flat" 
          rounded="xl" 
          block 
          class="cart-checkout-btn text-white"
          @click="closeSuccess"
        >
          Continue Wandering
        </v-btn>
      </v-card>
    </v-dialog>

  </v-container>
</template>

<script setup>
import { ref } from 'vue'
import '~/assets/css/cart.css'
const { cart, removeFromCart, cartTotal } = useCart()

const checkoutComplete = ref(false)

const checkout = () => {
  checkoutComplete.value = true
}

const closeSuccess = () => {
  checkoutComplete.value = false
  cart.value = []
  navigateTo('/shop')
}
</script>