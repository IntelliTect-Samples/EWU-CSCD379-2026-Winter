<template>
  <nav class="navbar">
    <div class="nav-left">
      <NuxtLink to="/">Home</NuxtLink>
      <NuxtLink to="/products">Products</NuxtLink>
    </div>

    <div class="nav-center">
      <NuxtLink to="/">
        <img src="/logo.png" alt="HomeSweet Logo" class="logo-img" />
      </NuxtLink>
    </div>

    <div class="nav-right">
      <NuxtLink to="/cart">Cart</NuxtLink>
      <NuxtLink to="/admin/login">Admin</NuxtLink>
    </div>
  </nav>
</template>

<style scoped>
.navbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 14px 28px;
  background: linear-gradient(90deg, #fff0f3, #fffaf0);
  position: relative;
  font-family: 'Helvetica Neue', Arial, sans-serif;
  box-shadow: 0 2px 6px rgba(0,0,0,0.04);
}

.nav-left,
.nav-right {
  display: flex;
  gap: 18px;
  align-items: center;
}

.nav-center {
  position: absolute;
  left: 50%;
  transform: translateX(-50%);
}

.logo-img {
  height: 64px;
  width: auto;
  display: block;
}

a {
  text-decoration: none;
  color: #4b3b57;
  font-weight: 600;
}

a:hover {
  color: #a33b6a;
}
</style>
