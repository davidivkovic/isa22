import { instance, fetch } from './http.js'

const getFinanceParams = async () => fetch(instance.get('finances/params'))
const setFinanceParams = async params =>
  fetch(instance.post('finances/params/set', params))

const getReport = async (startDate, endDate, type = 'earnings') =>
  fetch(
    instance.get('finances/report', {
      params: {
        startDate,
        endDate,
        type
      }
    })
  )

export default {
  getFinanceParams,
  setFinanceParams,
  getReport
}
