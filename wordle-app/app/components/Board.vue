<script setup lang="ts">
import Row from '~/components/Row.vue'
import type { TileState } from '~/utils/wordle'

const props = defineProps<{
    guesses: string[]
    states: TileState[][]
    shakeRow: number | null
}>()

function guessAt(r : number) {
    return props.guesses?.[r] ?? ''
}

function statesAt(r: number): TileState[] {
    return props.states?.[r] ?? []
}
</script>

<template>
    <div class="board">
        <Row
            v-for="r in 6"
            :key="r"
            :guess="guessAt(r - 1)"
            :states="statesAt(r - 1)"
            :shake="props.shakeRow === r - 1"
        />
    </div>
</template>

<style scoped>
.board {
    display: grid;
    gap: 6px;
    justify-content: center;
    padding: 18px 0 10px;
}
</style>