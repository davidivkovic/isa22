<template>
  <Logo class="" />
  <div class="flex space-x-8">
    <div class="cursor-pointer space-y-1">
      <div>My profile</div>
      <div
        v-if="selectedTab === 3"
        class="h-0.5 w-full rounded-full bg-emerald-600"
      ></div>
    </div>
    <div class="cursor-pointer space-y-1">
      <div>My {{ businessName }}</div>
      <div
        v-if="selectedTab === 4"
        class="h-0.5 w-full rounded-full bg-emerald-600"
      ></div>
    </div>
    <div class="cursor-pointer space-y-1">
      <div>Income</div>
      <div
        v-if="selectedTab === 5"
        class="h-0.5 w-full rounded-full bg-emerald-600"
      ></div>
    </div>
    <div class="cursor-pointer space-y-1">
      <div>Resevations</div>
      <div
        v-if="selectedTab === 5"
        class="h-0.5 w-full rounded-full bg-emerald-600"
      ></div>
    </div>
  </div>
  <div class="flex space-x-3">
    <RouterLink :to="`/business/${businessName}/create`">
      <Button class="bg-emerald-600 text-white"> List your business </Button>
    </RouterLink>
    <SignInButton />
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { RouterLink, useRoute } from 'vue-router'
import Logo from './Logo.vue'
import SignInButton from './SignInButton.vue'
import Button from '../ui/Button.vue'

const currentRoute = computed(() => useRoute().path)

const businessName = computed(() => {
  return {
    0: 'cabins',
    1: 'boats',
    2: 'adventures'
  }[selectedTab.value]
})

const selectedTab = computed(() => {
  if (currentRoute.value.includes('cabins')) return 0
  else if (currentRoute.value.includes('boats')) return 1
  else if (currentRoute.value.includes('adventures')) return 2
  else if (selectedTab.value) return selectedTab.value
  return 0
})
</script>
