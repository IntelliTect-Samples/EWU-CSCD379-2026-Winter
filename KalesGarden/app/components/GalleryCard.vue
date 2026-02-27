<template>
  <article class="gallery-card" @click="$emit('click')">
    <header class="card-image">
      <img
        v-if="piece.imageUrl"
        :src="piece.imageUrl"
        :alt="piece.name"
        loading="lazy"
      />
      <div v-else class="no-image">No Image</div>
    </header>
    <hgroup>
      <h3>{{ piece.name }}</h3>
      <p class="card-description">{{ piece.description }}</p>
    </hgroup>
    <footer class="card-footer">
      <strong>${{ piece.price.toFixed(2) }}</strong>
      <span v-if="piece.isAvailable" class="badge available">✔ Available</span>
      <span v-else class="badge sold">✘ Sold</span>
      <div v-if="showAdmin" class="card-actions" @click.stop>
        <button class="outline" @click="$emit('edit', piece)">Edit</button>
        <button class="outline secondary" @click="$emit('delete', piece.id)">
          Delete
        </button>
      </div>
    </footer>
  </article>
</template>

<script setup lang="ts">
import type { ArtPiece } from "~/types";

defineProps<{
  piece: ArtPiece;
  showAdmin?: boolean;
}>();

defineEmits<{
  click: [];
  edit: [piece: ArtPiece];
  delete: [id: number];
}>();
</script>

<style scoped>
.gallery-card {
  cursor: pointer;
  margin-bottom: 0;
  overflow: hidden;
}

.card-image img {
  width: 100%;
  max-height: 250px;
  object-fit: cover;
}

.card-image .no-image {
  width: 100%;
  height: 200px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--pico-muted-border-color);
  color: var(--pico-muted-color);
}

.card-description {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.card-footer {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 0.5rem;
}

.badge {
  font-size: 0.85rem;
}

.badge.available {
  color: var(--pico-ins-color);
}

.badge.sold {
  color: var(--pico-del-color);
}

.card-actions {
  display: flex;
  gap: 0.25rem;
  margin-left: auto;
}

.card-actions button {
  padding: 0.25rem 0.5rem;
  font-size: 0.8rem;
  margin: 0;
}
</style>
