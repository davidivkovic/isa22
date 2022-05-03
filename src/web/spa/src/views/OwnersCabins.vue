<template>
 <div class="justify-between">
     <div class="flex mx-auto items-center justify-between py-6 max-w-6xl">
         <p>Search part</p>
     </div>
     <br>
    <div class="mx-auto max-w-[70%] bg-white shadow overflow-hidden sm:rounded-md">
        <ul role="list" class="divide-y divide-gray-200">
            <li  v-for="result in results"
                :key="result.id"
                class="group space-y-3.5 max-h-[12rem] h-[12rem]">
                <div class="flex items-center px-4 py-4 sm:px-6">
                    <div class="min-2-0 flex-1 flex">
                        <div class="relative h-36 w-48 ">
                            <h4 class="absolute top-2 right-1.5 rounded-xl bg-emerald-50 px-2.5 text-[13px] font-semibold tracking-tight text-emerald-300">
                                {{result.rating.toPrecision(2)}}
                            </h4>
                                <img
                                alt=""
                                :src="result.image"
                                class="h-full w-full rounded object-cover ring-2 ring-emerald-500"
                                :class="{'ring-opacity-100': result.selected}"/>
                        </div>
                        <div class="ml-3 flex-1 px-4">
                            <div class="w-full flex justify-between">
                                <div class="mb-20 content-start">
                                    <div>
                                        <h1 class="text-[20px] font-semibold">
                                            {{result.name}}
                                        </h1>
                                        <h2 class="text-[17px] text-gray-500">
                                            {{result.address.street}} {{result.address.apartment}}
                                        </h2>
                                    </div>
                                    <div class="max-w-md p-1">
                                        <p class="text-[15px] max-h-[5.6rem] text-ellipsis overflow-clip">{{result.description}}</p>
                                    </div>
                                </div>
                                <div class="space-x-2 space-y- items-end mt-8">
                                    <div class="ml-[8px] flex justify-left space-x-1 text-neutral-500">
                                        <h4 class="font- text-[13px] font-mono">Rooms: &nbsp;{{result.rooms}} </h4>
                                        <HotelServiceIcon stroke-width="2" class="h-4 w-4 pb-px text-emerald-500"/>
                                    </div>
                                    <div class="flex justify-left space-x-1 text-neutral-500">
                                        <h4 class="font- text-[13px] font-mono">Beds: &nbsp;&nbsp;{{ result.beds }} </h4>
                                        <BedIcon stroke-width="2" class="h-4 w-4 pb-px text-emerald-500" />
                                    </div>
                                    <div class="flex justify-left space-x-1 text-neutral-500">
                                        <h4 class="font- text-[13px] font-mono">People: {{ result.people }} </h4>
                                        <UserIcon stroke-width="2" class="h-4 w-4 pb-px text-emerald-500" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="space-y-2 pt-1.5 mt-4 w-[13%]">
                            <div class="flex items-center justify-center space-x-1 pt-1.5">
                                <h3 class="text-lg font-semibold tracking-tight text-emerald-500">
                                    {{result.price.symbol}} {{result.price.amount}}
                                </h3>
                                <span class="font text-sm text-gray-600">/night</span>
                            </div>
                            <div class="items-center justify-center flex space-x-1">
                                <h2 class="text-[11px]">
                                    Cancellation fee 
                                </h2>
                                <h3 class="text-[10px] font-bold">
                                    {{result.price.symbol}} {{result.cancellationFee}}
                                </h3>
                            </div>
                            <Button class="w-full bg-emerald-500 text-white text-center">
                                Show
                            </Button>
                        </div>
                    </div>
                </div> 
            </li>
        </ul>
    </div>
    
 </div>
</template>

<script setup>
import {
  UserIcon,
  BedIcon,
  HotelServiceIcon
} from 'vue-tabler-icons'
import Button from '../components/ui/Button.vue';

const results = [{
    id: 0,
    name: "Vikendica na Kopaoniku",
    rating: 4.123,
    image: "https://cf.bstatic.com/xdata/images/hotel/max1024x768/237771595.jpg?k=74b6fd4d57358758e4a2bd4e44e067009229efa17dab6d64deddcbce7e8e8a93&o=&hp=1",
    address: {
        street: "Starzilovska",
        apartment: "15",
    longitude: 43.7389411,
    latitude: 19.698573
    },
    price: {
        symbol: "RSD",
        amount: 150000
    },
    rooms: 30,
    beds: 30,
    people: 15,
    selected: false,
    longitude: 43.7389411,
    latitude: 19.698573,
    cancellationFee: 30000,
    description: "Objekat MountainView19 Zoned II Kopaonik nudi pogled na planinu, a nalazi se na Kopaoniku, na 2,5 km od Ski-centra Kopaonik. Gosti koji borave u ovom apartmanu mogu da koriste potpuno opremljenu kuhinju i balkondasssssssssssssssss ssssssssssssssssdasdsadasdasdasdasdadasdsadasdasdsadsadsadasdasdsadsadsadadsdsasadsadasdsad cvxcvasdedawqsdDASDASDSADSADS ADSADSADSASDASADSADSDASDASDASDASDADSASD DASDASDASDADSASDFADSADSADSADSADSADSASDADSASDADSASSDADWWQQDWASDQAWWDQWADASDSADSA DSADSADAS DSAD ASD AS DAS DAS ",
},{
    id: 0,
    name: "kucica",
    rating: 4.123,
    image: "https://cf.bstatic.com/xdata/images/hotel/max1024x768/237771595.jpg?k=74b6fd4d57358758e4a2bd4e44e067009229efa17dab6d64deddcbce7e8e8a93&o=&hp=1",
    address: {
        street: "ulica",
        apartment: "apartman",
    longitude: 43.7389411,
    latitude: 19.698573
    },
    price: {
        symbol: "RSD",
        amount: 100
    },
    rooms: 3,
    beds: 3,
    people: 5,
    selected: false,
    longitude: 43.7389411,
    latitude: 19.698573,
    cancellationFee: 30,
    description: "Opis vikendice",
},{
    id: 0,
    name: "kucica",
    rating: 4.123,
    image: "https://cf.bstatic.com/xdata/images/hotel/max1024x768/237771595.jpg?k=74b6fd4d57358758e4a2bd4e44e067009229efa17dab6d64deddcbce7e8e8a93&o=&hp=1",
    address: {
        street: "ulica",
        apartment: "apartman",
    longitude: 43.7389411,
    latitude: 19.698573
    },
    price: {
        symbol: "RSD",
        amount: 100
    },
    rooms: 3,
    beds: 3,
    people: 5,
    selected: false,
    longitude: 43.7389411,
    latitude: 19.698573,
    cancellationFee: 30,
    description: "Opis vikendice"
},{
    id: 0,
    name: "kucica",
    rating: 4.123,
    image: "https://cf.bstatic.com/xdata/images/hotel/max1024x768/237771595.jpg?k=74b6fd4d57358758e4a2bd4e44e067009229efa17dab6d64deddcbce7e8e8a93&o=&hp=1",
    address: {
        street: "ulica",
        apartment: "apartman",
    longitude: 43.7389411,
    latitude: 19.698573
    },
    price: {
        symbol: "RSD",
        amount: 100
    },
    rooms: 3,
    beds: 3,
    people: 5,
    selected: false,
    longitude: 43.7389411,
    latitude: 19.698573,
    cancellationFee: 30,
    description: "Objekat MountainView19 Zoned II Kopaonik nudi pogled na planinu, a nalazi se na Kopaoniku, na 2,5 km od Ski-centra Kopaonik. Gosti koji borave u ovom apartmanu mogu da koriste potpuno opremljenu kuhinju i balkondasssssssssssssssss ssssssssssssssssdasdsadasdasdasdasdadasdsadasdasdsadsadsadasdasdsadsadsadadsdsasadsadasdsad cvxcvasdedawqsdDASDASDSADSADS ADSADSADSASDASADSADSDASDASDASDASDADSASD DASDASDASDADSASDFADSADSADSADSADSADSASDADSASDADSASSDADWWQQDWASDQAWWDQWADASDSADSA DSADSADAS DSAD ASD AS DAS DAS "
},{
    id: 0,
    name: "kucica",
    rating: 4.123,
    image: "https://cf.bstatic.com/xdata/images/hotel/max1024x768/237771595.jpg?k=74b6fd4d57358758e4a2bd4e44e067009229efa17dab6d64deddcbce7e8e8a93&o=&hp=1",
    address: {
        street: "ulica",
        apartment: "apartman",
    longitude: 43.7389411,
    latitude: 19.698573
    },
    price: {
        symbol: "RSD",
        amount: 100
    },
    rooms: 3,
    beds: 3,
    people: 5,
    selected: false,
    longitude: 43.7389411,
    latitude: 19.698573,
    cancellationFee: 30,
    description: "Opis vikendice"
},{
    id: 0,
    name: "kucica",
    rating: 4.123,
    image: "https://cf.bstatic.com/xdata/images/hotel/max1024x768/237771595.jpg?k=74b6fd4d57358758e4a2bd4e44e067009229efa17dab6d64deddcbce7e8e8a93&o=&hp=1",
    address: {
        street: "ulica",
        apartment: "apartman",
    longitude: 43.7389411,
    latitude: 19.698573
    },
    price: {
        symbol: "RSD",
        amount: 100
    },
    rooms: 3,
    beds: 3,
    people: 5,
    selected: false,
    longitude: 43.7389411,
    latitude: 19.698573,
    cancellationFee: 30,
    description: "Opis vikendice"
},{
    id: 0,
    name: "kucica",
    rating: 4.123,
    image: "https://cf.bstatic.com/xdata/images/hotel/max1024x768/237771595.jpg?k=74b6fd4d57358758e4a2bd4e44e067009229efa17dab6d64deddcbce7e8e8a93&o=&hp=1",
    address: {
        street: "ulica",
        apartment: "apartman",
    longitude: 43.7389411,
    latitude: 19.698573
    },
    price: {
        symbol: "RSD",
        amount: 100
    },
    rooms: 3,
    beds: 3,
    people: 5,
    selected: false,
    longitude: 43.7389411,
    latitude: 19.698573,
    cancellationFee: 30,
    description: "Opis vikendice"
},{
    id: 0,
    name: "kucica",
    rating: 4.123,
    image: "https://cf.bstatic.com/xdata/images/hotel/max1024x768/237771595.jpg?k=74b6fd4d57358758e4a2bd4e44e067009229efa17dab6d64deddcbce7e8e8a93&o=&hp=1",
    address: {
        street: "ulica",
        apartment: "apartman",
    longitude: 43.7389411,
    latitude: 19.698573
    },
    price: {
        symbol: "RSD",
        amount: 100
    },
    rooms: 3,
    beds: 3,
    people: 5,
    selected: false,
    longitude: 43.7389411,
    latitude: 19.698573,
    cancellationFee: 30,
    description: "Opis vikendice"
},{
    id: 0,
    name: "kucica",
    rating: 4.123,
    image: "https://cf.bstatic.com/xdata/images/hotel/max1024x768/237771595.jpg?k=74b6fd4d57358758e4a2bd4e44e067009229efa17dab6d64deddcbce7e8e8a93&o=&hp=1",
    address: {
        street: "ulica",
        apartment: "apartman",
    longitude: 43.7389411,
    latitude: 19.698573
    },
    price: {
        symbol: "RSD",
        amount: 100
    },
    rooms: 3,
    beds: 3,
    people: 5,
    selected: false,
    longitude: 43.7389411,
    latitude: 19.698573,
    cancellationFee: 30,
    description: "Opis vikendice"
}]

</script>