<template>
  <div class="results-wrapper">
    <div class="victory-card">
      <h2 class="victory-title">
        {{ isLose ? 'Game Over' : 'Grid Cleared!' }}
      </h2>
      <div v-if="!isLose" class="final-time">
        You completed the grid in: {{ animatedScore }}s
      </div>
      <div v-else class="final-time">
        You haven’t completed the grid in time.
      </div>
      <div v-if="showStats" class="stats-grid">
        <div class="stat-item">
          <span class="label">Player:</span>
          <span class="value">{{ route.query.name }}</span>
        </div>
        <div class="stat-item">
          <span class="label">Rank:</span>
          <span class="value">{{ route.query.diff }}</span>
        </div>
      </div>
      <div class="action-group">
        <button
          v-if="canSubmit"
          @click="saveScore"
          class="btn-primary"
        >
          Submit to Leaderboard
        </button>

        <button
          v-if="isLose"
          @click="tryAgain"
          class="btn-primary"
        >
          Try Again
        </button>

        <button @click="goToMenu" class="btn-secondary">
          Main Menu
        </button>
      </div>

    </div>
  </div>
</template>

<script setup>
import { onMounted, ref, computed } from 'vue';
const config = useRuntimeConfig();
const route = useRoute()

const score = computed(() => route.query.score)
const difficulty = computed(() => route.query.diff)
const isLose = computed(() => route.query.result === 'lose')
const isDuel = computed(() => difficulty.value === 'Duel')

const showStats = computed(() => !isDuel.value && !isLose.value)
const canSubmit = computed(() => !isLose.value)

const animatedScore = ref(0)

const saveScore = async () => {
  const scoreData = {
    PlayerName: route.query.name || 'Duel',
    Time: parseFloat(score.value),
    Difficulty: difficulty.value,
    DateAchieved: new Date().toISOString()
  }

  await $fetch(
    config.public.api,
    {
      method: 'POST',
      body: scoreData
    }
  )

  alert('Score Saved!')
  navigateTo('/')
}

onMounted(() => {
  if (isLose.value) return

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

const tryAgain = () => {
  navigateTo({
    path: '/game',
    query: {
      mode: 'duel'
    }
  })
}

const goToMenu = () => {
  navigateTo('/')
}
</script>
