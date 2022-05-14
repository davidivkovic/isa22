<template>
  <div>
    <div class="my-6 flex flex-col items-center justify-between">
      <div class="ml-2 grid auto-cols-fr grid-cols-3">
        <div class="ml-auto mr-8">
          <div v-for="(step, idx) in stepNames" :key="step">
            <h2
              class="flex text-lg font-medium"
              :class="{
                'text-neutral-300': idx != currentStep
              }"
            >
              <div
                class="mr-3.5 mt-0.5 flex aspect-square h-8 w-8 items-center justify-center rounded-full transition-all"
                :class="[
                  idx == currentStep
                    ? 'bg-emerald-600 text-white'
                    : 'border border-neutral-300 text-neutral-300',
                  idx < currentStep ? 'border border-emerald-600' : ''
                ]"
              >
                <CheckIcon
                  v-if="idx < currentStep"
                  class="h-5 w-5 text-emerald-600"
                />
                <div v-else>
                  {{ idx + 1 }}
                </div>
              </div>
              <div>
                {{ step }}
                <div
                  :class="{ 'text-neutral-500': idx == currentStep }"
                  class="text-sm"
                >
                  {{ stepDescriptions[idx] }}
                </div>
              </div>
            </h2>
            <div
              v-if="idx < steps.length - 1"
              class="-mt-2 mb-0.5 ml-[15.5px] h-7 w-px bg-neutral-300"
            ></div>
          </div>
        </div>
        <KeepAlive>
          <Component
            ref="currentComponent"
            @change="e => onChange(e)"
            :is="steps[currentStep]"
            v-bind="business"
            class="mt-0.5"
          />
        </KeepAlive>
      </div>
    </div>
    <div class="mx-auto flex w-96 items-end">
      <Button
        v-if="currentStep > 0"
        @click="back()"
        type="button"
        class="-ml-2.5 flex items-center space-x-1 !pr-5 transition hover:bg-gray-50"
      >
        <ArrowNarrowLeftIcon stroke-width="1.5" class="text-gray-800" />
        <div>Previous</div>
      </Button>
      <Button
        v-if="currentStep < steps.length - 1"
        @click="forward()"
        type="submit"
        class="ml-auto -mr-1 bg-emerald-600 !px-7 text-white transition hover:bg-emerald-700"
      >
        Next
      </Button>
      <Button
        v-if="currentStep == steps.length - 1"
        @click="forward()"
        :disabled="isLoading"
        type="submit"
        class="ml-auto -mr-1 bg-emerald-600 !px-12 text-white transition hover:bg-emerald-700 disabled:cursor-not-allowed disabled:hover:bg-emerald-600"
      >
        <div v-if="isLoading" class="flex items-center">
          <Loader class="mr-3" />
          Processing...
        </div>
        <div v-else>Finish</div>
      </Button>
    </div>
  </div>
</template>

<script setup>
import { ref, shallowRef } from 'vue'
import { ArrowNarrowLeftIcon, CheckIcon } from 'vue-tabler-icons'
import { useRouter, useRoute } from 'vue-router'
import api from '@/api/api.js'

import MainInfo from '@/components/business/create/MainInfo.vue'
import AddLocation from '@/components/business/create/AddLocation.vue'
import AddImages from '@/components/business/create/AddImages.vue'
import AddRules from '@/components/business/create/AddRules.vue'
import AddFishingEquipment from '@/components/business/create/AddFishingEquipment.vue'
import AddBoatEquipment from '@/components/business/create/AddBoatEquipment.vue'
import AddRooms from '@/components/business/create/AddRooms.vue'
import Pricing from '@/components/business/create/Pricing.vue'
import Button from '@/components/ui/Button.vue'
import Loader from '@/components/ui/Loader.vue'

const router = useRouter()
const route = useRoute()

const businessType = route.params.type
const action = route.params.action

console.log(businessType)

const business = ref({})

if (action == 'update') {
  const [res, error] = await api.business.get(route.query.id, businessType)
  if (!error) {
    business.value = res
  }
}

const stepNames = [
  'Main Information',
  'Location',
  'Images',
  'Rules & Capacity',
  'Additional Information',
  'Pricing'
]

const stepDescriptions = [
  'Tell us the most important details',
  {
    adventures: 'Where the adventure starts',
    cabins: 'Where the cabin is located',
    boats: 'Where the journey starts'
  }[businessType],
  'Show us some pretty shots',
  'What is allowed and what is not',
  'Tell us some specific details',
  'Define the prices of your services'
]

const currentStep = ref(0)
const steps = shallowRef([
  MainInfo,
  AddLocation,
  AddImages,
  AddRules,
  {
    adventures: AddFishingEquipment,
    boats: AddBoatEquipment,
    cabins: AddRooms
  }[businessType],
  Pricing
])

console.log(steps.value)

const isLoading = ref(false)
const currentComponent = ref()

const forward = () => {
  currentComponent.value.getValues()
}

const back = () => currentStep.value--

const onChange = async newData => {
  business.value = {
    ...business.value,
    ...newData
  }

  if (currentStep.value == stepNames.length - 1) {
    isLoading.value = true
    const [id] =
      action == 'update'
        ? await api.business.update(
            {
              id: route.query.id,
              ...business.value
            },
            businessType
          )
        : await api.business.create(business.value, businessType)
    if (id) {
      router.push(`/${businessType.slice(0, -1)}-profile/${id}`)
    }
    isLoading.value = false
  } else {
    currentStep.value++
  }
}
</script>
