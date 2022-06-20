<template>
  <Modal
    :isOpen="isOpen"
    :light="true"
    @modalClosed="closeModal()"
    class="w-[500px] bg-white px-14 py-10"
  >
    <form v-if="!success" @submit.prevent="confirm" class="space-y-3 text-left">
      <div class="mt-4 mb-8">
        <h1 class="text-3xl font-medium tracking-tight">Change password</h1>
        <p>
          Please enter your current password and your new password to proceed.
        </p>
      </div>

      <PasswordInput
        required
        class="h-12 w-full"
        name="currentPassword"
        label="Current password"
        placeholder="Enter Password"
      />
      <PasswordInput
        required
        class="h-12 w-full"
        name="newPassword"
        label="New password"
        placeholder="Enter Password"
      />
      <div class="text-sm text-red-500">
        {{ formError }}
      </div>
      <div class="flex justify-end">
        <Button
          type="submit"
          class="group mt-4 flex items-center space-x-2 !rounded-full bg-emerald-600 !px-6 !py-2.5 text-white"
        >
          <div>Confirm</div>
          <ArrowNarrowRightIcon stroke-width="1.25" />
        </Button>
      </div>
      <div>
        <p class="mt-6 text-center text-xs leading-tight text-gray-600">
          By proceeding, you agree to our
          <span class="underline">Terms of Use</span> and confirm you have read
          our <span class="underline"> Privacy and Cookie Statement</span>.
        </p>
      </div>
    </form>
    <div v-else class="pt-4 pb-4">
      <h1 class="mb-6 text-2xl font-medium">Success</h1>
      <VerificationPanel
        success-text="You have successfully changed your password. Please sign in using your new password."
      />
      <Button
        @click="closeModal()"
        class="mt-8 bg-emerald-600 px-8 text-white hover:bg-emerald-700"
        >Confirm</Button
      >
    </div>
  </Modal>
</template>

<script setup>
import { ref } from 'vue'
import Modal from '../ui/Modal.vue'
import { formData } from '../utility/forms'
import PasswordInput from '../ui/PasswordInput.vue'
import VerificationPanel from './VerificationPanel.vue'
import Button from '../ui/Button.vue'
import { ArrowNarrowRightIcon } from 'vue-tabler-icons'
import api from '@/api/api'

defineProps(['isOpen'])
const emit = defineEmits(['closed'])

const formError = ref('')
const success = ref(false)

const confirm = async event => {
  formError.value = ''
  const form = formData(event)
  const [, error] = await api.auth.changePassword(
    form.currentPassword,
    form.newPassword
  )
  if (error) {
    formError.value = error
  } else {
    success.value = true
  }
}

const closeModal = () => {
  if (success.value) {
    window.location.href = '/'
  } else {
    emit('closed')
  }
}
</script>
