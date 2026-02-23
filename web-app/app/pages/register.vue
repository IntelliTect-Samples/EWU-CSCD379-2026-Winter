<template>
  <div class="login-wrapper">
    <div class="login-card">
      <h2>Create Account</h2>

      <div v-if="error" class="error">
        {{ error }}
      </div>

      <form @submit.prevent="handleRegister">
        <input v-model="email" type="email" placeholder="Email" />
        <input v-model="password" type="password" placeholder="Password" />

        <button type="submit">Register</button>
      </form>
    </div>
  </div>
</template>

<script setup>
definePageMeta({ public: true })

const email = ref('')
const password = ref('')
const error = ref(null)

const { register } = useAuth()

const handleRegister = async () => {
  try {
    error.value = null
    await register(email.value, password.value)
    navigateTo('/admin/login')
  } catch (err) {
    error.value = err.message
  }
}
</script>