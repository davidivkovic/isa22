import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import CreateAdventureView from '../views/CreateAdventure.vue'

const modalRouteFunc = (to, from, component) => {
  const fromMatch = from.matched[0]
  const toMatch = to.matched[0]

  toMatch.components['modal-router'] = () => import(component)
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
      path: '/fishing/create',
      name: 'fishing-create',
      component: CreateAdventureView
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
    }
  ]
})

const modalNames = ['signin', 'verification', 'admin-verification']

export { modalNames }
export default router
