import { describe, it, expect } from 'vitest'

// Small grid builder that mirrors the logic in game.vue for display values
const toRoman = (num: number) => {
  const romanMap: Record<number, string> = {
    1: 'I',2: 'II',3: 'III',4: 'IV',5: 'V',6: 'VI',7: 'VII',8: 'VIII',9: 'IX',10: 'X',11: 'XI',12: 'XII',13: 'XIII',14: 'XIV',15: 'XV',16: 'XVI'
  }
  return romanMap[num]
}
const toBinary = (num: number) => num.toString(2).padStart(6, '0')

const buildGrid = (size: number, difficulty: 'Easy' | 'Medium' | 'Hard') => {
  return Array.from({ length: size }, (_, i) => {
    const value = i + 1
    let display: string | number = value
    if (difficulty === 'Medium') display = toRoman(value)
    if (difficulty === 'Hard') display = toBinary(value)
    return { id: value, number: value, display }
  })
}

describe('Grid builder', () => {
  it('builds Easy grid with numeric displays and correct length', () => {
    const g = buildGrid(9, 'Easy')
    expect(g).toHaveLength(9)
    expect(g[0].display).toBe(1)
    expect(g[8].display).toBe(9)
    const ids = g.map(x => x.id)
    expect(new Set(ids).size).toBe(9)
  })

  it('builds Medium grid with Roman displays', () => {
    const g = buildGrid(16, 'Medium')
    expect(g).toHaveLength(16)
    expect(g[0].display).toBe('I')
    expect(g[3].display).toBe('IV')
    expect(g[15].display).toBe('XVI')
  })

  it('builds Hard grid with binary displays (6-bit)', () => {
    const g = buildGrid(25, 'Hard')
    expect(g).toHaveLength(25)
    expect(g[0].display).toBe('000001')
    expect(g[9].display).toBe('001010')
    expect(g[24].display).toBe('011001')
  })
})
