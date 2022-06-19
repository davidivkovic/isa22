<template>
  <div>
    <div class="mb-6">
      <div class="flex items-center space-x-2">
        <h2 class="text-lg font-medium">Selected event</h2>
        <div
          v-if="selectedEvent"
          class="h-2.5 w-2.5 rounded-full"
          :class="{
            'bg-emerald-600': ['reservation', 'sale'].includes(
              selectedEvent.type
            ),
            'bg-indigo-600': selectedEvent.type == 'unavailable',
            '!bg-neutral-300': isPast(
              selectedEvent.chunked
                ? selectedEvent.originalEvent.end
                : selectedEvent.end
            )
          }"
        ></div>
      </div>
      <h4 v-if="!selectedEvent" class="text-neutral-600">
        Please select an event to show its' details
      </h4>
      <div
        v-if="selectedEvent"
        class="mt-3 rounded-lg border px-4 py-3 text-[15px]"
      >
        <div class="flex items-center justify-between">
          <div>
            <div v-if="selectedEvent.type == 'unavailable'" class="font-medium">
              {{ selectedEvent.name }}
            </div>
            <div
              v-else-if="selectedEvent.type == 'reservation'"
              class="font-medium"
            >
              Reservation
            </div>
            <div v-else class="font-medium">Sale</div>
          </div>
          <Button
            @click="$emit('deleteEvent', selectedEvent)"
            v-if="
              selectedEvent.type == 'unavailable' ||
              (selectedEvent.type == 'sale' && selectedEvent.name == '')
            "
            class="-mr-2 !px-3 !py-1 font-semibold text-red-600 hover:bg-red-50"
          >
            Delete
          </Button>
        </div>
        <div class="flex items-center space-x-4">
          <div>
            <div class="font-medium">
              {{ format(getEventStart(selectedEvent), 'MMM dd') }}
            </div>
            <div class="text-sm text-neutral-600">
              {{ format(getEventStart(selectedEvent), 'hh:mm a') }}
            </div>
          </div>
          <div class="text-sm text-neutral-600">&rarr;</div>
          <div>
            <div class="font-medium">
              {{ format(getEventEnd(selectedEvent), 'MMM dd') }}
            </div>
            <div class="text-sm text-neutral-600">
              {{ format(getEventEnd(selectedEvent), 'hh:mm a') }}
            </div>
          </div>
        </div>
        <div
          v-if="selectedEvent.type == 'sale' && selectedEvent.name == ''"
          class="mt-1 text-sm font-medium"
        >
          Not yet booked
        </div>
        <div
          v-else-if="['reservation', 'sale'].includes(selectedEvent.type)"
          class="mt-1 flex items-end justify-between"
        >
          <div>
            <p class="text-[13.5px] font-medium">Customer</p>
            <p
              class="cursor-pointer text-[15px] font-medium text-indigo-600 underline hover:text-purple-700"
            >
              {{ selectedEvent.name }}
            </p>
          </div>
          <button
            @click="report()"
            :title="!selectedEvent.reported ? 'Write a report' : 'Reported'"
            v-if="
              !(selectedEvent.chunked
                ? selectedEvent.originalEvent.reported
                : selectedEvent.reported) &&
              isPast(
                sub(
                  selectedEvent.chunked
                    ? selectedEvent.originalEvent.end
                    : selectedEvent.end,
                  { hours: 2 }
                )
              )
            "
            class="-mr-1 rounded-lg p-1.5 hover:bg-neutral-100"
          >
            <ReportSearchIcon stroke-width="1.75" class="pointer-events-none" />
          </button>
          <Button
            v-if="
              isPast(
                sub(
                  selectedEvent.chunked
                    ? selectedEvent.originalEvent.start
                    : selectedEvent.start,
                  { hours: 2 }
                )
              ) &&
              isFuture(
                sub(
                  selectedEvent.chunked
                    ? selectedEvent.originalEvent.end
                    : selectedEvent.end,
                  { hours: 2 }
                )
              )
            "
            @click="renewReservation(selectedEvent)"
            class="bg-emerald-600 !px-4 !py-2 text-white"
          >
            Renew Reservation
          </Button>
        </div>
      </div>
    </div>
    <div>
      <h2 class="mb-3 text-lg font-medium">
        Showing schedule for {{ format(selectedDay, 'MMM dd') }}
      </h2>
      <div v-if="selectedDayEvents.length == 0" class="text-neutral-700">
        No events for selected date
      </div>
      <div v-else class="space-y-3 overflow-y-auto">
        <div
          v-for="event in selectedDayEvents"
          :key="event.id"
          @click="selectEvent(event)"
          class="cursor-pointer rounded-lg border px-4 py-3 text-[15px] hover:border-neutral-700"
          :class="{ 'border-neutral-700': selectedEvent?.id == event.id }"
        >
          <div class="flex items-center justify-between">
            <div>
              <div v-if="event.type == 'unavailable'" class="font-medium">
                {{ event.name }}
              </div>
              <div v-else-if="event.type == 'reservation'" class="font-medium">
                Reservation
              </div>
              <div v-else class="font-medium">Sale</div>
            </div>
          </div>
          <div class="flex items-center space-x-4">
            <div>
              <div class="font-medium">
                {{ format(getEventStart(event), 'MMM dd') }}
              </div>
              <div class="text-sm text-neutral-600">
                {{ format(getEventStart(event), 'hh:mm a') }}
              </div>
            </div>
            <div class="text-sm text-neutral-600">&rarr;</div>
            <div>
              <div class="font-medium">
                {{ format(getEventEnd(event), 'MMM dd') }}
              </div>
              <div class="text-sm text-neutral-600">
                {{ format(getEventEnd(event), 'hh:mm a') }}
              </div>
            </div>
          </div>
          <div
            v-if="event.type == 'sale' && event.name == ''"
            class="mt-1 text-sm font-medium"
          >
            Not yet booked
          </div>
          <div
            v-else-if="['reservation', 'sale'].includes(event.type)"
            class="mt-1 flex items-end justify-between"
          >
            <div>
              <p class="text-[13.5px] font-medium">Customer</p>
              <p class="text-[15px]">
                {{ event.name }}
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
  <CreateComplaint
    @modalClosed="isReportModalOpen = false"
    :isOpen="isReportModalOpen"
    type="report"
    :reservationId="selectedEvent?.id"
    :user="selectedEvent?.name"
    :businessType="businessType"
    @success="$emit('eventReported')"
  />
  <CreateSaleModal
    v-if="business"
    :is-renewal="true"
    :businessId="business.id"
    :services="business.services"
    :pricePerUnit="business.pricePerUnit.amount"
    :businessType="businessType"
    :is-open="isRenewModalOpen"
    :customer-id="customerId"
    @modal-closed="isRenewModalOpen = false"
  />
</template>

<script setup>
import { ref } from 'vue'
import { sub, format, isPast, isFuture } from 'date-fns'
import { ReportSearchIcon } from 'vue-tabler-icons'
import Button from '@/components/ui/Button.vue'
import CreateComplaint from '@/components/reservations/CreateComplaint.vue'
import { businessType } from '@/stores/userStore'

import CreateSaleModal from '../view/CreateSaleModal.vue'
import api from '@/api/api'

const props = defineProps(['selectedEvent', 'selectedDayEvents', 'selectedDay'])
const emit = defineEmits(['eventSelected', 'deleteEvent', 'eventReported'])

const isReportModalOpen = ref(false)
const isRenewModalOpen = ref(false)
const report = () =>
  !props.selectedEvent.reported && (isReportModalOpen.value = true)
const business = ref()
const customerId = ref()

const selectEvent = event => {
  const selectedEvent = props.selectedEvent?.id == event.id ? null : event
  customerId.value = selectedEvent?.userId
  emit('eventSelected', selectedEvent)
}

const getEventStart = e =>
  sub(e.chunked ? e.originalEvent.start : e.start, { hours: 2 })
const getEventEnd = e =>
  sub(e.chunked ? e.originalEvent.end : e.end, { hours: 2 })

const renewReservation = async event => {
  const [data, error] = await api.business.get(
    event.businessId,
    businessType.value,
    {}
  )
  if (!error) {
    isRenewModalOpen.value = true
    business.value = data
  }
}
</script>
