<template>
  <form
    @submit.prevent="search"
    class="z-10 mx-auto -mt-20 flex w-3/4 max-w-6xl justify-between rounded-2xl border border-gray-300 bg-white px-7 py-10 2xl:px-12"
  >
    <div id="navigation" class="flex justify-start space-x-3">
      <SearchBarLocationField
        @valueChanged="addValue"
        name="Location"
        description="Where are you going?"
      >
        <BrandTelegramIcon />
      </SearchBarLocationField>
      <SearchBarDateField
        @valueChanged="addValue"
        name="Start"
        description="When does your journey start?"
        :hasTime="hasTime"
        class="-ml-2"
        :upperLimit="values['end']"
      >
        <CalendarIcon />
      </SearchBarDateField>
      <SearchBarDateField
        @valueChanged="addValue"
        name="End"
        description="When does your journey end?"
        :hasTime="hasTime"
        :lowerLimit="values['start']"
      >
        <CalendarIcon />
      </SearchBarDateField>
      <SearchBarNumberField
        @valueChanged="addValue"
        name="People"
        description="How many?"
        inputType="number"
        class="!mr-12 w-20"
      >
        <UserIcon />
      </SearchBarNumberField>
    </div>
    <Button
      type="submit"
      class="flex h-12 w-12 flex-col items-center justify-center rounded-full bg-emerald-600"
    >
      <SearchIcon class="text-white" />
    </Button>
  </form>
</template>

<script setup>
import { ref, computed } from 'vue'
import SearchBarLocationField from './SearchBarLocationField.vue'
import SearchBarNumberField from './SearchBarNumberField.vue'
import SearchBarDateField from './SearchBarDateField.vue'
import Button from '../ui/Button.vue'
import { useRoute, useRouter } from 'vue-router'
import {
  CalendarIcon,
  SearchIcon,
  UserIcon,
  BrandTelegramIcon
} from 'vue-tabler-icons'

const route = useRoute()
const router = useRouter()
const values = ref({})
const hasTime = computed(() => route.name !== 'cabins' && route.path !== '/')

const addValue = v => (values.value = { ...values.value, ...v })

const search = () => {
  router.push({
    name: 'search',
    params: { type: route.name ?? 'cabins' },
    query: {
      ...values.value,
      direction: 'rating_desc'
    }
  })
}
</script>
