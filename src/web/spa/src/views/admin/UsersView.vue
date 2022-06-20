<template>
  <div class="mx-auto max-w-4.5xl">
    <h1 class="text-2xl font-medium">Users</h1>
    <h2 class="text-gray-600">
      These are the registered users. You can search and delete them.
    </h2>
    <div class="mt-4 w-56">
      <Input
        v-model="query"
        class="w-full"
        placeholder="First Name, Last Name or Email"
      />
    </div>
    <div class="mt-4 grid auto-cols-fr grid-cols-3 gap-4">
      <div
        v-for="user in users"
        :key="user.id"
        class="rounded-md border px-4 py-2.5"
      >
        <div>
          <div class="flex items-center space-x-3">
            <h1 class="whitespace-nowrap text-lg font-medium leading-5">
              {{ user.firstName }} {{ user.lastName }}
            </h1>

            <div
              v-for="role in user.roles.filter(r => r != 'Customer')"
              :key="role"
              class="whitespace-nowrap rounded-full border border-neutral-300 px-2.5 py-1 text-sm font-medium"
            >
              {{ role }}
            </div>
          </div>
          <h2 class="text-sm text-neutral-500">{{ user.email }}</h2>
          <h3 class="text-sm text-neutral-500">{{ user.phoneNumber }}</h3>
          <div class="flex items-center justify-between space-x-2.5">
            <div>
              <h3 class="mt-1 text-sm text-neutral-500">
                {{ user.address.street }} {{ user.address.apartment }}
              </h3>
              <h3 class="text-sm text-neutral-500">
                {{ user.address.city }}, {{ user.address.country }}
              </h3>
            </div>
            <Button
              v-if="!user.roles.includes('Admin')"
              @click="
                () => {
                  userToDelete = user
                  deletionModalOpen = true
                }
              "
              class="ml-auto space-x-1.5 border border-red-800 border-opacity-10 bg-red-600 !px-5 !py-1.5 text-white transition hover:bg-red-700"
            >
              Delete
            </Button>
          </div>
        </div>
      </div>
    </div>
    <div
      v-if="users.length"
      class="mt-10 flex justify-between space-x-10 text-sm"
    >
      <p>
        Showing
        <span class="font-medium"> {{ resultsFrom }}</span> to
        <span class="font-medium">
          {{ resultsTo }}
        </span>
        of <span class="font-medium"> {{ totalResults }} </span> results
      </p>
      <div class="flex space-x-5">
        <button
          @click="previousPage()"
          v-if="hasPrevious"
          class="flex items-center space-x-2 hover:underline"
        >
          <ArrowLeftIcon class="h-4 w-4" />
          <p>Previous</p>
        </button>
        <p>
          Page <span class="font-medium">{{ currentPage }}</span> of
          <span class="font-medium">{{ totalPages }} </span>
        </p>
        <button
          @click="nextPage()"
          v-if="hasNext"
          class="flex items-center space-x-2 hover:underline"
        >
          <p>Next</p>
          <ArrowRightIcon class="h-4 w-4" />
        </button>
      </div>
    </div>
    <DeleteUserModal
      :is-open="deletionModalOpen"
      :user="userToDelete"
      @modal-closed="deletionModalOpen = false"
      @user-deleted="userDeleted()"
    />
  </div>
</template>

<script setup>
import { ref, watch, computed } from 'vue'
import api from '@/api/api.js'
import Button from '@/components/ui/Button.vue'
import Input from '@/components/ui/Input.vue'
import DeleteUserModal from '@/components/admin/DeleteUserModal.vue'
import { debounce } from '@/components/utility/forms'
import { ArrowLeftIcon, ArrowRightIcon } from 'vue-tabler-icons'

const users = ref([])
const query = ref('')
const currentPage = ref(1)
const deletionModalOpen = ref(false)
const userToDelete = ref()

const userDeleted = () => {
  users.value = users.value.filter(u => u.id != userToDelete.value?.id)
  deletionModalOpen.value = false
}

const nextPage = () => {
  currentPage.value++
  search()
}

const previousPage = () => {
  currentPage.value--
  search()
}

const pageSize = 9
const totalPages = ref(1)
const totalResults = ref(0)
const resultsFrom = computed(() => 1 + pageSize * (currentPage.value - 1))
const resultsTo = computed(
  () => pageSize * (currentPage.value - 1) + users.value.length
)
const hasNext = computed(() => currentPage.value < totalPages.value)
const hasPrevious = computed(() => currentPage.value > 1)

const search = async () => {
  if (query.value != '') currentPage.value = 1
  const [data, error] = await api.users.searchUsers(
    query.value,
    currentPage.value - 1 || 0
  )
  if (!error) {
    users.value = data.results
    totalResults.value = data.totalResults
    totalPages.value = Math.ceil(totalResults.value / pageSize)
  }
}

await search()
watch(
  query,
  debounce(() => search(), 200)
)
</script>
