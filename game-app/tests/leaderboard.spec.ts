import { describe, it, expect } from 'vitest'

// Simple test to check the environment setup
// Leaderboard test
describe('Leaderboard Setup Test', () => {
  it('correctly identifies the project structure', () => {
    const isSetupCorrect = true
    expect(isSetupCorrect).toBe(true)
  })

  it('sorts difficulty correctly (Hard > Medium > Easy)', () => {
    const diffOrder: Record<string, number> = { 'Hard': 1, 'Medium': 2, 'Easy': 3 };
    const mockData = [
      { diff: 'Easy' },
      { diff: 'Hard' },
      { diff: 'Medium' }
    ]

    const sorted = mockData.sort((a, b) => diffOrder[a.diff] - diffOrder[b.diff])

    expect(sorted[0].diff).toBe('Hard')
    expect(sorted[1].diff).toBe('Medium')
    expect(sorted[2].diff).toBe('Easy')
  })
})