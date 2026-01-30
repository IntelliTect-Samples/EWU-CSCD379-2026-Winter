<template>
  <div class="wordle-container">
    <div class="top-right-buttons">
      <label class="hardmode-toggle">
        <input type="checkbox" v-model="hardMode" /> Hard Mode
      </label>
      <button
        class="hint-btn"
        @click="handleHint"
        :disabled="hardMode"
        :class="{ disabled: hardMode }"
      >
        Hint
      </button>
      <button class="rules-btn" @click="showRules = true">Rules</button>
    </div>
    <h1>Wordle</h1>
    <p class="challenge-label">
      {{ isPlayingDailyWord ? "📅 Word of the Day" : "🎲 Random Word" }}
    </p>

    <RulesModal :show="showRules" @close="showRules = false" />

    <GameMessage
      v-if="gameOver"
      :won="won"
      :secretWord="secretWord"
      :stats="stats"
      :winRatio="winRatio"
      :averageGuesses="averageGuesses"
      @reset="resetGame"
    />

    <div v-else>
      <GuessGrid :guesses="guesses" :getLetterClass="getLetterClass" />

      <GuessInput
        v-model="currentGuess"
        @submit="submitGuess"
        @focus="errorMessage = ''"
        ref="guessInputRef"
      />

      <p class="attempts">Attempts: {{ guesses.length }} / 6</p>
      <p v-if="errorMessage" class="error">{{ errorMessage }}</p>

      <GameKeyboard
        :getKeyboardClass="getKeyboardClass"
        @addLetter="addLetter"
        @backspace="backspace"
        @submit="submitGuess"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import allWordsList from "~~/public/allWords.json";
import {
  getDailyWord,
  getRandomWord,
  getTodayDateString,
} from "~/scripts/wordUtils";
import { useStats } from "~/scripts/useStats";
import { useGameState } from "~/scripts/useGameState";
import { useHint } from "~/scripts/useHint";
import { useLetterClasses } from "~/scripts/useLetterClasses";

const allWords: string[] = allWordsList;

// Refs
const showRules = ref(false);
const secretWord = ref<string>("wordle");
const guesses = ref<string[]>([]);
const currentGuess = ref("");
const errorMessage = ref("");
const isPlayingDailyWord = ref(true);
const hardMode = ref(false);
const guessInputRef = ref<{ blur: () => void; shake: () => void } | null>(null);

// Composables
const { stats, winRatio, averageGuesses, loadStats, recordGameEnd } =
  useStats();
const { saveGameState, loadGameState, loadGameModePreference } = useGameState();
const { getKeyboardClass, getLetterClass } = useLetterClasses(
  guesses,
  secretWord,
);

// Computed
const gameOver = computed(() => guesses.value.length >= 6 || won.value);
const won = computed(() =>
  guesses.value.some((guess) => guess === secretWord.value),
);

// Hint composable
const { useHint: handleHint } = useHint(
  guesses,
  secretWord,
  currentGuess,
  gameOver,
);

// Game actions
const initGame = () => {
  const savedState = loadGameState();

  if (savedState) {
    // Same day - restore full game state
    secretWord.value = savedState.secretWord;
    guesses.value = savedState.guesses;
    isPlayingDailyWord.value = savedState.isDaily ?? true;
  } else {
    // New day or no saved state - check the user's mode preference
    const prefersDaily = loadGameModePreference();
    isPlayingDailyWord.value = prefersDaily;
    guesses.value = [];

    if (prefersDaily) {
      secretWord.value = getDailyWord();
    } else {
      // User was playing random words, start a new random game
      secretWord.value = getRandomWord();
    }
  }
  currentGuess.value = "";
  errorMessage.value = "";
};

const submitGuess = () => {
  errorMessage.value = "";

  if (currentGuess.value.length !== 5) {
    errorMessage.value = "Word must be 5 letters";
    return;
  }

  const guess = currentGuess.value.toLowerCase();
  if (!allWords.includes(guess)) {
    errorMessage.value = "Not a valid word";
    currentGuess.value = "";
    guessInputRef.value?.shake();
    guessInputRef.value?.blur();
    return;
  }

  // Hardmode validation
  if (hardMode.value && guesses.value.length > 0) {
    const target = secretWord.value;
    const targetArr = target.split("");

    // Build constraints from all previous guesses
    const mustBeAt: (string | null)[] = [null, null, null, null, null];
    const mustInclude = new Set<string>();
    const mustNotUse = new Set<string>();

    for (const prevGuess of guesses.value) {
      const prevArr = prevGuess.split("");
      const used = Array(5).fill(false);

      // Pass 1: find greens
      for (let i = 0; i < 5; i++) {
        if (prevArr[i] === targetArr[i]) {
          mustBeAt[i] = prevArr[i] ?? null;
          used[i] = true;
        }
      }

      // Pass 2: find yellows and greys
      for (let i = 0; i < 5; i++) {
        if (prevArr[i] === targetArr[i]) continue;
        let foundElsewhere = false;
        for (let j = 0; j < 5; j++) {
          if (!used[j] && prevArr[i] === targetArr[j]) {
            foundElsewhere = true;
            used[j] = true;
            break;
          }
        }
        const letter = prevArr[i];
        if (letter) {
          if (foundElsewhere) {
            mustInclude.add(letter);
          } else {
            mustNotUse.add(letter);
          }
        }
      }
    }

    // Check grey letters: guess must not contain any mustNotUse letters
    for (const letter of mustNotUse) {
      if (guess.includes(letter)) {
        errorMessage.value = `Hard Mode: Cannot use letter "${letter.toUpperCase()}"`;
        currentGuess.value = "";
        guessInputRef.value?.shake();
        guessInputRef.value?.blur();
        return;
      }
    }

    // Check yellow letters: guess must include all mustInclude letters
    for (const letter of mustInclude) {
      if (!guess.includes(letter)) {
        errorMessage.value = `Hard Mode: Must use letter "${letter.toUpperCase()}"`;
        currentGuess.value = "";
        guessInputRef.value?.shake();
        guessInputRef.value?.blur();
        return;
      }
    }

    // Check green letters: guess must have mustBeAt letters at exact positions
    for (let i = 0; i < 5; i++) {
      if (mustBeAt[i] && guess[i] !== mustBeAt[i]) {
        errorMessage.value = `Hard Mode: Letter "${mustBeAt[i]!.toUpperCase()}" must be at position ${i + 1}`;
        currentGuess.value = "";
        guessInputRef.value?.shake();
        guessInputRef.value?.blur();
        return;
      }
    }
  }

  guesses.value.push(guess);
  currentGuess.value = "";

  saveGameState(secretWord.value, guesses.value, isPlayingDailyWord.value);

  if (gameOver.value) {
    recordGameEnd(won.value, guesses.value.length);
  }
};

const addLetter = (letter: string) => {
  if (currentGuess.value.length < 5) {
    currentGuess.value += letter.toLowerCase();
  }
};

const backspace = () => {
  currentGuess.value = currentGuess.value.slice(0, -1);
};

const resetGame = () => {
  let newWord = getRandomWord();
  let attempts = 0;
  while (newWord === secretWord.value && attempts < 10) {
    newWord = getRandomWord();
    attempts++;
  }
  secretWord.value = newWord;
  isPlayingDailyWord.value = false;
  guesses.value = [];
  currentGuess.value = "";
  errorMessage.value = "";
};

onMounted(() => {
  loadStats();
  initGame();

  // Midnight reset logic
  let lastDateString = getTodayDateString();
  setInterval(() => {
    const currentDateString = getTodayDateString();
    if (currentDateString !== lastDateString) {
      lastDateString = currentDateString;
      initGame();
    }
  }, 60 * 1000);
});
</script>

<style scoped>
.wordle-container {
  max-width: 500px;
  margin: 40px auto;
  padding: 20px;
  font-family: Arial, sans-serif;
  text-align: center;
}

h1 {
  color: #333;
  margin-bottom: 10px;
}

.challenge-label {
  font-size: 16px;
  color: #666;
  margin-bottom: 20px;
  font-weight: 500;
}

.attempts {
  color: #666;
  margin-bottom: 10px;
}

.error {
  color: #d32f2f;
  font-weight: bold;
}

.top-right-buttons {
  position: absolute;
  top: 20px;
  right: 20px;
  display: flex;
  gap: 10px;
}

.hint-btn,
.rules-btn {
  padding: 8px 14px;
  background-color: #e3e3e3;
  color: #333;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  font-weight: bold;
}

.hint-btn:hover,
.rules-btn:hover {
  background-color: #d3d3d3;
}

.hardmode-toggle {
  display: flex;
  align-items: center;
  gap: 4px;
  font-size: 14px;
  font-weight: bold;
  color: #333;
  cursor: pointer;
}

.hardmode-toggle input {
  cursor: pointer;
}

.hint-btn.disabled {
  opacity: 0.5;
  cursor: not-allowed;
}
</style>
