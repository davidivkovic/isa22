import { setUser, removeUser } from '../stores/userStore.js'
import { instance, fetch, setAccesToken, removeAccesToken } from './http.js'

const setAuthData = data => {
  window.location.href = '/'
  setAccesToken(data.accessToken)
  setUser(data.user)
}

const signIn = body => {
  return fetch(instance.post('auth/email/sign-in', body), data =>
    setAuthData(data)
  )
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
  email = email.replace(' ', '+')
  return fetch(instance.post('auth/email/confirm', { email, code }))
}

const sendConfirmation = email => {
  email = email.replace(' ', '+')
  return fetch(
    instance.post(
      `auth/email/send-confirmation?email=${encodeURIComponent(email)}`
    )
  )
}

const setAdminPassword = (email, password, newPassword) =>
  fetch(
    instance.post('auth/password/set', {
      email,
      password,
      newPassword
    }),
    data => setAuthData(data)
  )

const changePassword = (password, newPassword) =>
  fetch(
    instance.post('auth/password/reset', {
      password,
      newPassword
    }),
    () => {
      removeAccesToken()
      setUser({})
    }
  )

export default {
  signIn,
  signOut,
  register,
  confirmEmail,
  sendConfirmation,
  setAdminPassword,
  changePassword
}
