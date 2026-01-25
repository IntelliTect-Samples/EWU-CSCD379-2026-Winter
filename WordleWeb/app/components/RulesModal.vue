<template>
  <div v-if="show" class="modal-overlay" @click.self="$emit('close')">
    <div class="modal">
      <h2>Rules</h2>
      <p>
        Guess the secret 5-letter word in 6 tries. Each guess must be a valid
        word. After each guess, the color of the tiles will change to show how
        close your guess was to the word.
      </p>
      <ul class="rules-legend">
        <li>
          <span class="emoji-box" style="background: #c9b458">🟨</span> Letter
          in the word but not in the right place
        </li>
        <li>
          <span class="emoji-box" style="background: #6aaa64">🟩</span> Letter
          in the word and in the right place
        </li>
        <li>
          <span class="emoji-box" style="background: #787c7e">⬜</span> Letter
          not in the word
        </li>
      </ul>
      <button @click="$emit('close')">Close</button>
    </div>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  show: boolean;
}>();

defineEmits<{
  close: [];
}>();
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background: rgba(0, 0, 0, 0.3);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal {
  background: #fff;
  padding: 30px 24px 18px 24px;
  border-radius: 10px;
  box-shadow: 0 2px 16px rgba(0, 0, 0, 0.15);
  min-width: 300px;
  text-align: center;
}

.modal h2 {
  margin-top: 0;
}

.modal button {
  margin-top: 18px;
  padding: 8px 18px;
  background-color: #6aaa64;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  font-weight: bold;
}

.modal button:hover {
  background-color: #5a9a54;
}

.rules-legend {
  list-style: none;
  padding: 0;
  margin: 18px 0 10px 0;
  text-align: left;
}

.rules-legend li {
  display: flex;
  align-items: center;
  margin-bottom: 8px;
  font-size: 15px;
}

.emoji-box {
  display: inline-block;
  width: 28px;
  height: 28px;
  border-radius: 6px;
  margin-right: 10px;
  text-align: center;
  font-size: 22px;
  line-height: 28px;
  color: #fff;
  font-weight: bold;
}
</style>
