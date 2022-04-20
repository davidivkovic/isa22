<template>
  <form
    @submit.prevent="search"
    class="z-10 mx-auto -mt-20 flex w-full max-w-4.5xl items-center justify-end space-x-14 rounded-2xl border border-gray-300 bg-white px-6 py-10"
  >
    <div id="navigation" class="flex justify-start space-x-3">
      <SearchBarLocationField
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
        class="pl-2"
        :upperLimit="dateValues['End']"
      >
        <CalendarIcon />
      </SearchBarDateField>
      <SearchBarDateField
        @valueChanged="addValue"
        name="End"
        description="When does your journey end?"
        :hasTime="hasTime"
        :lowerLimit="dateValues['Start']"
      >
        <CalendarIcon />
      </SearchBarDateField>
      <SearchBarNumberField
        name="People"
        description="How many?"
        inputType="number"
        class="!mr-3 w-20"
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
import { formData } from '../utility/forms.js'
import {
  CalendarIcon,
  SearchIcon,
  UserIcon,
  BrandTelegramIcon
} from 'vue-tabler-icons'

const route = useRoute()
const router = useRouter()

let dateValues = ref({})

const addValue = v => {
  console.log(dateValues.value['Start'])
  dateValues.value = { ...dateValues.value, ...v }
}

const search = e => {
  const data = formData(e)
  const [City, Country] = data['Location'].split(', ')
  router.push({
    name: 'search',
    query: { ...data, City, Country }
  })
}

const hasTime = computed(() => route.name !== 'cabins')
</script>
