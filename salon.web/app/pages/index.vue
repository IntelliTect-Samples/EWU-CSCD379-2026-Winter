<template>
    <div class="text-center pa-8">
        <h1 class="text-h3 mb-4">Welcome to Salon Management</h1>
        <v-btn color="primary" to="/stylists-list" size="large" class="mr-4">
            View Stylists
        </v-btn>
        <v-btn v-if="isAdmin" color="secondary" to="/users" size="large">
            Manage Users
        </v-btn>
    </div>
</template>

<script setup lang="ts">
const { roles } = useAuth()
const isAdmin = computed(() => roles.value.includes('Admin'))
</script>
