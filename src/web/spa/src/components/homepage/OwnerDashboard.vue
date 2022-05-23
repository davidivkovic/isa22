<template>
  <div class="mx-auto flex h-full w-3/4 flex-col py-8">
    <h1 class="text-4xl font-bold">👋 Welcome Milica,</h1>
    <h2 class="mt-2 text-lg text-gray-800">
      You currently have
      <span class="underline underline-offset-2"> 4 active reservations.</span>
    </h2>
    <div class="mt-10 flex w-full flex-1 space-x-5">
      <div
        class="relative h-[85%] w-1/3 rounded-3xl border border-gray-300 py-5 px-6"
      >
        <div class="flex items-center justify-between">
          <h3 class="font-medium">My Adventures</h3>
          <Button
            class="flex items-center space-x-1 !rounded-md border border-gray-300 !py-2.5"
          >
            <p class="!font-medium">Add</p>
            <PlusIcon class="h-4 w-4 stroke-[3] text-emerald-600" />
          </Button>
        </div>
        <div class="mt-5 space-y-4">
          <RouterLink
            :to="{ name: 'adventure-profile', params: { id: business.id } }"
            v-for="business in businesses"
            :key="business?.id"
            class="flex space-x-5"
          >
            <img
              :src="business.images[0]"
              alt="Business image"
              class="h-28 w-28 rounded-xl object-cover"
            />
            <div class="flex flex-col justify-between">
              <div>
                <div class="font-medium">{{ business.name }}</div>
                <div class="text-[13px] text-gray-600">
                  {{ formatAddress(business.address) }}
                </div>

                <div class="mt-1 flex items-center space-x-1 rounded-full">
                  <UserIcon class="h-4 w-4" />
                  <div>{{ business.people }}</div>
                </div>
              </div>

              <div class="text-sm text-gray-400">
                Current status:
                <span
                  class="rounded-full bg-emerald-50 px-3 py-1.5 text-[13px] font-bold text-emerald-700"
                  >Booked</span
                >
              </div>
            </div>
          </RouterLink>
        </div>
        <a href="" class="absolute bottom-6 font-medium underline">
          View all adventures
        </a>
      </div>
      <div class="h-[85%] w-1/3 rounded-3xl border border-gray-300 px-5 py-6">
        <div class="-mt-1.5 flex items-center justify-between">
          <h3 class="font-medium">Earnings Trend</h3>
          <Dropdown :slim="false" class="!px-2 !py-1" :values="incomeOptions" />
        </div>
        <div class="mt-4 text-3xl font-bold">$4,565</div>
      </div>
      <div class="h-[85%] w-1/3 rounded-3xl border border-gray-300 px-5 py-6">
        <h3 class="font-medium">Current reservations</h3>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import api from '@/api/api.js'
import { PlusIcon, UserIcon } from 'vue-tabler-icons'
import Button from '../ui/Button.vue'
import Dropdown from '../ui/Dropdown.vue'
import { formatAddress } from '@/components/utility/address.js'

const incomeOptions = [
  {
    name: 'Monthly',
    value: 'monthly'
  },
  {
    name: 'Weekly',
    value: 'weekly'
  },
  {
    name: 'Yearly',
    value: 'yearly'
  }
]
const businesses = ref([])
const fetchBusinesses = async () => {
  const [data] = await api.business.ownersBusinesses('adventures')
  data && (businesses.value = data)
}
fetchBusinesses()
</script>
