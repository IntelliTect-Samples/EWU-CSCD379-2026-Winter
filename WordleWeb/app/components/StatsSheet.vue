<template>
  <div class="stats-button">
    <button @click="showStats = !showStats" class="stats-btn">📊 Stats</button>
  </div>

  <div v-if="showStats" class="stats-sheet">
    <h2>Your Stats</h2>
    <div class="stats-grid">
      <div class="stat">
        <div class="stat-value">{{ stats.totalGames }}</div>
        <div class="stat-label">Games Played</div>
      </div>
      <div class="stat">
        <div class="stat-value">{{ stats.wins }}</div>
        <div class="stat-label">Wins</div>
      </div>
      <div class="stat">
        <div class="stat-value">{{ stats.losses }}</div>
        <div class="stat-label">Losses</div>
      </div>
      <div class="stat">
        <div class="stat-value">{{ winRatio }}</div>
        <div class="stat-label">Win %</div>
      </div>
      <div class="stat">
        <div class="stat-value">{{ averageGuesses }}</div>
        <div class="stat-label">Avg Guesses</div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Stats } from "~/scripts/useStats";

defineProps<{
  stats: Stats;
  winRatio: string;
  averageGuesses: string;
}>();

const showStats = ref(false);
</script>

<style scoped>
.stats-button {
  margin-bottom: 20px;
}

.stats-btn {
  padding: 8px 16px;
  background-color: #e3e3e3;
  color: #333;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  font-weight: bold;
}

.stats-btn:hover {
  background-color: #d3d3d3;
}

.stats-sheet {
  background-color: #f5f5f5;
  border: 2px solid #ddd;
  border-radius: 8px;
  padding: 20px;
  margin-bottom: 20px;
}

.stats-sheet h2 {
  color: #333;
  margin-top: 0;
  margin-bottom: 15px;
  font-size: 18px;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  gap: 10px;
}

@media (max-width: 700px) {
  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 400px) {
  .stats-grid {
    grid-template-columns: 1fr;
  }
}

.stat {
  background-color: white;
  padding: 10px;
  border-radius: 6px;
  border: 1px solid #ddd;
}

.stat-value {
  font-size: 24px;
  font-weight: bold;
  color: #6aaa64;
  margin-bottom: 5px;
}

.stat-label {
  font-size: 12px;
  color: #666;
  text-transform: uppercase;
}
</style>
