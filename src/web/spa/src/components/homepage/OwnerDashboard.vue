<template>
  <div class="mx-auto mt-3 mb-8 flex h-full w-4/5 max-w-7xl flex-col">
    <h1 class="text-4xl font-bold">👋 Welcome {{ user.firstName }}</h1>
    <h2 class="mt-2 text-lg text-gray-800">
      You currently have
      <span :class="reservationsCount > 0 && 'underline underline-offset-2'">
        {{ reservationsCount > 0 ? reservationsCount : 'no' }} active
        reservations.</span
      >
    </h2>
    <div class="mt-10 flex w-full flex-1 space-x-5">
      <div
        class="relative h-[35rem] w-1/3 rounded-2xl border border-gray-300 py-5 px-6"
      >
        <div class="flex items-center justify-between">
          <h3 class="text-lg font-medium capitalize">
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
          <TransitionGroup
            enter-active-class="duration-500 ease-in-out"
            enter-from-class="opacity-0"
            enter-to-class="opacity-100"
          >
            <div
              v-for="(business, index) in businesses.slice(0, 4)"
              :key="business?.id"
              :style="{ transitionDelay: 120 * index + 'ms' }"
            >
              <RouterLink
                :to="`/${businessProfiles[currentBusinessType]}/${business.id}`"
                class="flex items-center space-x-3"
              >
                <div>
                  <img
                    :src="business.image"
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
          </TransitionGroup>
        </div>
        <a
          href=""
          class="absolute bottom-6 cursor-pointer font-medium underline"
        >
          View all
          <RouterLink :to="`my-${businessType[user.roles[0]]}`">
            <span class="capitalize">
              {{ businessType[user.roles[0]] }}
            </span>
          </RouterLink>
        </a>
      </div>
      <div class="h-[35rem] w-1/3 rounded-2xl border border-gray-300 px-5 py-6">
        <div class="-mt-1.5 flex items-center justify-between">
          <h3 class="text-lg font-medium">Earnings Trend</h3>
          <Dropdown
            @change="selectIncomeOption"
            :slim="false"
            class="!px-2 !py-1"
            :values="incomeOptions"
          />
        </div>
        <Transition
          enter-active-class="duration-500 ease-in-out"
          enter-from-class="opacity-0"
          enter-to-class="opacity-100"
        >
          <div v-if="earnings.total !== 0">
            <div class="text-3xl font-bold">
              {{ earnings.total }}
            </div>
            <div class="text-sm text-neutral-500">
              In the past {{ periodName }}
            </div>
          </div>
        </Transition>
        <BarChart :points="earnings.values" :is-money="true" class="mt-5" />
        <div class="mt-2">
          <Transition
            enter-active-class="duration-500 ease-in-out"
            enter-from-class="opacity-0"
            enter-to-class="opacity-100"
          >
            <div v-if="attendance.total !== 0">
              <div>
                <span class="text-3xl font-bold">
                  {{ attendance.total }}
                </span>
                <span class="ml-2 text-lg">Reservations</span>
              </div>
              <div class="text-sm leading-3 text-neutral-500">
                In the past {{ periodName }}
              </div>
            </div>
          </Transition>
        </div>
        <BarChart :points="attendance.values" :is-money="false" class="mt-5" />
      </div>
      <div
        class="h-[35rem] w-1/3 space-y-4 rounded-2xl border border-gray-300 px-5 py-6"
      >
        <h3 class="text-lg font-medium">Upcoming reservations</h3>
        <div class="space-y-5 overflow-y-auto">
          <TransitionGroup
            enter-active-class="duration-500 ease-in-out"
            enter-from-class="opacity-0"
            enter-to-class="opacity-100"
          >
            <div
              v-for="(reservation, index) in reservations.slice(0, 4)"
              :style="{ transitionDelay: 120 * index + 'ms' }"
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
          </TransitionGroup>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { RouterLink } from 'vue-router'
import api from '@/api/api.js'
import { PlusIcon } from 'vue-tabler-icons'
import Button from '../ui/Button.vue'
import Dropdown from '../ui/Dropdown.vue'
import { user } from '@/stores/userStore'
import BarChart from '@/components/ui/charts/BarChart.vue'
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
const currentIncomeOption = ref('monthly')

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
const reservationsCount = ref(0)
const currentBusinessType = ref(businessType[user.roles[0]])
const earnings = ref({
  total: 0,
  values: []
})

const attendance = ref({
  total: 0,
  values: []
})

const businessProfiles = {
  adventures: 'adventure-profile',
  cabins: 'cabin-profile',
  boats: 'boat-profile'
}

const selectIncomeOption = e => {
  currentIncomeOption.value = e.value
  fetchEarnings()
  fetchAttendance()
}

const monthsFromIncome = computed(() => {
  if (currentIncomeOption.value == 'yearly') return 24
  else if (currentIncomeOption.value == 'monthly') return 6
  else return 1
})

const periodName = computed(() => {
  if (currentIncomeOption.value == 'yearly') return '2 years'
  else if (currentIncomeOption.value == 'monthly') return '6 months'
  else return '4 weeks'
})

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
  const [data] = await api.business.getBusinesses(businessType[user.roles[0]])
  data && (businesses.value = data.results)
}

const fetchReservations = async () => {
  const [data, error] = await api.business.getReservations(
    businessType[user.roles[0]],
    'pending',
    0,
    3,
    true
  )
  if (!error) {
    reservations.value = data.results
    reservationsCount.value = data.totalResults
  }
}

const fetchEarnings = async () => {
  const [data, error] = await api.finances.getReport(
    sub(new Date(), { months: monthsFromIncome.value }),
    add(new Date(), { months: 0 })
  )
  if (!error) {
    data.forEach(p => (p.total = Number(p.total)))
    earnings.value = {
      values: data,
      total: data
        .reduce((sum, point) => sum + point.total, 0)
        .toLocaleString('en-US', {
          style: 'currency',
          currency: 'USD',
          maximumFractionDigits: 0
        })
    }
  }
}

const fetchAttendance = async () => {
  const [data, error] = await api.finances.getReport(
    sub(new Date(), { months: monthsFromIncome.value }),
    add(new Date(), { months: 0 }),
    'attendance'
  )
  if (!error) {
    data.forEach(p => (p.total = Number(p.total)))
    attendance.value = {
      values: data,
      total: data.reduce((sum, point) => sum + point.total, 0)
    }
  }
}

onMounted(async () => {
  fetchReservations()
  await fetchBusinesses()
  await fetchEarnings()
  await fetchAttendance()
})
</script>
