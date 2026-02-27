<template>
  <div class="table-responsive">
    <table role="grid">
      <thead>
        <tr>
          <th>Name</th>
          <th class="hide-mobile">Description</th>
          <th>Medium</th>
          <th>Price</th>
          <th>Status</th>
          <th v-if="showInvoiceButton"></th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="c in commissions" :key="c.id">
          <td>{{ c.name }}</td>
          <td class="hide-mobile">{{ c.description }}</td>
          <td>{{ c.type?.medium ?? "—" }}</td>
          <td>${{ c.price.toFixed(2) }}</td>
          <td>
            <span v-if="c.isCompleted">✔ Done</span>
            <span v-else>⏳ Active</span>
          </td>
          <td v-if="showInvoiceButton">
            <button
              class="outline invoice-btn"
              @click="$emit('viewInvoice', c.id)"
            >
              Invoice
            </button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup lang="ts">
import type { Commission } from "~/types";

defineProps<{
  commissions: Commission[];
  showInvoiceButton?: boolean;
}>();

defineEmits<{
  viewInvoice: [commissionId: number];
}>();
</script>

<style scoped>
.table-responsive {
  overflow-x: auto;
  -webkit-overflow-scrolling: touch;
}

.invoice-btn {
  padding: 0.25rem 0.5rem;
  font-size: 0.8rem;
  margin: 0;
  white-space: nowrap;
}

@media (max-width: 600px) {
  .hide-mobile {
    display: none;
  }
}
</style>
