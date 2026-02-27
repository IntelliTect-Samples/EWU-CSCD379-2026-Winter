<template>
  <main class="container">
    <article>
      <hgroup>
        <h2>Commission New Work</h2>
        <p>Request a custom piece from the artist</p>
      </hgroup>

      <div v-if="typesLoading" aria-busy="true">
        Loading commission types...
      </div>

      <form v-else @submit.prevent="handleSubmit">
        <label for="name">
          Piece Name
          <input
            id="name"
            v-model="form.name"
            type="text"
            placeholder="What should we call it?"
            required
          />
        </label>

        <label for="description">
          Description
          <textarea
            id="description"
            v-model="form.description"
            placeholder="Describe your vision..."
            required
          />
        </label>

        <label for="typeId">
          Commission Type
          <select id="typeId" v-model="form.typeId" required>
            <option value="" disabled>Select a medium...</option>
            <option v-for="ct in commissionTypes" :key="ct.id" :value="ct.id">
              {{ ct.medium }} — ${{ ct.price.toFixed(2) }}
            </option>
          </select>
        </label>

        <button type="submit" :aria-busy="submitting">
          {{ submitting ? "Submitting..." : "Submit Commission" }}
        </button>
      </form>

      <p v-if="errorMsg" role="alert">{{ errorMsg }}</p>
      <p v-if="successMsg" role="alert">{{ successMsg }}</p>
    </article>
  </main>
</template>

<script setup lang="ts">
import type { CommissionType } from "~/types";
import { useAuth } from "~/composables/useAuth";
import { useApi } from "~/composables/useApi";
import { watchEffect } from "vue";
import { useRouter } from "#imports";

const { apiFetch, apiBase } = useApi();
const { user } = useAuth();
const router = useRouter();

// Redirect to login if not authenticated
watchEffect(() => {
  if (import.meta.client && !user.value) {
    router.push("/login");
  }
});

const form = reactive({
  name: "",
  description: "",
  typeId: "" as string | number,
});

const submitting = ref(false);
const errorMsg = ref("");
const successMsg = ref("");

const { data: commissionTypes, pending: typesLoading } = useFetch<
  CommissionType[]
>(`${apiBase}/CommissionTypes`);

async function handleSubmit() {
  submitting.value = true;
  errorMsg.value = "";
  successMsg.value = "";

  const selectedType = commissionTypes.value?.find(
    (ct) => ct.id === Number(form.typeId),
  );

  try {
    await apiFetch("/Commissions", {
      method: "POST",
      body: {
        name: form.name,
        description: form.description,
        typeId: Number(form.typeId),
        price: selectedType?.price ?? 0,
        isCompleted: false,
      },
    });
    successMsg.value = "Commission submitted successfully!";
    form.name = "";
    form.description = "";
    form.typeId = "";
  } catch (err: any) {
    errorMsg.value =
      err?.data?.detail || "Failed to submit commission. Are you logged in?";
  } finally {
    submitting.value = false;
  }
}
</script>
