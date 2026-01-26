export function readJSON<T>(key: string, fallback: T): T {
    if (import.meta.server) return fallback
    const raw = localStorage.getItem(key)
    return raw ? JSON.parse(raw) : fallback
}

export function writeJSON<T>(key: string, value: T) {
    if (import.meta.server) return
    localStorage.setItem(key, JSON.stringify(value))
}
