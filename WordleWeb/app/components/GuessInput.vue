<template>
  <div class="input-section">
    <input
      :value="modelValue"
      @input="
        $emit('update:modelValue', ($event.target as HTMLInputElement).value)
      "
      @keyup.enter="$emit('submit')"
      @focus="$emit('focus')"
      ref="inputRef"
      placeholder="Enter 5-letter word"
      maxlength="5"
      :class="['guess-input', { shake: isShaking }]"
    />
    <button @click="$emit('submit')" class="submit-btn">Guess</button>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  modelValue: string;
}>();

defineEmits<{
  "update:modelValue": [value: string];
  submit: [];
  focus: [];
}>();

const inputRef = ref<HTMLInputElement | null>(null);
const isShaking = ref(false);

const blur = () => {
  inputRef.value?.blur();
};

const shake = () => {
  isShaking.value = true;
  setTimeout(() => {
    isShaking.value = false;
  }, 500);
};

defineExpose({ blur, shake });
</script>

<style scoped>
.input-section {
  margin-bottom: 20px;
  display: flex;
  gap: 10px;
  justify-content: center;
}

.guess-input {
  padding: 10px;
  font-size: 16px;
  border: 2px solid #ddd;
  border-radius: 4px;
  width: 200px;
}

.submit-btn {
  padding: 10px 20px;
  background-color: #6aaa64;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 16px;
  font-weight: bold;
}

.submit-btn:hover {
  background-color: #5a9a54;
}

.shake {
  animation: shake 0.5s ease-in-out;
}

@keyframes shake {
  0%,
  100% {
    transform: translateX(0);
  }
  10%,
  30%,
  50%,
  70%,
  90% {
    transform: translateX(-5px);
  }
  20%,
  40%,
  60%,
  80% {
    transform: translateX(5px);
  }
}
</style>
