<template>
  <div class="w-96">
    <h3 class="text-lg font-medium">Rooms</h3>
    <h5 class="mb-5 whitespace-nowrap text-neutral-500">
      Tell us how many rooms and beds are there in the cabin
    </h5>
    <div class="mt-5 flex items-end space-x-3">
      <Input
        maxlength="40"
        v-model="item"
        label="New room"
        class="h-12 w-full"
        type="number"
        placeholder="Number of beds"
      >
      </Input>
      <Button
        :disabled="!itemValid"
        @click="addItem()"
        class="h-12 rounded-md border border-neutral-300 hover:bg-neutral-50 disabled:text-neutral-400 disabled:hover:bg-white"
      >
        Add
      </Button>
    </div>
    <p class="my-3 text-[13px] text-neutral-500">
      Tip: Remove rooms by clicking on them
    </p>
    <div class="mt-3 mr-16 flex flex-wrap gap-x-4 gap-y-1">
      <div
        v-for="item in equipment"
        @click="removeItem(item)"
        class="flex cursor-pointer items-center space-x-1"
      >
        <BedIcon class="h-5 w-5 text-neutral-400" />
        <p class="hover:text-neutral-500">Room with {{ item.beds }} beds</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { BedIcon } from 'vue-tabler-icons'

import Input from '@/components/ui/Input.vue'
import Button from '@/components/ui/Button.vue'

const props = defineProps(['rooms'])
const emit = defineEmits(['change'])

const item = ref('')
const equipment = ref(props.rooms ?? [])

const itemValid = computed(() => item.value != '')

const addItem = () => {
  equipment.value.push({ beds: item.value })
  item.value = ''
}

const removeItem = itemName => {
  equipment.value = equipment.value.filter(i => i.beds != itemName)
}

const getValues = () =>
  emit('change', {
    rooms: equipment.value
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
