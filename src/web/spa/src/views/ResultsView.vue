<template>
  <div class="flex h-full flex-col space-y-10 px-10 py-3">
    <form
      @submit.prevent="search()"
      class="mx-auto ml-60 flex max-w-6xl justify-center space-x-3"
    >
      <div class="w-64">
        <Input
          id="location-input"
          required
          v-model="newLocation"
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
      <div class="w-60">
        <DateInput
          required
          @change="value => (start = value)"
          id="start"
          type="datetime-local"
          v-model="start"
          :hasTime="
            $route.params.type !== 'cabins' && $route.params.type !== 'home'
          "
          placeholder="Start of the journey"
          clearable
          class="h-12 w-full pl-8 text-sm"
        >
        </DateInput>
      </div>
      <div class="w-60">
        <DateInput
          required
          @change="value => (end = value)"
          type="datetime-local"
          v-model="end"
          :hasTime="
            $route.params.type !== 'cabins' && $route.params.type !== 'home'
          "
          placeholder="End of the journey"
          :lowerLimit="start"
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
      <Button
        type="submit"
        class="!ml-5 !rounded-md bg-emerald-600 !px-10 text-white"
        >Search</Button
      >
    </form>
    <div class="flex w-full flex-1 space-x-10 px-5">
      <FilterPanel
        @filter="f => filter(f)"
        :businessType="$route.params.type"
      />
      <div class="inline-block">
        <div class="justify mb-6 flex items-end justify-between">
          <h2 class="text-xl font-medium">
            {{ results.length }}
            {{ results.length === 1 ? 'Result' : 'Results' }}
            <span class="font-normal"> for {{ currentLocation }}</span>
            <div v-if="results.length === 0" class="mt-3 text-base font-normal">
              Unfortunately, there are no available
              {{ $route.params.type }}. Please change the search parameters.
            </div>
          </h2>
          <Dropdown
            v-if="results.length !== 0"
            @change="changeSelectedOption"
            :slim="true"
            :selectedValue="direction"
            class="-mb-2.5"
            :values="sortingOptions"
          />
        </div>
        <div class="grid h-3/4 basis-52 auto-rows-fr grid-cols-3 gap-8">
          <div
            v-for="result in results"
            :key="result.id"
            @mouseenter="showMarker(result)"
            class="group max-w-[12rem] cursor-pointer space-y-3.5"
          >
            <RouterLink
              :to="{
                name: routeName,
                params: { id: result.id },
                query: { start, end, people }
              }"
            >
              <component :is="component" :result="result" />
            </RouterLink>
          </div>
        </div>
        <div
          v-if="results.length"
          class="mt-10 flex justify-between space-x-10 text-sm"
        >
          <p>
            Showing
            <span class="font-medium"> {{ resultsFrom }}</span> to
            <span class="font-medium">
              {{ resultsTo }}
            </span>
            of <span class="font-medium"> {{ totalResults }} </span> results
          </p>
          <div class="flex space-x-5">
            <button
              @click="previousPage()"
              v-if="hasPrevious"
              class="flex items-center space-x-2 hover:underline"
            >
              <ArrowLeftIcon class="h-4 w-4" />
              <p>Previous</p>
            </button>
            <p>
              Page <span class="font-medium">{{ currentPage }}</span> of
              <span class="font-medium">{{ totalPages }} </span>
            </p>
            <button
              @click="nextPage()"
              v-if="hasNext"
              class="flex items-center space-x-2 hover:underline"
            >
              <p>Next</p>
              <ArrowRightIcon class="h-4 w-4" />
            </button>
          </div>
        </div>
      </div>
      <div
        v-show="results.length > 0"
        id="google-map"
        class="!mt-12 !ml-14 h-5/6 flex-1 rounded-xl bg-orange-50"
      ></div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import api from '../api/api'
import Dropdown from '@/components/ui/Dropdown.vue'
import Input from '../components/ui/Input.vue'
import DateInput from '../components/ui/DateInput.vue'
import NumberInput from '../components/ui/NumberInput.vue'
import Button from '../components/ui/Button.vue'
import FilterPanel from '../components/search/FilterPanel.vue'
import {
  MapPinIcon,
  UsersIcon,
  ArrowLeftIcon,
  ArrowRightIcon
} from 'vue-tabler-icons'
import { useRoute, useRouter, RouterLink } from 'vue-router'
import { googleMapsFlatStyle } from '@/components/utility/maps.js'
import CabinPreviewItem from '../components/search/CabinPreviewItem.vue'
import BoatPreviewItem from '../components/search/BoatPreviewItem.vue'
import AdventurePreviewItem from '../components/search/AdventurePreviewItem.vue'
import { parseISO, formatISO } from 'date-fns'

const route = useRoute()
const router = useRouter()
let markerRef, mapRef

const city = ref(route.query.city)
const country = ref(route.query.country)
const component = computed(() =>
  route.params.type === 'cabins'
    ? CabinPreviewItem
    : route.params.type == 'boats'
    ? BoatPreviewItem
    : AdventurePreviewItem
)
const routeName = computed(() =>
  route.params.type === 'cabins'
    ? 'cabin-profile'
    : route.params.type == 'boats'
    ? 'boat-profile'
    : 'adventure-profile'
)

const sortingOptions = [
  {
    name: 'Price - Highest first',
    value: 'price_desc'
  },
  {
    name: 'Price - Lowest first',
    value: 'price_asc'
  },
  {
    name: 'Rating - Highest first',
    value: 'rating_desc'
  }
]

const currentPage = ref(route.query?.page ?? 1)
const totalPages = ref(1)
const totalResults = ref(0)
const resultsFrom = computed(() => 1 + 6 * (currentPage.value - 1))
const resultsTo = computed(
  () => 6 * (currentPage.value - 1) + results.value.length
)
const hasNext = computed(() => currentPage.value < totalPages.value)
const hasPrevious = computed(() => currentPage.value > 1)
const filters = ref({})
const currentLocation = ref(`${city.value}, ${country.value}`)
const newLocation = ref(currentLocation.value)
const start = ref(route.query.start)
const end = ref(route.query.end)
const people = ref(Number(route.query.people))
const direction = ref(
  sortingOptions.find(option => option.value === route.query.direction) ??
    sortingOptions[0]
)

const results = ref([])

const fetchResults = async () => {
  if (Object.keys(route.query).length < 4) return
  const queryData = { ...route.query }
  if (route.params.type === 'cabins') {
    queryData.start = formatISO(parseISO(start.value).setHours(14))
    queryData.end = formatISO(parseISO(end.value).setHours(12))
  }
  const [data] = await api.business.search(
    {
      ...queryData
    },
    route.params.type,
    route.query.page - 1 || 0
  )
  if (data) {
    results.value = data.results
    totalResults.value = data.totalResults
    totalPages.value = Math.ceil(totalResults.value / 6)
  }
}

const changeSelectedOption = value => {
  direction.value = value
  search()
}

const filter = filtered => (filters.value = filtered)

const search = () => {
  router.push({
    name: 'search',
    query: {
      country: country.value,
      city: city.value,
      start: start.value,
      end: end.value,
      people: people.value,
      direction: direction.value.value,
      page: currentPage.value,
      ...filters.value
    }
  })
  setTimeout(() => {
    fetchResults()
  }, 100)
}

fetchResults()

const nextPage = () => {
  currentPage.value++
  search()
}

const previousPage = () => {
  currentPage.value--
  search()
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

if (!document.getElementById('google-maps-script')) {
  const mapsScript = document.createElement('script')
  mapsScript.setAttribute('id', 'google-maps-script')
  mapsScript.setAttribute(
    'src',
    `https://maps.googleapis.com/maps/api/js?key=${
      import.meta.env.VITE_GOOGLE_MAPS_KEY
    }&libraries=places&language=en`
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
        city.value = address.city
        country.value = address.country
        newLocation.value = `${city.value}, ${country.value}`
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
    styles: googleMapsFlatStyle
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
}
</script>
