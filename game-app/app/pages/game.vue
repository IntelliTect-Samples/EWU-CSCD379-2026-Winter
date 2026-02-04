<template>
  <div class="game-container">
    <div class="game-header">
      <span class="target-display">Next Number: </span>
      <span class="target-number">{{ targetNumber }}</span>
      <button class="quit-btn" @click="showQuitConfirm = true">Quit Game</button>
    </div>

    <div class="schulte-grid" :class="[gridClass, { reshuffle: reshuffling }]">
      <button 
        v-for="tile in grid" 
        :key="tile.id" 
        class="number-tile"
        :class="tile.state"
        @click="handleTileClick(tile)"
      >
        {{ tile.number }}
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
const reshuffling = ref(false);

const difficulty = route.query.diff || 'Medium';
const playerName = route.query.name || 'Player';
const gridClass = computed(() => `grid-${difficulty.toLowerCase()}`);

const correctSound = new Audio('/sounds/correct.mp3');
const wrongSound = new Audio('/sounds/wrong.mp3');

const playCorrect = () => {
  correctSound.currentTime = 0;
  correctSound.play();
};

const playWrong = () => {
  wrongSound.currentTime = 0;
  wrongSound.play();
};

const settings = {
  Easy: { size: 9, shuffle: 6000 },
  Medium: { size: 16, shuffle: 5000 },
  Hard: { size: 25, shuffle: 4000 }
};

const shuffleArray = (arr) => {
  return [...arr].sort(() => Math.random() - 0.5);
};

const setupGame = () => {
  const config = settings[difficulty];
  if (!config) return;

  grid.value = shuffleArray(
    Array.from({ length: config.size }, (_, i) => ({
      id: i + 1,
      number: i + 1,
      state: ''
    }))
  );

  startTime.value = Date.now();

  shuffleInterval.value = setInterval(() => {
    if (targetNumber.value <= config.size) {
      reshuffling.value = true;
      grid.value = shuffleArray(grid.value);

      setTimeout(() => {
        reshuffling.value = false;
      }, 300); // fast visual only
    }
  }, config.shuffle);
};

const confirmQuit = () => {
  if (shuffleInterval.value) {
    clearInterval(shuffleInterval.value);
    shuffleInterval.value = null;
  }

  showQuitConfirm.value = false;
  navigateTo({ path: '/' });
};

const handleTileClick = (tile) => {
  const currentTarget = targetNumber.value;
  const max = settings[difficulty].size;

  if (tile.number === currentTarget) {
    playCorrect();
    tile.state = 'correct';

    setTimeout(() => {
      tile.state = '';
    }, 200);

    if (currentTarget === max) {
      const finalTime = (Date.now() - startTime.value) / 1000;

      setTimeout(() => {
        navigateTo({
          path: '/results',
          query: {
            name: route.query.name,
            diff: route.query.diff,
            score: finalTime.toFixed(2)
          }
        });
      }, 200);
    } else {
      targetNumber.value++;
    }
  } else {
    playWrong();
    tile.state = 'wrong';

    setTimeout(() => {
      tile.state = '';
    }, 200);
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