<template>
    <v-container class="mx-auto">
        <v-row width="auto" v-for="word of guesses">
            <v-spacer />
            <v-col class="py-1" cols="12" sm="8" md="6" lg="4">
                <v-btn tile min-width="20px" variant="flat" class="mr-1 letter"
                    :color="game.getColorForState(letter.state)" v-for="letter of word">{{
                        letter.character }}</v-btn>
            </v-col>
            <v-spacer />
        </v-row>
        <v-row>
            <v-col>
                <v-text-field append-inner-icon="mdi-arrow-right" @click:append-inner="submit" @keyup.enter="submit"
                    label="Guess" variant="outlined" v-model="guess"></v-text-field>
            </v-col>
        </v-row>
    </v-container>
</template>

<script setup lang="ts">
import { WordleGame } from "../classes/wordle-game";

const game = new WordleGame();
const guess = ref("");
const guesses = ref(game.getGuesses());

function submit() {
    game.submitGuess(guess.value);
    guesses.value = [...game.getGuesses()];
    guess.value = "";
}
</script>

<style lang="css" scoped>
div .guess-word {
    margin: 5px;
    border: black solid 5px;
    padding: 5px;
    width: 60%;
    left: 20%;
}

.letter {
    width: calc(20% - 4px);
}
</style>