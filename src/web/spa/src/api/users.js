import { instance, fetch } from './http.js'

const updateRegistrationRequest = (email, denialReason = '') =>
  fetch(
    instance.post(`users/registrations/${email}/update`, denialReason, {
      headers: { 'Content-Type': 'application/json' }
    })
  )

const getRegistrationRequests = (before = new Date().toISOString()) =>
  fetch(
    instance.get('users/registrations/pending', {
      params: {
        before
      }
    })
  )

const updateDeletionRequest = (email, denialReason = '') =>
  fetch(
    instance.post(`users/delete-requests/${email}/update`, denialReason, {
      headers: { 'Content-Type': 'application/json' }
    })
  )

const getDeletionRequests = (before = new Date().toISOString()) =>
  fetch(
    instance.get('users/delete-requests/pending', {
      params: {
        before
      }
    })
  )

const getProfile = () => fetch(instance.get('users/profile'))
const update = data => fetch(instance.post('users/update', data))

const requestDeletion = reason =>
  fetch(
    instance.post('users/delete', reason, {
      headers: { 'Content-Type': 'application/json' }
    })
  )

export default {
  updateRegistrationRequest,
  getRegistrationRequests,
  updateDeletionRequest,
  getDeletionRequests,
  update,
  requestDeletion,
  getProfile
}
