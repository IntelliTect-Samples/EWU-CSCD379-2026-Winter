<template>
  <div class="page">
    <div class="card form-card">
      <h2>Bakery Admin</h2>

      <div v-if="error" class="error">
        {{ error }}
      </div>

      <form @submit.prevent="handleSubmit" class="form-grid">
        <input
          v-model="email"
          type="email"
          placeholder="Email"
        />

        <input
          v-model="password"
          :type="showPassword ? 'text' : 'password'"
          placeholder="Password"
        />

        <button type="submit">
          Login
        </button>
      </form>
    </div>
  </div>
</template>

<script setup>
definePageMeta({ public: true })

const email = ref('')
const password = ref('')
const error = ref(null)

const { login, isAdmin } = useAuth()

const handleSubmit = async () => {
  try {
    error.value = null
    await login(email.value, password.value)

    if (isAdmin.value) {
      navigateTo('/admin/dashboard')
    } else {
      error.value = 'You are not an admin'
    }
  } catch (err) {
    error.value = err.message
  }
}
</script>

