<template>
  <div class="w-96">
    <h3 class="text-lg font-medium">Equipment</h3>
    <h5 class="mb-5 text-neutral-500">
      Tell us what equipment is included in the adventure
    </h5>
    <div class="mt-5 flex items-end space-x-3">
      <Input
        maxlength="40"
        v-model="item"
        label="New item"
        class="h-12 w-full"
        placeholder="Add a new item"
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
      Tip: Remove equipment by clicking on it
    </p>
    <div class="mt-3 mr-16 flex flex-wrap gap-x-4 gap-y-1">
      <div
        v-for="item in equipment"
        @click="removeItem(item)"
        class="flex cursor-pointer items-center space-x-1"
      >
        <ChecksIcon class="h-5 w-5 text-neutral-400" />
        <p class="hover:text-neutral-500">{{ item }}</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { ChecksIcon } from 'vue-tabler-icons'

import Input from '@/components/ui/Input.vue'
import Button from '@/components/ui/Button.vue'

const props = defineProps(['fishingEquipment'])
const emit = defineEmits(['change'])

const item = ref('')
const equipment = ref(props.fishingEquipment ?? [])

const itemValid = computed(() => {
  return (
    !equipment.value.some(i => i.toLowerCase() == item.value.toLowerCase()) &&
    item.value != ''
  )
})

const addItem = () => {
  equipment.value.push(item.value)
  item.value = ''
}

const removeItem = itemName => {
  equipment.value = equipment.value.filter(i => i != itemName)
}

const getValues = () =>
  emit('change', {
    fishingEquipment: equipment.value
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
