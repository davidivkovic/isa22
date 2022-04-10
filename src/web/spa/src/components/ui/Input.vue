<template>
  <div class="w-full">
    <label
      v-if="$attrs.label"
      :for="$attrs.id"
      class="mb-1 block pl-px text-[15px] font-medium tracking-tight text-neutral-700"
    >
      {{ $attrs.label }}
    </label>
    <div class="relative">
      <div
        ref="prepend"
        v-if="$slots.prepend"
        class="absolute left-0 flex h-full items-center pb-0.5"
      >
        <slot name="prepend" :focused="focused" :hovered="hovered"></slot>
      </div>
      <input
        ref="input"
        type="text"
        spellcheck="false"
        v-bind="$attrs"
        :value="inputValue"
        :style="inputStyle"
        :name="name"
        @focusin="focused = true"
        @focusout="focused = false"
        @mouseover="hovered = true"
        @mouseleave="hovered = false"
        @input="e => onInput(e)"
        class="hover:disabled:border-neutral-30 0 block rounded-md border-neutral-300 px-2.5 py-2.5 text-sm placeholder-neutral-400 transition-colors hover:border-neutral-500 focus:border-neutral-500 focus:ring-0 focus:ring-transparent disabled:text-neutral-300 focus:disabled:border-neutral-300"
      />
      <div
        ref="append"
        class="absolute inset-y-0 right-0 flex h-full items-center"
      >
        <div
          @click="clearInput()"
          v-if="(clearable || clearable == '') && !!inputValue"
          class="m-1 cursor-pointer p-1 text-neutral-400 hover:text-gray-700"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-[18px] w-[18px]"
            viewBox="0 0 24 24"
            stroke-width="2"
            stroke="currentColor"
            fill="none"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <path stroke="none" d="M0 0h24v24H0z" fill="none"></path>
            <line x1="18" y1="6" x2="6" y2="18"></line>
            <line x1="6" y1="6" x2="18" y2="18"></line>
          </svg>
        </div>
        <slot
          name="append"
          v-else-if="$slots.append"
          :input="input"
          :focused="focused"
          :hovered="hovered"
        ></slot>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const props = defineProps(['modelValue', 'clearable', 'name'])
const emit = defineEmits(['update:modelValue'])

const input = ref()
const prepend = ref()
const append = ref()
const inputStyle = ref('')
const inputValue = ref(props.modelValue)
const focused = ref(false)
const hovered = ref(false)

const onInput = event => {
  inputValue.value = event.target.value
  emit('update:modelValue', inputValue.value)
}

const clearInput = () => {
  inputValue.value = ''
  emit('update:modelValue', '')
}

onMounted(() => {
  inputStyle.value = `
    padding-left: ${prepend.value?.clientWidth - 4}px;
    padding-right: ${append.value?.clientWidth - 4}px;
  `
})
</script>

<script>
export default {
  inheritAttrs: false
}
</script>
