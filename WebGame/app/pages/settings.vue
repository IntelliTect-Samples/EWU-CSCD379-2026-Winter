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

          <!-- Send Feedback -->
          <div class="setting-row">
            <label class="setting-label">💬 Send Feedback</label>
            <div class="feedback-area">
              <textarea
                v-if="showFeedbackInput"
                class="nes-textarea"
                v-model="feedbackText"
                placeholder="Tell us what you think..."
                rows="3"
              ></textarea>
              <div class="feedback-buttons">
                <button
                  v-if="!showFeedbackInput"
                  class="nes-btn is-primary"
                  @click="showFeedbackInput = true"
                >
                  Write Feedback
                </button>
                <template v-else>
                  <button
                    class="nes-btn is-success"
                    @click="submitFeedback"
                    :disabled="!feedbackText.trim()"
                  >
                    Send
                  </button>
                  <button
                    class="nes-btn is-error"
                    @click="
                      showFeedbackInput = false;
                      feedbackText = '';
                    "
                  >
                    Cancel
                  </button>
                </template>
              </div>
              <p
                v-if="feedbackMessage"
                class="feedback-message"
                :class="feedbackMessageClass"
              >
                {{ feedbackMessage }}
              </p>
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

// Feedback
const showFeedbackInput = ref(false);
const feedbackText = ref("");
const feedbackMessage = ref("");
const feedbackMessageClass = ref("");

function submitFeedback() {
  if (!feedbackText.value.trim()) return;
  // For now just acknowledge — no backend
  feedbackMessage.value = "Thanks for your feedback!";
  feedbackMessageClass.value = "is-success";
  feedbackText.value = "";
  showFeedbackInput.value = false;
  setTimeout(() => {
    feedbackMessage.value = "";
  }, 3000);
}

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

/* Feedback */
.feedback-area {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.feedback-area .nes-textarea {
  font-family: "Press Start 2P", cursive;
  font-size: 0.6rem;
  resize: vertical;
}

.feedback-buttons {
  display: flex;
  gap: 10px;
}

.feedback-buttons .nes-btn {
  font-size: 0.6rem !important;
  padding: 6px 14px !important;
}

.feedback-message {
  font-family: "Press Start 2P", cursive;
  font-size: 0.6rem;
  margin: 0;
}

.feedback-message.is-success {
  color: #4ade80;
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
