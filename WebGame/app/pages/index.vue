<template>
  <v-app>
    <v-main class="game-area d-flex align-center justify-center">
      <!-- Pixel Art Background -->
      <div class="pixel-background">
        <img src="/images/Background.png" class="pixel-bg-image" />
      </div>

      <!-- Menu Container -->
      <div class="menu-container">
        <!-- Title Card -->
        <div class="nes-container is-dark is-centered title-card">
          <h1 class="title-text">Doobles!</h1>
        </div>

        <!-- Menu Buttons -->
        <div class="menu-buttons">
          <button
            type="button"
            class="nes-btn is-primary menu-btn"
            @click="startNewGame"
          >
            <span class="btn-icon">▶</span> Start
          </button>
          <button
            type="button"
            class="nes-btn is-warning menu-btn"
            @click="openSettings"
          >
            <span class="btn-icon">⚙</span> Settings
          </button>
        </div>
      </div>
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
import "nes.css/css/nes.min.css";

const router = useRouter();

function startNewGame() {
  router.push("/game");
}

function openSettings() {
  router.push("/settings");
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

/* Menu Styling */
.menu-container {
  position: relative;
  z-index: 10;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 40px;
}

.title-card {
  background: rgba(33, 33, 33, 0.95) !important;
  padding: 20px 50px !important;
}

.title-text {
  font-family: "Press Start 2P", cursive;
  font-size: 2.5rem;
  color: #ffd93d;
  text-shadow:
    4px 4px 0 #e65100,
    -2px -2px 0 #ff8f00;
  letter-spacing: 4px;
}

.menu-buttons {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.menu-btn {
  font-size: 1.2rem !important;
  padding: 12px 40px !important;
  min-width: 280px;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
}

.btn-icon {
  font-size: 1rem;
}
</style>
