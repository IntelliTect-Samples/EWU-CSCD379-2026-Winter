import { useStats } from '../app/scripts/useStats';
import { ref, computed } from 'vue';
import { describe, it, expect, vi, beforeEach } from 'vitest';

global.ref = ref;
global.computed = computed;

describe('useStats Logic', () => {
  beforeEach(() => {
    vi.stubGlobal('localStorage', {
      getItem: vi.fn(),
      setItem: vi.fn(),
    });
  });

  it('should calculate win ratio correctly', () => {
    const { stats, winRatio, recordGameEnd } = useStats();
    recordGameEnd(true, 3);
    recordGameEnd(true, 4);
    recordGameEnd(false, 6);
    expect(stats.value.totalGames).toBe(3);
    expect(stats.value.wins).toBe(2);
    expect(winRatio.value).toBe('67%');
  });

  it('should calculate average guesses only based on wins', () => {
    const { stats, averageGuesses, recordGameEnd } = useStats();
    recordGameEnd(true, 2);
    recordGameEnd(true, 4);
    expect(averageGuesses.value).toBe('3.00');
  });

  it('should return "-" for average guesses if no wins exist', () => {
    const { averageGuesses } = useStats();
    expect(averageGuesses.value).toBe('-');
  });

  it('should save to localStorage when recordGameEnd is called', () => {
    const { recordGameEnd } = useStats();
    recordGameEnd(true, 3);
    expect(localStorage.setItem).toHaveBeenCalledWith(
      'wordleStats',
      expect.stringContaining('"wins":1')
    );
  });

  it('should handle corrupted localStorage data', () => {
    const { stats, loadStats } = useStats();
    vi.mocked(localStorage.getItem).mockReturnValue("this is not json");
    loadStats();
    expect(stats.value.totalGames).toBe(0);
  });
});