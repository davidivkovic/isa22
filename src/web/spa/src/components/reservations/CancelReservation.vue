<template>
  <Modal :isOpen="props.isOpen" class="bg-white">
    <div class="max-w-md divide-y divide-gray-300">
      <div
        class="flex flex-col items-center justify-between space-y-8 py-10 px-10"
      >
        <div>
          Are you sure you want to cancel your reservation? If you cancel now,
          you might not be able to make the same reservation anymore.
        </div>
        <div class="text-sm text-red-500">{{ cancelError }}</div>
      </div>
      <button
        @click="cancelReservation()"
        class="w-full p-3 font-medium text-red-500"
      >
        Yes, cancel my reservation
      </button>
      <button @click="$emit('modalClosed')" class="w-full p-3">Dismiss</button>
    </div></Modal
  >
</template>
<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import Modal from '../ui/Modal.vue'
import api from '@/api/api'

const router = useRouter()
const props = defineProps(['isOpen', 'reservationId', 'businessType'])
const cancelError = ref('')

const cancelReservation = async () => {
  console.log(props.businessType, props.reservationId)
  const [data, error] = await api.business.cancelReservation(
    props.businessType,
    props.reservationId
  )
  if (error) {
    cancelError.value = error
  } else {
    router.go()
  }
}
</script>
