export interface Stats {
  totalGames: number;
  wins: number;
  losses: number;
  totalGuesses: number;
}

export const useStats = () => {
  const stats = ref<Stats>({
    totalGames: 0,
    wins: 0,
    losses: 0,
    totalGuesses: 0,
  });

  const winRatio = computed(() => {
    if (stats.value.totalGames === 0) return "0";
    return Math.round((stats.value.wins / stats.value.totalGames) * 100) + "%";
  });

  const averageGuesses = computed(() => {
    if (stats.value.wins === 0) return "-";
    return (stats.value.totalGuesses / stats.value.wins).toFixed(2);
  });

  const saveStats = () => {
    if (typeof window === "undefined") return;
    localStorage.setItem("wordleStats", JSON.stringify(stats.value));
  };

  const loadStats = () => {
    if (typeof window === "undefined") return;
    const saved = localStorage.getItem("wordleStats");
    if (!saved) {
      stats.value = {
        totalGames: 0,
        wins: 0,
        losses: 0,
        totalGuesses: 0,
      };
      return;
    }

    try {
      stats.value = JSON.parse(saved);
    } catch {
      stats.value = {
        totalGames: 0,
        wins: 0,
        losses: 0,
        totalGuesses: 0,
      };
    }
  };

  const recordGameEnd = (won: boolean, guessCount: number) => {
    stats.value.totalGames++;
    if (won) {
      stats.value.wins++;
      stats.value.totalGuesses += guessCount;
    } else {
      stats.value.losses++;
    }
    saveStats();
  };

  return {
    stats,
    winRatio,
    averageGuesses,
    loadStats,
    saveStats,
    recordGameEnd,
  };
};
