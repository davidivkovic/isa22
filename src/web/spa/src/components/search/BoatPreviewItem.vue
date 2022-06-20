<template>
  <ResultPreviewItem priceUnit="hour" :result="props.result">
    <div class="flex items-center space-x-1 text-neutral-500">
      <ArmchairIcon stroke-width="2" class="h-4 w-4 pb-px" />
      <h4 class="font- text-[13px]">
        {{ props.result.characteristics.seats }}
      </h4>
    </div>
    <div class="flex items-center space-x-1 text-neutral-500">
      <BoltIcon stroke-width="2" class="h-4 w-4 pb-px" />
      <h4 class="font- text-[13px]">
        {{ props.result.characteristics.bhp }} bhp
      </h4>
    </div>
    <div class="flex items-center space-x-1 text-neutral-500">
      <UserIcon stroke-width="2" class="h-4 w-4 pb-px" />
      <h4 class="font- text-[13px]">{{ props.result.people }}</h4>
    </div>
  </ResultPreviewItem>
</template>
<script setup>
import { BoltIcon, UserIcon, ArmchairIcon } from 'vue-tabler-icons'
import ResultPreviewItem from './ResultPreviewItem.vue'
const props = defineProps(['result'])
</script>
