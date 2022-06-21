<template>
  <form @submit.prevent="forward" class="w-full space-y-3">
    <div class="flex space-x-2">
      <div class="w-full">
        <Input
          required
          class="h-12 w-full"
          name="street"
          placeholder="Street"
          label="Street"
        />
      </div>
      <Input
        required
        class="h-12 w-full"
        name="apartment"
        placeholder="Apartment"
        label="Apartment"
      />
    </div>
    <div class="flex space-x-2">
      <Input
        required
        class="h-12 w-full"
        name="postalCode"
        placeholder="Postal Code"
        label="Postal Code"
        type="number"
        min="0"
      />
      <Input
        required
        class="h-12 w-full"
        name="city"
        placeholder="City"
        label="City"
      />
    </div>
    <Input
      required
      class="h-12 w-full"
      name="country"
      placeholder="Country"
      label="Country"
    />
    <p class="mt-5 text-sm text-red-500">{{ error }}</p>
    <div class="flex justify-end">
      <Button
        type="submit"
        class="group mt-7 flex items-center space-x-2 !rounded-full bg-emerald-600 !px-6 !py-2.5 text-white"
      >
        <div>Next</div>
        <ArrowNarrowRightIcon stroke-width="1.25" />
      </Button>
    </div>
  </form>
</template>
<script setup>
import { ref } from 'vue'
import { formData } from '../utility/forms'
import Button from '../ui/Button.vue'
import Input from '../ui/Input.vue'
import { ArrowNarrowRightIcon } from 'vue-tabler-icons'

const emit = defineEmits(['next'])
const error = ref('')
const forward = event => {
  const data = formData(event)
  emit('next', { address: data })
}
</script>
