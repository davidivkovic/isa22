<template>
  <div class="mt-10 space-y-4">
    <button
      v-for="role in roles"
      :key="role"
      @click="selectRole(role)"
      :class="{
        'outline outline-2 outline-emerald-600 ': selectedRole === role
      }"
      class="h-[4.5rem] w-full cursor-pointer rounded-full border px-6 py-2 transition hover:bg-gray-50"
    >
      <div class="flex h-full items-center justify-between">
        <div class="flex items-center space-x-4">
          <img :src="images[role]" alt="Icon" class="h-10 w-10 object-cover" />
          <p>I want to be {{ addArticle(role) }}</p>
        </div>
        <ChevronRightIcon
          class="h-8 w-8 text-black"
          :class="{
            '!text-emerald-700': selectedRole === role
          }"
          stroke-width="1.5"
        />
      </div>
    </button>
    <div class="flex justify-end">
      <Button
        v-if="selectedRole != ''"
        @click="forward()"
        class="group mt-7 flex items-center space-x-2 !rounded-full bg-emerald-600 !px-6 !py-2.5 text-white"
      >
        <div>Next</div>
        <ArrowNarrowRightIcon stroke-width="1.25" />
      </Button>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import Button from '../ui/Button.vue'
import { ChevronRightIcon, ArrowNarrowRightIcon } from 'vue-tabler-icons'

const selectedRole = ref('')
const roles = ['customer', 'advertiser']
const emit = defineEmits(['selectedRole', 'next'])

const selectRole = role => {
  selectedRole.value = role
}
const forward = () => {
  emit('selectedRole', selectedRole.value)
  emit('next')
}
const images = {
  customer: 'https://cdn-icons-png.flaticon.com/512/6714/6714037.png',
  advertiser: 'https://cdn-icons-png.flaticon.com/512/5692/5692207.png'
}
const addArticle = role =>
  role === 'customer' ? 'a customer' : 'an advertiser'
</script>
