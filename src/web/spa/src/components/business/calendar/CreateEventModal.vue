<template>
  <Modal class="bg-white pt-10 pb-8 pl-6 pr-9 text-left" light>
    <h1 class="mb-6 ml-3 text-center text-xl font-medium">
      Add unavailability period
    </h1>
    <form @submit.prevent="createUnavailability()" class="space-y-3">
      <div class="">
        <label
          class="mb-2 ml-3 block w-min whitespace-nowrap pl-px text-sm font-medium tracking-tight text-neutral-700"
        >
          Start
        </label>
        <DateInput
          :has-time="true"
          required
          @change="value => (start = value)"
          placeholder="Start date"
          class="h-12 w-full pl-11"
        />
      </div>
      <div class="">
        <label
          class="mb-2 ml-3 block w-min whitespace-nowrap pl-px text-sm font-medium tracking-tight text-neutral-700"
        >
          End
        </label>
        <DateInput
          :has-time="true"
          required="true"
          @change="value => (end = value)"
          placeholder="End date"
          class="h-12 w-full pl-11"
        />
      </div>
      <div class="ml-1.5 flex w-full justify-center">
        <Button class="mt-2.5 w-56 bg-emerald-600 text-white">Add</Button>
      </div>
    </form>
  </Modal>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRoute } from 'vue-router'
import Modal from '@/components/ui/Modal.vue'
import DateInput from '@/components/ui/DateInput.vue'
import Button from '@/components/ui/Button.vue'
import { businessType } from '@/stores/userStore'
import api from '@/api/api.js'

const emit = defineEmits('success', 'modalClosed')

const start = ref()
const end = ref()
const businessId = useRoute().params.id

const createUnavailability = async () => {
  const [data, error] = await api.business.createUnavailability(
    businessId,
    start.value,
    end.value,
    businessType.value
  )
  if (!error) {
    emit('success', data)
    emit('modalClosed')
  }
}
</script>
