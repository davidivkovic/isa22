<template>
  <div class="w-full">
    <div class="xl mx-auto max-w-4.5xl">
      <div class="flex items-center justify-between py-6">
        <CustomerNavigation v-if="isCustomer" />
        <AdminNavigation v-else-if="isAdmin" />
        <OwnerNavigation v-else />
      </div>
    </div>
  </div>
</template>

<script setup>
import OwnerNavigation from './OwnerNavigation.vue'
import CustomerNavigation from './CustomerNavigation.vue'
import AdminNavigation from './AdminNavigation.vue'
import { isCustomer, isAdmin } from '@/stores/userStore'
</script>
