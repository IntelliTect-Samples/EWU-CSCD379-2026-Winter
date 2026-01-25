<script setup>
import { ref, onMounted, onUnmounted, computed } from 'vue'
import { useTheme } from 'vuetify'
import { getRandomWord, WORD_LIST, VALID_GUESSES, generateHint } from './utils/wordlist'

// <><><><> Toggle Theme <><><><>
const theme = useTheme()
const themeIcon = computed(() => theme.global.name.value === 'dark' ? 'mdi-weather-sunny' : 'mdi-weather-night')

function toggleTheme() {
  const isDark = theme.global.name.value === 'dark'
  const newTheme = isDark ? 'light' : 'dark'
  theme.global.name.value = newTheme
  localStorage.setItem('user-theme', newTheme)
}

// <><><><> Game State <><><><>
const currentWordData = ref({ word: '', hint: '' })
const solution = computed(() => (currentWordData.value?.word || '').toUpperCase())
const currentHint = computed(() => currentWordData.value?.hint || '')

const board = ref(Array.from({ length: 6 }, () => Array.from({ length: 5 }, () => ({ letter: '', status: 'default' }))))
const currentRow = ref(0)
const currentCol = ref(0)
const gameOver = ref(false)
const letterStates = ref({})

const stats = ref({
  wins: 0,
  losses: 0,
  totalAttempts: 0,
  gamesPlayed: 0,
  averageAttempts: 0
})

const loadingHint = ref(false)

// <><><><> UI State <><><><>
const snackbar = ref(false)
const snackbarMsg = ref('')
const hintSnackbar = ref(false)
const showStats = ref(false)
const isDaily = ref(false)

const rows = [
  ['Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P'],
  ['A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L'],
  ['ENTER', 'Z', 'X', 'C', 'V', 'B', 'N', 'M', 'BACKSPACE']
]

// <><><><> Game Logic <><><><>
const resetGame = () => {
  isDaily.value = false
  if (gameOver.value) {
    currentWordData.value = getRandomWord()
  }
  
  board.value = Array.from({ length: 6 }, () => Array.from({ length: 5 }, () => ({ letter: '', status: 'default' })))
  letterStates.value = {}
  currentRow.value = 0
  currentCol.value = 0
  gameOver.value = false
  showStats.value = false
  snackbar.value = false
  hintSnackbar.value = false
}

// <><><><> Stats Tracker <><><><>
const updateStats = (isWin) => {
  stats.value.gamesPlayed++
  if (isWin) {
    stats.value.wins++
    stats.value.totalAttempts += (currentRow.value + 1)
  } else {
    stats.value.losses++
  }
  
  // Calculate Average
  if (stats.value.wins > 0) {
    stats.value.averageAttempts = (stats.value.totalAttempts / stats.value.wins).toFixed(1)
  }
  localStorage.setItem('wordle-stats', JSON.stringify(stats.value))

  const today = new Date().toDateString()
  localStorage.setItem('daily-finished-' + today, 'true')
}

// <><><><> Word Checking Logic <><><><>
const checkWord = () => {
  const currentGuess = board.value[currentRow.value]
  const guessString = currentGuess.map(cell => cell.letter).join('')
  
  // Solution Letter Counts
  const solutionArray = solution.value.split('')
  const solutionCounts = {}
  solutionArray.forEach(l => solutionCounts[l] = (solutionCounts[l] || 0) + 1)

  // Pass 1: Mark Correct (Green) on Board
  let tempCounts = { ...solutionCounts }

  currentGuess.forEach((cell, i) => {
    if (cell.letter === solution.value[i]) {
      cell.status = 'correct'
      tempCounts[cell.letter]--
    }
  })

  // Pass 2: Mark Present (Yellow) or Absent (Grey) on Board
  currentGuess.forEach((cell, i) => {
    if (cell.status === 'correct') return
    if (solution.value.includes(cell.letter) && tempCounts[cell.letter] > 0) {
      cell.status = 'present'
      tempCounts[cell.letter]--
    } else {
      cell.status = 'absent'
    }
  })

  // Keyboard Update with Exact Counts
  currentGuess.forEach((cell) => {
    const letter = cell.letter
    const oldStatus = letterStates.value[letter]
    
    if (cell.status === 'correct') {
      letterStates.value[letter] = 'correct'
    } else if (cell.status === 'present' && oldStatus !== 'correct') {
      letterStates.value[letter] = 'present'
    } else if (!oldStatus) {
      letterStates.value[letter] = 'absent'
    }
  })

  const isWin = guessString === solution.value;
  const isLoss = !isWin && currentRow.value === 5;

  if (isWin || isLoss) {
    gameOver.value = true;
    showStats.value = true;
    updateStats(isWin);

    if (!currentWordData.value.hint) {
      generateHint(solution.value).then(hint => {
        currentWordData.value.hint = hint;
      });
    }

    snackbarMsg.value = isWin ? '🎉 You guessed the word! 🎉' : `💔 Better luck next time! 💔`;
    snackbar.value = true;
  } else {
    currentRow.value++;
    currentCol.value = 0;
  }

  localStorage.setItem('daily-board-state', JSON.stringify({
    board: board.value,
    currentRow: currentRow.value,
    letterStates: letterStates.value,
    solution: solution.value,
    gameOver: gameOver.value,
    snackbarMsg: snackbarMsg.value
  }))
}

const handleInput = (e) => {
  if (gameOver.value) {
     return
  }

  if (e instanceof KeyboardEvent && e.key === 'Enter') {
    e.preventDefault()
  }

  const key = (typeof e === 'string' ? e : e.key).toUpperCase()
  
  if (key === 'ENTER') {
    if (currentCol.value === 5) {
      const guessString = board.value[currentRow.value].map(cell => cell.letter).join('').toLowerCase()
      const isAnswer = WORD_LIST.map(w => (typeof w === 'string' ? w : w.word).toLowerCase()).includes(guessString)
      const isDictionaryWord = VALID_GUESSES.value.includes(guessString);

      if (isAnswer || isDictionaryWord) {
        checkWord()
      } else {
        showTemporaryMessage('Not in word list')
        shakeRow()
      }
    }
  } else if (key === 'BACKSPACE' || key === 'DELETE') {
    if (currentCol.value > 0) {
      currentCol.value--
      board.value[currentRow.value][currentCol.value].letter = ''
    }
  } else if (/^[A-Z]$/.test(key) && currentCol.value < 5) {
    board.value[currentRow.value][currentCol.value].letter = key
    currentCol.value++
  }
}

// <><><><> Shows Hint <><><><>

const revealHint = async () => {
  if (!currentWordData.value || typeof currentWordData.value !== 'object') {
    currentWordData.value = { word: currentWordData.value || '', hint: '' }
  }

  if (currentWordData.value.hint) {
    hintSnackbar.value = true
    return
  }

  loadingHint.value = true
  try {
    const fetchedHint = await generateHint(solution.value)
    currentWordData.value.hint = fetchedHint || "Definition not found in dictionary."
  } catch (error) {
    console.error("Hint Error:", error)
    currentWordData.value.hint = "Service temporarily unavailable."
  } finally {
    loadingHint.value = false
    hintSnackbar.value = true
  }
}

// <><><><> Show Temp Message <><><><>
const showTemporaryMessage = (message) => {
  if (gameOver.value) return
  snackbarMsg.value = message
  showStats.value = false
  snackbar.value = true
  
  setTimeout(() => {
    if (!gameOver.value) snackbar.value = false
  }, 2000)
}

// <><><><> Show Stats <><><><>
const showStatsOnly = () => {
  snackbarMsg.value = "Your Stats"
  showStats.value = true
  snackbar.value = true
}

// <><><><> Shakes Tiles <><><><>
const shakeActive = ref(false)
const shakeRow = () => {
  shakeActive.value = true
  setTimeout(() => {
    shakeActive.value = false
  }, 500)
}

// <><><><> Expose For Testing <><><><> 
defineExpose({
  revealHint,
  hintSnackbar,
  currentWordData
})

// <><><><> Initialize Game Session & Restore State <><><><>
onMounted(async() => { 
  await loadDictionary()
  window.addEventListener('keydown', handleInput)
  
  const savedTheme = localStorage.getItem('user-theme')
  if (savedTheme) theme.global.name.value = savedTheme

  const savedStats = localStorage.getItem('wordle-stats')
  if (savedStats) stats.value = JSON.parse(savedStats)

  const today = new Date().toDateString()
  const playedDailyToday = localStorage.getItem('daily-finished-' + today)
  const savedStateStr = localStorage.getItem('daily-board-state')
  const savedState = savedStateStr ? JSON.parse(savedStateStr) : null

  const dateSeed = new Date().setHours(0,0,0,0)
  const dailyIndex = dateSeed % (WORD_LIST.length || 1)
  const dailyWordObj = WORD_LIST[dailyIndex]

  if (dailyWordObj && (!playedDailyToday || (savedState && savedState.solution === (dailyWordObj.word || dailyWordObj).toUpperCase()))) {
    currentWordData.value = typeof dailyWordObj === 'string' ? { word: dailyWordObj, hint: '' } : { ...dailyWordObj }
    isDaily.value = true
    localStorage.setItem('last-played-daily', today)
  } else {
    const randomWord = getRandomWord()
    currentWordData.value = typeof randomWord === 'string' ? { word: randomWord, hint: '' } : { ...randomWord }
  }

  if (!currentWordData.value || !currentWordData.value.word) {
    currentWordData.value = WORD_LIST[0] || { word: 'APPLE', hint: 'A fruit' }
  }

  if (savedState && currentWordData.value.word) {
    const currentWordUpper = currentWordData.value.word.toUpperCase()
    
    if (savedState.solution === currentWordUpper) {
      board.value = savedState.board
      currentRow.value = savedState.currentRow
      letterStates.value = savedState.letterStates
      gameOver.value = savedState.gameOver
      
      if (gameOver.value) {
        showStats.value = true
        snackbar.value = true
        snackbarMsg.value = savedState.snackbarMsg || 'Game Over!'
      }
      console.log("Restoration successful for:", currentWordUpper)
    } else {
      console.log("Solution mismatch. Saved:", savedState.solution, "Current:", currentWordUpper)
      localStorage.removeItem('daily-board-state')
    }
  }
})

onUnmounted(() => window.removeEventListener('keydown', handleInput))
</script>

<template>
  <v-app>
    <v-app-bar flat border px-2>
      <div class="d-flex align-center ml-2 ml-sm-4" style="min-width: 0;">
        <span class="wordle-title">Wordle</span>
        <v-chip 
          v-if="isDaily" 
          size="x-small" 
          color="green" 
          class="ml-2 font-weight-black" 
          variant="flat"
          style="height: 18px;"
        >
          DAILY
        </v-chip>
      </div>
      
      <v-spacer></v-spacer>

      <div class="d-flex align-center">
        <v-btn 
          icon="mdi-chart-line" 
          @click="showStatsOnly" 
          @mousedown.prevent
        ></v-btn>
        
        <v-btn 
          icon="mdi-lightbulb-on-outline"
          @click="revealHint" 
          :disabled="gameOver"
          :loading="loadingHint"
          @mousedown.prevent
        ></v-btn>

        <client-only>
          <v-btn 
            :icon="themeIcon" 
            @click="toggleTheme" 
            @mousedown.prevent
          ></v-btn>
        </client-only>

        
      </div>
    </v-app-bar>

    <v-main class="d-flex flex-column" style="height: calc(100vh - 64px); overflow: hidden;">
      <v-spacer></v-spacer>

      <v-snackbar
        v-model="hintSnackbar"
        location="top"
        elevation="24"
        color="#212121"
        :timeout="6000"
        class="game-over-snackbar mini-box" 
      >
        <div class="d-flex align-center">
          <v-icon start color="white" size="24"class="me-4">mdi-information-outline</v-icon>
          <span class="text-body-2">{{ currentWordData.hint || 'Generating Hint...' }}</span>
        </div>
        
        <template v-slot:actions>
          <v-btn 
            class="play-again-btn mr-2" 
            variant="flat"
            rounded="xl"
            size="small"
            @click="hintSnackbar = false"
          >
            Close
          </v-btn>
        </template>
      </v-snackbar>

      <v-container class="py-2">
        <div class="board-wrapper">
          <v-row v-for="(row, i) in board" :key="i" justify="center" :class="{ 'shake': shakeActive && i === currentRow }"class="board-row mb-2" no-gutters style="gap: 8px;">
            <v-col v-for="(cell, j) in row" :key="j" cols="auto">
              <v-sheet class="tile-sheet" :class="{ 'tile-active': cell.letter !== '', 'tile-flip': cell.status !== 'default' }" elevation="0" :style="{ transitionDelay: cell.status !== 'default' ? `${j * 150}ms` : '0ms' }">
                <div class="tile-inner">
                  <div class="tile-front d-flex align-center justify-center font-weight-bold">{{ cell.letter }}</div>
                  <div class="tile-back d-flex align-center justify-center font-weight-bold text-white" :class="cell.status">{{ cell.letter }}</div>
                </div>
              </v-sheet>
            </v-col>
          </v-row>
        </div>
      </v-container>

      <v-spacer></v-spacer>

      <v-container class="keyboard-container px-1">
        <div v-for="(row, i) in rows" :key="i" class="keyboard-row">
          <div 
            v-for="key in row" 
            :key="key" 
            class="key-wrapper"
            :class="{ 'wide-wrapper': key.length > 1 }"
          >
            <v-btn 
              block
              class="keyboard-btn font-weight-bold" 
              :class="{ 'text-white': letterStates[key] }" 
              :color="letterStates[key] === 'correct' ? 'green' : letterStates[key] === 'present' ? 'yellow-darken-2' : letterStates[key] === 'absent' ? 'grey-darken-4' : 'grey-darken-2'" 
              @click="handleInput(key)" 
              @mousedown.prevent
            >
              <template v-if="key === 'BACKSPACE'"><v-icon icon="mdi-backspace-outline" class="backspace-icon"></v-icon></template>
              <template v-else>{{ key }}</template>
            </v-btn>
          </div>
        </div>
      </v-container>

      <v-fade-transition>
        <div v-if="gameOver" class="glass-overlay"></div>
      </v-fade-transition>
    </v-main>

    <v-snackbar 
      v-model="snackbar" 
      :location="gameOver ? 'center' : 'top'" 
      :timeout="gameOver ? -1 : 5000" 
      :color="gameOver ? '#121212' : 'grey-darken-3'"
      elevation="24"
      :class="['game-over-snackbar', { 'mini-box': !gameOver }]"
    >
      <div class="d-flex flex-column align-center pa-1">
        <div :class="[gameOver ? 'text-h6 font-weight-bold' : 'text-body-2 font-weight-medium']" class="text-center mb-2">
          {{ snackbarMsg }}
        </div>

        <div v-if="showStats && (gameOver || stats.gamesPlayed > 0)" class="stats-container d-flex justify-space-around w-100 my-4">
          <div class="stat-box">
            <div class="text-h5 font-weight-bold">{{ stats.gamesPlayed }}</div>
            <div class="text-caption">PLAYED</div>
          </div>
          <div class="stat-box">
            <div class="text-h5 font-weight-bold text-green">{{ stats.wins }}</div>
            <div class="text-caption">WINS</div>
          </div>
            <div class="stat-box text-center">
            <div class="text-h5 font-weight-bold text-red">{{ stats.losses }}</div>
            <div class="text-caption">LOSSES</div>
          </div>
          <div class="stat-box text-center">
            <div class="text-h5 font-weight-bold">{{ stats.averageAttempts }}</div>
            <div class="text-caption">AVG</div>
          </div>
        </div>

        <div v-if="gameOver" class="text-center w-100 mt-2">
          <div class="text-overline text-grey-lighten-1">The word was</div>
          <div class="text-h4 font-weight-black text-green mb-2" style="letter-spacing: 2px;">
            {{ solution || ''}}
          </div>
          
          <v-divider class="mx-10 mb-3" color="white"></v-divider>

          <div class="text-body-2 px-6 mb-4 text-grey-lighten-1 italic">
            {{ currentWordData.hint ? currentWordData.hint.replace(new RegExp(solution, 'gi'), solution) : 'Loading definition...' }}
          </div>
        </div>

        <v-btn 
          v-if="gameOver" 
          class="play-again-btn px-10 mt-2"
          variant="flat"
          rounded="xl"
          size="large"
          @click="resetGame"
        >
          Next Word
        </v-btn>
      </div>
      <template v-slot:actions>
        <v-btn
          v-if="!gameOver"
          class="play-again-btn mr-2" 
          variant="flat"
          rounded="xl"
          size="small"
          @click="snackbar = false"
        >
          Close
        </v-btn> 
      </template>
    </v-snackbar>
  </v-app>
</template>

<style>

/* <><><><> Fonts <><><><> */
.wordle-title {
  font-family: 'Grand Hotel', cursive !important;
  text-transform: none !important; 
  font-size: 3.2rem !important;   
  letter-spacing: 0.5px !important;
  line-height: 1 !important;
  font-weight: 400 !important;
}

.v-application,
.v-btn,
.tile-front, 
.tile-back {
  font-family: 'Libre Franklin', sans-serif !important;
}

.tile-sheet {
  perspective: 1000px;
  background: transparent !important;
  border: none !important;
  width: 58px;
  height: 58px;
}

.tile-inner {
  position: relative;
  width: 100%;
  height: 100%;
  transition: transform 0.6s cubic-bezier(0.45, 0.05, 0.55, 0.95);
  transform-style: preserve-3d;
}

/* <><><><> Flip Animation <><><><>*/
.tile-flip .tile-inner {
  transform: rotateX(180deg);
}

.tile-front, .tile-back {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  backface-visibility: hidden;
  border-radius: 4px;
  text-transform: uppercase;
  display: flex;
  align-items: center;
  justify-content: center;
}

.tile-front {
  border: 2px solid #3a3a3c;
}

.v-theme--light .tile-front {
  border-color: #d3d6da;
}

.tile-active .tile-front {
  border-color: #818384;
}

.tile-back {
  transform: rotateX(180deg);
}

/* <><><><> Wordle Colors <><><><> */
.correct { background-color: #4caf50; }
.present { background-color: #fbc02d; }
.absent  { background-color: #424242; }

/* <><><><> Glass Overlay <><><><> */
.glass-overlay {
  position: absolute;
  top: 0; left: 0; right: 0; bottom: 0;
  z-index: 5;
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  background: rgba(255, 255, 255, 0.3);
  pointer-events: none;
  transition: opacity 0.6s ease;
}

.v-theme--dark .glass-overlay {
  background: rgba(18, 18, 18, 0.5);
  backdrop-filter: blur(12px) brightness(0.8);
  -webkit-backdrop-filter: blur(12px) brightness(0.8);
}

/* <><><><> Button Styles <><><><> */
.play-again-btn {
  background-color: rgb(76, 175, 80) !important; 
  color: white !important;
  font-weight: 700 !important;
  letter-spacing: 1.2px;
  text-transform: uppercase;
  border: 1px solid rgba(255, 255, 255, 0.2);
  transition: all 0.3s cubic-bezier(0.25, 0.8, 0.25, 1) !important;
  box-shadow: 0 4px 15px rgba(76, 175, 80, 0.3);
}

.play-again-btn:hover {
  background-color: rgb(67, 160, 71) !important;
  transform: translateY(-2px);
  letter-spacing: 2px;
  box-shadow: 0 6px 20px rgba(76, 175, 80, 0.4);
}

.play-again-btn:active {
  transform: translateY(1px);
  box-shadow: 0 2px 10px rgba(76, 175, 80, 0.3);
}

.mini-box :deep(.v-snackbar__content) {
  padding: 12px 20px !important;
  width: auto !important;
}

.game-over-snackbar:not(.mini-box) :deep(.v-snackbar__content) {
  padding: 40px 24px !important;
  width: 340px !important;
  border-radius: 24px !important;
}

.text-green {
  color: #4caf50 !important
}

.italic {
  font-style: italic;
  line-height: 1.4;
}

/* <><><><> Desktop Settings <><><><> */
.keyboard-row {
  display: flex;
  justify-content: center;
  width: 100%;
  max-width: 500px;
  margin: 0 auto 8px auto !important;
  touch-action: none;
}

.key-wrapper {
  flex: 0 0 44px; 
  height: 58px;
  margin: 0 3px;
}

.wide-wrapper {
  flex: 0 0 65px;
}

.keyboard-btn {
  height: 100% !important;
  width: 100% !important;
  min-width: 0 !important;
  padding: 0 !important;
  text-transform: uppercase;
}

.keyboard-container {
  margin-bottom: 60px !important;
}

/* <><><><> Mobile Settings <><><><> */
@media (max-width: 600px) {
  .keyboard-row {
    max-width: 100%;
    margin-bottom: 5px !important;
    padding: 0 2px;
  }

  .keyboard-container {
    margin-bottom: 45px !important;
    padding-bottom: env(safe-area-inset-bottom) !important;
  }

  .key-wrapper {
    flex: 1 1 calc(10% - 2px) !important;
    margin: 0 1px !important;
    height: 45px;
  }

  .wide-wrapper {
    flex: 1.5 1 0 !important;
  }

  .keyboard-btn {
    font-size: 0.70rem !important;
  }

  .wordle-title {
    font-size: 2.2rem !important; 
    margin-left: 8px !important;
  }

  .v-app-bar .v-btn {
    width: 36px !important;
    height: 36px !important;
  }
}

/* <><><><> Snackbar Styles <><><><> */
.game-over-snackbar :deep(.v-snackbar__content) {
  padding: 40px 24px !important;
  border-radius: 24px !important;
  border: 1px solid rgba(76, 175, 80, 0.4) !important;
  background: #121212 !important;
  box-shadow: 0 20px 50px rgba(0, 0, 0, 0.7) !important;
  max-width: 90vw !important;
  width: 340px !important;
}

.text-overline {
  font-family: 'Libre Franklin', sans-serif !important;
  font-size: 0.75rem !important;
  text-transform: uppercase;
}

/* <><><><> Shake Animation <><><><> */
.shake {
  animation: shake 0.5s cubic-bezier(0.36, 0.07, 0.19, 0.97) both;
}

@keyframes shake {
  10%, 90% { transform: translate3d(-1px, 0, 0); }
  20%, 80% { transform: translate3d(2px, 0, 0); }
  30%, 50%, 70% { transform: translate3d(-4px, 0, 0); }
  40%, 60% { transform: translate3d(4px, 0, 0); }
}

/* <><><><> Backspace Icon Adjustments <><><><> */
.wide-wrapper .v-btn {
  padding: 0 4px !important;
}

.backspace-icon {
  font-size: 20px !important;
  display: flex;
  align-items: center;
  justify-content: center;
}

@media (max-width: 600px) {
  .backspace-icon {
    font-size: 16px !important;
  }

  .wide-wrapper {
    flex: 1.8 1 0 !important;
  }
}
</style>