<template>
  <div class="space-y-7">
    <CheckIcon
      class="mx-auto h-28 w-28 rounded-full bg-emerald-100 stroke-emerald-500 p-5"
    />
    <h1 class="text-lg">
      {{ panelText }}
    </h1>
  </div>
</template>

<script setup>
import { computed } from '@vue/reactivity'
import { CheckIcon } from 'vue-tabler-icons'
const props = defineProps({
  role: String
})
const panelText = computed(() =>
  props.role === 'customer'
    ? 'A verification link is sent to your email address. Open the link to verify your account.'
    : 'Aftering approving your registration by admin, a message will be sent to your email address.'
)
</script>
