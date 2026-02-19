<template>
    <v-container class="fill-height d-flex align-center justify-center">
        <v-card width="420" class="pa-6" elevation="8" rounded="lg">
            <v-card-title class="text-center pb-0">
                <div class="text-h5 font-weight-black text-uppercase">
                    Death Metal Salon
                </div>
                <div class="text-subtitle-1 text-medium-emphasis mt-1">
                    (only for bald men)
                </div>
            </v-card-title>

            <v-card-text class="pt-6">
                <v-alert v-if="error" type="error" variant="tonal" density="compact" closable class="mb-4"
                    @click:close="clearError">
                    {{ error }}
                </v-alert>

                <v-form @submit.prevent="handleSubmit">
                    <v-text-field v-model="email" label="Username" type="email" variant="outlined" density="comfortable"
                        prepend-inner-icon="mdi-account" class="mb-2" :rules="[rules.required, rules.email]"
                        :disabled="isLoading" />

                    <v-text-field v-model="password" label="Password" :type="showPassword ? 'text' : 'password'"
                        variant="outlined" density="comfortable" prepend-inner-icon="mdi-lock"
                        :append-inner-icon="showPassword ? 'mdi-eye-off' : 'mdi-eye'" class="mb-2"
                        :rules="[rules.required]" :disabled="isLoading"
                        @click:append-inner="showPassword = !showPassword" />

                    <v-text-field v-if="isRegisterMode" v-model="confirmPassword" label="Confirm Password"
                        :type="showPassword ? 'text' : 'password'" variant="outlined" density="comfortable"
                        prepend-inner-icon="mdi-lock-check" class="mb-2" :rules="[rules.required, rules.passwordMatch]"
                        :disabled="isLoading" />

                    <div class="text-center mb-4">
                        <a href="#" class="text-caption text-decoration-none" @click.prevent="toggleMode">
                            {{ isRegisterMode ? 'Already have an account? Login' : 'Register' }}
                        </a>
                    </div>

                    <v-btn type="submit" color="primary" variant="elevated" size="large" block :loading="isLoading"
                        :disabled="isLoading">
                        {{ isRegisterMode ? 'Register' : 'Login' }}
                    </v-btn>
                </v-form>
            </v-card-text>
        </v-card>
    </v-container>
</template>

<script setup lang="ts">
definePageMeta({ public: true })

const { login, register, isLoading, error } = useAuth()

const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const showPassword = ref(false)
const isRegisterMode = ref(false)

const rules = {
    required: (v: string) => !!v || 'Required',
    email: (v: string) => /.+@.+\..+/.test(v) || 'Must be a valid email',
    passwordMatch: (v: string) => v === password.value || 'Passwords must match',
}

function toggleMode() {
    isRegisterMode.value = !isRegisterMode.value
    confirmPassword.value = ''
}

function clearError() {
    error.value = null
}

async function handleSubmit() {
    if (!email.value || !password.value) return

    if (isRegisterMode.value) {
        if (password.value !== confirmPassword.value) return
        const success = await register(email.value, password.value)
        if (success) {
            navigateTo('/stylists-list')
        }
    } else {
        const success = await login(email.value, password.value)
        if (success) {
            navigateTo('/stylists-list')
        }
    }
}
</script>
