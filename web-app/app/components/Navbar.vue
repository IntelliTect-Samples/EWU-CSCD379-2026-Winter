<template>
  <nav class="navbar">

    <!-- LEFT -->
    <div class="nav-left">
      <NuxtLink to="/">Home</NuxtLink>
      <NuxtLink to="/products">Products</NuxtLink>
    </div>

    <!-- CENTER LOGO -->
    <div class="nav-center">
        <img src="/logo.png" alt="HomeSweet Logo" class="logo-img" />
    </div>

    <!-- RIGHT -->
    <div class="nav-right">
      <NuxtLink to="/cart">Go To Cart</NuxtLink>
      <NuxtLink to="/admin/login">Admin Login</NuxtLink>
    </div>

  </nav>
</template>

<style scoped>
.navbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 18px 40px;
  background: #f8d7da;
  position: relative;
}

/* Left & Right */
.nav-left,
.nav-right {
  display: flex;
  gap: 24px;
}

/* Center logo */
.nav-center {
  position: absolute;
  left: 50%;
  transform: translateX(-50%);
}

.logo-img {
  height: 90px;   /* adjust if needed */
  width: auto;
}

/* Links */
a {
  text-decoration: none;
  color: #333;
  font-weight: 500;
}

a:hover {
  color: #c2185b;
}
</style>s