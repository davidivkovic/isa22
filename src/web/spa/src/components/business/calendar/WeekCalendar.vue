<template>
  <div>
    <div class="flex space-x-8">
      <div
        id="current-calendar"
        style="height: calc(100vh - 15rem)"
        class="flex flex-1 flex-col"
      >
        <div
          ref="container"
          class="flex flex-auto flex-col overflow-auto border-b border-gray-300 bg-white"
        >
          <div
            style="width: 165%"
            class="flex max-w-full flex-none flex-col sm:max-w-none md:max-w-full"
          >
            <div
              ref="containerNav"
              class="sticky top-0 z-10 flex-none bg-white ring-1 ring-black ring-opacity-5 sm:pr-8"
            >
              <div
                class="-mr-px grid grid-cols-7 divide-x divide-gray-100 border-r border-gray-100 text-sm leading-6 text-gray-500"
              >
                <div class="col-end-1 w-14" />
                <div
                  v-for="(day, date) in days"
                  :key="date"
                  @click="selectDay(day.date)"
                  class="flex cursor-pointer items-center justify-center py-2.5 hover:bg-neutral-200"
                  :class="{ 'bg-neutral-200': selectedDay == day.date }"
                >
                  <span :class="{ 'flex items-baseline': day.isToday }"
                    >{{ day.nameOfDay }}
                    <span
                      :class="{
                        '-my-1 ml-1.5 flex h-7 w-7 items-center justify-center rounded-full bg-emerald-600 font-semibold text-white':
                          day.isToday,
                        'items-center justify-center font-semibold text-gray-900':
                          !day.isToday
                      }"
                      class=""
                    >
                      {{ day.day }}
                    </span>
                  </span>
                </div>
              </div>
            </div>
            <div class="flex flex-auto">
              <div
                class="sticky left-0 z-10 w-14 flex-none bg-white ring-1 ring-gray-100"
              />
              <div class="grid flex-auto grid-cols-1 grid-rows-1">
                <!-- Horizontal lines -->
                <div
                  class="col-start-1 col-end-2 row-start-1 grid divide-y divide-gray-100"
                  style="grid-template-rows: repeat(24, 40px)"
                >
                  <div ref="containerOffset" />
                  <div>
                    <div
                      class="sticky left-0 z-20 -mt-2.5 -ml-14 w-14 pr-2 text-right text-xs leading-5 text-gray-400"
                    >
                      1AM
                    </div>
                  </div>
                  <div>
                    <div
                      class="sticky left-0 z-20 -mt-2.5 -ml-14 w-14 pr-2 text-right text-xs leading-5 text-gray-400"
                    >
                      2AM
                    </div>
                  </div>
                  <div>
                    <div
                      class="sticky left-0 z-20 -mt-2.5 -ml-14 w-14 pr-2 text-right text-xs leading-5 text-gray-400"
                    >
                      3AM
                    </div>
                  </div>
                  <div>
                    <div
                      class="sticky left-0 z-20 -mt-2.5 -ml-14 w-14 pr-2 text-right text-xs leading-5 text-gray-400"
                    >
                      4AM
                    </div>
                  </div>
                  <div>
                    <div
                      class="sticky left-0 z-20 -mt-2.5 -ml-14 w-14 pr-2 text-right text-xs leading-5 text-gray-400"
                    >
                      5AM
                    </div>
                  </div>
                  <div>
                    <div
                      class="sticky left-0 z-20 -mt-2.5 -ml-14 w-14 pr-2 text-right text-xs leading-5 text-gray-400"
                    >
                      6AM
                    </div>
                  </div>
                  <div>
                    <div
                      class="sticky left-0 z-20 -mt-2.5 -ml-14 w-14 pr-2 text-right text-xs leading-5 text-gray-400"
                    >
                      7AM
                    </div>
                  </div>
                  <div>
                    <div
                      class="sticky left-0 z-20 -mt-2.5 -ml-14 w-14 pr-2 text-right text-xs leading-5 text-gray-400"
                    >
                      8AM
                    </div>
                  </div>
                  <div>
                    <div
                      class="sticky left-0 z-20 -mt-2.5 -ml-14 w-14 pr-2 text-right text-xs leading-5 text-gray-400"
                    >
                      9AM
                    </div>
                  </div>
                  <div>
                    <div
                      class="sticky left-0 z-20 -mt-2.5 -ml-14 w-14 pr-2 text-right text-xs leading-5 text-gray-400"
                    >
                      10AM
                    </div>
                  </div>
                  <div>
                    <div
                      class="sticky left-0 z-20 -mt-2.5 -ml-14 w-14 pr-2 text-right text-xs leading-5 text-gray-400"
                    >
                      11AM
                    </div>
                  </div>
                  <div>
                    <div
                      class="sticky left-0 z-20 -mt-2.5 -ml-14 w-14 pr-2 text-right text-xs leading-5 text-gray-400"
                    >
                      12PM
                    </div>
                  </div>
                  <div>
                    <div
                      class="sticky left-0 z-20 -mt-2.5 -ml-14 w-14 pr-2 text-right text-xs leading-5 text-gray-400"
                    >
                      1PM
                    </div>
                  </div>
                  <div>
                    <div
                      class="sticky left-0 z-20 -mt-2.5 -ml-14 w-14 pr-2 text-right text-xs leading-5 text-gray-400"
                    >
                      2PM
                    </div>
                  </div>
                  <div>
                    <div
                      class="sticky left-0 z-20 -mt-2.5 -ml-14 w-14 pr-2 text-right text-xs leading-5 text-gray-400"
                    >
                      3PM
                    </div>
                  </div>
                  <div>
                    <div
                      class="sticky left-0 z-20 -mt-2.5 -ml-14 w-14 pr-2 text-right text-xs leading-5 text-gray-400"
                    >
                      4PM
                    </div>
                  </div>
                  <div>
                    <div
                      class="sticky left-0 z-20 -mt-2.5 -ml-14 w-14 pr-2 text-right text-xs leading-5 text-gray-400"
                    >
                      5PM
                    </div>
                  </div>
                  <div>
                    <div
                      class="sticky left-0 z-20 -mt-2.5 -ml-14 w-14 pr-2 text-right text-xs leading-5 text-gray-400"
                    >
                      6PM
                    </div>
                  </div>
                  <div>
                    <div
                      class="sticky left-0 z-20 -mt-2.5 -ml-14 w-14 pr-2 text-right text-xs leading-5 text-gray-400"
                    >
                      7PM
                    </div>
                  </div>
                  <div>
                    <div
                      class="sticky left-0 z-20 -mt-2.5 -ml-14 w-14 pr-2 text-right text-xs leading-5 text-gray-400"
                    >
                      8PM
                    </div>
                  </div>
                  <div>
                    <div
                      class="sticky left-0 z-20 -mt-2.5 -ml-14 w-14 pr-2 text-right text-xs leading-5 text-gray-400"
                    >
                      9PM
                    </div>
                  </div>
                  <div>
                    <div
                      class="sticky left-0 z-20 -mt-2.5 -ml-14 w-14 pr-2 text-right text-xs leading-5 text-gray-400"
                    >
                      10PM
                    </div>
                  </div>
                  <div>
                    <div
                      class="sticky left-0 z-20 -mt-2.5 -ml-14 w-14 pr-2 text-right text-xs leading-5 text-gray-400"
                    >
                      11PM
                    </div>
                  </div>
                </div>

                <!-- Vertical lines -->
                <div
                  class="col-start-1 col-end-2 row-start-1 hidden grid-cols-7 grid-rows-1 divide-x divide-gray-100 sm:grid sm:grid-cols-7"
                >
                  <div class="col-start-1 row-span-full" />
                  <div class="col-start-2 row-span-full" />
                  <div class="col-start-3 row-span-full" />
                  <div class="col-start-4 row-span-full" />
                  <div class="col-start-5 row-span-full" />
                  <div class="col-start-6 row-span-full" />
                  <div class="col-start-7 row-span-full" />
                  <div class="col-start-8 row-span-full w-8" />
                </div>

                <!-- Events -->
                <ol
                  @click="selectedEvent = null"
                  class="col-start-1 col-end-2 row-start-1 grid grid-cols-1 sm:grid-cols-7 sm:pr-8"
                  style="grid-template-rows: repeat(120, minmax(0, 1fr))"
                >
                  <template v-for="(day, date, index) in days">
                    <li
                      v-for="event in day.events"
                      :key="event.id"
                      @click.stop="selectedEvent = event"
                      @mouseover="hoveredEvent = event"
                      @mouseleave="hoveredEvent = null"
                      class="relative mt-px flex"
                      :class="colStart[index + 1]"
                      :style="`grid-row: ${Math.max(
                        Math.floor(event.minutesStart / 12 + 1),
                        1
                      )} / span ${Math.floor(event.minutesDuration / 12 + 1)}`"
                    >
                      <div
                        :class="{
                          'bg-emerald-600': ['reservation', 'sale'].includes(
                            event.type
                          ),
                          'bg-emerald-700':
                            ['reservation', 'sale'].includes(event.type) &&
                            hoveredEvent?.id == event.id,
                          'bg-emerald-700 ring-2 ring-emerald-700 ring-offset-2':
                            ['reservation', 'sale'].includes(event.type) &&
                            selectedEvent?.id == event.id,
                          'bg-indigo-600': event.type == 'unavailable',
                          'bg-indigo-700':
                            event.type == 'unavailable' &&
                            hoveredEvent?.id == event.id,
                          'bg-indigo-700 ring-2 ring-indigo-700 ring-offset-2':
                            event.type == 'unavailable' &&
                            selectedEvent?.id == event.id,
                          '!bg-neutral-200 !text-black': isPast(
                            event.chunked ? event.originalEvent.end : event.end
                          ),
                          '!bg-neutral-300 !text-black':
                            isPast(
                              event.chunked
                                ? event.originalEvent.end
                                : event.end
                            ) && hoveredEvent?.id == event.id,
                          '!bg-neutral-300 ring-2 !ring-neutral-400 ring-offset-2':
                            isPast(
                              event.chunked
                                ? event.originalEvent.end
                                : event.end
                            ) && selectedEvent?.id == event.id
                        }"
                        class="group absolute inset-1 flex cursor-pointer flex-col overflow-y-auto rounded-lg p-2 text-xs leading-5 text-white"
                      >
                        <div class="order-1 font-medium">
                          <div v-if="event.type == 'reservation'">
                            Reservation
                          </div>
                          <div v-else-if="event.type == 'sale'">Sale</div>
                          {{ event.name }}
                        </div>
                        <p class="">
                          <time :datetime="event.date">{{ event.time }}</time>
                        </p>
                      </div>
                    </li>
                  </template>
                </ol>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="basis-80">
        <EventsSidebar
          :selected-event="selectedEvent"
          :selected-day="selectedDay"
          :selected-day-events="selectedDayEvents"
          @event-selected="e => (selectedEvent = e)"
          @delete-event="e => deleteEvent(e)"
        />
      </div>
    </div>
  </div>
  <CreateEventModal
    :isOpen="eventModalOpen"
    @modalClosed="eventModalOpen = false"
    @success="renderCalendar()"
  />
</template>

<script setup>
import { ref } from 'vue'
import {
  add,
  sub,
  format,
  parseISO,
  startOfWeek,
  isSameDay,
  isPast
} from 'date-fns'

import EventsSidebar from './EventsSidebar.vue'
import CreateEventModal from './CreateEventModal.vue'
import { useEvents } from '@/components/utility/events.js'
import api from '@/api/api.js'

defineEmits(['switchCalendar'])
const props = defineProps(['businessId', 'businessType', 'businessName'])

const today = new Date()
const currentDate = ref(new Date())
const container = ref()
const containerNav = ref()
const containerOffset = ref()
const hoveredEvent = ref()
const eventModalOpen = ref(false)

const {
  days,
  selectedDay,
  selectedDayEvents,
  selectedEvent,
  selectDay,
  chunkEvents,
  deleteEvent,
  transformEvent
} = useEvents(props.businessId, props.businessType)

const colStart = {
  1: 'col-start-1',
  2: 'col-start-2',
  3: 'col-start-3',
  4: 'col-start-4',
  5: 'col-start-5',
  6: 'col-start-6',
  7: 'col-start-7'
}

const nextWeek = async () => {
  currentDate.value = add(currentDate.value, { days: 7 })
  await renderCalendar()
}

const previousWeek = async () => {
  currentDate.value = sub(currentDate.value, { days: 7 })
  await renderCalendar()
}

const goToToday = async () => {
  currentDate.value = today
  await renderCalendar()
  selectDay(today)
}

const renderDays = weekStart => {
  days.value = {}
  for (let i = 0; i < 7; i++) {
    const date = add(weekStart, { days: i, hours: 2 })
    days.value[date.toISOString().split('T')[0]] = {
      date,
      nameOfDay: format(date, 'iii'),
      day: date.getDate(),
      isToday: today.toDateString() == date.toDateString(),
      events: []
    }
  }
}

const renderCalendar = async () => {
  const weekStart = startOfWeek(currentDate.value, { weekStartsOn: 1 })
  const [data, error] = await api.business.getCalendar(
    props.businessId,
    weekStart.toISOString(),
    add(weekStart, { days: 7 }).toISOString(),
    props.businessType
  )

  renderDays(weekStart)

  if (error) return
  data.forEach(e => {
    e.start = parseISO(e.start)
    e.end = parseISO(e.end)

    const sameDay = isSameDay(
      sub(e.start, { hours: 2 }),
      sub(e.end, { hours: 2 })
    )

    console.log(sameDay)
    if (!sameDay) {
      chunkEvents(e, days)
    } else {
      const datetime = e.start.toISOString().split('T')[0]
      days.value[datetime].events.push(
        transformEvent(datetime, {
          ...e,
          time: format(sub(e.start, { hours: 2 }), 'hh:mm a')
        })
      )
    }
  })

  console.log(days.value)
}

defineExpose({
  currentDate,
  previous: previousWeek,
  next: nextWeek,
  openEventModal: () => (eventModalOpen.value = true),
  goToToday
})

await renderCalendar()
selectDay(selectedDay.value)
</script>
