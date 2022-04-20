<template>
  <Input
    @mounted="saveInputReference"
    @focusin="showDatePicker()"
    @change="formatInput()"
    class="group relative"
    v-model="inputValue"
    clearable
  >
    <template #prepend="{ focused, hovered, input }">
      <div></div>
      <div
        @click="input.focus()"
        class="group absolute left-10 top-3.5 z-20 h-7 w-36 whitespace-nowrap bg-white text-sm"
        :class="textColor"
      >
        {{ dateValue }}
      </div>

      <CalendarIcon
        class="z-10 ml-2.5 mr-1.5 h-[18px] w-[18px] transition-all"
        :class="[focused || hovered ? 'text-neutral-700' : 'text-neutral-400']"
      /> </template
  ></Input>
</template>
<script setup>
import { ref, watchEffect, computed, onMounted } from 'vue'
import Input from './Input.vue'
import { parseISO, format } from 'date-fns'
import { CalendarIcon } from 'vue-tabler-icons'

const props = defineProps(['hasTime', 'placeholder', 'modelValue'])
const emit = defineEmits(['change'])
const inputRef = ref(null)
const dateValue = ref(props.placeholder)
const inputValue = ref(props.modelValue)
const textColor = computed(() =>
  dateValue.value === props.placeholder ? 'text-gray-400' : 'text-black'
)

const saveInputReference = input => {
  inputRef.value = input.value
  inputRef.value.type = props.hasTime ? 'datetime-local' : 'date'
}

onMounted(() => formatInput())

watchEffect(() => {
  if (!inputValue.value) dateValue.value = props.placeholder
})

const showDatePicker = () => inputRef.value.showPicker()
const formatInput = () => {
  const date = parseISO(inputValue.value)
  const dateFormat = props.hasTime ? 'EEE, MMM dd, HH:mm' : 'EEE, MMM dd'
  const newDate = format(date, dateFormat)
  dateValue.value = newDate
  emit('change', inputValue.value)
}
</script>
