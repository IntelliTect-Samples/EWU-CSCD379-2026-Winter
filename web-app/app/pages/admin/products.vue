<script setup>
import { ref, onMounted } from 'vue'
import { getProducts, addProduct, uploadImage, deleteProduct } from '~/services/api'

definePageMeta({
  middleware: 'admin'
})
const products = ref([])

const name = ref('')
const price = ref('')
const description = ref('')
const selectedFile = ref(null)

const loadProducts = async () => {
  products.value = await getProducts()
}

onMounted(loadProducts)

const handleFileChange = (event) => {
  selectedFile.value = event.target.files[0]
}

const submitProduct = async () => {
  if (!name.value || !price.value) {
    alert("Name and Price are required")
    return
  }
  let imageUrl = ""

  if (selectedFile.value) {
    const uploadResponse = await uploadImage(selectedFile.value)
    imageUrl = uploadResponse.imageUrl
  }

  await addProduct({
    name: name.value,
    description: description.value,
    price: Number(price.value),
    imageUrl: imageUrl
  })

  name.value = ''
  description.value = ''
  price.value = ''
  selectedFile.value = null

  await loadProducts()
}

const removeProduct = async (id) => {
  if (!confirm("Are you sure you want to delete this cake?")) return

  await deleteProduct(id)
  await loadProducts()
}
</script>

<template>
  <div>
    <h1>Manage Products</h1>

    <h3>Add New Cake</h3>

    <input v-model="name" placeholder="Cake Name" />
    <input v-model="description" placeholder="Description" />
    <input v-model="price" type="number" placeholder="Price" />
    <input type="file" @change="handleFileChange" />
    

    <button @click="submitProduct">Add Cake</button>

    <hr />

    <h3>All Products</h3>

    <div v-for="p in products" :key="p.id">
      <p>{{ p.name }} - ${{ p.price }}</p>

      <img v-if="p.imageUrl" :src="p.imageUrl" width="150" />

      <br />
      <button @click="removeProduct(p.id)">
        Delete
      </button>

      <hr />
    </div>
  </div>
</template>