/* eslint-disable no-console */

import { register } from 'register-service-worker'

let enable_sw = process.env.NODE_ENV === 'production'
let sw = 'sw.js';

if (process.env.VUE_APP_DEV_SW) {
  enable_sw = true;
  sw = process.env.VUE_APP_DEV_SW

  console.warn("using DEV service-worker! To disable in development set environment: VUE_APP_DEV_SW to false'")
}

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
