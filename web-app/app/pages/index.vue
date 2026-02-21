<script setup>
</script>

<template>
  <div class="page">
    <img src="/homepage1.png" alt="Bakery Homepage" style="max-width: 100%; height: auto; margin-bottom: 24px;" />
    <h1>Welcome to HomeSweet Bakery 🎂</h1>
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