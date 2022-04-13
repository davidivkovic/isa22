<template>
  <div>
    <div class="my-6 flex flex-col justify-between">
      <div class="ml-2 grid grid-cols-3">
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
            class="mx-auto mt-0.5"
          />
        </KeepAlive>
      </div>
    </div>
    <div class="mx-auto flex w-96 items-end">
      <Button
        v-if="currentStep > 0"
        @click="back()"
        class="flex items-center space-x-1"
      >
        <ArrowNarrowLeftIcon class="text-gray-800" />
        <div>Previous</div>
      </Button>
      <Button
        v-if="currentStep < steps.length - 1"
        @click="forward()"
        class="ml-auto bg-emerald-600 !px-7 text-white"
      >
        Next
      </Button>
      <Button
        v-if="currentStep == steps.length - 1"
        @click="forward()"
        class="-mr-11 ml-auto bg-emerald-600 !px-12 text-white"
      >
        Finish
      </Button>
    </div>
  </div>
</template>

<script setup>
import { ref, shallowRef, KeepAlive } from 'vue'
import { ArrowNarrowLeftIcon, CheckIcon } from 'vue-tabler-icons'
import { useRouter } from 'vue-router'

import adventures from '@/api/adventures.js'

import MainInfo from '@/components/adventure/create/MainInfo.vue'
import AddLocation from '@/components/adventure/create/AddLocation.vue'
import AddImages from '@/components/adventure/create/AddImages.vue'
import AddRules from '@/components/adventure/create/AddRules.vue'
import AddEquipment from '@/components/adventure/create/AddEquipment.vue'
import Pricing from '@/components/adventure/create/Pricing.vue'
import Button from '@/components/ui/Button.vue'

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
  'Where the adventure starts',
  'Show us some pretty shots',
  "What's allowed and what's not",
  'Tell us some specific details',
  'Define the prices of your services'
]
const router = useRouter()
const currentStep = ref(0)
const steps = shallowRef([
  MainInfo,
  AddLocation,
  AddImages,
  AddRules,
  AddEquipment,
  Pricing
])

let data = {}
const currentComponent = ref()

const forward = () => {
  currentComponent.value.getValues()
}

const back = () => currentStep.value--

const onChange = async newData => {
  data = {
    ...data,
    ...newData
  }
  if (currentStep.value == stepNames.length - 1) {
    const images = data.images
    delete data[images]
    const [id, error] = await adventures.create(data, images)
    if (!error) {
      router.push(`/adventure-profile/${id}`)
    }
  } else {
    currentStep.value++
  }
  console.log(data)
}
</script>
