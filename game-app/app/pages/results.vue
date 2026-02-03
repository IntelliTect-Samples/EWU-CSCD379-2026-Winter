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
const saveScore = async () => {
  const scoreData = {
    playerName: route.query.name,
    time: parseFloat(route.query.score),
    difficulty: route.query.diff,
    dateAchieved: new Date().toISOString()
  };

  try {
    // Call the api
    const response = await $fetch('http://localhost:5143/api/score', {
      method: 'POST',
      body: scoreData
    });

    console.log('Score saved:', response);
    alert("Score recorded on the C# Backend!");
    navigateTo('/');
  } catch (error) {
    console.error('Submission failed:', error);
    alert("Could not connect to API. Is it running on port 5143?");
  }
};
</script>