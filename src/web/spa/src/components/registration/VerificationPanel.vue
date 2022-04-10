<template>
  <div class="space-y-7">
    <CheckIcon
      class="mx-auto h-28 w-28 rounded-full bg-emerald-100 stroke-emerald-500 p-5"
    />
    <h1 class="text-center text-lg">
      {{ panelText }}
    </h1>
  </div>
</template>

<script setup>
import { CheckIcon } from 'vue-tabler-icons'
const panelText =
  'A registration request successfully sent. You will be notified by email if we accept your registration.'
</script>
