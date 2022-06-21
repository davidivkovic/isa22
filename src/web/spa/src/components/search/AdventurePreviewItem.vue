<template>
  <ResultPreviewItem priceUnit="hour" :result="props.result">
    <div class="flex items-center space-x-1 text-neutral-500">
      <UserIcon stroke-width="2" class="h-4 w-4 pb-px" />
      <h4 class="font- text-[13px]">{{ props.result.people }}</h4>
    </div>
  </ResultPreviewItem>
</template>
<script setup>
import { UserIcon } from 'vue-tabler-icons'
import ResultPreviewItem from './ResultPreviewItem.vue'
const props = defineProps(['result'])
</script>
