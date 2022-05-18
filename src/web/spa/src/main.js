import { createApp } from 'vue'

import App from './App.vue'
import router from './router'
import './index.css'

Date.prototype.addDays = function (days) {
  this.setDate(this.getDate() + parseInt(days))
  return this
}

const app = createApp(App)

app.use(router)

app.mount('#app')
