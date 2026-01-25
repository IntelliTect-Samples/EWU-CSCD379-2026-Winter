export type TileState = 'empty' | 'correct' | 'present' | 'absent'

export function scoreGuess(guess: string, answer: string): TileState[] {
    const g = guess.toLowerCase().slice(0, 5) // guess
    const a = answer.toLowerCase().slice(0, 5) // answer

    const result: TileState[] = Array(5).fill('absent')
    const answerChars = a.split('')
    // First pass
    for (let i = 0; i < 5; i++) {
        if (g.charAt(i) === answerChars[i]) {
            result[i] = 'correct'
            answerChars[i] = '_'
    }
}
    // Second pass
    for (let i = 0; i < 5; i++) {
        if (result[i] === 'correct') continue
        const ch = g.charAt(i)
        const idx = answerChars.indexOf(ch)
        if (idx !== -1) {
            result[i] = 'present'
            answerChars[idx] = '_'
        }
    }
    return result
}

const priority = {
    empty: 0,
    absent: 1,
    present: 2,
    correct: 3,
}

export function bestState(prev: TileState, next: TileState): TileState {
    return priority[next] > priority[prev] ? next : prev
}