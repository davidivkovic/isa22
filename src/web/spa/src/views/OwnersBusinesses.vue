<template>
  <div class="justify-between">
    <div class="space-y-2">
      <div class="mx-auto flex max-w-5xl items-center justify-between py-6">
        <form
          @submit.prevent="search()"
          class="max-auto ml-20 flex max-w-6xl justify-center space-x-3"
        >
          <div class="flex w-64 space-x-10">
            <Input
              id="name-input"
              v-model="cabinsName"
              placeholder="Enter name"
              clearable
              class="h-12 w-64"
            />
            <Input
              id="location-input"
              v-model="newLocation"
              placeholder="Enter a location"
              clearable
              class="h-12 w-64 !pl-7"
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
            <NumberInput
              required
              v-model="people"
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
            <Button type="submit" class="bg-emerald-600 !px-10 text-white">
              Search
            </Button>
          </div>
        </form>
      </div>
      <div class="ml-[50%] flex justify-center">
        <Dropdown
          @change="changeSelectedOption"
          :slim="true"
          :selectedValue="direction"
          class="-mb-2.5"
          :values="sortingOption"
        />
      </div>
    </div>
    <br />

    <div class="mx-auto max-w-5xl bg-white sm:rounded-md">
      <h2 class="text-xl font-medium">
        {{ totalResults }}
        {{ totalResults === 1 ? 'Result' : 'Results' }}
        <span v-if="results.length != 0" class="font-normal">
          with average rating of
          <strong>{{ averageRating?.toPrecision(2) }}</strong>
        </span>
        <div v-if="results.length === 0" class="mt-3 text-base font-normal">
          Unfortunatelly, there are no available
          {{ $route.params.type }}. Please change the search parameters.
        </div>
      </h2>
      <br />
      <div role="list" class="space-y-6">
        <div
          v-for="result in results"
          :key="result.id"
          class="h-[13rem] max-h-[11rem] rounded-xl border border-neutral-300"
        >
          <div class="flex items-center px-4 py-4 sm:px-6">
            <div class="min-2-0 flex flex-1">
              <div class="relative h-36 w-48">
                <h4
                  class="absolute top-2 right-1.5 rounded-xl bg-emerald-50 px-2.5 text-[13px] font-semibold tracking-tight text-emerald-300"
                >
                  {{ result.rating?.toPrecision(2) ?? 0 }}
                </h4>
                <img
                  alt=""
                  :src="result.image"
                  class="h-full w-full cursor-pointer rounded-xl object-cover"
                  @click="showElement(result.id)"
                  :class="{ 'ring-opacity-100': result.selected }"
                />
              </div>
              <div class="ml-3 flex-1 px-4">
                <div class="flex w-full justify-between">
                  <div class="mb-20 content-start">
                    <div>
                      <h1
                        @click="showElement(result.id)"
                        class="cursor-pointer text-[20px] font-semibold hover:text-emerald-500"
                      >
                        {{ result.name }}
                      </h1>
                      <h2 class="text-[16px] text-gray-500">
                        {{ result.address?.city }},
                        {{ result.address?.country }}
                      </h2>
                      <h2 class="text-[16px] text-gray-500">
                        {{ result.address?.street }}
                        {{ result.address?.apartment }}
                      </h2>
                    </div>
                    <div class="max-w-md p-1">
                      <p
                        class="max-h-[3.7rem] overflow-hidden text-ellipsis text-[14px]"
                      >
                        {{ result.description }}
                      </p>
                    </div>
                  </div>
                  <div class="mt-4 w-[7rem] items-end space-x-2">
                    <div class="justify-left flex space-x-1 text-neutral-500">
                      <h4 class="text-[13px]">People: {{ result.people }}</h4>
                      <UserIcon
                        stroke-width="2"
                        class="h-4 w-4 pb-px text-emerald-500"
                      />
                    </div>
                  </div>
                </div>
              </div>
              <div class="w-[13%] space-y-2 pt-1.5">
                <div class="flex items-center justify-center space-x-1 pt-1.5">
                  <h3
                    class="text-lg font-semibold tracking-tight text-emerald-400"
                  >
                    {{ result.price.symbol }} {{ result.price.amount }}
                  </h3>
                  <span class="font text-sm text-gray-600">/night</span>
                </div>
                <div class="flex items-center justify-center space-x-1">
                  <h2 class="text-[11px]">Cancellation fee</h2>
                  <h2 class="text-[10px] font-bold">
                    {{ result.price.symbol }} {{ result.cancellationFee }}
                  </h2>
                </div>
                <Menu
                  @selected="e => optionSelected(e)"
                  :values="result.values"
                />
              </div>
            </div>
          </div>
        </div>
      </div>
      <div
        v-if="results.length"
        class="mt-10 flex justify-between space-x-10 text-sm"
      >
        <p>
          Showing <span class="font-medium"> {{ resultsFrom }}</span> to
          <span class="font-medium"> {{ resultsTo }} </span> of
          <span class="font-medium"> {{ totalResults }} </span> results
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
  </div>
  <DeleteBusinessModal
    :isOpen="isDeleteModalOpen"
    :businessId="entityId"
    type="cabins"
    :back="false"
    @modalClosed="isDeleteModalOpen = false"
    @elementDeleted="e => reloadList(e)"
  />
</template>

<script setup>
import { shallowRef, ref, onMounted, computed, watchEffect } from 'vue'
import Button from '../components/ui/Button.vue'
import Dropdown from '../components/ui/Dropdown.vue'
import NumberInput from '../components/ui/NumberInput.vue'
import CabinPreviewItem from '../components/search/CabinPreviewItem.vue'
import DeleteBusinessModal from '@/components/business/delete/DeleteBusinessModal.vue'
import Menu from '@/components/ui/Menu.vue'
import {
  UserIcon,
  BedIcon,
  HotelServiceIcon,
  MapPinIcon,
  UsersIcon,
  TrashIcon,
  CalendarIcon,
  PencilIcon,
  ArrowLeftIcon,
  ArrowRightIcon
} from 'vue-tabler-icons'
import { useRoute, useRouter, RouterLink } from 'vue-router'
import api from '../api/api'
import Input from '../components/ui/Input.vue'
import { user } from '@/stores/userStore'

const route = useRoute()
const router = useRouter()
const isDeleteModalOpen = ref(false)
const entityId = ref()

const city = ref(route.query.city != undefined ? route.query.city : '')
const country = ref(route.query.country != undefined ? route.query.country : '')
const cabinsName = ref(route.query.name)
const people = ref(Number(route.query.people ?? 0))

const reloadList = id => {
  isDeleteModalOpen.value = false
  search()
}

const userTypes = {
  'Cabin Owner': ['cabins', 'cabin-owner', 'owners cabins', 'cabin-profile'],
  'Boat Owner': ['boats', 'boat-owner', 'owners boats', 'boat-profile'],
  Fisher: [
    'adventures',
    'adventure-owner',
    'owners adventures',
    'adventure-profile'
  ]
}

const userType = userTypes[user.roles[0]]

const userTypeRef = ref(userType)

const sortingOption = [
  {
    name: 'Price - Highes first',
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
const averageRating = ref(0)
const hasNext = computed(() => currentPage.value < totalPages.value)
const hasPrevious = computed(
  () => currentPage.value >= totalPages.value && totalPages.value > 1
)
const resultsFrom = computed(() => 1 + 6 * (currentPage.value - 1))
const resultsTo = computed(
  () => 6 * (currentPage.value - 1) + results.value.length
)

const deleteBusiness = () => {
  isDeleteModalOpen.value = true
}

const currentLocation = ref(`${city.value} ${country.value}`)
const newLocation = ref(currentLocation.value)

const direction = ref(
  sortingOption.find(option => option.value === route.query.direction) ??
    sortingOption[0]
)

const results = ref([])

const fetchResults = async () => {
  const [data, error] = await api.business.getBusinesses(
    userType[0],
    {
      ...route.query
    },
    route.query.page - 1 || 0
  )
  if (!error) {
    averageRating.value = data.averageRating
    totalResults.value = data.totalResults
    totalPages.value = Math.ceil(totalResults.value / 6)
    data?.results.forEach(b => {
      b.values = [
        {
          name: 'Calendar',
          value: `/${userType[3]}/${b.id}/calendar`,
          icon: shallowRef(CalendarIcon)
        },
        { name: 'Edit', value: ['edit', b.id], icon: shallowRef(PencilIcon) },
        { name: 'Delete', value: ['delete', b], icon: shallowRef(TrashIcon) }
      ]
    })
    results.value = data.results
  }
}

watchEffect(() => fetchResults())

const showElement = id => {
  router.push(`/${userType[3]}/` + id)
}

const search = () => {
  if (newLocation.value == '') {
    country.value = ''
    city.value = ''
  }
  console.log(newLocation)
  router.push({
    name: `${userType[2]}`,
    query: {
      country: country.value,
      city: city.value,
      name: cabinsName.value,
      people: people.value,
      direction: direction.value.value,
      page: currentPage.value - 1
    }
  })
  setTimeout(() => {
    fetchResults()
  }, 100)
}

const optionSelected = value => {
  if (value[0] == 'delete') {
    entityId.value = value[1].id
    deleteBusiness()
  } else if (value[0] == 'edit') {
    router.push({
      name: 'create-update-business',
      params: { type: `${userType[0]}`, action: 'update' },
      query: { id: value[1] }
    })
  } else {
    router.push(value)
  }
}

const changeSelectedOption = value => {
  direction.value = value
  search()
}

const nextPage = () => {
  currentPage.value++
  search()
}

const previousPage = () => {
  currentPage.value--
  search()
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
  mapsScript.onload = () => createAutocompleteInput()
  document.head.appendChild(mapsScript)
} else {
  onMounted(() => createAutocompleteInput())
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
      (searchRes, status) => {
        if (status != window.google.maps.GeocoderStatus.OK) return
        const address = getAddress(searchRes)
        city.value = address.city
        country.value = address.country
        newLocation.value = `${city.value}, ${country.value}`
      }
    )
  })
}
const getAddress = searchRes => {
  const address = searchRes.find(r =>
    r.address_components.find(c => c.types.includes('postal_code'))
  )
  return {
    postalCode: extractFromAddress(address, 'postal_code'),
    country: extractFromAddress(searchRes[0], 'country'),
    city: extractFromAddress(searchRes[0], 'locality'),
    street: extractFromAddress(searchRes[0], 'route'),
    apartment: extractFromAddress(searchRes[0], 'street_number'),
    region: extractFromAddress(searchRes[0], 'administrative_area_level_1'),
    latitude: searchRes[0].geometry.location.lat(),
    longitude: searchRes[0].geometry.location.lng()
  }
}
const extractFromAddress = (address, key) => {
  return address?.address_components?.find(a => a.types.includes(key))
    ?.long_name
}
</script>
