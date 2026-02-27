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

    <article v-if="user">
      <hgroup>
        <h3>My Commissions</h3>
        <p>Click "Invoice" to view the invoice for a commission</p>
      </hgroup>

      <div v-if="commissionsLoading" aria-busy="true">
        Loading commissions...
      </div>

      <div v-else-if="commissions.length === 0">
        <p>You have no commissions yet.</p>
      </div>

      <CommissionTable
        v-else
        :commissions="commissions"
        :show-invoice-button="true"
        @view-invoice="viewInvoice"
      />
    </article>

    <InvoiceModal
      :open="showInvoice"
      :loading="invoiceLoading"
      :invoice="selectedInvoice"
      @close="showInvoice = false"
    />
  </main>
</template>

<script setup lang="ts">
import type { Commission, Invoice } from "~/types";
import { useAuth } from "~/composables/useAuth";
import { useApi } from "~/composables/useApi";
import { ref, watchEffect, onMounted } from "vue";
import { useRouter } from "#imports";

const { user } = useAuth();
const { apiFetch } = useApi();
const router = useRouter();

watchEffect(() => {
  if (import.meta.client && !user.value) {
    router.push("/login");
  }
});

const commissions = ref<Commission[]>([]);
const commissionsLoading = ref(true);

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
