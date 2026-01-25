import { getTodayDateString } from "./wordUtils";

export interface GameState {
  secretWord: string;
  guesses: string[];
  date: string;
  isDaily: boolean;
}

export const useGameState = () => {
  const saveGameState = (
    secretWord: string,
    guesses: string[],
    isDaily: boolean,
  ) => {
    if (typeof window === "undefined") return;
    const gameState: GameState = {
      secretWord,
      guesses,
      date: getTodayDateString(),
      isDaily,
    };
    localStorage.setItem("wordleGameState", JSON.stringify(gameState));
  };

  const loadGameState = (): GameState | null => {
    if (typeof window === "undefined") return null;
    const saved = localStorage.getItem("wordleGameState");
    if (!saved) return null;

    try {
      const gameState = JSON.parse(saved) as GameState;
      const today = getTodayDateString();

      // Only load if it's the same day
      if (gameState.date === today) {
        return gameState;
      }
    } catch {
      return null;
    }

    return null;
  };

  return {
    saveGameState,
    loadGameState,
  };
};
