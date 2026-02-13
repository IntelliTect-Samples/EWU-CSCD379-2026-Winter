<template>
    <v-dialog :model-value="modelValue" @update:model-value="$emit('update:modelValue', $event)" max-width="600">
        <v-card>
            <v-card-title>
                {{ isEditMode ? 'Edit Stylist' : 'Add Stylist' }}
            </v-card-title>
            <v-card-text>
                <v-form ref="formRef" v-model="valid">
                    <v-text-field v-model="formData.name" label="Name" :rules="[(v: any) => !!v || 'Name is required']"
                        required></v-text-field>

                    <v-text-field v-model="formData.phoneNumber" label="Phone Number"
                        :rules="[(v: any) => !!v || 'Phone number is required']" required></v-text-field>

                    <v-text-field v-model="formData.chairName" label="Chair Name"
                        :rules="[(v: any) => !!v || 'Chair name is required']" required></v-text-field>

                    <v-text-field v-model.number="formData.workStartTime24H"
                        label="Work Start Time (24H, e.g., 9.0 or 9.5)" type="number" step="0.5" min="0" max="24"
                        :rules="[(v: any) => v !== null && v !== undefined || 'Start time is required']"
                        required></v-text-field>

                    <v-text-field v-model.number="formData.workEndTime24H"
                        label="Work End Time (24H, e.g., 17.0 or 17.5)" type="number" step="0.5" min="0" max="24"
                        :rules="[(v: any) => v !== null && v !== undefined || 'End time is required']"
                        required></v-text-field>

                    <!-- Image Section -->
                    <div v-if="isEditMode && formData.imageUrl" class="mb-4">
                        <v-img :src="formData.imageUrl" max-height="200" contain></v-img>
                    </div>

                    <v-file-input v-model="imageFile" label="Upload Image" accept="image/*" prepend-icon="mdi-camera"
                        clearable></v-file-input>
                </v-form>
            </v-card-text>
            <v-card-actions>
                <v-btn v-if="isEditMode" color="error" @click="confirmDelete">
                    Delete
                </v-btn>
                <v-spacer></v-spacer>
                <v-btn @click="close">Cancel</v-btn>
                <v-btn color="primary" @click="save" :disabled="!valid">Save</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useRuntimeConfig } from '#app';

const config = useRuntimeConfig();
const apiBase = config.public.apiBase;

interface StylistDto {
    stylistId?: string;
    name: string;
    phoneNumber: string;
    chairName: string;
    workStartTime24H: number;
    workEndTime24H: number;
    imageUrl?: string | null;
    isActive: boolean;
}

const props = defineProps<{
    modelValue: boolean;
    stylist: StylistDto | null;
}>();

const emit = defineEmits<{
    'update:modelValue': [value: boolean];
    'saved': [];
}>();

const formRef = ref();
const valid = ref(false);
const imageFile = ref<File | null>(null);

const formData = ref({
    name: '',
    phoneNumber: '',
    chairName: '',
    workStartTime24H: 9.0,
    workEndTime24H: 17.0,
    imageUrl: null as string | null
});

const isEditMode = computed(() => props.stylist !== null);

// Watch for changes to the stylist prop to populate the form
watch(() => props.stylist, (newStylist: StylistDto | null) => {
    if (newStylist) {
        formData.value = {
            name: newStylist.name,
            phoneNumber: newStylist.phoneNumber,
            chairName: newStylist.chairName,
            workStartTime24H: newStylist.workStartTime24H,
            workEndTime24H: newStylist.workEndTime24H,
            imageUrl: newStylist.imageUrl ?? null
        };
    } else {
        // Reset form for new stylist
        formData.value = {
            name: '',
            phoneNumber: '',
            chairName: '',
            workStartTime24H: 9.0,
            workEndTime24H: 17.0,
            imageUrl: null
        };
    }
    imageFile.value = null;
}, { immediate: true });

const close = () => {
    emit('update:modelValue', false);
};

const save = async () => {
    if (!valid.value) return;
    try {
        const dto = {
            stylistId: props.stylist?.stylistId || null,
            name: formData.value.name,
            phoneNumber: formData.value.phoneNumber,
            chairName: formData.value.chairName,
            workStartTime24H: formData.value.workStartTime24H,
            workEndTime24H: formData.value.workEndTime24H,
            isActive: true
        };

        // Save stylist data
        const response = await fetch(`${apiBase}/Stylist`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(dto)
        });

        if (!response.ok) {
            throw new Error('Failed to save stylist');
        }

        const savedStylist = await response.json();

        // Use existing ID if editing, otherwise use the newly created ID
        const stylistId = props.stylist?.stylistId || savedStylist.stylistId;

        // Upload image if selected
        if (imageFile.value) {
            const formDataImage = new FormData();
            formDataImage.append('image', imageFile.value);

            const imageResponse = await fetch(`${apiBase}/Stylist/Image/${stylistId}`, {
                method: 'PUT',
                body: formDataImage
            });

            if (!imageResponse.ok) {
                console.error('Failed to upload image');
            }
        }

        emit('saved');
    } catch (error) {
        console.error('Error saving stylist:', error);
        alert('Failed to save stylist. Please try again.');
    }
};

const confirmDelete = async () => {
    if (!props.stylist?.stylistId) return;

    if (confirm('Are you sure you want to delete this stylist?')) {
        try {
            const response = await fetch(`${apiBase}/Stylist/${props.stylist.stylistId}`, {
                method: 'DELETE'
            });

            if (!response.ok) {
                throw new Error('Failed to delete stylist');
            }

            emit('saved');
        } catch (error) {
            console.error('Error deleting stylist:', error);
            alert('Failed to delete stylist. Please try again.');
        }
    }
};
</script>

<style scoped>
/* Additional styles if needed */
</style>