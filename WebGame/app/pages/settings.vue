<template>
  <v-app>
    <v-main class="game-area d-flex align-center justify-center">
      <!-- Pixel Art Background -->
      <div class="pixel-background">
        <img src="/images/Background.png" class="pixel-bg-image" />
      </div>

      <!-- Settings Container -->
      <div class="settings-container">
        <div class="nes-container is-dark is-rounded settings-card">
          <h2 class="settings-title">⚙ Settings</h2>

          <!-- Music Volume -->
          <div class="setting-row">
            <label class="setting-label">🎵 Music Volume</label>
            <div class="volume-control">
              <input
                type="range"
                min="0"
                max="100"
                v-model.number="musicVolume"
                class="volume-slider"
                @input="updateVolume"
              />
              <span class="volume-value">{{ musicVolume }}%</span>
            </div>
          </div>

          <!-- Write a Review -->
          <div class="setting-row">
            <label class="setting-label">⭐ Write a Review</label>
            <div class="review-area">
              <div v-if="showReviewInput" class="review-form">
                <input
                  type="text"
                  class="nes-input review-input"
                  v-model="reviewerName"
                  placeholder="Your name..."
                />
                <div class="star-rating">
                  <span class="star-label">Rating:</span>
                  <span
                    v-for="star in 5"
                    :key="star"
                    class="star"
                    :class="{ filled: star <= reviewStars }"
                    @click="reviewStars = star"
                  >
                    {{ star <= reviewStars ? "★" : "☆" }}
                  </span>
                </div>
                <textarea
                  class="nes-textarea"
                  v-model="reviewText"
                  placeholder="Tell us what you think..."
                  rows="3"
                ></textarea>
              </div>
              <div class="review-buttons">
                <button
                  v-if="!showReviewInput"
                  class="nes-btn is-primary"
                  @click="showReviewInput = true"
                >
                  Write Review
                </button>
                <template v-else>
                  <button
                    class="nes-btn is-success"
                    @click="submitReview"
                    :disabled="
                      !reviewText.trim() || !reviewerName.trim() || isSubmitting
                    "
                  >
                    {{ isSubmitting ? "Sending..." : "Send" }}
                  </button>
                  <button class="nes-btn is-error" @click="cancelReview">
                    Cancel
                  </button>
                </template>
              </div>
              <p
                v-if="reviewMessage"
                class="review-message"
                :class="reviewMessageClass"
              >
                {{ reviewMessage }}
              </p>
            </div>
          </div>

          <!-- All Reviews -->
          <div class="setting-row">
            <label class="setting-label">📝 All Reviews</label>
            <div class="reviews-list">
              <button
                class="nes-btn is-primary refresh-btn"
                @click="fetchReviews"
                :disabled="isLoadingReviews"
              >
                {{ isLoadingReviews ? "Loading..." : "🔄 Refresh" }}
              </button>
              <div
                v-if="reviews.length === 0 && !isLoadingReviews"
                class="no-reviews"
              >
                No reviews yet. Be the first to write one!
              </div>
              <div
                v-for="review in reviews"
                :key="review.id"
                class="review-card nes-container is-rounded"
              >
                <div class="review-header">
                  <span class="review-author">{{ review.reviewer }}</span>
                  <span class="review-stars">
                    <span v-for="s in 5" :key="s" class="star-display">
                      {{ s <= review.stars ? "★" : "☆" }}
                    </span>
                  </span>
                </div>
                <p class="review-text">{{ review.reviewText }}</p>
              </div>
            </div>
          </div>

          <!-- Cheat Code -->
          <div class="setting-row">
            <label class="setting-label">🔑 Input a Cheat Code</label>
            <div class="cheat-area">
              <input
                type="text"
                class="nes-input cheat-input"
                v-model="cheatCode"
                placeholder="Enter code..."
                @keyup.enter="submitCheatCode"
              />
              <button class="nes-btn is-warning" @click="submitCheatCode">
                Redeem
              </button>
              <p
                v-if="cheatMessage"
                class="cheat-message"
                :class="cheatMessageClass"
              >
                {{ cheatMessage }}
              </p>
            </div>
          </div>
        </div>

        <!-- Back Button -->
        <button class="nes-btn is-error back-btn" @click="goBack">
          ← Back to Menu
        </button>
      </div>
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
import "nes.css/css/nes.min.css";
import { gameState } from "~/scripts/GameState";

const router = useRouter();

// Music volume (persisted in localStorage)
const VOLUME_KEY = "dooble_music_volume";
const savedVolume =
  typeof localStorage !== "undefined"
    ? parseInt(localStorage.getItem(VOLUME_KEY) || "50", 10)
    : 50;
const musicVolume = ref(savedVolume);

function updateVolume() {
  localStorage.setItem(VOLUME_KEY, String(musicVolume.value));
}

// Review API
const API_BASE = "https://doobleapi.azurewebsites.net";

interface Review {
  id: number;
  stars: number;
  reviewText: string;
  reviewer: string;
}

// Review form state
const showReviewInput = ref(false);
const reviewerName = ref("");
const reviewStars = ref(5);
const reviewText = ref("");
const reviewMessage = ref("");
const reviewMessageClass = ref("");
const isSubmitting = ref(false);

// Reviews list state
const reviews = ref<Review[]>([]);
const isLoadingReviews = ref(false);

// Fetch all reviews from API
async function fetchReviews() {
  isLoadingReviews.value = true;
  try {
    const response = await fetch(`${API_BASE}/review`);
    if (!response.ok) throw new Error("Failed to fetch reviews");
    reviews.value = await response.json();
  } catch (error) {
    console.error("Error fetching reviews:", error);
    reviewMessage.value = "Failed to load reviews";
    reviewMessageClass.value = "is-error";
    setTimeout(() => {
      reviewMessage.value = "";
    }, 3000);
  } finally {
    isLoadingReviews.value = false;
  }
}

// Submit a review to API
async function submitReview() {
  if (!reviewText.value.trim() || !reviewerName.value.trim()) return;

  isSubmitting.value = true;
  try {
    const response = await fetch(`${API_BASE}/review`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        stars: reviewStars.value,
        reviewText: reviewText.value.trim(),
        reviewer: reviewerName.value.trim(),
      }),
    });

    if (!response.ok) throw new Error("Failed to submit review");

    reviewMessage.value = "Thanks for your review!";
    reviewMessageClass.value = "is-success";
    resetReviewForm();
    // Refresh the reviews list
    await fetchReviews();
  } catch (error) {
    console.error("Error submitting review:", error);
    reviewMessage.value = "Failed to submit review. Please try again.";
    reviewMessageClass.value = "is-error";
  } finally {
    isSubmitting.value = false;
    setTimeout(() => {
      reviewMessage.value = "";
    }, 3000);
  }
}

function resetReviewForm() {
  reviewText.value = "";
  reviewerName.value = "";
  reviewStars.value = 5;
  showReviewInput.value = false;
}

function cancelReview() {
  resetReviewForm();
}

// Fetch reviews on mount
onMounted(() => {
  fetchReviews();
});

// Cheat codes
const cheatCode = ref("");
const cheatMessage = ref("");
const cheatMessageClass = ref("");

function submitCheatCode() {
  const code = cheatCode.value.trim();
  if (!code) return;

  if (code === "MoneyMoneyMoney") {
    gameState.loadFromLocal();
    gameState.addCoins(10000);
    gameState.saveToLocal();
    cheatMessage.value = "💰 +10,000 gold added!";
    cheatMessageClass.value = "is-success";
  } else {
    cheatMessage.value = "❌ Invalid cheat code.";
    cheatMessageClass.value = "is-error";
  }

  cheatCode.value = "";
  setTimeout(() => {
    cheatMessage.value = "";
  }, 3000);
}

function goBack() {
  router.push("/");
}
</script>

<style scoped>
.game-area {
  min-height: 100vh;
  position: relative;
  overflow: hidden;
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
}

.game-area :deep(.v-main__wrap) {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  width: 100%;
}

.pixel-background {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  overflow: hidden;
}

.pixel-bg-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  image-rendering: pixelated;
  image-rendering: crisp-edges;
}

.settings-container {
  position: relative;
  z-index: 10;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 24px;
  max-width: 550px;
  width: 90%;
}

.settings-card {
  width: 100%;
  padding: 24px !important;
}

.settings-title {
  font-family: "Press Start 2P", cursive;
  font-size: 1.3rem;
  color: #ffd93d;
  text-align: center;
  margin-bottom: 28px;
}

.setting-row {
  margin-bottom: 24px;
}

.setting-label {
  font-family: "Press Start 2P", cursive;
  font-size: 0.7rem;
  color: #fff;
  display: block;
  margin-bottom: 10px;
}

/* Volume */
.volume-control {
  display: flex;
  align-items: center;
  gap: 12px;
}

.volume-slider {
  flex: 1;
  height: 8px;
  accent-color: #ffd93d;
  cursor: pointer;
}

.volume-value {
  font-family: "Press Start 2P", cursive;
  font-size: 0.6rem;
  color: #ffd93d;
  min-width: 48px;
  text-align: right;
}

.review-message.is-success {
  color: #4ade80;
}

.review-message.is-error {
  color: #f87171;
}

/* Review Form */
.review-area {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.review-form {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.review-input {
  font-family: "Press Start 2P", cursive;
  font-size: 0.6rem;
}

.star-rating {
  display: flex;
  align-items: center;
  gap: 8px;
}

.star-label {
  font-family: "Press Start 2P", cursive;
  font-size: 0.6rem;
  color: #fff;
}

.star {
  font-size: 1.5rem;
  cursor: pointer;
  color: #666;
  transition: color 0.2s;
}

.star.filled {
  color: #ffd93d;
}

.star:hover {
  color: #ffd93d;
}

.review-area .nes-textarea {
  font-family: "Press Start 2P", cursive;
  font-size: 0.6rem;
  resize: vertical;
}

.review-buttons {
  display: flex;
  gap: 10px;
}

.review-buttons .nes-btn {
  font-size: 0.6rem !important;
  padding: 6px 14px !important;
}

.review-message {
  font-family: "Press Start 2P", cursive;
  font-size: 0.6rem;
  margin: 0;
}

/* Reviews List */
.reviews-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
  max-height: 300px;
  overflow-y: auto;
}

.refresh-btn {
  font-size: 0.6rem !important;
  padding: 6px 14px !important;
  align-self: flex-start;
}

.no-reviews {
  font-family: "Press Start 2P", cursive;
  font-size: 0.6rem;
  color: #888;
  text-align: center;
  padding: 20px;
}

.review-card {
  background: rgba(50, 50, 50, 0.9) !important;
  padding: 12px !important;
}

.review-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.review-author {
  font-family: "Press Start 2P", cursive;
  font-size: 0.6rem;
  color: #ffd93d;
}

.review-stars {
  color: #ffd93d;
}

.star-display {
  font-size: 0.8rem;
}

.review-text {
  font-family: "Press Start 2P", cursive;
  font-size: 0.55rem;
  color: #ddd;
  margin: 0;
  line-height: 1.5;
}

/* Cheat */
.cheat-area {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  align-items: center;
}

.cheat-input {
  flex: 1;
  min-width: 180px;
  font-family: "Press Start 2P", cursive;
  font-size: 0.6rem;
}

.cheat-area .nes-btn {
  font-size: 0.6rem !important;
  padding: 6px 14px !important;
}

.cheat-message {
  width: 100%;
  font-family: "Press Start 2P", cursive;
  font-size: 0.6rem;
  margin: 0;
}

.cheat-message.is-success {
  color: #4ade80;
}

.cheat-message.is-error {
  color: #f87171;
}

/* Back button */
.back-btn {
  font-size: 0.9rem !important;
}
</style>
