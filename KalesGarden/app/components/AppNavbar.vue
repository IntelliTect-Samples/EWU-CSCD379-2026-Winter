<template>
  <nav class="navbar">
    <div class="navbar-brand">
      <NuxtLink to="/" class="brand-link">
        <strong>🌿 Kale's Garden</strong>
      </NuxtLink>
      <button
        class="hamburger"
        :class="{ open: menuOpen }"
        @click="menuOpen = !menuOpen"
        aria-label="Toggle navigation"
      >
        <span></span>
        <span></span>
        <span></span>
      </button>
    </div>
    <ul class="nav-links" :class="{ open: menuOpen }">
      <li><NuxtLink to="/" @click="menuOpen = false">Gallery</NuxtLink></li>
      <li v-if="user">
        <NuxtLink to="/commission" @click="menuOpen = false"
          >Commission</NuxtLink
        >
      </li>
      <li v-if="user">
        <NuxtLink to="/account" @click="menuOpen = false">My Account</NuxtLink>
      </li>
      <li v-if="isAdmin">
        <NuxtLink to="/admin" @click="menuOpen = false">Admin</NuxtLink>
      </li>
      <li v-if="!user">
        <NuxtLink to="/login" @click="menuOpen = false">Login</NuxtLink>
      </li>
      <li v-if="user">
        <a href="#" @click.prevent="handleLogout">Logout</a>
      </li>
    </ul>
  </nav>
</template>

<script setup lang="ts">
import { ref, computed } from "vue";
import { useAuth } from "~/composables/useAuth";

const emit = defineEmits<{ logout: [] }>();
const { user } = useAuth();
const menuOpen = ref(false);

const isAdmin = computed(() => user.value?.roles?.includes("Admin") ?? false);

function handleLogout() {
  menuOpen.value = false;
  emit("logout");
}
</script>

<style scoped>
.navbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.75rem 1.5rem;
  flex-wrap: wrap;
}

.navbar-brand {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: auto;
}

.brand-link {
  text-decoration: none;
  color: inherit;
}

.nav-links {
  display: flex;
  list-style: none;
  margin: 0;
  padding: 0;
  gap: 0.25rem;
  align-items: center;
}

.nav-links li a {
  padding: 0.5rem 0.75rem;
  white-space: nowrap;
}

.hamburger {
  display: none;
  flex-direction: column;
  justify-content: center;
  gap: 5px;
  width: 36px;
  height: 36px;
  background: none;
  border: none;
  cursor: pointer;
  padding: 4px;
  margin: 0;
}

.hamburger span {
  display: block;
  width: 24px;
  height: 2px;
  background: var(--pico-color);
  border-radius: 2px;
  transition:
    transform 0.3s,
    opacity 0.3s;
}

.hamburger.open span:nth-child(1) {
  transform: translateY(7px) rotate(45deg);
}
.hamburger.open span:nth-child(2) {
  opacity: 0;
}
.hamburger.open span:nth-child(3) {
  transform: translateY(-7px) rotate(-45deg);
}

@media (max-width: 768px) {
  .navbar {
    flex-wrap: wrap;
  }

  .navbar-brand {
    width: 100%;
    justify-content: space-between;
  }

  .hamburger {
    display: flex;
  }

  .nav-links {
    display: none;
    flex-direction: column;
    width: 100%;
    padding: 0.5rem 0;
  }

  .nav-links.open {
    display: flex;
  }

  .nav-links li {
    width: 100%;
  }

  .nav-links li a {
    display: block;
    padding: 0.75rem 0;
  }
}
</style>
