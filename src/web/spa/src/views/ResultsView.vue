<template>
  <div class="flex h-full flex-col space-y-10 px-10 py-3">
    <form
      @submit.prevent="search()"
      class="mx-auto flex max-w-6xl justify-center space-x-3"
    >
      <div class="w-64">
        <Input
          id="location-input"
          required
          v-model="location"
          placeholder="Enter a destination"
          clearable
          class="h-12 w-64 pl-8"
        >
          <template #prepend="{ focused, hovered }">
            <MapPinIcon
              class="ml-2.5 mr-1.5 h-[18px] w-[18px] transition-all"
              :class="[
                focused || hovered ? 'text-neutral-700' : 'text-neutral-400'
              ]"
            />
          </template>
        </Input>
      </div>
      <div class="w-52">
        <DateInput
          required
          @change="value => (start = value)"
          id="start"
          type="datetime-local"
          ref="startInput"
          v-model="start"
          :hasTime="false"
          placeholder="Start of the journey"
          clearable
          class="h-12 w-full pl-8 text-sm"
        >
        </DateInput>
      </div>
      <div class="w-52">
        <DateInput
          required
          @change="value => (end = value)"
          type="datetime-local"
          ref="startInput"
          v-model="end"
          :hasTime="false"
          placeholder="End of the journey"
          clearable
          class="h-12 w-full pl-8 text-sm"
        >
        </DateInput>
      </div>
      <NumberInput
        required
        v-model="people"
        min="1"
        clearable
        class="h-12 w-24 pl-10"
      >
        <template #prepend="{ focused, hovered }">
          <UsersIcon
            class="ml-2.5 mr-1.5 h-[18px] w-[18px] transition-all"
            :class="[
              focused || hovered ? 'text-neutral-700' : 'text-neutral-400'
            ]"
          />
        </template>
      </NumberInput>
      <Button type="submit" class="bg-emerald-600 !px-10 text-white"
        >Search</Button
      >
    </form>
    <div class="flex w-full flex-1 space-x-10 px-5">
      <div class="-mt-20 h-full basis-[11.2rem] space-y-3">
        <p class="text-lg font-medium">Filter by:</p>
        <div id="rating" class="space-y-1.5 py-3">
          <p class="font-medium">Rating</p>
          <RadioButton label="Superb: 9+" id="9" name="rating" />
          <RadioButton label="Very: good 8+" id="8" name="rating" />
          <RadioButton label="Good: 7+" id="7" name="rating" />
          <RadioButton label="Pleasant: 6+" id="6" name="rating" />
        </div>
        <div id="rooms" class="space-y-1.5 py-3">
          <p class="font-medium">Room number</p>
          <RadioButton label="1 room" id="1" name="rooms" />
          <RadioButton label="2 rooms" id="2" name="rooms" />
          <RadioButton label="3 rooms" id="3" name="rooms" />
          <RadioButton label="more" id="more" name="rooms" />
        </div>
        <div class="space-y-1.5 py-3">
          <p class="font-medium">Price</p>
          <RadioButton label="$0-20 /night" id="0-20" name="price" />
          <RadioButton label="$20-30 /night" id="20-30" name="price" />
          <RadioButton label="$30-50 /night" id="30-50" name="price" />
          <RadioButton label="$50-100 /night" id="50-100" name="price" />
          <RadioButton label="more" value="more" />
        </div>
        <Button
          @click="applyFilters()"
          class="!mt-24 w-full !border border-emerald-600 text-emerald-600"
          >Apply</Button
        >
      </div>
      <div class="inline-block">
        <div class="justify mb-6 flex items-end justify-between">
          <h2 class="text-xl font-medium">
            {{ results.length }} Results
            <span class="font-normal">
              for {{ $route.query.City }}, {{ $route.query.Country }}</span
            >
          </h2>
          <Dropdown
            @change="changeSelectedOption"
            :slim="true"
            class="-mb-2.5"
            :values="sortingOptions"
          />
        </div>
        <div class="grid auto-rows-fr grid-cols-3 gap-8">
          <div
            v-for="result in results"
            :key="result.id"
            @mouseenter="showMarker(result)"
            class="group max-w-[12rem] cursor-pointer space-y-3.5"
          >
            <div class="relative h-36 w-48">
              <h4
                class="absolute top-2 right-1.5 rounded-xl bg-emerald-50 px-2.5 py-0.5 text-[13px] font-semibold tracking-tight text-[#05a876]"
              >
                {{ result.rating.toPrecision(2) }}
              </h4>
              <img
                alt=""
                :src="result.image"
                class="h-full w-full rounded object-cover ring-2 ring-emerald-500 ring-opacity-0 ring-offset-2 transition-all group-hover:ring-opacity-100"
                :class="{
                  'ring-opacity-100': result.selected
                }"
              />
            </div>
            <div>
              <div class="flex items-start justify-between">
                <div>
                  <h2 class="font-semibold">
                    {{ result.name }}
                  </h2>
                  <h3 class="text-[13px] text-gray-500">
                    {{ result.address.street }} {{ result.address.apartment }}
                  </h3>
                  <div class="flex items-center space-x-1 pt-1.5">
                    <h3
                      class="text-lg font-semibold tracking-tight text-emerald-500"
                    >
                      {{ result.price.symbol }}{{ result.price.amount }}
                    </h3>
                    <span class="font text-sm text-gray-600">/ night</span>
                  </div>
                </div>
              </div>
              <div class="mt-1.5 flex items-center space-x-2.5">
                <div class="flex items-center space-x-1 text-neutral-500">
                  <HotelServiceIcon stroke-width="2" class="h-4 w-4 pb-px" />
                  <h4 class="font- text-[13px]">{{ result.rooms }}</h4>
                </div>
                <div class="flex items-center space-x-1 text-neutral-500">
                  <BedIcon stroke-width="2" class="h-4 w-4 pb-px" />
                  <h4 class="font- text-[13px]">{{ result.beds }}</h4>
                </div>
                <div class="flex items-center space-x-1 text-neutral-500">
                  <UserIcon stroke-width="2" class="h-4 w-4 pb-px" />
                  <h4 class="font- text-[13px]">{{ result.people }}</h4>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div
        id="google-map"
        class="!mt-12 !ml-14 h-5/6 flex-1 rounded-xl bg-orange-50"
      ></div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch, computed } from 'vue'
import { format } from 'date-fns'
import api from '../api/api'
import Dropdown from '@/components/ui/Dropdown.vue'
import Input from '../components/ui/Input.vue'
import DateInput from '../components/ui/DateInput.vue'
import NumberInput from '../components/ui/NumberInput.vue'
import Button from '../components/ui/Button.vue'
import RadioButton from '../components/ui/RadioButton.vue'
import {
  UserIcon,
  BedIcon,
  HotelServiceIcon,
  MapPinIcon,
  UsersIcon
} from 'vue-tabler-icons'
import { useRoute, useRouter } from 'vue-router'

const route = useRoute()
const router = useRouter()
let markerRef, mapRef

const location = computed(() => `${route.query.City}, ${route.query.Country}`)
const start = ref(route.query.Start)
const end = ref(route.query.End)
const people = ref(Number(route.query.People))
const selectedSortOption = ref('')

const results = ref([])

watch(
  () => route.query,
  async () => {
    const [data, error] = await api.cabins.search(
      route.query.City,
      route.query.Country,
      format(Date.parse(start.value), "yyyy-MM-dd'T'HH:mm:ss'Z'"),
      format(Date.parse(end.value), "yyyy-MM-dd'T'HH:mm:ss'Z'"),
      people.value
    )
    data && (results.value = data)
  },
  {
    immediate: true
  }
)

const sortingOptions = [
  {
    name: 'Price - Highest first',
    value: 'price_dsc'
  },
  {
    name: 'Price - Lowest first',
    value: 'price_asc'
  },
  {
    name: 'Rating - Highest first',
    value: 'rating_dsc'
  }
]

const changeSelectedOption = value => {
  selectedSortOption.value = value
  console.log(selectedSortOption.value)
}

const search = () => {
  console.log(location.value, start.value, end.value, people.value)
  const [City, Country] = location.value.split(', ')
  console.log(start.value)
  router.push({
    name: 'search',
    query: {
      City,
      Country,
      Start: start.value,
      End: end.value,
      People: people.value
    }
  })
}

const applyFilters = () => {
  const rating = document.querySelector('input[name="rating"]:checked')?.id
  const rooms = document.querySelector('input[name="rooms"]:checked')?.id
  let price = document.querySelector('input[name="price"]:checked')?.id
  const [min, max] = price.split('-')
  price = { min, max, currency: 'USD' }
  const filters = { rating, rooms, price }
  console.log(filters)
}

const showMarker = result => {
  const latlng = new google.maps.LatLng(
    result.address.latitude,
    result.address.longitude
  )
  markerRef.setPosition(latlng)
  markerRef.setVisible(true)
  mapRef.setCenter(markerRef.getPosition())
  mapRef.setZoom(16)
}

// const results = [
//   {
//     id: 1,
//     latitude: 43.72609,
//     longitude: 19.702924,
//     selected: false,
//     name: 'Davidova kuca',
//     street: 'Avenue M',
//     number: '4308',
//     area: 'Flatlands',
//     city: 'Brooklyn',
//     price: '$1500',
//     rating: 8.4,
//     people: 2,
//     beds: 2,
//     rooms: 1,
//     image:
//       'https://images.unsplash.com/photo-1562663474-6cbb3eaa4d14?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=987&q=80'
//   },
//   {
//     id: 2,
//     name: 'Binkovo odmaraliste malo slatko',
//     latitude: 43.72949650770515,
//     longitude: 19.704549372377482,
//     selected: false,
//     street: 'E 92nd St',
//     number: '636',
//     area: 'Canarise',
//     city: 'Brooklyn',
//     price: '$140',
//     rating: 8.0,
//     people: 1,
//     beds: 2,
//     rooms: 1,
//     image:
//       'https://images.unsplash.com/photo-1586023492125-27b2c045efd7?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=1558&q=80'
//   },
//   {
//     id: 3,
//     name: 'Ljiljini konaci',
//     latitude: 43.722411,
//     longitude: 19.697138,
//     selected: false,
//     street: 'Barnes Ave',
//     number: '2922',
//     area: 'Williamsbridge',
//     city: 'Bronx',
//     price: '$200',
//     rating: 9.4,
//     people: 3,
//     beds: 4,
//     rooms: 2,
//     image:
//       'https://images.unsplash.com/photo-1522444195799-478538b28823?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=987&q=80'
//   },
//   {
//     id: 4,
//     name: 'Micina vikendica',
//     latitude: 43.7292411,
//     longitude: 19.696778,
//     selected: false,
//     street: 'Broome St',
//     number: '565',
//     area: 'SoHo',
//     city: 'New York',
//     price: '$360',
//     rating: 9.8,
//     people: 6,
//     beds: 6,
//     rooms: 3,
//     image:
//       'https://images.unsplash.com/photo-1493663284031-b7e3aefcae8e?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2070&q=80'
//   },
//   {
//     id: 5,
//     name: 'Teddijev vrt',
//     latitude: 43.7296664,
//     longitude: 19.697782,
//     selected: false,
//     street: 'Broome St',
//     number: '565',
//     area: 'SoHo',
//     city: 'New York',
//     price: '$360',
//     rating: 9.8,
//     people: 6,
//     beds: 6,
//     rooms: 3,
//     image:
//       'https://images.unsplash.com/photo-1600210491369-e753d80a41f3?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=1974&q=80'
//   },
//   {
//     id: 6,
//     name: 'Eddijeva vikendica',
//     latitude: 43.7389411,
//     longitude: 19.698573,
//     selected: false,
//     street: 'Broome St',
//     number: '565',
//     area: 'SoHo',
//     city: 'New York',
//     price: '$360',
//     rating: 9.8,
//     people: 6,
//     beds: 6,
//     rooms: 3,
//     image:
//       'https://images.unsplash.com/photo-1537726235470-8504e3beef77?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2070&q=80'
//   }
// ]

if (!document.getElementById('google-maps-script')) {
  const mapsScript = document.createElement('script')
  mapsScript.setAttribute('id', 'google-maps-script')
  mapsScript.setAttribute(
    'src',
    `https://maps.googleapis.com/maps/api/js?key=${
      import.meta.env.VITE_GOOGLE_MAPS_KEY
    }&libraries=places`
  )
  mapsScript.onload = () => googleMapsSetUp()
  document.head.appendChild(mapsScript)
} else {
  onMounted(() => googleMapsSetUp())
}

const googleMapsSetUp = () => {
  createAutocompleteInput()
  createMap()
}

const createAutocompleteInput = () => {
  const geocoder = new window.google.maps.Geocoder()
  const input = document.getElementById('location-input')
  const options = {
    types: ['(regions)']
  }
  const autocomplete = new window.google.maps.places.Autocomplete(
    input,
    options
  )
  autocomplete.addListener('place_changed', () => {
    const place = autocomplete.getPlace()
    geocoder.geocode(
      {
        latLng: place.geometry.location
      },
      (results, status) => {
        if (status != window.google.maps.GeocoderStatus.OK) return
        const address = getAddress(results)
        location.value = `${address.city}, ${address.country}`
      }
    )
  })
}
const getAddress = results => {
  const address = results.find(r =>
    r.address_components.find(c => c.types.includes('postal_code'))
  )
  return {
    postalCode: extractFromAddress(address, 'postal_code'),
    country: extractFromAddress(results[0], 'country'),
    city: extractFromAddress(results[0], 'locality'),
    street: extractFromAddress(results[0], 'route'),
    apartment: extractFromAddress(results[0], 'street_number'),
    region: extractFromAddress(results[0], 'administrative_area_level_1'),
    latitude: results[0].geometry.location.lat(),
    longitude: results[0].geometry.location.lng()
  }
}

const extractFromAddress = (address, key) => {
  return address?.address_components?.find(a => a.types.includes(key))
    ?.long_name
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
}
</script>
