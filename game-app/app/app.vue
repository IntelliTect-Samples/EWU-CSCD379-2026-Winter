<template>
  <div class="app-shell">
    <ClientOnly>
      <NuxtPage />
      <template #fallback>
        <div class="loading-state">
          Loading Grid Snap...
        </div>
      </template>
    </ClientOnly>
  </div>
</template>

<style>
.app-shell {
  background: var(--bg-dark);
  min-height: 100vh;
  width: 100%;
  overflow-x: hidden;
}

.loading-state {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  font-family: sans-serif;
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