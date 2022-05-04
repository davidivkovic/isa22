<template>
    <div class="justify-between">
        <div class="flex mx-auto items-center justify-between py-6 max-w-6xl p-3 shadow shadow-emerald-300">
            <p class="text-3xl">Welcome</p>
            <Button class="flex items-center space-x-3 bg-emerald-600 text-white"><UserCircleIcon/></Button>
        </div>
        <div class="mx-auto h-2/3 max-w-6xl mt-20 rounded-lg bg-white overflow-auto shadow divide-y divide-gray-200 sm:divide-y-0 sm:grid sm:grid-cols-2 sm:gap-px">
            <OptionCard 
            name="Profile"
            routeName=""
            description="View your profile page"
            >
                <UserCircleIcon/>
            </OptionCard>
            <OptionCard 
            name="Boats"
            routeName="owners boats"
            description="Overview of cabins. Search, filter, delete and add new boats!"
            >
                <ShipIcon/>
            </OptionCard>
            <OptionCard 
            name="Average ratings"
            routeName=""
            description="View average rating of your boats"
            >
                <StarIcon/>
            </OptionCard>
            <OptionCard 
            name="Income"
            routeName=""
            description="View income"
            >
                <CashIcon/>
            </OptionCard>
            <OptionCard 
            name="Reservations graph"
            routeName=""
            description="View boats reservation graph"
            >
                <ChartLineIcon/>
            </OptionCard>
        </div>
    </div>
</template>
<script setup>
import OptionCard from '../components/OptionCard.vue'
import { ShipIcon, CashIcon, ChartLineIcon, UserCircleIcon, StarIcon} from 'vue-tabler-icons'
import Button from '../components/ui/Button.vue'


</script>
