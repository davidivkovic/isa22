<template>
  <!-- <Transition> -->
  <div class="h-full">
    <div
      :key="selectedImage"
      class="bg-image relative mx-auto h-[80%] w-4/5 overflow-clip rounded-2xl"
      :style="{ backgroundImage: `url(${selectedImage})` }"
    >
      <div v-once class="mx-auto max-w-4xl pt-24">
        <h1 class="w-full text-8xl font-bold text-white">
          Air, sleep, <br />dream
        </h1>
        <h2 class="mt-2 max-w-[320px] text-lg text-white">
          Choose one of many great experiences to go on with your friends.
        </h2>
        <Button class="mt-5 bg-emerald-600 py-2.5 text-white">
          Start your search
        </Button>
      </div>
    </div>
    <!-- </Transition> -->
    <SearchBar class="relative" />
  </div>
</template>

<script setup>
import { useRoute } from 'vue-router'
import { computed } from 'vue'
import Button from '../components/ui/Button.vue'
import SearchBar from '../components/SearchBar.vue'

const route = computed(() => useRoute().name)

const defaultImage =
  'https://images.unsplash.com/photo-1618221195710-dd6b41faaea6?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2900&q=80'

const selectedImage = computed(() => {
  if (route.value in images) return images[route.value]
  else if (selectedImage.value) return selectedImage.value
  return defaultImage
})
const images = {
  cabins:
    'https://images.unsplash.com/photo-1618221195710-dd6b41faaea6?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2900&q=80',
  boats:
    'https://images.unsplash.com/photo-1491800943052-1059ce1e1012?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=1974&q=80',
  fishing:
    'https://images.unsplash.com/photo-1502802316010-612622d21ca3?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2072&q=80'
}

// const image = ref('')

// const image = computed(() => {
//   return route.name in images
//     ? images[route.name]
//     : images[route.meta.selectedView]
// })
</script>

<style>
.bg-image {
  background-size: cover;
  background-position: left;
}
</style>
