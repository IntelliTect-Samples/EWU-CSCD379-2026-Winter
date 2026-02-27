<template>
  <main class="container">
    <hgroup>
      <h1>Kale's Garden Gallery</h1>
      <p>Browse our collection of beautiful art pieces</p>
    </hgroup>

    <ClientOnly>
      <div v-if="isAdmin" class="admin-actions">
        <button @click="openAddModal">Add New Art Piece</button>
      </div>
    </ClientOnly>

    <ClientOnly>
      <div v-if="pending" aria-busy="true">Loading gallery...</div>

      <div v-else-if="error">
        <article>
          <p>Could not load art pieces. Is the API running?</p>
          <button @click="() => refresh()">Retry</button>
        </article>
      </div>

      <div v-else-if="artPieces && artPieces.length" class="gallery-grid">
        <GalleryCard
          v-for="piece in artPieces"
          :key="piece.id"
          :piece="piece"
          :show-admin="isAdmin"
          @click="lightboxPiece = piece"
          @edit="openEditModal"
          @delete="deleteArt"
        />
      </div>

      <div v-else>
        <article>
          <p>No art pieces found. Check back soon!</p>
        </article>
      </div>

      <template #fallback>
        <div aria-busy="true">Loading gallery...</div>
      </template>
    </ClientOnly>

    <LightboxModal :piece="lightboxPiece" @close="lightboxPiece = null" />

    <ArtPieceFormModal
      ref="artFormRef"
      :open="showModal"
      :editing="!!editingPiece"
      :initial-data="editingPiece"
      @close="showModal = false"
      @submit="submitArtPiece"
    />
  </main>
</template>

<script setup lang="ts">
import type { ArtPiece } from "~/types";
import { useAuth } from "~/composables/useAuth";
import { useApi } from "~/composables/useApi";
import { computed, ref, onMounted, type Ref } from "vue";

const { apiBase } = useApi();
const { user, token, restoreToken, fetchUser } = useAuth();

onMounted(async () => {
  restoreToken();
  if (token.value && !user.value) {
    await fetchUser();
  }
});

const isAdmin = computed(() => user.value?.roles?.includes("Admin") ?? false);

const {
  data: artPieces,
  pending,
  error,
  refresh,
} = useFetch<ArtPiece[]>(`${apiBase}/ArtPieces`, { server: false });

// Lightbox
const lightboxPiece = ref<ArtPiece | null>(null);

// Art form modal
const showModal = ref(false);
const editingPiece = ref<ArtPiece | null>(null);
const artFormRef = ref<{ submitting: Ref<boolean>; error: Ref<string> } | null>(
  null,
);

function openAddModal() {
  editingPiece.value = null;
  showModal.value = true;
}

function openEditModal(piece: ArtPiece) {
  editingPiece.value = piece;
  showModal.value = true;
}

async function submitArtPiece(formData: FormData) {
  if (!artFormRef.value) return;
  artFormRef.value.submitting = true;
  artFormRef.value.error = "";
  try {
    const headers: Record<string, string> = {};
    if (token.value) headers["Authorization"] = `Bearer ${token.value}`;

    if (editingPiece.value) {
      await $fetch(`${apiBase}/ArtPieces/${editingPiece.value.id}`, {
        method: "PUT",
        body: formData,
        headers,
      });
    } else {
      await $fetch(`${apiBase}/ArtPieces`, {
        method: "POST",
        body: formData,
        headers,
      });
    }
    showModal.value = false;
    editingPiece.value = null;
    refresh();
  } catch (e: any) {
    artFormRef.value.error = e?.data?.detail || "Failed to save art piece.";
  } finally {
    artFormRef.value.submitting = false;
  }
}

async function deleteArt(id: number) {
  if (!confirm("Are you sure you want to delete this art piece?")) return;
  try {
    const headers: Record<string, string> = {};
    if (token.value) headers["Authorization"] = `Bearer ${token.value}`;
    await $fetch(`${apiBase}/ArtPieces/${id}`, { method: "DELETE", headers });
    refresh();
  } catch (e: any) {
    alert(e?.data?.detail || "Failed to delete art piece.");
  }
}
</script>

<style scoped>
.admin-actions {
  margin-bottom: 1rem;
}

.gallery-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1.5rem;
}

@media (max-width: 1200px) {
  .gallery-grid {
    grid-template-columns: repeat(3, 1fr);
  }
}

@media (max-width: 900px) {
  .gallery-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 600px) {
  .gallery-grid {
    grid-template-columns: 1fr;
  }
}
</style>
