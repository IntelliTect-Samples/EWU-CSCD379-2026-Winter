import { describe, it, expect, beforeEach } from "vitest";
import {
  GameStateManager,
  createDefaultGameState,
  DEFAULT_CURRENCY,
  DEFAULT_UPGRADES,
} from "../app/scripts/GameState";

describe("GameState", () => {
  describe("createDefaultGameState", () => {
    it("should create a state with version 1", () => {
      const state = createDefaultGameState();
      expect(state.version).toBe(1);
    });

    it("should create a state with default currency", () => {
      const state = createDefaultGameState();
      expect(state.currency.coins).toBe(0);
      expect(state.currency.gems).toBe(0);
    });

    it("should create a state with empty doobles array", () => {
      const state = createDefaultGameState();
      expect(state.doobles).toEqual([]);
    });

    it("should create a state with zeroed stats", () => {
      const state = createDefaultGameState();
      expect(state.stats.totalDooblesHatched).toBe(0);
      expect(state.stats.totalDooblesDied).toBe(0);
      expect(state.stats.totalFoodFed).toBe(0);
      expect(state.stats.longestLivedDoobleAge).toBe(0);
    });
  });

  describe("DEFAULT_CURRENCY", () => {
    it("should have coins set to 0", () => {
      expect(DEFAULT_CURRENCY.coins).toBe(0);
    });

    it("should have gems set to 0", () => {
      expect(DEFAULT_CURRENCY.gems).toBe(0);
    });
  });

  describe("DEFAULT_UPGRADES", () => {
    it("should have all numeric upgrades at 0", () => {
      expect(DEFAULT_UPGRADES.extraFoodHearts).toBe(0);
      expect(DEFAULT_UPGRADES.fasterFoodRecharge).toBe(0);
      expect(DEFAULT_UPGRADES.hungerDecayReduction).toBe(0);
      expect(DEFAULT_UPGRADES.fasterSpawnRate).toBe(0);
      expect(DEFAULT_UPGRADES.maxPopulationIncrease).toBe(0);
    });

    it("should have boolean upgrades as false", () => {
      expect(DEFAULT_UPGRADES.unlockBloobles).toBe(false);
      expect(DEFAULT_UPGRADES.hasTrophy).toBe(false);
    });

    it("should have empty arrays for unlocked items", () => {
      expect(DEFAULT_UPGRADES.unlockedThemes).toEqual([]);
      expect(DEFAULT_UPGRADES.unlockedDoobleColors).toEqual([]);
      expect(DEFAULT_UPGRADES.unlockedBackgrounds).toEqual([]);
    });
  });

  describe("GameStateManager", () => {
    let manager: GameStateManager;

    beforeEach(() => {
      manager = new GameStateManager();
    });

    describe("initial state", () => {
      it("should start with default currency", () => {
        const currency = manager.getCurrency();
        expect(currency.coins).toBe(0);
        expect(currency.gems).toBe(0);
      });

      it("should start with default upgrades", () => {
        const upgrades = manager.getUpgrades();
        expect(upgrades.extraFoodHearts).toBe(0);
        expect(upgrades.unlockBloobles).toBe(false);
      });
    });

    describe("currency operations", () => {
      it("should add coins correctly", () => {
        manager.addCoins(100);
        expect(manager.getCurrency().coins).toBe(100);
      });

      it("should add gems correctly", () => {
        manager.addGems(50);
        expect(manager.getCurrency().gems).toBe(50);
      });

      it("should spend coins when sufficient", () => {
        manager.addCoins(100);
        const result = manager.spendCoins(30);

        expect(result).toBe(true);
        expect(manager.getCurrency().coins).toBe(70);
      });

      it("should not spend coins when insufficient", () => {
        manager.addCoins(20);
        const result = manager.spendCoins(50);

        expect(result).toBe(false);
        expect(manager.getCurrency().coins).toBe(20);
      });

      it("should spend gems when sufficient", () => {
        manager.addGems(100);
        const result = manager.spendGems(40);

        expect(result).toBe(true);
        expect(manager.getCurrency().gems).toBe(60);
      });

      it("should not spend gems when insufficient", () => {
        manager.addGems(10);
        const result = manager.spendGems(20);

        expect(result).toBe(false);
        expect(manager.getCurrency().gems).toBe(10);
      });
    });

    describe("upgrade operations", () => {
      it("should report hasUpgrade false for numeric upgrades at 0", () => {
        expect(manager.hasUpgrade("extraFoodHearts")).toBe(false);
      });

      it("should report hasUpgrade true for numeric upgrades > 0", () => {
        manager.setUpgrade("extraFoodHearts", 1);
        expect(manager.hasUpgrade("extraFoodHearts")).toBe(true);
      });

      it("should report hasUpgrade false for boolean false", () => {
        expect(manager.hasUpgrade("unlockBloobles")).toBe(false);
      });

      it("should report hasUpgrade true for boolean true", () => {
        manager.setUpgrade("unlockBloobles", true);
        expect(manager.hasUpgrade("unlockBloobles")).toBe(true);
      });

      it("should get upgrade level for numeric upgrades", () => {
        manager.setUpgrade("extraFoodHearts", 5);
        expect(manager.getUpgradeLevel("extraFoodHearts")).toBe(5);
      });

      it("should increment numeric upgrades", () => {
        expect(manager.getUpgradeLevel("extraFoodHearts")).toBe(0);
        manager.incrementUpgrade("extraFoodHearts");
        expect(manager.getUpgradeLevel("extraFoodHearts")).toBe(1);
      });
    });

    describe("food slot operations", () => {
      it("should return base food slot count of 10", () => {
        expect(manager.getBaseFoodSlotCount()).toBe(10);
      });

      it("should calculate total food slots with upgrades", () => {
        expect(manager.getTotalFoodSlotCount()).toBe(10);
        manager.setUpgrade("extraFoodHearts", 5);
        expect(manager.getTotalFoodSlotCount()).toBe(15);
      });

      it("should calculate food recharge multiplier", () => {
        expect(manager.getFoodRechargeMultiplier()).toBe(1);
        manager.setUpgrade("fasterFoodRecharge", 3);
        expect(manager.getFoodRechargeMultiplier()).toBe(0.7);
      });

      it("should cap food recharge multiplier at 0.3", () => {
        manager.setUpgrade("fasterFoodRecharge", 20);
        expect(manager.getFoodRechargeMultiplier()).toBe(0.3);
      });
    });

    describe("stat modifiers", () => {
      it("should calculate hunger decay multiplier", () => {
        expect(manager.getHungerDecayMultiplier()).toBe(1);
        manager.setUpgrade("hungerDecayReduction", 2);
        expect(manager.getHungerDecayMultiplier()).toBe(0.8);
      });

      it("should cap hunger decay multiplier at 0.5", () => {
        manager.setUpgrade("hungerDecayReduction", 20);
        expect(manager.getHungerDecayMultiplier()).toBe(0.5);
      });

      it("should calculate max population", () => {
        expect(manager.getMaxPopulation()).toBe(150);
        manager.setUpgrade("maxPopulationIncrease", 2);
        expect(manager.getMaxPopulation()).toBe(200);
      });

      it("should calculate spawn rate multiplier", () => {
        expect(manager.getSpawnRateMultiplier()).toBe(1);
        manager.setUpgrade("fasterSpawnRate", 4);
        expect(manager.getSpawnRateMultiplier()).toBe(0.8);
      });

      it("should cap spawn rate multiplier at 0.3", () => {
        manager.setUpgrade("fasterSpawnRate", 20);
        expect(manager.getSpawnRateMultiplier()).toBe(0.3);
      });
    });

    describe("statistics", () => {
      it("should record dooble hatched", () => {
        manager.recordDoobleHatched();
        manager.recordDoobleHatched();
        expect(manager.getState().stats.totalDooblesHatched).toBe(2);
      });

      it("should record dooble death and track longest lived", () => {
        manager.recordDoobleDeath(10);
        expect(manager.getState().stats.totalDooblesDied).toBe(1);
        expect(manager.getState().stats.longestLivedDoobleAge).toBe(10);
      });

      it("should update longest lived when new record is set", () => {
        manager.recordDoobleDeath(5);
        manager.recordDoobleDeath(15);
        manager.recordDoobleDeath(8);
        expect(manager.getState().stats.longestLivedDoobleAge).toBe(15);
      });

      it("should record food fed", () => {
        manager.recordFoodFed();
        manager.recordFoodFed();
        manager.recordFoodFed();
        expect(manager.getState().stats.totalFoodFed).toBe(3);
      });
    });
  });
});
