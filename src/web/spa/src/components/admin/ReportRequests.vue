<template>
  <div class="mx-auto max-w-4.5xl">
    <h1 class="text-2xl font-medium">Pending reports</h1>
    <h4 class="pb-8">
      Advertisers and clients will be notified via email upon accepting a
      penalization request
    </h4>
    <div v-if="reports.length > 0" class="grid grid-cols-12 items-center">
      <p class="col-span-2 ml-1 text-sm text-neutral-500">User</p>
      <p class="col-span-3 ml-8 text-sm text-neutral-500">Business</p>
      <p class="col-span-2 ml-1 text-sm text-neutral-500">Date</p>
      <p class="col-span-2 text-sm text-neutral-500">Reason</p>
      <p class="col-span-2 ml-10 text-sm text-neutral-500">Showed up</p>
      <p class="text-sm text-neutral-500">Penalize</p>
      <div class="col-span-12 my-3 h-px bg-neutral-100"></div>
      <template v-for="report in reports" :key="report.id">
        <div class="col-span-2 my-2 flex items-center space-x-4">
          <div
            class="mb-1 flex h-12 w-12 items-center justify-center space-x-px rounded-full bg-emerald-50 text-xl font-semibold text-emerald-700"
          >
            <p>
              {{ report.user.firstName[0].toUpperCase() }}
            </p>
            <p>
              {{ report.user.lastName[0].toUpperCase() }}
            </p>
          </div>
          <div>
            <h2 class="text-lg leading-5">
              {{ report.user.firstName }} {{ report.user.lastName }}
            </h2>
            <h4 class="text-[13.5px] leading-4 text-neutral-500">
              {{ report.user.roles[0] }}
            </h4>
          </div>
        </div>
        <div class="col-span-3 ml-8">
          <h2 class="text-lg leading-5">
            {{ report.business.name }}
          </h2>
          <h3 class="text-sm text-neutral-500">
            {{ formatAddress(report.business.address) }}
          </h3>
        </div>
        <p class="col-span-2 text-sm text-neutral-500">
          {{ format(parseJSON(report.timestamp), 'MMM d, yyyy') }}
        </p>
        <p class="col-span-2 text-sm text-neutral-500">
          {{ report.reason }}
        </p>
        <p class="col-span-2 ml-10 text-sm text-neutral-500">
          {{ report.penalize ? 'No' : 'Yes' }}
        </p>
        <div v-if="!report.isApproved" class="flex space-x-2.5">
          <Button
            @click="decline(report)"
            type="button"
            title="Decline"
            class="h-8 w-8 rounded-md border !p-0 hover:bg-gray-100"
          >
            <XIcon
              class="pointer-events-none mx-auto h-4 w-4 text-neutral-600"
            />
          </Button>
          <Button
            @click="approve(report)"
            type="button"
            title="Accept"
            class="h-8 w-8 rounded-md border border-emerald-800 border-opacity-10 bg-emerald-50 !p-0 hover:bg-emerald-100"
          >
            <CheckIcon
              stroke-width="3"
              class="pointer-events-none mx-auto h-[18px] w-[18px] text-emerald-600"
            />
          </Button>
        </div>
        <div
          v-else-if="report.penalized"
          class="rounded-xl bg-emerald-100 px-2.5 py-1 text-xs font-medium text-emerald-700"
        >
          User penalized
        </div>
        <div
          v-else-if="!report.penalized"
          class="rounded-xl bg-red-100 px-2.5 py-1 text-xs font-medium text-red-700"
        >
          User not penalized
        </div>
      </template>
    </div>
    <div v-else class="text-gray-700">
      There are currently no pending reports.
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import api from '@/api/api'
import { formatAddress } from '../utility/address'
import { parseJSON, format } from 'date-fns'
import { XIcon, CheckIcon } from 'vue-tabler-icons'
import Button from '../ui/Button.vue'

const reports = ref([])

const [data, error] = await api.users.getPendingReports()
!error && (reports.value = data)
reports.value['penalized'] = false

const approve = async report => {
  const [, fetchError] = await api.users.approveReport(report.id, true)
  if (!fetchError) {
    report.isApproved = true
    report.penalized = true
  }
}

const decline = async report => {
  const [, fetchError] = await api.users.approveReport(report.id, false)
  if (!fetchError) {
    report.isApproved = true
    report.penalized = false
  }
}
</script>
