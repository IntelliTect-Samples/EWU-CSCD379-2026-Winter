<script setup lang="ts">
import type { TileState } from '~/utils/wordle'

const props = defineProps<{
    letter: string
    state: TileState
}>()

function tileClass() {
    switch (props.state) {
    case 'correct': return 'tile tile--correct'
    case 'present': return 'tile tile--present'
    case 'absent': return 'tile tile--absent'
    default: return 'tile'
    }
}
</script>

<template>
    <div :class="tileClass()">
    {{ letter.toUpperCase() }}
    </div>
</template>

<style scoped>
.tile {
    width: 62px;
    height: 62px;
    border: 2px solid var(--tile-border);
    display: grid;
    place-items: center;
    font-size: 2rem;
    font-weight: 800;
    user-select: none;
    border-radius: 2px;
    background: var(--tile-empty);
    color: var(--text);
    text-transform: uppercase;
}

.tile--correct {
    background: var(--correct);
    border-color: var(--correct);
    color: white;
}

.tile--present {
    background: var(--present);
    border-color: var(--present);
    color: white;
}

.tile--absent {
    background: var(--absent);
    border-color: var(--absent);
    color: white;
}

</style>
