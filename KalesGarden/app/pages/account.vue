<template>
  <main class="container">
    <article>
      <hgroup>
        <h2>My Account</h2>
        <p>Your account details</p>
      </hgroup>

      <div v-if="!user">
        <p>Redirecting to login...</p>
      </div>
      <div v-else>
        <p><strong>Email:</strong> {{ user.email }}</p>
        <p>
          <strong>Role:</strong>
          {{ user.roles?.includes("Admin") ? "Admin" : "User" }}
        </p>
      </div>
    </article>

    <!-- My Commissions -->
    <article v-if="user">
      <hgroup>
        <h3>My Commissions</h3>
        <p>Click a commission to view its invoice</p>
      </hgroup>

      <div v-if="commissionsLoading" aria-busy="true">
        Loading commissions...
      </div>

      <div v-else-if="commissions.length === 0">
        <p>You have no commissions yet.</p>
      </div>

      <table v-else role="grid">
        <thead>
          <tr>
            <th>Name</th>
            <th>Medium</th>
            <th>Price</th>
            <th>Status</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="c in commissions" :key="c.id">
            <td>{{ c.name }}</td>
            <td>{{ c.type?.medium ?? "—" }}</td>
            <td>${{ c.price.toFixed(2) }}</td>
            <td>
              <span v-if="c.isCompleted">&#x2714; Completed</span>
              <span v-else>&#x23F3; Active</span>
            </td>
            <td>
              <button class="outline" @click="viewInvoice(c.id)">
                View Invoice
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </article>

    <!-- Invoice Detail Modal -->
    <dialog :open="showInvoice" v-if="showInvoice">
      <article>
        <header>
          <button
            aria-label="Close"
            rel="prev"
            @click="showInvoice = false"
          ></button>
          <h3>Invoice Details</h3>
        </header>
        <div v-if="invoiceLoading" aria-busy="true">Loading invoice...</div>
        <div v-else-if="!selectedInvoice">
          <p>No invoice found for this commission.</p>
        </div>
        <div v-else>
          <p><strong>Invoice #:</strong> {{ selectedInvoice.id }}</p>
          <p>
            <strong>Commission:</strong>
            {{ selectedInvoice.commission?.name ?? "—" }}
          </p>
          <p>
            <strong>Description:</strong>
            {{ selectedInvoice.commission?.description ?? "—" }}
          </p>
          <p>
            <strong>Medium:</strong>
            {{ selectedInvoice.commission?.type?.medium ?? "—" }}
          </p>
          <p>
            <strong>Total Price:</strong> ${{
              selectedInvoice.totalPrice.toFixed(2)
            }}
          </p>
          <p>
            <strong>Status:</strong>
            {{
              selectedInvoice.commission?.isCompleted
                ? "Completed"
                : "In Progress"
            }}
          </p>
        </div>
        <footer>
          <button @click="showInvoice = false">Close</button>
        </footer>
      </article>
    </dialog>
  </main>
</template>

<script setup lang="ts">
import type { Commission, Invoice } from "~/types";
import { useAuth } from "../composables/useAuth";
import { useApi } from "../composables/useApi";
import { ref, watchEffect, onMounted } from "vue";
import { useRouter } from "#imports";

const { user } = useAuth();
const { apiFetch } = useApi();
const router = useRouter();

// Redirect to login if not authenticated
watchEffect(() => {
  if (import.meta.client && !user.value) {
    router.push("/login");
  }
});

// Commissions
const commissions = ref<Commission[]>([]);
const commissionsLoading = ref(true);

// Invoice modal
const showInvoice = ref(false);
const selectedInvoice = ref<Invoice | null>(null);
const invoiceLoading = ref(false);

onMounted(async () => {
  if (!user.value) return;
  try {
    commissions.value = await apiFetch<Commission[]>("/Commissions/my");
  } catch {
    commissions.value = [];
  } finally {
    commissionsLoading.value = false;
  }
});

async function viewInvoice(commissionId: number) {
  showInvoice.value = true;
  invoiceLoading.value = true;
  selectedInvoice.value = null;
  try {
    selectedInvoice.value = await apiFetch<Invoice>(
      `/Invoices/commission/${commissionId}`,
    );
  } catch {
    selectedInvoice.value = null;
  } finally {
    invoiceLoading.value = false;
  }
}
</script>
