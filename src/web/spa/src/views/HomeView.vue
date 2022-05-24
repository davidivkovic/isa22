<template>
  <div v-if="isCustomer" class="h-full">
    <div
      :key="selectedImage"
      class="bg-image relative mx-auto h-[80%] overflow-clip rounded-2xl lg:w-4/5"
      :style="{ backgroundImage: `url(${selectedImage})` }"
    >
      <div
        v-once
        class="mx-auto flex h-full max-w-4xl flex-col justify-center pb-24 2xl:max-w-5xl 2xl:pb-52"
      >
        <h1 class="w-full text-8xl font-bold text-white">
          Air, sleep, <br />dream
        </h1>
        <h2 class="mt-2 max-w-[320px] text-lg text-white">
          Choose one of many great experiences to go on with your friends.
        </h2>
        <Button
          @click="startSearch()"
          class="mt-5 flex w-fit items-center space-x-2 bg-emerald-600 pl-6 pr-4 text-white"
        >
          <div>Start your search</div>
          <ArrowRightIcon stroke-width="1.25" />
        </Button>
      </div>
    </div>
    <SearchBar class="relative -mt-20" />
  </div>
  <OwnerDashboard v-else />
</template>

<script setup>
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import Button from '@/components/ui/Button.vue'
import SearchBar from '@/components/homepage/SearchBar.vue'
import { ArrowRightIcon } from 'vue-tabler-icons'
import OwnerDashboard from '@/components/homepage/OwnerDashboard.vue'
import { isCustomer } from '@/stores/userStore'

import cabinsURL from '@/assets/images/cabins.png'
import boatsURL from '@/assets/images/boats.png'
import fishingURL from '@/assets/images/fishing.png'

const defaultImage = cabinsURL

const images = {
  cabins: cabinsURL,
  boats: boatsURL,
  adventures: fishingURL
}

const route = computed(() => useRoute()?.name)
const pacStyle = computed(() => (route.value in images ? '-230px' : '0px'))

const selectedImage = computed(() => {
  if (route.value in images) return images[route.value]
  else if (selectedImage.value) return selectedImage.value
  return defaultImage
})

const startSearch = () => {
  document.getElementById('Location').focus()
}
</script>

<style>
.bg-image {
  background-size: cover;
  background-position: left;
}
.pac-container {
  margin-top: v-bind(pacStyle) !important;
  height: 200px;
}
</style>
