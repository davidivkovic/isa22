<template>
  <BusinessProfileView v-if="cabin" :entity="cabin" entityType="cabins">
    ></BusinessProfileView
  >
</template>

<script setup>
import { ref } from 'vue'
import api from '@/api/api.js'
import BusinessProfileView from './BusinessProfileView.vue'
import { parseISO } from 'date-fns'
import { useRoute } from 'vue-router'

const cabin = ref()
const route = useRoute()

const start = route.query.start ? parseISO(route.query.start) : null
const end = route.query.start ? parseISO(route.query.end) : null

const fetchCabin = async () => {
  const [data] = await api.business.get(route.params.id, 'cabins', {
    start,
    end
  })
  if (data) cabin.value = data
}

fetchCabin()
</script>
