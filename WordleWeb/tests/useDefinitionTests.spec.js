import { useDefinition } from '../app/scripts/useDefinition';
import { describe, it, expect, vi, beforeEach } from 'vitest';

describe('useDefinition Composable', () => {
    beforeEach(() => {
        vi.stubGlobal('fetch', vi.fn());
    });

    it('should start in default state', () => {
        const { definition, loading, error } = useDefinition();
        expect(definition.value).toBe(null);
        expect(loading.value).toBe(false);
        expect(error.value).toBe(null);
    });

    it('should return a definition successfully', async () => {
    const { definition, loading, fetchDefinition } = useDefinition();
    
    const mockResponse = [{
      meanings: [{
        definitions: [{ definition: 'A fruit that grows on trees' }]
      }]
    }];

    fetch.mockResolvedValue({
      ok: true,
      json: () => Promise.resolve(mockResponse),
    });

    const fetchPromise = fetchDefinition('apple');
    expect(loading.value).toBe(true);
    await fetchPromise;
    expect(definition.value).toBe('A fruit that grows on trees');
    expect(loading.value).toBe(false);
  });

  it('should handle any "No definition found" errors', async () => {
    const { definition, error, fetchDefinition } = useDefinition();

    fetch.mockResolvedValue({
      ok: false,
    });

    await fetchDefinition('notaword');
    expect(definition.value).toBe(null);
    expect(error.value).toBe('No definition found');
  });
});