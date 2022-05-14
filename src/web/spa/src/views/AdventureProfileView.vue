<template>
  <div v-if="adventure">
    <EntityProfileView :entity="adventure">
      <template v-slot:tag>
        <Tag>fishing adventure</Tag>
      </template>
      <template v-slot:biography>
        <div class="mt-10 w-full space-y-3 border-t p-8">
          <h2 class="text-2xl font-semibold">
            Organized by {{ adventure.owner.fullName }}
          </h2>
          <div class="flex items-center space-x-10 !py-5">
            <img
              src="https://naulobazar.com/public/uploads/products/thumbnail/9MrG8gCrSVHxIm31iXwAysKswOeenUHxgjOlCQ4L.png"
              class="h-32 w-32"
            />
            <p>{{ adventure.biography }}</p>
          </div>
        </div>
      </template>
      <template v-slot:equipment>
        <h3 class="!mt-10 text-lg font-semibold">Equipment</h3>
        <h3 v-if="adventure.fishingEquipment.length !== 0">
          Some equipment is included in the offer.
        </h3>
        <h3 v-else>No fishing equipment is included in this offer.</h3>
        <div class="!mt-1 grid w-2/3 grid-cols-2 gap-y-1">
          <div
            v-for="equipment in adventure.fishingEquipment"
            :key="equipment"
            class="flex w-fit items-center space-x-2"
          >
            <CheckIcon class="h-5 w-5 text-emerald-500" />
            <p>{{ equipment }}</p>
          </div>
        </div>
      </template>
    </EntityProfileView>
  </div>
</template>
<script setup>
import { ref } from 'vue'
import api from '../api/api.js'
import EntityProfileView from './EntityProfileView.vue'
import Tag from '../components/ui/Tag.vue'
import { CheckIcon } from 'vue-tabler-icons'
import { useRoute } from 'vue-router'

const adventure = ref()
const route = useRoute()

const fetchAdventure = async () => {
  const [data] = await api.business.get(route.params.id, 'adventures')
  if (data) adventure.value = data
}

fetchAdventure()
</script>
