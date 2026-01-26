<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref, watch } from 'vue'
import Board from '~/components/Board.vue'
import Keyboard from '~/components/Keyboard.vue'
import StatsDialog from '~/components/StatsDialog.vue'
import DefinitionDialog from '~/components/DefinitionDialog.vue'
import { useWordleGame } from '~/composables/useWordleGame'
import { useStats } from '~/composables/useStats'
import { useDefinition } from '~/composables/useDefinition'
import type { WordDefinition } from '~/composables/useDefinition'

const game = useWordleGame()

const message = computed(() => game.message.value)
const hint = computed(() => game.hint.value)

const guesses = computed(() => game.guesses.value)
const states = computed(() => game.states.value)
const keyState = computed(() => game.keyboard.value)
const shakeRow = computed(() => game.shakeRow.value)

const status = computed(() => game.status.value)
const answer = computed(() => game.answer.value)

const stats = useStats()
const showStats = ref(false)

const currentRow = computed(() => game.row.value)
const recorded = ref(false)

const defApi = useDefinition()
const showDefinition = ref(false)
const definition = ref<WordDefinition | null>(null)
const definitionShownFor = ref<string | null>(null)

watch(status, async (s) => {
    if (!recorded.value) {
        if (s === 'won') {
            stats.recordWin(currentRow.value + 1)
            recorded.value = true
        } else if (s === 'lost') {
            stats.recordLoss()
            recorded.value = true
            }
        }

    if (s === 'won') {
        const w = answer.value
        if (definitionShownFor.value === w) return
        definitionShownFor.value = w

        definition.value = await defApi.fetchDefinition(w)
        showDefinition.value = true
    }
})

function onKeyDown(e: KeyboardEvent) {
    const key = e.key.toLowerCase()
    if (key === 'enter') return game.enter()
    if (key === 'backspace') return game.backspace()
    if (/^[a-z]$/.test(key)) return game.input(key)
}

function onNewWord() {
    game.newGame()
    recorded.value = false


    definitionShownFor.value = null
    showDefinition.value = false
    definition.value = null
}

function onResetStats() {
    stats.resetStats()
    recorded.value = false
}

onMounted(() => window.addEventListener('keydown', onKeyDown))
onBeforeUnmount(() => window.removeEventListener('keydown', onKeyDown))
</script>

<template>
    <div class="page">
    
    <header class="nyt-header">
        <div class="hdr-left">☰</div>

        <div class="hdr-title">WORDLE</div>

        <div class="hdr-right">
        <button class="hdr-icon" type="button" @click="game.giveHintNotInWord()">💡</button>
        <button class="hdr-icon" type="button" @click="showStats = true">📊</button>
        <button class="hdr-icon" type="button" @click="onNewWord">⟳</button>
        </div>
    </header>

    <div class="game">
        <div v-if="status !== 'playing'" class="banner" :class="status">
        <div v-if="status === 'won'">🎉 You Win!</div>
        <div v-else>😬 You Lose — word was <b>{{ answer.toUpperCase() }}</b></div>
        </div>

        <Board :guesses="guesses" :states="states" :shake-row="shakeRow" />

        <p v-if="message" class="msg">{{ message }}</p>
        <p v-if="hint" class="msg">{{ hint }}</p>

        <Keyboard
        :key-state="keyState"
        @press="game.input"
        @enter="game.enter"
        @backspace="game.backspace"
        />

        <StatsDialog
        v-model="showStats"
        :wins="stats.stats.value.wins"
        :losses="stats.stats.value.losses"
        :avg-attempts="stats.avgAttempts.value"
        @reset="onResetStats"
        />

        <DefinitionDialog v-model="showDefinition" :definition="definition" />
    </div>
    </div>
</template>

<style scoped>
.page {
    min-height: 100vh;
    background: var(--bg, #121213);
    color: var(--text, #ffffff);
}

.nyt-header {
    height: 52px;
    display: grid;
    grid-template-columns: 56px 1fr 180px; /* ⬅️ a bit wider for 💡 */
    align-items: center;
    border-bottom: 1px solid var(--tile-border, #3a3a3c);
    padding: 0 12px;
}

.hdr-left {
    font-size: 22px;
    opacity: 0.95;
}

.hdr-title {
    text-align: center;
    letter-spacing: 0.12em;
    font-weight: 900;
    font-size: 18px;
}

.hdr-right {
    display: flex;
    gap: 12px;
    justify-content: flex-end;
    align-items: center;
}

.hdr-icon {
    background: transparent;
    border: none;
    color: var(--text, #fff);
    font-size: 18px;
    cursor: pointer;
    padding: 6px;
    border-radius: 8px;
}
.hdr-icon:hover {
    background: rgba(255, 255, 255, 0.08);
}

.game {
    max-width: 520px;
    margin: 0 auto;
    padding: 18px 16px 24px;
}

.banner {
    text-align: center;
    font-weight: 800;
    padding: 10px 12px;
    border-radius: 10px;
    margin: 10px auto 6px;
}

.banner.won {
    background: rgba(106, 170, 100, 0.15);
}

.banner.lost {
    background: rgba(120, 124, 126, 0.15);
}

.msg {
    text-align: center;
    font-weight: 700;
    margin: 10px 0 0;
}
</style>
