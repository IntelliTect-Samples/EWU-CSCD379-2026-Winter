<script setup lang="ts">
import type { TileState } from '~/utils/wordle'

const props = defineProps<{
    keyState: Record<string, TileState>
}>()

const emit = defineEmits<{
    (e: 'press', letter: string): void
    (e: 'enter'): void
    (e: 'backspace'): void
}>()

const ROWS = [
    ['q','w','e','r','t','y','u','i','o','p'],
    ['a','s','d','f','g','h','j','k','l'],
    ['enter','z','x','c','v','b','n','m','backspace'],
] as const

function keyClass(k: string) {
    const state = props.keyState[k] ?? 'empty'
    return {
    key: true,
    'key--correct': state === 'correct',
    'key--present': state === 'present',
    'key--absent': state === 'absent',
    'key--wide': k === 'enter' || k === 'backspace',
    }
}

function label(k: string) {
    if (k === 'enter') return 'Enter'
    if (k === 'backspace') return '⌫'
    return k.toUpperCase()
}

function onClick(k: string) {
    if (k === 'enter') emit('enter')
    else if (k === 'backspace') emit('backspace')
    else emit('press', k)
}
</script>

<template>
    <div class="kbd">
        <div v-for="(row, idx) in ROWS" :key="idx" class="kbd-row">
        <button
        v-for="k in row"
        :key="k"
        type="button"
        :class="keyClass(k)"
        @click="onClick(k)"
    >
        {{ label(k) }}
        </button>
        </div>
    </div>
</template>

<style scoped>
.kbd {
    width: 100%;
    max-width: 520px;
    margin: 0 auto;
    display: grid;
    gap: 8px;
    padding: 10px 8px 18px;
    box-sizing: border-box;
}

.kbd-row {
    display: flex;
    gap: 6px;
    width: 100%;
    justify-content: center;
}

.key {
    flex: 1 1 0;
    min-width: 0;
    height: 58px;
    padding: 0 6px;

    border: none;
    border-radius: 4px;
    font-weight: 800;

    background: var(--key-bg);
    color: var(--key-text);

    user-select: none;
    text-transform: uppercase;

    touch-action: manipulation;
    -webkit-tap-highlight-color: transparent;
}

.key--wide {
    flex: 1.6 1 0;
    font-size: 12px;
}

.key--correct { background: var(--correct); color: #fff; }
.key--present { background: var(--present); color: #fff; }
.key--absent  { background: var(--absent);  color: #fff; }

@media (max-width: 420px) {
    .kbd { padding: 8px 6px 14px; gap: 6px; }
    .kbd-row { gap: 4px; }
    .key { height: 52px; font-size: 12px; }
    .key--wide { font-size: 11px; }
}
</style>
