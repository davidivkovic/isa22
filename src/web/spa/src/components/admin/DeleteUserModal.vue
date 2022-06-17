<template>
  <Modal
    :isOpen="isOpen"
    @modalClosed="$emit('modalClosed')"
    class="max-w-sm bg-white text-sm"
  >
    <div class="divide-y divide-gray-300">
      <div
        class="flex flex-col items-center justify-between space-y-8 py-10 px-10"
      >
        <div>
          Are you sure you want to delete the account of user
          {{ user.firstName }} {{ user.lastName }}?
        </div>
      </div>
      <button @click="deleteUser()" class="w-full p-3 font-medium text-red-500">
        Delete account
      </button>
      <button @click="$emit('modalClosed')" class="w-full p-3">Cancel</button>
    </div>
  </Modal>
</template>
<script setup>
import api from '@/api/api.js'
import Modal from '@/components/ui/Modal.vue'

const props = defineProps(['isOpen', 'user'])
const emit = defineEmits(['modalClosed', 'userDeleted'])

const deleteUser = async () => {
  const [, error] = await api.users.deleteUser(props.user.id)
  if (!error) {
    emit('userDeleted', props.user.id)
  }
}
</script>
