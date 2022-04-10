<template>
  <div
    class="relative flex h-10 w-full items-center rounded-md border px-2.5 py-2.5 transition-all hover:border-neutral-500"
    :class="{
      'pointer-events-none text-neutral-300': $attrs.disabled == ''
    }"
  >
    <div class="absolute right-1.5 select-none">
      <ChevronUpIcon
        @click="
          $emit('update:modelValue', Math.min(modelValue + 1, $attrs.max))
        "
        class="h-4 w-4 cursor-pointer rounded hover:bg-gray-100"
      />
      <ChevronDownIcon
        @click="
          $emit('update:modelValue', Math.max(modelValue - 1, $attrs.min))
        "
        class="h-4 w-4 cursor-pointer rounded hover:bg-gray-100"
      />
    </div>
    <div class="select-none pr-5 text-sm">
      {{ modelValue }}
    </div>
  </div>
</template>

<script setup>
import { ChevronUpIcon, ChevronDownIcon } from 'vue-tabler-icons'
defineProps(['modelValue'])
defineEmits(['update:modelValue'])
</script>

<style>
input[type='number']::-webkit-outer-spin-button,
input[type='number']::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

input[type='number'] {
  -moz-appearance: textfield;
}
</style>
