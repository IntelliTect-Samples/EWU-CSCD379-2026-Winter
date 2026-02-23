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
        <!-- Commission Types / Mediums Management -->
        <h3>Commission Mediums</h3>
        <table role="grid">
          <thead>
            <tr>
              <th>Medium</th>
              <th>Price</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="ct in commissionTypes" :key="ct.id">
              <td>{{ ct.medium }}</td>
              <td>${{ ct.price.toFixed(2) }}</td>
              <td>
                <button class="outline secondary" @click="deleteMedium(ct.id)">
                  Delete
                </button>
              </td>
            </tr>
          </tbody>
        </table>
        <form @submit.prevent="addMedium" style="margin-bottom: 2rem">
          <div class="grid">
            <label>
              Medium Name
              <input
                v-model="newMedium.medium"
                placeholder="e.g. Watercolor"
                required
              />
            </label>
            <label>
              Price
              <input
                v-model.number="newMedium.price"
                type="number"
                min="0"
                step="0.01"
                required
              />
            </label>
            <label>
              &nbsp;
              <button type="submit" :aria-busy="addingMedium">
                {{ addingMedium ? "Adding..." : "Add Medium" }}
              </button>
            </label>
          </div>
          <p v-if="mediumError" style="color: red">{{ mediumError }}</p>
        </form>

        <!-- Active Commissions -->
        <h3>Active Commissions</h3>
        <p v-if="activeCommissions.length === 0">No active commissions.</p>
        <table v-else role="grid">
          <thead>
            <tr>
              <th>Name</th>
              <th>Description</th>
              <th>Type</th>
              <th>Price</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="c in activeCommissions" :key="c.id">
              <td>{{ c.name }}</td>
              <td>{{ c.description }}</td>
              <td>{{ c.type?.medium ?? "—" }}</td>
              <td>${{ c.price.toFixed(2) }}</td>
              <td>
                <span v-if="c.isCompleted">&#x2714; Completed</span>
                <span v-else>&#x23F3; Active</span>
              </td>
            </tr>
          </tbody>
        </table>

        <!-- Completed Commissions -->
        <h3>Completed Commissions</h3>
        <p v-if="completedCommissions.length === 0">
          No completed commissions.
        </p>
        <table v-else role="grid">
          <thead>
            <tr>
              <th>Name</th>
              <th>Description</th>
              <th>Type</th>
              <th>Price</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="c in completedCommissions" :key="c.id">
              <td>{{ c.name }}</td>
              <td>{{ c.description }}</td>
              <td>{{ c.type?.medium ?? "—" }}</td>
              <td>${{ c.price.toFixed(2) }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </article>
  </main>
</template>

<script setup lang="ts">
import type { Commission, CommissionType } from "~/types";
import { useApi } from "../composables/useApi";
import { useAuth } from "../composables/useAuth";
import { ref, reactive, computed, onMounted } from "vue";
import { useRouter } from "#imports";

const { apiFetch } = useApi();
const { user } = useAuth();
const router = useRouter();

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

// Medium management
const newMedium = reactive({ medium: "", price: 0 });
const addingMedium = ref(false);
const mediumError = ref("");

async function addMedium() {
  addingMedium.value = true;
  mediumError.value = "";
  try {
    const created = await apiFetch<CommissionType>("/CommissionTypes", {
      method: "POST",
      body: { medium: newMedium.medium, price: newMedium.price },
    });
    commissionTypes.value.push(created);
    newMedium.medium = "";
    newMedium.price = 0;
  } catch (e: any) {
    mediumError.value = e?.data?.detail || "Failed to add medium.";
  } finally {
    addingMedium.value = false;
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
