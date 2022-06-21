<template>
  <div class="relative h-36 w-48">
    <h4
      v-if="props.result.rating > 0"
      class="absolute top-2 right-1.5 rounded-xl bg-emerald-50 px-2.5 py-0.5 text-[13px] font-semibold tracking-tight text-[#05a876]"
    >
      {{ props.result.rating.toPrecision(2) }}
    </h4>
    <img
      alt=""
      :src="props.result.image ?? defaultImage"
      class="h-full w-full rounded object-cover ring-2 ring-emerald-500 ring-opacity-0 ring-offset-2 transition-all group-hover:ring-opacity-100"
      :class="{
        'ring-opacity-100': props.result.selected
      }"
    />
  </div>
  <div>
    <div class="flex items-start justify-between">
      <div>
        <h2 class="mt-2 font-semibold">
          {{ props.result.name }}
        </h2>
        <h3 class="text-[13px] text-gray-500">
          {{ props.result.address.street }} {{ props.result.address.apartment }}
        </h3>
        <div class="flex items-center space-x-1 pt-1.5">
          <h3 class="text-lg font-semibold tracking-tight text-emerald-500">
            {{ props.result.price.symbol }}{{ props.result.price.amount }}
          </h3>
          <span class="font text-sm text-gray-600">/ {{ priceUnit }}</span>
        </div>
      </div>
    </div>
    <div class="mt-1.5 flex items-center space-x-2.5">
      <slot></slot>
    </div>
  </div>
</template>

<script setup>
import defaultImage from '@/assets/images/default.png'
const props = defineProps(['result', 'priceUnit'])
</script>
