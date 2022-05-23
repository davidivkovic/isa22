<script setup>
import { Suspense } from 'vue'
import { RouterView } from 'vue-router'
import TheHeader from './components/header/TheHeader.vue'
</script>

<template>
  <div class="flex h-screen flex-col">
    <Suspense>
      <TheHeader />
    </Suspense>
    <RouterView v-slot="{ Component }">
      <Suspense>
        <div class="flex-1">
          <Component :is="Component" />
        </div>
      </Suspense>
    </RouterView>

    <RouterView name="modal-router" v-slot="{ Component }">
      <Suspense>
        <div>
          <Component :is="Component" />
        </div>
      </Suspense>
    </RouterView>
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
.pac-container {
  height: 150px;
}
</style>
