<script setup>
</script>

<template>
  <div class="home">
    <section class="hero">
      <div class="hero-content">
        <img src="/logo.png" alt="HomeSweet Logo" class="hero-logo" />
        <h1>HomeSweet Bakery</h1>
        <p>Artisan cakes baked fresh daily — crafted with love for every celebration.</p>
        <NuxtLink to="/products">
          <button class="cta">Shop Now</button>
        </NuxtLink>
      </div>
      <img src="/homepage1.png" alt="Delicious cakes" class="hero-img" />
    </section>
  </div>
</template>

<style scoped>
.home { font-family: 'Helvetica Neue', Arial, sans-serif; }
.hero { display:flex; align-items:center; justify-content:space-between; gap:32px; padding:40px; background: linear-gradient(90deg,#fff7fb,#fffaf0); border-radius:12px; }
.hero-content { max-width:520px; }
.hero-logo { height:72px; margin-bottom:12px; }
.hero h1 { margin:0 0 12px 0; color:#6d3b57; font-size:34px; }
.hero p { margin:0 0 20px 0; color:#6b5869; }
.cta { background:#ff7faa; color:white; border:none; padding:12px 18px; border-radius:8px; cursor:pointer; font-weight:700; }
.hero-img { max-width:420px; width:40%; border-radius:10px; box-shadow:0 6px 18px rgba(105,82,114,0.12); }
</style>
