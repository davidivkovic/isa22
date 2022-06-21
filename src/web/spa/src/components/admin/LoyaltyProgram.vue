<template>
  <div class="mx-auto mb-8 max-w-4.5xl">
    <h1 class="text-2xl font-medium">Loyalty program</h1>
    <h4 class="pb-8">
      Define how many points customers and business owners earn after a
      successful reservation
    </h4>
    <form @submit.prevent="saveChanges()" class="flex items-start space-x-14">
      <div>
        <h2 class="mb-2 text-lg font-medium">Define loyalty points</h2>
        <div class="flex items-start space-x-4">
          <NumberInput
            required
            v-model="customerPoints"
            min="0"
            max="10000"
            label="Customer Points"
            class="h-11 w-24"
          />
          <NumberInput
            required
            v-model="businessOwnerPoints"
            min="0"
            max="10000"
            label="Business Owner Points"
            class="h-11 w-24"
          />
        </div>
        <h2 class="mb-2 mt-6 text-lg font-medium">Add a loyalty level</h2>
        <form @submit.prevent="addLevel()" class="w-fit">
          <Input
            required
            v-model="levelName"
            label="Level Name"
            placeholder="Enter a level name"
            class="h-11 w-52"
          />
          <div class="mt-4 flex items-center space-x-4">
            <NumberInput
              required
              v-model="pointsThreshold"
              label="Points Threshold"
              step="5"
              min="0"
              max="10000"
              class="h-11 w-28"
            />
            <NumberInput
              required
              v-model="discountPercentage"
              min="0"
              max="100"
              step="0.5"
              label="Discount %"
              class="h-11 w-20"
            />
          </div>
          <p
            class="mt-4 text-[15px] font-medium tracking-tight text-neutral-700"
          >
            Color
          </p>
          <div class="mt-2 flex justify-around">
            <div v-for="color in colors" :key="color">
              <div
                @click="selectedColor = color"
                :class="[
                  `bg-[${color}]`,
                  selectedColor == color
                    ? 'ring-neutral-800 ring-offset-1'
                    : 'ring-neutral-400',
                  'h-5 w-5 cursor-pointer rounded-full ring-1 transition hover:ring-neutral-800 hover:ring-offset-1'
                ]"
              ></div>
            </div>
          </div>
          <Button
            :disabled="levelValid"
            type="submit"
            class="mt-6 border border-neutral-300 transition hover:bg-neutral-100 disabled:text-gray-400 disabled:hover:bg-white"
          >
            Add Level
          </Button>
        </form>
      </div>
      <div>
        <div class="mt-1.5 grid grid-cols-2 gap-6">
          <div
            v-for="level in loyaltyLevels"
            :key="level.name"
            :class="[
              { 'text-white': level.color == '#000000' },
              `relative h-[140px] w-[270px] overflow-clip rounded-lg py-5 px-6 bg-[${level.color}]`
            ]"
          >
            <GiftIcon
              stroke-width="1.75"
              class="text-neutral-black absolute right-5 top-5 h-8 w-8"
            />
            <h2 class="text-lg font-semibold leading-6">{{ level.name }}</h2>
            <h2 class="font-medium">{{ level.threshold }} Points</h2>
            <p class="mt-3 w-40 text-sm font-medium leading-5">
              Earn a {{ level.discountPercentage }}% reservation discount or tax
              relief
            </p>
            <Button
              @click="deleteLevel(level)"
              :class="[
                { 'text-black': level.color == '#000000' },
                'absolute bottom-5 right-4 bg-white !py-2 text-[12.5px] font-semibold transition hover:bg-neutral-50'
              ]"
            >
              Delete
            </Button>
          </div>
        </div>
        <Button
          :disabled="!dirty || loading"
          type="submit"
          class="mt-8 ml-auto mr-0 block bg-emerald-600 text-white hover:bg-emerald-700 disabled:cursor-not-allowed disabled:bg-neutral-100 disabled:text-black"
        >
          <div v-if="loading" class="flex items-center">
            <Loader class="mr-3" />
            Processing...
          </div>
          <div v-else>Save Changes</div>
        </Button>
      </div>
    </form>
  </div>
</template>

<script setup>
import { computed, ref, watch } from 'vue'
import { GiftIcon } from 'vue-tabler-icons'
import Input from '@/components/ui/Input.vue'
import NumberInput from '@/components/ui/NumberInput.vue'
import Button from '@/components/ui/Button.vue'
import Loader from '@/components/ui/Loader.vue'
import api from '@/api/api'

// Hack so tailwind classes don't get purged away by purgecss
// eslint-disable-next-line no-unused-vars
const tailwindHack = [
  'bg-[#e5e6aa]',
  'bg-[#f1f5f9]',
  'bg-[#fffcc2]',
  'bg-[#e9fef9]',
  'bg-[#e0e7ff]',
  'bg-[#000000]'
]

const colors = [
  '#e5e6aa',
  '#f1f5f9',
  '#fffcc2',
  '#e9fef9',
  '#e0e7ff',
  '#000000'
]

const taxPercentage = ref(20)
const customerPoints = ref(0)
const businessOwnerPoints = ref(0)

const loyaltyLevels = ref([
  // {
  //   name: 'Bronze',
  //   threshold: 20,
  //   discountPercentage: 2.5,
  //   color: '#e5e6aa'
  // },
  // {
  //   name: 'Silver',
  //   threshold: 60,
  //   discountPercentage: 5,
  //   color: '#f1f5f9'
  // },
  // {
  //   name: 'Gold',
  //   threshold: 100,
  //   discountPercentage: 10,
  //   color: '#fffcc2'
  // },
  // {
  //   name: 'Platinum',
  //   threshold: 180,
  //   discountPercentage: 15,
  //   color: '#e9fef9'
  // },
  // {
  //   name: 'Diamond',
  //   threshold: 340,
  //   discountPercentage: 30,
  //   color: '#e0e7ff'
  // },
  // {
  //   name: 'Ultra',
  //   threshold: 700,
  //   discountPercentage: 60,
  //   color: '#000000'
  // }
])
const levelName = ref('')
const pointsThreshold = ref(0)
const discountPercentage = ref(0)
const selectedColor = ref(colors[0])

const loading = ref(false)
const dirty = ref(false)

const [data, error] = await api.finances.getFinanceParams()
if (!error) {
  loyaltyLevels.value = data.loyaltyLevels
  loyaltyLevels.value.sort((a, b) => a.threshold - b.threshold)
  customerPoints.value = data.finance.customerPoints
  businessOwnerPoints.value = data.finance.businessOwnerPoints
  taxPercentage.value = data.finance.taxPercentage
}

watch(
  [() => loyaltyLevels.value.length, customerPoints, businessOwnerPoints],
  () => (dirty.value = true)
)

const levelValid = computed(
  () =>
    loyaltyLevels.value.find(
      l => l.name.toLowerCase() == levelName.value.toLowerCase()
    ) || levelName.value == ''
)

const addLevel = () => {
  const newLevel = {
    name: levelName.value,
    threshold: pointsThreshold.value,
    discountPercentage: discountPercentage.value,
    color: selectedColor.value
  }

  const index = loyaltyLevels.value.findIndex(
    l => l.threshold >= pointsThreshold.value
  )
  if (index == -1) {
    loyaltyLevels.value.push(newLevel)
  } else {
    loyaltyLevels.value.splice(index, 0, newLevel)
  }

  levelName.value = ''
  pointsThreshold.value = 0
  discountPercentage.value = 0
  selectedColor.value = colors[0]
}

const deleteLevel = level => {
  loyaltyLevels.value = loyaltyLevels.value.filter(l => l.name != level.name)
}

const saveChanges = async () => {
  loading.value = true
  const [, error] = await api.finances.setFinanceParams({
    loyaltyLevels: loyaltyLevels.value,
    finance: {
      taxPercentage: taxPercentage.value,
      customerPoints: customerPoints.value,
      businessOwnerPoints: businessOwnerPoints.value
    }
  })
  loading.value = false
  if (!error) {
    dirty.value = false
  }
}
</script>
