<template>
  <div class="mx-auto mt-3 mb-8 flex h-full w-4/5 max-w-7xl flex-col">
    <h1 class="text-4xl font-bold">👋 Welcome {{ user.firstName }}</h1>
    <h2 class="mt-2 text-lg text-gray-800">
      You currently have
      <span :class="reservations.length > 0 && 'underline underline-offset-2'">
        {{ reservations.length > 0 ? reservations.length : 'no' }} active
        reservations.</span
      >
    </h2>
    <div class="mt-10 flex w-full flex-1 space-x-5">
      <div
        class="relative h-[35rem] w-1/3 rounded-2xl border border-gray-300 py-5 px-6"
      >
        <div class="flex items-center justify-between">
          <h3 class="font-medium capitalize">
            My {{ businessType[user.roles[0]] }}
          </h3>
          <Button
            class="cursor-pointer !rounded-md border border-gray-300 !p-0 hover:bg-neutral-50"
          >
            <RouterLink
              :to="`/business/${currentBusinessType}/create`"
              class="flex items-center justify-center space-x-1 py-2.5 pl-4 pr-3"
            >
              <p class="!font-medium">Add</p>
              <PlusIcon class="h-4 w-4 stroke-[3] text-emerald-600" />
            </RouterLink>
          </Button>
        </div>
        <div class="mt-5 h-[80%] space-y-5 overflow-y-auto">
          <div v-for="business in businesses" :key="business?.id">
            <RouterLink
              :to="`/${businessProfiles[currentBusinessType]}/${business.id}`"
              class="flex items-center space-x-3"
            >
              <div>
                <img
                  :src="business.images[0]"
                  alt="Business image"
                  class="h-14 w-[4.5rem] rounded-t-md object-cover"
                />
                <div
                  class="w-full rounded-b-md bg-emerald-600 px-3 py-0.5 text-center text-[13px] font-medium text-white"
                >
                  Booked
                </div>
              </div>
              <div>
                <span class="font-medium"> {{ business.name }} </span>
                <div class="text-sm leading-5 text-gray-600">
                  {{ business.address.city }}, {{ business.address.country }}
                </div>
                <div class="text-sm text-gray-600">
                  {{ business.address.street }},
                  {{ business.address.apartment }}
                </div>
              </div>
            </RouterLink>
          </div>
        </div>
        <a
          href=""
          class="absolute bottom-6 cursor-pointer font-medium underline"
        >
          View all
          <span class="capitalize">
            {{ businessType[user.roles[0]] }}
          </span>
        </a>
      </div>
      <div class="h-[35rem] w-1/3 rounded-2xl border border-gray-300 px-5 py-6">
        <div class="-mt-1.5 flex items-center justify-between">
          <h3 class="font-medium">Earnings Trend</h3>
          <Dropdown :slim="false" class="!px-2 !py-1" :values="incomeOptions" />
        </div>
        <div class="mt-2 text-3xl font-bold">$4,565</div>
        <div class="mt-5 flex items-end space-x-3">
          <TransitionGroup name="list">
            <div
              v-for="(point, index) in earnings.values"
              :key="point.year"
              :style="{ transitionDelay: 40 * (index + 1) + 'ms' }"
            >
              <div
                :style="{
                  height: (point.total / earnings.max) * 120 + 'px'
                }"
                class="w-4 rounded bg-gradient-to-b from-teal-200 via-indigo-400 to-purple-500"
              ></div>
            </div>
          </TransitionGroup>
        </div>
      </div>
      <div
        class="h-[35rem] w-1/3 space-y-4 rounded-2xl border border-gray-300 px-5 py-6"
      >
        <h3 class="font-medium">Upcoming reservations</h3>
        <div class="space-y-5 overflow-y-auto">
          <div
            class="relative"
            v-for="reservation in reservations"
            :key="reservation.id"
          >
            <div class="flex items-center space-x-3">
              <div>
                <img
                  :src="reservation.business.images[0]"
                  alt="Business image"
                  class="h-14 w-[4.5rem] rounded-t-md object-cover"
                />
                <div
                  class="w-full rounded-b bg-emerald-600 py-0.5 px-2 text-center text-[13px] font-medium text-white 2xl:right-0"
                >
                  {{ cost(reservation) }}
                </div>
              </div>
              <div>
                <RouterLink
                  :to="`/${businessProfiles[currentBusinessType]}/${reservation.business.id}`"
                  class="cursor-pointer font-medium hover:text-emerald-400"
                  >{{ reservation.business.name }}</RouterLink
                >
                <div class="text-sm leading-4 text-gray-600">
                  {{ reservation.business.address.city }},
                  {{ reservation.business.address.country }}
                </div>
                <div class="mt-1 text-sm font-medium">
                  {{
                    format(
                      parseJSON(reservation.start),
                      timeFormats[currentBusinessType]
                    )
                  }}
                </div>
                <div class="text-sm">
                  <span class="text-sm text-neutral-500">Client</span>
                  {{ reservation.user.firstName }}
                  {{ reservation.user.lastName }}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style>
.list-enter-active,
.list-leave-active {
  transition: all 0.4s cubic-bezier(1, 0.18, 0.38, 0.98);
  transform: scaleY(1);
  transform-origin: bottom;
}
.list-enter-from,
.list-leave-to {
  transform: scaleY(0);
  transform-origin: bottom;
}
</style>

<script setup>
import { ref } from 'vue'
import { RouterLink } from 'vue-router'
import api from '@/api/api.js'
import { PlusIcon } from 'vue-tabler-icons'
import Button from '../ui/Button.vue'
import Dropdown from '../ui/Dropdown.vue'
import { user } from '@/stores/userStore'
import { add, format, parseJSON, sub } from 'date-fns'

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

const timeFormats = {
  cabins: 'MMM d, yyyy',
  boats: 'MMM d, yyyy HH:mm',
  adventures: 'MMM d, yyyy HH:mm'
}

const businesses = ref([])
const businessType = {
  'Cabin Owner': 'cabins',
  'Boat Owner': 'boats',
  Fisher: 'adventures'
}
const reservations = ref([])
const currentBusinessType = ref(businessType[user.roles[0]])
const earnings = ref({
  max: 0,
  values: []
})

const businessProfiles = {
  adventures: 'adventure-profile',
  cabins: 'cabin-profile',
  boats: 'boat-profile'
}

const cost = reservation => {
  return (
    reservation.payment.price.symbol +
    (
      reservation.payment.price.amount *
      (1 - reservation.payment.taxPercentage / 100)
    ).toFixed(2)
  )
}

const fetchBusinesses = async () => {
  const [data] = await api.business.ownersBusinesses(
    businessType[user.roles[0]]
  )
  data && (businesses.value = data)
}

const fetchReservations = async () => {
  const [data, error] = await api.business.getReservations(
    'pending',
    businessType[user.roles[0]],
    3
  )
  if (!error) {
    reservations.value = data
  }
}

const fetchReport = async () => {
  const [data, error] = await api.finances.getReport(
    sub(new Date(), { months: 1 }),
    add(new Date(), { months: 3 })
  )
  if (!error) {
    earnings.value = {
      max: Math.max(...data.map(r => Number(r.total))),
      values: data
    }
  }
}

fetchReservations()
fetchBusinesses()
fetchReport()
</script>
