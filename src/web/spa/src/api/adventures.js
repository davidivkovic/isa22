import { instance, fetch } from './http.js'

const get = async id => fetch(instance.get(`adventures/${id}`))

const create = async (data, images) => {
  console.log(images)
  const [id, error] = await fetch(instance.post('adventures/create', data))
  if (error) return
  const formData = new FormData()
  images.forEach(i => formData.append('files', i))
  await fetch(instance.post(`adventures/${id}/images/add`, formData))
  return [id, error]
}

export default { get, create }
