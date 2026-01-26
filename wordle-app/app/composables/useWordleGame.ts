import { ref } from 'vue'
import { scoreGuess, bestState } from '~/utils/wordle'
import type { TileState } from '~/utils/wordle'
import { commonWords } from '~/data/common-words'
import { allWords } from '~/data/all-words'

type GameStatus = 'playing' | 'won' | 'lost'

function randomWord(words: string[]): string {
    if (words.length === 0) {
        throw new Error('Word list is empty')
    }
    return words[Math.floor(Math.random() * words.length)]!
}

export function useWordleGame() {
    const DAILY_PLAYED_KEY = 'wordle.daily-played.v1'

    function todayLocalKey(): string {
        const d = new Date()
        const yyyy = d.getFullYear()
        const mm = String(d.getMonth() + 1).padStart(2, '0')
        const dd = String(d.getDate()).padStart(2, '0')
        return `${yyyy}-${mm}-${dd}`
    }

    function hashString(s: string): number {
        let h = 5381
        for (let i = 0; i < s.length; i++) {
            h = ((h << 5) + h) + s.charCodeAt(i)
            h |= 0
        }
        return Math.abs(h)
    }

    function dailyWordFor(dateKey: string, words: string[]): string {
        if (words.length === 0) throw new Error('Word list is empty')
        const idx = hashString(dateKey) % words.length
        return words[idx]!
    }

    function isClient(): boolean {
        return typeof window !== 'undefined'
    }

    function chooseInitialAnswer(): string {
        const today = todayLocalKey()
        const daily = dailyWordFor(today, commonWords)

        if (!isClient()) {
            return daily
        }

        const played = localStorage.getItem(DAILY_PLAYED_KEY)

        if (played !== today) {
            localStorage.setItem(DAILY_PLAYED_KEY, today)
            return daily
        }

        let next = randomWord(commonWords)
        if (commonWords.length > 1) {
            while (next === daily) {
            next = randomWord(commonWords)
        }
    }
        return next
    }

    const answer = ref<string>(chooseInitialAnswer())

    const message = ref('')
    const shakeRow = ref<number | null>(null)
    const guesses = ref<string[]>(Array(6).fill(''))
    const states = ref<TileState[][]>(Array(6).fill([]))
    const row = ref(0)
    const col = ref(0)
    const status = ref<GameStatus>('playing')
    const keyboard = ref<Record<string, TileState>>({})

    const hint = ref('')
    const usedHintLetters = ref<Set<string>>(new Set())

    function giveHintNotInWord() {
        if (status.value !== 'playing') return

        const alphabet = 'abcdefghijklmnopqrstuvwxyz'
        const answerLetters = new Set(answer.value.split(''))
        const guessedLetters = new Set(
        guesses.value.join('').split('').filter(Boolean)
        )

        const candidates = alphabet
        .split('')
        .filter(
        ch =>
            !answerLetters.has(ch) &&
            !guessedLetters.has(ch) &&
            !usedHintLetters.value.has(ch)
        )

        if (candidates.length === 0) {
            hint.value = 'No more hints available.'
            return
        }

        const letter = candidates[Math.floor(Math.random() * candidates.length)]!
        usedHintLetters.value.add(letter)
        hint.value = `Hint: The letter "${letter.toUpperCase()}" is NOT in the word.`
    }

    function newGame() {
        const today = todayLocalKey()
        const daily = dailyWordFor(today, commonWords)

        let next = randomWord(commonWords)
        if (commonWords.length > 1) {
            while (next === daily || next === answer.value) {
            next = randomWord(commonWords)
        }
    }

    answer.value = next
    guesses.value = Array(6).fill('')
    states.value = Array(6).fill([])
    row.value = 0
    col.value = 0
    status.value = 'playing'
    keyboard.value = {}
    message.value = ''
    shakeRow.value = null

    hint.value = ''
    usedHintLetters.value.clear()
    }

    function triggerInvalid(msg: string) {
        message.value = msg
        shakeRow.value = row.value

        setTimeout(() => { shakeRow.value = null }, 350)
        setTimeout(() => { message.value = '' }, 1200)
    }

    function input(letter: string) {
        if (status.value !== 'playing') return
        if (row.value >= 6) return
        if (col.value >= 5) return

        guesses.value[row.value] = (guesses.value[row.value] ?? '') + letter.toLowerCase()
        col.value++
    }

    function backspace() {
        if (status.value !== 'playing') return
        if (row.value >= 6) return
        if (col.value <= 0) return

        const current = guesses.value[row.value] ?? ''
        guesses.value[row.value] = current.slice(0, -1)
        col.value--
    }

    function submit() {
        if (status.value !== 'playing') return
        if (row.value >= 6) return

        const guess = (guesses.value[row.value] ?? '').toLowerCase()
        if (guess.length !== 5) return
        if (!allWords.includes(guess)) {
        triggerInvalid('Not in word list')
        return
    }

        const result = scoreGuess(guess, answer.value)
        states.value[row.value] = result

        for (let i = 0; i < 5; i++) {
            const letter = guess.charAt(i)
            keyboard.value[letter] = bestState(keyboard.value[letter] || 'empty', result[i]!)
        }

        const won = result.every(s => s === 'correct')
        if (won) {
            status.value = 'won'
            return
        }

        row.value++
        col.value = 0

        if (row.value >= 6) {
            status.value = 'lost'
        }
    }

        return {
        answer,
        guesses,
        states,
        keyboard,
        row,
        col,
        status,
        input,
        backspace,
        submit,
        enter: submit,
        message,
        shakeRow,
        newGame,

        hint,
        giveHintNotInWord,
        }
}
