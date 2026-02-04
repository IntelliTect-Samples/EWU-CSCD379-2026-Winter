<template>
  <div class="game-container">
    <div class="game-header">
      <span class="target-display">Next Number: </span>
      <span class="target-number">{{ targetNumber }}</span>
      <button class="quit-btn" @click="showQuitConfirm = true">Quit Game</button>
    </div>

    <div class="schulte-grid" :class="gridClass">
      <button 
        v-for="num in grid" 
        :key="num" 
        class="number-tile"
        @click="handleTileClick(num)"
      >
        {{ num }}
      </button>
    </div>

    <div v-if="showQuitConfirm" class="confirm-overlay" @click.self="showQuitConfirm = false">
      <div class="confirm-box">
        <p>You want to stop the game...</p>
        <div class="confirm-actions">
          <button class="confirm-yes" @click="confirmQuit">Yes</button>
          <button class="confirm-no" @click="showQuitConfirm = false">No</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
const route = useRoute();
const grid = ref([]);
const targetNumber = ref(1);
const startTime = ref(null);
const showQuitConfirm = ref(false);
const shuffleInterval = ref(null);

const difficulty = route.query.diff || 'Medium';
const playerName = route.query.name || 'Player';
const gridClass = computed(() => `grid-${difficulty.toLowerCase()}`);

// Config based on selection
const settings = {
  Easy: { size: 9, shuffle: 6000 },
  Medium: { size: 16, shuffle: 5000 },
  Hard: { size: 25, shuffle: 4000 }
};

const setupGame = () => {
  const config = settings[difficulty];

  if (!config) {
    return;
  }

  grid.value = Array.from({ length: config.size }, (_, i) => i + 1).sort(() => Math.random() - 0.5);
  startTime.value = Date.now();

  // Reshuffle logic (store the interval so we can clear it later)
  shuffleInterval.value = setInterval(() => {

    if (targetNumber.value <= config.size) {
      grid.value = [...grid.value].sort(() => Math.random() - 0.5);
    }

  }, config.shuffle);
};

const confirmQuit = () => {
  // Stop automatic reshuffles and return to lobby
  if (shuffleInterval.value) {
    clearInterval(shuffleInterval.value);
    shuffleInterval.value = null;
  }

  showQuitConfirm.value = false;
  navigateTo({ path: '/' });
};

const handleTileClick = (num) => {
  const currentTarget = targetNumber.value;
  const max = settings[difficulty].size;

  console.log(`Clicked: ${num} | Target: ${currentTarget}`);

  if (Number(num) === currentTarget) {
    if (currentTarget === max) {
      
      const finalTime = (Date.now() - startTime.value) / 1000;
      
      navigateTo({
        path: '/results',
        query: { 
          name: route.query.name, 
          diff: route.query.diff, 
          score: finalTime.toFixed(2) 
        }
      });
    } else {
      targetNumber.value++;
    }
  }
};

onMounted(setupGame);
onUnmounted(() => {
  if (shuffleInterval.value) {
    clearInterval(shuffleInterval.value);
    shuffleInterval.value = null;
  }
});
</script>

<style scoped>
.quit-btn {
  margin-right: 12px;
  background: #ff5252;
  color: #fff;
  border: none;
  padding: 6px 20px;
  border-radius: 6px;
  cursor: pointer;
}
.confirm-overlay {
  position: fixed;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(70, 65, 65, 0.45);
}
.confirm-box {
  background: #65696d;
  padding: 18px;
  border-radius: 8px;
  min-width: 260px;
  text-align: center;
}
.confirm-actions { display:flex; justify-content:space-around; margin-top:12px; }
.confirm-yes { background:#4caf50; color:#fff; border:none; padding:6px 12px; border-radius:6px; cursor:pointer; }
.confirm-no { background:#e0e0e0; border:none; padding:6px 12px; border-radius:6px; cursor:pointer; }
</style>