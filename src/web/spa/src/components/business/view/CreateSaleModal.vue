<template>
  <Modal
    :isOpen="isOpen"
    :light="true"
    @modalClosed="$emit('modalClosed')"
    class="w-[550px] bg-white text-left text-sm"
  >
    <form
      ref="saleForm"
      @submit.prevent="previewSale()"
      class="space-y-3.5 py-8 px-10"
    >
      <h1 v-if="isRenewal" class="text-center text-2xl font-medium">
        Renew reservation
      </h1>
      <h1 v-else class="text-center text-2xl font-medium">Create a new sale</h1>
      <p class="text-sm text-gray-500">
        1. Select starting and ending date of
        {{ isRenewal ? 'the reservation' : 'your sale' }}.
      </p>
      <div class="!mt-2 flex">
        <div class="w-72 space-y-1">
          <label for="">Start</label>
          <DateInput
            @change="value => (start = value)"
            placeholder="Start date"
            :lowerLimit="format(Date.now(), dateFormat)"
            required
            :hasTime="businessType !== 'cabins'"
            type="datetime-local"
            class="h-12 w-full pl-8"
          />
        </div>
        <div class="w-72 space-y-1">
          <label for="">End</label>

          <DateInput
            :lowerLimit="start"
            placeholder="End date"
            @change="value => (end = value)"
            required
            :hasTime="businessType !== 'cabins'"
            type="datetime-local"
            class="h-12 w-full pl-8"
          />
        </div>
      </div>
      <p class="text-sm text-gray-500">
        2. Select how many people are allowed.
      </p>
      <NumberInput
        min="1"
        required
        v-model="people"
        label="People"
        class="h-12 w-32"
      />
      <div class="space-y-1">
        <p class="text-sm text-gray-500">
          3. Select additional services you wish to include in your offer.
        </p>
        <div class="w-full">
          <div
            v-for="service in selectedServices"
            :key="service.name"
            @click="service.selected = !service.selected"
            class="mt-3 flex cursor-pointer items-end justify-between"
          >
            <Checkbox v-model="service.selected" class="!-mt-6 !mr-2" />
            <p class="whitespace-nowrap text-[15px]">{{ service.name }}</p>
            <div
              class="border-top mx-2 mb-1.5 basis-10/12 border border-b-0 border-dashed border-neutral-300"
            ></div>
            <p class="font-semibold text-emerald-500">
              {{ symbols[service.price.currency]
              }}{{ service.price.amount.toFixed(2) }}
            </p>
          </div>
        </div>
      </div>
      <p class="text-sm text-gray-500">
        4. Select how much your customers are going to save with this
        {{ isRenewal ? 'reservation' : 'sale' }}.
      </p>
      <NumberInput
        required
        v-model="discount"
        min="0"
        max="100"
        label="Discount percentage (%)"
        class="h-12 w-40"
      />
      <div class="text-base">
        <div class="flex justify-between">
          <p>Price per unit:</p>
          <p v-if="previewData">
            {{ previewData.price.symbol }}{{ pricePerUnit.toFixed(2) }}
          </p>
          <p v-else>-</p>
        </div>
        <div class="flex justify-between">
          <p>Starting price:</p>
          <p v-if="previewData">
            {{ previewData.price.symbol }}{{ startingPrice }}
          </p>
          <p v-else>-</p>
        </div>
        <div class="flex justify-between pb-1">
          <p>Price with discount:</p>
          <p v-if="previewData">
            {{ previewData.price.symbol
            }}{{ previewData.price.amount.toFixed(2) }}
          </p>
          <p v-else>-</p>
        </div>
        <div class="border-t pt-2 text-lg font-bold">
          <p class="text-sm font-normal text-gray-500">
            20% Adventure.com commission
          </p>
          <div class="flex justify-between">
            <p>Your earnings:</p>
            <p v-if="previewData">
              {{ previewData.price.symbol
              }}{{
                (
                  (previewData.price.amount.toFixed(2) *
                    (100 - previewData.taxPercentage)) /
                  100
                ).toFixed(2)
              }}
            </p>
            <p v-else>-</p>
          </div>
        </div>
      </div>
      <div class="text-center text-red-500">{{ previewError }}</div>
      <div class="mx-auto !mt-1 w-1/3">
        <Button
          :disabled="previewError != '' && !previewData"
          class="mx-auto !mt-5 w-full bg-emerald-600 text-white"
          @click="createSale()"
          >Create</Button
        >
      </div>
    </form>
  </Modal>
</template>

<script setup>
import { ref, watch } from 'vue'
import Modal from '@/components/ui/Modal.vue'
import NumberInput from '@/components/ui/NumberInput.vue'
import Checkbox from '@/components/ui/Checkbox.vue'
import Button from '@/components/ui/Button.vue'
import DateInput from '@/components/ui/DateInput.vue'
import api from '@/api/api.js'
import { debounce } from '@/components/utility/forms.js'
import { format } from 'date-fns'

const props = defineProps([
  'businessId',
  'isOpen',
  'services',
  'pricePerUnit',
  'businessType',
  'isRenewal',
  'customerId'
])
const emits = defineEmits(['success', 'modalClosed'])

const saleForm = ref()
const start = ref()
const end = ref()
const people = ref(1)
const discount = ref(0)
const selectedServices = ref(JSON.parse(JSON.stringify(props.services)))
const previewError = ref()
const previewData = ref()
const startingPrice = ref('-')

const dateFormat =
  props.businessType !== 'cabins' ? "yyyy-MM-dd'T'HH:mm" : 'yyyy-MM-dd'

const symbols = {
  USD: '$',
  EUR: '€',
  RSD: 'din '
}

const previewSale = async () => {
  if (start.value == null || end.value == null) return
  const [data, error] = await api.business.previewCreateSale(
    props.businessId,
    props.businessType,
    {
      start: start.value,
      end: end.value,
      people: people.value,
      services: selectedServices.value.filter(s => s.selected),
      discountPercentage: discount.value
    }
  )

  previewError.value = error
  previewData.value = data
  data &&
    (startingPrice.value = (
      (previewData.value.price.amount * 100) /
      (100 - discount.value)
    ).toFixed(2))
}

const createSale = async () => {
  const [data, error] = await api.business.createSale(
    props.businessId,
    props.businessType,
    {
      start: start.value,
      end: end.value,
      people: people.value,
      services: selectedServices.value.filter(s => s.selected),
      discountPercentage: discount.value,
      ...(props.isRenewal && { customerId: props.customerId })
    }
  )
  if (!error && data) {
    emits('modalClosed')
    window.location.reload()
  }
}

watch(
  [start, end, people, discount, selectedServices.value],
  debounce(() => {
    saleForm.value.requestSubmit()
    previewData.value = null
    previewError.value = null
  }, 200)
)
</script>
