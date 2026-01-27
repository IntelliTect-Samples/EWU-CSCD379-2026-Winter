<template>
    <v-btn @mouseenter="$emit('enter')" tile min-width="20px" variant="flat" class="mr-1 letter"
        :color="getColorForState()">
        {{ letter.character }}
    </v-btn>
</template>

<script setup lang="ts">
import { Letter, LetterState } from '~~/classes/wordle-game';

const emit = defineEmits<{
    enter: []
}>()

const props = defineProps({
    letter: { type: Letter, required: true }
});

const getColorForState = (): string => {
    switch (props.letter.state) {
        case LetterState.Correct:
            return "green";
        case LetterState.Misplaced:
            return "yellow";
        default:
            return "grey";
    }
}
</script>