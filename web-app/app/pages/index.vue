<script setup>
</script>

<template>
  <div class="page">
    <h1>Welcome to Sweet Crumbs Bakery 🎂</h1>
    <p>Fresh baked cakes made with love.</p>

    <NuxtLink to="/products">
      <button>Place Order</button>
    </NuxtLink>
  </div>
</template>

<style scoped>
.page {
  padding: 40px;
  text-align: center;
}
button {
  margin-top: 20px;
  padding: 10px 20px;
}
</style>