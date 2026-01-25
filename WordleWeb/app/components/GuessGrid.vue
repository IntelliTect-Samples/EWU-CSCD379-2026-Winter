<template>
  <div class="guesses">
    <div v-for="(guess, idx) in guesses" :key="idx" class="guess-row">
      <span
        v-for="(letter, letterIdx) in guess"
        :key="letterIdx"
        :class="getLetterClass(idx, letterIdx)"
      >
        {{ letter.toUpperCase() }}
      </span>
    </div>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  guesses: string[];
  getLetterClass: (guessIdx: number, letterIdx: number) => string;
}>();
</script>

<style scoped>
.guesses {
  margin-bottom: 30px;
  min-height: 250px;
}

.guess-row {
  display: flex;
  gap: 8px;
  margin-bottom: 8px;
  justify-content: center;
}

.guess-row span {
  width: 50px;
  height: 50px;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 2px solid #ccc;
  font-weight: bold;
  font-size: 18px;
  border-radius: 4px;
}

:deep(.correct) {
  background-color: #6aaa64;
  color: white;
  border-color: #6aaa64;
}

:deep(.wrong-position) {
  background-color: #c9b458;
  color: white;
  border-color: #c9b458;
}

:deep(.not-found) {
  background-color: #d3d6da;
  color: #333;
  border-color: #d3d6da;
}
</style>
