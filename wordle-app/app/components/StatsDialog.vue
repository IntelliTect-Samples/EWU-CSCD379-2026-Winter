<script setup lang="ts">
const props = defineProps<{
    modelValue: boolean
    wins: number
    losses: number
    avgAttempts: number
}>()

const emit = defineEmits<{
    (e: 'update:modelValue', v: boolean): void
    (e: 'reset'): void
}>()

function close() {
    emit('update:modelValue', false)
}
</script>

<template>
    <v-dialog :model-value="props.modelValue" max-width="420" @update:model-value="(v) => emit('update:modelValue', v)">
    <v-card>
        <v-card-title class="text-h6 font-weight-bold">Stats</v-card-title>

        <v-card-text>
        <div class="stat-grid">
            <div class="stat">
            <div class="label">Wins</div>
            <div class="value">{{ props.wins }}</div>
            </div>

            <div class="stat">
            <div class="label">Losses</div>
            <div class="value">{{ props.losses }}</div>
            </div>

            <div class="stat" style="grid-column: 1 / -1;">
            <div class="label">Average attempts (wins only)</div>
            <div class="value">{{ props.avgAttempts.toFixed(2) }}</div>
            </div>
        </div>
        </v-card-text>

        <v-card-actions>
        <v-btn variant="text" @click="emit('reset')">Reset</v-btn>
        <v-spacer />
        <v-btn color="primary" @click="close">Close</v-btn>
        </v-card-actions>
    </v-card>
    </v-dialog>
</template>

<style scoped>
.stat-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 14px;
}

.stat {
    padding: 12px;
    border-radius: 12px;
    background: rgba(0,0,0,0.04);
}

.label {
    font-size: 0.9rem;
    opacity: 0.8;
}

.value {
    font-size: 1.6rem;
    font-weight: 800;
    margin-top: 4px;
}
</style>
