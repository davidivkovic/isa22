<template>
  <div class="mx-auto mb-10 max-w-4.5xl">
    <h1 class="text-2xl font-medium">Profile</h1>

    <div v-if="!editMode" class="mt-2 flex">
      <div class="mt-5 mr-7 flex space-x-4">
        <div
          class="flex h-12 w-12 items-center justify-center space-x-px rounded-full bg-neutral-100 text-xl font-semibold"
        >
          <p>
            {{ user.firstName[0].toUpperCase() }}
          </p>
          <p>
            {{ user.lastName[0].toUpperCase() }}
          </p>
        </div>
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
          <div class="mt-3 flex space-x-2.5">
            <Button
              @click="editMode = true"
              class="flex space-x-1.5 border border-neutral-300 !px-5 !py-1.5 transition hover:bg-neutral-50"
            >
              <span> Edit </span>
              <EditIcon class="-mt-px h-[18px] w-[18px]" />
            </Button>
            <Button
              v-if="!deletionRequest || deletionRequest.rejected"
              @click="deletionModalOpen = true"
              class="space-x-1.5 border border-red-800 border-opacity-10 bg-red-600 !px-5 !py-1.5 text-white transition hover:bg-red-700"
            >
              Delete
            </Button>
          </div>
        </div>
      </div>

      <div
        v-if="deletionRequest && !deletionRequest.rejected"
        class="mr-5 flex w-[300px] flex-col justify-between rounded-lg bg-amber-100 py-4 px-5 text-amber-800"
      >
        <div>
          <h2 class="text-lg font-semibold">Account deletion requested</h2>
          <p class="text-sm font-medium">
            {{ format(parseJSON(deletionRequest.timestamp), 'do MMM yyyy') }}
          </p>
        </div>

        <p class="text-sm font-medium">
          We will notify you via email whether or not your request gets accepted
          by our adminstartors.
        </p>
      </div>

      <div
        :class="[
          {
            'text-white': loyaltyLevel.current?.color == '#000000',
            border: loyaltyLevel.current == null
          },
          `relative w-[300px] overflow-clip rounded-lg  border-neutral-300 py-4 px-5 bg-[${
            loyaltyLevel.current?.color ?? '#ebebeb'
          }]`
        ]"
      >
        <div class="flex items-center space-x-1">
          <h2 class="text-lg font-semibold">
            {{ loyaltyLevel.current?.name ?? 'Regular' }} Member
          </h2>
          <DiamondIcon class="h-5 w-5 text-black" />
        </div>
        <p
          v-if="loyaltyLevel.current"
          class="whitespace-nowrap text-sm font-medium leading-5"
        >
          Enjoy your {{ loyaltyLevel.current.discountPercentage }}% reservation
          discount
          <!-- or tax relief -->
        </p>
        <h2 class="mt-3 font-semibold">{{ loyaltyLevel.points }} Points</h2>
        <div class="relative mt-2">
          <div
            class="absolute h-2 w-full rounded-lg bg-black opacity-[15%]"
          ></div>
          <div
            :style="{
              width:
                (loyaltyLevel.points / loyaltyLevel.next.threshold) * 100 + '%'
            }"
            class="absolute h-2 rounded-lg bg-black"
          ></div>
        </div>
        <p class="mt-6 text-[13px] font-medium text-neutral-600">
          Earn {{ loyaltyLevel.next.threshold - loyaltyLevel.points }} more
          points to reach {{ loyaltyLevel.next.name }} level and enjoy a
          {{ loyaltyLevel.next.discountPercentage }}% discount
        </p>
      </div>
    </div>

    <div v-else>
      <button @click="editMode = false" class="group my-4 flex space-x-2">
        <ArrowNarrowLeftIcon stroke-width="1.25" class="group-hover:stroke-2" />
        <div class="font-medium group-hover:font-semibold">Back</div>
      </button>

      <form @submit.prevent="updateUser()" class="ml-8 flex space-x-10">
        <div class="space-y-3">
          <div class="flex space-x-2">
            <Input
              required
              v-model="user.firstName"
              placeholder="First Name"
              class="h-12 w-full"
              name="firstName"
              label="First Name"
            />
            <Input
              required
              v-model="user.lastName"
              placeholder="Last Name"
              class="h-12 w-full"
              name="lastName"
              label="Last Name"
            />
          </div>
          <Input
            disabled
            required
            v-model="user.email"
            placeholder="Email address"
            class="h-12 w-full"
            name="emailAddress"
            type="tel"
            label="Email address"
          />
          <Input
            required
            v-model="user.phoneNumber"
            placeholder="Phone number"
            class="h-12 w-full"
            name="phoneNumber"
            type="tel"
            label="Phone number"
          />
        </div>

        <div class="space-y-3">
          <div class="flex space-x-2">
            <div class="w-full">
              <Input
                required
                v-model="user.address.street"
                class="h-12 w-full"
                name="street"
                placeholder="Street"
                label="Street"
              />
            </div>
            <Input
              required
              v-model="user.address.apartment"
              class="h-12 w-full"
              name="apartment"
              placeholder="Apartment"
              label="Apartment"
            />
          </div>
          <div class="flex space-x-2">
            <Input
              required
              v-model="user.address.postalCode"
              class="h-12 w-full"
              name="postalCode"
              placeholder="Postal Code"
              label="Postal Code"
              type="number"
              min="0"
            />
            <Input
              required
              v-model="user.address.city"
              class="h-12 w-full"
              name="city"
              placeholder="City"
              label="City"
            />
          </div>
          <Input
            required
            v-model="user.address.country"
            class="h-12 w-full"
            name="country"
            placeholder="Country"
            label="Country"
          />
          <div class="flex justify-end">
            <Button
              type="submit"
              class="group mt-4 flex items-center space-x-2 !rounded-full bg-emerald-600 !px-6 !py-2.5 text-white"
            >
              <div v-if="loading" class="flex items-center">
                <Loader class="mr-3" />
                Processing...
              </div>
              <div v-else>Save Changes</div>
            </Button>
          </div>
        </div>
      </form>
    </div>

    <Modal
      light
      :isOpen="deletionModalOpen"
      @modalClosed="deletionModalOpen = false"
      class="-mt-10 max-w-sm !rounded-xl bg-white text-sm"
    >
      <form
        @submit.prevent="confirmDeleteAccount()"
        class="divide-y divide-gray-300"
      >
        <div class="w-full space-y-5 px-6 pt-8 pb-5">
          <div class="text-base">
            Please tell us why you want to delete your account. <br />
            <span class="text-sm text-gray-400"
              >(E.g. "I dont't find adventure.com profitable enough.")
            </span>
          </div>
          <textarea
            required
            rows="5"
            placeholder="Enter a message here.."
            v-model="deletionReason"
            class="w-full resize-none rounded-lg border border-gray-300 text-sm focus:border-gray-300 focus:ring-0"
          >
          </textarea>
        </div>
        <button
          type="submit"
          class="w-full py-4 px-20 font-medium text-red-500"
        >
          Delete my account
        </button>
        <button @click="deletionModalOpen = false" class="w-full p-4">
          Cancel
        </button>
      </form>
    </Modal>

    <div>
      <h1 class="mt-8 text-2xl font-medium">Reservations</h1>
      <div class="mt-3 flex space-x-3">
        <Dropdown
          @change="changeBusinessType"
          label="Type"
          :values="businessTypes"
          class="w-fit"
        />
        <Dropdown
          @change="change"
          label="Status"
          :values="reservationStatuses"
          class="mt-auto w-fit"
        />
      </div>

      <!-- <div class="mx-auto flex w-fit space-x-1 rounded-lg bg-neutral-100 p-1.5">
        <div
          v-for="businessType in businessTypes"
          :key="businessType"
          class="cursor-pointer rounded-lg px-3 py-2 font-medium transition hover:bg-white"
          :class="{ 'bg-white': currentBusinessType == businessType }"
          @click="currentBusinessType = businessType"
        >
          {{ businessType }}
        </div>
      </div> -->
      <div class="mt-4">
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
                  {{ format(parseJSON(reservation.start), 'MMM d, yyyy') }}
                </span>
                <span class="text-neutral-500">to</span>
                <span class="font-medium">
                  {{ format(parseJSON(reservation.end), 'MMM d, yyyy') }}
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
                <!-- We straight up lying at this point -->
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
                  :to="`/adventure-profile/${reservation.business.id}`"
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
                  <div class="text-sm">
                    {{ reservation.business.people }} People x
                    {{ reservation.units }} {{ reservation.business.unitName }}
                  </div>
                  <div class="mt-2 font-medium">Services</div>
                  <div
                    v-for="service in reservation.services"
                    :key="service.name"
                    class="text-sm"
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
                  <div class="text-sm">
                    {{ reservation.cost.baseDetails }}
                  </div>
                  <div class="mt-2 font-medium">
                    {{ reservation.cost.servicesTotal }}
                  </div>
                  <div
                    v-for="service in reservation.cost.services"
                    :key="service.name"
                    class="text-sm"
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
                  <div class="mt-0.5 text-sm">
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
          <!-- {{ JSON.stringify(reservation) }} -->
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { RouterLink } from 'vue-router'
import { format, parseJSON, isPast } from 'date-fns'
import {
  DiamondIcon,
  EditIcon,
  ArrowNarrowLeftIcon,
  ChevronDownIcon,
  ChevronUpIcon
} from 'vue-tabler-icons'
import Button from '@/components/ui/Button.vue'
import Input from '@/components/ui/Input.vue'
import Loader from '@/components/ui/Loader.vue'
import Modal from '@/components/ui/Modal.vue'
import Dropdown from '@/components/ui/Dropdown.vue'
import api from '@/api/api'

const editMode = ref(false)
const loading = ref(false)
const deletionModalOpen = ref(false)

const user = ref()
const loyaltyLevel = ref()
const reservations = ref()
const deletionRequest = ref()
const deletionReason = ref('')

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
  }
]

const currentBusinessType = ref(businessTypes[0])
const currentReservationStatus = ref(reservationStatuses[0])

const reservationStatus = reservation => {
  if (isPast(parseJSON(reservation.start))) {
    if (isPast(parseJSON(reservation.end))) {
      return 'Completed'
    }
    return 'Ongoing'
  }
  return 'Pending'
}

const costs = reservation => {
  const symbol = reservation.payment.price.symbol
  const total = reservation.payment.price.amount
  const tax = 1 + reservation.payment.taxPercentage / 100

  return {
    base: symbol + calculateBase(reservation),
    baseDetails: `
      ${reservation.business.people}
       x 
      ${reservation.units}
       x 
      ${symbol}${reservation.business.pricePerUnit.amount.toFixed(2)}
    `,
    servicesTotal: symbol + calculateServices(reservation),
    services: reservation.services.map(s => ({
      name: s.name,
      price: s.price.symbol + (s.price.amount / tax).toFixed(2)
    })),
    subtotal: symbol + calculateSubtotal(reservation).toFixed(2),
    discount:
      symbol +
      (
        calculateSubtotal(reservation) *
        (reservation.payment.discountPercentage / 100)
      ).toFixed(2),
    tax: symbol + (total - total / tax).toFixed(2),
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

const updateUser = async () => {
  loading.value = true
  const [, error] = await api.users.update(user.value)
  if (!error) loading.value = editMode.value = false
}

const confirmDeleteAccount = async () => {
  const [, error] = await api.users.requestDeletion(deletionReason.value)
  if (!error) {
    deletionRequest.value = {
      accepted: false,
      rejected: false,
      timestamp: new Date().toJSON()
    }
  }
  deletionModalOpen.value = false
}

const [profileData, profileError] = await api.users.getProfile()
if (!profileError) {
  user.value = profileData.user
  loyaltyLevel.value = profileData.loyaltyLevel
  reservations.value = profileData.reservations
  deletionRequest.value = profileData.deletionRequest
}

const [reservationsData, reservationsError] =
  await api.business.getReservations('all', 'adventure')

if (!reservationsError) {
  reservations.value = reservationsData
  reservations.value.forEach(r => {
    r.cost = costs(r)
    r.detailsVisible = false
  })
}
</script>
