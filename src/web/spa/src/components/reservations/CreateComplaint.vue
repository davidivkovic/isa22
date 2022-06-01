<template>
  <Modal
    :isOpen="isOpen"
    @modalClosed="$emit('modalClosed')"
    :light="true"
    class="bg-white py-14 px-10 text-left"
  >
    <form ref="reviewForm" @submit.prevent="submit()">
      <div v-if="type === 'complaint'">
        <h1 class="text-gray-600">You had a bad experice?</h1>
        <h2 class="text-3xl font-medium">{{ name }}</h2>
        <div class="text-gray-600">
          {{ duration }} {{ unit }} in {{ address }}
        </div>
        <div class="text-gray-600">From {{ start }} to {{ end }}</div>
      </div>
      <div v-else>
        <h1 class="text-gray-600">You had a bad experice with</h1>
        <h2 class="text-3xl font-medium">{{ user }}?</h2>
      </div>
      <h2 class="mt-7 font-medium">1. Give us your feedback</h2>
      <h3 class="text-sm text-gray-600">
        In a few sentences, describe your experience. Tell us why was it bad.
      </h3>
      <TextArea
        required
        v-model="content"
        rows="5"
        placeholder="The adventure wasn't fun at all..."
        class="mt-3 rounded-xl"
      />
      <div v-if="type === 'report'">
        <h2 class="mt-5 mb-1 font-medium">
          2. Should we penalize the customer?
        </h2>
        <Checkbox
          v-model="penalize"
          label="Yes, the customer did not show up"
        />
      </div>
      <p class="mt-5 text-sm text-red-500">{{ errorMessage }}</p>
      <Button class="float-right mt-5 bg-emerald-600 text-white">Submit</Button>
    </form>
  </Modal>
</template>
<script setup>
import { ref, watch } from 'vue'
import Modal from '@/components/ui/Modal.vue'
import TextArea from '@/components/ui/TextArea.vue'
import Button from '@/components/ui/Button.vue'
import Checkbox from '../ui/Checkbox.vue'
import api from '@/api/api'

const props = defineProps([
  'name',
  'address',
  'start',
  'end',
  'isOpen',
  'duration',
  'unit',
  'businessType',
  'reservationId',
  'type',
  'user'
])

const emit = defineEmits(['modalClosed'])
const reviewForm = ref()
const content = ref('')
const errorMessage = ref('')
const penalize = ref(false)

const submitReport = async () => {
  console.log(penalize.value)
  errorMessage.value = ''
  const [_, error] = await api.business.report(
    props.reservationId,
    props.businessType,
    content.value,
    penalize.value
  )

  if (error) {
    errorMessage.value = error
  } else {
    emit('modalClosed')
  }
}

const submitComplaint = async () => {
  errorMessage.value = ''
  const [_, error] = await api.business.complain(
    props.reservationId,
    props.businessType,
    content.value
  )

  if (error) {
    errorMessage.value = error
  } else {
    emit('modalClosed')
  }
}

const submit = () =>
  props.type === 'complaint' ? submitComplaint() : submitReport()
</script>
