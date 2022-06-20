<template>
  <div class="flex space-x-3">
    <div class="-mt-1 flex flex-col justify-between pb-9">
      <TransitionGroup
        enter-active-class="duration-500 ease-in-out"
        enter-from-class="opacity-0"
        enter-to-class="opacity-100"
        leave-active-class="hidden"
      >
        <div
          v-for="(step, index) in data.steps"
          :key="step"
          :style="{ transitionDelay: 80 * index + 'ms' }"
          class="text-[13.5px] font-medium"
        >
          {{ step }}
        </div>
      </TransitionGroup>
    </div>
    <div>
      <div class="flex h-32 items-end space-x-3">
        <TransitionGroup name="bars">
          <div
            v-for="(point, index) in data.values"
            :key="point.year"
            :style="{
              transitionDelay: 75 * index + 'ms',
              height: (point.total / data.max) * 100 + '%'
            }"
            :class="isMoney ? 'bars-gradient-green' : 'bars-gradient-orange'"
            class="h-full w-4 cursor-pointer rounded hover:opacity-90"
          ></div>
        </TransitionGroup>
      </div>
      <div class="-ml-1.5 mt-1 flex">
        <TransitionGroup
          enter-active-class="duration-500 ease-in-out"
          enter-from-class="opacity-0"
          enter-to-class="opacity-100"
          leave-active-class="hidden"
        >
          <div
            v-for="(point, index) in data.values"
            :key="point.year"
            class="w-7 text-center text-xs font-medium"
            :style="{ transitionDelay: 50 * index + 'ms' }"
          >
            <div v-if="point?.week">W{{ point.week }}</div>
            <div v-else-if="point?.month">
              {{
                format(
                  parseISO(
                    `${point.year}-${point.month >= 10 ? '' : '0'}${
                      point.month
                    }-01`
                  ),
                  'MMM'
                )
              }}
            </div>
            <div v-else>
              {{ point.year }}
            </div>
          </div>
        </TransitionGroup>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'
import { format, parseISO } from 'date-fns'

const props = defineProps({
  points: Array,
  isMoney: Boolean
})

const data = ref({
  max: 0,
  steps: [],
  values: []
})

watch(
  () => props.points.length,
  () => {
    if (props.points.length == 0) return
    props.points.forEach(p => (p.total = Number(p.total)))

    const max = Math.max(...props.points.map(r => r.total))
    const min = Math.min(...props.points.map(r => r.total))

    let range = max - min
    if (range == 0) range = max
    const exponent = Math.round(Math.log10(range))
    const magnitude = Math.pow(10, exponent)
    const step = ((magnitude / 5) * Math.round((max / magnitude) * 10)) / 10
    data.value = {
      max: 0,
      steps: [],
      values: []
    }
    setTimeout(
      () => {
        data.value = {
          max,
          steps: [
            ...new Set(
              [...Array(5).keys()]
                .reverse()
                .map(s => (s + 1) * step)
                .map(s =>
                  props.isMoney
                    ? s.toLocaleString('en-US', {
                        style: 'currency',
                        currency: 'USD',
                        maximumFractionDigits: 0
                      })
                    : Math.round(s)
                )
            )
          ],
          values: props.points
        }
      },
      data.value.values.length > 0 ? data.value.values.length * 65 : 0
    )
  }
)
</script>

<style scoped>
.bars-gradient-orange {
  background: linear-gradient(
    to top,
    #a855f7 20px,
    rgb(129 140 248),
    rgb(153 246 228)
  );
}

.bars-gradient-green {
  background: linear-gradient(to top, #10b981, #86efac);
}

.bars-leave-active {
  display: none;
}
.bars-enter-active {
  transition: all 0.55s cubic-bezier(0.9, 0.12, 0.42, 0.82);
  transform: scaleY(1);
  transform-origin: bottom;
}

.bars-enter-from,
.bars-leave-to {
  transform: scaleY(0);
  transform-origin: bottom;
}
</style>
