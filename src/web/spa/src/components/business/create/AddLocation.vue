<template>
  <form
    id="add-location-form"
    name="add-location-form"
    @submit.prevent="onSubmit()"
    class="w-96"
  >
    <h3 class="text-lg font-medium">{{ headings[$route.params.type] }}</h3>
    <h5 class="mb-5 whitespace-nowrap text-neutral-500">
      Customers will want to know the precise location
    </h5>
    <Input
      required
      id="map-location-input"
      v-model="formattedLocation"
      autocomplete="off"
      class="h-12 w-full"
      :placeholder="placeholders[$route.params.type]"
      label="Location"
    />
    <div id="google-map" class="mt-6 h-96 rounded-md"></div>
  </form>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import { formatAddress } from '@/components/utility/address.js'
import Input from '@/components/ui/Input.vue'

const props = defineProps(['address'])
const emit = defineEmits(['change'])

const headings = {
  adventure: 'Show us where the adventure starts',
  boat: 'Show us where the journey starts',
  cabin: 'Show us where the cabin is located'
}

const placeholders = {
  adventure: 'Location where the adventure starts',
  boat: 'Location where the journey starts',
  cabin: 'Location of the cabin'
}

const formattedLocation = ref(props.address ? formatAddress(props.address) : '')
const formattedAddress = ref(props.address ?? {})

const onSubmit = () => {
  console.log(formattedAddress.value)
  emit('change', {
    address: formattedAddress.value
  })
}

const getValues = () =>
  document.getElementById('add-location-form').requestSubmit()

defineExpose({
  getValues
})

if (!document.getElementById('google-maps-script')) {
  const mapsScript = document.createElement('script')
  mapsScript.setAttribute('id', 'google-maps-script')
  mapsScript.setAttribute(
    'src',
    `https://maps.googleapis.com/maps/api/js?key=${
      import.meta.env.VITE_GOOGLE_MAPS_KEY
    }&libraries=places`
  )
  mapsScript.onload = () => createMap()
  document.head.appendChild(mapsScript)
} else {
  onMounted(() => createMap())
}

const createMap = () => {
  const center = {
    lat: props.address?.latitude ?? 45.249739,
    lng: props.address?.longitude ?? 19.848263
  }
  const map = new google.maps.Map(document.getElementById('google-map'), {
    center,
    zoom: 13,
    mapTypeControl: false,
    styles: [{}]
  })

  const geocoder = new google.maps.Geocoder()

  const input = document.getElementById('map-location-input')
  const autocomplete = new google.maps.places.Autocomplete(input)
  const marker = new google.maps.Marker({
    position: new google.maps.LatLng(center.lat, center.lng),
    map: map,
    draggable: true,
    visible: props.address != null
    // icon: 'assets/images/map-marker.png',
    // size: new google.maps.Size(20, 32)
  })

  const updateInput = e => {
    geocoder.geocode(
      {
        latLng: e.latLng
      },
      (results, status) => {
        if (status != google.maps.GeocoderStatus.OK) return
        formattedLocation.value = results[0].formatted_address
        formattedAddress.value = getAddress(results)
      }
    )
  }

  google.maps.event.addListener(map, 'click', e => {
    marker.setPosition(e.latLng)
    marker.setVisible(true)
    updateInput(e)
  })

  google.maps.event.addListener(marker, 'dragend', e => updateInput(e))

  autocomplete.addListener('place_changed', () => {
    const place = autocomplete.getPlace()

    if (!place.geometry) return

    marker.setPosition(place.geometry.location)
    marker.setVisible(true)
    map.setCenter(marker.getPosition())
    map.setZoom(17)

    geocoder.geocode(
      {
        latLng: place.geometry.location
      },
      (results, status) => {
        if (status != google.maps.GeocoderStatus.OK) return
        formattedAddress.value = getAddress(results)
      }
    )
  })
}

const extractFromAddress = (address, key) => {
  return address?.address_components?.find(a => a.types.includes(key))
    ?.long_name
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
    latitude: results[0].geometry.location.lat(),
    longitude: results[0].geometry.location.lng()
  }
}
</script>

<script>
export default {
  inheritAttrs: false
}
</script>

<style>
.gmnoprint a,
.gmnoprint span,
.gm-style-cc {
  display: none;
}

.gm-svpc {
  display: none;
}

.gmnoprint div {
  background: white !important;
}

.pac-container {
  text-rendering: optimizeLegibility;
  -webkit-font-smoothing: antialiased;
  font-family: 'Circular Std', sans-serif;
  box-shadow: none;
  @apply ml-px py-1 shadow-md;
}

.pac-container::after {
  content: none;
}

.pac-item {
  @apply cursor-pointer border-none px-3 py-2 text-[13px];
}

.pac-item-selected {
  @apply bg-emerald-50 text-emerald-700;
}

.pac-matched {
  @apply text-sm;
}
</style>
