<template>
  <div class="mx-auto mb-5 h-full max-w-6xl space-y-5 border-t !py-8">
    <div class="flex h-[420px] w-full space-x-10">
      <div class="h-full w-[65%] space-y-3">
        <div class="flex h-full space-x-4">
          <div class="relative h-full flex-1">
            <img
              :src="props.entity?.images[0] ?? defaultImg"
              alt="Business image"
              class="h-full w-full rounded-sm object-cover"
            />
            <div
              class="absolute right-3 bottom-3 rounded-full bg-black/[0.5] py-2 px-10 text-sm text-white"
            >
              1 / {{ props.entity?.images?.length }}
            </div>
            <div
              class="absolute top-5 left-8 flex items-center justify-center space-x-1 whitespace-nowrap rounded-full bg-emerald-50 py-1.5 px-3 text-sm font-medium text-[#05a876]"
            >
              <div>
                <span class="font-bold"
                  >{{ props.entity.rating.toFixed(2) }}
                </span>
                ({{ props.entity.numberOfReviews }}
                reviews)
              </div>
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
      </div>
      <div class="h-fit flex-1 space-y-5">
        <div
          v-if="start && end"
          class="flex-col space-y-2 rounded-lg border p-5"
        >
          <h2 class="text-2xl font-medium">Your reservation</h2>
          <p class="!mt-1 text-sm">
            From
            <span class="font-medium text-emerald-600">{{ start }}</span> to
            <span class="font-medium text-emerald-600">{{ end }}</span> for
            <span class="font-medium text-emerald-600">{{ people }}</span>
            people.
          </p>
          <div class="!mt-3 flex justify-between">
            <div>
              <p>Price:</p>
              <p class="text-xs text-gray-500">
                {{ props.entity.pricePerUnit.symbol
                }}{{ props.entity.pricePerUnit.amount }} x {{ units }} nights x
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
            <div>
              <p>Services:</p>
              <p class="text-xs text-gray-500">no additional services</p>
            </div>
            <p>{{ props.entity.pricePerUnit.symbol }}0.00</p>
          </div>
          <div class="flex justify-between">
            <div>
              <p>Discount:</p>
              <!-- <p class="text-xs text-gray-500">no loyalty status</p> -->
              <div
                class="flex items-center space-x-1 rounded-lg bg-amber-300 px-1.5 py-0.5 text-xs"
              >
                <GiftIcon class="h-3 w-3 text-amber-800" />
                <p class="text-amber-800">Gold loyalty status</p>
              </div>
            </div>
            <p>{{ props.entity.pricePerUnit.symbol }}0.00</p>
          </div>
          <div class="!mt-5 flex justify-between border-t pt-2">
            <p class="text-lg font-bold">Total amount:</p>
            <p class="text-lg font-medium text-emerald-600">
              {{ totalPrice.toFixed(2) }}
            </p>
          </div>
          <div class="!mt-3 space-y-2">
            <div v-show="!isAuthenticated" class="text-xs text-gray-700">
              You need to
              <RouterLink to="/signin" class="underline">sign in </RouterLink>in
              order to make a reservation!
            </div>
            <Button
              :disabled="!isAuthenticated"
              class="w-full bg-emerald-600 text-white"
              >Make a reservation</Button
            >
          </div>
        </div>
        <div id="google-map" class="h-52 w-full rounded-md"></div>
        <div
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
    <div class="relative !mt-3 h-max w-[65%] flex-1">
      <div class="flex w-full items-center justify-between">
        <div class="text-[38px] font-medium">
          {{ props.entity.name }}
        </div>
        <button
          title="Share link"
          class="flex h-12 w-12 cursor-pointer items-center justify-center rounded-full bg-gray-100"
        >
          <ShareIcon class="h-5 w-5 text-gray-600" />
        </button>
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
      <div v-if="$route.name.includes('cabin')" class="mt-5 flex space-x-10">
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
      <div class="!mt-10 space-y-3">
        <h2 class="text-2xl font-medium">Rules</h2>
        <p class="text-[15px] text-gray-600">
          There are some rules during the stay in this cabin. Not following the
          rules may result in a bad review and penalty points.
        </p>
        <div class="!mt-3 flex flex-wrap gap-x-2 gap-y-3">
          <div
            v-for="rule in props.entity.rules"
            :key="rule.name"
            class="flex items-center justify-center space-x-1 rounded-full px-3 py-1 text-sm"
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
      </div>
      <div>
        <h2 class="!mt-10 text-2xl font-medium">Services</h2>
        <p class="text-[15px] text-gray-600">
          The cabin owner may offer some services that are additionally paid. If
          you want an additional service, please select it. It will be included
          in the total price.
        </p>
        <div class="mt-3">
          <div
            v-for="service in props.entity.services"
            :key="service.name"
            class="flex w-1/3 cursor-pointer items-end justify-between"
          >
            <Checkbox class="!-mt-6 !mr-2" />
            <p class="whitespace-nowrap text-[15px]">{{ service.name }}</p>
            <div
              class="border-top mx-2 mb-1.5 basis-10/12 border border-b-0 border-dashed border-neutral-300"
            ></div>
            <p class="font-semibold text-emerald-500">
              {{ symbols[service.price.currency] }}{{ service.price.amount }}
            </p>
          </div>
        </div>
      </div>
    </div>
    <div class="!mt-10 h-[260px] w-full space-y-3">
      <h2 class="text-lg font-medium">
        See what guests said about this cabin:
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
      <Button class="flex items-center space-x-2 border">
        <p>See all reviews</p>
        <MessagesIcon class="h-4 w-4" />
      </Button>
    </div>
    <slot name="characteristics"></slot>
    <slot name="biography"></slot>
  </div>
  <ImagesModal
    @modalClosed="isModalOpen = false"
    :isOpen="isModalOpen"
    :images="props.entity.images"
  />
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { RouterLink } from 'vue-router'
import {
  PhotoIcon,
  ShareIcon,
  CircleCheckIcon,
  CircleXIcon,
  MapPinIcon,
  UserIcon,
  BedIcon,
  HotelServiceIcon,
  ArrowNarrowRightIcon,
  MessagesIcon,
  GiftIcon
} from 'vue-tabler-icons'
import Checkbox from '../components/ui/Checkbox.vue'
import Button from '../components/ui/Button.vue'
import ImagesModal from '../components/business/view/ImagesModal.vue'
import defaultImg from '@/assets/images/default.png'
import { isAuthenticated } from '../stores/userStore.js'
import { computed } from '@vue/reactivity'
import { useRoute } from 'vue-router'
import { format, parseISO, differenceInDays } from 'date-fns'

const symbols = {
  USD: '$',
  EUR: '€',
  RSD: 'din '
}
const isModalOpen = ref(false)
const props = defineProps({
  entity: {
    type: Object,
    required: true
  }
})
const route = useRoute()
const people = ref(route.query.people)
const discount = ref(1)
console.log(route.query.start)
const start = route.query.start
  ? ref(format(parseISO(route.query.start), 'EEE, MMM dd'))
  : null
const end = route.query.start
  ? ref(format(parseISO(route.query.end), 'EEE, MMM dd'))
  : null
const units = ref(
  differenceInDays(parseISO(route.query.end), parseISO(route.query.start))
)
const totalPrice = computed(
  () =>
    props.entity.pricePerUnit.amount *
    units.value *
    people.value *
    discount.value
)

let markerRef, mapRef

const showMarker = () => {
  const latlng = new google.maps.LatLng(
    props.entity.address.latitude,
    props.entity.address.longitude
  )
  markerRef.setPosition(latlng)
  markerRef.setVisible(true)
  mapRef.setCenter(markerRef.getPosition())
  mapRef.setZoom(16)
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
    draggable: true,
    visible: false
    // icon: 'assets/images/map-marker.png',
    // size: new google.maps.Size(20, 32)
  })

  markerRef = marker

  google.maps.event.addListener(map, 'click', e => {
    marker.setPosition(e.latLng)
    marker.setVisible(true)
  })
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
