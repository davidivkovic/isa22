<template>
  <BusinessProfileView :entity="cabin"> ></BusinessProfileView>
</template>

<script setup>
import { ref } from 'vue'
import api from '../api/api'
import BusinessProfileView from './BusinessProfileView.vue'

import { useRoute } from 'vue-router'

const cabin = ref()
const route = useRoute()

const fetchCabin = async () => {
  const [data] = await api.business.get(route.params.id, 'cabin')
  if (data) cabin.value = data
}

fetchCabin()
</script>
