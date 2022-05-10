<template>
  <Modal
    @modalClosed="$emit('modalClosed')"
    :isOpen="props.isOpen"
    class="relative h-[700px] w-[1000px] bg-white"
  >
    <XIcon
      @click="$emit('modalClosed')"
      class="absolute top-3 right-3 h-6 w-6 cursor-pointer text-black"
    ></XIcon>
    <img
      :src="props.images[selectedImageIndex]"
      alt=""
      class="h- mx-auto h-full w-full select-none rounded-md object-cover"
    />
    <div
      class="absolute left-1/2 bottom-3 -translate-x-1/2 rounded-full bg-black/[0.5] py-2 px-8 text-sm text-white"
    >
      {{ selectedImageIndex + 1 }} / {{ props.images.length }}
    </div>
    <ChevronLeftIcon
      @click="previousImage()"
      class="absolute left-3 top-1/2 bottom-1/2 h-16 w-16 cursor-pointer text-black"
    />
    <ChevronRightIcon
      @click="nextImage()"
      class="absolute right-3 top-1/2 bottom-1/2 h-16 w-16 cursor-pointer text-black"
    />
  </Modal>
</template>
<script setup>
import { ref } from 'vue'
import { ChevronLeftIcon, ChevronRightIcon, XIcon } from 'vue-tabler-icons'
import Modal from '../../ui/Modal.vue'
const props = defineProps(['images', 'isOpen'])
const selectedImageIndex = ref(0)

const nextImage = () => {
  selectedImageIndex.value =
    (selectedImageIndex.value + 1) % props.images.length
}
const previousImage = () =>
  (selectedImageIndex.value =
    (selectedImageIndex.value + props.images.length - 1) % props.images.length)

const selectImage = index => (selectedImageIndex.value = index)
</script>
