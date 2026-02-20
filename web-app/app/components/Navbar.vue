<template>
  <nav class="navbar">
    <NuxtLink to="/">Home</NuxtLink>
    <NuxtLink to="/products">Products</NuxtLink>
    <NuxtLink to="/admin/login">Admin</NuxtLink>
  </nav>
</template>

<style scoped>
.navbar {
  display: flex;
  gap: 20px;
  padding: 15px;
  background: #f8d7da;
}
</style>