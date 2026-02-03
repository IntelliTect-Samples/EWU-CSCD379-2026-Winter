<template>
  <div class="game-container">
    <div class="game-header">
      <span class="target-display">Next Number: </span>
      <span class="target-number">{{ targetNumber }}</span>
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
  </div>
</template>

<script setup>
const route = useRoute();
const grid = ref([]);
const targetNumber = ref(1);
const startTime = ref(null);

const difficulty = route.query.diff || 'Medium';
const playerName = route.query.name || 'Player';
const gridClass = computed(() => `grid-${difficulty.toLowerCase()}`);

// Config based on selection
const settings = {
  Easy: { size: 9, shuffle: 12000 },
  Medium: { size: 16, shuffle: 8000 },
  Hard: { size: 25, shuffle: 4000 }
};

const setupGame = () => {
  const config = settings[difficulty];

  if (!config) {
    return;
  }

  grid.value = Array.from({ length: config.size }, (_, i) => i + 1).sort(() => Math.random() - 0.5);
  startTime.value = Date.now();

  // Reshuffle logic
  setInterval(() => {

    if (targetNumber.value <= config.size) {
      grid.value = [...grid.value].sort(() => Math.random() - 0.5);
    }

  }, config.shuffle);
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
</script>