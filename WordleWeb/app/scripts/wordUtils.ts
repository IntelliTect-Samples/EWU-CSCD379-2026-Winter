import wordList from "~~/public/wordList.json";

export const words: string[] = wordList;

// FNV-1a hash for better distribution
function fnv1a(str: string): number {
  let hash = 2166136261;
  for (let i = 0; i < str.length; i++) {
    hash ^= str.charCodeAt(i);
    hash +=
      (hash << 1) + (hash << 4) + (hash << 7) + (hash << 8) + (hash << 24);
  }
  return hash >>> 0;
}

export const getDailyWord = (): string => {
  const today = new Date();
  const dateString = today.toISOString().split("T")[0] ?? "";
  let index = fnv1a(dateString) % words.length;

  // Avoid consecutive repeats (yesterday's word)
  const yesterday = new Date(today.getTime() - 86400000);
  const yesterdayString = yesterday.toISOString().split("T")[0] ?? "";
  const yesterdayIndex = fnv1a(yesterdayString) % words.length;
  if (index === yesterdayIndex) {
    index = (index + 1) % words.length;
  }
  return words[index] ?? words[0] ?? "wordle";
};

export const getRandomWord = (): string => {
  return (
    words[Math.floor(Math.random() * words.length)] ?? words[0] ?? "wordle"
  );
};

export const getTodayDateString = (): string => {
  const today = new Date();
  return today.toISOString().split("T")[0] ?? "";
};
