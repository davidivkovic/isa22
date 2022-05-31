<template>
  <div v-if="!isNonSearchRoute">
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

  <Logo />
  <div v-if="isNonSearchRoute" class="flex space-x-12 pl-8">
    <RouterLink
      :to="{ name: 'profile' }"
      v-slot="{ isActive }"
      class="w-24 cursor-pointer space-y-1 text-center"
    >
      <div class="font-medium">Profile</div>
      <div v-if="isActive" class="h-1 w-full rounded-full bg-emerald-600"></div>
    </RouterLink>
    <RouterLink
      :to="{ name: 'reservations' }"
      v-slot="{ isActive }"
      class="w-24 cursor-pointer space-y-1 text-center"
    >
      <div class="font-medium">Reservations</div>
      <div v-if="isActive" class="h-1 w-full rounded-full bg-emerald-600"></div>
    </RouterLink>
  </div>
  <div class="flex items-center space-x-5">
    <div v-if="isAuthenticated" class="flex space-x-2">
      <CustomerProfileOptions />
    </div>
    <RouterLink v-else :to="'/signin'">
      <Button class="bg-emerald-600 text-white"> List your business </Button>
    </RouterLink>
    <SignInButton />
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute, RouterLink } from 'vue-router'
import HeaderNavigation from './HeaderNavigation.vue'
import { TreesIcon, SailboatIcon, FishIcon } from 'vue-tabler-icons'
import Logo from './Logo.vue'
import Button from '../ui/Button.vue'
import SignInButton from './SignInButton.vue'
import { isAuthenticated } from '@/stores/userStore'
import CustomerProfileOptions from './CustomerProfileOptions.vue'
import { nonSearchRoutes } from '@/router/index.js'

const currentRoute = computed(() => useRoute().path)
const currentRouteName = computed(() => useRoute().name)
const defaultTransition = 'translate-x-0'

const isNonSearchRoute = computed(() =>
  nonSearchRoutes.includes(currentRouteName.value)
)

const selectedTab = computed(() => {
  if (currentRoute.value.includes('cabins') || currentRouteName.value == 'home')
    return 0
  else if (currentRoute.value.includes('boats')) return 1
  else if (currentRoute.value.includes('adventures')) return 2
  else if (currentRoute.value.includes('profile')) return 3
  else if (selectedTab.value) return selectedTab.value
  return 0
})

const translation = computed(() => {
  if (selectedTab.value === 0) return 'translate-x-0 w-[5.1rem]'
  else if (selectedTab.value === 1) return 'translate-x-[105px] w-[5.1rem]'
  else if (selectedTab.value === 2) return 'translate-x-[212px] w-[7rem]'
  else if (translation.value) return translation.value
  return defaultTransition
})
</script>
