<template>
  <Modal
    :isOpen="isModalOpen"
    @modalClosed="closeModal()"
    :light="true"
    class="w-[500px] bg-white px-14 py-10"
  >
    <XIcon
      @click="closeModal()"
      class="absolute top-3 right-3 h-6 w-6 cursor-pointer text-black"
    ></XIcon>
    <Transition
      enter-active-class="transition-opacity duration-100"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition-opacity duration-100"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
      mode="out-in"
    >
      <Component @switchAuth="switchAuth()" :is="authPanel" />
    </Transition>
  </Modal>
</template>

<script setup>
import { shallowRef } from 'vue'
import Modal from './ui/Modal.vue'
import SignInForm from './SignInPanel.vue'
import RegistrationForm from './RegistrationPanel.vue'
import { XIcon } from 'vue-tabler-icons'
import { useModal } from '../stores/modalStore.js'

const authPanel = shallowRef(SignInForm)
const switchAuth = () => {
  authPanel.value =
    authPanel.value === SignInForm ? RegistrationForm : SignInForm
}

const { isModalOpen, closeModal } = useModal()
</script>
