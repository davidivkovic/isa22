import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import SignInModal from '../components/registration/SignInModal.vue'

const modalRouteFunc = (to, from, component) => {
  const fromMatch = from.matched[0]
  const toMatch = to.matched[0]

  if (typeof component == 'string') {
    toMatch.components['modal-router'] = () =>
      import(/* @vite-ignore */ component)
  } else {
    toMatch.components['modal-router'] = component
  }
  fromMatch && (toMatch.components.default = fromMatch.components.default)
  !fromMatch &&
    (toMatch.components.default = () => import('../views/HomeView.vue'))
}

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      // redirect: '/cabins',
      // name: 'home',
      component: HomeView
    },
    {
      path: '/cabins',
      name: 'cabins',
      component: HomeView
    },
    {
      path: '/boats',
      name: 'boats',
      component: HomeView
    },
    {
      path: '/adventures',
      name: 'adventures',
      component: HomeView
    },
    {
      path: '/business/:type/:action',
      name: 'create-update-business',
      component: () => import('../views/CreateUpdateBusinessView.vue')
    },
    {
      path: '/results',
      name: 'results',
      component: () => import('../views/ResultsView.vue')
    },
    {
      path: '/signin',
      name: 'signin',
      beforeEnter: (to, from) => modalRouteFunc(to, from, SignInModal)
    },
    {
      path: '/verification',
      name: 'verification',
      props: true,
      beforeEnter: (to, from) =>
        modalRouteFunc(
          to,
          from,
          '../components/registration/VerificationCodeModal.vue'
        )
    },
    {
      path: '/admin-verification',
      name: 'admin-verification',
      beforeEnter: (to, from) =>
        modalRouteFunc(to, from, '../components/NewAdminPasswordModal.vue')
    },
    {
      path: '/adventure-profile/:id',
      name: 'adventure-profile',
      component: () => import('../views/business/AdventureProfileView.vue')
    },
    {
      path: '/adventure-profile/:id/calendar',
      name: 'adventure-calendar',
      component: () => import('../views/CalendarView.vue')
    },
    {
      path: '/my-adventures',
      name: 'owners adventures',
      component: () => import('../views/OwnersBusinesses.vue')
    },
    {
      path: '/adventures-reservations',
      name: 'adventure owners reservations',
      component: () => import('../views/OwnersReservations.vue')
    },
    {
      path: '/cabin-profile/:id',
      name: 'cabin-profile',
      component: () => import('../views/business/CabinProfileView.vue')
    },
    {
      path: '/cabin-profile/:id/calendar',
      name: 'cabin-calendar',
      component: () => import('../views/CalendarView.vue')
    },
    {
      path: '/cabins-reservations',
      name: 'cabin owners reservations',
      component: () => import('../views/OwnersReservations.vue')
    },
    {
      path: '/boats-reservations',
      name: 'boat owners reservations',
      component: () => import('../views/OwnersReservations.vue')
    },
    {
      path: '/boat-profile/:id',
      name: 'boat-profile',
      component: () => import('../views/business/BoatProfileView.vue')
    },
    {
      path: '/boat-profile/:id/calendar',
      name: 'boat-calendar',
      component: () => import('../views/CalendarView.vue')
    },
    {
      path: '/:type/search',
      name: 'search',
      component: () => import('../views/ResultsView.vue')
    },
    {
      path: '/cabin-owner/:id',
      name: 'cabin owner',
      component: () => import('../views/CabinOwnerHomePage.vue')
    },
    {
      path: '/my-cabins',
      name: 'owners cabins',
      component: () => import('../views/OwnersBusinesses.vue')
    },
    {
      path: '/boat-owner/:id',
      name: 'boat owner',
      component: () => import('../views/BoatOwnerHomePage.vue')
    },
    {
      path: '/my-boats',
      name: 'owners boats',
      component: () => import('../views/OwnersBusinesses.vue')
    },
    {
      path: '/profile',
      name: 'profile',
      component: () => import('../views/user/ProfileView.vue')
    },
    {
      path: '/my-reservations',
      name: 'reservations',
      component: () => import('../views/user/Reservations.vue')
    },
    {
      path: '/requests',
      name: 'requests',
      component: () => import('../views/admin/RequestsView.vue')
    },
    {
      path: '/loyalty',
      name: 'loyalty',
      component: () => import('../components/admin/LoyaltyProgram.vue')
    },
    {
      path: '/get-profile/:id',
      name: 'user profile',
      component: () => import('../views/UserProfile.vue')
    },
    {
      path: '/reviews',
      name: 'reviews',
      component: () => import('../views/admin/ReviewsView.vue')
    },
    {
      path: '/users',
      name: 'users',
      component: () => import('../views/admin/UsersView.vue')
    },
    {
      path: '/businesses',
      name: 'businesses',
      component: () => import('../views/OwnersBusinesses.vue')
    },
    {
      path: '/admin-signup',
      name: 'admin-signup',
      beforeEnter: (to, from) =>
        modalRouteFunc(
          to,
          from,
          '../components/registration/AdminSignUpModal.vue'
        )
    }
  ]
})

const modalNames = [
  'signin',
  'admin-signup',
  'verification',
  'admin-verification'
]
const nonSearchRoutes = ['profile', 'reservations']

export { modalNames, nonSearchRoutes }
export default router
