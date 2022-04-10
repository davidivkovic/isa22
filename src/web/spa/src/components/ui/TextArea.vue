<template>
  <textarea
    @input="e => onInput(e)"
    :value="props.modelValue"
    rows="3"
    class="mt-3 w-full resize-none rounded-lg border-gray-300 text-sm text-gray-600 focus:border-gray-500 focus:outline-none focus:outline-transparent focus:ring-0 focus:ring-transparent"
  ></textarea>
</template>
<script setup>
import { ref } from 'vue'
const inputValue = ref(props.modelValue)
const props = defineProps(['modelValue'])
const emit = defineEmits(['update:modelValue'])

const onInput = event => {
  inputValue.value = event.target.value
  emit('update:modelValue', inputValue.value)
}
</script>
