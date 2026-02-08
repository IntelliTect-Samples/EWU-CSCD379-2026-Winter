/**
 * GameState.ts
 *
 * Central game state management with database-ready architecture.
 * All game data flows through this module to enable future persistence.
 */

import type { Dooble, DoobleStats, DoobleType } from "./Dooble";

// ============================================
// SERIALIZABLE DATA INTERFACES
// These interfaces define the shape of data that will be saved/loaded
// ============================================

/**
 * Represents a serialized Dooble for database storage
 */
export interface SerializedDooble {
  id: string;
  name: string;
  type: DoobleType;
  stats: DoobleStats;
  createdAt: number; // timestamp
}

/**
 * Represents the player's currency
 */
export interface PlayerCurrency {
  coins: number;
  gems: number;
}

/**
 * Represents a food slot state
 */
export interface FoodSlotData {
  id: number;
  available: boolean;
  rechargeAt: number | null; // timestamp when available again
}

/**
 * Tracks purchased upgrades and their levels
 */
export interface PurchasedUpgrades {
  // Coin shop upgrades
  extraFoodHearts: number; // How many extra hearts purchased (starts at 0)
  fasterFoodRecharge: number; // Level of upgrade (0 = none)
  hungerDecayReduction: number; // Level of upgrade (0 = none)
  fasterSpawnRate: number; // Level of upgrade (0 = none) - faster dooble spawning
  unlockBloobles: boolean; // Whether bloobles are unlocked
  maxPopulationIncrease: number; // Extra population cap (each level = +25)
  hasTrophy: boolean; // Win condition

  // Gem shop upgrades (aesthetic only)
  unlockedThemes: string[]; // IDs of unlocked themes
  unlockedDoobleColors: string[]; // IDs of unlocked colors
  unlockedBackgrounds: string[]; // IDs of unlocked backgrounds
}

/**
 * Main game state that will be serialized/deserialized
 */
export interface GameStateData {
  version: number;
  lastSaved: number;

  currency: PlayerCurrency;
  foodSlots: FoodSlotData[];
  upgrades: PurchasedUpgrades;
  doobles: SerializedDooble[];

  stats: {
    totalDooblesHatched: number;
    totalDooblesDied: number;
    totalFoodFed: number;
    longestLivedDoobleAge: number;
  };
}

// ============================================
// DEFAULT VALUES
// ============================================

export const DEFAULT_UPGRADES: PurchasedUpgrades = {
  extraFoodHearts: 0,
  fasterFoodRecharge: 0,
  hungerDecayReduction: 0,
  fasterSpawnRate: 0,
  unlockBloobles: false,
  maxPopulationIncrease: 0,
  hasTrophy: false,
  unlockedThemes: [],
  unlockedDoobleColors: [],
  unlockedBackgrounds: [],
};

export const DEFAULT_CURRENCY: PlayerCurrency = {
  coins: 0,
  gems: 0,
};

export function createDefaultGameState(): GameStateData {
  return {
    version: 1,
    lastSaved: Date.now(),
    currency: { ...DEFAULT_CURRENCY },
    foodSlots: [],
    upgrades: { ...DEFAULT_UPGRADES },
    doobles: [],
    stats: {
      totalDooblesHatched: 0,
      totalDooblesDied: 0,
      totalFoodFed: 0,
      longestLivedDoobleAge: 0,
    },
  };
}

// ============================================
// GAME STATE MANAGER CLASS
// ============================================

/**
 * GameStateManager handles all game state operations.
 * Designed to work offline-first with optional cloud sync.
 */
export class GameStateManager {
  private state: GameStateData;
  private listeners: Set<() => void> = new Set();

  private static readonly LOCAL_STORAGE_KEY = "dooble_game_state";

  constructor() {
    this.state = createDefaultGameState();
  }

  // ============================================
  // STATE ACCESS
  // ============================================

  getState(): Readonly<GameStateData> {
    return this.state;
  }

  getCurrency(): Readonly<PlayerCurrency> {
    return this.state.currency;
  }

  getUpgrades(): Readonly<PurchasedUpgrades> {
    return this.state.upgrades;
  }

  // ============================================
  // CURRENCY OPERATIONS
  // ============================================

  addCoins(amount: number): void {
    this.state.currency.coins += amount;
    this.notifyListeners();
  }

  addGems(amount: number): void {
    this.state.currency.gems += amount;
    this.notifyListeners();
  }

  spendCoins(amount: number): boolean {
    if (this.state.currency.coins >= amount) {
      this.state.currency.coins -= amount;
      this.notifyListeners();
      return true;
    }
    return false;
  }

  spendGems(amount: number): boolean {
    if (this.state.currency.gems >= amount) {
      this.state.currency.gems -= amount;
      this.notifyListeners();
      return true;
    }
    return false;
  }

  // ============================================
  // UPGRADE OPERATIONS
  // ============================================

  hasUpgrade(upgradeId: keyof PurchasedUpgrades): boolean {
    const value = this.state.upgrades[upgradeId];
    if (typeof value === "boolean") return value;
    if (typeof value === "number") return value > 0;
    if (Array.isArray(value)) return value.length > 0;
    return false;
  }

  getUpgradeLevel(upgradeId: keyof PurchasedUpgrades): number {
    const value = this.state.upgrades[upgradeId];
    if (typeof value === "number") return value;
    if (typeof value === "boolean") return value ? 1 : 0;
    return 0;
  }

  setUpgrade<K extends keyof PurchasedUpgrades>(
    upgradeId: K,
    value: PurchasedUpgrades[K],
  ): void {
    this.state.upgrades[upgradeId] = value;
    this.notifyListeners();
  }

  incrementUpgrade(
    upgradeId:
      | "extraFoodHearts"
      | "fasterFoodRecharge"
      | "hungerDecayReduction"
      | "fasterSpawnRate"
      | "maxPopulationIncrease",
  ): void {
    (this.state.upgrades[upgradeId] as number)++;
    this.notifyListeners();
  }

  // ============================================
  // FOOD SLOT OPERATIONS
  // ============================================

  getBaseFoodSlotCount(): number {
    return 10; // Base amount
  }

  getTotalFoodSlotCount(): number {
    return this.getBaseFoodSlotCount() + this.state.upgrades.extraFoodHearts;
  }

  getFoodRechargeMultiplier(): number {
    // Each level reduces recharge time by 10%
    const level = this.state.upgrades.fasterFoodRecharge;
    return Math.max(0.3, 1 - level * 0.1); // Minimum 30% of original time
  }

  // ============================================
  // STAT MODIFIERS
  // ============================================

  getHungerDecayMultiplier(): number {
    // Each level reduces hunger decay by 10%
    const level = this.state.upgrades.hungerDecayReduction;
    return Math.max(0.5, 1 - level * 0.1); // Minimum 50% of original
  }

  getMaxPopulation(): number {
    const base = 150;
    return base + this.state.upgrades.maxPopulationIncrease * 25;
  }

  getSpawnRateMultiplier(): number {
    // Each level reduces spawn time by 5%
    const level = this.state.upgrades.fasterSpawnRate;
    return Math.max(0.3, 1 - level * 0.05); // Minimum 30% of original time
  }

  // ============================================
  // STATISTICS
  // ============================================

  recordDoobleHatched(): void {
    this.state.stats.totalDooblesHatched++;
  }

  recordDoobleDeath(age: number): void {
    this.state.stats.totalDooblesDied++;
    if (age > this.state.stats.longestLivedDoobleAge) {
      this.state.stats.longestLivedDoobleAge = age;
    }
  }

  recordFoodFed(): void {
    this.state.stats.totalFoodFed++;
  }

  // ============================================
  // SAVE/LOAD OPERATIONS (LOCAL)
  // ============================================

  saveToLocal(): void {
    this.state.lastSaved = Date.now();
    try {
      localStorage.setItem(
        GameStateManager.LOCAL_STORAGE_KEY,
        JSON.stringify(this.state),
      );
    } catch (e) {
      console.error("Failed to save game state to localStorage:", e);
    }
  }

  loadFromLocal(): boolean {
    try {
      const saved = localStorage.getItem(GameStateManager.LOCAL_STORAGE_KEY);
      if (saved) {
        const parsed = JSON.parse(saved) as GameStateData;
        this.state = this.migrateState(parsed);
        this.notifyListeners();
        return true;
      }
    } catch (e) {
      console.error("Failed to load game state from localStorage:", e);
    }
    return false;
  }

  clearLocal(): void {
    localStorage.removeItem(GameStateManager.LOCAL_STORAGE_KEY);
    this.state = createDefaultGameState();
    this.notifyListeners();
  }

  /**
   * Reset game state for game over — keeps currency and upgrades,
   * clears doobles and resets stats.
   */
  resetForGameOver(): void {
    this.state.doobles = [];
    this.state.foodSlots = [];
    this.saveToLocal();
    this.notifyListeners();
  }

  // ============================================
  // STATE MIGRATION
  // ============================================

  private migrateState(savedState: GameStateData): GameStateData {
    const state = { ...savedState };

    // Ensure new upgrade fields exist
    if (state.upgrades.maxPopulationIncrease === undefined) {
      state.upgrades.maxPopulationIncrease = 0;
    }
    if (state.upgrades.hasTrophy === undefined) {
      state.upgrades.hasTrophy = false;
    }
    if (state.upgrades.fasterSpawnRate === undefined) {
      state.upgrades.fasterSpawnRate = 0;
    }

    return state;
  }

  // ============================================
  // CHANGE LISTENERS
  // ============================================

  subscribe(listener: () => void): () => void {
    this.listeners.add(listener);
    return () => this.listeners.delete(listener);
  }

  private notifyListeners(): void {
    this.listeners.forEach((listener) => listener());
  }
}

// Singleton instance for global access
export const gameState = new GameStateManager();
