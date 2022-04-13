<template>
  <div class="mx-auto h-full max-w-6xl space-y-5 border-t py-5">
    <div class="flex h-[750px] w-full space-x-5 pt-5">
      <div class="flex h-[85%] w-[24%] flex-col space-y-4">
        <div class="space-y-10 rounded-md bg-amber-400 p-5">
          <div class="space-y-3">
            <Input
              class="w-full border-0"
              type="datetime-local"
              label="Start"
            />
            <Input class="w-full border-0" type="datetime-local" label="End" />
            <Input
              class="w-full border-0"
              type="number"
              label="People number"
              placeholder="Number of people"
              min="1"
            />
          </div>
          <Button class="w-full bg-emerald-600 font-medium text-white"
            >Search</Button
          >
        </div>
        <div
          class="relative flex flex-1 items-center justify-center rounded-md bg-gray-100 bg-[url(https://media.wired.com/photos/59269cd37034dc5f91bec0f1/master/pass/GoogleMapTA.jpg)]"
        >
          <div
            class="absolute top-0 z-10 h-full w-full rounded-md bg-black opacity-30"
          />
          <Button
            class="z-20 mx-auto !rounded-full border border-emerald-400 bg-emerald-50 px-3 !py-2 !text-sm !font-normal text-emerald-700"
            >Show on map</Button
          >
        </div>
      </div>
      <div class="flex h-full w-[76%] flex-col space-y-3">
        <div class="space-y-1">
          <div class="flex items-center justify-start space-x-4">
            <h1 class="rounded-md text-3xl font-bold">
              {{ props.entity.name }}
            </h1>
            <slot name="tag"></slot>
          </div>
          <div class="flex space-x-1">
            <MapPinIcon class="h-5 w-5" />
            <h2 class="text-sm text-gray-700">
              {{ props.entity.address.street }}
              {{ props.entity.address.apartment }},
              {{ props.entity.address.postalCode }}
              {{ props.entity.address.city }},
              {{ props.entity.address.country }}
            </h2>
          </div>
        </div>
        <div class="flex h-3/4 flex-col space-y-2">
          <div class="flex h-3/4 space-x-2">
            <div class="flex h-full w-1/3 flex-col space-y-2">
              <div class="h-1/2 overflow-hidden">
                <img
                  :src="props.entity.images[1]"
                  class="h-full w-full object-cover"
                />
              </div>
              <div class="h-1/2 overflow-hidden">
                <img
                  :src="props.entity.images[2]"
                  class="h-full w-full object-cover"
                />
              </div>
            </div>
            <div class="w-2/3">
              <img
                :src="props.entity.images[0]"
                class="h-full w-full object-cover"
              />
            </div>
          </div>
          <div class="flex h-1/4 flex-1 space-x-2">
            <div class="h-full w-1/4">
              <img
                :src="props.entity.images[3]"
                class="h-full w-full object-cover"
              />
            </div>
            <div class="h-full w-1/4">
              <img
                :src="props.entity.images[4]"
                class="h-full w-full object-cover"
              />
            </div>
            <div class="h-full w-1/4">
              <img
                :src="props.entity.images[5]"
                class="h-full w-full object-cover"
              />
            </div>
            <div class="relative h-full w-1/4">
              <img
                :src="props.entity.images[6]"
                class="h-full w-full object-cover"
              />
              <div
                class="absolute top-0 z-10 flex h-full w-full items-center justify-center rounded-md bg-black/[0.5]"
              >
                <button class="text-lg font-medium text-white underline">
                  + {{ props.entity.images.length - 7 }} pictures
                </button>
              </div>
            </div>
          </div>
        </div>
        <div class="!mt-8 flex justify-between px-10">
          <div class="space-y-2">
            <UsersIcon
              class="mx-auto h-7 w-7 text-gray-600"
              stroke-width="1.3"
            />
            <p>{{ props.entity.people }} people</p>
          </div>
          <div class="space-y-2">
            <StarIcon
              class="mx-auto h-7 w-7 text-gray-600"
              stroke-width="1.3"
            />
            <p>
              {{ props.entity.rating }}/10 ({{ props.entity.numberOfReviews }}
              reviews)
            </p>
          </div>
          <div class="space-y-2">
            <CashIcon
              class="mx-auto h-7 w-7 text-gray-600"
              stroke-width="1.3"
            />
            <p>
              {{ props.entity.pricePerUnit.symbol }}
              {{ props.entity.pricePerUnit.amount }}/per hour
            </p>
          </div>
          <div class="space-y-2">
            <CircleXIcon
              class="mx-auto h-7 w-7 text-gray-600"
              stroke-width="1.3"
            />
            <p>{{ props.entity.cancellationFee }} cancellation fee</p>
          </div>
          <div class="space-y-2">
            <MoodHappyIcon
              class="mx-auto h-7 w-7 text-gray-600"
              stroke-width="1.3"
            />
            <p>Much fun</p>
          </div>
        </div>
      </div>
    </div>
    <div class="flex space-x-20 border-t p-8">
      <div class="w-2/3 space-y-2">
        <h2 class="text-2xl font-semibold">About</h2>
        <p>{{ props.entity.description }}</p>
        <h3 class="!mt-10 text-lg font-semibold">Rules</h3>
        <p v-if="props.entity.rules.length !== 0">
          There are things that are allowed and not allowed.
        </p>
        <p v-else>There are no rules defined.</p>
        <div class="mt-3 flex flex-wrap gap-x-2 gap-y-3">
          <div
            v-for="rule in props.entity.rules"
            :key="rule.name"
            class="flex items-center justify-center space-x-1 rounded-full px-3 py-1"
            :class="rule.allowed ? 'bg-emerald-50' : 'bg-red-50'"
          >
            <Component
              :is="rule.allowed ? CircleCheckIcon : CircleXIcon"
              class="h-5 w-5"
              :class="rule.allowed ? 'text-emerald-700' : 'text-red-700'"
              stroke-width="2.2"
            />
            <p :class="rule.allowed ? 'text-emerald-700' : 'text-red-700'">
              {{ rule.name }}
            </p>
          </div>
        </div>
        <slot name="equipment"></slot>
      </div>
      <div class="h-fit w-1/3 space-y-3 rounded-md bg-gray-50 p-8">
        <h2 class="text-2xl font-semibold">Services</h2>
        <h3 v-if="props.entity.services.length !== 0" class="text-base">
          There are some services that are additionally payed.
        </h3>
        <h3 v-else>
          There are no additional services defined for this adventure.
        </h3>
        <div class="border-t py-2">
          <div
            v-for="service in props.entity.services"
            :key="service.name"
            class="flex justify-between py-1.5 text-sm font-medium"
          >
            <p>{{ service.name }}</p>
            <p>
              {{ service.price.symbol }}{{ service.price.amount.toFixed(2) }}
            </p>
          </div>
        </div>
      </div>
    </div>
    <slot name="characteristics"></slot>
    <slot name="biography"></slot>
  </div>
</template>

<script setup>
import {
  MapPinIcon,
  CircleCheckIcon,
  CircleXIcon,
  StarIcon,
  UsersIcon,
  CashIcon,
  MoodHappyIcon
} from 'vue-tabler-icons'
import Input from '../components/ui/Input.vue'
import Button from '../components/ui/Button.vue'

const props = defineProps({
  entity: {
    type: Object,
    required: true
  }
})
</script>

<style>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease-out;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
