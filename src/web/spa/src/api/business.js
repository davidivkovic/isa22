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

const search = (query, type) =>
  fetch(instance.get(`${type}/search`, { params: { ...query } }))

const searchCabins = (query, id) =>
  fetch(instance.get(`cabin-owner/${id}/cabins`, { params: { ...query } }))

const searchBoats = (query, id) =>
  fetch(instance.get(`boat-owner/${id}/boats`, { params: { ...query } }))

const ownersBusinesses = async type => fetch(instance.get(`${type}`))

const getReservations = async (status, type) =>
  fetch(
    instance.get(`${endpoints[type]}/reservations`, {
      params: {
        status
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

export default {
  get,
  create,
  remove,
  update,
  getName,
  search,
  searchBoats,
  searchCabins,
  getReservations,
  getCalendar,
  createUnavailability,
  deleteUnavailability,
  previewCreateSale,
  createSale,
  deleteSale,
  ownersBusinesses,
  makeQuickReservation,
  makeResrvation,
  cancelReservation,
  review
}
