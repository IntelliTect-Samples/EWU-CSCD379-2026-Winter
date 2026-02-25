<template>
  <main class="container">
    <hgroup>
      <h1>Kale's Garden Gallery</h1>
      <p>Browse our collection of beautiful art pieces</p>
    </hgroup>

    <ClientOnly>
      <div v-if="isAdmin" style="margin-bottom: 1rem">
        <button @click="openAddModal">Add New Art Piece</button>
      </div>
    </ClientOnly>

    <div v-if="pending" aria-busy="true">Loading gallery...</div>

    <div v-else-if="error">
      <article>
        <p>Could not load art pieces. Is the API running?</p>
        <button @click="() => refresh()">Retry</button>
      </article>
    </div>

    <div v-else-if="artPieces && artPieces.length">
      <div class="gallery-grid">
        <article
          v-for="piece in artPieces"
          :key="piece.id"
          class="gallery-card"
          @click="openLightbox(piece)"
          style="cursor: pointer"
        >
          <header>
            <img
              v-if="piece.imageUrl"
              :src="piece.imageUrl"
              :alt="piece.name"
              style="width: 100%; max-height: 250px; object-fit: cover"
            />
          </header>
          <hgroup>
            <h3>{{ piece.name }}</h3>
            <p>{{ piece.description }}</p>
          </hgroup>
          <footer>
            <strong>${{ piece.price.toFixed(2) }}</strong>
            <span
              v-if="piece.isAvailable"
              data-tooltip="This piece is available"
            >
              &nbsp;&#x2714; Available
            </span>
            <span v-else data-tooltip="This piece has been sold">
              &nbsp;&#x2718; Sold
            </span>
            <button v-if="isAdmin" @click.stop="openEditModal(piece)">
              Edit
            </button>
            <button
              v-if="isAdmin"
              class="secondary"
              @click.stop="deleteArt(piece.id)"
            >
              Delete
            </button>
          </footer>
        </article>
      </div>
    </div>

    <div v-else>
      <article>
        <p>No art pieces found. Check back soon!</p>
      </article>
    </div>

    <!-- Lightbox Modal -->
    <dialog
      :open="!!lightboxPiece"
      v-if="lightboxPiece"
      class="lightbox-dialog"
      @click.self="lightboxPiece = null"
    >
      <article class="lightbox-content" @click.stop>
        <header>
          <button
            aria-label="Close"
            rel="prev"
            @click="lightboxPiece = null"
          ></button>
          <h3>{{ lightboxPiece.name }}</h3>
        </header>
        <img
          v-if="lightboxPiece.imageUrl"
          :src="lightboxPiece.imageUrl"
          :alt="lightboxPiece.name"
          class="lightbox-image"
        />
        <p>{{ lightboxPiece.description }}</p>
        <footer>
          <strong>${{ lightboxPiece.price.toFixed(2) }}</strong>
          <span v-if="lightboxPiece.isAvailable">
            &nbsp;&#x2714; Available</span
          >
          <span v-else> &nbsp;&#x2718; Sold</span>
        </footer>
      </article>
    </dialog>

    <!-- Add / Edit Modal -->
    <dialog :open="showModal" v-if="showModal">
      <article>
        <header>
          <button
            aria-label="Close"
            rel="prev"
            @click="showModal = false"
          ></button>
          <h3>{{ editingPiece ? "Edit Art Piece" : "Add New Art Piece" }}</h3>
        </header>
        <form @submit.prevent="submitArtPiece">
          <label>
            Name
            <input v-model="artForm.name" required />
          </label>
          <label>
            Description
            <textarea v-model="artForm.description" required />
          </label>
          <label>
            Price
            <input
              v-model.number="artForm.price"
              type="number"
              min="0"
              step="0.01"
              required
            />
          </label>
          <label>
            <input
              v-model="artForm.isAvailable"
              type="checkbox"
              role="switch"
            />
            Available
          </label>
          <label>
            Image {{ editingPiece ? "(leave blank to keep current)" : "" }}
            <input
              type="file"
              @change="onFileChange"
              accept="image/*"
              :required="!editingPiece"
            />
          </label>
          <footer>
            <button type="button" class="secondary" @click="showModal = false">
              Cancel
            </button>
            <button type="submit" :aria-busy="submitting">
              {{ submitting ? "Saving..." : editingPiece ? "Update" : "Add" }}
            </button>
          </footer>
          <p v-if="formError" style="color: red">{{ formError }}</p>
        </form>
      </article>
    </dialog>
  </main>
</template>

<script setup lang="ts">
import type { ArtPiece } from "~/types";
import { useAuth } from "../composables/useAuth";
import { useApi } from "../composables/useApi";
import { computed, ref, reactive, onMounted } from "vue";

const { apiBase } = useApi();
const { user, token, restoreToken, fetchUser } = useAuth();

// Ensure auth state is available on this page
onMounted(async () => {
  restoreToken();
  if (token.value && !user.value) {
    await fetchUser();
  }
});

const isAdmin = computed(() => {
  return user.value?.roles?.includes("Admin") ?? false;
});

const {
  data: artPieces,
  pending,
  error,
  refresh,
} = useFetch<ArtPiece[]>(`${apiBase}/ArtPieces`, { server: false });

// Lightbox state
const lightboxPiece = ref<ArtPiece | null>(null);
function openLightbox(piece: ArtPiece) {
  lightboxPiece.value = piece;
}

// Modal state
const showModal = ref(false);
const editingPiece = ref<ArtPiece | null>(null);
const submitting = ref(false);
const formError = ref("");
const artForm = reactive({
  name: "",
  description: "",
  price: 0,
  isAvailable: true,
  image: null as File | null,
});

function resetForm() {
  artForm.name = "";
  artForm.description = "";
  artForm.price = 0;
  artForm.isAvailable = true;
  artForm.image = null;
  formError.value = "";
}

function openAddModal() {
  editingPiece.value = null;
  resetForm();
  showModal.value = true;
}

function openEditModal(piece: ArtPiece) {
  editingPiece.value = piece;
  artForm.name = piece.name;
  artForm.description = piece.description;
  artForm.price = piece.price;
  artForm.isAvailable = piece.isAvailable;
  artForm.image = null;
  formError.value = "";
  showModal.value = true;
}

function onFileChange(e: Event) {
  const files = (e.target as HTMLInputElement).files;
  artForm.image = files && files[0] ? files[0] : null;
}

async function submitArtPiece() {
  submitting.value = true;
  formError.value = "";
  try {
    const formData = new FormData();
    formData.append("name", artForm.name);
    formData.append("description", artForm.description);
    formData.append("price", String(artForm.price));
    formData.append("isAvailable", String(artForm.isAvailable));
    if (artForm.image) formData.append("image", artForm.image);

    const headers: Record<string, string> = {};
    if (token.value) {
      headers["Authorization"] = `Bearer ${token.value}`;
    }

    if (editingPiece.value) {
      // Update existing
      await $fetch(`${apiBase}/ArtPieces/${editingPiece.value.id}`, {
        method: "PUT",
        body: formData,
        headers,
      });
    } else {
      // Create new
      await $fetch(`${apiBase}/ArtPieces`, {
        method: "POST",
        body: formData,
        headers,
      });
    }

    showModal.value = false;
    resetForm();
    editingPiece.value = null;
    refresh();
  } catch (e: any) {
    formError.value = e?.data?.detail || "Failed to save art piece.";
  } finally {
    submitting.value = false;
  }
}

async function deleteArt(id: number) {
  if (!confirm("Are you sure you want to delete this art piece?")) return;
  try {
    const headers: Record<string, string> = {};
    if (token.value) {
      headers["Authorization"] = `Bearer ${token.value}`;
    }
    await $fetch(`${apiBase}/ArtPieces/${id}`, {
      method: "DELETE",
      headers,
    });
    refresh();
  } catch (e: any) {
    alert(e?.data?.detail || "Failed to delete art piece.");
  }
}
</script>

<style scoped>
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

.gallery-card {
  margin-bottom: 0;
}

.lightbox-dialog {
  position: fixed;
  inset: 0;
  z-index: 999;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 2rem;
  width: 100%;
  height: 100%;
  border: none;
}

.lightbox-content {
  max-width: 800px;
  max-height: 90vh;
  overflow-y: auto;
  margin: 0 auto;
}

.lightbox-image {
  width: 100%;
  max-height: 70vh;
  object-fit: contain;
}
</style>
