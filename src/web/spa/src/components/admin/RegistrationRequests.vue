<template>
  <div class="mx-auto max-w-4.5xl">
    <h1 class="text-2xl font-medium">Pending registration requests</h1>
    <h4 class="pb-8">
      Advertisers will be notified via email upon accepting or rejecting their
      requests
    </h4>
    <div v-if="requests.length > 0" class="grid grid-cols-4 items-center">
      <p class="ml-1 text-sm text-neutral-500">Name</p>
      <p class="col-span-2 ml-10 text-sm text-neutral-500">Comment</p>
      <p class="ml-12 text-sm text-neutral-500">Date</p>
      <div class="col-span-4 my-3 h-px bg-neutral-100"></div>
      <template
        v-for="request in requests"
        :key="request.user.id"
        v-memo="[request.request.approved, request.request.rejected]"
      >
        <div class="my-2 flex items-center space-x-4">
          <div
            :class="[
              getRandomColors(),
              'mb-1 flex h-12 w-12 items-center justify-center space-x-px rounded-full text-xl font-semibold'
            ]"
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
            <h3 class="text-sm text-neutral-500">
              {{ request.user.email }}
            </h3>

            <h4 class="text-[13.5px] leading-4 text-neutral-500">
              {{ request.user.roles[0] }}
            </h4>
          </div>
        </div>
        <p class="col-span-2 ml-10 text-sm text-neutral-500">
          {{ request.request.reason }}
        </p>
        <div class="ml-12 flex items-center space-x-6">
          <p class="whitespace-nowrap text-sm text-neutral-500">
            {{ format(parseJSON(request.request.timestamp), 'MMM d, yyyy') }}
          </p>
          <div
            v-if="!request.request.approved && !request.request.rejected"
            class="flex space-x-2.5"
          >
            <Button
              type="button"
              title="Decline"
              @click="declineRequest(request)"
              class="h-8 w-8 rounded-md border !p-0 hover:bg-gray-100"
            >
              <XIcon
                class="pointer-events-none mx-auto h-4 w-4 text-neutral-600"
              />
            </Button>
            <Button
              type="button"
              title="Accept"
              @click="acceptRequest(request)"
              class="h-8 w-8 rounded-md border border-emerald-800 border-opacity-10 bg-emerald-50 !p-0 hover:bg-emerald-100"
            >
              <CheckIcon
                stroke-width="3"
                class="pointer-events-none mx-auto h-[18px] w-[18px] text-emerald-600"
              />
            </Button>
          </div>
          <div
            v-else-if="request.request.approved"
            class="rounded-xl bg-emerald-100 px-2.5 py-1 text-xs font-medium text-emerald-700"
          >
            Approved
          </div>
          <div
            v-else-if="request.request.rejected"
            class="rounded-xl bg-red-100 px-3.5 py-1 text-xs font-medium text-red-700"
          >
            Declined
          </div>
        </div>
        <div class="col-span-4 my-3 h-px bg-neutral-100"></div>
      </template>
    </div>
    <div class="text-gray-700" v-else>
      There are currently no pending registration requests.
    </div>
  </div>
  <Modal
    light
    :isOpen="modalOpen"
    @modalClosed="modalOpen = false"
    class="-mt-10 max-w-sm !rounded-xl bg-white text-sm"
  >
    <form
      @submit.prevent="confirmDeclineRequest()"
      class="divide-y divide-gray-300"
    >
      <div class="w-full space-y-5 px-6 pt-8 pb-5">
        <div class="text-base">
          Please explain why you want to decline this user's registration
          request. <br />
          <span class="text-sm text-gray-400"
            >(E.g. "It is abusive or harmful.")
          </span>
        </div>
        <textarea
          required
          rows="5"
          placeholder="Enter a message here.."
          v-model="denialReason"
          class="w-full resize-none rounded-lg border border-gray-300 text-sm focus:border-gray-300 focus:ring-0"
        >
        </textarea>
      </div>
      <button type="submit" class="w-full py-4 px-20 font-medium text-red-500">
        Decline request
      </button>
      <button @click="modalOpen = false" class="w-full p-4">Cancel</button>
    </form>
  </Modal>
</template>

<script setup>
import { ref } from 'vue'
import api from '@/api/api.js'
import Button from '@/components/ui/Button.vue'
import Modal from '@/components/ui/Modal.vue'
import { CheckIcon, XIcon } from 'vue-tabler-icons'
import { format, parseJSON } from 'date-fns'

const colors = [
  ['bg-red-50', 'text-red-700'],
  ['bg-emerald-50', 'text-emerald-700'],
  ['bg-blue-50', 'text-blue-700'],
  ['bg-amber-50', 'text-amber-700'],
  ['bg-purple-50', 'text-purple-700'],
  ['bg-teal-50', 'text-teal-700'],
  ['bg-slate-50', 'text-slate-700'],
  ['bg-orange-50', 'text-orange-700'],
  ['bg-yellow-50', 'text-yellow-700'],
  ['bg-lime-50', 'text-lime-700'],
  ['bg-green-50', 'text-green-700'],
  ['bg-cyan-50', 'text-cyan-700'],
  ['bg-sky-50', 'text-sky-700'],
  ['bg-indigo-50', 'text-indigo-700'],
  ['bg-violet-50', 'text-violet-700'],
  ['bg-fuchsia-50', 'text-fuchsia-700'],
  ['bg-pink-50', 'text-pink-700'],
  ['bg-rose-50', 'text-rose-700']
]

const requests = ref([])
const modalOpen = ref(false)
const denialReason = ref('')
const requestToDecline = ref()

const getRandomColors = () => {
  return colors[(colors.length * Math.random()) | 0]
}

const [data, error] = await api.users.getRegistrationRequests()
if (!error) requests.value = data

const acceptRequest = async request => {
  const [, error] = await api.users.updateRegistrationRequest(
    request.user.email
  )
  if (!error) request.request.approved = true
}

const confirmDeclineRequest = async () => {
  const [, error] = await api.users.updateRegistrationRequest(
    requestToDecline.value.user.email,
    denialReason.value
  )
  if (!error) {
    modalOpen.value = false
    requestToDecline.value.request.rejected = true
  }
}

const declineRequest = request => {
  denialReason.value = ''
  requestToDecline.value = request
  modalOpen.value = true
}
</script>
