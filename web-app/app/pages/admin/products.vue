<script setup>
import { ref, onMounted } from 'vue'
import { getProducts, addProduct, uploadImage } from '~/services/api'

const products = ref([])

const name = ref('')
const price = ref('')
const selectedFile = ref(null)

const loadProducts = async () => {
  products.value = await getProducts()
}

onMounted(loadProducts)

const handleFileChange = (event) => {
  selectedFile.value = event.target.files[0]
}

const submitProduct = async () => {
  let imageUrl = ""

  if (selectedFile.value) {
    const uploadResponse = await uploadImage(selectedFile.value)
    imageUrl = uploadResponse.imageUrl
  }

  await addProduct({
    name: name.value,
    price: Number(price.value),
    imageUrl: imageUrl
  })

  name.value = ''
  price.value = ''
  selectedFile.value = null

  await loadProducts()
}
</script>

<template>
  <div>
    <h1>Manage Products</h1>

    <h3>Add New Cake</h3>

    <input v-model="name" placeholder="Cake Name" />
    <input v-model="price" type="number" placeholder="Price" />
    <input type="file" @change="handleFileChange" />

    <button @click="submitProduct">Add Cake</button>

    <hr />

    <h3>All Products</h3>

    <div v-for="p in products" :key="p.id">
      <p>{{ p.name }} - ${{ p.price }}</p>
      <img v-if="p.imageUrl" :src="p.imageUrl" width="150" />
    </div>
  </div>
</template>