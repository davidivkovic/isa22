<template>
  <div class="mx-auto max-w-4.5xl">
    <h1 class="text-2xl font-medium">Pending reviews</h1>
    <h4 class="pb-8">
      Advertisers will be notified via email upon accepting a customer's review
    </h4>
    <div v-if="requests.length > 0" class="grid grid-cols-8 items-center">
      <p class="col-span-2 ml-1 text-sm text-neutral-500">User</p>
      <p class="col-span-2 ml-1 text-sm text-neutral-500">Business</p>
      <p class="col-span-2 ml-10 text-sm text-neutral-500">Content</p>
      <p class="ml-10 text-sm text-neutral-500">Rating</p>
      <p class="ml-12 text-sm text-neutral-500">Date</p>
      <div class="col-span-8 my-3 h-px bg-neutral-100"></div>
      <template v-for="request in requests" :key="request.user.id">
        <div class="col-span-2 my-2 flex items-center space-x-4">
          <div
            class="mb-1 flex h-12 w-12 items-center justify-center space-x-px rounded-full bg-emerald-50 text-xl font-semibold text-emerald-700"
          >
            <p>
              {{ request.user.firstName[0].toUpperCase() }}
            </p>
            <p>
              {{ request.user.lastName[0].toUpperCase() }}
            </p>
          </div>
          <div>
            <h2 class="text-lg leading-5">
              {{ request.user.firstName }} {{ request.user.lastName }}
            </h2>
            <h4 class="text-[13.5px] leading-4 text-neutral-500">
              {{ request.user.roles[0] }}
            </h4>
          </div>
        </div>
        <div class="col-span-2">
          <h2 class="text-lg leading-5">
            {{ request.business.name }}
          </h2>
          <h3 class="text-sm text-neutral-500">
            {{ formatAddress(request.business.address) }}
          </h3>
        </div>
        <p class="col-span-2 ml-10 text-sm text-neutral-500">
          {{ request.content }}
        </p>
        <p class="col-span ml-10 text-sm text-neutral-500">
          {{ request.rating }}
        </p>
        <div class="ml-12 flex items-center space-x-6">
          <p class="whitespace-nowrap text-sm text-neutral-500">
            {{ format(parseJSON(request.timestamp), 'MMM d, yyyy') }}
          </p>
          <div
            v-if="!request.approved && !request.rejected"
            class="flex space-x-2.5"
          >
            <Button
              @click="decline(request)"
              type="button"
              title="Decline"
              class="h-8 w-8 rounded-md border !p-0 hover:bg-gray-100"
            >
              <XIcon
                class="pointer-events-none mx-auto h-4 w-4 text-neutral-600"
              />
            </Button>
            <Button
              @click="accept(request)"
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
            v-else-if="request.approved"
            class="rounded-xl bg-emerald-100 px-2.5 py-1 text-xs font-medium text-emerald-700"
          >
            Approved
          </div>
          <div
            v-else-if="request.rejected"
            class="rounded-xl bg-red-100 px-3.5 py-1 text-xs font-medium text-red-700"
          >
            Declined
          </div>
        </div>
        <div class="col-span-8 my-3 h-px bg-neutral-100"></div>
      </template>
    </div>
    <div class="text-gray-700" v-else>
      There are currently no pending review requests.
    </div>
  </div>
</template>
<script setup>
import { ref } from 'vue'
import api from '@/api/api'
import Button from '../ui/Button.vue'
import { format, parseJSON } from 'date-fns'
import { formatAddress } from '../utility/address'
import { CheckIcon, XIcon } from 'vue-tabler-icons'

const requests = ref([])
const [data, error] = await api.users.getPendingReviews()
!error && (requests.value = data)

const accept = async request => {
  console.log('accept')
  const [, error] = await api.users.approveReview(request.id, true)
  if (!error) {
    request.rejected = false
    request.approved = true
  }
}

const decline = async request => {
  const [, error] = await api.users.approveReview(request.id, false)
  if (!error) {
    request.rejected = true
    request.approved = false
  }
}
</script>
