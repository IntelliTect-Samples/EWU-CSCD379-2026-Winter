import { ref } from 'vue'

export type WordDefinition = {
    word: string
    phonetic?: string
    partOfSpeech?: string
    definition?: string
    example?: string
    source?: string
}

export function useDefinition() {
    const loading = ref(false)
    const error = ref<string | null>(null)

    async function fetchDefinition(word: string): Promise<WordDefinition | null> {
        const w = word.trim().toLowerCase()
        if (!w) return null

        loading.value = true
        error.value = null

        try {
            const res = await fetch(`https://api.dictionaryapi.dev/api/v2/entries/en/${encodeURIComponent(w)}`)
            if (!res.ok) {
                return {
                    word: w,
                    definition: 'No definition found.',
                    source: 'dictionaryapi.dev',
                }
            }
            const data = await res.json()
            const entry = data?.[0]
            const phonetic = entry?.phonetic || entry?.phonetics?.find((p: any) => p?.text)?.text

            const meaning = entry?.meanings?.[0]
            const partOfSpeech = meaning?.partOfSpeech

            const defObj = meaning?.definitions?.[0]
            const definition = defObj?.definition
            const example = defObj?.example

            return {
                word: w,
                phonetic,
                partOfSpeech,
                definition: definition ?? 'No definition found.',
                example,
                source: 'dictionaryapi.dev',
            }
        } catch (e: any) {
            error.value = 'Failed to fetch definition.'
            return {
                word: w,
                definition: 'Failed to fetch definition.',
                source: 'dictionaryapi.dev',
            }
        } finally {
            loading.value = false
        }
    }
    return { loading, error, fetchDefinition }
}