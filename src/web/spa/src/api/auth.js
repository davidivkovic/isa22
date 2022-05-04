import { setUser, removeUser } from '../stores/userStore.js'
import { instance, fetch, setAccesToken, removeAccesToken } from './http.js'

const signIn = data => {
  return fetch(instance.post('auth/email/sign-in', data), data => {
    window.location.href = '/'
    setAccesToken(data.accessToken)
    setUser(data.user)
  })
}

const signOut = async () => {
  await instance.post('auth/sign-out', {})
  removeUser()
  removeAccesToken()
  window.location.href = '/'
}

const register = data => {
  return fetch(instance.post('auth/email/sign-up', data))
}

const confirmEmail = (email, code) => {
  return fetch(instance.post('auth/email/confirm', { email, code }))
}

const sendConfirmation = email => {
  return fetch(
    instance.post(
      `auth/email/send-confirmation?email=${encodeURIComponent(email)}`
    )
  )
}

export default {
  signIn,
  signOut,
  register,
  confirmEmail,
  sendConfirmation
}
