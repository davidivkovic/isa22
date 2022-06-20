<template>
  <label :for="props.name" class="group relative cursor-pointer">
    <input
      required
      :type="props.inputType"
      :id="props.name"
      placeholder=""
      v-model="location"
      :name="props.name"
      class="focus-within::outline-none peer ml-8 w-full border-0 py-0 text-[1.075rem] font-semibold valid:translate-y-3 valid:text-black focus-within:translate-y-3 focus-within:ring-0 focus:outline-none"
    />
    <div
      class="absolute top-0 flex cursor-pointer items-center space-x-2 text-[22px] transition-all group-focus-within:-translate-y-4 group-focus-within:text-base peer-valid:-translate-y-4 peer-valid:text-base"
    >
      <div class="-mt-0.5 text-gray-400">
        <slot></slot>
      </div>
      <p class="items-center text-black" style="font-size: inherit">
        {{ props.name }}
      </p>
    </div>
    <div
      class="mt-1 ml-8 whitespace-nowrap text-sm text-gray-400 transition-all group-focus-within:translate-y-2 peer-valid:translate-y-2"
    >
      <p class="">{{ props.description }}</p>
    </div>
  </label>
</template>

<script setup>
import { onMounted, ref } from 'vue'
const props = defineProps({
  name: String,
  description: String
})

const emit = defineEmits(['valueChanged'])

const location = ref('')
const city = ref('')
const country = ref('')

const emitChange = () => {
  emit('valueChanged', { country: country.value, city: city.value })
}

if (!document.getElementById('google-maps-script')) {
  const mapsScript = document.createElement('script')
  mapsScript.setAttribute('id', 'google-maps-script')
  mapsScript.setAttribute(
    'src',
    `https://maps.googleapis.com/maps/api/js?key=${
      import.meta.env.VITE_GOOGLE_MAPS_KEY
    }&libraries=places&language=en`
  )
  mapsScript.onload = () => createField()
  document.head.appendChild(mapsScript)
} else {
  onMounted(() => createField())
}

const createField = () => {
  const geocoder = new window.google.maps.Geocoder()
  const input = document.getElementById(props.name)
  const options = {
    types: ['(regions)']
  }
  const autocomplete = new window.google.maps.places.Autocomplete(
    input,
    options
  )
  autocomplete.addListener('place_changed', () => {
    const place = autocomplete.getPlace()
    geocoder.geocode(
      {
        latLng: place.geometry.location
      },
      (results, status) => {
        if (status != window.google.maps.GeocoderStatus.OK) return
        const address = getAddress(results)
        city.value = address.city
        country.value = address.country
        //location.value = `${address.city}, ${address.country}`
        emitChange()
      }
    )
  })
}

const getAddress = results => {
  const address = results.find(r =>
    r.address_components.find(c => c.types.includes('postal_code'))
  )
  return {
    postalCode: extractFromAddress(address, 'postal_code'),
    country: extractFromAddress(results[0], 'country'),
    city: extractFromAddress(results[0], 'locality'),
    street: extractFromAddress(results[0], 'route'),
    apartment: extractFromAddress(results[0], 'street_number'),
    region: extractFromAddress(results[0], 'administrative_area_level_1'),
    latitude: results[0].geometry.location.lat(),
    longitude: results[0].geometry.location.lng()
  }
}

const extractFromAddress = (address, key) => {
  return address?.address_components?.find(a => a.types.includes(key))
    ?.long_name
}
</script>

<style scoped>
.pac-container {
  margin-top: 500px;
}
</style>
