<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { createOrder } from '~/services/api'

const router = useRouter()

const cart = ref([])
const showForm = ref(false)

const customerName = ref('')
const customerEmail = ref('')
const customerPhone = ref('')

onMounted(() => {
  cart.value = JSON.parse(localStorage.getItem('cart') || '[]')
})

const total = computed(() => {
  return cart.value.reduce((sum, item) => {
    return sum + item.price * item.quantity
  }, 0)
})

const checkout = () => {
  showForm.value = true
}

const submitOrder = async () => {
  if (!customerName.value || !customerEmail.value || !customerPhone.value) {
    alert("Please fill all fields")
    return
  }

  const order = {
    CustomerName: customerName.value,
    CustomerEmail: customerEmail.value,
    CustomerPhone: customerPhone.value,
    TotalAmount: total.value,
    Status: "Pending",
    OrderItems: cart.value.map(item => ({
      ProductId: item.id,
      Quantity: item.quantity,
      UnitPrice: item.price
    }))
  }

  await createOrder(order)

  alert("Order placed successfully!")

  localStorage.removeItem('cart')
  router.push('/')
}
</script>

<template>
  <div>
    <h1>Your Cart</h1>

    <div v-if="cart.length === 0">
      Cart is empty
    </div>

    <div v-for="item in cart" :key="item.id">
      <h3>{{ item.name }}</h3>
      <p>Price: ${{ item.price }}</p>
      <p>Quantity: {{ item.quantity }}</p>
      <p>Subtotal: ${{ item.price * item.quantity }}</p>
      <hr />
    </div>

    <h2 v-if="cart.length > 0">Total: ${{ total }}</h2>

    <!-- STEP 1: CLICK CHECKOUT -->
    <button v-if="cart.length > 0 && !showForm" @click="checkout">
      Checkout
    </button>

    <!-- STEP 2: SHOW FORM -->
    <div v-if="showForm">
      <h2>Customer Information</h2>

      <input v-model="customerName" placeholder="Full Name" />
      <input v-model="customerEmail" placeholder="Email" />
      <input v-model="customerPhone" placeholder="Phone Number" />

      <br /><br />

      <button @click="submitOrder">
        Confirm Order
      </button>
    </div>
  </div>
</template>