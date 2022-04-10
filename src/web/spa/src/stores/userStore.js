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

export { user, setUser, removeUser, isAuthenticated }
