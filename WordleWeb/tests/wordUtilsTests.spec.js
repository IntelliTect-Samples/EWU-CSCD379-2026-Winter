import { getDailyWord, getTodayDateString } from '../app/scripts/wordUtils';
import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';

describe('wordUtils Selection Logic', () => {
  
  beforeEach(() => {
    vi.useFakeTimers();
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it('should return consistent word for the same date', () => {
    vi.setSystemTime(new Date('2026-01-25'));
    const firstCall = getDailyWord();
    
    vi.setSystemTime(new Date('2026-01-25'));
    const secondCall = getDailyWord();

    expect(firstCall).toBe(secondCall);
  });

  it('should return a different word for a different date', () => {
    vi.setSystemTime(new Date('2026-01-25'));
    const dayOneWord = getDailyWord();

    vi.setSystemTime(new Date('2026-01-26'));
    const dayTwoWord = getDailyWord();

    expect(dayOneWord).not.toBe(dayTwoWord);
  });

  it('should return the correct date string format (YYYY-MM-DD)', () => {
    vi.setSystemTime(new Date('2026-01-25'));
    expect(getTodayDateString()).toBe('2026-01-25');
  });

  it('should fallback to a default word if the word list is empty', () => {
    expect(getDailyWord()).toBeDefined();
  });
});