import { instance, fetch } from './http.js'

const endpoints = {
  cabin: 'cabins',
  boat: 'boats',
  adventure: 'adventures'
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

export default { get, create, update }
