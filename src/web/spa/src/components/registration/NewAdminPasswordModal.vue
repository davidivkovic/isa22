<template>
  <Modal
    :isOpen="isModalOpen"
    @modalClosed="closeModal()"
    :light="true"
    class="w-[500px] space-y-5 bg-white px-14 py-16 text-left"
  >
    <XIcon
      @click="closeModal()"
      class="absolute top-3 right-3 h-6 w-6 cursor-pointer text-black"
    ></XIcon>
    <h1 class="text-3xl font-medium">Welcome.</h1>
    <h2 class="mt-5 text-base leading-6">
      In order to fulfill your role as admin, please enter a new password for
      your account.
    </h2>
    <PasswordInput
      class="h-12 w-full"
      label="New password"
      v-model="newPassword"
    />
    <div class="flex justify-center pt-5">
      <Button
        @click="changePassword"
        type="submit"
        class="!rounded-full bg-emerald-600 !py-4 !px-24 text-lg text-white"
        >Chane password</Button
      >
    </div>
  </Modal>
</template>
<script setup>
import { ref } from 'vue'
import Modal from './ui/Modal.vue'
import { useModal } from '../stores/modalStore'
import PasswordInput from './ui/PasswordInput.vue'
import Button from './ui/Button.vue'
import { XIcon } from 'vue-tabler-icons'
const { isModalOpen, closeModal } = useModal()

const newPassword = ref('')
const changePassword = () => {
  console.log(newPassword.value)
}
</script>
