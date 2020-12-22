/**
 * Welcome to your Workbox-powered service worker!
 *
 * You'll need to register this file in your web app and you should
 * disable HTTP caching for this file too.
 * See https://goo.gl/nhQhGp
 *
 * The rest of the code is auto-generated. Please don't update this file
 * directly; instead, make changes to your Workbox build configuration
 * and re-run your build process.
 * See https://goo.gl/2aRDsh
 */

importScripts("https://storage.googleapis.com/workbox-cdn/releases/4.3.1/workbox-sw.js");

workbox.core.setCacheNameDetails({ prefix: "media-ui" });

self.addEventListener('message', (event) => {
    console.log('SW Message XXXXXX', event)
    if (event.data && event.data.type === 'SKIP_WAITING') {
        self.skipWaiting()
    }
});

self.addEventListener('fetch', (event) => {
    console.log(event.request.url)
    //console.log(event.request)
});

/**
 * The workboxSW.precacheAndRoute() method efficiently caches and responds to
 * requests for URLs in the manifest.
 * See https://goo.gl/S9QRab
 */
