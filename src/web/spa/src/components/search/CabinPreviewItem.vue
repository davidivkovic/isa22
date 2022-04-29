<template>
  <ResultPreviewItem priceUnit="night" :result="props.result">
    <slot>
      <div class="flex items-center space-x-1 text-neutral-500">
        <HotelServiceIcon stroke-width="2" class="h-4 w-4 pb-px" />
        <h4 class="font- text-[13px]">{{ props.result.rooms }}</h4>
      </div>
      <div class="flex items-center space-x-1 text-neutral-500">
        <BedIcon stroke-width="2" class="h-4 w-4 pb-px" />
        <h4 class="font- text-[13px]">{{ props.result.beds }}</h4>
      </div>
      <div class="flex items-center space-x-1 text-neutral-500">
        <UserIcon stroke-width="2" class="h-4 w-4 pb-px" />
        <h4 class="font- text-[13px]">{{ props.result.people }}</h4>
      </div>
    </slot>
  </ResultPreviewItem>
</template>
<script setup>
import { UserIcon, BedIcon, HotelServiceIcon } from 'vue-tabler-icons'
import ResultPreviewItem from './ResultPreviewItem.vue'
const props = defineProps(['result'])
</script>
