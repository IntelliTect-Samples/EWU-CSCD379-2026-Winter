<template>
  <v-app>
    <!-- Background Music -->
    <audio ref="bgMusic" src="/sounds/DoobleTheme.mp3" autoplay loop />
    <v-main class="game-area">
      <!-- Game Background -->
      <div class="game-background">
        <img src="/images/GameBackground.png" class="game-bg-image" />
      </div>

      <!-- Game Content -->
      <div class="game-content">
        <!-- Back button -->
        <button
          type="button"
          class="nes-btn is-error back-btn"
          @click.stop="goBack"
        >
          ← Back to Menu
        </button>

        <!-- Top Right HUD: Food Bar + Currency + Shop -->
        <div class="top-right-hud">
          <!-- Food Bar -->
          <div class="food-bar">
            <div
              v-for="slot in foodSlots"
              :key="slot.id"
              class="food-slot"
              :class="{ 'is-recharging': !slot.available }"
              :title="
                slot.available
                  ? 'Available'
                  : `Recharging: ${getSlotTimer(slot)}`
              "
            >
              <i
                class="nes-icon heart"
                :class="{ 'is-empty': !slot.available }"
              ></i>
              <span v-if="!slot.available" class="slot-timer">{{
                getSlotTimer(slot)
              }}</span>
            </div>
          </div>

          <!-- Currency Display -->
          <div class="currency-display">
            <div class="currency-item coins" @click="openShop('coins')">
              <i class="nes-icon coin is-small"></i>
              <span class="currency-value">{{ coins }}</span>
            </div>
            <div class="currency-item gems" @click="openShop('gems')">
              <i class="nes-icon like is-small"></i>
              <span class="currency-value">{{ gems }}</span>
            </div>
          </div>

          <!-- Shop Button -->
          <button
            class="nes-btn is-warning shop-btn"
            @click="openShop('coins')"
          >
            🛒 Shop
          </button>
        </div>

        <!-- Game area placeholder -->
        <div class="game-container" ref="gameContainer">
          <!-- Game World text that fades out -->
          <p v-if="showGameWorldText" class="game-text fade-out">Game World</p>

          <!-- All Doobles -->
          <div
            v-for="doobleVisual in doobleVisuals"
            :key="doobleVisual.id"
            class="dooble"
            :class="{ spawning: doobleVisual.state === 'spawning' }"
            :style="{
              transform: `translate(${doobleVisual.x}px, ${doobleVisual.y}px) scaleX(${doobleVisual.facingDirection})`,
            }"
            @click.stop="selectDooble(doobleVisual.id)"
          >
            <div v-if="doobleVisual.isUnnamed" class="exclamation-mark">!</div>
            <img
              :src="getDoobleFrame(doobleVisual)"
              class="dooble-sprite"
              alt="Dooble"
            />
          </div>
        </div>

        <!-- Population Warning -->
        <div v-if="showPopulationWarning" class="population-warning">
          <div class="nes-container is-dark is-rounded warning-banner">
            <p class="warning-text">
              ⚠️ Your population of doobles is getting to be too much... time to
              make some hard decisions.
            </p>
          </div>
        </div>

        <!-- Selected Dooble Stats Panel -->
        <div
          v-if="selectedDoobleId !== null"
          class="nes-container is-dark stats-panel"
          @click.stop
        >
          <button
            class="nes-btn is-error close-btn"
            @click.stop="selectedDoobleId = null"
          >
            ×
          </button>
          <h3 class="stats-title">{{ selectedDooble?.dooble.name }}</h3>

          <div class="stat-row">
            <label>Hunger:</label>
            <progress
              class="nes-progress"
              :class="getHungerClass(selectedDooble?.dooble.stats.hunger || 0)"
              :value="
                Math.round(100 - (selectedDooble?.dooble.stats.hunger || 0))
              "
              max="100"
            ></progress>
            <span class="stat-value"
              >{{
                Math.round(100 - (selectedDooble?.dooble.stats.hunger || 0))
              }}%</span
            >
          </div>

          <div class="stat-row">
            <label>Age:</label>
            <span class="stat-value"
              >{{
                Math.round(selectedDooble?.dooble.stats.age || 0)
              }}
              days</span
            >
          </div>

          <div class="action-buttons">
            <button
              class="nes-btn is-success"
              @click.stop="feedDooble"
              :disabled="
                selectedDooble?.dooble.stats.hunger === 0 ||
                availableFoodCount === 0
              "
            >
              🍖 Feed
            </button>
            <button class="nes-btn is-error" @click.stop="sacrificeDooble">
              💀 Sacrifice
            </button>
          </div>
        </div>

        <!-- Feed Mini-game Modal -->
        <div v-if="showFeedModal" class="modal-overlay" @click.stop>
          <div class="nes-container is-dark is-rounded modal-content mini-game">
            <h2 class="modal-title">
              <i class="nes-icon trophy is-small"></i> Feed Time!
            </h2>

            <p class="modal-text mini-game-instructions">
              Press <span class="nes-text is-primary">SPACE</span> or
              <span class="nes-text is-primary">CLICK</span> when the indicator
              is in the <span class="nes-text is-success">green zone</span>!
            </p>

            <!-- Power Bar -->
            <div class="power-bar-container" @click.stop="attemptFeed">
              <div class="power-bar">
                <!-- Sweet spot zone (success area) -->
                <div
                  class="sweet-spot"
                  :style="{
                    left: sweetSpotStart + '%',
                    width: sweetSpotWidth + '%',
                  }"
                ></div>

                <!-- Moving indicator -->
                <div
                  class="indicator"
                  :class="{ stopped: indicatorStopped }"
                  :style="{ left: indicatorX + '%' }"
                >
                  <i class="nes-icon star is-small"></i>
                </div>
              </div>

              <!-- Markers -->
              <div class="bar-markers">
                <span>0</span>
                <span>50</span>
                <span>100</span>
              </div>
            </div>

            <!-- Attempts remaining -->
            <div class="attempts-display">
              <span class="nes-text">Attempts: </span>
              <span v-for="i in maxAttempts" :key="i">
                <i
                  class="nes-icon heart"
                  :class="{ 'is-empty': i > attemptsLeft }"
                ></i>
              </span>
            </div>

            <!-- Result message -->
            <div
              v-if="feedMessage"
              class="feed-message-box nes-container is-rounded"
              :class="feedMessageClass"
            >
              <span>{{ feedMessage }}</span>
            </div>

            <!-- Buttons -->
            <div class="mini-game-buttons">
              <button
                v-if="indicatorStopped && attemptsLeft > 0"
                class="nes-btn is-primary"
                @click.stop="retryAttempt"
              >
                Try Again
              </button>
              <button class="nes-btn is-error" @click.stop="cancelFeed">
                Give Up
              </button>
            </div>
          </div>
        </div>

        <!-- New Dooble Modal -->
        <div v-if="showNameDoobleModal" class="modal-overlay">
          <div class="nes-container is-dark modal-content">
            <h2 class="modal-title">Name this Dooble</h2>
            <p class="modal-text">Give your Dooble a name:</p>
            <input
              type="text"
              class="nes-input"
              v-model="newDoobleName"
              placeholder="Enter name..."
              @keyup.enter="confirmDoobleName"
            />
            <button
              class="nes-btn is-primary modal-btn"
              @click="confirmDoobleName"
            >
              Confirm
            </button>
          </div>
        </div>

        <!-- Death Notification -->
        <div v-if="showDeathModal" class="death-notification">
          <div class="nes-container is-dark is-rounded death-modal">
            <h2 class="modal-title death-title">
              <i class="nes-icon close is-small"></i> Dooble Died
            </h2>
            <p class="modal-text death-message">{{ deathMessage }}</p>
            <div v-if="deathReward" class="death-reward">
              <span class="reward-label">You received:</span>
              <div class="reward-display">
                <i
                  class="nes-icon is-medium"
                  :class="deathReward.type === 'gem' ? 'like' : 'coin'"
                ></i>
                <span class="reward-amount">+{{ deathReward.amount }}</span>
                <span class="reward-type">{{
                  deathReward.type === "gem" ? "Gem" : "Coin"
                }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Game Over Modal -->
        <div v-if="showGameOverModal" class="modal-overlay game-over-overlay">
          <div class="nes-container is-dark is-rounded game-over-modal">
            <h2 class="modal-title game-over-title">Game Over</h2>
            <p class="modal-text game-over-message">
              {{
                gameOverReason === "too_many"
                  ? "Your doobles have overrun the world! There are too many doobles, chaos has taken over."
                  : "All your doobles are gone... There is nothing left."
              }}
            </p>
            <button class="nes-btn is-error" @click="dismissGameOver">
              Back to Menu
            </button>
          </div>
        </div>

        <!-- Win Modal -->
        <div v-if="showWinModal" class="modal-overlay game-over-overlay">
          <div class="nes-container is-dark is-rounded game-over-modal">
            <h2 class="modal-title win-title">🏆 You Win! 🏆</h2>
            <p class="modal-text game-over-message">
              You purchased the Golden Trophy! Your dooble empire stands
              triumphant. Congratulations!
            </p>
            <button class="nes-btn is-warning" @click="dismissWin">
              Back to Menu
            </button>
          </div>
        </div>

        <!-- Shop Modal -->
        <div v-if="showShopModal" class="modal-overlay" @click.self="closeShop">
          <div class="nes-container is-dark is-rounded shop-modal">
            <button class="nes-btn is-error close-btn" @click="closeShop">
              ×
            </button>

            <h2 class="shop-title">🛒 Shop</h2>

            <!-- Shop Tabs -->
            <div class="shop-tabs">
              <button
                class="nes-btn"
                :class="{
                  'is-warning': activeShopTab === 'coins',
                  'is-disabled': activeShopTab !== 'coins',
                }"
                @click="activeShopTab = 'coins'"
              >
                <i class="nes-icon coin is-small"></i> Coin Shop
              </button>
              <button
                class="nes-btn"
                :class="{
                  'is-primary': activeShopTab === 'gems',
                  'is-disabled': activeShopTab !== 'gems',
                }"
                @click="activeShopTab = 'gems'"
              >
                <i class="nes-icon like is-small gem-icon"></i> Gem Shop
              </button>
            </div>

            <!-- Current Currency Display -->
            <div class="shop-currency">
              <span v-if="activeShopTab === 'coins'">
                <i class="nes-icon coin is-small"></i> {{ coins }} coins
              </span>
              <span v-else>
                <i class="nes-icon like is-small gem-icon"></i> {{ gems }} gems
              </span>
            </div>

            <!-- Coin Shop Items -->
            <div v-if="activeShopTab === 'coins'" class="shop-items">
              <div
                v-for="item in coinShopItems"
                :key="item.id"
                class="shop-item"
                :class="{
                  'is-maxed':
                    item.maxPurchases !== -1 &&
                    item.purchaseCount >= item.maxPurchases,
                }"
              >
                <div class="item-icon">{{ item.icon }}</div>
                <div class="item-info">
                  <div class="item-name">{{ item.name }}</div>
                  <div class="item-description">{{ item.description }}</div>
                  <div v-if="item.maxPurchases > 1" class="item-level">
                    Level: {{ item.purchaseCount }} / {{ item.maxPurchases }}
                  </div>
                </div>
                <div class="item-price">
                  <button
                    class="nes-btn is-success"
                    :class="{ 'is-disabled': !item.canPurchase }"
                    :disabled="!item.canPurchase"
                    @click="purchaseItem(item)"
                  >
                    <template
                      v-if="
                        item.maxPurchases !== -1 &&
                        item.purchaseCount >= item.maxPurchases
                      "
                    >
                      MAX
                    </template>
                    <template v-else>
                      {{ item.currentPrice }}
                      <i class="nes-icon coin is-small"></i>
                    </template>
                  </button>
                </div>
              </div>
            </div>

            <!-- Gem Shop Items -->
            <div v-else class="shop-items">
              <div v-if="gemShopItems.length === 0" class="shop-empty">
                <p>✨ Coming Soon! ✨</p>
                <p class="shop-empty-sub">
                  Aesthetic upgrades will be available here.
                </p>
              </div>
              <div
                v-for="item in gemShopItems"
                :key="item.id"
                class="shop-item"
                :class="{
                  'is-maxed':
                    item.maxPurchases !== -1 &&
                    item.purchaseCount >= item.maxPurchases,
                }"
              >
                <div class="item-icon">{{ item.icon }}</div>
                <div class="item-info">
                  <div class="item-name">{{ item.name }}</div>
                  <div class="item-description">{{ item.description }}</div>
                </div>
                <div class="item-price">
                  <button
                    class="nes-btn is-primary"
                    :class="{ 'is-disabled': !item.canPurchase }"
                    :disabled="!item.canPurchase"
                    @click="purchaseItem(item)"
                  >
                    <template
                      v-if="
                        item.maxPurchases !== -1 &&
                        item.purchaseCount >= item.maxPurchases
                      "
                    >
                      MAX
                    </template>
                    <template v-else>
                      {{ item.currentPrice }}
                      <i class="nes-icon like is-small gem-icon"></i>
                    </template>
                  </button>
                </div>
              </div>
            </div>

            <!-- Purchase Message -->
            <div
              v-if="shopMessage"
              class="shop-message"
              :class="shopMessageClass"
            >
              {{ shopMessage }}
            </div>
          </div>
        </div>
      </div>
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
import "nes.css/css/nes.min.css";
import { Dooble, type DoobleType } from "~/scripts/Dooble";
import { gameState } from "~/scripts/GameState";
import { shopManager, type ShopItem } from "~/scripts/ShopSystem";

const bgMusic = ref<HTMLAudioElement | null>(null);

function getMusicVolume(): number {
  try {
    const saved = localStorage.getItem("dooble_music_volume");
    return saved ? parseInt(saved, 10) / 100 : 0.5;
  } catch {
    return 0.5;
  }
}

const router = useRouter();

// Game constants
const DOOBLE_SPAWN_TIME_MS = 15000; // Time in milliseconds before a new dooble spawns
const FEED_AMOUNT = 100; // How much feeding reduces hunger
const STAT_DECAY_INTERVAL_MS = 1000; // How often stats decay (every 1 second)
const BASE_HUNGER_DECAY = 5.0; // Base amount hunger increases per interval
const AGE_DECAY_MULTIPLIER = 0.02; // How much age affects decay rate (higher = faster decay when older)
const AGE_INCREMENT_INTERVAL_MS = 24 * 60 * 60 * 1000; // 1 real-world day (24 hours)

// Game World text visibility
const showGameWorldText = ref(true);

// Animation frames
const idleFrames = [
  "/images/Animations/IdleFrames/character_2.png",
  "/images/Animations/IdleFrames/character_3.png",
  "/images/Animations/IdleFrames/character_4.png",
];

const walkingFrames = [
  "/images/Animations/WalkingFrames/character_5.png",
  "/images/Animations/WalkingFrames/character_6.png",
  "/images/Animations/WalkingFrames/character_7.png",
  "/images/Animations/WalkingFrames/character_8.png",
];

const bloobleIdleFrames = [
  "/images/Animations/BloobleIdleFrames/character_2.png",
  "/images/Animations/BloobleIdleFrames/character_3.png",
  "/images/Animations/BloobleIdleFrames/character_4.png",
];

const bloobleWalkingFrames = [
  "/images/Animations/BloobleWalkingFrames/character_5.png",
  "/images/Animations/BloobleWalkingFrames/character_6.png",
  "/images/Animations/BloobleWalkingFrames/character_7.png",
  "/images/Animations/BloobleWalkingFrames/character_8.png",
];

// Funny sounding names for automatic naming
const funnyNames = [
  "Blibber",
  "Zorp",
  "Flibberty",
  "Wobblekins",
  "Squiggle",
  "Boing",
  "Zizzle",
  "Flapjack",
  "Wibbly",
  "Zog",
  "Blorp",
  "Fizwidget",
  "Gloop",
  "Noodle",
  "Pogo",
  "Quibble",
  "Razzle",
  "Snicker",
  "Toodle",
  "Wizzle",
  "Zizzer",
  "Bloop",
  "Fizzle",
  "Gobble",
  "Jibber",
  "Kablooey",
  "Lollygag",
  "Mizzle",
  "Nibble",
  "Oodle",
];

// Get a random funny name (fallback)
function getRandomFunnyName(): string {
  return funnyNames[Math.floor(Math.random() * funnyNames.length)]!;
}

// Fetch a dooble name from the Azure API
async function fetchDoobleNameFromAPI(): Promise<string> {
  try {
    const response = await fetch(
      "https://doobleapi.azurewebsites.net/dooble/dooblename",
    );
    if (!response.ok) throw new Error("API request failed");
    const name = await response.text();
    return name.trim() || getRandomFunnyName();
  } catch (error) {
    console.warn(
      "Failed to fetch dooble name from API, using fallback:",
      error,
    );
    return getRandomFunnyName();
  }
}

// Dooble visual state interface
interface DoobleVisual {
  id: number;
  dooble: Dooble;
  x: number;
  y: number;
  targetX: number;
  targetY: number;
  facingDirection: number;
  state: "idle" | "walking" | "spawning";
  frameIndex: number;
  animationInterval: ReturnType<typeof setInterval> | null;
  movementInterval: ReturnType<typeof setInterval> | null;
  behaviorTimeout: ReturnType<typeof setTimeout> | null;
  spawnTimer: ReturnType<typeof setTimeout> | null;
  isUnnamed: boolean;
  defaultNameTimer: ReturnType<typeof setTimeout> | null;
}

// Doobles collection
const doobleVisuals = shallowRef<DoobleVisual[]>([]);
const gameContainer = ref<HTMLElement | null>(null);
let nextDoobleId = 0;

// Force re-render helper
function triggerUpdate() {
  doobleVisuals.value = [...doobleVisuals.value];
}

// Start spawn timer for a dooble
function startSpawnTimer(visual: DoobleVisual) {
  if (visual.spawnTimer) clearTimeout(visual.spawnTimer);
  const spawnMultiplier = gameState.getSpawnRateMultiplier();
  const baseTime = DOOBLE_SPAWN_TIME_MS * spawnMultiplier;
  const variation = (Math.random() - 0.5) * 0.4 * baseTime;
  const spawnTime = Math.max(2000, baseTime + variation);
  visual.spawnTimer = setTimeout(() => {
    triggerSpawnAnimation(visual);
  }, spawnTime);
}

// Placeholder animation before spawning
function triggerSpawnAnimation(visual: DoobleVisual) {
  if (!doobleVisuals.value.some((d) => d.id === visual.id)) return;

  // Only spawn when idle — if walking, retry after a short delay
  if (visual.state !== "idle") {
    setTimeout(() => triggerSpawnAnimation(visual), 1000);
    return;
  }

  visual.state = "spawning";
  triggerUpdate();
  setTimeout(() => {
    spawnCopyOfDooble(visual);
  }, 1500);
}

// Spawn a copy of the dooble
async function spawnCopyOfDooble(visual: DoobleVisual) {
  if (!doobleVisuals.value.some((d) => d.id === visual.id)) return;

  const name = await fetchDoobleNameFromAPI();
  const newDooble = new Dooble(name, visual.dooble.type);
  newDooble.stats = { hunger: 0, age: 0 };
  // Spawn near the parent
  const newVisual = createDoobleVisual(newDooble, visual.x, visual.y);
  newVisual.isUnnamed = true;
  newVisual.defaultNameTimer = setTimeout(() => {
    newVisual.isUnnamed = false;
    newVisual.defaultNameTimer = null;
    triggerUpdate();
  }, 5000);
  doobleVisuals.value.push(newVisual);
  startIdleBehavior(newVisual);
  startSpawnTimer(newVisual);

  visual.state = "idle";
  triggerUpdate();

  // Lose condition: too many doobles
  const maxPop = gameState.getMaxPopulation();
  if (doobleVisuals.value.length > maxPop) {
    triggerGameOver("too_many");
  }

  checkPopulationWarning();
}

// Selected dooble for stats panel
const selectedDoobleId = ref<number | null>(null);
const selectedDooble = computed(() =>
  selectedDoobleId.value !== null
    ? doobleVisuals.value.find((d) => d.id === selectedDoobleId.value) || null
    : null,
);

// Feed mini-game modal state
const showFeedModal = ref(false);
const feedTargetId = ref<number | null>(null);

// ============================================
// FOOD SYSTEM
// ============================================
const FOOD_RECHARGE_MS = 60 * 4000;

interface FoodSlot {
  id: number;
  available: boolean;
  rechargeAt: number | null;
}

const foodSlots = ref<FoodSlot[]>(
  Array.from({ length: gameState.getTotalFoodSlotCount() }, (_, i) => ({
    id: i,
    available: true,
    rechargeAt: null,
  })),
);

// Count available foods
const availableFoodCount = computed(
  () => foodSlots.value.filter((s) => s.available).length,
);

// Currency
const coins = ref(0);
const gems = ref(0);
const DOOBLE_MAX_AGE = 10; // Days before dying of old age
const BLOOBLE_MAX_AGE = 20; // Bloobles live longer

function getMaxAge(doobleType: DoobleType): number {
  return doobleType === "blooble" ? BLOOBLE_MAX_AGE : DOOBLE_MAX_AGE;
}

// Death notification state
const showDeathModal = ref(false);
const deathMessage = ref("");
const deathReward = ref<{ type: "coin" | "gem"; amount: number } | null>(null);

// Game over state
const showGameOverModal = ref(false);
const gameOverReason = ref<"too_many" | "all_gone">("all_gone");

// Win state
const showWinModal = ref(false);

// Population warning
const showPopulationWarning = ref(false);

// Shop
const showShopModal = ref(false);
const activeShopTab = ref<"coins" | "gems">("coins");
const shopMessage = ref<string | null>(null);
const shopMessageClass = ref("");
const purchaseCounter = ref(0); // Force reactivity updates

// Computed shop items with current prices and purchase status
const coinShopItems = computed(() => {
  purchaseCounter.value; // Dependency for reactivity
  coins.value; // Depend on currency changes
  return shopManager.getCoinShopItems();
});
const gemShopItems = computed(() => {
  purchaseCounter.value; // Dependency for reactivity
  gems.value; // Depend on currency changes
  return shopManager.getGemShopItems();
});

function openShop(tab: "coins" | "gems" = "coins") {
  activeShopTab.value = tab;
  showShopModal.value = true;
  shopMessage.value = null;
}

function closeShop() {
  showShopModal.value = false;
  shopMessage.value = null;
}

function purchaseItem(
  item: ShopItem & {
    currentPrice: number;
    purchaseCount: number;
    canPurchase: boolean;
  },
) {
  const result = shopManager.purchase(item);

  if (result.success) {
    shopMessage.value = result.message;
    shopMessageClass.value = "is-success";

    // Sync local reactive values with gameState
    const currency = gameState.getCurrency();
    coins.value = currency.coins;
    gems.value = currency.gems;

    // Force reactivity update for shop items
    purchaseCounter.value++;

    // Update food slots if extra hearts purchased
    if (item.id === "extraFoodHearts") {
      updateFoodSlotCount();
    }

    // Spawn first Blooble if unlockBloobles purchased
    if (item.id === "unlockBloobles") {
      spawnBlooble();
    }

    // Win condition: trophy purchased
    if (item.id === "trophy") {
      showWinModal.value = true;
      cleanupAllDoobles();
    }

    // Clear message after delay
    setTimeout(() => {
      shopMessage.value = null;
    }, 2000);
  } else {
    shopMessage.value = result.message;
    shopMessageClass.value = "is-error";
    setTimeout(() => {
      shopMessage.value = null;
    }, 2000);
  }
}

// Spawn a blooble helper
async function spawnBlooble() {
  const name = await fetchDoobleNameFromAPI();
  const blooble = new Dooble(name, "blooble");
  blooble.stats = { hunger: 0, age: 0 };
  const visual = createDoobleVisual(blooble, 100, 100);
  visual.isUnnamed = true;
  visual.defaultNameTimer = setTimeout(() => {
    visual.isUnnamed = false;
    visual.defaultNameTimer = null;
    triggerUpdate();
  }, 5000);
  doobleVisuals.value.push(visual);
  startIdleBehavior(visual);
  startSpawnTimer(visual);
}

// Sacrifice the selected dooble
function sacrificeDooble() {
  if (!selectedDooble.value) return;
  const visual = selectedDooble.value;
  const name = visual.dooble.name;
  const isBlooble = visual.dooble.type === "blooble";
  const coinAmount = isBlooble ? 3 : 1;
  gameState.addCoins(coinAmount);
  coins.value = gameState.getCurrency().coins;
  deathMessage.value = `${name}${isBlooble ? " the Blooble" : ""} was sacrificed... A necessary evil.`;
  deathReward.value = { type: "coin", amount: coinAmount };
  gameState.saveToLocal();
  removeDooble(visual.id);
  showDeathModal.value = true;
  setTimeout(() => {
    showDeathModal.value = false;
    closeDeathModal();
  }, 2300);
}

// Dismiss win modal
function dismissWin() {
  showWinModal.value = false;
  gameState.clearLocal();
  router.push("/");
}

// Check population warning
function checkPopulationWarning() {
  const maxPop = gameState.getMaxPopulation();
  const warningThreshold = Math.floor(maxPop * 0.85);
  showPopulationWarning.value =
    doobleVisuals.value.length >= warningThreshold &&
    doobleVisuals.value.length <= maxPop;
}

// Update food slot count based on upgrades
function updateFoodSlotCount() {
  const totalSlots = gameState.getTotalFoodSlotCount();
  const currentSlots = foodSlots.value.length;

  if (totalSlots > currentSlots) {
    // Add new slots
    for (let i = currentSlots; i < totalSlots; i++) {
      foodSlots.value.push({
        id: i,
        available: true,
        rechargeAt: null,
      });
    }
  }
}

function closeDeathModal() {
  showDeathModal.value = false;
  deathMessage.value = "";
  deathReward.value = null;
}

// Trigger game over
function triggerGameOver(reason: "too_many" | "all_gone") {
  gameOverReason.value = reason;
  showGameOverModal.value = true;
  cleanupAllDoobles();
}

function dismissGameOver() {
  showGameOverModal.value = false;
  gameState.resetForGameOver();
  router.push("/");
}

// Remove a dooble (clean up intervals and remove from array)
function removeDooble(visualId: number) {
  const index = doobleVisuals.value.findIndex((d) => d.id === visualId);
  if (index === -1) return;

  const visual = doobleVisuals.value[index];
  if (visual) {
    if (visual.animationInterval) clearInterval(visual.animationInterval);
    if (visual.movementInterval) clearInterval(visual.movementInterval);
    if (visual.behaviorTimeout) clearTimeout(visual.behaviorTimeout);
    if (visual.spawnTimer) clearTimeout(visual.spawnTimer);
    if (visual.defaultNameTimer) clearTimeout(visual.defaultNameTimer);
  }

  // Clear selection if this dooble was selected
  if (selectedDoobleId.value === visualId) {
    selectedDoobleId.value = null;
  }

  doobleVisuals.value.splice(index, 1);
  triggerUpdate();

  // Lose condition: all doobles gone
  if (
    doobleVisuals.value.length === 0 &&
    !showGameOverModal.value &&
    !showWinModal.value
  ) {
    triggerGameOver("all_gone");
  }
}

// Handle dooble death
function handleDeath(visual: DoobleVisual, reason: "starvation" | "old_age") {
  const name = visual.dooble.name;
  const age = visual.dooble.stats.age;
  const isBlooble = visual.dooble.type === "blooble";

  // Record death in game state
  gameState.recordDoobleDeath(age);

  if (reason === "old_age") {
    if (isBlooble) {
      gameState.addCoins(5);
      coins.value = gameState.getCurrency().coins;
      deathMessage.value = `${name} the Blooble lived a full life and passed away peacefully at ${age} days old.`;
      deathReward.value = { type: "coin", amount: 5 };
    } else {
      gameState.addGems(1);
      gems.value = gameState.getCurrency().gems;
      deathMessage.value = `${name} lived a full life and passed away peacefully at ${age} days old.`;
      deathReward.value = { type: "gem", amount: 1 };
    }
  } else {
    const coinAmount = isBlooble ? 5 : 1;
    gameState.addCoins(coinAmount);
    coins.value = gameState.getCurrency().coins;
    deathMessage.value = `${name}${isBlooble ? " the Blooble" : ""} Starved to death... You Are A Horrible Person!`;
    deathReward.value = { type: "coin", amount: coinAmount };
  }

  // Save game state
  gameState.saveToLocal();

  removeDooble(visual.id);
  showDeathModal.value = true;

  // Auto-dismiss after 2.3 seconds (2s display + 0.3s slide out animation)
  setTimeout(() => {
    showDeathModal.value = false;
    closeDeathModal();
  }, 2300);
}

// Get remaining time string for a slot (MM:SS)
function getSlotTimer(slot: FoodSlot): string {
  if (slot.available || slot.rechargeAt === null) return "";
  const remaining = Math.max(0, slot.rechargeAt - Date.now());
  const secs = Math.floor(remaining / 1000);
  const mins = Math.floor(secs / 60);
  const s = secs % 60;
  return `${mins}:${String(s).padStart(2, "0")}`;
}

// Consume one food (find first available slot and mark it recharging)
function consumeFood(): boolean {
  const slot = foodSlots.value.find((s) => s.available);
  if (!slot) return false;
  slot.available = false;
  // Apply recharge multiplier from upgrades
  const rechargeTime = FOOD_RECHARGE_MS * gameState.getFoodRechargeMultiplier();
  slot.rechargeAt = Date.now() + rechargeTime;
  return true;
}

// Recharge timer - runs every second
let foodRechargeTimer: ReturnType<typeof setInterval> | null = null;

function tickFoodRecharge() {
  const now = Date.now();
  for (const slot of foodSlots.value) {
    if (!slot.available && slot.rechargeAt !== null && now >= slot.rechargeAt) {
      slot.available = true;
      slot.rechargeAt = null;
    }
  }
}

// Feed mini-game runtime state
const indicatorX = ref(0); // percent position of indicator (0-100)
const indicatorSpeed = ref(2.0); // percent per tick (faster!)
const indicatorDirection = ref(1); // 1 = right, -1 = left
const indicatorStopped = ref(false);
const sweetSpotStart = ref(40); // start of green zone
const sweetSpotWidth = ref(12); // width of green zone (smaller!)
const maxAttempts = ref(2); // only 2 attempts!
const attemptsLeft = ref(2);
const feedMessage = ref<string | null>(null);
const feedMessageClass = ref("");
let indicatorInterval: ReturnType<typeof setInterval> | null = null;

function startIndicatorMovement() {
  stopIndicatorMovement();
  indicatorInterval = setInterval(() => {
    if (indicatorStopped.value) return;

    indicatorX.value += indicatorSpeed.value * indicatorDirection.value;

    // Bounce off edges
    if (indicatorX.value >= 100) {
      indicatorX.value = 100;
      indicatorDirection.value = -1;
    } else if (indicatorX.value <= 0) {
      indicatorX.value = 0;
      indicatorDirection.value = 1;
    }
  }, 16);
}

function stopIndicatorMovement() {
  if (indicatorInterval) {
    clearInterval(indicatorInterval);
    indicatorInterval = null;
  }
}

function resetMiniGameState() {
  indicatorX.value = 0;
  indicatorDirection.value = 1;
  indicatorStopped.value = false;
  attemptsLeft.value = maxAttempts.value;
  feedMessage.value = null;
  feedMessageClass.value = "";

  // Randomize sweet spot position and width for variety (harder!)
  sweetSpotWidth.value = 8 + Math.floor(Math.random() * 6); // 8-14% (much smaller)
  sweetSpotStart.value =
    10 + Math.floor(Math.random() * (80 - sweetSpotWidth.value)); // can appear anywhere
}

function attemptFeed() {
  if (indicatorStopped.value || attemptsLeft.value <= 0) return;

  indicatorStopped.value = true;

  // Check if indicator is in sweet spot
  const indicatorPos = indicatorX.value;
  const sweetEnd = sweetSpotStart.value + sweetSpotWidth.value;

  if (indicatorPos >= sweetSpotStart.value && indicatorPos <= sweetEnd) {
    // Success!
    feedMessage.value = "🎉 Perfect! Dooble fed!";
    feedMessageClass.value = "is-success";
    setTimeout(() => {
      confirmFeed();
    }, 800);
  } else {
    // Miss
    attemptsLeft.value--;
    if (attemptsLeft.value > 0) {
      feedMessage.value = `❌ Missed! ${attemptsLeft.value} attempts left`;
      feedMessageClass.value = "is-warning";
    } else {
      feedMessage.value = "💔 Out of attempts!";
      feedMessageClass.value = "is-error";
      setTimeout(() => {
        cancelFeed();
      }, 1200);
    }
  }
}

function retryAttempt() {
  indicatorStopped.value = false;
  feedMessage.value = null;
  feedMessageClass.value = "";
  // Increase speed significantly for more challenge
  indicatorSpeed.value = Math.min(4.0, indicatorSpeed.value + 0.5);
  // Also shrink sweet spot on retry!
  sweetSpotWidth.value = Math.max(5, sweetSpotWidth.value - 2);
}

// Handle keyboard input for the mini-game
function handleKeydown(e: KeyboardEvent) {
  if (showFeedModal.value && e.code === "Space") {
    e.preventDefault();
    if (indicatorStopped.value && attemptsLeft.value > 0) {
      retryAttempt();
    } else {
      attemptFeed();
    }
  }
}

// start/stop movement when modal opens/closes
watch(
  () => showFeedModal.value,
  (open) => {
    if (open) {
      resetMiniGameState();
      indicatorSpeed.value = 2.0; // reset speed (faster base!)
      startIndicatorMovement();
      window.addEventListener("keydown", handleKeydown);
    } else {
      stopIndicatorMovement();
      window.removeEventListener("keydown", handleKeydown);
    }
  },
);

// New dooble modal
const showNameDoobleModal = ref(false);
const newDoobleName = ref("");
const pendingNameDooble = ref<DoobleVisual | null>(null);
const pendingSpawnPosition = ref<{ x: number; y: number } | null>(null);

// Stat decay and age timers
let statDecayTimer: ReturnType<typeof setInterval> | null = null;
let ageTimer: ReturnType<typeof setInterval> | null = null;

const moveSpeed = 2;

// Get frame for a dooble based on its state
function getDoobleFrame(visual: DoobleVisual): string {
  const isBlooble = visual.dooble.type === "blooble";

  switch (visual.state) {
    case "walking":
      if (isBlooble) {
        return (
          bloobleWalkingFrames[
            visual.frameIndex % bloobleWalkingFrames.length
          ] ?? bloobleIdleFrames[0]!
        );
      }
      return (
        walkingFrames[visual.frameIndex % walkingFrames.length] ??
        idleFrames[0]!
      );
    case "spawning":
      // Use idle frames during spawn animation (CSS handles the glow)
      if (isBlooble) {
        return (
          bloobleIdleFrames[visual.frameIndex % bloobleIdleFrames.length] ??
          bloobleIdleFrames[0]!
        );
      }
      return (
        idleFrames[visual.frameIndex % idleFrames.length] ?? idleFrames[0]!
      );
    default:
      if (isBlooble) {
        return (
          bloobleIdleFrames[visual.frameIndex % bloobleIdleFrames.length] ??
          bloobleIdleFrames[0]!
        );
      }
      return (
        idleFrames[visual.frameIndex % idleFrames.length] ?? idleFrames[0]!
      );
  }
}

// Progress bar color classes
function getHungerClass(hunger: number): string {
  const fullness = 100 - hunger;
  if (fullness > 60) return "is-success";
  if (fullness > 30) return "is-warning";
  return "is-error";
}

// Create a new dooble visual
function createDoobleVisual(
  dooble: Dooble,
  x: number = 0,
  y: number = 0,
): DoobleVisual {
  return {
    id: nextDoobleId++,
    dooble,
    x,
    y,
    targetX: 0,
    targetY: 0,
    facingDirection: 1,
    state: "idle",
    frameIndex: 0,
    animationInterval: null,
    movementInterval: null,
    behaviorTimeout: null,
    spawnTimer: null,
    isUnnamed: false,
    defaultNameTimer: null,
  };
}

// Start animation for a specific dooble
function startAnimation(visual: DoobleVisual, frameRate: number = 200) {
  if (visual.animationInterval) clearInterval(visual.animationInterval);
  visual.frameIndex = 0;
  visual.animationInterval = setInterval(() => {
    visual.frameIndex++;
    triggerUpdate();
  }, frameRate);
}

// Start idle behavior for a specific dooble
function startIdleBehavior(visual: DoobleVisual) {
  visual.state = "idle";
  startAnimation(visual, 200);

  const nextActionDelay = 2000 + Math.random() * 3000;
  visual.behaviorTimeout = setTimeout(() => {
    startWalking(visual);
  }, nextActionDelay);
}

// Start walking for a specific dooble
function startWalking(visual: DoobleVisual) {
  if (!gameContainer.value) return;

  visual.state = "walking";
  startAnimation(visual, 150);

  const bounds = gameContainer.value.getBoundingClientRect();
  const margin = 100;
  const maxX = bounds.width / 2 - margin;
  const maxY = bounds.height / 2 - margin;

  visual.targetX = (Math.random() * 2 - 1) * maxX;
  visual.targetY = (Math.random() * 2 - 1) * maxY;

  if (visual.targetX > visual.x) {
    visual.facingDirection = -1;
  } else if (visual.targetX < visual.x) {
    visual.facingDirection = 1;
  }

  if (visual.movementInterval) clearInterval(visual.movementInterval);
  visual.movementInterval = setInterval(() => {
    const dx = visual.targetX - visual.x;
    const dy = visual.targetY - visual.y;
    const distance = Math.sqrt(dx * dx + dy * dy);

    if (distance < moveSpeed) {
      visual.x = visual.targetX;
      visual.y = visual.targetY;
      if (visual.movementInterval) clearInterval(visual.movementInterval);
      visual.movementInterval = null;
      triggerUpdate();
      startIdleBehavior(visual);
    } else {
      visual.x += (dx / distance) * moveSpeed;
      visual.y += (dy / distance) * moveSpeed;
      triggerUpdate();
    }
  }, 16);
}

// Select a dooble to show stats
function selectDooble(id: number) {
  const visual = doobleVisuals.value.find((d) => d.id === id);
  if (!visual) return;

  // Dismiss exclamation mark on click
  if (visual.isUnnamed) {
    visual.isUnnamed = false;
    if (visual.defaultNameTimer) {
      clearTimeout(visual.defaultNameTimer);
      visual.defaultNameTimer = null;
    }
    triggerUpdate();
  }

  selectedDoobleId.value = id;
}

// Feed selected dooble
function feedDooble() {
  // Open feed mini-game modal instead of immediately feeding
  if (selectedDoobleId.value !== null) {
    feedTargetId.value = selectedDoobleId.value;
    showFeedModal.value = true;
  }
}

// Confirm feeding after mini-game completes
function confirmFeed() {
  if (feedTargetId.value !== null) {
    // Try to consume a food slot
    if (!consumeFood()) {
      feedMessage.value = "No food available!";
      setTimeout(() => {
        feedMessage.value = null;
        showFeedModal.value = false;
        feedTargetId.value = null;
      }, 900);
      return;
    }

    const visual = doobleVisuals.value.find((d) => d.id === feedTargetId.value);
    if (visual) {
      visual.dooble.feed(FEED_AMOUNT);
      gameState.recordFoodFed();
      triggerUpdate();
    }
  }
  showFeedModal.value = false;
  feedTargetId.value = null;
}

function cancelFeed() {
  showFeedModal.value = false;
  feedTargetId.value = null;
}

// Spawn a new dooble (shows modal)
function spawnNewDooble() {
  pendingNameDooble.value = null;
  showNameDoobleModal.value = true;
  newDoobleName.value = "";

  // Pre-calculate spawn position
  let spawnX = 0;
  let spawnY = 0;
  if (gameContainer.value) {
    const bounds = gameContainer.value.getBoundingClientRect();
    const margin = 100;
    const maxX = bounds.width / 2 - margin;
    const maxY = bounds.height / 2 - margin;
    spawnX = (Math.random() * 2 - 1) * maxX;
    spawnY = (Math.random() * 2 - 1) * maxY;
  }

  pendingSpawnPosition.value = { x: spawnX, y: spawnY };
}

// Confirm dooble name
async function confirmDoobleName() {
  const name = newDoobleName.value.trim() || (await fetchDoobleNameFromAPI());

  if (pendingNameDooble.value) {
    // Naming an existing unnamed dooble
    pendingNameDooble.value.dooble.name = name;
    pendingNameDooble.value.isUnnamed = false;
    if (pendingNameDooble.value.defaultNameTimer) {
      clearTimeout(pendingNameDooble.value.defaultNameTimer);
      pendingNameDooble.value.defaultNameTimer = null;
    }
    triggerUpdate();
  } else {
    // Initial spawn
    const newDooble = new Dooble(name);
    gameState.recordDoobleHatched();
    const visual = createDoobleVisual(
      newDooble,
      pendingSpawnPosition.value?.x || 0,
      pendingSpawnPosition.value?.y || 0,
    );
    doobleVisuals.value.push(visual);
    startIdleBehavior(visual);
    startSpawnTimer(visual);
  }

  showNameDoobleModal.value = false;
  newDoobleName.value = "";
  pendingNameDooble.value = null;
  pendingSpawnPosition.value = null;
}

// Cleanup all dooble intervals
function cleanupAllDoobles() {
  for (const visual of doobleVisuals.value) {
    if (visual.animationInterval) clearInterval(visual.animationInterval);
    if (visual.movementInterval) clearInterval(visual.movementInterval);
    if (visual.behaviorTimeout) clearTimeout(visual.behaviorTimeout);
    if (visual.spawnTimer) clearTimeout(visual.spawnTimer);
    if (visual.defaultNameTimer) clearTimeout(visual.defaultNameTimer);
  }
}

// Decay stats for all doobles (scales with age)
function decayAllStats() {
  // Collect doobles that died this tick
  const deadDoobles: DoobleVisual[] = [];

  // Get upgrade multipliers
  const hungerMultiplier = gameState.getHungerDecayMultiplier();

  for (const visual of doobleVisuals.value) {
    const dooble = visual.dooble;
    const age = dooble.stats.age;

    // Calculate decay multiplier based on age (older = faster decay)
    const ageMultiplier = 1 + age * AGE_DECAY_MULTIPLIER;

    // Increase hunger (0 = full, 100 = starving) - apply upgrade reduction
    const hungerDecay = BASE_HUNGER_DECAY * ageMultiplier * hungerMultiplier;
    dooble.stats.hunger = Math.min(100, dooble.stats.hunger + hungerDecay);

    // Check for death by starvation
    if (dooble.stats.hunger >= 100) {
      deadDoobles.push(visual);
    }
  }

  // Handle all deaths (removeDooble handles the all-gone check)
  for (const dead of deadDoobles) {
    handleDeath(dead, "starvation");
  }

  checkPopulationWarning();
  triggerUpdate();
}

// Increment age for all doobles
function incrementAllAges() {
  const oldAgeDoobles: DoobleVisual[] = [];

  for (const visual of doobleVisuals.value) {
    visual.dooble.stats.age++;
    if (visual.dooble.stats.age >= getMaxAge(visual.dooble.type)) {
      oldAgeDoobles.push(visual);
    }
  }

  for (const visual of oldAgeDoobles) {
    handleDeath(visual, "old_age");
  }

  triggerUpdate();
}

// Shared cleanup for all timers and intervals
function cleanup() {
  gameState.saveToLocal();
  cleanupAllDoobles();
  if (statDecayTimer) clearInterval(statDecayTimer);
  if (ageTimer) clearInterval(ageTimer);
  if (foodRechargeTimer) clearInterval(foodRechargeTimer);
}

onMounted(() => {
  // Load saved game state
  gameState.loadFromLocal();
  const savedCurrency = gameState.getCurrency();
  coins.value = savedCurrency.coins;
  gems.value = savedCurrency.gems;

  // Update food slots based on upgrades
  updateFoodSlotCount();

  // Start background music
  if (bgMusic.value) {
    bgMusic.value.volume = getMusicVolume();
    bgMusic.value.play().catch(() => {});
  }

  // Hide "Game World" text after 3 seconds
  setTimeout(() => {
    showGameWorldText.value = false;
  }, 3000);

  // Show modal to name the first dooble
  pendingSpawnPosition.value = { x: 0, y: 0 };
  pendingNameDooble.value = null;
  showNameDoobleModal.value = true;

  // Auto-spawn a blooble if unlocked from a previous game
  if (gameState.getUpgrades().unlockBloobles) {
    setTimeout(() => spawnBlooble(), 500);
  }

  // Start stat decay timer
  statDecayTimer = setInterval(decayAllStats, STAT_DECAY_INTERVAL_MS);

  // Start age increment timer
  ageTimer = setInterval(incrementAllAges, AGE_INCREMENT_INTERVAL_MS);

  // Start food recharge timer (ticks every second)
  foodRechargeTimer = setInterval(tickFoodRecharge, 1000);
});

onUnmounted(cleanup);

function goBack() {
  cleanup();
  router.push("/");
}
</script>

<style scoped>
.game-area {
  min-height: 100vh;
  position: relative;
  overflow: hidden;
}

.game-background {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  overflow: hidden;
}

.game-bg-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  image-rendering: pixelated;
  image-rendering: crisp-edges;
}

.game-content {
  position: relative;
  z-index: 10;
  width: 100%;
  height: 100vh;
  display: flex;
  flex-direction: column;
  padding: 20px;
}

.back-btn {
  position: absolute;
  top: 20px;
  left: 20px;
  font-size: 0.9rem !important;
  z-index: 1000;
  pointer-events: auto;
}

.game-container {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
}

.game-text {
  font-family: "Press Start 2P", cursive;
  font-size: 2rem;
  color: #fff;
  text-shadow: 2px 2px 0 #000;
  position: absolute;
}

.fade-out {
  animation: fadeOut 1s ease-out 2s forwards;
}

@keyframes fadeOut {
  from {
    opacity: 1;
  }
  to {
    opacity: 0;
  }
}

/* Dooble character */

.dooble {
  position: absolute;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  /* No transition for hard flip */
  cursor: pointer;
}

.dooble:hover {
  filter: brightness(1.2);
}

.dooble-sprite {
  width: 128px;
  height: 128px;
  image-rendering: pixelated;
  image-rendering: crisp-edges;
}

.dooble.spawning {
  animation: spawnGlow 1.5s ease-in-out;
}

@keyframes spawnGlow {
  0% {
    filter: brightness(1);
  }
  50% {
    filter: brightness(2) drop-shadow(0 0 12px #ffd93d);
  }
  100% {
    filter: brightness(1);
  }
}

.exclamation-mark {
  font-family: "Press Start 2P", cursive;
  font-size: 1.5rem;
  color: #ffd93d;
  text-shadow: 2px 2px 0 #000;
  position: absolute;
  top: -20px;
  animation: bounce 1s infinite;
}

@keyframes bounce {
  0%,
  20%,
  50%,
  80%,
  100% {
    transform: translateY(0);
  }
  40% {
    transform: translateY(-10px);
  }
  60% {
    transform: translateY(-5px);
  }
}

/* Stats Panel */
.stats-panel {
  position: absolute;
  bottom: 20px;
  right: 20px;
  width: 320px;
  padding: 20px !important;
  z-index: 100;
}

.close-btn {
  position: absolute;
  top: 8px;
  right: 8px;
  padding: 4px 10px !important;
  font-size: 1rem !important;
  min-width: auto;
}

.stats-title {
  font-family: "Press Start 2P", cursive;
  font-size: 1rem;
  color: #ffd93d;
  margin-bottom: 16px;
  text-align: center;
}

.stat-row {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 12px;
}

.stat-row label {
  font-family: "Press Start 2P", cursive;
  font-size: 0.6rem;
  min-width: 80px;
  color: #fff;
}

.stat-row progress {
  flex: 1;
  height: 20px;
  min-width: 100px;
}

.stat-row :deep(.nes-progress) {
  height: 20px;
}

.stat-value {
  font-family: "Press Start 2P", cursive;
  font-size: 0.5rem;
  min-width: 40px;
  text-align: right;
  color: #fff;
}

.action-buttons {
  display: flex;
  gap: 10px;
  margin-top: 16px;
  justify-content: center;
}

.action-buttons button {
  font-size: 0.7rem !important;
  padding: 8px 16px !important;
}

/* Modal */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.8);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2000;
}

.modal-content {
  max-width: 400px;
  padding: 30px !important;
  text-align: center;
}

.modal-title {
  font-family: "Press Start 2P", cursive;
  font-size: 1rem;
  color: #ffd93d;
  margin-bottom: 20px;
}

.modal-text {
  font-family: "Press Start 2P", cursive;
  font-size: 0.7rem;
  color: #fff;
  margin-bottom: 16px;
}

.modal-content input {
  margin-bottom: 20px;
}

.modal-btn {
  font-size: 0.8rem !important;
}

/* Mini-game styles */
.mini-game {
  max-width: 500px;
  width: 90%;
}

.mini-game-instructions {
  font-size: 0.6rem !important;
  margin-bottom: 16px !important;
}

.power-bar-container {
  margin: 20px auto;
  width: 100%;
  cursor: pointer;
}

.power-bar {
  position: relative;
  height: 40px;
  background: #222;
  border: 4px solid #fff;
  border-radius: 4px;
  overflow: hidden;
}

.sweet-spot {
  position: absolute;
  top: 0;
  height: 100%;
  background: linear-gradient(180deg, #4ade80 0%, #22c55e 50%, #16a34a 100%);
  box-shadow: 0 0 10px rgba(74, 222, 128, 0.5);
}

.indicator {
  position: absolute;
  top: 50%;
  transform: translate(-50%, -70%);
  z-index: 10;
  transition: none;
}

.indicator.stopped {
  animation: pulse 0.3s ease-in-out infinite;
}

.indicator .nes-icon.star {
  transform: scale(2);
}

@keyframes pulse {
  0%,
  100% {
    transform: translate(-50%, -70%) scale(1);
  }
  50% {
    transform: translate(-50%, -70%) scale(1.2);
  }
}

.bar-markers {
  display: flex;
  justify-content: space-between;
  margin-top: 4px;
  font-family: "Press Start 2P", cursive;
  font-size: 0.5rem;
  color: #888;
}

.attempts-display {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  margin: 16px 0;
}

.attempts-display .nes-icon.heart {
  transform: scale(1.2);
}

.feed-message-box {
  padding: 8px 16px !important;
  margin: 12px 0;
  text-align: center;
  font-family: "Press Start 2P", cursive;
  font-size: 0.7rem;
}

.feed-message-box.is-success {
  background: #166534 !important;
  color: #4ade80;
}

.feed-message-box.is-warning {
  background: #854d0e !important;
  color: #fbbf24;
}

.feed-message-box.is-error {
  background: #7f1d1d !important;
  color: #f87171;
}

.mini-game-buttons {
  display: flex;
  gap: 12px;
  justify-content: center;
  margin-top: 16px;
}

.mini-game-buttons .nes-btn {
  font-size: 0.7rem !important;
}

/* Top Right HUD Container */
.top-right-hud {
  position: absolute;
  top: 20px;
  right: 20px;
  z-index: 1000;
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 10px;
}

/* Food Bar */
.food-bar {
  display: flex;
  gap: 6px;
  background: rgba(0, 0, 0, 0.6);
  padding: 8px 12px;
  border-radius: 8px;
}

.food-slot {
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.food-slot .nes-icon.heart {
  transform: scale(1.5);
}

.food-slot.is-recharging .nes-icon.heart {
  filter: grayscale(100%) brightness(0.5);
}

.slot-timer {
  position: absolute;
  bottom: -14px;
  font-family: "Press Start 2P", cursive;
  font-size: 0.4rem;
  color: #fff;
  white-space: nowrap;
}

/* Currency Display */
.currency-display {
  display: flex;
  flex-direction: row;
  gap: 16px;
  background: rgba(0, 0, 0, 0.6);
  padding: 10px 14px;
  border-radius: 8px;
}

.currency-item {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  transition: transform 0.1s;
}

.currency-item:hover {
  transform: scale(1.05);
}

.currency-item .nes-icon {
  transform: scale(1.2);
}

.currency-item.coins .nes-icon.coin {
  filter: drop-shadow(0 0 2px #ffd700);
}

.currency-item.gems .nes-icon.like {
  filter: hue-rotate(260deg) saturate(1.5) drop-shadow(0 0 2px #a855f7);
}

.currency-value {
  font-family: "Press Start 2P", cursive;
  font-size: 0.8rem;
  color: #fff;
  text-shadow: 1px 1px 0 #000;
  min-width: 40px;
}

/* Shop Button */
.shop-btn {
  font-size: 0.7rem !important;
  padding: 8px 16px !important;
}

/* Shop Modal */
.shop-modal {
  width: 90%;
  max-width: 600px;
  max-height: 80vh;
  overflow-y: auto;
  padding: 24px !important;
  position: relative;
}

.shop-modal > .close-btn {
  position: sticky;
  top: 0;
  z-index: 10;
  margin-left: auto;
  display: block;
}

.shop-title {
  font-family: "Press Start 2P", cursive;
  font-size: 1.2rem;
  color: #ffd93d;
  text-align: center;
  margin-bottom: 20px;
}

.shop-tabs {
  display: flex;
  gap: 12px;
  justify-content: center;
  margin-bottom: 16px;
}

.shop-tabs .nes-btn {
  font-size: 0.6rem !important;
  padding: 8px 12px !important;
}

.shop-currency {
  text-align: center;
  font-family: "Press Start 2P", cursive;
  font-size: 0.8rem;
  color: #fff;
  margin-bottom: 20px;
  padding: 8px;
  background: rgba(0, 0, 0, 0.3);
  border-radius: 4px;
}

.shop-currency .nes-icon {
  transform: scale(1.2);
  vertical-align: middle;
}

.gem-icon {
  filter: hue-rotate(260deg) saturate(1.5);
}

.shop-items {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.shop-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
  border: 2px solid rgba(255, 255, 255, 0.1);
}

.shop-item.is-maxed {
  opacity: 0.6;
}

.item-icon {
  font-size: 2rem;
  width: 50px;
  text-align: center;
}

.item-info {
  flex: 1;
}

.item-name {
  font-family: "Press Start 2P", cursive;
  font-size: 0.7rem;
  color: #ffd93d;
  margin-bottom: 4px;
}

.item-description {
  font-family: "Press Start 2P", cursive;
  font-size: 0.5rem;
  color: #aaa;
  line-height: 1.4;
}

.item-level {
  font-family: "Press Start 2P", cursive;
  font-size: 0.5rem;
  color: #4ade80;
  margin-top: 4px;
}

.item-price .nes-btn {
  font-size: 0.6rem !important;
  padding: 6px 10px !important;
  white-space: nowrap;
}

.item-price .nes-icon {
  transform: scale(0.8);
  vertical-align: middle;
}

.shop-empty {
  text-align: center;
  padding: 40px 20px;
}

.shop-empty p {
  font-family: "Press Start 2P", cursive;
  font-size: 1rem;
  color: #ffd93d;
}

.shop-empty-sub {
  font-size: 0.6rem !important;
  color: #aaa !important;
  margin-top: 12px;
}

.shop-message {
  margin-top: 16px;
  padding: 10px;
  border-radius: 4px;
  text-align: center;
  font-family: "Press Start 2P", cursive;
  font-size: 0.6rem;
}

.shop-message.is-success {
  background: #166534;
  color: #4ade80;
}

.shop-message.is-error {
  background: #7f1d1d;
  color: #f87171;
}

/* Death Notification */
.death-notification {
  position: fixed;
  top: 50%;
  right: 20px;
  transform: translateY(-50%);
  z-index: 1000;
  animation:
    slideIn 0.3s ease-out,
    slideOut 0.3s ease-in 2s forwards;
}

@keyframes slideIn {
  from {
    right: -400px;
  }
  to {
    right: 20px;
  }
}

@keyframes slideOut {
  from {
    right: 20px;
    opacity: 1;
  }
  to {
    right: -400px;
    opacity: 0;
  }
}

/* Death Modal */
.death-modal {
  text-align: center;
  padding: 20px;
  width: 350px;
}

.death-title {
  font-family: "Press Start 2P", cursive;
  font-size: 1.2rem;
  color: #888;
  margin-bottom: 16px;
}

.death-message {
  font-family: "Press Start 2P", cursive;
  font-size: 0.7rem;
  color: #ccc;
  margin-bottom: 20px;
  line-height: 1.6;
}

.death-reward {
  background: rgba(0, 0, 0, 0.3);
  padding: 12px;
  border-radius: 8px;
  margin-bottom: 20px;
}

.death-reward p {
  font-family: "Press Start 2P", cursive;
  font-size: 0.6rem;
  color: #aaa;
  margin-bottom: 8px;
}

.reward-display {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
}

.reward-display .nes-icon {
  transform: scale(1.5);
}

.reward-display .nes-icon.coin {
  filter: drop-shadow(0 0 3px #ffd700);
}

.reward-display .nes-icon.like {
  filter: hue-rotate(260deg) saturate(1.5) drop-shadow(0 0 3px #a855f7);
}

.reward-amount {
  font-family: "Press Start 2P", cursive;
  font-size: 1rem;
  color: #ffd700;
  text-shadow: 1px 1px 0 #000;
}

.reward-amount.gem {
  color: #a855f7;
}

.game-over-overlay {
  z-index: 9999;
  background: rgba(0, 0, 0, 0.85);
}

.game-over-modal {
  text-align: center;
  max-width: 500px;
  width: 90%;
  animation: fadeIn 0.5s ease-in;
}

.game-over-title {
  font-family: "Press Start 2P", cursive;
  font-size: 1.5rem;
  color: #ff4444;
  margin-bottom: 16px;
}

.game-over-message {
  font-family: "Press Start 2P", cursive;
  font-size: 0.7rem;
  line-height: 1.8;
  margin-bottom: 24px;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: scale(0.8);
  }
  to {
    opacity: 1;
    transform: scale(1);
  }
}

.population-warning {
  position: fixed;
  bottom: 20px;
  left: 50%;
  transform: translateX(-50%);
  z-index: 500;
  animation: warningPulse 2s ease-in-out infinite;
}

.warning-banner {
  padding: 12px 24px !important;
  text-align: center;
  border: 2px solid #ff6b35 !important;
}

.warning-text {
  font-family: "Press Start 2P", cursive;
  font-size: 0.6rem;
  color: #ff6b35;
  line-height: 1.6;
  margin: 0;
}

@keyframes warningPulse {
  0%,
  100% {
    opacity: 1;
  }
  50% {
    opacity: 0.6;
  }
}

.win-title {
  font-family: "Press Start 2P", cursive;
  font-size: 1.5rem;
  color: #ffd700;
  margin-bottom: 16px;
  text-shadow: 0 0 10px #ffd700;
}
</style>
