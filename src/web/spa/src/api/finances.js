import { instance, fetch } from './http.js'

const getFinanceParams = () => fetch(instance.get('finances/params'))
const setFinanceParams = params =>
  fetch(instance.post('finances/params/set', params))

export default {
  getFinanceParams,
  setFinanceParams
}
