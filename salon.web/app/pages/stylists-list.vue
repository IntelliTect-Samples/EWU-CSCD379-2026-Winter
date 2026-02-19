<template>
    <div>
        <v-row>
            <v-col v-for="stylist in stylists" :key="stylist.stylistId" cols="12" sm="6" md="4" lg="3">
                <v-card>
                    <v-img :src="stylist.imageUrl || '/person.webp'" height="200" cover>
                        <template v-slot:error>
                            <v-row class="fill-height ma-0" align="center" justify="center">
                                <v-icon size="64" color="grey-lighten-2">mdi-account-circle</v-icon>
                            </v-row>
                        </template>
                    </v-img>
                    <v-card-title>{{ stylist.name }}</v-card-title>
                    <v-card-text>
                        <div><strong>Phone:</strong> {{ stylist.phoneNumber }}</div>
                        <div><strong>Chair:</strong> {{ stylist.chairName }}</div>
                        <div><strong>Hours:</strong> {{ formatTime(stylist.workStartTime24H) }} - {{
                            formatTime(stylist.workEndTime24H) }}
                        </div>
                    </v-card-text>
                    <v-card-actions v-if="canEdit">
                        <v-spacer></v-spacer>
                        <v-btn color="primary" icon="mdi-pencil" @click="editStylist(stylist)"></v-btn>
                    </v-card-actions>
                </v-card>
            </v-col>
        </v-row>

        <!-- Floating Action Button for Adding New Stylist -->
        <v-btn v-if="canEdit" color="primary" icon="mdi-plus" size="large" position="fixed" location="bottom right"
            class="ma-4" @click="addStylist"></v-btn>

        <!-- Stylist Dialog -->
        <StylistDialog v-model="dialogVisible" :stylist="selectedStylist" @saved="refreshList" />
    </div>
</template>

<script setup lang="ts">
import StylistDialog from '../../components/StylistDialog.vue';

const { apiFetch } = useApiFetch();
const { roles } = useAuth();

const canEdit = computed(() => roles.value.includes('Admin') || roles.value.includes('Stylist'));

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

const stylists = ref<StylistDto[]>([]);
const dialogVisible = ref(false);
const selectedStylist = ref<StylistDto | null>(null);

// Fetch stylists on mount
const fetchStylists = async () => {
    try {
        const { data } = await apiFetch<StylistDto[]>('/Stylist/List');
        if (data) {
            stylists.value = data;
        }
    } catch (error) {
        console.error('Error fetching stylists:', error);
    }
};

// Format time from decimal to HH:MM
const formatTime = (decimalTime: number) => {
    const hours = Math.floor(decimalTime);
    const minutes = Math.round((decimalTime - hours) * 60);
    return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}`;
};

const editStylist = (stylist: StylistDto) => {
    selectedStylist.value = { ...stylist };
    dialogVisible.value = true;
};

const addStylist = () => {
    selectedStylist.value = null;
    dialogVisible.value = true;
};

const refreshList = () => {
    fetchStylists();
    dialogVisible.value = false;
};

// Initial fetch
onMounted(() => {
    fetchStylists();
});
</script>

<style scoped>
/* Additional styles if needed */
</style>
