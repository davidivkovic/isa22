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

              <div
                v-for="role in user.roles.filter(r => r != 'Customer')"
                :key="role"
                class="-my-1.5 whitespace-nowrap rounded-full border border-neutral-300 px-2.5 py-1 text-sm font-medium"
              >
                {{ role }}
              </div>
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
        <div
          :class="[
            {
              'text-white': loyaltyLevel.current?.color == '#000000',
              border: loyaltyLevel.current == null
            },
            `relative  flex flex-1  items-center justify-between space-x-5 overflow-clip rounded-lg border-neutral-300 py-4 px-5 bg-[${
              loyaltyLevel.current?.color ?? '#ebebeb'
            }]`
          ]"
        >
          <div class="w-3/4">
            <div class="flex items-center space-x-1">
              <h3 class="text-lg font-semibold">
                {{ loyaltyLevel.current?.name ?? 'Regular' }} Member
              </h3>
              <DiamondIcon class="h-5 w-5 text-black" />
            </div>
            <h2 class="mt-3 font-semibold">{{ loyaltyLevel.points }} Points</h2>
            <div class="relative mt-2">
              <div
                class="absolute h-2 w-full rounded-lg bg-black opacity-[15%]"
              ></div>
              <div
                :style="{
                  width:
                    (loyaltyLevel.points / loyaltyLevel.next?.threshold) * 100 +
                    '%'
                }"
                class="absolute h-2 rounded-lg bg-black"
              ></div>
            </div>
          </div>
          <div
            class="flex h-28 w-28 items-center justify-center space-x-px rounded-full bg-neutral-100 text-2xl font-semibold"
          >
            <p>
              {{ user.firstName[0].toUpperCase() }}
            </p>
            <p>
              {{ user.lastName[0].toUpperCase() }}
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRoute } from 'vue-router'
import { DiamondIcon } from 'vue-tabler-icons'
import api from '@/api/api'

const route = useRoute()
const user = ref()
const loyaltyLevel = ref()

const [profileData, profileError] = await api.users.getUser(route.params.id)
if (!profileError) {
  user.value = profileData.user
  loyaltyLevel.value = profileData.loyaltyLevel
}
</script>
