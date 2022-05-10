<template>
  <BusinessProfileView :entity="boat"> ></BusinessProfileView>
</template>
<script setup>
import { ref } from 'vue'
import api from '@/api/api'
import { useRoute } from 'vue-router'
import BusinessProfileView from './BusinessProfileView.vue'

const boat = ref()
const route = useRoute()

const fetchCabin = async () => {
  const [data] = await api.business.get(route.params.id, 'boat')
  if (data) boat.value = data
}

fetchCabin()
</script>
