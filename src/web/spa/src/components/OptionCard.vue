<template>
    <div class="rounded-tl-lg rounded-tr-lg sm:rounded-tr-none relative group bg-white p-6 focus-within:ring-2 focus-within:ring-inset focus-within:ring-indigo-500">
            <div>
            <span class="rounded-lg inline-flex p-3 bg-emerald-50 text-emerald-700 ring-4 ring-white">
            <slot></slot>
            </span>
            </div>
            <div class="mt-8">
            <h3 class="text-lg font-medium">
                <RouterLink :to="{ name:props.routeName }">
                <!-- Extend touch target to entire panel -->
                <span class="absolute inset-0" aria-hidden="true"></span>
                {{ props.name }}
                </RouterLink>
            </h3>
            <p class="mt-2 text-base text-gray-500">{{props.description}}</p>
            </div>
            <span class="pointer-events-none absolute top-6 right-6 text-gray-300 group-hover:text-gray-400" aria-hidden="true">
            <ArrowUpRightIcon class="w-6 h-9"/>
            </span>
    </div>
</template>

<script setup>
import {RouterLink} from 'vue-router'
import { ArrowUpRightIcon } from 'vue-tabler-icons';

const props = defineProps({
    name: String,
    description: String,
    routeName: String
})
</script>