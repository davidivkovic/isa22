<template>
  <form
    @submit.prevent="e => onSubmit(e)"
    id="main-info-form"
    name="main-info-form"
    class="w-96"
  >
    <h3 class="text-lg font-medium">Tell us about your adventure</h3>
    <h5 class="mb-5 whitespace-nowrap text-neutral-500">
      Your customers will value an appealing description
    </h5>
    <Input
      required
      autocomplete="false"
      class="h-12 w-full"
      placeholder="Name of the adventure"
      name="name"
      label="Name"
    />
    <div class="mt-3"></div>
    <TextArea
      required
      rows="8"
      maxlength="2000"
      name="description"
      class="pt-1"
      placeholder="Promotional description of the adventure"
      label="Description"
    ></TextArea>
    <p class="mt-0 text-xs text-neutral-400">Maximum 2000 characters</p>
  </form>
</template>

<script setup>
import { formData } from '@/components/utility/forms.js'

import Input from '@/components/ui/Input.vue'
import TextArea from '@/components/ui/TextArea.vue'

const emit = defineEmits(['change'])

const onSubmit = e => emit('change', formData(e))

const getValues = () =>
  document.getElementById('main-info-form').requestSubmit()

defineExpose({
  getValues
})
</script>
