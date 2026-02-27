<template>
  <main class="container auth-container">
    <article>
      <div class="auth-tabs">
        <button :class="{ active: mode === 'login' }" @click="mode = 'login'">
          Sign In
        </button>
        <button :class="{ active: mode === 'signup' }" @click="mode = 'signup'">
          Sign Up
        </button>
      </div>

      <!-- Login Form -->
      <form v-if="mode === 'login'" @submit.prevent="handleLogin">
        <hgroup>
          <h2>Welcome Back</h2>
          <p>Sign in to your account</p>
        </hgroup>

        <label for="login-email">
          Email
          <input
            id="login-email"
            v-model="email"
            type="email"
            placeholder="you@example.com"
            autocomplete="email"
            required
          />
        </label>

        <label for="login-password">
          Password
          <input
            id="login-password"
            v-model="password"
            type="password"
            placeholder="Your password"
            autocomplete="current-password"
            required
          />
        </label>

        <button type="submit" :aria-busy="loading">
          {{ loading ? "Signing in..." : "Sign In" }}
        </button>

        <p class="switch-text">
          Don't have an account?
          <a href="#" @click.prevent="mode = 'signup'">Sign up</a>
        </p>
      </form>

      <!-- Signup Form -->
      <form v-else @submit.prevent="handleSignup">
        <hgroup>
          <h2>Create Account</h2>
          <p>Sign up, then start commissioning art</p>
        </hgroup>

        <label for="signup-email">
          Email
          <input
            id="signup-email"
            v-model="signupEmail"
            type="email"
            placeholder="you@example.com"
            autocomplete="email"
            required
          />
        </label>

        <label for="signup-password">
          Password
          <input
            id="signup-password"
            v-model="signupPassword"
            type="password"
            placeholder="Min 6 chars, upper + lower + digit"
            autocomplete="new-password"
            required
            minlength="6"
          />
        </label>

        <label for="signup-confirm">
          Confirm Password
          <input
            id="signup-confirm"
            v-model="signupConfirm"
            type="password"
            placeholder="Re-enter your password"
            autocomplete="new-password"
            required
          />
        </label>

        <button type="submit" :aria-busy="signupLoading">
          {{ signupLoading ? "Creating account..." : "Create Account" }}
        </button>

        <p class="switch-text">
          Already have an account?
          <a href="#" @click.prevent="mode = 'login'">Sign in</a>
        </p>
      </form>

      <p v-if="errorMsg" role="alert" class="error-msg">{{ errorMsg }}</p>
      <p v-if="successMsg" role="alert" class="success-msg">{{ successMsg }}</p>
    </article>
  </main>
</template>

<script setup lang="ts">
import { useAuth } from "~/composables/useAuth";
import { useRouter } from "#imports";
import { ref } from "vue";

const { login, register } = useAuth();
const router = useRouter();

const mode = ref<"login" | "signup">("login");

// Login state
const email = ref("");
const password = ref("");
const loading = ref(false);

// Signup state
const signupEmail = ref("");
const signupPassword = ref("");
const signupConfirm = ref("");
const signupLoading = ref(false);

// Shared messages
const errorMsg = ref("");
const successMsg = ref("");

async function handleLogin() {
  loading.value = true;
  errorMsg.value = "";
  successMsg.value = "";
  try {
    await login(email.value, password.value);
    successMsg.value = "Logged in successfully! Redirecting...";
    setTimeout(() => router.push("/"), 1000);
  } catch (err: any) {
    errorMsg.value =
      err?.message || "Login failed. Please check your credentials.";
  } finally {
    loading.value = false;
  }
}

async function handleSignup() {
  errorMsg.value = "";
  successMsg.value = "";

  if (signupPassword.value !== signupConfirm.value) {
    errorMsg.value = "Passwords do not match.";
    return;
  }

  signupLoading.value = true;
  try {
    await register(signupEmail.value, signupPassword.value);
    successMsg.value = "Account created! Redirecting...";
    setTimeout(() => router.push("/"), 1000);
  } catch (err: any) {
    const detail = err?.data?.errors;
    if (detail && typeof detail === "object") {
      // ASP.NET Identity returns validation errors as { "FieldName": ["error1", "error2"] }
      errorMsg.value = Object.values(detail).flat().join(" ");
    } else {
      errorMsg.value =
        err?.data?.detail ||
        err?.message ||
        "Registration failed. Please try again.";
    }
  } finally {
    signupLoading.value = false;
  }
}
</script>

<style scoped>
.auth-container {
  max-width: 480px;
}

.switch-text {
  text-align: center;
  font-size: 0.9rem;
  margin-top: 1rem;
}

.error-msg {
  color: var(--pico-del-color);
}

.success-msg {
  color: var(--pico-ins-color);
}
</style>
