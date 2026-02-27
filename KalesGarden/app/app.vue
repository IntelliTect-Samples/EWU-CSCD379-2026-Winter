<template>
  <div>
    <AppNavbar @logout="handleLogout" />
    <NuxtPage />
  </div>
</template>

<script setup lang="ts">
import { useAuth } from "./composables/useAuth";
import { onMounted } from "vue";
import { useRouter } from "#imports";

const { logout, restoreToken, fetchUser, token, user } = useAuth();
const router = useRouter();

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
