<template>
  <EntityProfileView :entity="cabin">
    <template v-slot:tag>
      <Tag>cabin</Tag>
    </template></EntityProfileView
  >
</template>

<script setup>
import EntityProfileView from './EntityProfileView.vue'
import Tag from '../components/ui/Tag.vue'

const cabin = {
  owner: {
    fullName: 'David Ivkovic'
  },
  name: 'Cabin Mona',
  description:
    'I’ve been fishing for sport since I was a small child. I can teach open faced reels, spinning tackle, fly fishing, and even spear fishing! I can also consult you on which equipment is right for specific species of fish, and what type of gear will give you the highest success rate.',
  address: 'Milana Pecinara 21, Zlatibor 31315 Serbia',
  images: [
    'https://images.unsplash.com/photo-1554995207-c18c203602cb?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2070&q=80',
    'https://images.unsplash.com/photo-1556912173-3bb406ef7e77?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2070&q=80',
    'https://images.unsplash.com/photo-1512918728675-ed5a9ecdebfd?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2070&q=80',
    'https://images.unsplash.com/photo-1512916958891-fcf61b2160df?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2971&q=80',
    'https://images.unsplash.com/photo-1557429287-b2e26467fc2b?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=1974&q=80',
    'https://images.unsplash.com/photo-1527359443443-84a48aec73d2?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2070&q=80',
    'https://images.unsplash.com/photo-1608658405784-4c3ef05ebcc3?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2070&q=80',
    'https://images.unsplash.com/photo-1507096098333-b3bf7a975088?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2076&q=80',
    'https://images.unsplash.com/photo-1507096098333-b3bf7a975088?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2076&q=80'
  ],
  rating: 9.8,
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
  cancellationFee: '$10'
}
</script>
