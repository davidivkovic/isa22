<template>
  <div class="mx-auto flex h-full w-3/4 flex-col py-8">
    <h1 class="text-4xl font-bold">👋 Welcome Miladin</h1>
    <h2 class="mt-2 text-lg text-gray-800">
      You currently haves
      <span class="underline underline-offset-2"> {{reservations.length}} active reservations.</span>
    </h2>
    <div class="mt-10 flex w-full flex-1 space-x-5">
      <div
        class="relative h-[35rem] w-1/3 rounded-3xl border border-gray-300 py-5 px-6 " 
      >
        <div class="flex items-center justify-between">
          <h3 class="font-medium">My {{ businessType[user.roles[0]][0] }}</h3>
          <Button
            class="flex items-center space-x-1 !rounded-md border border-gray-300 !py-2.5 cursor-pointer"
          >
              <RouterLink :to="`/business/${currentBusinessType}/create`">
                <p class="!font-medium">Add</p>
              </RouterLink>
              <PlusIcon class="h-4 w-4 stroke-[3] text-emerald-600" />
          </Button>
        </div>
        <div class="mt-5 h-[80%] space-y-4 overflow-y-scroll">
          <div
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
                <div class="font-medium">              
                  <RouterLink
                :to="`/${businessProfiles[currentBusinessType]}/${business.id}`"
                class="font-medium hover:text-emerald-400 cursor-pointer"
                >{{ business.name }}</RouterLink
              ></div>
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
          </div>
        </div>
        <a href="" class="absolute bottom-6 font-medium underline cursor-pointer">
          View all {{businessType[user.roles[0]][0]}}
        </a>
      </div>
      <div class="h-[35rem] w-1/3 rounded-3xl border border-gray-300 px-5 py-6">
        <div class="-mt-1.5 flex items-center justify-between">
          <h3 class="font-medium">Earnings Trend</h3>
          <Dropdown :slim="false" class="!px-2 !py-1" :values="incomeOptions" />
        </div>
        <div class="mt-4 text-3xl font-bold">$4,565</div>
      </div>
      <div class="h-[35rem] w-1/3 rounded-3xl border border-gray-300 px-5 py-6 space-y-4">
        <h3 class="font-medium">Upcoming reservations</h3>
        <div class="overflow-y-scroll h-[90%] space-y-3">
          <div
          v-for="reservation in reservations"
          :key="reservation.id"
          class="px-4 py-3.5"
          >
         <div class="flex justify-between pt-0">
          <div class="flex space-x-4">
            <div class="relative">            
              <div class="text-sm rounded-xl absolute bottom-2 right-1 bg-emerald-50 px-2">
                  {{ reservation.cost.subtotal }}
              </div>
              <img
              :src="reservation.business.images[0]"
              alt=""
              class="h-24 w-24 rounded-lg object-cover"
            /></div>
            <div class="space-y-1">
              <RouterLink
                :to="`/${businessProfiles[currentBusinessType]}/${reservation.business.id}`"
                class="font-medium hover:text-emerald-400 cursor-pointer"
                >{{ reservation.business.name }}</RouterLink
              >
              <h3 class="mt-1 text-sm text-neutral-500">
                {{ reservation.business.address.street }}
                {{ reservation.business.address.apartment }}
              </h3>
              <h3 class="text-sm text-neutral-500">
                {{ reservation.business.address.city }},
                {{ reservation.business.address.country }}
              </h3>
              <div class="space-x-1 text-sm">
                <span class="text-neutral-500">from</span>
                <span class="font-medium">
                  {{
                    format(
                      parseJSON(reservation.start),
                      timeFormats[currentBusinessType]
                    )
                  }}
                </span>
                <span class="text-neutral-500">to</span>
                <span class="font-medium">
                  {{
                    format(
                      parseJSON(reservation.end),
                      timeFormats[currentBusinessType]
                    )
                  }}
                </span>
              </div>
            </div>
          </div>
      </div>
      </div>
    </div>
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
import { user } from '@/stores/userStore'
import {
  format,
  parseJSON,
  isPast,
  differenceInHours,
  parseISO
} from 'date-fns'

const businessTypes = [
  {
    name: 'Cabins',
    value: 'cabins'
  },
  {
    name: 'Boats',
    value: 'boats'
  },
  {
    name: 'Adventures',
    value: 'adventures'
  },
]

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
  'Cabin Owner': ['Cabins',0],
  'Boat Owner': ['Boats',1],
  'Fishing Instructor': ['Adventures',2]
}
const reservations = ref([])

const currentBusinessType = ref(businessTypes[businessType[user.roles[0]][1]].value)

const businessProfiles = {
  adventures: 'adventure-profile',
  cabins: 'cabin-profile',
  boats: 'boat-profile'
}

const costs = reservation => {
  const symbol = reservation.payment.price.symbol
  const total = reservation.payment.price.amount
  const tax = 1 + reservation.payment.taxPercentage / 100

  return {
    base: symbol + calculateBase(reservation).toFixed(2),
    baseDetails: `
      ${reservation.business.people}
       x
      ${reservation.units}
       x
      ${symbol}${reservation.business.pricePerUnit.amount.toFixed(2)}
    `,
    servicesTotal: symbol + calculateServices(reservation).toFixed(2),
    services: reservation.services.map(s => ({
      name: s.name,
      price: (s.price.amount / tax).toFixed(2)
    })),
    subtotal: symbol + calculateSubtotal(reservation).toFixed(2),
    discount: (
      calculateSubtotal(reservation) *
      (reservation.payment.discountPercentage / 100)
    ).toFixed(2),
    tax: (total - total / tax).toFixed(2),
    total: symbol + total.toFixed(2)
  }
}

const calculateBase = reservation => {
  return (
    (reservation.business.pricePerUnit.amount /
      (1 + reservation.payment.taxPercentage / 100)) *
    reservation.business.people *
    reservation.units
  )
}

const calculateServices = reservation => {
  return (
    reservation.services.reduce(
      (sum, service) => sum + service.price.amount,
      0
    ) /
    (1 + reservation.payment.taxPercentage / 100)
  )
}

const calculateSubtotal = reservation => {
  return calculateBase(reservation) + calculateServices(reservation)
}

const fetchBusinesses = async () => {
  const [data] = await api.business.ownersBusinesses(
    businessType[user.roles[0]][0].toLowerCase()
  )
  data && (businesses.value = data)
}

const fetchReservations = async () => {
  const [data, error] = await api.business.upcomingReservations(
    businessType[user.roles[0]][0].toLowerCase()
  )
  if (!error) {
    data.forEach(r => {
      r.cost = costs(r)
    })
  }
  data && (reservations.value = data)
}

fetchReservations()
fetchBusinesses()
</script>
