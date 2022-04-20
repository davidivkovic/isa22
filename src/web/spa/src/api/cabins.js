import { instance, fetch } from './http.js'

const search = (city, country, start, end, people) => {
  return fetch(
    instance.get(
      `search/?city=${city}&country=${country}&start=${start}&end=${end}&people=${people}`
    )
  )
}

export default { search }
