<template>
  <div class="mx-auto max-w-4.5xl pb-10">
    <h1 class="mt-8 text-2xl font-medium">Reservations</h1>
    <div class="mt-3 flex space-x-3">
      <Dropdown
        @change="e => (currentBusinessType = e.value)"
        label="Type"
        :values="businessTypes"
        class="w-fit"
      />
      <Dropdown
        @change="e => (currentReservationStatus = e.value)"
        label="Status"
        :values="reservationStatuses"
        class="mt-auto w-fit"
      />
    </div>
    <div v-if="reservations?.length != 0" class="mt-4 space-y-6">
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
            <div
              v-if="allowReview(reservation)"
              class="flex items-center space-x-2 text-sm"
            >
              <button
                @click="writeReview(reservation)"
                class="mt-2 flex items-center space-x-1 text-sm font-medium text-emerald-700 hover:underline"
              >
                <div>Write a review</div>
                <PencilIcon class="h-4 w-4 stroke-2 text-emerald-700" />
              </button>
              <span class="mt-1.5 text-gray-500">or</span>
              <button
                @click="writeComplaint(reservation)"
                class="mt-2 flex items-center space-x-1 text-sm font-medium hover:underline"
              >
                <div>Write a complaint</div>
                <MessageReportIcon class="h-4 w-4 stroke-2" />
              </button>
            </div>

            <div v-if="reservation.isCancellable">
              <p class="mt-2 text-sm text-gray-600">
                If you cancel your reservation, you must pay the cancellation
                fee of
                {{ reservation.business.cancellationFee }}%.
              </p>
              <button
                @click="cancelReservation(reservation)"
                class="mt-1 flex items-center space-x-1 text-sm font-medium hover:underline"
              >
                <div>Cancel reservation</div>
                <XIcon class="h-3 w-3 stroke-2" />
              </button>
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
              class="h-24 w-24 rounded-lg object-cover"
            />
            <div>
              <RouterLink
                :to="`/${businessProfiles[currentBusinessType]}/${reservation.business.id}`"
                class="font-medium"
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
            </div>
          </div>
          <div v-if="reservation.detailsVisible">
            <div class="flex justify-between space-x-24">
              <div>
                <div class="font-medium">Base</div>
                <div class="text-sm text-neutral-700">
                  {{ reservation.business.people }} People x
                  {{ reservation.units }}
                  {{ reservation.business.unitName }}
                </div>
                <div class="mt-2 font-medium">Services</div>
                <div
                  v-for="service in reservation.services"
                  :key="service.name"
                  class="text-sm text-neutral-700"
                >
                  {{ service.name }}
                </div>

                <div class="mt-2 font-medium">Subtotal</div>
                <div class="mt-1 text-sm font-medium">
                  Discount ({{ reservation.payment.discountPercentage }}%)
                </div>
                <div class="mt-1 text-sm">
                  Tax ({{ reservation.payment.taxPercentage }}%)
                </div>
                <div class="mt-4 font-semibold">Total</div>
              </div>
              <div class="pr-1 text-right">
                <div class="font-medium">{{ reservation.cost.base }}</div>
                <div class="text-sm text-neutral-700">
                  {{ reservation.cost.baseDetails }}
                </div>
                <div class="mt-2 font-medium">
                  {{ reservation.cost.servicesTotal }}
                </div>
                <div
                  v-for="service in reservation.cost.services"
                  :key="service.name"
                  class="text-sm text-neutral-700"
                >
                  {{ service.price }}
                </div>
                <div class="mt-2 font-medium">
                  {{ reservation.cost.subtotal }}
                </div>
                <div
                  class="-mr-1.5 -mt-0.5 ml-auto w-fit rounded-lg bg-red-600 px-2 py-1 text-sm text-white"
                >
                  -
                  {{ reservation.cost.discount }}
                </div>
                <div class="mt-0.5 text-sm text-neutral-700">
                  {{ reservation.cost.tax }}
                </div>
                <div class="mt-4 font-semibold">
                  {{ reservation.cost.total }}
                </div>
              </div>
            </div>
            <div
              @click="reservation.detailsVisible = false"
              class="mt-3 flex cursor-pointer justify-end space-x-1"
            >
              <span> Show Less </span>
              <ChevronUpIcon />
            </div>
          </div>
          <div class="flex flex-col justify-center" v-else>
            <div class="flex space-x-1.5">
              <div class="font-medium">Total:</div>
              <div class="font-medium">
                {{ reservation.cost.total }}
              </div>
            </div>
            <div
              @click="reservation.detailsVisible = true"
              class="flex cursor-pointer space-x-1 justify-self-end"
            >
              <span> Show Details </span>
              <ChevronDownIcon />
            </div>
          </div>
        </div>
      </div>
    </div>
    <div v-else class="mt-3">
      There are currently no reservations for defined parameters.
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
  <CancelReservation
    @modalClosed="isCancelModalOpen = false"
    :isOpen="isCancelModalOpen"
    :reservationId="reservationId"
    :businessType="currentBusinessType"
  />
  <CreateComplaint
    @modalClosed="isComplaintModalOpen = false"
    :isOpen="isComplaintModalOpen"
    :name="name"
    :start="start"
    :end="end"
    :address="address"
    :duration="duration"
    :unit="unit"
    :businessType="currentBusinessType"
    :reservationId="reservationId"
  />
</template>
<script setup>
import { ref, watchEffect } from 'vue'
import { RouterLink } from 'vue-router'
import {
  ChevronDownIcon,
  ChevronUpIcon,
  PencilIcon,
  XIcon,
  MessageReportIcon
} from 'vue-tabler-icons'
import {
  format,
  parseJSON,
  isPast,
  differenceInHours,
  parseISO
} from 'date-fns'
import api from '@/api/api'
import Dropdown from '@/components/ui/Dropdown.vue'
import CreateReview from '@/components/reservations/CreateReview.vue'
import CancelReservation from '@/components/reservations/CancelReservation.vue'
import CreateComplaint from '@/components/reservations/CreateComplaint.vue'
import { formatAddress } from '@/components/utility/address'

const businessTypes = [
  {
    name: 'Adventures',
    value: 'adventures'
  },
  {
    name: 'Boats',
    value: 'boats'
  },
  {
    name: 'Cabins',
    value: 'cabins'
  }
]

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

const reservationStatuses = [
  {
    name: 'Pending',
    value: 'pending'
  },
  {
    name: 'Ongoing',
    value: 'ongoing'
  },
  {
    name: 'Completed',
    value: 'completed'
  },
  {
    name: 'All',
    value: 'all'
  }
]

const reservations = ref()
const isReviewModalOpen = ref(false)
const isCancelModalOpen = ref(false)
const isComplaintModalOpen = ref(false)
const id = ref()
const name = ref()
const address = ref()
const start = ref()
const end = ref()
const duration = ref()
const unit = ref()
const reservationId = ref()
const currentBusinessType = ref(businessTypes[0].value)
const currentReservationStatus = ref(reservationStatuses[0].value)

const reservationStatus = reservation => {
  if (isPast(parseJSON(reservation.end))) return 'Completed'
  if (isPast(parseJSON(reservation.start))) return 'Ongoing'
  return 'Pending'
}

const allowReview = reservation => {
  return (
    reservationStatus(reservation) == 'Completed' &&
    reservation.status != 'Cancelled'
  )
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

watchEffect(async () => {
  const [reservationsData, reservationsError] =
    await api.business.getReservations(
      currentReservationStatus.value,
      currentBusinessType.value
    )

  if (!reservationsError) {
    reservations.value = reservationsData
    reservations.value.forEach(r => {
      r.cost = costs(r)
      r.detailsVisible = false
    })
  }
})

const saveData = reservation => {
  name.value = reservation.business.name
  start.value = format(parseISO(reservation.start), 'MMM d, yyyy')
  end.value = format(parseISO(reservation.end), 'MMM d, yyyy')
  address.value = formatAddress(reservation.business.address)
  id.value = reservation.business.id
  reservationId.value = reservation.id
  duration.value = differenceInHours(
    parseISO(reservation.end),
    parseISO(reservation.start)
  )
  unit.value =
    currentBusinessType.value == 'Cabins'
      ? duration.value == 1
        ? 'day'
        : 'days'
      : duration.value == 1
      ? 'hour'
      : 'hours'
}

const writeReview = reservation => {
  saveData(reservation)
  isReviewModalOpen.value = true
}

const writeComplaint = reservation => {
  saveData(reservation)
  isComplaintModalOpen.value = true
}

const cancelReservation = reservation => {
  console.log(reservation.id)
  reservationId.value = reservation.id
  isCancelModalOpen.value = true
}
</script>
