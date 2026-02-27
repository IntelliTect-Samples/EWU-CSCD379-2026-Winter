<template>
  <div class="page">
    <h1>Your Cart</h1>

    <div v-if="cart.length === 0" class="card">
      Cart is empty
    </div>

    <div v-if="cart.length > 0" class="grid">
      <div class="card" v-for="item in cart" :key="item.id">
        <h3>{{ item.name }}</h3>
        <p>Price: ${{ item.price }}</p>
        <p>Quantity: {{ item.quantity }}</p>
        <p>
          Subtotal:
          ${{ (Number(item.price) || 0) * (Number(item.quantity) || 0) }}
        </p>
      </div>
    </div>

    <div v-if="cart.length > 0" style="margin-top: 24px;">
      <h2>Total: ${{ total }}</h2>

      <button v-if="!showForm" @click="checkout">
        Checkout
      </button>
    </div>

    <div v-if="showForm" class="card form-card" style="margin-top: 24px;">
      <h2>Customer Information</h2>

      <div class="form-grid">
        <input v-model="customerName" placeholder="Full Name" />
        <input v-model="customerEmail" placeholder="Email" />
        <input v-model="customerPhone" placeholder="Phone Number" />
      </div>

      <button @click="submitOrder">
        Confirm Order
      </button>
    </div>
  </div>
</template>

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
    const price = Number(item.price) || 0
    const quantity = Number(item.quantity) || 0
    return sum + price * quantity
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

  const orderPayload = {
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

  try {
    const res = await createOrder(orderPayload)
    console.log('Order response:', res)
    alert("Order placed successfully!")
    localStorage.removeItem('cart')
    router.push('/')
  } catch (err) {
    console.error('Order failed:', err)
    alert('Failed to submit order: ' + (err.data?.title || err.message))
  }
}
</script>