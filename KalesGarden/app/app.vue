<template>
  <div>
    <nav class="container">
      <ul>
        <li><strong>🌿 Kale's Garden</strong></li>
      </ul>
      <ul>
        <li><NuxtLink to="/">Gallery</NuxtLink></li>
        <li v-if="user">
          <NuxtLink to="/commission">Commission</NuxtLink>
        </li>
        <li v-if="user">
          <NuxtLink to="/account">My Account</NuxtLink>
        </li>
        <li v-if="isAdmin">
          <NuxtLink to="/admin">Admin Account</NuxtLink>
        </li>
        <li v-if="!user">
          <NuxtLink to="/login">Login</NuxtLink>
        </li>
        <li v-if="user">
          <a href="#" @click.prevent="handleLogout">Logout</a>
        </li>
      </ul>
    </nav>
    <NuxtPage />
  </div>
</template>

<script setup lang="ts">
import { useAuth } from "./composables/useAuth";
import { computed, onMounted } from "vue";
import { useRouter } from "#imports";

const { user, logout, restoreToken, fetchUser, token } = useAuth();
const router = useRouter();

const isAdmin = computed(() => {
  return user.value?.roles?.includes("Admin") ?? false;
});

// Restore session from localStorage on client mount
onMounted(async () => {
  restoreToken();
  if (token.value && !user.value) {
    await fetchUser();
  }
});

function handleLogout() {
  logout();
  router.push("/login");
}
</script>
