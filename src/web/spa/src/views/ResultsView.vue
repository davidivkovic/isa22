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
      <div class="w-10"></div>
      <Button type="submit" class="bg-emerald-600 !px-10 text-white"
        >Search</Button
      >
    </form>
    <div class="flex w-full flex-1 space-x-10 px-5">
      <FilterPanel :type="$route.params.type" />
      <div class="inline-block">
        <div class="justify mb-6 flex items-end justify-between">
          <!-- <div v-if="isLoading">Loading.. Please wait</div> -->

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
        <div class="grid auto-rows-fr grid-cols-3 gap-8">
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
import { MapPinIcon, UsersIcon } from 'vue-tabler-icons'
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
  console.log(queryData)
  const [data] = await api.business.search(
    {
      ...queryData
    },
    route.params.type
  )
  data && (results.value = data.results)
}

const changeSelectedOption = value => {
  direction.value = value
  search()
}

const search = () => {
  router.push({
    name: 'search',
    query: {
      country: country.value,
      city: city.value,
      start: start.value,
      end: end.value,
      people: people.value,
      direction: direction.value.value
    }
  })
  setTimeout(() => {
    fetchResults()
  }, 100)
}

fetchResults()

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
