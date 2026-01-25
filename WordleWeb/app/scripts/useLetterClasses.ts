export const useLetterClasses = (
  guesses: Ref<string[]>,
  secretWord: Ref<string>,
) => {
  const getKeyboardClass = (letter: string) => {
    const lowerLetter = letter.toLowerCase();
    let hasAppeared = false;
    let isCorrect = false;
    let isWrongPosition = false;

    for (const guess of guesses.value) {
      for (let i = 0; i < guess.length; i++) {
        if (guess[i] === lowerLetter) {
          hasAppeared = true;
          if (guess[i] === secretWord.value[i]) {
            isCorrect = true;
          } else if (secretWord.value.includes(lowerLetter)) {
            isWrongPosition = true;
          }
        }
      }
    }

    if (!hasAppeared) return "";
    if (isCorrect) return "key-correct";
    if (isWrongPosition) return "key-wrong-position";
    return "key-not-found";
  };

  const getLetterClass = (guessIdx: number, letterIdx: number) => {
    const guess = guesses.value[guessIdx];
    const target = secretWord.value;
    const letter = guess?.[letterIdx];
    if (!letter) return "not-found";

    // First, mark all correct (green) letters
    const targetArr = target.split("");
    const guessArr = guess.split("");
    const marks: ("correct" | "wrong-position" | "not-found")[] = Array(
      guessArr.length,
    ).fill("not-found");
    const used = Array(targetArr.length).fill(false);

    // Pass 1: mark correct
    for (let i = 0; i < guessArr.length; i++) {
      if (guessArr[i] === targetArr[i]) {
        marks[i] = "correct";
        used[i] = true;
      }
    }

    // Pass 2: mark wrong-position
    for (let i = 0; i < guessArr.length; i++) {
      if (marks[i] === "correct") continue;
      for (let j = 0; j < targetArr.length; j++) {
        if (!used[j] && guessArr[i] === targetArr[j]) {
          marks[i] = "wrong-position";
          used[j] = true;
          break;
        }
      }
    }

    return marks[letterIdx] ?? "not-found";
  };

  return {
    getKeyboardClass,
    getLetterClass,
  };
};
