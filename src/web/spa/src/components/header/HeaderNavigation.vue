<template>
  <RouterLink :to="{ name: props.routeName }">
    <div class="flex cursor-pointer space-x-1.5 py-2">
      <div :class="{ 'text-emerald-700': props.isActive }">
        <slot></slot>
      </div>
      <p
        class="text-[15px] font-medium"
        :class="{ 'text-emerald-600': props.isActive }"
      >
        {{ props.name }}
      </p>
    </div>
  </RouterLink>
</template>

<script setup>
import { RouterLink } from 'vue-router'
const props = defineProps({
  name: String,
  routeName: String,
  isActive: Boolean
})
</script>
