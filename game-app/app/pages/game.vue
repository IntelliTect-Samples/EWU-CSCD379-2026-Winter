<template>
  <div class="game-container">
    <div class="game-header">
      <span v-if="!isDuel">
        Next Number: <strong>{{ targetNumber }}</strong>
      </span>

      <span v-else>
        Player {{ currentPlayer }} • Next: {{ targetNumber }} • Time: {{ turnTime }}s
      </span>

      <button class="quit-btn" @click="showQuitConfirm = true">Quit Game</button>
    </div>

    <div class="schulte-grid" :class="[gridClass, { reshuffle: reshuffling }]">
      <button 
        v-for="tile in grid" 
        :key="tile.id" 
        class="number-tile"
        :class="tile.state"
        @click="isDuel ? handleDuelClick(tile) : handleTileClick(tile)"
      >
        {{ tile.display }}
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

const difficulty = computed(() =>
  isDuel.value ? null : route.query.diff || 'Medium'
);

const playerName = route.query.name || 'Player';
const gridClass = computed(() =>
  isDuel.value ? 'grid-duel' : `grid-${difficulty.value.toLowerCase()}`
);


const correctSound = new Audio('/sounds/correct.mp3');
const wrongSound = new Audio('/sounds/wrong.mp3');

const currentPlayer = ref(1) // 1 or 2
const turnTime = ref(5)
const turnTimer = ref(null)
const gameOver = ref(false)

const mode = route.query.mode || 'solo';
const isDuel = computed(() => mode === 'duel');



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
  Hard: { size: 25, shuffle: 10000 }
};

const shuffleArray = (arr) => {
  return [...arr].sort(() => Math.random() - 0.5);
};

const toRoman = (num) => {
  const romanMap = {
    1: 'I',
    2: 'II',
    3: 'III',
    4: 'IV',
    5: 'V',
    6: 'VI',
    7: 'VII',
    8: 'VIII',
    9: 'IX',
    10: 'X',
    11: 'XI',
    12: 'XII',
    13: 'XIII',
    14: 'XIV',
    15: 'XV',
    16: 'XVI'
  }
  return romanMap[num]
}


const toBinary = (num) => num.toString(2).padStart(6, '0');

const setupGame = () => {
  if (isDuel.value) return;
  const config = settings[difficulty.value];
  if (!config) return;

  grid.value = shuffleArray(
  Array.from({ length: config.size }, (_, i) => {
    const value = i + 1
    let display = value

    if (difficulty.value === 'Medium') display = toRoman(value)
    if (difficulty.value === 'Hard') display = toBinary(value)

    return {
      id: value,
      number: value,
      display,
      state: ''
    }
  })
);

  startTime.value = Date.now();
  targetNumber.value = 1;
  if (!isDuel.value) {
    shuffleInterval.value = setInterval(() => {
      if (targetNumber.value <= config.size) {
        reshuffling.value = true;
        grid.value = shuffleArray(grid.value);

        setTimeout(() => {
          reshuffling.value = false;
        }, 300);
      }
    }, config.shuffle);
  }
};

const confirmQuit = () => {
  if (shuffleInterval.value) {
    clearInterval(shuffleInterval.value);
    shuffleInterval.value = null;
  }
  if (turnTimer.value) {
    clearInterval(turnTimer.value);
    turnTimer.value = null;
  }
  showQuitConfirm.value = false;
  navigateTo({ path: '/' });
};

const handleTileClick = (tile) => {
  const currentTarget = targetNumber.value;
  const max = settings[difficulty.value].size;

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

const setupDuelGame = () => {
  grid.value = shuffleArray(
    Array.from({ length: 100 }, (_, i) => ({
      id: i + 1,
      number: i + 1,
      display: i + 1,
      state: ''
    }))
  )

  targetNumber.value = 1
  currentPlayer.value = 1
  gameOver.value = false;
  startTurnTimer()
}
const startTurnTimer = () => {
  turnTime.value = 10

  clearInterval(turnTimer.value)
  turnTimer.value = setInterval(() => {
    turnTime.value--

    if (turnTime.value <= 0) {
      endGame(`Player ${currentPlayer.value} ran out of time`)
    }
  }, 1000)
}

const handleDuelClick = (tile) => {
  if (gameOver.value) return

  if (tile.number === targetNumber.value) {
    tile.state = 'correct'
    targetNumber.value++

    switchPlayer()
  } else {
    tile.state = 'wrong'
    endGame(`Player ${currentPlayer.value} clicked wrong`)
  }
}
const switchPlayer = () => {
  currentPlayer.value = currentPlayer.value === 1 ? 2 : 1
  startTurnTimer()
}

const endGame = (reason) => {
  gameOver.value = true
  clearInterval(turnTimer.value)

  const loser = currentPlayer.value
  const winner = loser === 1 ? 2 : 1

  setTimeout(() => {
    navigateTo({
      path: '/results',
      query: {
        name: `Player ${loser}`,
        diff: 'Duel',
        score: 0,
        result: 'lose',
        reason
      }
    })
  }, 500)
}


onMounted(() => {
  if (isDuel.value) {
    setupDuelGame();
  } else {
    setupGame();
  }
});

onUnmounted(() => {
  clearInterval(shuffleInterval.value)
  clearInterval(turnTimer.value)
})


</script>