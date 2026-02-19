<template>
    <div>
        <v-row class="mb-4">
            <v-col>
                <h1 class="text-h4">User Management</h1>
            </v-col>
        </v-row>

        <v-alert v-if="errorMessage" type="error" variant="tonal" closable class="mb-4"
            @click:close="errorMessage = null">
            {{ errorMessage }}
        </v-alert>

        <v-card>
            <v-data-table :headers="headers" :items="users" :loading="loading" item-value="id"
                no-data-text="No users found">
                <template #item.roles="{ item }">
                    <v-chip v-for="role in item.roles" :key="role" size="small" class="mr-1" :color="roleColor(role)">
                        {{ role }}
                    </v-chip>
                    <span v-if="!item.roles.length" class="text-grey">No roles</span>
                </template>
                <template #item.actions="{ item }">
                    <v-btn icon="mdi-delete" size="small" color="error" variant="text"
                        :disabled="isCurrentUser(item.email)" @click="confirmDelete(item)">
                    </v-btn>
                </template>
            </v-data-table>
        </v-card>

        <!-- Delete Confirmation Dialog -->
        <v-dialog v-model="deleteDialog" max-width="420">
            <v-card>
                <v-card-title>Delete User</v-card-title>
                <v-card-text>
                    Are you sure you want to delete <strong>{{ userToDelete?.email }}</strong>?
                    This action cannot be undone.
                </v-card-text>
                <v-card-actions>
                    <v-spacer />
                    <v-btn variant="text" @click="deleteDialog = false">Cancel</v-btn>
                    <v-btn color="error" variant="elevated" :loading="deleting" @click="deleteUser">Delete</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </div>
</template>

<script setup lang="ts">
const { apiFetch } = useApiFetch()
const { email: currentEmail } = useAuth()

interface UserDto {
    id: string
    email: string
    roles: string[]
}

const users = ref<UserDto[]>([])
const loading = ref(false)
const errorMessage = ref<string | null>(null)
const deleteDialog = ref(false)
const userToDelete = ref<UserDto | null>(null)
const deleting = ref(false)

const headers = [
    { title: 'Email', key: 'email' },
    { title: 'Roles', key: 'roles', sortable: false },
    { title: 'Actions', key: 'actions', sortable: false, align: 'end' as const },
]

function roleColor(role: string) {
    switch (role) {
        case 'Admin': return 'red'
        case 'Stylist': return 'blue'
        case 'Customer': return 'green'
        default: return 'grey'
    }
}

function isCurrentUser(userEmail: string) {
    return userEmail === currentEmail.value
}

async function fetchUsers() {
    loading.value = true
    errorMessage.value = null
    try {
        const { data, response } = await apiFetch<UserDto[]>('/User/List')
        if (response.ok && data) {
            users.value = data
        } else if (response.status === 403) {
            errorMessage.value = 'You do not have permission to view users.'
        } else {
            errorMessage.value = 'Failed to load users.'
        }
    } catch {
        errorMessage.value = 'Failed to load users.'
    } finally {
        loading.value = false
    }
}

function confirmDelete(user: UserDto) {
    userToDelete.value = user
    deleteDialog.value = true
}

async function deleteUser() {
    if (!userToDelete.value) return

    deleting.value = true
    try {
        const { response } = await apiFetch(`/User/${userToDelete.value.id}`, {
            method: 'DELETE',
        })

        if (response.ok) {
            deleteDialog.value = false
            userToDelete.value = null
            await fetchUsers()
        } else if (response.status === 400) {
            const text = await response.text()
            errorMessage.value = text || 'Cannot delete this user.'
        } else {
            errorMessage.value = 'Failed to delete user.'
        }
    } catch {
        errorMessage.value = 'Failed to delete user.'
    } finally {
        deleting.value = false
    }
}

onMounted(() => {
    fetchUsers()
})
</script>
