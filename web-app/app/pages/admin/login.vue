<template>
  <div class="login-wrapper">
    <div class="login-card">
      <h2>Bakery Admin</h2>
      <p>Admin Login Only</p>

      <div v-if="error" class="error">
        {{ error }}
      </div>

      <form @submit.prevent="handleSubmit">
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

<style scoped>
.login-wrapper {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
}

.login-card {
  width: 350px;
  padding: 30px;
  border-radius: 12px;
  background: white;
  box-shadow: 0 10px 25px rgba(0,0,0,0.1);
  text-align: center;
}

input {
  width: 100%;
  margin-bottom: 12px;
  padding: 10px;
  border-radius: 6px;
  border: 1px solid #ccc;
}

button {
  width: 100%;
  padding: 10px;
  margin-top: 8px;
  border: none;
  border-radius: 6px;
  background: #ff6b6b;
  color: white;
  cursor: pointer;
}

.error {
  background: #ffe5e5;
  color: #cc0000;
  padding: 8px;
  margin-bottom: 12px;
  border-radius: 6px;
}

.register-link {
  margin-top: 12px;
  font-size: 14px;
}

.register-link a {
  color: #ff6b6b;
  text-decoration: none;
  font-weight: 600;
}
</style>