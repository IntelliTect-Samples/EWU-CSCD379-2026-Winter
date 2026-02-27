<script setup>
  import { ref, onMounted } from 'vue'
  import { getProducts } from '~/services/api'

  const topCakes = ref([])

  function seededRandom(seed) {
    const x = Math.sin(seed) * 10000
    return x - Math.floor(x)
  }

  onMounted(async () => {
    const products = await getProducts()

    if (!products?.length) return

    // Use today's date as seed
    const today = new Date().toDateString()
    const seed = today.split('').reduce((a, b) => a + b.charCodeAt(0), 0)

    const shuffled = [...products].sort(() => 0.5 - seededRandom(seed))

    topCakes.value = shuffled.slice(0, 3)
  })
</script>
<template>
  <div class="home">
    <section class="hero">
      <div class="hero-content">
        <h1>HomeSweet Bakery</h1>
        <p>Artisan cakes baked fresh daily - crafted with love for every celebration.</p>

        <NuxtLink to="/products">
          <button class="cta">Explore All Cakes</button>
        </NuxtLink>
      </div>

      <img src="/homepage1.png" alt="Delicious cakes" class="hero-img" />
    </section>
    <section v-if="topCakes.length === 3" class="top3">
      <h2>🍰 Today's Top Picks</h2>

      <div class="cake-grid">
        <div class="side">
          <img :src="topCakes[1].imageUrl" />
          <p>{{ topCakes[1].name }}</p>
        </div>

        <div class="main">
          <img :src="topCakes[0].imageUrl" />
          <h3>{{ topCakes[0].name }}</h3>
          <p>${{ topCakes[0].price }}</p>
        </div>

        <div class="side">
          <img :src="topCakes[2].imageUrl" />
          <p>{{ topCakes[2].name }}</p>
        </div>
      </div>

      <NuxtLink to="/products">
        <button class="cta">Shop Now</button>
      </NuxtLink>
    </section>
  </div>
</template>

<style scoped>
.home {
  min-height: 100vh;
  background: linear-gradient(135deg, #fff1f6, #ffe4ec, #fff7fa);
  background-size: 300% 300%;
  animation: gradientMove 12s ease infinite;
}

.hero {
  max-width: 1200px;
  margin: 0 auto;
  display: grid;
  grid-template-columns: 1fr 1fr;
  align-items: center;
  padding: 100px 40px 60px;
  gap: 60px;
}

.hero-content {
  animation: fadeUp 1.2s ease forwards;
}

.hero-logo {
  width: 90px;
  margin-bottom: 20px;
  animation: float 4s ease-in-out infinite;
}

.hero h1 {
  font-size: 3rem;
  color: #6d3b57;
  margin-bottom: 16px;
}

.hero p {
  font-size: 1.1rem;
  color: #6b5a63;
  margin-bottom: 24px;
  line-height: 1.6;
}

.cta {
  background: #a33b6a;
  color: white;
  border: none;
  padding: 14px 28px;
  border-radius: 30px;
  font-size: 1rem;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 6px 18px rgba(163, 59, 106, 0.3);
}

.cta:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 28px rgba(163, 59, 106, 0.4);
  background: #8e315b;
}

.hero-img {
  width: 100%;
  max-width: 520px;
  border-radius: 30px;
  box-shadow: 0 30px 60px rgba(109, 59, 87, 0.15);
  animation: float 6s ease-in-out infinite;
  opacity: 0.95;
}

.top3 {
  padding: 70px 8%;
  text-align: center;
  max-width: 1000px;
  margin: 0 auto;
}

.top3 h2 {
  font-size: 1.8rem;
  margin-bottom: 40px;
  color: #6d3b57;
}

.cake-grid {
  display: grid;
  grid-template-columns: 1fr 1.2fr 1fr; 
  align-items: center;
  gap: 30px;
  max-width: 850px; 
  margin: 0 auto;
}

.cake-grid img {
  width: 100%;
  max-width: 240px;   
  aspect-ratio: 1 / 1;
  object-fit: cover;
  border-radius: 20px;
  margin: 0 auto;
  transition: transform 0.4s ease, box-shadow 0.4s ease;
}

.cake-grid img:hover {
  transform: scale(1.05);
  box-shadow: 0 20px 50px rgba(0, 0, 0, 0.15);
}

.main img {
  transform: scale(1.1);
}

.main h3 {
  margin-top: 20px;
  font-size: 1.4rem;
  font-weight: 600;
  color: #6d3b57;
}

.side p {
  margin-top: 12px;
  font-weight: 500;
  color: #6b5a63;
}

@keyframes gradientMove {
  0% { background-position: 0% 50%; }
  50% { background-position: 100% 50%; }
  100% { background-position: 0% 50%; }
}

@keyframes fadeUp {
  from {
    opacity: 0;
    transform: translateY(40px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes float {
  0%, 100% { transform: translateY(0px); }
  50% { transform: translateY(-10px); }
}

@media (max-width: 900px) {
  .hero {
    grid-template-columns: 1fr;
    text-align: center;
  }

  .cake-grid {
    grid-template-columns: 1fr;
  }

  .main img {
    transform: scale(1);
  }
}
</style>