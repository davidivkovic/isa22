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
                :to="`/profile`"
                class="flex hover:text-emerald-700">
                Miladin Momcilovic
                </RouterLink>
              </div>
            </div>
          </div>
          <div class="flex flex-col relative w-[10.5rem]">
            <div class="absolute bottom-0 left-0">
              <div class="font-medium">Total Income: {{ reservation.cost.subtotal }}</div>
            </div>
          </div>
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
  'Cabin Owner': ['Cabins',0],
  'Boat Owner': ['Boats',1],
  'Fishing Instructor': ['Adventures',2]
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
const currentBusinessType = ref(businessTypes[businessType[user.roles[0]][1]].value)

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

watchEffect(async () => {
  const [reservationsData, reservationsError] =
    await api.business.allReservations(    businessType[user.roles[0]][0].toLowerCase())
  if (!reservationsError) {
    reservations.value = reservationsData
    reservations.value.forEach(r => {
      r.cost = costs(r)
      r.detailsVisible = false
    })
  }
})

</script>
