<template>
  <div class="h-full">
    <div
      :key="selectedImage"
      class="bg-image relative mx-auto h-[75%] w-4/5 overflow-clip rounded-2xl"
      :style="{ backgroundImage: `url(${selectedImage})` }"
    >
      <div v-once class="mx-auto max-w-4xl pt-24">
        <h1 class="w-full text-8xl font-bold text-white">
          Air, sleep, <br />dream
        </h1>
        <h2 class="mt-2 max-w-[320px] text-lg text-white">
          Choose one of many great experiences to go on with your friends.
        </h2>
        <Button
          @click="startSearch()"
          class="mt-5 flex items-center space-x-2 bg-emerald-600 py-2.5 text-white"
        >
          <div>Start your search</div>
          <ArrowRightIcon stroke-width="1.25" />
        </Button>
      </div>
    </div>
    <SearchBar class="relative" />
  </div>
</template>

<script setup>
import { useRoute } from 'vue-router'
import { computed } from 'vue'
import Button from '@/components/ui/Button.vue'
import SearchBar from '@/components/homepage/SearchBar.vue'
import { ArrowRightIcon } from 'vue-tabler-icons'

import cabinsURL from '@/assets/images/cabins.png'
import boatsURL from '@/assets/images/boats.png'
import fishingURL from '@/assets/images/fishing.png'

const route = computed(() => useRoute().name)

const defaultImage =
  'https://images.unsplash.com/photo-1618221195710-dd6b41faaea6?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2900&q=80'

const selectedImage = computed(() => {
  if (route.value in images) return images[route.value]
  else if (selectedImage.value) return selectedImage.value
  return defaultImage
})
const images = {
  cabins: cabinsURL,
  boats: boatsURL,
  fishing: fishingURL
}
const startSearch = () => {
  document.getElementById('Location').focus()
}
</script>

<style>
.bg-image {
  background-size: cover;
  background-position: left;
}
</style>
