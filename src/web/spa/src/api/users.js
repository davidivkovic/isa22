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
const searchUsers = (query, page) =>
  fetch(
    instance.get('users', {
      params: {
        query,
        page
      }
    })
  )

const requestDeletion = reason =>
  fetch(
    instance.post('users/delete', reason, {
      headers: { 'Content-Type': 'application/json' }
    })
  )

const deleteUser = id => fetch(instance.post(`users/${id}/delete`))

const getPendingReviews = () => fetch(instance.get('users/reviews/pending'))

const approveReview = (id, approve) =>
  fetch(
    instance.post(`users/reviews/${id}/update`, {}, { params: { approve } })
  )

const getPendingReports = () => fetch(instance.get('users/reports/pending'))

const approveReport = (id, penalize) =>
  fetch(
    instance.post(
      `users/reservations/${id}/report/update`,
      {},
      { params: { penalize } }
    )
  )

export default {
  updateRegistrationRequest,
  getRegistrationRequests,
  updateDeletionRequest,
  getDeletionRequests,
  update,
  deleteUser,
  requestDeletion,
  getProfile,
  getUser,
  searchUsers,
  getPendingReviews,
  approveReview,
  getPendingReports,
  approveReport
}
