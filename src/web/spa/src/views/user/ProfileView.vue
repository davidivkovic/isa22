<template>
  <div class="mx-auto mb-10 max-w-4.5xl space-y-16">
    <div class="flex-1">
      <h1 class="text-2xl font-medium">Profile</h1>

      <div v-if="!editMode" class="mt-2 flex space-x-5">
        <div class="flex space-x-4 rounded-lg border border-neutral-300 p-5">
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
                v-if="
                  (!deletionRequest || deletionRequest.rejected) &&
                  !user.roles.includes('Admin')
                "
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
            We will notify you via email whether or not your request gets
            accepted by our adminstartors.
          </p>
        </div>

        <div
          v-if="!user.roles.includes('Admin')"
          :class="[
            {
              'text-white': loyaltyLevel.current?.color == '#000000'
            },
            `relative flex max-w-[360px] items-center justify-between space-x-5 overflow-clip rounded-lg border border-neutral-300 py-4 px-5 bg-[${
              loyaltyLevel.current?.color ?? '#ebebeb'
            }]`
          ]"
        >
          <div class="">
            <div class="flex items-center space-x-1">
              <span class="text-lg">Hi there! You are a</span>
              <h3 class="text-lg font-semibold">
                {{ loyaltyLevel.current?.name ?? 'Regular' }} Member
              </h3>
              <DiamondIcon class="h-5 w-5 text-black" />
            </div>
            <p
              v-if="loyaltyLevel.current"
              class="whitespace-nowrap text-sm font-medium leading-5"
            >
              Enjoy your {{ loyaltyLevel.current.discountPercentage }}%
              reservation discount
            </p>
            <p v-else class="text-sm font-medium leading-5">
              You currently have no discount
            </p>
            <h2 class="mt-3 font-semibold">{{ loyaltyLevel.points }} Points</h2>
            <div class="relative mt-2">
              <div
                class="absolute h-2 w-full rounded-lg bg-black opacity-[15%]"
              ></div>
              <div
                :style="{
                  width:
                    (loyaltyLevel.points / loyaltyLevel.next?.threshold) * 100 +
                    '%'
                }"
                class="absolute h-2 rounded-lg bg-black"
              ></div>
            </div>
            <p class="mt-6 text-[13px] font-medium text-neutral-600">
              Earn {{ loyaltyLevel.next?.threshold - loyaltyLevel.points }} more
              points to reach {{ loyaltyLevel.next?.name }} level and enjoy a
              {{ loyaltyLevel.next?.discountPercentage }}% discount
            </p>
          </div>
          <!-- <div
            class="flex h-28 w-28 items-center justify-center space-x-px rounded-full bg-neutral-100 text-2xl font-semibold"
          >
            <p>
              {{ user.firstName[0].toUpperCase() }}
            </p>
            <p>
              {{ user.lastName[0].toUpperCase() }}
            </p>
          </div> -->
        </div>
      </div>

      <div v-else>
        <button @click="editMode = false" class="group my-4 flex space-x-2">
          <ArrowNarrowLeftIcon
            stroke-width="1.25"
            class="group-hover:stroke-2"
          />
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
    </div>

    <div
      v-if="!user.roles.includes('Admin')"
      class="mx-auto max-w-4.5xl space-y-2"
    >
      <h1 class="text-2xl font-medium">Subscriptions</h1>
      <h2 class="text-gray-600">
        These are businesses you are subscribed to. You will get notified by
        email when a new sale is created.
      </h2>
      <Dropdown
        @change="e => (currentBusinessType = e.value)"
        label="Type"
        :values="businessTypes"
        class="w-fit"
      />

      <div
        v-for="subscription in subscriptions"
        :key="subscription.id"
        class="!mt-5 flex items-center justify-between rounded-xl border border-neutral-300 px-4 py-3.5"
      >
        <div class="flex space-x-4">
          <img
            :src="subscription.images[0]"
            alt=""
            class="h-24 w-24 rounded-lg object-cover"
          />
          <div class="font-medium">
            <RouterLink
              :to="`/${businessProfiles[currentBusinessType]}/${subscription.id}`"
            >
              {{ subscription.name }}
            </RouterLink>
            <h3 class="mt-1 text-sm font-normal text-neutral-500">
              {{ subscription.address.street }}
              {{ subscription.address.apartment }}
            </h3>
            <h3 class="text-sm font-normal text-neutral-500">
              {{ subscription.address.city }},
              {{ subscription.address.country }}
            </h3>
            <div
              class="mt-1 flex w-min items-center space-x-2 bg-emerald-50 p-1 text-sm font-bold text-emerald-700"
            >
              <StarIcon class="h-4 w-4" />
              <h3>
                {{ subscription.rating.toFixed(2) }}
              </h3>
            </div>
          </div>
        </div>
        <Button
          @click="unsubscribe(subscription)"
          class="flex !h-12 items-center space-x-2 border border-gray-300"
        >
          <p>Unsubscribe</p>
          <BellOffIcon class="h-4 w-4" />
        </Button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watchEffect } from 'vue'
import { format, parseJSON } from 'date-fns'
import {
  DiamondIcon,
  EditIcon,
  ArrowNarrowLeftIcon,
  StarIcon,
  BellOffIcon
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

const businessProfiles = {
  adventures: 'adventure-profile',
  cabins: 'cabin-profile',
  boats: 'boat-profile'
}

const currentBusinessType = ref(businessTypes[0].value)
const subscriptions = ref()

watchEffect(async () => {
  const [data, error] = await api.business.getSubscriptions(
    currentBusinessType.value
  )
  data && (subscriptions.value = data)
})

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
  deletionRequest.value = profileData.deletionRequest
}

const unsubscribe = async subscription => {
  const [data, error] = await api.business.unsubscribe(
    subscription.id,
    currentBusinessType.value
  )
  console.log(subscription.id)
  if (!error) {
    subscriptions.value = subscriptions.value.filter(
      s => s.id != subscription.id
    )
  }
}
</script>
