<template>
    <div class="text-center pa-8">
        <h1 class="text-h3 mb-4">Welcome to Salon Management</h1>
        <v-btn color="primary" to="/stylists-list" size="large">
            View Stylists
        </v-btn>
    </div>
</template>

<script setup lang="ts">
// Automatically redirect to stylists list
navigateTo('/stylists-list');
</script>
