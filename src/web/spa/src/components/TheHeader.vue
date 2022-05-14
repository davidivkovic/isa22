<template>
  <div class="w-full">
    <div class="xl mx-auto max-w-4.5xl">
      <div class="flex items-center justify-between py-6">
        <div>
          <div class="flex items-center space-x-8">
            <HeaderNavigation
              :isActive="selectedTab === 0"
              name="Cabins"
              routeName="cabins"
            >
              <TreesIcon class="-mt-1 h-6 w-6" />
            </HeaderNavigation>
            <HeaderNavigation
              :isActive="selectedTab === 1"
              name="Boats"
              routeName="boats"
            >
              <SailboatIcon class="h-6 w-6" />
            </HeaderNavigation>
            <HeaderNavigation
              :isActive="selectedTab === 2"
              name="Adventures"
              routeName="adventures"
            >
              <FishIcon class="h-6 w-6" />
            </HeaderNavigation>
          </div>
          <div
            class="h-1 rounded-3xl bg-emerald-600 transition duration-200 ease-in-out"
            :class="translation"
          ></div>
        </div>
        <div class="-ml-12 text-2xl font-medium tracking-tight">
          adventure.com
        </div>
        <div class="flex items-center space-x-3">
          <RouterLink :to="`/business/${businessName}/create`">
            <Button class="bg-emerald-600 text-white">
              List your business
            </Button>
          </RouterLink>
          <RouterLink v-if="!isAuthenticated" to="/signin">
            <Button class="border border-gray-300">Sign in</Button>
          </RouterLink>
          <Button v-else @click="signOut()" class="border border-gray-300"
            >Sign out</Button
          >
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute, RouterLink } from 'vue-router'
import HeaderNavigation from './HeaderNavigation.vue'
import { TreesIcon, SailboatIcon, FishIcon } from 'vue-tabler-icons'
import Button from './ui/Button.vue'
import api from '../api/api'

import { isAuthenticated } from '../stores/userStore.js'

const currentRoute = computed(() => useRoute().path)
const defaultTransition = 'translate-x-0'

const signOut = () => api.auth.signOut()

const selectedTab = computed(() => {
  if (currentRoute.value.includes('cabins')) return 0
  else if (currentRoute.value.includes('boats')) return 1
  else if (currentRoute.value.includes('adventures')) return 2
  else if (selectedTab.value) return selectedTab.value
  return 0
})

const businessName = computed(() => {
  return {
    0: 'cabins',
    1: 'boats',
    2: 'adventures'
  }[selectedTab.value]
})

const translation = computed(() => {
  if (selectedTab.value === 0) return 'translate-x-0 w-[5.1rem]'
  else if (selectedTab.value === 1) return 'translate-x-[105px] w-[5.1rem]'
  else if (selectedTab.value === 2) return 'translate-x-[212px] w-[7rem]'
  else if (translation.value) return translation.value
  return defaultTransition
})
</script>
