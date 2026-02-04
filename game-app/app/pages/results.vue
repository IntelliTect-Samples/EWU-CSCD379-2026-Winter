<template>
  <div class="results-wrapper">
    <div class="victory-card">
      <h2 class="victory-title">Grid Cleared!</h2>
      <div class="final-time"> You completed the grid in: {{ animatedScore }}s</div>
      
      <div class="stats-grid">
        <div class="stat-item">
          <span class="label">Player: </span>
          <span class="value">{{ route.query.name }}</span>
        </div>
        <div class="stat-item">
          <span class="label">Rank: </span>
          <span class="value">{{ route.query.diff }}</span>
        </div>
      </div>

      <div class="action-group">
        <button @click="saveScore" class="btn-primary">Submit to Leaderboard</button>
        <button @click="goToMenu" class="btn-secondary">Main Menu</button>
      </div>
    </div>
  </div>
</template>

<script setup>
const route = useRoute()

const score = computed(() => route.query.score)
const name = computed(() => route.query.name)
const difficulty = computed(() => route.query.diff)

const animatedScore = ref(0)

const saveScore = async () => {
  const scoreData = {
    PlayerName: String(name.value),
    Time: parseFloat(score.value),
    Difficulty: String(difficulty.value),
    DateAchieved: new Date().toISOString()
  }

  await $fetch('http://localhost:5143/api/score', {
    method: 'POST',
    body: scoreData
  })

  alert("Score Saved!")
  navigateTo('/')
}

onMounted(() => {
  const ripple = document.createElement('div')
  ripple.className = 'victory-ripple'
  document.body.appendChild(ripple)

  setTimeout(() => {
    ripple.remove()
  }, 1200)

  let current = 0
  const target = parseFloat(score.value)

  const interval = setInterval(() => {
    current += target / 30

    if (current >= target) {
      animatedScore.value = target.toFixed(2)
      clearInterval(interval)
    } else {
      animatedScore.value = current.toFixed(2)
    }
  }, 30)
})

const theme = computed(() => route.query.theme || 'dark')

const goToMenu = () => {
  navigateTo({
    path: '/',
    query: {
      theme: theme.value
    }
  })
}

</script>