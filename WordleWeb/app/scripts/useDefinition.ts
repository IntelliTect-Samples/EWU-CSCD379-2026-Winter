import { ref } from "vue";

export function useDefinition() {
  const definition = ref<string | null>(null);
  const loading = ref(false);
  const error = ref<string | null>(null);

  async function fetchDefinition(word: string) {
    definition.value = null;
    error.value = null;
    loading.value = true;
    try {
      // Use Free Dictionary API
      const res = await fetch(
        `https://api.dictionaryapi.dev/api/v2/entries/en/${word}`,
      );
      if (!res.ok) throw new Error("No definition found");
      const data = await res.json();
      // Try to get the first definition
      const def = data?.[0]?.meanings?.[0]?.definitions?.[0]?.definition;
      if (def) {
        definition.value = def;
      } else {
        throw new Error("No definition found");
      }
    } catch (e: any) {
      error.value = e.message || "No definition found";
    } finally {
      loading.value = false;
    }
  }

  return { definition, loading, error, fetchDefinition };
}
