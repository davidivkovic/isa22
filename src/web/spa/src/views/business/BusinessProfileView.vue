<template>
  <ErrorAlert @close="errorAlert = false" v-if="errorAlert">{{
    reservationError
  }}</ErrorAlert>
  <SuccessAlert @close="successAlert = false" v-if="successAlert"
    >Reservation sucessfully made.</SuccessAlert
  >
  <div class="mx-auto h-screen max-w-6xl space-y-5 border-t !py-8">
    <div v-if="isOwnersBusiness || user.role == 'Admin'" class="">
      <Button
        @click="editBusiness()"
        class="border !py-1 !text-base font-medium"
        >Edit</Button
      >
      <Button
        v-if="props.entity.isDeletable"
        @click="deleteBusiness()"
        class="!py-0 !text-base font-bold text-red-500"
        >Delete</Button
      >
    </div>
    <div class="flex w-full space-x-10">
      <div class="h-full w-[62.5%] space-y-3">
        <div class="flex h-[405px] space-x-4">
          <div class="relative flex-1">
            <img
              :src="props.entity?.images[0] ?? defaultImg"
              alt="Business image"
              class="h-full w-full rounded-sm object-cover"
            />
            <div
              class="absolute right-3 bottom-3 rounded-full bg-black/[0.5] py-2 px-10 text-sm text-white"
            >
              {{ props.entity.images.length == 0 ? 0 : 1 }} /
              {{ props.entity?.images?.length }}
            </div>
          </div>
          <div class="max h-full w-1/4 space-y-4">
            <div class="h-[31%] w-full">
              <img
                :src="props.entity?.images[1] ?? defaultImg"
                alt=""
                class="h-full w-full rounded-sm object-cover"
              />
            </div>
            <div class="h-[31%] w-full">
              <img
                :src="props.entity?.images[2] ?? defaultImg"
                alt=""
                class="h-full w-full rounded-sm object-cover"
              />
            </div>
            <div class="relative h-[31%] w-full">
              <img
                :src="props.entity?.images[3] ?? defaultImg"
                alt=""
                class="h-full w-full rounded-sm object-cover"
              />
              <button
                @click="isModalOpen = true"
                class="absolute top-0 left-0 z-10 flex h-full w-full cursor-pointer items-center justify-center space-x-1 rounded-sm bg-black/[0.5] text-sm text-white"
              >
                <PhotoIcon class="h-5 w-5 text-white" />
                <div>{{ props.entity?.images?.length }}</div>
              </button>
            </div>
          </div>
        </div>
        <div class="relative !mt-3 h-max flex-1">
          <div class="flex w-full items-center justify-between">
            <div class="text-[38px] font-medium">
              {{ props.entity.name }}
            </div>
            <p class="">
              <span class="text-3xl font-bold text-emerald-600"
                >{{ props.entity.pricePerUnit.symbol
                }}{{ props.entity.pricePerUnit.amount.toFixed(2) }}</span
              >
              / {{ entityType == 'cabins' ? 'night' : 'hour' }}
            </p>
          </div>
          <div
            class="-mt-2 flex items-center space-x-1 border-b pb-5 text-gray-500"
          >
            <MapPinIcon class="h-5 w-5" />
            <h2 class="">
              {{ props.entity.address.street }}
              {{ props.entity.address.apartment }},
              {{ props.entity.address.postalCode }}
              {{ props.entity.address.city }},
              {{ props.entity.address.country }}
            </h2>
          </div>
          <div v-if="props.entityType == 'cabins'" class="mt-5 flex space-x-14">
            <div class="flex items-center space-x-1 rounded-full">
              <HotelServiceIcon class="h-5 w-5" />
              <div class="text-lg">
                {{ props.entity.rooms.length }}
                {{ props.entity.rooms.length === 1 ? 'room' : 'rooms' }}
              </div>
            </div>
            <div class="flex items-center space-x-1 rounded-full">
              <BedIcon class="h-5 w-5" />
              <div class="text-lg">
                {{
                  props.entity.rooms
                    .map(item => item.beds)
                    .reduce((prev, curr) => prev + curr, 0)
                }}
                beds
              </div>
            </div>
            <div class="flex items-center space-x-1 rounded-full">
              <UserIcon class="h-5 w-5" />
              <div class="text-lg">{{ props.entity.people }} people</div>
            </div>
          </div>
          <div class="!mt-5 space-y-3">
            <h2 class="text-2xl font-medium">About</h2>
            <p class="text-[15px] text-gray-600">
              {{ props.entity.description }}
            </p>
          </div>
          <div v-if="entityType == 'boats'" class="!mt-10 w-full space-y-3">
            <div class="text-2xl font-medium">Characteristics</div>
            <div class="flex space-x-20">
              <div class="grid grid-cols-3 gap-x-10 gap-y-2">
                <div class="">
                  <ArrowsVerticalIcon class="-ml-2 stroke-2 text-emerald-700" />
                  <p class="mt-2 text-gray-400">Length</p>
                  <p class="font-bold">
                    {{ props.entity.characteristics.length }} meters
                  </p>
                </div>
                <div class="">
                  <UsersIcon class="-ml-1 stroke-2 text-emerald-700" />
                  <p class="mt-2 text-gray-400">Capacity</p>
                  <p class="font-bold">
                    {{ props.entity.characteristics.seats }} seats
                  </p>
                </div>
                <div class="">
                  <EngineIcon class="-ml-1 stroke-2 text-emerald-700" />
                  <p class="mt-2 text-gray-400">Engines</p>
                  <p class="font-bold">
                    {{ props.entity.characteristics.engines }} engines
                  </p>
                </div>
                <div class="">
                  <ArrowBigRightLinesIcon
                    class="-ml-1 stroke-2 text-emerald-700"
                  />
                  <p class="mt-2 text-gray-400">Top speed</p>
                  <p class="font-bold">
                    {{ props.entity.characteristics.topSpeed }} km/h
                  </p>
                </div>
                <div class="">
                  <BoltIcon class="-ml-1 stroke-2 text-emerald-700" />
                  <p class="mt-2 text-gray-400">Power</p>
                  <p class="font-bold">
                    {{ props.entity.characteristics.bhp }} bph
                  </p>
                </div>
              </div>
              <img :src="boatImage" alt="Boat image" class="h-52" />
            </div>
          </div>
          <div class="!mt-10 space-y-3">
            <h2 class="text-2xl font-medium">Rules</h2>
            <p class="text-[15px] text-gray-600">
              There are some rules during the stay in this cabin. Not following
              the rules may result in a bad review and penalty points.
            </p>
            <div class="flex space-x-20">
              <div class="space-y-2">
                <div>Allowed:</div>
                <div
                  v-for="rule in props.entity.rules.filter(r => r.allowed)"
                  :key="rule.name"
                  class="flex items-center space-x-1.5 text-sm"
                >
                  <CheckIcon class="h-4 w-4 text-gray-500" />
                  <p class="text-gray-500">
                    {{ rule.name }}
                  </p>
                </div>
              </div>
              <div class="space-y-2">
                <div>Not allowed:</div>
                <div
                  v-for="rule in props.entity.rules.filter(r => !r.allowed)"
                  :key="rule.name"
                  class="flex items-center space-x-1.5 text-sm"
                >
                  <XIcon class="h-4 w-4 text-gray-500" />
                  <p class="text-gray-500">
                    {{ rule.name }}
                  </p>
                </div>
              </div>
            </div>
          </div>
          <div v-if="props.entityType == 'adventures'" class="!mt-10 space-y-3">
            <h2 class="text-2xl font-medium">Equipment</h2>
            <p class="text-[15px] text-gray-600">
              Some equipment may be included in the offer and does not require
              additional payment.
            </p>
            <div class="flex items-center space-x-3">
              <FishIcon class="stroke-1" />
              <p class="font-medium">Fishing equipment</p>
            </div>
            <div
              v-for="equipment in props.entity.fishingEquipment"
              :key="equipment"
              class="flex items-center space-x-3"
            >
              <ChecksIcon class="h-5 w-5 stroke-1 text-gray-600" />
              <p class="text-sm text-gray-600">{{ equipment }}</p>
            </div>
          </div>
          <div v-if="entityType == 'boats'" class="!mt-10 space-y-3">
            <h2 class="text-2xl font-medium">Equipment</h2>
            <p class="text-[15px] text-gray-600">
              Some equipment may be included in the offer and does not require
              additional payment.
            </p>
            <div class="flex space-x-20">
              <div class="space-y-2">
                <div class="flex items-center space-x-3">
                  <CompassIcon class="stroke-1" />
                  <p class="font-medium">Navigational equipment</p>
                </div>
                <div
                  v-for="equipment in props.entity.equipment.navigational"
                  :key="equipment"
                  class="flex items-center space-x-3"
                >
                  <ChecksIcon class="h-5 w-5 stroke-1 text-gray-600" />
                  <p class="text-sm text-gray-600">{{ equipment }}</p>
                </div>
              </div>
              <div class="space-y-2">
                <div class="flex items-center space-x-3">
                  <FishIcon class="stroke-1" />
                  <p class="font-medium">Fishing equipment</p>
                </div>
                <div
                  v-for="equipment in props.entity.equipment.fishing"
                  :key="equipment"
                  class="flex items-center space-x-3"
                >
                  <ChecksIcon class="h-5 w-5 stroke-1 text-gray-600" />
                  <p class="text-sm text-gray-600">{{ equipment }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="flex-1 space-y-5">
        <div class="flex space-x-2">
          <div class="bg-emerald-50 p-1 text-xl font-bold text-emerald-700">
            {{ props.entity.rating.toFixed(2) }}
          </div>
          <div class="-space-y-1">
            <div class="text-sm font-medium text-gray-500">
              {{ ratingDesc }}
            </div>
            <div class="text-[13px]">
              {{ props.entity.numberOfReviews }} reviews
            </div>
          </div>
        </div>
        <div v-if="start && end" class="!mt-2 flex justify-between px-2">
          <div
            class="flex items-center space-x-2 text-sm font-medium text-gray-600"
          >
            <UsersIcon class="h-4 w-4 text-gray-600" />
            <div>{{ people }} {{ people == 1 ? 'Person' : 'Persons' }}</div>
          </div>
          <div class="text-sm">
            {{ units }} {{ entityType == 'cabins' ? 'Nights' : 'Hours' }} incl.
            tax
          </div>
        </div>
        <div
          v-if="start && end && !isOwnersBusiness"
          class="!mt-2 flex-col space-y-2 rounded-lg border p-5"
        >
          <h2 class="text-2xl font-medium">Your reservation</h2>
          <div class="!mt-3 flex justify-between">
            <div>
              <p>Price:</p>
              <p class="text-xs text-gray-500">
                {{ props.entity.pricePerUnit.symbol
                }}{{ props.entity.pricePerUnit.amount }} x {{ units }}
                {{ entityType == 'cabins' ? 'nights' : 'hours' }} x
                {{ people }} people
              </p>
            </div>
            <p>
              {{ props.entity.pricePerUnit.symbol
              }}{{
                (props.entity.pricePerUnit.amount * units * people).toFixed(2)
              }}
            </p>
          </div>
          <div class="flex justify-between">
            <p>Services:</p>
            <p v-if="selectedServices.length != 0">
              {{ props.entity.pricePerUnit.symbol
              }}{{ totalServices.toFixed(2) }}
            </p>
            <button
              @click="servicesOpen = !servicesOpen"
              v-else
              class="flex items-center space-x-1 text-xs text-emerald-600 hover:underline"
            >
              <p>{{ servicesOpen ? 'Close' : 'Add' }} services</p>
              <component
                :is="servicesOpen ? MinusIcon : PlusIcon"
                class="h-3 w-3"
              />
            </button>
          </div>
          <div v-if="servicesOpen" class="space-y-1">
            <p class="text-xs text-gray-500">
              Select additional services you wish to include in your
              reservation.
            </p>
            <div
              v-for="service in services"
              :key="service.name"
              class="flex cursor-pointer items-end justify-between"
            >
              <Checkbox v-model="service.selected" class="!-mt-6 !mr-2" />
              <p class="whitespace-nowrap text-[15px]">{{ service.name }}</p>
              <div
                class="border-top mx-2 mb-1.5 basis-10/12 border border-b-0 border-dashed border-neutral-300"
              ></div>
              <p class="font-semibold text-emerald-500">
                {{ symbols[service.price.currency]
                }}{{ service.price.amount.toFixed(2) }}
              </p>
            </div>
          </div>
          <div class="flex justify-between">
            <div>
              <p>Discount:</p>
              <p v-if="!user?.loyaltyLevel" class="text-xs text-gray-500">
                no loyalty status
              </p>
              <div
                v-else
                class="flex items-center justify-center space-x-1 rounded-lg bg-amber-300 py-0.5 text-xs"
              >
                <GiftIcon class="h-3 w-3 text-amber-800" />
                <p class="text-amber-800">user.loyaltyLevel.name</p>
              </div>
            </div>
            <p>{{ props.entity.pricePerUnit.symbol }}0.00</p>
          </div>
          <div class="!mt-5 flex justify-between border-t pt-2">
            <p class="text-lg font-bold">Total amount:</p>
            <p class="text-lg font-medium text-emerald-600">
              {{ props.entity.pricePerUnit.symbol }}{{ totalPrice.toFixed(2) }}
            </p>
          </div>
          <div class="text-gray-600">
            From
            <span class="font-bold text-emerald-700 underline">{{
              start
            }}</span>
            to
            <span class="font-bold text-emerald-700 underline">{{ end }}</span
            >.
          </div>
          <div class="!mt-3 space-y-2">
            <div v-show="!isAuthenticated" class="text-xs text-gray-700">
              You need to
              <RouterLink to="/signin" class="underline">sign in </RouterLink>in
              order to make a reservation!
            </div>
            <div
              v-if="isAuthenticated && !user.roles.includes('Customer')"
              class="text-xs text-gray-700"
            >
              You cannot make a reservation as a business owner. Please make a
              customer account.
            </div>
            <Button
              @click="makeReservation()"
              :disabled="!isCustomer"
              class="ml-48 bg-emerald-600 text-white"
              >Make a reservation</Button
            >
          </div>
        </div>
        <div
          id="google-map"
          class="h-52 w-full rounded-md"
          :class="{ 'h-[400px]': isOwnersBusiness }"
        ></div>
        <div class="space-y-3">
          <div class="flex items-center justify-between">
            <p class="text-2xl font-bold text-emerald-600">Sales</p>
            <Button
              v-if="!isOwnersBusiness && !props.entity.isSubscribed"
              class="border !py-2"
              >Subscribe</Button
            >
            <Button v-else-if="!isOwnersBusiness" class="border !py-2"
              >Unsubscribe</Button
            >
            <Button @click="createNewSale()" class="border !py-2" v-else
              >Create new sale</Button
            >
          </div>
          <p v-if="!isOwnersBusiness" class="text-sm text-gray-600">
            By subscribing, you will get notified when a new sale is created for
            this adventure. It's best to subsribe and never miss out a discount!
          </p>
          <div v-for="(sale, ind) in sortedSales" :key="sale.id">
            <div class="rounded-md border px-3 py-4">
              <div class="flex w-full items-center justify-between">
                <div class="grid space-y-1 text-[13px] text-gray-500">
                  <p v-if="sale.services.length == 0">No services</p>
                  <div v-else class="whitespace-nowrap font-medium">
                    <p>
                      {{
                        sale.services
                          .slice(0, 2)
                          .map(s => s.name)
                          .join(' ∙ ')
                      }}
                    </p>
                    <p>
                      {{
                        sale.services
                          .slice(2, 4)
                          .map(s => s.name)
                          .join(' ∙ ')
                      }}
                    </p>
                  </div>
                </div>
                <div class="flex space-x-5">
                  <div class="flex flex-col items-end">
                    <p
                      v-if="ind != 0"
                      class="text-sm text-gray-400 line-through"
                    >
                      {{ sale.price.symbol
                      }}{{
                        oldPrice(
                          sale.price.amount,
                          sale.discountPercentage
                        ).toFixed(2)
                      }}
                    </p>
                    <p
                      class="bg-emerald-50 p-1 text-xs font-medium text-emerald-700"
                      v-else
                    >
                      Top deal
                    </p>
                    <p class="text-lg font-bold text-emerald-700">
                      {{ sale.price.symbol }}{{ sale.price.amount.toFixed(2) }}
                    </p>
                  </div>
                  <Button
                    @click="makeQuickReservation(sale)"
                    v-if="!isOwnersBusiness"
                    class="= whitespace-nowrap rounded-full bg-emerald-600 text-white"
                  >
                    Book now
                  </Button>
                </div>
              </div>
              <details class="space-y-2">
                <summary
                  class="-mt-1.5 flex cursor-pointer items-center space-x-1"
                >
                  <p class="text-[13px]">Read more</p>
                  <ChevronDownIcon
                    id="summary-chevron"
                    class="h-4 w-4 transition"
                  />
                </summary>
                <div class="flex justify-between text-sm">
                  <div class="space-y-2 text-sm">
                    <div class="flex items-center space-x-2">
                      <CalendarIcon class="h-4 w-4" />
                      <p>
                        Starting at
                        <span class="font-medium text-emerald-700">
                          {{ format(parseISO(sale.start), dateFormat) }}
                        </span>
                      </p>
                    </div>
                    <div class="flex items-center space-x-2">
                      <CalendarIcon class="h-4 w-4" />
                      <p>
                        Ending at
                        <span class="font-medium text-emerald-700">
                          {{ format(parseISO(sale.end), dateFormat) }}
                        </span>
                      </p>
                    </div>
                    <div v-if="sale.services.length != 0">
                      <div class="flex flex-wrap space-x-1">
                        <div
                          v-for="service in sale.services"
                          :key="service.name"
                        >
                          <div class="flex items-center space-x-1">
                            <CheckIcon class="h-3 w-3" />
                            <p>{{ service.name }}</p>
                          </div>
                        </div>
                      </div>
                    </div>
                    <div class="flex items-center space-x-2">
                      <UserIcon class="h-4 w-4" />
                      <p>For {{ sale.people }} people</p>
                    </div>
                  </div>
                  <div class="text-gray-500">
                    <span class="font-bold text-emerald-600">
                      {{ sale.discountPercentage }}%
                    </span>
                    discount
                  </div>
                </div>
              </details>
            </div>
          </div>
        </div>
        <div
          v-if="!isAuthenticated || user.role == 'Customer'"
          class="to h-32 w-full rounded-md bg-amber-50 bg-gradient-to-b from-emerald-50 px-8 py-4"
        >
          <div class="w-2/3 space-y-1">
            <p class="text-2xl font-medium">We have the best offers!</p>
            <RouterLink
              :to="'/cabins'"
              class="flex items-center space-x-1 text-sm font-medium text-emerald-800 hover:underline"
            >
              <p>Explore more</p>
              <ArrowNarrowRightIcon class="h-4 w-4 text-emerald-800" />
            </RouterLink>
          </div>
        </div>
      </div>
    </div>
    <div class="!mt-10 h-[260px] w-full space-y-3">
      <h2 class="text-lg font-medium">
        See what guests said about this {{ entityType }}:
      </h2>
      <div class="flex h-full space-x-5">
        <div class="w-1/3 space-y-3 rounded-lg border px-5 py-6">
          <div class="flex items-center space-x-3">
            <div
              class="flex h-8 w-8 items-center justify-center rounded-full bg-emerald-600 font-medium text-white"
            >
              DI
            </div>
            <p class="text-sm font-medium">David Ivkovic</p>
          </div>
          <p class="h-32 text-sm text-gray-600">
            “This place is perfect for those who want to experience the nature
            of Zlatibor - due to it constantly expanding, many areas are left
            without any trees - but we definitely got that mountain-feel as the
            house is surrounded by a forest."
          </p>
          <p class="!mt-5 text-sm text-emerald-700">Read more</p>
        </div>
        <div class="w-1/3 space-y-3 rounded-lg border px-5 py-6">
          <div class="flex items-center space-x-3">
            <div
              class="flex h-8 w-8 items-center justify-center rounded-full bg-amber-500 font-medium text-white"
            >
              MD
            </div>
            <p class="text-sm font-medium">Milica Draca</p>
          </div>
          <p class="h-32 text-sm text-gray-600">
            “Cosy&clean appartements 10 minutes on foot from center of town,
            very friendly and nice host, well-equipped kitchen, quite beautiful
            place. I'm happy to stay there for 4 nights. Highly recommended!
            Thank you, Sanja. Wish you to have tourists 365 days…
          </p>
          <p class="!mt-5 text-sm text-emerald-700">Read more</p>
        </div>
        <div class="w-1/3 space-y-3 rounded-lg border px-5 py-6">
          <div class="flex items-center space-x-3">
            <div
              class="flex h-8 w-8 items-center justify-center rounded-full bg-violet-600 font-medium text-white"
            >
              MM
            </div>
            <p class="text-sm font-medium">Miladin Momcilovic</p>
          </div>
          <p class="h-32 text-sm text-gray-600">
            “Underfloor heating is great. It is very warm in the building. A
            fireplace that can be used to complement a romantic ambiance when
            needed is a great thing. There was everything in the apartment, from
            coffee, tea, candy, etc.”
          </p>
          <p class="!mt-5 text-sm text-emerald-700">Read more</p>
        </div>
      </div>
      <Button class="!mt-5 flex items-center space-x-2 border py-4">
        <p>See all reviews</p>
        <MessagesIcon class="h-4 w-4" />
      </Button>
      <div class="h-8"></div>
    </div>
  </div>
  <ImagesModal
    @modalClosed="isModalOpen = false"
    :isOpen="isModalOpen"
    :images="props.entity.images"
  />
  <DeleteBusinessModal
    :isOpen="isDeleteModalOpen"
    :businessId="entity.id"
    :type="props.entityType"
    @modalClosed="isDeleteModalOpen = false"
  />
  <CreateSaleModal
    :isOpen="isSalesModalOpen"
    :businessId="props.entity.id"
    :services="props.entity.services"
    :pricePerUnit="props.entity.pricePerUnit.amount"
    @modalClosed="isSalesModalOpen = false"
  />
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { RouterLink } from 'vue-router'
import {
  PhotoIcon,
  XIcon,
  MapPinIcon,
  UserIcon,
  BedIcon,
  HotelServiceIcon,
  ArrowNarrowRightIcon,
  PlusIcon,
  MinusIcon,
  MessagesIcon,
  GiftIcon,
  ArrowsVerticalIcon,
  UsersIcon,
  EngineIcon,
  ArrowBigRightLinesIcon,
  BoltIcon,
  CompassIcon,
  FishIcon,
  ChecksIcon,
  CheckIcon,
  ChevronDownIcon,
  CalendarIcon
} from 'vue-tabler-icons'
import Checkbox from '@/components/ui/Checkbox.vue'
import Button from '@/components/ui/Button.vue'
import ImagesModal from '@/components/business/view/ImagesModal.vue'
import DeleteBusinessModal from '@/components/business/delete/DeleteBusinessModal.vue'
import CreateSaleModal from '@/components/business/view/CreateSaleModal.vue'
import defaultImg from '@/assets/images/default.png'
import { isAuthenticated, user } from '@/stores/userStore.js'
import { computed } from '@vue/reactivity'
import { useRoute, useRouter } from 'vue-router'
import { format, parseISO, differenceInDays, differenceInHours } from 'date-fns'
import boatImage from '@/assets/images/boat.png'
import api from '@/api/api'
import ErrorAlert from '@/components/ui/alerts/ErrorAlert.vue'
import SuccessAlert from '@/components/ui/alerts/SuccessAlert.vue'

const symbols = {
  USD: '$',
  EUR: '€',
  RSD: 'din '
}
const isModalOpen = ref(false)
const isDeleteModalOpen = ref(false)
const isSalesModalOpen = ref(false)
const reservationError = ref('')
const errorAlert = ref(false)
const successAlert = ref(false)
const props = defineProps({
  entity: {
    type: Object,
    required: true
  },
  entityType: {
    type: String,
    required: true
  }
})

const showErrorAlert = () => {
  setTimeout(() => (errorAlert.value = true), 100)
  setTimeout(() => (errorAlert.value = false), 5000)
}

const showSuccessAlert = () => {
  setTimeout(() => (successAlert.value = true), 100)
  setTimeout(() => (successAlert.value = false), 5000)
}

const route = useRoute()
const router = useRouter()

const makeQuickReservation = async sale => {
  const [_, error] = await api.business.makeQuickReservation(
    props.entityType,
    sale.id
  )
  if (error) {
    reservationError.value = error
    showErrorAlert()
  } else {
    showSuccessAlert()
  }

  //TODO redirect to reservation preview
}

const makeReservation = async () => {
  const [_, error] = await api.business.makeResrvation(
    props.entity.id,
    props.entityType,
    parseISO(route.query.start),
    parseISO(route.query.end),
    people.value,
    selectedServices.value
  )

  if (error) {
    reservationError.value = error
    showErrorAlert()
  } else {
    showSuccessAlert()
  }
  //TODO redirect to reservation preview
}

const isCustomer = isAuthenticated.value && user.roles.includes('Customer')
const isOwnersBusiness = user.id == props.entity.owner?.id
const services = ref(props.entity.services)
services.value.forEach(s => (s.selected = false))

const people = ref(route.query.people)
const discount = ref(1)
const servicesOpen = ref(false)
const selectedServices = computed(() =>
  services.value.filter(service => service.selected)
)
const totalServices = computed(() =>
  selectedServices.value.reduce((acc, ser) => acc + ser.price.amount, 0)
)

const sortedSales = ref(props.entity.sales)
sortedSales.value.sort(
  (s1, s2) => s2.discountPercentage - s1.discountPercentage
)

const dateFormat =
  props.entityType == 'cabins' ? 'EEE, MMM dd' : 'EEE, MMM dd HH:mm'

const start = route.query.start
  ? ref(format(parseISO(route.query.start), dateFormat))
  : null
const end = route.query.start
  ? ref(format(parseISO(route.query.end), dateFormat))
  : null
const units = ref(
  props.entityType == 'cabins'
    ? differenceInDays(parseISO(route.query.end), parseISO(route.query.start))
    : differenceInHours(parseISO(route.query.end), parseISO(route.query.start))
)
const totalPrice = computed(
  () =>
    props.entity.pricePerUnit.amount *
      units.value *
      people.value *
      discount.value +
    totalServices.value
)

const ratingDesc = computed(() => {
  const rating = props.entity.rating
  if (rating >= 9) return 'Excellent'
  else if (rating >= 8) return 'Very good'
  else if (rating >= 7) return 'Good'
  else if (rating >= 6) return 'Not bad'
  else return 'Bad'
})

let markerRef, mapRef

const showMarker = () => {
  const latlng = new google.maps.LatLng(
    props.entity.address.latitude,
    props.entity.address.longitude
  )
  markerRef.setPosition(latlng)
  markerRef.setVisible(true)
  markerRef.setDraggable(false)
  mapRef.setCenter(markerRef.getPosition())
  mapRef.setZoom(16)
}

const deleteBusiness = () => {
  isDeleteModalOpen.value = true
}

const editBusiness = () => {
  router.push({
    name: 'create-update-business',
    params: { type: props.entityType, action: 'update' },
    query: { id: props.entity.id }
  })
}

const createNewSale = () => {
  isSalesModalOpen.value = true
}

const oldPrice = (newPrice, discPer) => {
  return (100 * newPrice) / (100 - discPer)
}

const createMap = () => {
  const center = {
    lat: 45.249739,
    lng: 19.848263
  }
  const map = new google.maps.Map(document.getElementById('google-map'), {
    center,
    zoom: 13,
    mapTypeControl: false,
    styles: [
      {
        featureType: 'administrative',
        elementType: 'labels.text.fill',
        stylers: [
          {
            color: '#444444'
          }
        ]
      },
      {
        featureType: 'landscape',
        elementType: 'all',
        stylers: [
          {
            color: '#f2f2f2'
          }
        ]
      },
      {
        featureType: 'poi',
        elementType: 'all',
        stylers: [
          {
            visibility: 'off'
          }
        ]
      },
      {
        featureType: 'poi',
        elementType: 'geometry.fill',
        stylers: [
          {
            visibility: 'on'
          },
          {
            color: '#e9e9e9'
          }
        ]
      },
      {
        featureType: 'poi.attraction',
        elementType: 'all',
        stylers: [
          {
            visibility: 'on'
          }
        ]
      },
      {
        featureType: 'poi.business',
        elementType: 'all',
        stylers: [
          {
            visibility: 'on'
          }
        ]
      },
      {
        featureType: 'poi.government',
        elementType: 'all',
        stylers: [
          {
            visibility: 'off'
          }
        ]
      },
      {
        featureType: 'poi.medical',
        elementType: 'all',
        stylers: [
          {
            visibility: 'on'
          }
        ]
      },
      {
        featureType: 'poi.park',
        elementType: 'geometry.fill',
        stylers: [
          {
            color: '#deebd8'
          },
          {
            visibility: 'on'
          }
        ]
      },
      {
        featureType: 'poi.school',
        elementType: 'all',
        stylers: [
          {
            visibility: 'on'
          }
        ]
      },
      {
        featureType: 'poi.sports_complex',
        elementType: 'all',
        stylers: [
          {
            visibility: 'on'
          }
        ]
      },
      {
        featureType: 'poi.sports_complex',
        elementType: 'labels',
        stylers: [
          {
            visibility: 'on'
          }
        ]
      },
      {
        featureType: 'road',
        elementType: 'all',
        stylers: [
          {
            saturation: -100
          },
          {
            lightness: 45
          }
        ]
      },
      {
        featureType: 'road.highway',
        elementType: 'all',
        stylers: [
          {
            visibility: 'simplified'
          }
        ]
      },
      {
        featureType: 'road.arterial',
        elementType: 'labels.icon',
        stylers: [
          {
            visibility: 'off'
          }
        ]
      },
      {
        featureType: 'transit',
        elementType: 'all',
        stylers: [
          {
            visibility: 'off'
          }
        ]
      },
      {
        featureType: 'water',
        elementType: 'all',
        stylers: [
          {
            color: '#c4e5f3'
          },
          {
            visibility: 'on'
          }
        ]
      }
    ]
  })
  mapRef = map

  const marker = new google.maps.Marker({
    position: new google.maps.LatLng(center.lat, center.lng),
    map: map,
    draggable: false,
    visible: false,
    clickable: false
    // icon: 'assets/images/map-marker.png',
    // size: new google.maps.Size(20, 32)
  })

  markerRef = marker
  showMarker()
}

if (!document.getElementById('google-maps-script')) {
  const mapsScript = document.createElement('script')
  mapsScript.setAttribute('id', 'google-maps-script')
  mapsScript.setAttribute(
    'src',
    `https://maps.googleapis.com/maps/api/js?key=${
      import.meta.env.VITE_GOOGLE_MAPS_KEY
    }&libraries=places`
  )
  mapsScript.onload = () => createMap()
  document.head.appendChild(mapsScript)
} else {
  onMounted(() => createMap())
}
</script>

<style scoped>
details > summary {
  list-style: none;
}
details > summary::marker {
  display: none;
}
details[open] > summary svg {
  transform: scaleY(-1);
}
</style>
