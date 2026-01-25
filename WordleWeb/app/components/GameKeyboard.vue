<template>
  <div class="keyboard">
    <div v-for="(row, idx) in keyboardRows" :key="idx" class="keyboard-row">
      <button
        v-for="letter in row"
        :key="letter"
        @click="$emit('addLetter', letter)"
        :class="getKeyboardClass(letter)"
        class="key"
      >
        {{ letter }}
      </button>
    </div>
    <div class="keyboard-row">
      <button @click="$emit('backspace')" class="key special-key">
        Backspace
      </button>
      <button @click="$emit('submit')" class="key special-key enter-key">
        Enter
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  getKeyboardClass: (letter: string) => string;
}>();

defineEmits<{
  addLetter: [letter: string];
  backspace: [];
  submit: [];
}>();

const keyboardRows = [
  "QWERTYUIOP".split(""),
  "ASDFGHJKL".split(""),
  "ZXCVBNM".split(""),
];
</script>

<style scoped>
.keyboard {
  margin-top: 30px;
  display: flex;
  flex-direction: column;
  gap: 6px;
  align-items: center;
}

.keyboard-row {
  display: flex;
  gap: 4px;
  justify-content: center;
  flex-wrap: wrap;
}

.key {
  min-width: 38px;
  height: 38px;
  padding: 0 8px;
  background-color: #d3d6da;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 13px;
  font-weight: 600;
  text-transform: uppercase;
  transition: all 0.1s;
}

.key:hover:not(:disabled) {
  background-color: #b0b3b8;
}

.key:disabled {
  cursor: not-allowed;
  opacity: 0.5;
}

:deep(.key-correct) {
  background-color: #6aaa64 !important;
  color: white;
  border: 1px solid #6aaa64;
}

:deep(.key-wrong-position) {
  background-color: #c9b458 !important;
  color: white;
  border: 1px solid #c9b458;
}

:deep(.key-not-found) {
  background-color: #787c7e !important;
  color: white;
  border: 1px solid #787c7e;
}

.special-key {
  min-width: 60px;
  background-color: #6aaa64;
  color: white;
  font-size: 12px;
}

.special-key:hover:not(:disabled) {
  background-color: #5a9a54;
}

.enter-key {
  min-width: 80px;
}
</style>
