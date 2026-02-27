<template>
  <dialog
    v-if="piece"
    :open="!!piece"
    class="lightbox-dialog"
    @click.self="$emit('close')"
  >
    <article class="lightbox-content" @click.stop>
      <header>
        <button aria-label="Close" rel="prev" @click="$emit('close')"></button>
        <h3>{{ piece.name }}</h3>
      </header>
      <img
        v-if="piece.imageUrl"
        :src="piece.imageUrl"
        :alt="piece.name"
        class="lightbox-image"
      />
      <p>{{ piece.description }}</p>
      <footer class="lightbox-footer">
        <strong>${{ piece.price.toFixed(2) }}</strong>
        <span v-if="piece.isAvailable"> ✔ Available</span>
        <span v-else> ✘ Sold</span>
      </footer>
    </article>
  </dialog>
</template>

<script setup lang="ts">
import type { ArtPiece } from "~/types";

defineProps<{
  piece: ArtPiece | null;
}>();

defineEmits<{
  close: [];
}>();
</script>

<style scoped>
.lightbox-dialog {
  position: fixed;
  inset: 0;
  z-index: 999;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 1rem;
  width: 100%;
  height: 100%;
  border: none;
}

.lightbox-content {
  max-width: 800px;
  max-height: 90vh;
  overflow-y: auto;
  margin: 0 auto;
  width: 100%;
}

.lightbox-image {
  width: 100%;
  max-height: 70vh;
  object-fit: contain;
}

.lightbox-footer {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  flex-wrap: wrap;
}

@media (max-width: 600px) {
  .lightbox-dialog {
    padding: 0.5rem;
  }

  .lightbox-content {
    max-height: 95vh;
  }

  .lightbox-image {
    max-height: 50vh;
  }
}
</style>
