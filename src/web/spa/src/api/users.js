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
const getUser = id => fetch(instance.get(`users/get-profile/${id}`))
const update = data => fetch(instance.post('users/update', data))

const requestDeletion = reason =>
  fetch(
    instance.post('users/delete', reason, {
      headers: { 'Content-Type': 'application/json' }
    })
  )

const getPendingReviews = () => fetch(instance.get('users/reviews/pending'))

const approveReview = (id, approve) =>
  fetch(
    instance.post(`users/reviews/${id}/update`, {}, { params: { approve } })
  )

export default {
  updateRegistrationRequest,
  getRegistrationRequests,
  updateDeletionRequest,
  getDeletionRequests,
  update,
  requestDeletion,
  getProfile,
  getUser,
  getPendingReviews,
  approveReview
}
