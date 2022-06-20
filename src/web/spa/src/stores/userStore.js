import { computed, reactive } from 'vue'

const authenticatedUser = JSON.parse(localStorage.getItem('authenticated-user'))
let user = reactive({
  ...authenticatedUser
})

const setUser = newUser => {
  console.log(newUser)
  Object.assign(user, newUser)
  localStorage.setItem('authenticated-user', JSON.stringify(newUser))
}

const removeUser = () => {
  user = reactive({})
  localStorage.removeItem('authenticated-user')
}

const isAuthenticated = computed(() => Object.keys(user).length !== 0)

const isCustomer = computed(
  () => !isAuthenticated.value || user?.roles.includes('Customer')
)

const isAdmin = computed(() => user?.roles.includes('Admin'))

const businessType = computed(() => {
  if (user?.roles.includes('Cabin Owner')) return 'cabins'
  else if (user?.roles.includes('Boat Owner')) return 'boats'
  else if (user?.roles.includes('Fisher')) return 'adventures'
  else return 'none'
})
export {
  user,
  setUser,
  removeUser,
  isAuthenticated,
  isCustomer,
  isAdmin,
  businessType
}
