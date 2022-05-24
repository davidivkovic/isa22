<template>
  <div class="mx-auto mb-10 flex max-w-4.5xl space-x-10">
    <div class="flex-1">
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
            We will notify you via email whether or not your request gets
            accepted by our adminstartors.
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
            Enjoy your {{ loyaltyLevel.current.discountPercentage }}%
            reservation discount
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
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { format, parseJSON } from 'date-fns'
import {
  DiamondIcon,
  EditIcon,
  ArrowNarrowLeftIcon,
  UserIcon
} from 'vue-tabler-icons'
import Button from '@/components/ui/Button.vue'
import Input from '@/components/ui/Input.vue'
import Loader from '@/components/ui/Loader.vue'
import Modal from '@/components/ui/Modal.vue'
import api from '@/api/api'

const editMode = ref(false)
const loading = ref(false)
const deletionModalOpen = ref(false)

const user = ref()
const loyaltyLevel = ref()
const deletionRequest = ref()
const deletionReason = ref('')

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
</script>
