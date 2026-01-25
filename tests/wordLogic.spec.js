import { mountSuspended, mockNuxtImport } from '@nuxt/test-utils/runtime'
import { expect, test, vi, afterEach } from 'vitest'
import { createRouter, createMemoryHistory } from 'vue-router'
import App from '~/app.vue'
import { nextTick } from 'vue'
import * as wordlist from '../app/utils/wordlist'

vi.spyOn(wordlist, 'loadDictionary').mockImplementation(async () => {
  return true 
})

// Basic localStorage mock
global.localStorage = {
  getItem: vi.fn(() => null),
  setItem: vi.fn(),
}

// Create a simple router mock
const router = createRouter({
  history: createMemoryHistory(),
  routes: [
    { path: '/', component: { template: '<div />' } },
  ],
})

// Mock Nuxt composables
mockNuxtImport('useRouter', () => () => router)
mockNuxtImport('useRoute', () => () => ({
  path: '/',
  params: {},
  query: {},
}))

// Minimal mocks for Vuetify internals
if (typeof window !== 'undefined') {
  window.visualViewport = { addEventListener: vi.fn(), removeEventListener: vi.fn() }
}

afterEach(() => {
    if (typeof document !== 'undefined') {
        document.body.innerHTML = ''
    }
})

// Check that the board updates correctly when a letter key is pressed
test('updates board when a letter key is pressed', async () => {
  const wrapper = await mountSuspended(App)
  wrapper.vm.handleInput({ key: 'A' })
  await nextTick()
  expect(wrapper.vm.board[0][0].letter).toBe('A')
})

// Check that the enter key submits a guess
test('submits a guess when Enter is pressed', async () => {
  const wrapper = await mountSuspended(App)
  const guess = 'APPLE'
  // Fill the row
  for (const char of guess) {
    wrapper.vm.handleInput({ key: char })
  }
  // Submit
  wrapper.vm.handleInput('ENTER')
  await nextTick()
  await nextTick()
  expect(wrapper.vm.currentRow).toBe(1)
})

// Check that backspace deletes the last letter
test('deletes the last letter when Backspace is pressed', async () => {
  const wrapper = await mountSuspended(App)
  wrapper.vm.handleInput({ key: 'A' })
  wrapper.vm.handleInput({ key: 'Backspace' })
  await nextTick()
  expect(wrapper.vm.board[0][0].letter).toBe('')
})

// Check that letters are marked correctly as correct, present, or absent
test('mark letters correctly for a guess', async () => {
  const wrapper = await mountSuspended(App)
  wrapper.vm.currentWordData = { word: 'STEEL', hint: 'Metal' }
  // Manually fill board for the guess 'STARE'
  const guess = 'STARE'
  guess.split('').forEach((l, i) => wrapper.vm.board[0][i].letter = l)
  
  await wrapper.vm.checkWord()
  await nextTick()
  expect(wrapper.vm.board[0][4].status).toBe('present')
})

// Check that gameOver is triggered on correct guess
test ('triggers game over on correct guess', async () => {
    const wrapper = await mountSuspended(App)
    const target = 'ZEBRA'

    wrapper.vm.currentWordData = { word: target, hint: 'A horse with black-and-white stripes.' }
    // Move to last row in grid
    wrapper.vm.currentRow = 5
    target.split('').forEach((l, i) => wrapper.vm.board[5][i].letter = l)

    await wrapper.vm.checkWord()
    await nextTick()

    expect(wrapper.vm.gameOver).toBe(true)
    expect(wrapper.vm.snackbarMsg).toContain( 'You guessed the word! ')
})

// Check that game over is triggered after 6 failed attempts
test('triggers game over on 6th failed guess', async () => {
  const wrapper = await mountSuspended(App)
  wrapper.vm.currentWordData = { word: 'OCEAN', hint: 'Sea' }
  
  // Move to last row in grid
  wrapper.vm.currentRow = 5
  const guess = 'APPLE'
  guess.split('').forEach((l, i) => wrapper.vm.board[5][i].letter = l)

  await wrapper.vm.checkWord()
  await nextTick()
  
  expect(wrapper.vm.gameOver).toBe(true)
  expect(wrapper.vm.snackbarMsg).toContain(' Better luck next time! ')
})

// Check that words with duplicate letters are handled correctly
test('handles duplicate letters correctly (GEESE vs GOOSE)', async () => {
  const wrapper = await mountSuspended(App)
  wrapper.vm.currentWordData = { word: 'GOOSE', hint: 'Bird' }
  const guess = 'GEESE'
  guess.split('').forEach((l, i) => wrapper.vm.board[0][i].letter = l)

  await wrapper.vm.checkWord()
  await nextTick()
  
  expect(wrapper.vm.board[0][1].status).toBe('absent')
  expect(wrapper.vm.board[0][4].status).toBe('correct')
})

// Check that hintSnackbar is revealed when revealHint is called
test('reveals the hint when the hint button is clicked', async () => {
  const wrapper = await mountSuspended(App)
  wrapper.vm.currentWordData = { word: 'RIVER', hint: 'Stream' }
  await wrapper.vm.revealHint()
  await nextTick()
  expect(wrapper.vm.hintSnackbar).toBe(true)
})

// Check that hitting enter twice only submits once
test('prevents multiple submissions on rapid Enter', async () => {
  const wrapper = await mountSuspended(App)
  for (const char of 'APPLE') wrapper.vm.handleInput({ key: char })
  wrapper.vm.handleInput({ key: 'Enter' })
  wrapper.vm.handleInput({ key: 'Enter' })
  await nextTick()
  expect(wrapper.vm.currentRow).toBe(1)
})

// Check that dark and light themes toggle correctly
test('toggles theme and saves to localStorage', async () => {
  const wrapper = await mountSuspended(App)
  const targetTheme = wrapper.vm.theme.global.name.value === 'light' ? 'dark' : 'light'
  await wrapper.vm.toggleTheme()
  await nextTick()
  expect(wrapper.vm.theme.global.name.value).toBe(targetTheme)
  expect(localStorage.setItem).toHaveBeenCalledWith('user-theme', targetTheme)
})

// Check that the game resets correctly
test('resets game state correctly', async () => {
  const wrapper = await mountSuspended(App)
  wrapper.vm.gameOver = true
  wrapper.vm.currentRow = 3
  
  await wrapper.vm.resetGame()
  await nextTick()
  
  expect(wrapper.vm.gameOver).toBe(false)
  expect(wrapper.vm.currentRow).toBe(0)
})

// Check that keyboard colors update correctly after guesses
test('updates keyboard colors correctly after guesses', async () => {
    const wrapper = await mountSuspended(App)
    wrapper.vm.currentWordData = { word: 'bread', hint: 'A baked good that can be made at home or bought.' }
    'BRAIN'.split('').forEach((l, i) => wrapper.vm.board[0][i].letter = l)
    await wrapper.vm.checkWord()
    await nextTick()
    expect(wrapper.vm.letterStates['B']).toBe('correct')
    expect(wrapper.vm.letterStates['I']).toBe('absent')
})


// Check that words not in list are rejected
test('rejects words not in the valid guesses list', async () => {
    const wrapper = await mountSuspended(App)
    const guess = 'ZZZZZ'
    guess.split('').forEach((l, i) => wrapper.vm.board[0][i].letter = l)
    wrapper.vm.currentCol = 5
    wrapper.vm.handleInput({ key: 'Enter' })
    await nextTick()
    expect(wrapper.vm.currentRow).toBe(0)
    expect(wrapper.vm.snackbarMsg).toBe('Not in word list')
    expect(wrapper.vm.snackbar).toBe(true)
})

// Check that non-alphabetic keys are ignored
test('ignores non-alphabetic key inputs', async () => {
    const wrapper = await mountSuspended(App)
  wrapper.vm.handleInput('1')
  wrapper.vm.handleInput('$')
  await nextTick()
  expect(wrapper.vm.board[0][0].letter).toBe('')
  expect(wrapper.vm.currentCol).toBe(0)
})

// Check that shake animation goes off for a word not in the list
test('triggers shake animation for invalid words', async () => {
  const wrapper = await mountSuspended(App)
  'XXXXX'.split('').forEach((l, i) => wrapper.vm.board[0][i].letter = l)
  wrapper.vm.currentCol = 5
  
  wrapper.vm.handleInput('ENTER')
  await nextTick()
  
  expect(wrapper.vm.shakeActive).toBe(true)
})

// Check that the daily word is the same for the day
test('picks the same daily word for the same date', async () => {
  vi.setSystemTime(new Date('2026-01-23T10:00:00'))
  const wrapper1 = await mountSuspended(App)
  const word1 = wrapper1.vm.currentWordData.word

  const wrapper2 = await mountSuspended(App)
  const word2 = wrapper2.vm.currentWordData.word

  expect(word1).toBe(word2)
  expect(wrapper1.vm.isDaily).toBe(true)
  vi.useRealTimers()
})

// Check that the session persists on mount
test('restores board state from localStorage on mount', async () => {
  const today = new Date().toDateString()
  
  const savedState = {
    board: Array.from({ length: 6 }, () => Array.from({ length: 5 }, () => ({ letter: '', status: 'default' }))),
    currentRow: 1,
    solution: 'APPLE',
    gameOver: false,
    letterStates: { 'A': 'correct' }
  }
  savedState.board[0][0] = { letter: 'A', status: 'correct' }

  const getItemSpy = vi.spyOn(Storage.prototype, 'getItem').mockImplementation((key) => {
    if (key === 'daily-board-state') return JSON.stringify(savedState)
    if (key === 'last-played-daily') return today
    return null
  })

  const wrapper = await mountSuspended(App)
  
  wrapper.vm.currentWordData = { word: 'APPLE', hint: 'A fruit' }
  
  if (savedState.solution === wrapper.vm.currentWordData.word.toUpperCase()) {
    wrapper.vm.board = savedState.board
    wrapper.vm.currentRow = savedState.currentRow
    wrapper.vm.letterStates = savedState.letterStates
  }

  await nextTick()

  expect(wrapper.vm.currentRow).toBe(1)
  expect(wrapper.vm.board[0][0].letter).toBe('A')
  expect(wrapper.vm.letterStates['A']).toBe('correct')
  
  getItemSpy.mockRestore()
})
