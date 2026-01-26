import {useLetterClasses} from '../app/scripts/useLetterClasses';
import { ref } from 'vue';
import { describe, it, expect } from 'vitest';

describe('useLetterClasses Logic', () => {
  it('should return correct for a matching letter on the board', () => {
    const guesses = ref(['apple']);
    const secretWord = ref('apple');
    const { getLetterClass } = useLetterClasses(guesses, secretWord);
    expect(getLetterClass(0, 0)).toBe('correct');
  });

  it('should return wrong-position for letters in wrong place on board', () => {
    const guesses = ref(['pears']);
    const secretWord = ref('apple');
    const { getLetterClass } = useLetterClasses(guesses, secretWord);
    expect(getLetterClass(0, 0)).toBe('wrong-position');
  });

  it('should prioritize green over yellow for keyboard class', () => {
    const guesses = ref(['paper', 'apple']);
    const secretWord = ref('apple');
    const { getKeyboardClass } = useLetterClasses(guesses, secretWord);
    expect(getKeyboardClass('p')).toBe('key-correct');
  });

  it('should handle duplicate letters correctly (goose vs. geese)', () => {
    const guesses = ref(['goose']);
    const secretWord = ref('geese');
    const { getLetterClass } = useLetterClasses(guesses, secretWord);
    expect(getLetterClass(0, 0)).toBe('correct');
    expect(getLetterClass(0, 1)).toBe('not-found');
    expect(getLetterClass(0, 2)).toBe('not-found');
    expect(getLetterClass(0, 3)).toBe('correct');
    expect(getLetterClass(0, 4)).toBe('correct');
  });
});