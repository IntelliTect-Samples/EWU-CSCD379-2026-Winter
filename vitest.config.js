import { defineVitestConfig } from '@nuxt/test-utils/config'

export default defineVitestConfig({
  test: {
    environment: 'nuxt',
    globals: true,
    pool: 'threads',
    threads: {
      singleThread: true,
    },
    environmentOptions: {
      nuxt: {
        domEnvironment: 'happy-dom',
      }
    },
    dangerouslyIgnoreUnhandledErrors: true,
    onConsoleLog(log) {
      if (log.includes('next is not a function') || log.includes('<Suspense>')) {
        return false
      }
    },
  }
})