<template>
  <Modal
    :isOpen="isOpen"
    @modalClosed="$emit('modalClosed')"
    :light="true"
    class="bg-white py-14 px-10 text-left"
  >
    <form ref="reviewForm" @submit.prevent="submitComplaint()">
      <h1 class="text-gray-600">You had a bad experice with</h1>
      <h2 class="text-3xl font-medium">{{ name }}</h2>
      <div class="text-gray-600">
        {{ duration }} {{ unit }} in {{ address }}
      </div>
      <div class="text-gray-600">From {{ start }} to {{ end }}</div>

      <h2 class="mt-10 font-medium">1. Give us your feedback</h2>
      <h3 class="text-sm text-gray-600">
        In a few sentences, describe your experience. Tell us why was it bad.
      </h3>
      <TextArea
        required
        v-model="content"
        rows="5"
        placeholder="The adventure was really fun because..."
        class="mt-3 rounded-2xl"
      />
      <p class="text-sm text-red-500">{{ errorMessage }}</p>
      <Button class="float-right mt-5 bg-emerald-600 text-white"
        >Submit your complaint</Button
      >
    </form>
  </Modal>
</template>
<script setup>
import { ref } from 'vue'
import Modal from '@/components/ui/Modal.vue'
import TextArea from '@/components/ui/TextArea.vue'
import Button from '@/components/ui/Button.vue'
import api from '@/api/api'
import {
  MoodCryIcon,
  MoodSadIcon,
  MoodEmptyIcon,
  MoodSmileIcon,
  MoodHappyIcon
} from 'vue-tabler-icons'
import { useRouter } from 'vue-router'

const props = defineProps([
  'name',
  'address',
  'start',
  'end',
  'isOpen',
  'duration',
  'unit',
  'businessType',
  'id'
])

const router = useRouter()
const reviewForm = ref()
const content = ref('')
const errorMessage = ref('')

const routes = {
  cabins: 'cabin-profile',
  boats: 'boat-profile',
  adventures: 'adventure-profile'
}

const submitComplaint = async () => {
  errorMessage.value = ''
  console.log(content.value)
  //   const [_, error] = await api.business.review(
  //     props.id,
  //     props.businessType,
  //     content.value,
  //     selectedGrade.value
  //   )

  //   if (error) {
  //     errorMessage.value = error
  //   } else {
  //     router.push({
  //       name: routes[props.businessType],
  //       params: { id: props.id }
  //     })
  //   }
}
</script>
