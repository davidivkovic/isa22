<template>
  <div>
    <label class=""
      >Why do you want to make this account? Explain in a couple of
      sentences.</label
    >
    <TextArea required v-model="reason"> </TextArea>
    <p class="mt-3 text-sm text-red-500">{{ error }}</p>
    <div class="flex justify-end">
      <Button
        @click="forward()"
        class="group mt-7 flex items-center space-x-2 !rounded-full bg-emerald-600 !px-6 !py-2.5 text-white"
      >
        <div>Next</div>
        <ArrowNarrowRightIcon stroke-width="1.25" />
      </Button>
    </div>
  </div>
</template>
<script setup>
import { ref } from 'vue'
import Button from '../ui/Button.vue'
import TextArea from '../ui/TextArea.vue'
import { ArrowNarrowRightIcon } from 'vue-tabler-icons'
const reason = ref('I would like to advertise and make some money.')
const error = ref('')
const emit = defineEmits(['next'])
const forward = () => {
  emit('next')
}
</script>
