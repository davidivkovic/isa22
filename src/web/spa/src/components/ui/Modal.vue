<template>
  <TransitionRoot as="template" :show="isOpen" appear>
    <Dialog
      as="div"
      static
      class="fixed inset-0 z-20 overflow-y-auto"
      @close="emitModalClosed()"
    >
      <div
        class="flex min-h-screen items-end justify-center px-4 pt-4 pb-20 text-center sm:block sm:p-0"
      >
        <TransitionChild
          as="template"
          enter="ease-out duration-150"
          enter-from="opacity-0"
          enter-to="opacity-100"
          leave="ease-in duration-100"
          leave-from="opacity-100"
          leave-to="opacity-0"
        >
          <DialogOverlay
            class="fixed inset-0 bg-black/[85%] transition-opacity"
            :class="{ 'bg-white/[65%]': light }"
          />
        </TransitionChild>
        <TransitionChild
          as="template"
          enter="ease-out duration-50"
          enter-from="opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95"
          enter-to="opacity-100 translate-y-0 sm:scale-100"
          leave="ease-in duration-100"
          leave-from="opacity-100 translate-y-0 sm:scale-100"
          leave-to="opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95"
        >
          <div class="flex h-screen items-center justify-center">
            <div
              class="transform rounded-2xl transition-all duration-500"
              style="box-shadow: 0 6px 50px 0 rgb(0 0 0 / 25%)"
              v-bind="$attrs"
            >
              <slot></slot>
            </div>
          </div>
        </TransitionChild>
      </div>
    </Dialog>
  </TransitionRoot>
</template>

<script>
import {
  Dialog,
  DialogOverlay,
  TransitionChild,
  TransitionRoot
} from '@headlessui/vue'

export default {
  inheritAttrs: false,
  components: {
    Dialog,
    DialogOverlay,
    TransitionChild,
    TransitionRoot
  },
  props: {
    isOpen: Boolean,
    classes: String,
    light: Boolean
  },
  emits: ['modalClosed'],
  setup: (props, { emit }) => {
    const emitModalClosed = () => emit('modalClosed')
    return {
      emitModalClosed
    }
  }
}
</script>
