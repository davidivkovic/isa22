<template>
  <form
    @submit.prevent="e => onSubmit(e)"
    id="add-boat-equipment-form"
    name="add-boat-equipment-form"
    class="w-96"
  >
    <h3 class="text-lg font-medium">Equipment</h3>
    <h5 class="mb-5 text-neutral-500">
      Tell us what equipment is present on the boat
    </h5>
    <div class="mt-5 flex items-end space-x-3">
      <Input
        maxlength="40"
        v-model="fishingItem"
        label="New fishing item"
        class="h-12 w-full"
        placeholder="Add a new fishing item"
      >
      </Input>
      <Button
        :disabled="!fishingItemValid"
        @click="addFishingItem()"
        class="h-12 rounded-md border border-neutral-300 hover:bg-neutral-50 disabled:text-neutral-400 disabled:hover:bg-white"
      >
        Add
      </Button>
    </div>
    <p class="my-3 text-[13px] text-neutral-500">
      Tip: Remove fishing equipment by clicking on it
    </p>
    <div class="mt-3 mr-16 flex flex-wrap gap-x-4 gap-y-1">
      <div
        v-for="item in fishingEquipment"
        @click="removeFishingItem(item)"
        class="flex cursor-pointer items-center space-x-1"
      >
        <ChecksIcon class="h-5 w-5 text-neutral-400" />
        <p class="hover:text-neutral-500">{{ item }}</p>
      </div>
    </div>
    <div class="mt-5 flex items-end space-x-3">
      <Input
        maxlength="40"
        v-model="navigationItem"
        label="New navigation item"
        class="h-12 w-full"
        placeholder="Add a new navigation item"
      >
      </Input>
      <Button
        :disabled="!navigationItemValid"
        @click="addNavigationItem()"
        class="h-12 rounded-md border border-neutral-300 hover:bg-neutral-50 disabled:text-neutral-400 disabled:hover:bg-white"
      >
        Add
      </Button>
    </div>
    <p class="my-3 text-[13px] text-neutral-500">
      Tip: Remove navigation equipment by clicking on it
    </p>
    <div class="mt-3 mr-16 flex flex-wrap gap-x-4 gap-y-1">
      <div
        v-for="item in navigationEquipment"
        @click="removeNavigationItem(item)"
        class="flex cursor-pointer items-center space-x-1"
      >
        <ChecksIcon class="h-5 w-5 text-neutral-400" />
        <p class="hover:text-neutral-500">{{ item }}</p>
      </div>
    </div>

    <div class="mt-5 flex space-x-4">
      <div class="w-20">
        <Input
          required
          v-model="state.seats"
          placeholder="10"
          type="number"
          label="Seats"
          class="h-12 w-full"
        />
      </div>
      <div required class="w-20">
        <Input
          required
          v-model="state.engines"
          placeholder="2"
          type="number"
          label="Engines"
          class="h-12 w-full"
        />
      </div>
      <div class="w-20">
        <Input
          required
          v-model="state.length"
          type="number"
          placeholder="25"
          label="Length"
          class="h-12 w-full"
        />
      </div>
    </div>
    <div class="mt-3 flex space-x-4">
      <div class="w-20">
        <Input
          v-model="state.bhp"
          required
          type="number"
          placeholder="550"
          label="BHP"
          class="h-12 w-full"
        />
      </div>
      <div class="w-20">
        <Input
          v-model="state.topSpeed"
          required
          type="number"
          placeholder="110"
          label="Top Speed"
          class="h-12 w-full"
        />
      </div>
    </div>
  </form>
</template>

<script setup>
import { ref, computed, reactive } from 'vue'
import { ChecksIcon } from 'vue-tabler-icons'

import Input from '@/components/ui/Input.vue'
import Button from '@/components/ui/Button.vue'

const props = defineProps(['equipment', 'characteristics'])
const emit = defineEmits(['change'])

const state = reactive({
  topSpeed: props.characteristics?.topSpeed ?? '',
  length: props.characteristics?.length ?? '',
  engines: props.characteristics?.engines ?? '',
  bhp: props.characteristics?.bhp ?? '',
  seats: props.characteristics?.seats ?? ''
})

const fishingItem = ref('')
const navigationItem = ref('')

const fishingEquipment = ref(props.equipment?.fishing ?? [])
const navigationEquipment = ref(props.equipment?.navigational ?? [])

const fishingItemValid = computed(() => {
  return (
    !fishingEquipment.value.some(
      i => i.toLowerCase() == fishingItem.value.toLowerCase()
    ) && fishingItem.value != ''
  )
})

const navigationItemValid = computed(() => {
  return (
    !navigationEquipment.value.some(
      i => i.toLowerCase() == navigationItem.value.toLowerCase()
    ) && navigationItem.value != ''
  )
})

const addFishingItem = () => {
  fishingEquipment.value.push(fishingItem.value)
  fishingItem.value = ''
}

const removeFishingItem = itemName => {
  fishingEquipment.value = fishingEquipment.value.filter(i => i != itemName)
}

const addNavigationItem = () => {
  navigationEquipment.value.push(navigationItem.value)
  navigationItem.value = ''
}

const removeNavigationItem = itemName => {
  navigationEquipment.value = navigationEquipment.value.filter(
    i => i != itemName
  )
}

const getValues = () => {
  document.getElementById('add-boat-equipment-form').requestSubmit()
}

const onSubmit = () =>
  emit('change', {
    equipment: {
      fishing: fishingEquipment.value,
      navigational: navigationEquipment.value
    },
    characteristics: {
      seats: state.seats,
      topSpeed: state.topSpeed,
      length: state.length,
      engines: state.engines,
      bhp: state.bhp
    }
  })

defineExpose({
  getValues
})
</script>

<script>
export default {
  inheritAttrs: false
}
</script>
