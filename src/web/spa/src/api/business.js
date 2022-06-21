import { instance, fetch } from './http.js'

// Typescript enum
const endpoints = {
  cabins: 'cabins',
  boats: 'boats',
  adventures: 'adventures'
}

const get = async (id, type, query) =>
  fetch(instance.get(`${endpoints[type]}/${id}`, { params: { ...query } }))

const createOrUpdate = async (data, type, action) => {
  const images = data.images
  delete data[images]
  const [id, error] = await fetch(
    instance.post(`${endpoints[type]}/${action}`, data)
  )

  if (error) return

  const formData = new FormData()
  images.forEach(i => formData.append('files', i))
  await fetch(instance.post(`${endpoints[type]}/${id}/images/add`, formData))

  return [id, error]
}

const create = async (data, type) => createOrUpdate(data, type, 'create')
const update = async (data, type) => createOrUpdate(data, type, 'update')

const getName = async (id, type) =>
  fetch(instance.get(`${endpoints[type]}/${id}/name`))

const search = async (query, type, page) =>
  fetch(instance.get(`${type}/search`, { params: { ...query, page } }))

const getBusinesses = async (type, query = {}) =>
  fetch(
    instance.get(`${endpoints[type]}/`, {
      params: { ...query }
    })
  )

const getReservations = async (
  type,
  status,
  page = 0,
  size = 10,
  isDashboard = false
) =>
  fetch(
    instance.get(`${endpoints[type]}/reservations`, {
      params: {
        status,
        page,
        size,
        isDashboard
      }
    })
  )

const getCalendar = async (id, start, end, type) =>
  fetch(
    instance.get(`${endpoints[type]}/${id}/calendar`, {
      params: {
        start,
        end
      }
    })
  )

const createUnavailability = async (id, start, end, type) =>
  fetch(
    instance.post(
      `${endpoints[type]}/${id}/calendar/create-unavailability`,
      {},
      {
        params: {
          start,
          end
        }
      }
    )
  )

const deleteUnavailability = async (id, eventId, type) =>
  fetch(
    instance.post(
      `${endpoints[type]}/${id}/calendar/delete-unavailability`,
      {},
      {
        params: {
          eventId
        }
      }
    )
  )
const previewCreateSale = async (id, type, data) =>
  fetch(instance.post(`${endpoints[type]}/${id}/sales/preview-create`, data))

const createSale = async (id, type, data) =>
  fetch(instance.post(`${endpoints[type]}/${id}/sales/create`, data))

const deleteSale = async (id, saleId, type) =>
  fetch(instance.post(`${endpoints[type]}/${id}/sales/${saleId}/delete`))

const remove = async (id, type) =>
  fetch(instance.post(`${endpoints[type]}/${id}/delete`))

const makeQuickReservation = async (id, type, saleId) =>
  fetch(instance.post(`${endpoints[type]}/${id}/sales/${saleId}/book`))

const makeResrvation = async (id, type, start, end, people, services) =>
  fetch(
    instance.post(`${endpoints[type]}/${id}/make-reservation`, {
      people,
      start,
      end,
      services
    })
  )

const cancelReservation = async (type, reservationId) =>
  fetch(
    instance.post(`${endpoints[type]}/reservations/${reservationId}/cancel`)
  )

const review = async (id, type, content, rating) =>
  fetch(
    instance.post(`${endpoints[type]}/${id}/review`, {
      content,
      rating
    })
  )

const complain = async (reservationId, type, content) =>
  fetch(
    instance.post(`${endpoints[type]}/reservations/${reservationId}/complain`, {
      content
    })
  )

const report = async (reservationId, type, reason, penalize) =>
  fetch(
    instance.post(`${endpoints[type]}/reservations/${reservationId}/report`, {
      reason,
      penalize
    })
  )

const subscribe = async (id, type) =>
  fetch(instance.post(`${endpoints[type]}/${id}/subscribe`))

const unsubscribe = async (id, type) =>
  fetch(instance.post(`${endpoints[type]}/${id}/unsubscribe`))

const getSubscriptions = async type =>
  fetch(instance.get(`${endpoints[type]}/subscriptions`))

export default {
  get,
  create,
  remove,
  update,
  getName,
  search,
  getReservations,
  getCalendar,
  createUnavailability,
  deleteUnavailability,
  previewCreateSale,
  createSale,
  deleteSale,
  makeQuickReservation,
  makeResrvation,
  cancelReservation,
  review,
  complain,
  getBusinesses,
  report,
  subscribe,
  unsubscribe,
  getSubscriptions
}
