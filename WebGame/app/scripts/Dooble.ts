export interface DoobleStats {
  age: number; // in days
  hunger: number; // 0 (full) to 100 (starving)
}

export type DoobleType = "dooble" | "blooble";

export class Dooble {
  name: string;
  type: DoobleType;
  stats: DoobleStats;

  constructor(name: string, type: DoobleType = "dooble") {
    this.name = name;
    this.type = type;
    this.stats = {
      age: 0,
      hunger: 0,
    };
  }

  feed(amount: number) {
    this.stats.hunger = Math.max(0, this.stats.hunger - amount);
  }
}
