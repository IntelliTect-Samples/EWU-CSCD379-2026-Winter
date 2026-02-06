<template>
  <div class="lobby-wrapper">
    <div class="topbar">
      <div class="brand">GridSnap</div>
      <div class="menu-actions">
        <button class="menu-btn" @click="toggleInstructions">How to play</button>
        <button class="menu-btn" @click="showLeaderboard = true">Leaderboard</button>
      
        <div v-if="showLeaderboard" class="instructions-modal" @click.self="showLeaderboard = false">
          <div class="instructions-card leaderboard-card">
            <h3>Leaderboard</h3>
            <ul class="leaderboard-list">
              <li
                v-for="(entry, i) in leaderboard"
                :key="i"
                class="leader-row"
              >
                <span :class="['fake-chip', entry.diff.toLowerCase()]">
                  {{ entry.diff }}
                </span>

                <span class="leader-text">
                  <strong>{{ entry.name }}</strong>
                  <span class="leader-score">— {{ entry.score }}s</span>
                </span>
              </li>

              <li v-if="leaderboard.length === 0" class="muted">
                No scores yet
              </li>
            </ul>

            <button class="close-btn" @click="showLeaderboard = false">
              Close
            </button>
          </div>
        </div>

        <button class="theme-toggle" @click="toggleTheme">Theme: {{ themeLabel }}</button>
      </div>
    </div>

    <div class="glass-card">
      <header>
        <h1 class="title">GridSnap </h1>
      </header>

      <div class="setup-section">
        <div class="solo-section">
          <button @click="showSoloSetup = !showSoloSetup" class="solo-btn">
            ⚡ Solo Mode
          </button>
        </div>
        <div v-if="showSoloSetup" class="solo-setup">
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

        <div class="duel-section">
          <button @click="startDuel" class="duel-btn">
            ⚔️ Duel Mode
            <span class="duel-desc">Local 2-Player · 10×10 Grid</span>
          </button>
        </div>
      </div>
    </div>

    <div v-if="showInstructions" class="instructions-modal" @click.self="showInstructions = false">
      <div class="instructions-card">
        <h3>How to Play</h3>
        <p>
          Click the tiles in ascending order as quickly as possible. 
          The grid may reshuffle during the game, so stay alert and adapt fast.
        </p>
        <p>
          <strong>Solo Mode:</strong> Race against the clock to complete the grid.
        </p>
        <p>
          <strong>Easy:</strong> Numbers displayed normally in a 3×3 grid. 
          Focus on speed and accuracy.
        </p>
        <p>
          <strong>Medium:</strong> Numbers displayed as <em>Roman numerals</em> in a 4×4 grid. 
          Convert symbols to numeric order while the grid reshuffles.
        </p>
        <p>
          <strong>Hard:</strong> Numbers displayed in <em>binary</em> in a 5×5 grid. 
          Test your number-system knowledge, pattern recognition, and reaction time.
        </p>
        <hr />
        <p>
          <strong>Duel Mode:</strong> Two players compete head-to-head using the same grid.
        </p>
        <p>
          Players take turns clicking the next correct number in the sequence.
          Each player has <strong>15 seconds per turn</strong>.
        </p>
        <p>
          Clicking the wrong tile or running out of time immediately ends the match.
          The remaining player wins.
        </p>
        <button class="close-btn" @click="showInstructions = false">Close</button>
      </div>
    </div>
  </div>
</template>

<script setup>
const playerName = ref('');

// UI state
const theme = useState('theme', () => 'dark');
const showInstructions = ref(false);
const showLeaderboard = ref(false);
const leaderboard = ref([]);
const showSoloSetup = ref(false);

const themeLabel = computed(() => theme.value === 'dark' ? 'Dark' : 'Light');

// Fetch leaderboard from backend
const fetchLeaderboard = async () => {
  try {
    const data = await $fetch('https://grid-snap-api-a7c2b6b9dygdc3gt.eastus2-01.azurewebsites.net/api/score');
    leaderboard.value = data.map(entry => ({
      name: entry.playerName,
      score: entry.time,
      diff: entry.difficulty
    })).sort((a, b) => {
      const diffOrder = { 'Hard': 1, 'Medium': 2, 'Easy': 3 };
      if (diffOrder[a.diff] !== diffOrder[b.diff]) {
        return diffOrder[a.diff] - diffOrder[b.diff];
      }
      return a.score - b.score;
    }).slice(0, 10);
      
  } catch (error) {
    console.error('Failed to fetch leaderboard:', error);
  }
};

// UI Theme toggle
const toggleTheme = () => {
  theme.value = theme.value === 'dark' ? 'light' : 'dark';
  localStorage.setItem('theme', theme.value);
  document.documentElement.setAttribute('data-theme', theme.value);
};

const toggleInstructions = () => {
  showInstructions.value = !showInstructions.value;
};

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

const isLoaded = ref(false);

const startDuel = () => {
  navigateTo({
    path: '/game',
    query: {
      mode: 'duel'
    }
  });
};

onMounted(async() => {
  if (isLoaded.value) return;
  document.documentElement.setAttribute('data-theme', theme.value);
  await fetchLeaderboard();
  isLoaded.value = true;
});
</script>