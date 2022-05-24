<template>
  <Modal
    :isOpen="isOpen"
    @modalClosed="$emit('modalClosed')"
    :light="true"
    class="bg-white py-14 px-10 text-left"
  >
    <form ref="reviewForm" @submit.prevent="submitReview()">
      <h1 class="text-gray-600">What was your experience with</h1>
      <h2 class="text-3xl font-medium">{{ name }}</h2>
      <div class="text-gray-600">
        {{ duration }} {{ unit }} in {{ address }}
      </div>
      <div class="text-gray-600">From {{ start }} to {{ end }}</div>
      <h2 class="mt-7 font-medium">1. How would you rate your experience?</h2>
      <h3 class="text-sm text-gray-600">
        Please give a rating for this business.
      </h3>
      <div class="mt-5 flex space-x-2">
        <div
          v-for="grade in grades"
          :key="grade.grade"
          @click="selectedGrade = grade.grade"
          :class="{
            'outline outline-2 outline-emerald-600': selected(grade.grade)
          }"
          class="flex h-32 w-32 cursor-pointer flex-col items-center justify-center space-y-1 rounded-2xl border border-gray-300 hover:bg-gray-50"
        >
          <component
            :is="grade.icon"
            :class="{ 'text-emerald-700': selected(grade.grade) }"
            class="h-10 w-10 stroke-1"
          />
          <div
            :class="{ 'font-medium text-emerald-700': selected(grade.grade) }"
            class="text-sm"
          >
            {{ grade.description }} ({{ grade.grade }})
          </div>
        </div>
      </div>
      <h2 class="mt-10 font-medium">2. Give us your review</h2>
      <h3 class="text-sm text-gray-600">
        In a few sentences, describe your experience. Point out some good and
        bad parts.
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
        >Submit your review</Button
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
const selectedGrade = ref(0)
const content = ref('')
const errorMessage = ref('')
const selected = grade => selectedGrade.value == grade

const grades = [
  { grade: 1, icon: MoodCryIcon, description: 'Bad' },
  { grade: 2, icon: MoodSadIcon, description: 'Okay' },
  { grade: 3, icon: MoodEmptyIcon, description: 'Good' },
  { grade: 4, icon: MoodSmileIcon, description: 'Very Good' },
  { grade: 5, icon: MoodHappyIcon, description: 'Excellent' }
]
const routes = {
  cabins: 'cabin-profile',
  boats: 'boat-profile',
  adventures: 'adventure-profile'
}

const submitReview = async () => {
  if (selectedGrade.value == 0 || !content.value) {
    errorMessage.value = 'You must leave a grade and a comment!'
  } else {
    errorMessage.value = ''
    console.log(selectedGrade.value, content.value)
    const [_, error] = await api.business.review(
      props.id,
      props.businessType,
      content.value,
      selectedGrade.value
    )

    if (error) {
      errorMessage.value = error
    } else {
      router.push({
        name: routes[props.businessType],
        params: { id: props.id }
      })
    }
  }
}
</script>
