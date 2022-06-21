<template>
  <div class="flex">
    <div id="current-calendar" class="flex-1 bg-white">
      <div
        style="height: calc(100vh - 15rem)"
        class="mr-5 overflow-y-scroll pl-1 pr-1"
      >
        <div
          class="mr-2 grid max-w-3xl grid-cols-1 gap-x-6 gap-y-8 py-6 sm:grid-cols-2 xl:max-w-none xl:grid-cols-3"
        >
          <section
            v-for="month in months"
            :key="month.name"
            class="text-center"
          >
            <h2 class="font-semibold text-gray-900">{{ month.name }}</h2>
            <div class="mt-2 grid grid-cols-7 text-xs leading-6 text-gray-500">
              <div>M</div>
              <div>T</div>
              <div>W</div>
              <div>T</div>
              <div>F</div>
              <div>S</div>
              <div>S</div>
            </div>
            <div
              class="isolate mt-2 grid grid-cols-7 gap-px rounded-lg bg-gray-200 text-sm ring-1 ring-gray-200"
            >
              <button
                v-for="(day, dayIdx) in month.days"
                :key="day.date"
                @click="selectDay(day.datetime)"
                :class="[
                  day.isCurrentMonth
                    ? 'bg-white text-gray-900'
                    : 'bg-gray-50 text-gray-400',
                  dayIdx === 0 && 'rounded-tl-lg',
                  dayIdx === 6 && 'rounded-tr-lg',
                  dayIdx === month.days.length - 7 && 'rounded-bl-lg',
                  dayIdx === month.days.length - 1 && 'rounded-br-lg',
                  selectedDay == day.datetime && 'bg-neutral-200',
                  'py-1.5 hover:bg-gray-100 focus:z-10'
                ]"
                type="button"
              >
                <time
                  :datetime="day.date"
                  :class="[
                    day.isToday && 'bg-emerald-600 font-semibold text-white',
                    'mx-auto flex h-7 w-7 items-center justify-center rounded-full'
                  ]"
                  >{{ day.date.split('-').pop().replace(/^0/, '') }}</time
                >
              </button>
            </div>
          </section>
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
        @event-reported="eventReported()"
      />
    </div>
  </div>
  <CreateEventModal
    :isOpen="eventModalOpen"
    @modalClosed="eventModalOpen = false"
    @success="selectDay(selectedDay)"
  />
</template>

<script setup>
import { ref } from 'vue'
import {
  format,
  parseISO,
  sub,
  add,
  startOfYear,
  endOfYear,
  eachMonthOfInterval,
  lastDayOfMonth,
  setDate,
  startOfDay,
  endOfDay
} from 'date-fns'
import { useEvents } from '@/components/utility/events.js'
import EventsSidebar from './EventsSidebar.vue'
import CreateEventModal from '@/components/business/calendar/CreateEventModal.vue'
import api from '@/api/api.js'

const props = defineProps(['businessId', 'businessType', 'businessName'])

const today = new Date()
const currentDate = ref(today)
const eventModalOpen = ref(false)
const months = ref([])

const {
  selectedDay,
  selectedDayEvents,
  selectedEvent,
  transformEvent,
  deleteEvent
} = useEvents(props.businessId, props.businessType)

const eventReported = () => {
  if (selectedEvent.value.chunked) {
    selectedEvent.value.originalEvent.reported = true
  } else {
    selectedEvent.value.reported = true
  }
}

const selectDay = async date => {
  const start = add(startOfDay(date), { hours: 2 })
  const end = add(endOfDay(date), { hours: 2 })
  const [data, error] = await api.business.getCalendar(
    props.businessId,
    start,
    end,
    props.businessType
  )
  selectedDay.value = date
  selectedDayEvents.value = []
  if (!error) {
    data.forEach(e => {
      e.start = parseISO(e.start)
      e.end = parseISO(e.end)
      selectedDayEvents.value.push(
        transformEvent(e.start.toISOString().split('T')[0], e)
      )
    })
  }
}

const previousYear = () => {
  currentDate.value = sub(currentDate.value, { years: 1 })
  renderCalendar()
}

const nextYear = () => {
  currentDate.value = add(currentDate.value, { years: 1 })
  renderCalendar()
}

const goToToday = async () => {
  if (currentDate.value.getFullYear() != today.getFullYear()) {
    currentDate.value = today
    renderCalendar()
  }
  await selectDay(today)
}

const renderCalendar = () => {
  months.value = []
  const start = startOfYear(currentDate.value)
  const end = endOfYear(currentDate.value)
  const intervalMonths = eachMonthOfInterval({ start, end })

  intervalMonths.forEach(m => {
    const month = {
      name: format(m, 'MMMM'),
      days: []
    }

    const currentMonthLastDay = lastDayOfMonth(m).getDate()
    const previousMonthLastDay = sub(m, { days: 1 })
    const nextMonthFirstDay = add(m, { months: 1 })
    const previousMonthLastDayDate = previousMonthLastDay.getDate()
    const currentMonthFirstDayIndex = (m.getDay() + 6) % 7
    const nextMonthDays = 42 - currentMonthFirstDayIndex - currentMonthLastDay

    for (let i = currentMonthFirstDayIndex; i > 0; i--) {
      const date = setDate(
        previousMonthLastDay,
        previousMonthLastDayDate - i + 2
      )
      month.days.push({
        date: date.toISOString().split('T')[0],
        datetime: sub(date, { hours: 2 }),
        events: []
      })
    }

    for (let i = 1; i <= currentMonthLastDay; i++) {
      const date = setDate(m, i + 1)
      month.days.push({
        date: date.toISOString().split('T')[0],
        datetime: sub(date, { hours: 2 }),
        isCurrentMonth: true,
        isToday:
          sub(today, { hours: 2 }).toDateString() ===
          sub(date, { hours: 2 }).toDateString(),
        events: []
      })
    }

    for (let i = 1; i <= nextMonthDays; i++) {
      const date = setDate(nextMonthFirstDay, i + 1)
      month.days.push({
        date: date.toISOString().split('T')[0],
        datetime: sub(date, { hours: 2 }),
        events: []
      })
    }
    months.value.push(month)
  })
}

defineExpose({
  currentDate,
  previous: previousYear,
  next: nextYear,
  openEventModal: () => (eventModalOpen.value = true),
  goToToday
})

renderCalendar()
await selectDay(today)
</script>
