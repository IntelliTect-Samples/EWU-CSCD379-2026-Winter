<template>
  <div class="app-shell">
    <NuxtPage />
  </div>
</template>

<style>
.app-shell {
  background: var(--bg-dark);
  min-height: 100vh;
  width: 100%;
  overflow-x: hidden;
}

/* Effects for page changes */
.page-enter-active,
.page-leave-active {
  transition: all 0.3s ease;
}
.page-enter-from,
.page-leave-to {
  opacity: 0;
  filter: blur(1rem);
}
</style>