<template>
  <div class="mt-10 space-y-4">
    <h3 class="text-lg">What do you advertise?</h3>
    <button
      v-for="service in services"
      :key="service"
      @click="selectService(service)"
      :class="{
        'outline outline-2 outline-emerald-600 ': selectedService === service
      }"
      class="mt-2 h-[4.5rem] w-full cursor-pointer rounded-full border px-6 py-2 hover:bg-gray-100"
    >
      <div class="flex h-full items-center justify-between">
        <div class="flex items-center space-x-4">
          <img
            :src="images[service]"
            alt=""
            class="h-10 w-10 rounded-full object-cover"
          />
          <p>I advertise {{ service }}</p>
        </div>
        <ChevronRightIcon class="h-8 w-8 text-black" stroke-width="1.5" />
      </div>
    </button>
    <div class="flex justify-end">
      <Button
        v-if="selectedService != ''"
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
const services = ['cabins', 'boats', 'fishing adventures']
const roles = {
  cabins: 'CabinOwner',
  boats: 'BoatOwner',
  'fishing adventures': 'Fisher'
}
const images = {
  cabins: 'https://cdn-icons-png.flaticon.com/512/4264/4264183.png',
  boats: 'https://cdn-icons-png.flaticon.com/512/186/186181.png',
  'fishing adventures':
    'https://cdn-icons-png.flaticon.com/512/1027/1027716.png'
}

const selectedService = ref('')
const emit = defineEmits(['selectedRole', 'next'])
const selectService = service => {
  selectedService.value = service
}
const forward = () => {
  emit('selectedRole', roles[selectedService.value])
  emit('next')
}
</script>
