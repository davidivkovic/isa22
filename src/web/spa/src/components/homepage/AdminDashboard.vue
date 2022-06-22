<template>
  <div class="mx-auto mt-3 mb-8 flex h-full w-4/5 max-w-7xl flex-col">
    <h1 class="text-4xl font-bold">👋 Welcome {{ user.firstName }}</h1>

    <div class="mt-10 flex w-full flex-1 space-x-5">
      <div
        class="relative h-[35rem] w-1/3 rounded-2xl border border-gray-300 py-5 px-6"
      >
        <div class="items-center justify-between">
          <h3 class="text-lg font-medium">Pending Registration Requests</h3>
          <div
            v-for="request in registrationRequests?.slice(0, 4)"
            :key="request.id"
            class="my-7"
          >
            <div class="my-6 flex items-start space-x-3">
              <div
                class="mb-1 flex h-12 w-12 items-center justify-center space-x-px rounded-full bg-emerald-50 text-lg font-semibold text-emerald-700"
              >
                <p>
                  {{ request.user.firstName[0].toUpperCase() }}
                </p>
                <p>
                  {{ request.user.lastName[0].toUpperCase() }}
                </p>
              </div>
              <div class="w-64">
                <h2 class="leading-5">
                  <span class="font-medium"
                    >{{ request.user.firstName }}
                    {{ request.user.lastName }}</span
                  >
                  wants to register as a
                  <span class="font-medium">{{ request.user.roles[0] }}</span>
                </h2>
                <h3 class="mt-2">
                  <p class="text-gray-700">{{ request.request.reason }}</p>
                </h3>
              </div>
            </div>
            <RouterLink
              to="/requests"
              class="absolute bottom-6 cursor-pointer font-medium underline"
            >
              View all pending registration requests
            </RouterLink>
          </div>
        </div>
      </div>
      <div
        class="relative h-[35rem] w-1/3 rounded-2xl border border-gray-300 py-5 px-6"
      >
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
        class="relative h-[35rem] w-1/3 rounded-2xl border border-gray-300 py-5 px-6"
      >
        <div class="items-center justify-between">
          <h3 class="text-lg font-medium">Pending Reviews</h3>
          <div
            v-for="review in reviews?.slice(0, 4)"
            :key="review.id"
            class="my-7"
          >
            <div class="my-6 flex items-start space-x-3">
              <div
                class="mb-1 flex h-12 w-12 items-center justify-center space-x-px rounded-full bg-emerald-50 text-lg font-semibold text-emerald-700"
              >
                <p>
                  {{ review.user.firstName[0].toUpperCase() }}
                </p>
                <p>
                  {{ review.user.lastName[0].toUpperCase() }}
                </p>
              </div>
              <div class="w-64">
                <h2 class="leading-5">
                  <span class="font-medium"
                    >{{ review.user.firstName }}
                    {{ review.user.lastName }}</span
                  >
                  reviewed
                  <span class="font-medium">{{ review.business.name }} </span>
                  with grade {{ review.rating }}⭐
                </h2>
                <h3 class="mt-2">
                  <p class="text-gray-700">
                    {{ review.content }}
                  </p>
                </h3>
              </div>
              <RouterLink
                to="/reviews"
                class="absolute bottom-6 cursor-pointer font-medium underline"
              >
                View all pending reviews
              </RouterLink>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup>
import { ref, onMounted, computed } from 'vue'
import { RouterLink } from 'vue-router'
import { user } from '@/stores/userStore'
import Dropdown from '../ui/Dropdown.vue'
import BarChart from '../ui/charts/BarChart.vue'
import api from '@/api/api'
import { add, sub } from 'date-fns'
import { da } from 'date-fns/locale'

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

const earnings = ref({
  total: 0,
  values: []
})
const attendance = ref({
  total: 0,
  values: []
})

const reviews = ref([])
const registrationRequests = ref([])

const currentIncomeOption = ref('monthly')

const selectIncomeOption = e => {
  currentIncomeOption.value = e.value
  fetchEarnings()
  fetchAttendance()
}

const periodName = computed(() => {
  if (currentIncomeOption.value == 'yearly') return '2 years'
  else if (currentIncomeOption.value == 'monthly') return '6 months'
  else return '4 weeks'
})

const monthsFromIncome = computed(() => {
  if (currentIncomeOption.value == 'yearly') return 24
  else if (currentIncomeOption.value == 'monthly') return 6
  else return 1
})

const fetchEarnings = async () => {
  const [data, error] = await api.finances.getReport(
    sub(new Date(), { months: monthsFromIncome.value }),
    add(new Date(), { months: 0 })
  )
  if (!error) {
    data.forEach(p => (p.total = Number(p.total)))
    if (currentIncomeOption.value == 'yearly') {
      data.sort((a, b) => Number(a.year) - Number(b.year))
    } else if (currentIncomeOption.value == 'monthly') {
      data.sort((a, b) => Number(a.month) - Number(b.month))
    } else {
      data.sort((a, b) => Number(a.week) - Number(b.week))
    }
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
    if (currentIncomeOption.value == 'yearly') {
      data.sort((a, b) => Number(a.year) - Number(b.year))
    } else if (currentIncomeOption.value == 'monthly') {
      data.sort((a, b) => Number(a.month) - Number(b.month))
    } else {
      data.sort((a, b) => Number(a.week) - Number(b.week))
    }
    attendance.value = {
      values: data,
      total: data.reduce((sum, point) => sum + point.total, 0)
    }
  }
}

const fetchReviews = async () => {
  const [data, error] = await api.users.getPendingReviews()

  if (!error) reviews.value = data
}

const fetchRegistrationRequests = async () => {
  const [data, error] = await api.users.getRegistrationRequests()
  if (!error) registrationRequests.value = data
}

onMounted(() => {
  fetchEarnings()
  fetchAttendance()
  fetchReviews()
  fetchRegistrationRequests()
})
</script>
