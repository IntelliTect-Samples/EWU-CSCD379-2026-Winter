<script setup lang="ts">
import { de } from 'vuetify/locale';
import type { WordDefinition } from '~/composables/useDefinition'

const props = defineProps<{
    modelValue: boolean
    definition: WordDefinition | null
}>()

const emit = defineEmits<{
    (e: 'update:modelValue', v: boolean): void
}>()

function close() {
    emit('update:modelValue', false)
}
</script>

<template>
    <v-dialog
        :model-value="props.modelValue"
        max-width="520"
        @update:model-value="(v) => emit('update:modelValue', v)"
>
    <v-card>
        <v-card-title class="text-h6 font-weight-bold">
            {{ props.definition?.word?.toUpperCase() ?? 'Definition' }}
        <span v-if="props.definition?.phonetic" style="opacity:.75; font-weight:600; margin-left:10px;">
            {{ props.definition.phonetic }}
        </span>
    </v-card-title>

    <v-card-text>
        <div v-if="props.definition?.partOfSpeech" style="opacity:.8; margin-bottom:10px;">
            <b>{{ props.definition.partOfSpeech }}</b>
        </div>

        <div style="font-size: 1.05rem; line-height: 1.45;">
            {{ props.definition?.definition ?? 'No definition found.' }}
        </div>

        <div v-if="props.definition?.example" style="margin-top:12px; opacity:.85;">
            <i>Example:</i> “{{ props.definition.example }}”
        </div>

        <div v-if="props.definition?.source" style="margin-top:14px; opacity:.6; font-size:.9rem;">
            Source: {{ props.definition.source }}
        </div>
        </v-card-text>

    <v-card-actions>
        <v-spacer />
        <v-btn color="primary" @click="close">Close</v-btn>
    </v-card-actions>
    </v-card>
    </v-dialog>
</template>
