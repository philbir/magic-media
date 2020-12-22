importScripts("https://storage.googleapis.com/workbox-cdn/releases/4.3.1/workbox-sw.js");

workbox.core.setCacheNameDetails({ prefix: "media-ui" });

self.addEventListener('message', (event) => {
    if (event.data && event.data.type === 'SKIP_WAITING') {
        self.skipWaiting()
    }
});

setInterval(() => {

    fetch('api/session').then(response => {

        response.json().then(data => {
            console.log(data)

            self.clients.matchAll().then(clients => {
                clients.forEach(client => {
                    client.postMessage({ msg: data })
                });
            })
        })
    })

}, 300000)

/*
self.addEventListener('fetch', (event) => {
    //console.log(event.request)
});*/

/**
 * The workboxSW.precacheAndRoute() method efficiently caches and responds to
 * requests for URLs in the manifest.
 * See https://goo.gl/S9QRab
 */
