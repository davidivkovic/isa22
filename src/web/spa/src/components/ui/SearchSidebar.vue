<template>
  <div
    class="flex h-[400px] w-[310px] flex-col justify-between rounded-md bg-amber-400 p-4"
  >
    <p class="text-xl font-medium text-gray-700">Search</p>
    <div class="!-mt-14 space-y-2">
      <Input
        class="h-9 w-full border-0 text-sm"
        type="datetime-local"
        label="Start"
      />
      <Input class="h-9 w-full border-0" type="datetime-local" label="End" />
      <Input
        class="h-9 w-full border-0"
        type="number"
        label="People number"
        placeholder="Number of people"
        min="1"
      />
    </div>
    <Button class="w-full bg-emerald-600 font-medium text-white">Search</Button>
  </div>
</template>
<script setup>
import Input from './Input.vue'
import Button from './Button.vue'
</script>
