<template>
  <div class="app-shell">
    <Head>
      <Meta
      name="viewport"
      content="width=device-width, initial-scale=1, viewport-fit=cover"
      />
    </Head>
    <NuxtPage />
  </div>
</template>

<style>
:root {
  --bg: #121213;
  --text: #ffffff;
  --tile-border: #3a3a3c;
  --tile-empty: transparent;

  --key-bg: #818384;
  --key-text: #ffffff;

  --correct: #6aaa64;
  --present: #c9b458;
  --absent: #3a3a3c;
}

html, body, #__nuxt {
  height: 100%;
}

body {
  margin: 0;
  background: var(--bg);
  color: var(--text);
  font-family: system-ui, -apple-system, Segoe UI, Roboto, Arial, sans-serif;

  overflow-x: hidden;
  touch-action: manipulation;
  -webkit-text-size-adjust: 100%;
}

.app-shell {
  min-height: 100vh;
  background: var(--bg);
  color: var(--text);

  padding-left: env(safe-area-inset-left);
  padding-right: env(safe-area-inset-right);
}
</style>
