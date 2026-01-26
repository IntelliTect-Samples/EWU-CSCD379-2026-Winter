import { useHint } from '../app/scripts/useHint';
import { ref, computed } from 'vue';
import { describe, it, expect, vi} from 'vitest';

vi.mock('../app/scripts/wordUtils', () => ({
  words: ['apple', 'audio', 'apply', 'beach', 'brain']
}));

describe('useHint Logic', () => {
  it('should suggest "audio" as the first hint if no guesses have been made', () => {
    const guesses = ref([]);
    const secretWord = ref('apple');
    const currentGuess = ref('');
    const gameOver = computed(() => false);

    const { useHint: triggerHint } = useHint(guesses, secretWord, currentGuess, gameOver);
    triggerHint();

    expect(currentGuess.value).toBe('audio');
  });

  it('should not provide a hint if the game is over', () => {
    const guesses = ref(['apple']);
    const secretWord = ref('apple');
    const currentGuess = ref('');
    const gameOver = computed(() => true);

    const { useHint: triggerHint } = useHint(guesses, secretWord, currentGuess, gameOver);
    triggerHint();

    expect(currentGuess.value).toBe('');
  });

  it('should narrow down the hint based on green letters', () => {
    const guesses = ref(['apply']);
    const secretWord = ref('apple');
    const currentGuess = ref('');
    const gameOver = computed(() => false);

    const { useHint: triggerHint } = useHint(guesses, secretWord, currentGuess, gameOver);
    triggerHint();
    expect(currentGuess.value).toBe('apple');
  });

  it('should exclude words containing gray letters', () => {
    const guesses = ref(['brain']);
    const secretWord = ref('beach');
    const currentGuess = ref('');
    const gameOver = computed(() => false);

    const { useHint: triggerHint } = useHint(guesses, secretWord, currentGuess, gameOver);
    triggerHint();
    expect(currentGuess.value).toBe('beach');
  });

  it('should fail when no matching word is found', () => {
    const { useHint: triggerHint } = useHint(
      ref(['apple']), 
      ref('xxxxx'),
      ref(''), 
      computed(() => false)
    );
    expect(() => triggerHint()).not.toThrow();
  });
});