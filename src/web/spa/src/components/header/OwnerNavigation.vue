<template>
  <Logo class="" />
  <div class="flex space-x-8">
    <div class="relative cursor-pointer">
      <RouterLink :to="{ name: 'profile' }" v-slot="{ isActive }">
        <div :class="{ 'font-medium text-emerald-600': isActive }">Profile</div>
        <div
          v-if="isActive"
          class="absolute left-1/2 h-1 w-14 -translate-x-1/2 rounded-full bg-emerald-600"
        ></div>
      </RouterLink>
    </div>
    <div class="relative cursor-pointer">
      <RouterLink
        :to="`/my-${businessName.toLowerCase()}`"
        v-slot="{ isActive }"
      >
        <div :class="{ 'font-medium text-emerald-600': isActive }">
          {{ businessName }}
        </div>
        <div
          v-if="isActive"
          class="absolute left-1/2 h-1 w-14 -translate-x-1/2 rounded-full bg-emerald-600"
        ></div>
      </RouterLink>
    </div>
    <div class="relative cursor-pointer">
      <RouterLink
        :to="`/${businessName.toLowerCase()}-reservations`"
        v-slot="{ isActive }"
      >
        <div :class="{ 'font-medium text-emerald-600': isActive }">
          Resevations
        </div>
        <div
          v-if="isActive"
          class="absolute left-1/2 h-1 w-14 -translate-x-1/2 rounded-full bg-emerald-600"
        />
      </RouterLink>
    </div>
  </div>
  <div class="flex space-x-3">
    <RouterLink :to="`/business/${businessName.toLowerCase()}/create`">
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
import { user } from '@/stores/userStore'

const currentRoute = computed(() => useRoute().path)

const businessName = computed(() => {
  return {
    'Cabin Owner': 'Cabins',
    'Boat Owner': 'Boats',
    Fisher: 'Adventures'
  }[user.roles[0]]
})

const selectedTab = computed(() => {
  if (currentRoute.value.includes('cabins')) return 0
  else if (currentRoute.value.includes('boats')) return 1
  else if (currentRoute.value.includes('adventures')) return 2
  else if (selectedTab.value) return selectedTab.value
  return 0
})
</script>
