import { words } from "./words";

export enum LetterState {
    Correct,
    Misplaced,
    Wrong,
}

export class Letter {
    constructor(
        public character: string,
        public state: LetterState
    ) { }
}

export class WordleGame {
    private targetWord: string = "";
    private guesses: Array<Array<Letter>> = [];

    constructor(word: string | undefined = undefined) {
        if (word === undefined) {
            this.pickRandomTargetWord();
        } else {
            this.targetWord = word;
        }
    }

    private pickRandomTargetWord(): void {
        this.targetWord = words[Math.floor(Math.random() * words.length)]!;
    }

    public getGuesses(): Array<Array<Letter>> {
        return this.guesses;
    }

    public submitGuess(guess: string): boolean {
        // Validate the guess is in the word list
        if (words.indexOf(guess.toLowerCase()) < 0) {
            return false;
        }

        const letters = this.evaluateGuess(guess);
        this.guesses.push(letters);
        return true;
    }

    private evaluateGuess(guess: string): Array<Letter> {
        const letters: Array<Letter> = [];
        const targetLetters = this.targetWord.toUpperCase().split("");
        const guessLetters = guess.toUpperCase().split("");

        // First pass: mark correct letters
        for (const [index, value] of guessLetters.entries()) {
            let isCorrect = false;
            if (targetLetters[index] === value) {
                isCorrect = true;
                guessLetters[index] = " ";
                targetLetters[index] = " ";
            }
            const letter = new Letter(value, isCorrect ? LetterState.Correct : LetterState.Wrong);
            letters.push(letter);
        }

        // Second pass: mark misplaced letters
        for (const [letterIndex, letter] of letters.entries()) {
            if (letter.state !== LetterState.Correct) {
                for (const [targetIndex, targetValue] of targetLetters.entries()) {
                    if (letter.character === targetValue) {
                        letter.state = LetterState.Misplaced;
                        targetLetters[targetIndex] = " ";
                        break;
                    }
                }
            }
        }

        return letters;
    }

    public getColorForState(state: LetterState): string {
        switch (state) {
            case LetterState.Correct:
                return "green";
            case LetterState.Misplaced:
                return "yellow";
            default:
                return "grey";
        }
    }

    public reset(): void {
        this.guesses = [];
        this.pickRandomTargetWord();
    }
}
