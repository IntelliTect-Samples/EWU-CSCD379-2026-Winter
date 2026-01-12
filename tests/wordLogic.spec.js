import { mountSuspended } from '@nuxt/test-utils/runtime'
import { expect, test, vi, afterEach } from 'vitest'
import { createRouter, createMemoryHistory } from 'vue-router'
import { mockNuxtImport } from '@nuxt/test-utils/runtime'
import App from '~/app.vue'
import { nextTick } from 'vue'

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
  expect(wrapper.vm.snackbarMsg).toContain('The word was OCEAN')
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
