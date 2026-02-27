<template>
  <section>
    <h3>Commission Mediums</h3>
    <div class="table-responsive">
      <table role="grid">
        <thead>
          <tr>
            <th>Medium</th>
            <th>Price</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="ct in types" :key="ct.id">
            <td>{{ ct.medium }}</td>
            <td>${{ ct.price.toFixed(2) }}</td>
            <td>
              <button
                class="outline secondary delete-btn"
                @click="$emit('delete', ct.id)"
              >
                Delete
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <form @submit.prevent="handleAdd" class="medium-form">
      <div class="form-grid">
        <label>
          Medium Name
          <input v-model="newMedium" placeholder="e.g. Watercolor" required />
        </label>
        <label>
          Price
          <input
            v-model.number="newPrice"
            type="number"
            min="0"
            step="0.01"
            required
          />
        </label>
        <label class="btn-label">
          &nbsp;
          <button type="submit" :aria-busy="adding">
            {{ adding ? "Adding..." : "Add Medium" }}
          </button>
        </label>
      </div>
      <p v-if="error" class="form-error">{{ error }}</p>
    </form>
  </section>
</template>

<script setup lang="ts">
import type { CommissionType } from "~/types";
import { ref } from "vue";

defineProps<{
  types: CommissionType[];
}>();

const emit = defineEmits<{
  add: [medium: string, price: number];
  delete: [id: number];
}>();

const newMedium = ref("");
const newPrice = ref(0);
const adding = ref(false);
const error = ref("");

function handleAdd() {
  emit("add", newMedium.value, newPrice.value);
  newMedium.value = "";
  newPrice.value = 0;
}

defineExpose({ adding, error });
</script>

<style scoped>
.table-responsive {
  overflow-x: auto;
  -webkit-overflow-scrolling: touch;
}

.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr auto;
  gap: 1rem;
  align-items: end;
}

.delete-btn {
  padding: 0.25rem 0.5rem;
  font-size: 0.8rem;
  margin: 0;
}

.btn-label {
  min-width: fit-content;
}

.form-error {
  color: var(--pico-del-color);
  margin-top: 0.5rem;
}

@media (max-width: 600px) {
  .form-grid {
    grid-template-columns: 1fr;
  }
}
</style>
