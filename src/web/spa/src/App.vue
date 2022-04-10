<script setup>
import { RouterView } from 'vue-router'
import TheHeader from './components/TheHeader.vue'
</script>

<template>
  <div class="flex h-screen w-screen flex-col">
    <TheHeader />
    <RouterView />
    <RouterView name="modal-router" />
  </div>
</template>

<style>
@font-face {
  font-family: 'Circular Std';
  src: url('/src/assets/fonts/CircularStd-Book.woff2') format('woff2');
  font-weight: 400;
  font-style: normal;
}

@font-face {
  font-family: 'Circular Std';
  src: url('/src/assets/fonts/CircularStd-Bold.woff2') format('woff2');
  font-weight: 700;
  font-style: normal;
}

@font-face {
  font-family: 'Circular Std';
  src: url('/src/assets/fonts/CircularStd-Medium.woff2') format('woff2');
  font-weight: 500;
  font-style: normal;
}

body {
  text-rendering: optimizeLegibility;
  -webkit-font-smoothing: antialiased;
  font-family: 'Circular Std', sans-serif;
}
</style>
