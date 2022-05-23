<template>
  <BusinessProfileView v-if="boat" :entity="boat" entityType="boats">
    ></BusinessProfileView
  >
</template>
<script setup>
import { ref } from 'vue'
import api from '@/api/api.js'
import { useRoute } from 'vue-router'
import BusinessProfileView from './BusinessProfileView.vue'
import { parseISO } from 'date-fns'

const boat = ref()
const route = useRoute()

const start = route.query.start ? parseISO(route.query.start) : null
const end = route.query.start ? parseISO(route.query.end) : null

const fetchBoat = async () => {
  const [data] = await api.business.get(route.params.id, 'boats', {
    start,
    end
  })
  if (data) boat.value = data
}

fetchBoat()
</script>
