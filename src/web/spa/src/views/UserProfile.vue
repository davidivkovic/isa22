<template>
  <div class="mx-auto mb-10 flex max-w-4.5xl space-x-10">
    <div class="flex-1">
      <h1 class="text-2xl font-medium">User Profile</h1>
      <div class="mt-2 flex space-x-5">
        <div class="flex space-x-4 rounded-lg border border-neutral-300 p-5">
          <div>
            <div class="flex items-center space-x-3">
              <h1 class="whitespace-nowrap text-lg font-medium leading-5">
                {{ user.firstName }} {{ user.lastName }}
              </h1>
            </div>
            <h2 class="text-sm text-neutral-500">{{ user.email }}</h2>
            <h3 class="text-sm text-neutral-500">{{ user.phoneNumber }}</h3>
            <h3 class="mt-1 text-sm text-neutral-500">
              {{ user.address.street }} {{ user.address.apartment }}
            </h3>
            <h3 class="text-sm text-neutral-500">
              {{ user.address.city }}, {{ user.address.country }}
            </h3>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRoute } from 'vue-router'
import api from '@/api/api'

const route = useRoute()
const user = ref()

const [profileData, profileError] = await api.users.getUser(route.params.id)
if (!profileError) {
  user.value = profileData
}
</script>
