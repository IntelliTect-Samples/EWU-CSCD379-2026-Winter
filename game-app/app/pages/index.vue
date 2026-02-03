<template>
  <div class="lobby-wrapper">
    <div class="glass-card">
      <header>
        <h1 class="title">Word Table <span class="accent"></span></h1>
      </header>

      <div class="setup-section">
        <input 
          v-model="playerName" 
          placeholder="Enter Your Name" 
          class="modern-input"
        />

        <div class="difficulty-grid">
          <button @click="startGame('Easy')" class="diff-btn easy">
            <span class="label">Easy</span>
            <span class="desc">3x3 Grid</span>
          </button>
          <button @click="startGame('Medium')" class="diff-btn medium">
            <span class="label">Medium</span>
            <span class="desc">4x4 Grid</span>
          </button>
          <button @click="startGame('Hard')" class="diff-btn hard">
            <span class="label">Hard</span>
            <span class="desc">5x5 Grid</span>
          </button>
        </div>
      </div>

      <div class="leaderboard-card">
        <h3>TOP SCORES</h3>
        <p class="loading-text">(None here yet)</p>
      </div>
    </div>
  </div>
</template>

<script setup>
const playerName = ref('');

const startGame = (difficulty) => {
  if (!playerName.value.trim()) {
    alert("Please enter a name to begin.");
    return;
  }

  navigateTo({
    path: '/game',
    query: { name: playerName.value, diff: difficulty }
  });
};
</script>