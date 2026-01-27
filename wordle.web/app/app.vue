<template>
    <v-layout>
        <v-app-bar color="primary" density="compact">
            <template v-slot:prepend>
                <v-app-bar-nav-icon></v-app-bar-nav-icon>
            </template>

            <v-app-bar-title>
                <v-btn to="/">

                    Wordle
                </v-btn>
            </v-app-bar-title>
            <v-btn to="wordle" icon>
                <v-icon>mdi-heart</v-icon>
            </v-btn>
            <template v-slot:append>
                <v-btn icon="mdi-dots-vertical"></v-btn>
            </template>
        </v-app-bar>
        <v-main>
            <v-container class="mx-auto">
                <NuxtPage />
            </v-container>
        </v-main>
    </v-layout>
</template>

<script setup lang="ts">

</script>