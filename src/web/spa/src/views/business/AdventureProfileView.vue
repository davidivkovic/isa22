<template>
  <BusinessProfileView
    v-if="adventure"
    :entity="adventure"
    entityType="adventures"
  >
    ></BusinessProfileView
  >
</template>

<script setup>
import { ref } from 'vue'
import api from '@/api/api.js'
import BusinessProfileView from './BusinessProfileView.vue'
import { parseISO } from 'date-fns'

import { useRoute } from 'vue-router'

const adventure = ref()
const route = useRoute()

const start = route.query.start ? parseISO(route.query.start) : null
const end = route.query.start ? parseISO(route.query.end) : null

const fetchAdventure = async () => {
  const [data] = await api.business.get(route.params.id, 'adventures', {
    start,
    end
  })
  if (data) adventure.value = data
}

fetchAdventure()
</script>
