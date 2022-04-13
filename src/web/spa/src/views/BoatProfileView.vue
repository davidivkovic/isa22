<template>
  <EntityProfileView :entity="boat">
    <template v-slot:tag>
      <Tag>boat</Tag>
    </template>
    <template v-slot:equipment>
      <h3 class="!mt-10 text-lg font-semibold">Equipment</h3>
      <div class="!mt-5 font-medium">Navigation equipment</div>
      <h3 v-if="boat.equipment.navigational.length !== 0">
        Some navigation equipment is included in the offer.
      </h3>
      <h3 v-else>No navigation equipment is included in this offer.</h3>
      <div class="mt-1 grid w-2/3 grid-cols-2 gap-y-1">
        <div
          v-for="equipment in boat.equipment.navigational"
          :key="equipment"
          class="flex w-fit items-center space-x-2"
        >
          <CheckIcon class="h-5 w-5 text-emerald-500" />
          <p>{{ equipment }}</p>
        </div>
      </div>
      <div class="!mt-5 font-medium">Fishing equipment</div>
      <h3 v-if="boat.equipment.fishing.length !== 0">
        Some navigation equipment is included in the offer.
      </h3>
      <h3 v-else>No fishing equipment is included in this offer.</h3>
      <div class="mt-1 grid w-2/3 grid-cols-2 gap-y-1">
        <div
          v-for="equipment in boat.equipment.fishing"
          :key="equipment"
          class="flex w-fit items-center space-x-2"
        >
          <CheckIcon class="h-5 w-5 text-emerald-500" />
          <p>{{ equipment }}</p>
        </div>
      </div>
    </template>
    <template v-slot:characteristics>
      <div class="border-t p-8">
        <h2 class="text-2xl font-semibold">Characteristics</h2>
      </div>
    </template>
  </EntityProfileView>
</template>
<script setup>
import EntityProfileView from './EntityProfileView.vue'
import Tag from '../components/ui/Tag.vue'
import { CheckIcon } from 'vue-tabler-icons'
const boat = {
  owner: {
    fullName: 'David Ivkovic'
  },
  name: 'David Boats',
  description:
    'I’ve been fishing for sport since I was a small child. I can teach open faced reels, spinning tackle, fly fishing, and even spear fishing! I can also consult you on which equipment is right for specific species of fish, and what type of gear will give you the highest success rate.',
  address: 'Milana Pecinara 21, Zlatibor 31315 Serbia',
  images: [
    'https://images.unsplash.com/photo-1476514525535-07fb3b4ae5f1?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2070&q=80',
    'https://images.unsplash.com/photo-1470770903676-69b98201ea1c?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2970&q=80',
    'https://images.unsplash.com/photo-1523496922380-91d5afba98a3?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2832&q=80',
    'https://images.unsplash.com/photo-1586699253884-e199770f63b9?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2070&q=80',
    'https://images.unsplash.com/photo-1447708754653-956cb5f39bac?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2308&q=80',
    'https://images.unsplash.com/photo-1543804491-66d81260fe90?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2070&q=80',
    'https://images.unsplash.com/photo-1463694372132-6c267f6ba561?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2071&q=80',
    'https://images.unsplash.com/photo-1507096098333-b3bf7a975088?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2076&q=80',
    'https://images.unsplash.com/photo-1507096098333-b3bf7a975088?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2076&q=80'
  ],
  rating: 9.5,
  services: [
    { name: 'Breakfast', price: '$7.00' },
    { name: 'Lunch', price: '$10.00' },
    { name: 'Dinner', price: '$9.00' },
    { name: 'WiFi', price: '$5.00' },
    { name: 'Towels', price: '$2.00' },
    { name: 'TV', price: '$5.00' }
  ],
  rules: [
    { name: 'Smoking', allowed: true },
    { name: 'Drugs', allowed: false },
    { name: 'Pets', allowed: true },
    { name: 'Shouting', allowed: false }
  ],
  numberOfReviews: 20,
  numberOfPeople: 4,
  pricePerUnit: '$15',
  cancellationFee: '$10',
  equipment: {
    navigational: [
      'Gyro Compas',
      'Magnetic Compas',
      'Radar',
      ' Automatic Tracking Aid'
    ],
    fishing: []
  },
  characteristics: {
    seats: 6,
    length: 6,
    engines: 2,
    BHP: 300,
    topSpeed: 100
  }
}
</script>
