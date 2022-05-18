import { instance, fetch } from './http.js'

const endpoints = {
  cabins: 'cabins',
  boats: 'boats',
  adventures: 'adventures'
}

const get = async (id, type) => fetch(instance.get(`${endpoints[type]}/${id}`))

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

const search = (query, type) =>
  fetch(instance.get(`${type}/search`, { params: { ...query } }))

const searchCabins = (query, id) =>
  fetch(instance.get(`cabin-owner/${id}/cabins`, { params: { ...query } }))

const searchBoats = (query, id) =>
  fetch(instance.get(`boat-owner/${id}/boats`, { params: { ...query } }))

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

export default {
  get,
  create,
  update,
  search,
  searchBoats,
  searchCabins,
  getReservations,
  getCalendar,
  createUnavailability,
  deleteUnavailability
}
