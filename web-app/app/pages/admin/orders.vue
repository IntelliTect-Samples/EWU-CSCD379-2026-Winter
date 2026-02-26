<script setup>
import { ref, onMounted } from 'vue'
import { getProducts } from '~/services/api'

definePageMeta({
  middleware: 'admin'
})

const config = useRuntimeConfig()
const orders = ref([])
const products = ref([])

const load = async () => {
  const token = localStorage.getItem("accessToken")

  const res = await fetch(`${config.public.apiBase}/api/orders`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  })

  if (!res.ok) {
    console.error("Failed to fetch orders:", res.status)
    return
  }

  orders.value = await res.json()
  products.value = await getProducts()
}

onMounted(load)

const findProduct = (id) => products.value.find(p => p.id === id)

const formatDate = (iso) => {
  try { return new Date(iso).toLocaleString() } catch { return iso }
}

const updateStatus = async (orderId, status) => {
  if (!confirm(`Change order #${orderId} status to '${status}'?`)) return

  const token = localStorage.getItem("accessToken")

  const res = await fetch(`${config.public.apiBase}/api/orders/${orderId}/status`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    },
    body: JSON.stringify({ status })
  })

  if (!res.ok) {
    console.error("Failed to update status:", res.status)
    return
  }

  await load()
}
</script>

<template>
  <div class="admin-orders">
    <h1>Manage Orders</h1>

    <div v-if="orders.length === 0">No orders yet.</div>

    <div class="order" v-for="o in orders" :key="o.id">
      <div class="order-header">
        <div>
          <h3>Order #{{ o.id }} <small>({{ formatDate(o.createdAt) }})</small></h3>
          <p><strong>{{ o.customerName }}</strong> — {{ o.customerEmail }} | {{ o.customerPhone }}</p>
        </div>
        <div class="status-block">
          <div class="status">Status: <span>{{ o.status }}</span></div>
          <div class="actions">
            <button v-if="o.status === 'Pending'" @click="updateStatus(o.id, 'Approved')">Approve</button>
            <button v-if="o.status !== 'Completed' && o.status !== 'Cancelled'" @click="updateStatus(o.id, 'Completed')">Complete</button>
            <button v-if="o.status !== 'Cancelled' && o.status !== 'Completed'" @click="updateStatus(o.id, 'Cancelled')">Cancel</button>
          </div>
        </div>
      </div>

      <div class="order-body">
        <div class="items">
          <h4>Items</h4>
          <div v-for="item in o.orderItems" :key="item.id" class="item-row">
            <img v-if="findProduct(item.productId) && findProduct(item.productId).imageUrl" :src="findProduct(item.productId).imageUrl" alt="product" />
            <div class="item-info">
              <div class="item-name">{{ findProduct(item.productId)?.name || ('Product #' + item.productId) }}</div>
              <div>Qty: {{ item.quantity }} • Unit: ${{ item.unitPrice }}</div>
            </div>
          </div>
        </div>

        <div class="summary">
          <p><strong>Total:</strong> ${{ o.totalAmount }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.admin-orders { padding:20px; font-family: 'Helvetica Neue', Arial, sans-serif; }
.order { border:1px solid #f0dce6; padding:14px; margin-bottom:14px; border-radius:8px; background: #fff; box-shadow:0 4px 14px rgba(105,82,114,0.04); }
.order-header { display:flex; justify-content:space-between; align-items:center; }
.order-header h3 { margin:0; color:#6d3b57; }
.status-block { text-align:right; }
.status span { font-weight:700; color:#a33b6a; }
.actions button { margin-left:8px; background:#ffd6e6; border:none; padding:8px 10px; border-radius:6px; cursor:pointer; }
.order-body { display:flex; gap:20px; margin-top:12px; }
.items { flex:1; }
.item-row { display:flex; gap:12px; align-items:center; margin-bottom:10px; }
.item-row img { width:72px; height:72px; object-fit:cover; border-radius:6px; }
.item-info { font-size:14px; }
.summary { min-width:160px; display:flex; align-items:center; justify-content:flex-end; font-weight:700; }
</style>
