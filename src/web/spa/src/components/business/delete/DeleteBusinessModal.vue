<template>
  <Modal
    :isOpen="props.isOpen"
    @modalClosed="$emit('modalClosed')"
    class="max-w-sm bg-white text-sm"
  >
    <div class="divide-y divide-gray-300">
      <div
        class="flex flex-col items-center justify-between space-y-8 py-10 px-10"
      >
        <div>Are you sure you want to delete your business?</div>
      </div>
      <button
        @click="deleteBusiness(this)"
        class="w-full p-3 font-medium text-red-500"
      >
        Delete business
      </button>
      <button @click="$emit('modalClosed')" class="w-full p-3">Cancel</button>
    </div>
  </Modal>
</template>
<script setup>
import api from '@/api/api.js'
import Modal from '@/components/ui/Modal.vue'
import { useRouter } from 'vue-router'

const props = defineProps(['isOpen', 'businessId', 'type', 'back'])

const router = useRouter()
const deleteBusiness = async self => {
  const [, error] = await api.business.remove(props.businessId, props.type)
  if (!error) {
    if (props.back) {
      router.back()
    }
    self.$emit('elementDeleted', props.businessId)
  }
}
</script>
