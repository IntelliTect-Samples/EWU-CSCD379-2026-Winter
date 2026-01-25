<template>
  <div class="message">
    <p v-if="won" class="won">You Won! 🎉</p>
    <p v-else class="lost">
      Game Over! The word was: <strong>{{ secretWord.toUpperCase() }}</strong>
    </p>

    <div v-if="won">
      <div v-if="loading">Looking up definition...</div>
      <div v-else-if="definition">
        Definition: <span class="definition">{{ definition }}</span>
      </div>
      <div v-else-if="error" class="definition-error">No definition found.</div>
    </div>

    <StatsSheet
      :stats="stats"
      :winRatio="winRatio"
      :averageGuesses="averageGuesses"
    />

    <button @click="$emit('reset')" class="reset-btn">
      Play Again With Random Word
    </button>
  </div>
</template>

<script setup lang="ts">
import type { Stats } from "~/scripts/useStats";
import { onMounted, watch } from "vue";
import { useDefinition } from "~/scripts/useDefinition";

const props = defineProps<{
  won: boolean;
  secretWord: string;
  stats: Stats;
  winRatio: string;
  averageGuesses: string;
}>();

defineEmits<{
  reset: [];
}>();

const { definition, loading, error, fetchDefinition } = useDefinition();

watch(
  () => props.won && props.secretWord,
  (val, oldVal) => {
    if (props.won && props.secretWord) {
      fetchDefinition(props.secretWord);
    }
  },
  { immediate: true },
);
</script>

<style scoped>
.message {
  font-size: 18px;
  margin-bottom: 30px;
}

.won {
  color: #6aaa64;
  font-weight: bold;
}

.definition {
  display: inline-block;
  margin-left: 6px;
  color: #333;
  font-style: italic;
}
.definition-error {
  color: #d32f2f;
  font-size: 14px;
  margin: 6px 0;
}

.lost {
  color: #d32f2f;
  font-weight: bold;
}

.reset-btn {
  padding: 10px 20px;
  background-color: #6aaa64;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 16px;
  font-weight: bold;
}

.reset-btn:hover {
  background-color: #5a9a54;
}
</style>
