<template>
  <div
    class="z-10 mx-auto -mt-16 flex h-32 w-full max-w-4.5xl items-center justify-end space-x-14 rounded-2xl border border-gray-300 bg-white px-6 py-5"
  >
    <div id="navigation" class="flex justify-start space-x-3">
      <SearchBarField
        name="Location"
        description="Where are you going?"
        inputType="text"
      >
        <BrandTelegramIcon />
      </SearchBarField>
      <SearchBarDateField
        name="Start"
        description="When does your journey start?"
        :hasTime="true"
        class="pl-2"
      >
        <CalendarIcon />
      </SearchBarDateField>
      <SearchBarDateField
        name="End"
        description="When does your journey end?"
        :hasTime="true"
      >
        <CalendarIcon />
      </SearchBarDateField>
      <SearchBarField
        name="People"
        description="How many?"
        inputType="number"
        class="w-20"
      >
        <UserIcon />
      </SearchBarField>
    </div>
    <Button
      @click="search()"
      class="flex h-12 w-12 flex-col items-center justify-center rounded-full bg-emerald-600"
    >
      <SearchIcon class="text-white" />
    </Button>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import SearchBarField from './SearchBarField.vue'
import SearchBarDateField from './SearchBarDateField.vue'
import Button from './ui/Button.vue'
import {
  CalendarIcon,
  SearchIcon,
  UserIcon,
  BrandTelegramIcon
} from 'vue-tabler-icons'

const location = ref()
const startDate = ref()
const endDate = ref()
const peopleCount = ref()

const search = () => {}
</script>
