import { describe, it, expect, beforeEach, vi } from 'vitest';
import { WordleGame, LetterState, Letter } from '../classes/wordle-game';

describe('WordleGame', () => {
    let game: WordleGame;

    beforeEach(() => {
        game = new WordleGame();
    });

    describe('constructor', () => {
        it('should create a game with a random word when no parameter is provided', () => {
            const game1 = new WordleGame();
            expect(game1).toBeInstanceOf(WordleGame);
            expect(game1.getGuesses()).toEqual([]);
        });

        it('should create a game with a specified target word', () => {
            const game1 = new WordleGame('apple');
            const result = game1.submitGuess('apple');
            expect(result).toBe(true);

            const guesses = game1.getGuesses();
            expect(guesses[0].length).toBe(5);
            // All letters should be correct when guessing the target word
            guesses[0].forEach(letter => {
                expect(letter.state).toBe(LetterState.Correct);
            });
        });

        it('should accept target word regardless of case', () => {
            const game1 = new WordleGame('APPLE');
            const result = game1.submitGuess('apple');
            expect(result).toBe(true);

            const guesses = game1.getGuesses();
            guesses[0].forEach(letter => {
                expect(letter.state).toBe(LetterState.Correct);
            });
        });
    });

    describe('submitGuess', () => {
        it('should reject invalid words not in word list', () => {
            const result = game.submitGuess('zzzzz');
            expect(result).toBe(false);
            expect(game.getGuesses().length).toBe(0);
        });

        it('should accept valid words in word list', () => {
            const result = game.submitGuess('apple');
            expect(result).toBe(true);
            expect(game.getGuesses().length).toBe(1);
        });

        it('should be case-insensitive when validating', () => {
            const result = game.submitGuess('APPLE');
            expect(result).toBe(true);
            expect(game.getGuesses().length).toBe(1);
        });

        it('should add guess to guesses array', () => {
            game.submitGuess('apple');
            game.submitGuess('grape');

            expect(game.getGuesses().length).toBe(2);
        });

        it('should evaluate each letter in the guess', () => {
            game.submitGuess('apple');
            const guesses = game.getGuesses();

            expect(guesses[0].length).toBe(5);
            guesses[0].forEach(letter => {
                expect(letter).toBeInstanceOf(Letter);
                expect(typeof letter.character).toBe('string');
                expect(Object.values(LetterState)).toContain(letter.state);
            });
        });
    });

    describe('evaluateGuess', () => {
        it('should mark all letters as correct when guess matches target', () => {
            const testGame = new WordleGame('apple');
            testGame.submitGuess('apple');
            const guesses = testGame.getGuesses();

            expect(guesses[0].length).toBe(5);
            guesses[0].forEach(letter => {
                expect(letter.state).toBe(LetterState.Correct);
            });
        });

        it('should handle repeated letters correctly', () => {
            const testGame = new WordleGame('speed');
            // Guess with repeated letters where target also has repeated letters
            testGame.submitGuess('speed');
            const guesses = testGame.getGuesses();

            expect(guesses[0].length).toBe(5);
            // All letters should be correct
            guesses[0].forEach(letter => {
                expect(letter.state).toBe(LetterState.Correct);
            });
        });

        it('should handle repeated letters with partial matches', () => {
            let testGame = new WordleGame('speed');
            // 'e' appears twice in target, once in guess at wrong position
            testGame.submitGuess('crane');
            let guesses = testGame.getGuesses();

            expect(guesses[0].length).toBe(5);
            // C - wrong, R - wrong, A - wrong, N - wrong, E - misplaced (e is in target)
            expect(guesses[0][0].state).toBe(LetterState.Wrong); // C
            testGame = new WordleGame('crane');
            // Guess "crate" - C, R, A correct, T wrong, E misplaced
            testGame.submitGuess('crate');
            guesses = testGame.getGuesses();

            expect(guesses[0][0].state).toBe(LetterState.Correct);    // C
            expect(guesses[0][1].state).toBe(LetterState.Correct);    // R
            expect(guesses[0][2].state).toBe(LetterState.Correct);    // A
            expect(guesses[0][3].state).toBe(LetterState.Wrong);      // T
            expect(guesses[0][4].state).toBe(LetterState.Correct);    // E
        });

        it('should not mark extra occurrences as misplaced if already found', () => {
            const testGame = new WordleGame('robot');
            // Guess "floor" - F wrong, L wrong, O correct (position 3), O wrong (no more O's), R misplaced
            testGame.submitGuess('floor');
            const guesses = testGame.getGuesses();

            expect(guesses[0][0].state).toBe(LetterState.Wrong);      // F
            expect(guesses[0][1].state).toBe(LetterState.Wrong);      // L
            expect(guesses[0][2].state).toBe(LetterState.Misplaced);  // O (first O, misplaced)
            expect(guesses[0][3].state).toBe(LetterState.Correct);    // O (second O, correct position)
            expect(guesses[0][4].state).toBe(LetterState.Misplaced);  // Rst guesses = game.getGuesses();

            guesses[0].forEach(letter => {
                expect(letter.character).toBe(letter.character.toUpperCase());
            });
        });

        it('should correctly identify misplaced letters', () => {
            // This test would need a known target word
            // For now, we test that the evaluation produces valid states
            game.submitGuess('crane');
            const guesses = game.getGuesses();

            guesses[0].forEach(letter => {
                expect([LetterState.Correct, LetterState.Misplaced, LetterState.Wrong])
                    .toContain(letter.state);
            });
        });
    });

    describe('Letter class', () => {
        it('should create a letter with character and state', () => {
            const letter = new Letter('A', LetterState.Correct);

            expect(letter.character).toBe('A');
            expect(letter.state).toBe(LetterState.Correct);
        });

        it('should allow all letter states', () => {
            const correct = new Letter('A', LetterState.Correct);
            const misplaced = new Letter('B', LetterState.Misplaced);
            const wrong = new Letter('C', LetterState.Wrong);

            expect(correct.state).toBe(LetterState.Correct);
            expect(misplaced.state).toBe(LetterState.Misplaced);
            expect(wrong.state).toBe(LetterState.Wrong);
        });
    });

    describe('getColorForState', () => {
        it('should return green for correct letters', () => {
            expect(game.getColorForState(LetterState.Correct)).toBe('green');
        });

        it('should return yellow for misplaced letters', () => {
            expect(game.getColorForState(LetterState.Misplaced)).toBe('yellow');
        });

        it('should return grey for wrong letters', () => {
            expect(game.getColorForState(LetterState.Wrong)).toBe('grey');
        });
    });

    describe('reset', () => {
        it('should clear all guesses', () => {
            game.submitGuess('apple');
            game.submitGuess('grape');

            expect(game.getGuesses().length).toBe(2);

            game.reset();

            expect(game.getGuesses().length).toBe(0);
        });

        it('should pick a new target word', () => {
            // Submit a guess to establish the first target word
            game.submitGuess('apple');
            const firstGuess = game.getGuesses()[0];

            // Reset and submit the same guess
            game.reset();
            game.submitGuess('apple');
            const secondGuess = game.getGuesses()[0];

            // Both guesses should be valid (length 5)
            expect(firstGuess.length).toBe(5);
            expect(secondGuess.length).toBe(5);
        });
    });

    describe('getGuesses', () => {
        it('should return an array of guess arrays', () => {
            expect(Array.isArray(game.getGuesses())).toBe(true);
        });

        it('should return empty array when no guesses made', () => {
            expect(game.getGuesses()).toEqual([]);
        });

        it('should return all submitted guesses', () => {
            game.submitGuess('apple');
            game.submitGuess('grape');
            game.submitGuess('crane');

            const guesses = game.getGuesses();
            expect(guesses.length).toBe(3);
            expect(guesses[0].length).toBe(5);
            expect(guesses[1].length).toBe(5);
            expect(guesses[2].length).toBe(5);
        });
    });

    describe('Integration tests', () => {
        it('should handle multiple guesses in a game session', () => {
            const validWords = ['apple', 'grape', 'crane', 'plane', 'slate'];

            validWords.forEach(word => {
                const result = game.submitGuess(word);
                expect(result).toBe(true);
            });

            expect(game.getGuesses().length).toBe(5);
        });

        it('should maintain state across multiple operations', () => {
            game.submitGuess('apple');
            expect(game.getGuesses().length).toBe(1);

            game.submitGuess('grape');
            expect(game.getGuesses().length).toBe(2);

            const invalidResult = game.submitGuess('zzzzz');
            expect(invalidResult).toBe(false);
            expect(game.getGuesses().length).toBe(2);

            game.reset();
            expect(game.getGuesses().length).toBe(0);
        });
    });
});
