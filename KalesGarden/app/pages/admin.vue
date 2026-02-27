<template>
  <main class="container">
    <article>
      <hgroup>
        <h2>Admin Dashboard</h2>
        <p>Manage commissions and site content</p>
      </hgroup>

      <div v-if="loading" aria-busy="true">Loading...</div>
      <div v-else-if="!isAdmin">
        <p>You are not authorized to view this page.</p>
      </div>
      <div v-else>
        <MediumManager
          ref="mediumManagerRef"
          :types="commissionTypes"
          @add="addMedium"
          @delete="deleteMedium"
        />

        <h3>Active Commissions</h3>
        <p v-if="activeCommissions.length === 0">No active commissions.</p>
        <CommissionTable v-else :commissions="activeCommissions" />

        <h3>Completed Commissions</h3>
        <p v-if="completedCommissions.length === 0">
          No completed commissions.
        </p>
        <CommissionTable v-else :commissions="completedCommissions" />
      </div>
    </article>
  </main>
</template>

<script setup lang="ts">
import type { Commission, CommissionType } from "~/types";
import { useApi } from "~/composables/useApi";
import { useAuth } from "~/composables/useAuth";
import { ref, computed, onMounted } from "vue";
import { useRouter } from "#imports";

const { apiFetch } = useApi();
const { user } = useAuth();
const router = useRouter();

const mediumManagerRef = ref<{ adding: boolean; error: string } | null>(null);

const isAdmin = computed(() => {
  return user.value?.roles?.includes("Admin") ?? false;
});

const commissions = ref<Commission[]>([]);
const commissionTypes = ref<CommissionType[]>([]);
const loading = ref(true);

const activeCommissions = computed(() =>
  commissions.value.filter((c) => !c.isCompleted),
);
const completedCommissions = computed(() =>
  commissions.value.filter((c) => c.isCompleted),
);

async function addMedium(medium: string, price: number) {
  if (!mediumManagerRef.value) return;
  mediumManagerRef.value.adding = true;
  mediumManagerRef.value.error = "";
  try {
    const created = await apiFetch<CommissionType>("/CommissionTypes", {
      method: "POST",
      body: { medium, price },
    });
    commissionTypes.value.push(created);
  } catch (e: any) {
    mediumManagerRef.value.error = e?.data?.detail || "Failed to add medium.";
  } finally {
    mediumManagerRef.value.adding = false;
  }
}

async function deleteMedium(id: number) {
  if (!confirm("Delete this medium? This cannot be undone.")) return;
  try {
    await apiFetch(`/CommissionTypes/${id}`, { method: "DELETE" });
    commissionTypes.value = commissionTypes.value.filter((ct) => ct.id !== id);
  } catch (e: any) {
    alert(e?.data?.detail || "Failed to delete medium.");
  }
}

onMounted(async () => {
  if (!isAdmin.value) {
    loading.value = false;
    router.push("/login");
    return;
  }
  try {
    const [commissionsData, typesData] = await Promise.all([
      apiFetch<Commission[]>("/Commissions"),
      apiFetch<CommissionType[]>("/CommissionTypes"),
    ]);
    commissions.value = commissionsData;
    commissionTypes.value = typesData;
  } catch {
    commissions.value = [];
    commissionTypes.value = [];
  } finally {
    loading.value = false;
  }
});
</script>
