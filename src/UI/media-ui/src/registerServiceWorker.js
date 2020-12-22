/* eslint-disable no-console */

import { register } from 'register-service-worker'

//const enable_sw = process.env.NODE_ENV === 'production'
//const sw = 'service-worker.js';

const enable_sw = true;
const sw = 'sw-dev.js';

if (enable_sw) {
  register(`${process.env.BASE_URL}${sw}`, {
    ready() {
      console.log(
        'App is being served from cache by a service worker.'
      )
    },
    registered() {
      console.log('Service worker has been registered.')
    },
    cached() {
      console.log('Content has been cached for offline use.')
    },
    updatefound() {
      console.log('New content is downloading.')
    },
    updated(registration) {
      console.log('New content is available; please refresh Now!.')
      document.dispatchEvent(
        new CustomEvent('swUpdated', { detail: registration })
      )
    },
    offline() {
      console.log('No internet connection found. App is running in offline mode.')
    },
    error(error) {
      console.error('Error during service worker registration:', error)
    }
  })
}
