<template>
    <v-app>
        <v-app-bar color="primary" density="compact">
            <v-app-bar-title>Salon Management</v-app-bar-title>
            <template #append>
                <div v-if="isAuthenticated" class="d-flex align-center ga-2">
                    <span class="text-body-2">{{ email }}</span>
                    <v-btn variant="outlined" size="small" @click="handleLogout">
                        Logout
                    </v-btn>
                </div>
                <v-btn v-else variant="outlined" size="small" to="/login">
                    Login
                </v-btn>
            </template>
        </v-app-bar>
        <v-main>
            <v-container>
                <NuxtPage />
            </v-container>
        </v-main>
    </v-app>
</template>

<script setup lang="ts">
const { isAuthenticated, email, logout } = useAuth()

function handleLogout() {
    logout()
    navigateTo('/login')
}
</script>
