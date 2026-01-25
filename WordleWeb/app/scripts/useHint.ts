import { words } from "./wordUtils";

export const useHint = (
  guesses: Ref<string[]>,
  secretWord: Ref<string>,
  currentGuess: Ref<string>,
  gameOver: ComputedRef<boolean>,
) => {
  const useHint = () => {
    // Don't provide hints if game is over
    if (gameOver.value) return;

    // First guess: use "audio" as a good starting word
    if (guesses.value.length === 0) {
      currentGuess.value = "audio";
      return;
    }

    // Analyze all previous guesses to determine constraints
    const correctLetters: (string | null)[] = [null, null, null, null, null];
    const requiredLetters: Set<string> = new Set();
    const excludedLetters: Set<string> = new Set();
    const wrongPositions: Map<number, Set<string>> = new Map();

    for (let i = 0; i < 5; i++) {
      wrongPositions.set(i, new Set());
    }

    for (const guess of guesses.value) {
      const target = secretWord.value;
      const targetArr = target.split("");
      const guessArr = guess.split("");
      const used = Array(5).fill(false);

      // Pass 1: Find correct (green) letters
      for (let i = 0; i < 5; i++) {
        if (guessArr[i] === targetArr[i]) {
          correctLetters[i] = guessArr[i] ?? null;
          requiredLetters.add(guessArr[i] ?? "");
          used[i] = true;
        }
      }

      // Pass 2: Find wrong position (yellow) and not found (gray) letters
      for (let i = 0; i < 5; i++) {
        if (guessArr[i] === targetArr[i]) continue;

        let foundElsewhere = false;
        for (let j = 0; j < 5; j++) {
          if (!used[j] && guessArr[i] === targetArr[j]) {
            foundElsewhere = true;
            used[j] = true;
            break;
          }
        }

        if (foundElsewhere) {
          requiredLetters.add(guessArr[i] ?? "");
          wrongPositions.get(i)?.add(guessArr[i] ?? "");
        } else {
          // Only exclude if letter is not already known to be in the word
          if (!requiredLetters.has(guessArr[i] ?? "")) {
            excludedLetters.add(guessArr[i] ?? "");
          }
        }
      }
    }

    // Find a word that matches all constraints
    const hintWord = words.find((word) => {
      // Skip words already guessed
      if (guesses.value.includes(word)) return false;

      // Check correct letters are in correct positions
      for (let i = 0; i < 5; i++) {
        if (correctLetters[i] && word[i] !== correctLetters[i]) {
          return false;
        }
      }

      // Check required letters are present
      for (const letter of requiredLetters) {
        if (!word.includes(letter)) return false;
      }

      // Check excluded letters are not present
      for (const letter of excludedLetters) {
        if (word.includes(letter)) return false;
      }

      // Check wrong position letters are not in those positions
      for (let i = 0; i < 5; i++) {
        const badLetters = wrongPositions.get(i);
        if (badLetters && badLetters.has(word[i] ?? "")) {
          return false;
        }
      }

      return true;
    });

    if (hintWord) {
      currentGuess.value = hintWord;
    }
  };

  return { useHint };
};
