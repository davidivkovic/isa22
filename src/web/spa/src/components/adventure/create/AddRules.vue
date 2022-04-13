<template>
  <form
    id="add-rules-form"
    name="add-rules-form"
    @submit.prevent="onSubmit()"
    class="w-96"
  >
    <h3 class="text-lg font-medium">Set some rules</h3>
    <h5 class="text-neutral-500">Show your customers what they can do</h5>
    <div class="mt-5 flex items-end space-x-3">
      <Input
        maxlength="40"
        v-model="rule"
        label="New Rule"
        class="h-12 w-full"
        placeholder="Add a new rule"
      >
        <template #append>
          <Switch
            v-model="allowed"
            :class="[
              allowed ? 'bg-emerald-50' : 'bg-red-50',
              'relative mr-3.5 inline-flex h-6 w-11 flex-shrink-0 cursor-pointer rounded-full border-2 border-transparent transition-colors duration-200 ease-in-out  focus:outline-none'
            ]"
          >
            <span class="sr-only">Allowed</span>
            <span
              :class="[
                allowed ? 'translate-x-5' : 'translate-x-0',
                'pointer-events-none relative inline-block h-5 w-5 transform transition duration-200 ease-in-out'
              ]"
            >
              <CircleXIcon
                :class="[
                  allowed
                    ? 'opacity-0 duration-100 ease-out'
                    : 'opacity-100 duration-200 ease-in',
                  'absolute inset-0 flex h-full w-full items-center justify-center text-red-700 transition-opacity'
                ]"
                aria-hidden="true"
              >
              </CircleXIcon>
              <CircleCheckIcon
                :class="[
                  allowed
                    ? 'opacity-100 duration-200 ease-in'
                    : 'opacity-0 duration-100 ease-out',
                  'absolute inset-0 flex h-full w-full items-center justify-center text-emerald-700 transition-opacity'
                ]"
                aria-hidden="true"
              >
              </CircleCheckIcon>
            </span>
          </Switch>
        </template>
      </Input>
      <Button
        :disabled="!ruleValid"
        @click="addRule()"
        class="h-12 rounded-md border border-neutral-300 hover:bg-neutral-50 disabled:text-neutral-400 disabled:hover:bg-white"
      >
        Add
      </Button>
    </div>
    <p class="my-3 text-[13px] text-neutral-500">
      Tip: Remove rules by clicking on them
    </p>
    <div class="flex flex-wrap gap-3">
      <div
        v-for="rule in rules"
        :key="rule.name"
        @click="removeRule(rule.name)"
        class="flex cursor-pointer select-none items-center space-x-1 whitespace-nowrap rounded-full py-1 pl-1.5 pr-3"
        :class="[rule.allowed ? 'bg-emerald-50' : 'bg-red-50']"
      >
        <CircleCheckIcon v-if="rule.allowed" class="h-5 w-5 text-emerald-700" />
        <CircleXIcon v-else class="h-5 w-5 text-red-700" />
        <p
          class="text-[13px] font-medium tracking-tight"
          :class="[rule.allowed ? 'text-emerald-700' : 'text-red-700']"
        >
          {{ rule.name }}
        </p>
      </div>
    </div>

    <h3 class="mt-4 text-lg font-medium">Capacity</h3>
    <h5 class="mb-3 text-neutral-500">Tell us how many people are allowed</h5>
    <NumberInput
      required
      v-model="people"
      min="1"
      max="100"
      class="h-12 w-20 cursor-default"
    />
  </form>
</template>

<script setup>
import { ref, computed } from 'vue'
import { Switch } from '@headlessui/vue'
import { CircleCheckIcon, CircleXIcon } from 'vue-tabler-icons'

import Input from '@/components/ui/Input.vue'
import NumberInput from '@/components/ui/NumberInput.vue'
import Button from '@/components/ui/Button.vue'

const emit = defineEmits(['change'])

const people = ref(1)
const rule = ref('')
const allowed = ref(false)
const rules = ref([])

const ruleValid = computed(() => {
  return (
    !rules.value.find(r => r.name.toLowerCase() == rule.value.toLowerCase()) &&
    rule.value != ''
  )
})

const addRule = () => {
  const newRule = {
    name: rule.value,
    allowed: allowed.value
  }

  allowed.value ? rules.value.unshift(newRule) : rules.value.push(newRule)

  rule.value = ''
  allowed.value = false
}

const removeRule = ruleName => {
  rules.value = rules.value.filter(r => r.name != ruleName)
}

const onSubmit = () =>
  emit('change', {
    people: people.value,
    rules: rules.value
  })

const getValues = () =>
  document.getElementById('add-rules-form').requestSubmit()

defineExpose({
  getValues
})
</script>
