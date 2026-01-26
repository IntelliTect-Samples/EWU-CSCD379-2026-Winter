import { computed, ref } from 'vue'

type Stats = {
    wins: number
    losses: number
    winAttemptsTotal: number
    winCount: number
}

const STORAGE_KEY = 'wordle.stats.v1'

function loadStats(): Stats {
    if (import.meta.server) {
    return { wins: 0, losses: 0, winAttemptsTotal: 0, winCount: 0 }
    }
    try {
        const raw = localStorage.getItem(STORAGE_KEY)
        if (!raw) return { wins: 0, losses: 0, winAttemptsTotal: 0, winCount: 0 }
        const parsed = JSON.parse(raw) as Partial<Stats>
    return {
        wins: parsed.wins ?? 0,
        losses: parsed.losses ?? 0,
        winAttemptsTotal: parsed.winAttemptsTotal ?? 0,
        winCount: parsed.winCount ?? 0,
    }
    } catch {
    return { wins: 0, losses: 0, winAttemptsTotal: 0, winCount: 0 }
    }
}

function saveStats(s: Stats) {
    if (import.meta.server) return
    localStorage.setItem(STORAGE_KEY, JSON.stringify(s))
}

export function useStats() {
    const stats = ref<Stats>(loadStats())

    const avgAttempts = computed(() => {
    if (stats.value.winCount === 0) return 0
    return stats.value.winAttemptsTotal / stats.value.winCount
    })

    function recordWin(attemptsUsed: number) {
    stats.value.wins += 1
    stats.value.winCount += 1
    stats.value.winAttemptsTotal += attemptsUsed
    saveStats(stats.value)
    }

    function recordLoss() {
    stats.value.losses += 1
    saveStats(stats.value)
    }

    function resetStats() {
    stats.value = { wins: 0, losses: 0, winAttemptsTotal: 0, winCount: 0 }
    saveStats(stats.value)
    }

    return {
    stats,
    avgAttempts,
    recordWin,
    recordLoss,
    resetStats,
    }
}
