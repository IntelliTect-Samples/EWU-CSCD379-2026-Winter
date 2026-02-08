/**
 * ShopSystem.ts
 *
 * Shop system with coin and gem shops.
 * Designed for database persistence and easy extensibility.
 */

import { gameState, type PurchasedUpgrades } from "./GameState";

// ============================================
// SHOP ITEM INTERFACES
// ============================================

export type CurrencyType = "coins" | "gems";

export interface ShopItemBase {
  id: string;
  name: string;
  description: string;
  icon: string; // Icon identifier (emoji or image path)
  category: "gameplay" | "aesthetic";
}

export interface StaticPriceItem extends ShopItemBase {
  priceType: "static";
  price: number;
  currencyType: CurrencyType;
  maxPurchases: number; // 1 for one-time purchases, -1 for unlimited
}

export interface ScalingPriceItem extends ShopItemBase {
  priceType: "scaling";
  basePrice: number;
  priceIncrement: number; // Added per purchase
  currencyType: CurrencyType;
  maxPurchases: number; // -1 for unlimited
}

export type ShopItem = StaticPriceItem | ScalingPriceItem;

// ============================================
// COIN SHOP ITEMS
// ============================================

export const COIN_SHOP_ITEMS: ShopItem[] = [
  {
    id: "extraFoodHearts",
    name: "Extra Food Heart",
    description: "Adds one more food slot to your food bar",
    icon: "❤️",
    category: "gameplay",
    priceType: "scaling",
    basePrice: 100,
    priceIncrement: 100,
    currencyType: "coins",
    maxPurchases: 20,
  },
  {
    id: "fasterFoodRecharge",
    name: "Faster Recharge",
    description: "Food slots recharge 10% faster (stacks)",
    icon: "⚡",
    category: "gameplay",
    priceType: "scaling",
    basePrice: 10,
    priceIncrement: 50,
    currencyType: "coins",
    maxPurchases: 20,
  },
  {
    id: "hungerDecayReduction",
    name: "Slow Hunger",
    description: "Doobles get hungry 10% slower (stacks)",
    icon: "🍖",
    category: "gameplay",
    priceType: "scaling",
    basePrice: 10,
    priceIncrement: 75,
    currencyType: "coins",
    maxPurchases: 20,
  },
  {
    id: "fasterSpawnRate",
    name: "Fertility Boost",
    description: "Doobles spawn 5% faster (stacks)",
    icon: "🥚",
    category: "gameplay",
    priceType: "scaling",
    basePrice: 150,
    priceIncrement: 150,
    currencyType: "coins",
    maxPurchases: 20,
  },
  {
    id: "unlockBloobles",
    name: "Unlock Bloobles",
    description:
      "Unlock the rare Blooble species! They live longer and give 5 coins when they die. Persists across games.",
    icon: "🔵",
    category: "gameplay",
    priceType: "static",
    price: 1000,
    currencyType: "coins",
    maxPurchases: 1,
  },
  {
    id: "maxPopulationIncrease",
    name: "Expand Territory",
    description: "Increases max dooble population by 25 (stacks)",
    icon: "🏠",
    category: "gameplay",
    priceType: "scaling",
    basePrice: 200,
    priceIncrement: 200,
    currencyType: "coins",
    maxPurchases: 20,
  },
  {
    id: "trophy",
    name: "Golden Trophy",
    description: "The ultimate prize. Buy this to win the game!",
    icon: "🏆",
    category: "gameplay",
    priceType: "static",
    price: 10000,
    currencyType: "coins",
    maxPurchases: 1,
  },
];

// ============================================
// GEM SHOP ITEMS (Aesthetic only - empty for now)
// ============================================

export const GEM_SHOP_ITEMS: ShopItem[] = [];

// ============================================
// SHOP MANAGER CLASS
// ============================================

export interface PurchaseResult {
  success: boolean;
  message: string;
  newLevel?: number;
}

export class ShopManager {
  /**
   * Get the current price of an item considering purchases
   */
  getItemPrice(item: ShopItem): number {
    if (item.priceType === "static") {
      return item.price;
    }

    const currentLevel = this.getPurchaseCount(item.id);
    return item.basePrice + currentLevel * item.priceIncrement;
  }

  /**
   * Get how many times an item has been purchased
   */
  getPurchaseCount(itemId: string): number {
    const upgrades = gameState.getUpgrades();
    const value = upgrades[itemId as keyof PurchasedUpgrades];

    if (typeof value === "boolean") return value ? 1 : 0;
    if (typeof value === "number") return value;
    if (Array.isArray(value)) return value.length;
    return 0;
  }

  /**
   * Check if an item can be purchased
   */
  canPurchase(item: ShopItem): { canBuy: boolean; reason?: string } {
    const currency = gameState.getCurrency();
    const price = this.getItemPrice(item);
    const currentCount = this.getPurchaseCount(item.id);

    // Check max purchases
    if (item.maxPurchases !== -1 && currentCount >= item.maxPurchases) {
      return { canBuy: false, reason: "Maximum purchases reached" };
    }

    // Check currency
    if (item.currencyType === "coins" && currency.coins < price) {
      return { canBuy: false, reason: "Not enough coins" };
    }
    if (item.currencyType === "gems" && currency.gems < price) {
      return { canBuy: false, reason: "Not enough gems" };
    }

    return { canBuy: true };
  }

  /**
   * Attempt to purchase an item
   */
  purchase(item: ShopItem): PurchaseResult {
    const canPurchaseResult = this.canPurchase(item);
    if (!canPurchaseResult.canBuy) {
      return {
        success: false,
        message: canPurchaseResult.reason || "Cannot purchase",
      };
    }

    const price = this.getItemPrice(item);

    // Deduct currency
    if (item.currencyType === "coins") {
      if (!gameState.spendCoins(price)) {
        return { success: false, message: "Failed to spend coins" };
      }
    } else {
      if (!gameState.spendGems(price)) {
        return { success: false, message: "Failed to spend gems" };
      }
    }

    // Apply upgrade
    const currentCount = this.getPurchaseCount(item.id);
    const newLevel = currentCount + 1;

    // Handle different upgrade types
    switch (item.id) {
      case "extraFoodHearts":
        gameState.setUpgrade("extraFoodHearts", newLevel);
        break;
      case "fasterFoodRecharge":
        gameState.setUpgrade("fasterFoodRecharge", newLevel);
        break;
      case "hungerDecayReduction":
        gameState.setUpgrade("hungerDecayReduction", newLevel);
        break;
      case "unlockBloobles":
        gameState.setUpgrade("unlockBloobles", true);
        break;
      case "fasterSpawnRate":
        gameState.setUpgrade("fasterSpawnRate", newLevel);
        break;
      case "maxPopulationIncrease":
        gameState.setUpgrade("maxPopulationIncrease", newLevel);
        break;
      case "trophy":
        gameState.setUpgrade("hasTrophy", true);
        break;
      // Gem shop items (aesthetic) - handle when implemented
      default:
        // For unlockable items (themes, colors, etc.)
        const upgrades = gameState.getUpgrades();
        if (item.id.startsWith("theme_")) {
          const themes = [...upgrades.unlockedThemes, item.id];
          gameState.setUpgrade("unlockedThemes", themes);
        } else if (item.id.startsWith("dooble_color_")) {
          const colors = [...upgrades.unlockedDoobleColors, item.id];
          gameState.setUpgrade("unlockedDoobleColors", colors);
        } else if (item.id.startsWith("background_")) {
          const bgs = [...upgrades.unlockedBackgrounds, item.id];
          gameState.setUpgrade("unlockedBackgrounds", bgs);
        }
    }

    // Save after purchase
    gameState.saveToLocal();

    return {
      success: true,
      message: `Purchased ${item.name}!`,
      newLevel,
    };
  }

  /**
   * Get all coin shop items with their current status
   */
  getCoinShopItems(): Array<
    ShopItem & {
      currentPrice: number;
      purchaseCount: number;
      canPurchase: boolean;
    }
  > {
    return COIN_SHOP_ITEMS.map((item) => ({
      ...item,
      currentPrice: this.getItemPrice(item),
      purchaseCount: this.getPurchaseCount(item.id),
      canPurchase: this.canPurchase(item).canBuy,
    }));
  }

  /**
   * Get all gem shop items with their current status
   */
  getGemShopItems(): Array<
    ShopItem & {
      currentPrice: number;
      purchaseCount: number;
      canPurchase: boolean;
    }
  > {
    return GEM_SHOP_ITEMS.map((item) => ({
      ...item,
      currentPrice: this.getItemPrice(item),
      purchaseCount: this.getPurchaseCount(item.id),
      canPurchase: this.canPurchase(item).canBuy,
    }));
  }
}

// Singleton instance
export const shopManager = new ShopManager();
