<script setup lang="ts">
import Tile from '~/components/Tile.vue'
import type { TileState } from '~/utils/wordle'

const props = defineProps<{
    guess: string
    states: TileState[]
    shake: boolean
}>()

function letterAt(i: number) {
    return props.guess?.charAt(i) ?? ''
}

function stateAt(i: number): TileState {
    return props.states?.[i] ?? 'empty'
}
</script>

<template>
    <div class="row" :class="{ shake: props.shake }">
    <Tile
        v-for="i in 5"
        :key="i"
        :letter="letterAt(i - 1)"
        :state="stateAt(i - 1)"
    />
    </div>
</template>

<style scoped>
.shake {
    animation: shake 0.35s ease-in-out;
}

.row {
    --gap: 6px;
    --tile: clamp(44px, calc((100vw - 48px - (var(--gap) * 4)) / 5), 62px);

    display: grid;
    grid-template-columns: repeat(5, var(--tile));
    gap: var(--gap);
    justify-content: center;
}

@media (max-width: 420px) {
    .row {
    --gap: 5px;
    }
}

@keyframes shake {
    0% { transform: translateX(0); }
    20% { transform: translateX(-8px); }
    40% { transform: translateX(8px); }
    60% { transform: translateX(-6px); }
    80% { transform: translateX(6px); }
    100% { transform: translateX(0); }
}
</style>
