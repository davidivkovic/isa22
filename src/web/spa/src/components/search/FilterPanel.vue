<template>
  <div class="-mt-20 h-full basis-[11.2rem] space-y-3">
    <p class="text-lg font-medium">Filter by</p>
    <div id="rating" class="space-y-1.5 py-3">
      <p class="font-medium">Rating</p>
      <CheckboxGroup
        :key="ratingHigher"
        @change="setRating"
        :items="ratingFilters"
        :selectedValue="
          ratingFilters.find(filter => filter.value == ratingHigher)
        "
      ></CheckboxGroup>
    </div>
    <div v-if="props.type === 'cabins'" id="rooms" class="space-y-1.5 py-3">
      <p class="font-medium">Room number</p>
      <CheckboxGroup
        :key="rooms"
        @change="setRoomNumber"
        :items="roomFilters"
        :selectedValue="roomFilters.find(filter => filter.value == rooms)"
      ></CheckboxGroup>
    </div>
    <div v-if="props.type === 'boats'" id="seats" class="space-y-1.5 py-3">
      <p class="font-medium">Seat number</p>
      <CheckboxGroup
        :key="seats"
        @change="setSeats"
        :items="seatFilters"
        :selectedValue="seatFilters.find(filter => filter.value == seats)"
      ></CheckboxGroup>
    </div>
    <div class="space-y-1.5 py-3">
      <p class="font-medium">Price (per person)</p>
      <CheckboxGroup
        :key="priceLow"
        @change="setPrice"
        :items="priceFilters"
        :selectedValue="
          priceFilters.find(
            filter =>
              filter.value.priceLow == priceLow &&
              filter.value.priceHigh == priceHigh
          )
        "
      ></CheckboxGroup>
    </div>
    <Button type="button" @click="resetFilters()" class="!px-0 font-normal"
      >Reset filters</Button
    >
    <Button
      @click="applyFilters()"
      class="w-full !border border-emerald-600 text-emerald-600"
      >Apply filters</Button
    >
  </div>
</template>
<script setup>
import { ref } from 'vue'
import CheckboxGroup from '../ui/CheckboxGroup.vue'
import Button from '../ui/Button.vue'
import { useRoute, useRouter } from 'vue-router'

const route = useRoute()
const router = useRouter()
const props = defineProps(['type'])

const ratingHigher = ref(route.query.ratingHigher)
const rooms = ref(route.query.rooms)
const priceLow = ref(route.query.priceLow)
const priceHigh = ref(route.query.priceHigh)
const seats = ref(route.query.seats)

const ratingFilters = [
  { label: 'Superb: 9+', id: 'rating9', value: 9 },
  { label: 'Very good: 8+', id: 'rating8', value: 8 },
  { label: 'Good: 7+', id: 'rating7', value: 7 },
  { label: 'Not bad 6+', id: 'rating6', value: 6 }
]
const roomFilters = [
  { label: '1 room', id: 'room1', value: 1 },
  {
    label: '2 rooms',
    id: 'room2',
    value: 2
  },
  {
    label: '3 rooms',
    id: 'room3',
    value: 3
  },
  {
    label: 'more',
    id: 'room_more',
    value: 4
  }
]
const seatFilters = [
  { label: '1 seat', id: 'seat1', value: 1 },
  {
    label: '2 seats',
    id: 'seat2',
    value: 2
  },
  {
    label: '3 seats',
    id: 'seat3',
    value: 3
  },
  {
    label: '4 seats',
    id: 'seat4',
    value: 4
  },
  {
    label: 'more',
    id: 'seat_more',
    value: 5
  }
]
const priceFilters = [
  {
    label: '$0-50 /unit',
    value: { priceLow: 0, priceHigh: 50 },
    id: 'price1'
  },
  {
    label: '$50-100 /unit',
    value: { priceLow: 50, priceHigh: 100 },
    id: 'price2'
  },
  {
    label: '$100-200 /unit',
    value: { priceLow: 100, priceHigh: 200 },
    id: 'price3'
  },
  { label: 'more', value: { priceLow: 200 }, id: 'price4' }
]

const setRating = newRating => (ratingHigher.value = newRating)
const setRoomNumber = newRoomNumber => (rooms.value = newRoomNumber)
const setPrice = newPrice => {
  priceLow.value = newPrice.priceLow
  priceHigh.value = newPrice.priceHigh
}
const setSeats = newSeatNumber => (seats.value = newSeatNumber)

const applyFilters = () => {
  const query = {
    ...route.query,
    ratingHigher: ratingHigher.value,
    rooms: rooms.value,
    priceLow: priceLow.value,
    priceHigh: priceHigh.value,
    seats: seats.value
  }
  let filtered = Object.fromEntries(
    // eslint-disable-next-line no-unused-vars
    Object.entries(query).filter(([_, v]) => v != '')
  )
  router.push({
    name: 'search',
    query: {
      ...filtered
    }
  })
}

const resetFilters = () => {
  ratingHigher.value = ''
  rooms.value = ''
  priceLow.value = ''
  priceHigh.value = ''
  seats.value = ''
  applyFilters()
}
</script>
