<template>
  <label
    @focusin="showDatePicker()"
    :for="props.name"
    class="group relative cursor-pointer"
  >
    <input
      @change="formatInput()"
      required
      :id="props.name"
      ref="dateInput"
      :type="props.hasTime ? 'datetime-local' : 'date'"
      class="peer -z-10 ml-5 w-[210px] select-none border-0 py-0 text-[1.075rem] font-semibold text-white valid:translate-y-3 valid:text-black focus-within:translate-y-3 focus-within:text-black focus:outline-none focus:ring-0"
    />
    <div
      :v-if="props.hasTime"
      class="absolute top-2.5 ml-8 h-7 w-full bg-white text-[1.075rem] font-semibold text-black"
    >
      {{ dateValue }}
    </div>
    <div
      class="visited:peer absolute top-0 flex cursor-pointer items-center space-x-2 text-[22px] transition-all group-focus-within:-translate-y-4 group-focus-within:text-base peer-valid:-translate-y-4 peer-valid:text-base"
    >
      <div class="-mt-0.5 text-gray-400">
        <slot></slot>
      </div>
      <p class="items-center text-black" style="font-size: inherit">
        {{ props.name }}
      </p>
    </div>
    <div
      class="fixed mt-1 ml-8 whitespace-nowrap text-sm text-gray-400 transition-all group-focus-within:translate-y-2 peer-valid:translate-y-2"
    >
      <p class="">{{ props.description }}</p>
    </div>
  </label>
</template>

<script setup>
import { ref } from 'vue'
import { format, parseISO } from 'date-fns'
const props = defineProps({
  name: String,
  description: String,
  hasTime: Boolean
})
const dateValue = ref()
const dateInput = ref(null)

const formatInput = () => {
  const date = parseISO(dateInput.value.value)
  const dateFormat = props.hasTime ? 'EEE, MMM dd, HH:mm' : 'EEE, MMM dd'
  const newDate = format(date, dateFormat)
  dateValue.value = newDate
}
const showDatePicker = () => {
  dateInput.value.showPicker()
}
</script>

<style scoped="css">
input[type='datetime-local']::-webkit-calendar-picker-indicator,
input[type='date']::-webkit-calendar-picker-indicator {
  background: transparent;
  bottom: 0;
  color: transparent;
  cursor: pointer;
  height: auto;
  left: 0;
  position: absolute;
  right: 0;
  top: 0;
}
</style>
