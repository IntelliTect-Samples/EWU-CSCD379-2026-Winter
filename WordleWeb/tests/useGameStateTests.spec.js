import { useGameState } from '../app/scripts/useGameState';
import * as wordUtils from '../app/scripts/wordUtils';
import { describe, it, expect, vi, beforeEach } from 'vitest';


describe('useGameState Persistence', () => {
  
  beforeEach(() => {
    vi.stubGlobal('localStorage', {
      getItem: vi.fn(),
      setItem: vi.fn(),
    });

    vi.spyOn(wordUtils, 'getTodayDateString').mockReturnValue('2026-01-25');
  });

  it('should save the game state to localStorage', () => {
    const { saveGameState } = useGameState();
    saveGameState('APPLE', ['APPLY'], true);

    expect(localStorage.setItem).toHaveBeenCalledWith(
      'wordleGameState',
      expect.stringContaining('"secretWord":"APPLE"')
    );
  });

  it('should load daily word if date matches today', () => {
    const { loadGameState } = useGameState();
    
    const mockState = JSON.stringify({
      secretWord: 'APPLE',
      guesses: ['APPLY'],
      date: '2026-01-25',
      isDaily: true
    });
    localStorage.getItem.mockReturnValue(mockState);

    const result = loadGameState();
    expect(result).not.toBeNull();
    expect(result.secretWord).toBe('APPLE');
  });

  it('should not load daily word if the date is from yesterday', () => {
    const { loadGameState } = useGameState();
    
    const mockState = JSON.stringify({
      secretWord: 'PEARL',
      guesses: ['PEARS'],
      date: '2026-01-24',
      isDaily: true
    });
    localStorage.getItem.mockReturnValue(mockState);

    const result = loadGameState();
    expect(result).toBeNull();
  });
});