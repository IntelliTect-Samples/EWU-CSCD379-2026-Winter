<template>
  <dialog :open="open" v-if="open">
    <article>
      <header>
        <button aria-label="Close" rel="prev" @click="$emit('close')"></button>
        <h3>Invoice Details</h3>
      </header>
      <div v-if="loading" aria-busy="true">Loading invoice...</div>
      <div v-else-if="!invoice">
        <p>No invoice found for this commission.</p>
      </div>
      <div v-else class="invoice-details">
        <p><strong>Invoice #:</strong> {{ invoice.id }}</p>
        <p>
          <strong>Commission:</strong> {{ invoice.commission?.name ?? "—" }}
        </p>
        <p>
          <strong>Description:</strong>
          {{ invoice.commission?.description ?? "—" }}
        </p>
        <p>
          <strong>Medium:</strong> {{ invoice.commission?.type?.medium ?? "—" }}
        </p>
        <p>
          <strong>Total Price:</strong> ${{ invoice.totalPrice.toFixed(2) }}
        </p>
        <p>
          <strong>Status:</strong>
          {{ invoice.commission?.isCompleted ? "Completed" : "In Progress" }}
        </p>
      </div>
      <footer>
        <button @click="$emit('close')">Close</button>
      </footer>
    </article>
  </dialog>
</template>

<script setup lang="ts">
import type { Invoice } from "~/types";

defineProps<{
  open: boolean;
  loading: boolean;
  invoice: Invoice | null;
}>();

defineEmits<{
  close: [];
}>();
</script>

<style scoped>
.invoice-details p {
  margin-bottom: 0.5rem;
}
</style>
