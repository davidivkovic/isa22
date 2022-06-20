<template>
  <Logo class="" />
  <div class="-ml-20 flex space-x-10">
    <FlyoutMenu title="Requests" :items="requestsMenuItems" />
    <FlyoutMenu title="People" :items="peopleMenuItems" />
    <FlyoutMenu title="Money" :items="earningsMenuItems" />
  </div>
  <div class="flex space-x-3">
    <SignInButton />
  </div>
</template>
<script setup>
import Logo from './Logo.vue'
import SignInButton from './SignInButton.vue'
import {
  UserPlusIcon,
  PencilIcon,
  FileAnalyticsIcon,
  UserIcon,
  BusinessplanIcon,
  CashIcon,
  GiftIcon,
  CircleCheckIcon
} from 'vue-tabler-icons'
import FlyoutMenu from '../ui/FlyoutMenu.vue'

const requestsMenuItems = [
  {
    name: 'Account requests',
    description: 'Accept or decline registration or deletion requests.',
    icon: UserPlusIcon,
    path: '/requests'
  },
  {
    name: 'Review requests',
    description: "Accept or decline customers' reviews.",
    icon: PencilIcon,
    path: '/reviews'
  },
  {
    name: 'Report requests',
    description:
      "Accept or decline owners' reservation reports and penalize customers",
    icon: FileAnalyticsIcon,
    path: '/reports'
  }
]

const peopleMenuItems = [
  {
    name: 'Businesses',
    description: 'Manipulate all of current businesses.',
    icon: BusinessplanIcon,
    path: '/businesses'
  },
  {
    name: 'Users',
    description: 'Manipulate all current user accounts.',
    icon: UserIcon,
    path: '/users'
  },
  {
    name: 'Admins',
    description: 'Add new admins to the system.',
    icon: CircleCheckIcon,
    path: '/admin-signup'
  }
]

const earningsMenuItems = [
  {
    name: 'Loyalty programs',
    description: 'Manipulate loyalty programs.',
    icon: GiftIcon,
    path: '/loyalty'
  }
]
</script>
