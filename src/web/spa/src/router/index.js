import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import CreateUpdateBusinessView from '../views/CreateUpdateBusinessView.vue'

const modalRouteFunc = (to, from, component) => {
  const fromMatch = from.matched[0]
  const toMatch = to.matched[0]

  toMatch.components['modal-router'] = () =>
    import(/* @vite-ignore */ component)
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
      component: CreateUpdateBusinessView
    },
    {
      path: '/results',
      name: 'results',
      component: () => import('../views/ResultsView.vue')
    },
    {
      path: '/signin',
      name: 'signin',
      beforeEnter: (to, from) =>
        modalRouteFunc(to, from, '../components/registration/SignInModal.vue')
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
      name: 'adventure profile',
      component: () => import('../views/CalendarView.vue')
    },
    {
      path: '/cabin-profile/:id',
      name: 'cabin-profile',
      component: () => import('../views/business/CabinProfileView.vue')
    },
    {
      path: '/boat-profile/:id',
      name: 'boat-profile',
      component: () => import('../views/business/BoatProfileView.vue')
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
      path: '/cabin-owner/:id/cabins',
      name: 'owners cabins',
      component: () => import('../views/OwnersCabins.vue')
    },
    {
      path: '/boat-owner/:id',
      name: 'boat owner',
      component: () => import('../views/BoatOwnerHomePage.vue')
    },
    {
      path: '/boat-owner/:id/boats',
      name: 'owners boats',
      component: () => import('../views/OwnersBoats.vue')
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
    }
  ]
})

const modalNames = ['signin', 'verification', 'admin-verification']
const nonSearchRoutes = ['profile', 'reservations']

export { modalNames, nonSearchRoutes }
export default router
