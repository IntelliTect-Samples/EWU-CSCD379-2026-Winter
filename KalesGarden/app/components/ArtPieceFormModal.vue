<template>
  <dialog :open="open" v-if="open">
    <article>
      <header>
        <button aria-label="Close" rel="prev" @click="$emit('close')"></button>
        <h3>{{ editing ? "Edit Art Piece" : "Add New Art Piece" }}</h3>
      </header>
      <form @submit.prevent="handleSubmit">
        <label>
          Name
          <input v-model="form.name" required />
        </label>
        <label>
          Description
          <textarea v-model="form.description" required />
        </label>
        <label>
          Price
          <input
            v-model.number="form.price"
            type="number"
            min="0"
            step="0.01"
            required
          />
        </label>
        <label>
          <input v-model="form.isAvailable" type="checkbox" role="switch" />
          Available
        </label>
        <label>
          Image {{ editing ? "(leave blank to keep current)" : "" }}
          <input
            type="file"
            @change="onFileChange"
            accept="image/*"
            :required="!editing"
          />
        </label>
        <footer class="form-actions">
          <button type="button" class="secondary" @click="$emit('close')">
            Cancel
          </button>
          <button type="submit" :aria-busy="submitting">
            {{ submitting ? "Saving..." : editing ? "Update" : "Add" }}
          </button>
        </footer>
        <p v-if="error" class="form-error">{{ error }}</p>
      </form>
    </article>
  </dialog>
</template>

<script setup lang="ts">
import type { ArtPiece } from "~/types";
import { reactive, ref, watch } from "vue";

const props = defineProps<{
  open: boolean;
  editing: boolean;
  initialData?: ArtPiece | null;
}>();

const emit = defineEmits<{
  close: [];
  submit: [formData: FormData];
}>();

const submitting = ref(false);
const error = ref("");

const form = reactive({
  name: "",
  description: "",
  price: 0,
  isAvailable: true,
  image: null as File | null,
});

watch(
  () => props.open,
  (isOpen) => {
    if (isOpen && props.initialData) {
      form.name = props.initialData.name;
      form.description = props.initialData.description;
      form.price = props.initialData.price;
      form.isAvailable = props.initialData.isAvailable;
      form.image = null;
      error.value = "";
    } else if (isOpen) {
      form.name = "";
      form.description = "";
      form.price = 0;
      form.isAvailable = true;
      form.image = null;
      error.value = "";
    }
  },
);

function onFileChange(e: Event) {
  const files = (e.target as HTMLInputElement).files;
  form.image = files?.[0] ?? null;
}

function handleSubmit() {
  const formData = new FormData();
  formData.append("name", form.name);
  formData.append("description", form.description);
  formData.append("price", String(form.price));
  formData.append("isAvailable", String(form.isAvailable));
  if (form.image) formData.append("image", form.image);
  emit("submit", formData);
}

defineExpose({ submitting, error });
</script>

<style scoped>
.form-actions {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.form-error {
  color: var(--pico-del-color);
  margin-top: 0.5rem;
}

@media (max-width: 600px) {
  .form-actions {
    flex-direction: column;
  }

  .form-actions button {
    width: 100%;
  }
}
</style>
