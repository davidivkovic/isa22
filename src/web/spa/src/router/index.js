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
      redirect: '/cabins'
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
      path: '/fishing',
      name: 'fishing',
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
        modalRouteFunc(to, from, '../components/SignInModal.vue')
    },
    {
      path: '/verification',
      name: 'verification',
      props: true,
      beforeEnter: (to, from) =>
        modalRouteFunc(to, from, '../components/VerificationCodeModal.vue')
    },
    {
      path: '/admin-verification',
      name: 'admin-verification',
      beforeEnter: (to, from) =>
        modalRouteFunc(to, from, '../components/NewAdminPasswordModal.vue')
    },
    {
      path: '/adventure-profile/:id',
      name: 'adventure profile',
      component: () => import('../views/AdventureProfileView.vue')
    },
    {
      path: '/cabin-profile',
      name: 'cabin profile',
      component: () => import('../views/CabinProfileView.vue')
    },
    {
      path: '/boat-profile',
      name: 'boat profile',
      component: () => import('../views/BoatProfileView.vue')
    },
    {
      path: '/search',
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
    }
  ]
})

const modalNames = ['signin', 'verification', 'admin-verification']

export { modalNames }
export default router
