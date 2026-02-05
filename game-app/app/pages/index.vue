<template>
  <div class="lobby-wrapper">
    <div class="topbar">
      <div class="brand">GridSnap</div>
      <div class="menu-actions">
        <button class="menu-btn" @click="toggleInstructions">How to play</button>
        <div class="leaderboard-wrapper">
          <button class="menu-btn" @click="showLeaderboard = !showLeaderboard">Leaderboard</button>
          <div v-if="showLeaderboard" class="dropdown-leaderboard">
            <h4>Top Scores</h4>
            <ul>
              <li v-for="(entry, i) in leaderboard" :key="i" class="leader-row">
                <span :class="['fake-chip', entry.diff.toLowerCase()]">
                  {{ entry.diff }}
                </span>
                
                <span class="leader-text">
                  <strong>{{ entry.name }}</strong> 
                  <span class="leader-score">— {{ entry.score }}s</span>
                </span>
              </li>
              <li v-if="leaderboard.length === 0" class="muted">(None here yet)</li>
            </ul>
          </div>
        </div>
        <button class="theme-toggle" @click="toggleTheme">Mode: {{ themeLabel }}</button>
      </div>
    </div>

    <div class="glass-card">
      <header>
        <h1 class="title">GridSnap </h1>
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
    </div>

    <div v-if="showInstructions" class="instructions-modal" @click.self="showInstructions = false">
      <div class="instructions-card">
        <h3>How to Play</h3>
        <p>
          <strong>Solo Mode:</strong> Click the numbers in ascending order (1 → 2 → 3 → ...). 
          Choose a difficulty to change the grid size (3x3, 4x4, 5x5). 
          The grid will reshuffle during the game, so stay fast and focused!
        </p>
        <p>
          <strong>Duel Mode:</strong> Connect with another player and take turns finding the next number in the sequence. 
          Each player has 5 seconds per turn. Click the wrong number or run out of time and you lose the match.
        </p>
        <button class="close-btn" @click="showInstructions = false">Close</button>
      </div>
    </div>
  </div>
</template>

<script setup>
const playerName = ref('');

// UI state
const theme = ref('dark');
const showInstructions = ref(false);
const showLeaderboard = ref(false);
const leaderboard = ref([]);

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

const getDiffColor = (diff) => {
  switch (diff) {
    case 'Hard': return 'red-darken-2';
    case 'Medium': return 'orange-darken-1';
    case 'Easy': return 'green-darken-1';
    default: return 'grey';
  }
};

const isLoaded = ref(false);

onMounted(async() => {
  if (isLoaded.value) return;
  document.documentElement.setAttribute('data-theme', theme.value);
  await fetchLeaderboard();
  isLoaded.value = true;
});
</script>