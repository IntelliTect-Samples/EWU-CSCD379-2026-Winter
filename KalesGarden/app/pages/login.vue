<template>
  <main class="container">
    <article>
      <hgroup>
        <h2>Login</h2>
        <p>Sign in to your account</p>
      </hgroup>

      <form @submit.prevent="handleLogin">
        <label for="email">
          Email
          <input
            id="email"
            v-model="email"
            type="email"
            placeholder="you@example.com"
            required
          />
        </label>

        <label for="password">
          Password
          <input
            id="password"
            v-model="password"
            type="password"
            placeholder="Your password"
            required
          />
        </label>

        <button type="submit" :aria-busy="loading">
          {{ loading ? "Signing in..." : "Sign In" }}
        </button>
      </form>

      <p v-if="errorMsg" role="alert">{{ errorMsg }}</p>
      <p v-if="success" role="alert">Logged in successfully! Redirecting...</p>
    </article>
  </main>
</template>

<script setup lang="ts">
import { useAuth } from "../composables/useAuth";
import { useRouter } from "#imports";
import { ref } from "vue";

const { login } = useAuth();
const router = useRouter();

const email = ref("");
const password = ref("");
const loading = ref(false);
const errorMsg = ref("");
const success = ref(false);

async function handleLogin() {
  loading.value = true;
  errorMsg.value = "";
  try {
    await login(email.value, password.value);
    success.value = true;
    setTimeout(() => router.push("/"), 1000);
  } catch (err: any) {
    errorMsg.value =
      err?.message || "Login failed. Please check your credentials.";
  } finally {
    loading.value = false;
  }
}
</script>
