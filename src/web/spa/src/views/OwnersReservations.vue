<template>
  <div class="mx-auto max-w-4.5xl pb-10">
    <h1 class="mt-8 text-2xl font-medium">Reservations</h1>
    <div class="mt-4 space-y-6">
      <div
        v-for="reservation in reservations"
        :key="reservation.id"
        class="divide-y rounded-xl border border-neutral-300 px-4 py-3.5"
      >
        <div class="flex items-center pb-3">
          <div>
            <span class="text-lg font-medium">{{
              reservationStatus(reservation)
            }}</span>
            <div class="flex items-center space-x-1.5 text-sm">
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
          <div class="ml-auto text-right text-sm">
            <p>
              <span class="text-neutral-500"> Reservation date: </span>
              {{ format(parseJSON(reservation.timestamp), 'MMM d, yyyy') }}
            </p>
            <p>
              <span class="text-neutral-500">Reservation ID:</span>
              {{
                reservation.id
                  .replaceAll('-', '')
                  .toUpperCase()
                  .substring(0, 16)
              }}
            </p>
          </div>
        </div>
        <div class="flex justify-between pt-4">
          <div class="flex space-x-4">
            <img
              :src="reservation.business.images[0]"
              alt=""
              class="h-[7rem] w-[7rem] rounded-lg object-cover"
            />
            <div>
              <RouterLink
                :to="`/${businessProfiles[currentBusinessType]}/${reservation.business.id}`"
                class="font-medium hover:text-emerald-400"
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

              <h3 class="mt-2 text-sm">
                Reservation for {{ reservation.business.people }} People
              </h3>
              <div class="mx-auto flex">
                <RouterLink
                  :to="`/get-profile/${reservation.user.id}`"
                  class="flex hover:text-emerald-700"
                >
                  {{
                    reservation.user.firstName + ' ' + reservation.user.lastName
                  }}
                </RouterLink>
              </div>
            </div>
          </div>
          <div class="relative flex w-[10.5rem] flex-col">
            <div class="absolute bottom-0 left-0">
              <div class="font-medium">
                Total Income: {{ reservation.cost.subtotal }}
              </div>
            </div>
          </div>
        </div>
      </div>
      <div
        v-if="reservations?.length"
        class="mt-10 flex justify-between space-x-10 text-sm"
      >
        <p>
          Showing <span class="font-medium"> 1</span> to
          <span class="font-medium"> {{ reservations?.length }} </span> of
          <span class="font-medium"> {{ totalResults }} </span> results
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
    </div>
  </div>
  <CreateReview
    @modalClosed="isReviewModalOpen = false"
    :isOpen="isReviewModalOpen"
    :name="name"
    :start="start"
    :end="end"
    :address="address"
    :duration="duration"
    :unit="unit"
    :businessType="currentBusinessType"
    :id="id"
  />
</template>
<script setup>
import { ref, watchEffect, computed } from 'vue'
import { RouterLink } from 'vue-router'
import Dropdown from '@/components/ui/Dropdown.vue'
import { user } from '@/stores/userStore'
import { ArrowLeftIcon, ArrowRightIcon } from 'vue-tabler-icons'
import {
  format,
  parseJSON,
  isPast,
  differenceInHours,
  parseISO
} from 'date-fns'
import api from '@/api/api'
import CreateReview from '@/components/reservations/CreateReview.vue'
import CancelReservation from '@/components/reservations/CancelReservation.vue'
import { formatAddress } from '@/components/utility/address'

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
  }
]
const businessType = {
  'Cabin Owner': 'cabins',
  'Boat Owner': 'boats',
  Fisher: 'adventures'
}

const businessProfiles = {
  adventures: 'adventure-profile',
  cabins: 'cabin-profile',
  boats: 'boat-profile'
}

const timeFormats = {
  cabins: 'MMM d, yyyy',
  boats: 'MMM d, yyyy HH:mm',
  adventures: 'MMM d, yyyy HH:mm'
}

const reservations = ref()
const isReviewModalOpen = ref(false)
const isCancelModalOpen = ref(false)
const id = ref()
const name = ref()
const address = ref()
const start = ref()
const end = ref()
const duration = ref()
const unit = ref()
const reservationId = ref()
const currentBusinessType = ref(businessType[user.roles[0]])

const currentPage = ref(1)
const totalPages = ref(1)
const totalResults = ref(0)
const hasNext = computed(() => currentPage.value < totalPages.value)
const hasPrevious = computed(
  () => currentPage.value >= totalPages.value && totalPages.value > 1
)

const reservationStatus = reservation => {
  if (isPast(parseJSON(reservation.end))) return 'Completed'
  if (isPast(parseJSON(reservation.start))) return 'Ongoing'
  return 'Pending'
}

const costs = reservation => {
  const symbol = reservation.payment.price.symbol
  const total = reservation.payment.price.amount
  const tax = 1 + reservation.payment.taxPercentage / 100

  return {
    subtotal: symbol + calculateSubtotal(reservation).toFixed(2)
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

const search = async () => {
  const [reservationsData, reservationsError] =
    await api.business.getReservations(
      businessType[user.roles[0]],
      'all',
      currentPage.value - 1
    )
  if (!reservationsError) {
    reservationsData.results.forEach(r => {
      r.cost = costs(r)
      r.detailsVisible = false
    })
    totalResults.value = reservationsData.totalResults
    reservations.value = reservationsData.results
  }
}

watchEffect(() => search())

const nextPage = () => {
  currentPage.value++
  search()
}

const previousPage = () => {
  currentPage.value--
  search()
}
</script>
