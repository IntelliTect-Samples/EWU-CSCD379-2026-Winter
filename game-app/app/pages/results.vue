<template>
  <div class="results-wrapper">
    <div class="victory-card">
      <h2 class="victory-title">Grid Cleared!</h2>
      <div class="final-time"> You completed the grid in: {{ route.query.score }}s</div>
      
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
        <button @click="navigateTo('/')" class="btn-secondary">Main Menu</button>
      </div>
    </div>
  </div>
</template>

<script setup>
const route = useRoute();
const score = computed(() => route.query.score);
const name = computed(() => route.query.name);
const difficulty = computed(() => route.query.diff);

const saveScore = async () => {
  const scoreData = {
    PlayerName: String(name.value),
    Time: parseFloat(score.value),
    Difficulty: String(difficulty.value),
    DateAchieved: new Date().toISOString()
  };

  await $fetch('http://localhost:5143/api/score', {
    method: 'POST',
    body: scoreData
  });
  
  alert("Score Saved!");
  navigateTo('/'); 
};
</script>