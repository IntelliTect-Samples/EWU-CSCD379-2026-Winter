<template>
    <v-container>
        <div v-for="word of guesses">
            <v-btn variant="flat" :color="getColor(letter.state)" v-for="letter of word">{{ letter.character }}</v-btn>
        </div>
        <v-text-field label="Label" variant="outlined" v-model="guess"></v-text-field>
        <v-btn variant="flat" color="primary" @click="submit">Submit</v-btn>
        {{ guess }}
    </v-container>
</template>

<script setup lang="ts">
import { words } from "../classes/words";

const guesses: Ref<Array<Array<Letter>>> = ref([]);
const targetWord = ref("");
const guess = ref("");

pickRandomTargetWord();

function pickRandomTargetWord() {
    targetWord.value = words[Math.floor(Math.random() * words.length)]!;
}

function submit() {
    const letters: Array<Letter> = [];

    // TODO: check wordlist
    if (words.indexOf(guess.value.toLowerCase()) < 0) {
        return; // bad!
    }

    const targetLetters = targetWord.value.toUpperCase().split("");
    const guessLetters = guess.value.toUpperCase().split("");

    for (const [index, value] of guessLetters.entries()) {
        let isCorrect = false;
        if (targetLetters[index] === value) {
            isCorrect = true;
            guessLetters[index] = " ";
            targetLetters[index] = " ";
        }
        const letter = new Letter(value, isCorrect ? LetterState.Correct : LetterState.Wrong);
        letters.push(letter);
    }

    for (const [letterIndex, letter] of letters.entries()) {
        if (letter.state !== LetterState.Correct) {
            for (const [targetIndex, targetValue] of targetLetters.entries()) {
                if (letter.character === targetValue) {
                    letter.state = LetterState.Misplaced;
                    targetLetters[targetIndex] = " ";
                    break;
                }
            }
        }
    }

    guesses.value.push(letters);
    guess.value = "";
}

function getColor(state: LetterState) {
    switch (state) {
        case LetterState.Correct:
            return "green";
        case LetterState.Misplaced:
            return "yellow";
        default:
            return "white";
    }
}

class Letter {
    constructor(
        public character: string,
        public state: LetterState
    ) { }
}

enum LetterState {
    Correct,
    Misplaced,
    Wrong,
}
</script>