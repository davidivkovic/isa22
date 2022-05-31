<template>
  <Logo class="" />
  <div class="flex space-x-8">
    <div class="cursor-pointer space-y-1">
      <RouterLink :to="{ name: 'profile' }" v-slot="{ isActive }">
        <div>My profile</div>
        <div
          v-if="isActive"
          class="h-1 w-full rounded-full bg-emerald-600"
        ></div>
      </RouterLink>
    </div>
    <div class="cursor-pointer space-y-1">
      <RouterLink :to="`/my-${businessName}`" v-slot="{ isActive }">
        <div>My {{ businessName }}</div>
        <div
          v-if="isActive"
          class="h-1 w-full rounded-full bg-emerald-600"
        ></div>
      </RouterLink>
    </div>
    <div class="cursor-pointer space-y-1">
      <div>Income</div>
      <div v-if="isActive" class="h-1 w-full rounded-full bg-emerald-600"></div>
    </div>
    <div class="cursor-pointer space-y-1">
      <RouterLink :to="`/${businessName}-reservations`" v-slot="{ isActive }">
        <div>Resevations</div>
        <div v-if="isActive" class="h-1 w-full rounded-full bg-emerald-600" />
      </RouterLink>
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
import { user } from '@/stores/userStore'

const currentRoute = computed(() => useRoute().path)

const ownerType = computed(() => {
  return {
    'Cabin Owner': 'cabin-owner',
    'Boat Owner': 'boat-owner',
    Fisher: 'adventure-owner'
  }[user.roles[0]]
})

const businessName = computed(() => {
  return {
    'Cabin Owner': 'cabins',
    'Boat Owner': 'boats',
    Fisher: 'adventures'
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
