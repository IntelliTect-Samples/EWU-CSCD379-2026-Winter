<template>
    <ClientOnly>
      <PetalBackground />
    </ClientOnly>
  <v-container class="py-16">
    <header class="text-center mb-12">
      <h1 class="display-main" style="font-size: 3rem;">Your Bouquet</h1>
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
              
              <v-list-item-title class="staff-name" style="font-size: 1.5rem;">
                {{ item.name }}
              </v-list-item-title>
              
              <v-list-item-subtitle class="editorial-text mt-1">
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
              <p class="brand-ethos mb-0">Total Arrangement</p>
              <h2 class="staff-name" style="font-size: 2rem;">${{ cartTotal }}</h2>
            </div>
            <v-btn 
              color="#2D5A27" 
              size="x-large" 
              rounded="xl" 
              class="px-10"
              @click="checkout"
            >
              Checkout
            </v-btn>
          </div>
        </v-card>

        <v-card v-else class="glass-card pa-16 text-center" elevation="0">
          <v-icon size="64" color="grey-lighten-2" class="mb-4">mdi-flower-outline</v-icon>
          <p class="editorial-text mb-8">Your basket is currently empty.</p>
          <v-btn to="/shop" variant="outlined" color="#2D5A27" rounded="xl">
            Return to Collection
          </v-btn>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup>
import '~/assets/css/NavigationBar.css'
const { cart, removeFromCart, cartTotal } = useCart()

const checkout = () => {
  alert("Thank you! Your order has been sent to our florists.")
  cart.value = []
}
</script>