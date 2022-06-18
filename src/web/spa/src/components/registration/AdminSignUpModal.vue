<template>
  <Modal
    :isOpen="isModalOpen"
    @modalClosed="closeModal()"
    :light="true"
    class="w-[500px] bg-white px-14 py-10"
  >
    <Transition
      enter-active-class="transition-opacity duration-100"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition-opacity duration-100"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
      mode="out-in"
    >
      <form @submit.prevent class="relative text-left">
        <h1 class="mt-10 text-2xl font-medium leading-7">
          {{ title }}
        </h1>
        <Transition mode="out-in">
          <div v-if="showPrevious" class="mt-8">
            <button @click="back()" class="group flex space-x-2">
              <ArrowNarrowLeftIcon
                stroke-width="1.25"
                class="group-hover:stroke-2"
              />
              <div class="group-hover:font-semibold">Previous</div>
            </button>
          </div>
        </Transition>
        <Transition mode="out-in">
          <KeepAlive>
            <Component
              :is="panels[selectedPanel]"
              @next="forward"
              @register="register"
              success-text="You have successfully registered a new admin. They will be prompted to set a new password upon signing in."
              class="mt-10"
            />
          </KeepAlive>
        </Transition>
        <div class="absolute top-[15.25rem] mt-5 text-sm text-red-500">
          {{ formError }}
        </div>
        <div class="flex justify-end"></div>
        <p class="mt-5 text-center text-xs leading-tight text-gray-600">
          By proceeding, you agree to our
          <span class="underline">Terms of Use</span> and confirm you have read
          our <span class="underline"> Privacy and Cookie Statement</span>.
        </p>
      </form>
    </Transition>
  </Modal>
</template>

<script setup>
import { ref, shallowRef, computed, watch, onBeforeMount } from 'vue'
import Modal from '../ui/Modal.vue'
import UserInformationForm from './UserInformationForm.vue'
import VerificationPanel from './VerificationPanel.vue'
import AccountInformationForm from './AccountInformationForm.vue'
import AddressForm from './AddressForm.vue'
import { ArrowNarrowLeftIcon } from 'vue-tabler-icons'
import api from '@/api/api.js'
import router from '@/router/index.js'

const selectedPanel = ref(0)
const selectedRole = ref('Admin')
let data = {}
const formError = ref('')

const panels = shallowRef([
  UserInformationForm,
  AddressForm,
  AccountInformationForm,
  VerificationPanel
])
const title = computed(() =>
  selectedPanel.value < panels.value.length - 1
    ? 'Register a new admin on adventure.com'
    : 'Registration successful!'
)

const showPrevious = computed(
  () => selectedPanel.value > 0 && selectedPanel.value < panels.value.length - 1
)

const back = () => {
  selectedPanel.value--
}

const forward = newData => {
  data = { ...data, ...newData }
  selectedPanel.value++
}

const register = async newData => {
  data = { ...data, ...newData }
  data['roles'] = [selectedRole.value]
  const [response, error] = await api.auth.register(data)
  formError.value = error

  if (response && selectedRole.value === 'customer') {
    router.push({ name: 'verification', query: { email: data.email } })
  } else !error && forward()
}

const isModalOpen = ref(false)
const closeModal = ref()

onBeforeMount(async () => {
  const { useModal } = await import('@/stores/modalStore.js')
  const { isModalOpen: isOpen, closeModal: close } = useModal()
  watch(isOpen, () => (isModalOpen.value = isOpen.value), { immediate: true })
  closeModal.value = close
})
</script>

<style scoped>
.v-enter-active,
.v-leave-active {
  transition: opacity 0.2s ease-in-out;
}

.v-enter-from,
.v-leave-to {
  opacity: 0;
}
</style>
