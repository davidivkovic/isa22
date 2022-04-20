<template>
  <form
    name="pricing-form"
    id="pricing-form"
    @submit.prevent="onSubmit()"
    class="w-96"
  >
    <div class="justify-betwen flex items-start space-x-7">
      <div>
        <h3 class="text-lg font-medium">Services</h3>
        <h5 class="mb-5 whitespace-nowrap text-neutral-500">
          List additional services you are offering
        </h5>
      </div>
      <div class="-mt-3">
        <label
          class="text-[15px] font-medium tracking-tight text-neutral-700"
          for="currency"
        >
          Currency
        </label>
        <select
          v-model="currency"
          id="currency"
          name="currency"
          class="mt-1 h-12 w-full cursor-pointer rounded-md border border-neutral-300 bg-transparent p-2.5 text-sm text-neutral-700 focus:border-neutral-500 focus:ring-0"
        >
          <option>USD</option>
          <option>EUR</option>
          <option>RSD</option>
        </select>
      </div>
    </div>

    <div class="mb-4 flex items-end space-x-2">
      <div class="flex-1">
        <Input
          v-model="service"
          class="h-12 w-full"
          label="Name"
          placeholder="Name of the item"
        />
      </div>
      <div>
        <Input
          v-model="servicePrice"
          class="h-12 w-32 !pl-7 !pr-2.5"
          :class="{ '!pl-10': currency == 'RSD' }"
          label="Price"
          min="0"
          type="number"
          name="price"
          id="price"
          placeholder="0.00"
          step=".01"
        >
          <template #prepend="{ focused, hovered }">
            <div class="pointer-events-none flex items-center pl-3">
              <span
                :class="[
                  'mt-0.5 text-[15px]',
                  focused || hovered ? 'text-neutral-600' : 'text-neutral-400'
                ]"
              >
                {{ symbols[currency] }}
              </span>
            </div>
          </template>
        </Input>
      </div>
      <Button
        :disabled="!serviceValid"
        @click="addService()"
        class="h-12 rounded-md border border-neutral-300 hover:bg-neutral-50 disabled:text-neutral-400 disabled:hover:bg-white"
      >
        Add
      </Button>
    </div>
    <p class="my-3 text-[13px] text-neutral-500">
      Tip: Remove services by clicking on them
    </p>
    <div
      v-for="service in services"
      :key="service.name"
      @click="removeService(service.name)"
      class="flex cursor-pointer items-end justify-between"
    >
      <p class="whitespace-nowrap text-[15px]">{{ service.name }}</p>
      <div
        class="border-top mx-2 mb-1.5 basis-10/12 border border-b-0 border-dashed border-neutral-300"
      ></div>
      <p class="font-semibold text-emerald-500">
        {{ symbols[service.price.currency] }}{{ service.price.amount }}
      </p>
    </div>

    <h3 class="mt-6 text-lg font-medium">Pricing</h3>
    <h5 class="mb-4 text-neutral-500">
      How much do you want to charge per hour?
    </h5>
    <div>
      <div class="flex w-full space-x-4">
        <div class="basis-32">
          <Input
            required
            min="0"
            max="1000000"
            v-model="unitPrice"
            class="h-12 w-full !pl-7 !pr-2.5"
            :class="{ '!pl-10': currency == 'RSD' }"
            label="Price each guest pays"
            type="number"
            name="price"
            id="price"
            placeholder="0.00"
            step="0.01"
          >
            <template #prepend="{ focused, hovered }">
              <div class="pointer-events-none flex items-center pl-3">
                <span
                  :class="[
                    'mt-0.5 text-[15px]',
                    focused || hovered ? 'text-neutral-600' : 'text-neutral-400'
                  ]"
                >
                  {{ symbols[currency] }}
                </span>
              </div>
            </template>
            <template #append> </template>
          </Input>
        </div>
        <div class="w-28">
          <Input
            required
            v-model="cancellationFee"
            type="number"
            placeholder="0.00"
            step="0.01"
            min="0"
            max="100"
            label="Cancellation Fee"
            class="h-12 w-full"
          >
            <template #append="{ focused, hovered }">
              <span
                class="px-3"
                :class="[
                  focused || hovered ? 'text-neutral-600' : 'text-neutral-400'
                ]"
                >%</span
              >
            </template>
          </Input>
        </div>
      </div>

      <div v-if="unitPrice != ''" class="mt-3">
        <p class="whitespace-nowrap text-[13px] text-neutral-500">
          20% Adventure.com commission
        </p>
        <p class="whitespace-nowrap">
          <span class="font-semibold text-emerald-500">
            {{ symbols[currency] }}{{ priceAfterTax }}
          </span>
          Your earnings (including taxes)
        </p>
      </div>
    </div>
  </form>
</template>

<script setup>
import { ref, computed } from 'vue'

import Input from '@/components/ui/Input.vue'
import Button from '@/components/ui/Button.vue'

const props = defineProps(['services', 'pricePerUnit', 'cancellationFee'])
const emit = defineEmits(['change'])

const symbols = {
  USD: '$',
  EUR: '€',
  RSD: 'din '
}

const service = ref('')
const servicePrice = ref('')
const unitPrice = ref(props.pricePerUnit?.amount ?? '')
const currency = ref(props.pricePerUnit?.currency ?? 'USD')
const cancellationFee = ref(props.cancellationFee ?? '')
const services = ref(props.services ?? [])

const priceAfterTax = computed(() => (Number(unitPrice.value) * 0.8).toFixed(2))

const serviceValid = computed(() => {
  return (
    !services.value.find(
      s => s.name.toLowerCase() == service.value.toLowerCase()
    ) &&
    service.value != '' &&
    servicePrice.value != '' &&
    Number(servicePrice.value) > 0
  )
})

const addService = () => {
  const newService = {
    name: service.value,
    price: {
      amount: Number(servicePrice.value),
      currency: currency.value
    }
  }
  services.value.push(newService)
  service.value = ''
  servicePrice.value = ''
}

const removeService = serviceName => {
  services.value = services.value.filter(r => r.name != serviceName)
}

const onSubmit = () =>
  emit('change', {
    services: services.value,
    pricePerUnit: {
      amount: unitPrice.value,
      currency: currency.value
    },
    cancellationFee: cancellationFee.value
  })

const getValues = () => document.getElementById('pricing-form').requestSubmit()

defineExpose({
  getValues
})
</script>

<script>
export default {
  inheritAttrs: false
}
</script>
