importScripts("https://storage.googleapis.com/workbox-cdn/releases/4.3.1/workbox-sw.js");

if (workbox) {

    workbox.core.setCacheNameDetails({ prefix: "media-ui" });
    // adjust log level for displaying workbox logs
    //workbox.core.setLogLevel(workbox.core.LOG_LEVELS.debug)

    // apply precaching. In the built version, the precacheManifest will
    // be imported using importScripts (as is workbox itself) and we can 
    // precache this. This is all we need for precaching
    //workbox.precaching.precacheAndRoute(self.__precacheManifest);

    // Make sure to return a specific response for all navigation requests.
    // Since we have a SPA here, this should be index.html always.
    // https://stackoverflow.com/questions/49963982/vue-router-history-mode-with-pwa-in-offline-mode
    //workbox.routing.registerNavigationRoute('/index.html')


    workbox.routing.registerRoute(
        ({ url }) => url.pathname.startsWith('/api/media/webimage'),
        new workbox.strategies.StaleWhileRevalidate({
            cacheName: 'images',
            plugins: [
                new workbox.cacheableResponse.Plugin({
                    statuses: [200],
                    headers: {
                        'X-Sw-Cache-Image': 'true',
                    },
                }),
                new workbox.expiration.Plugin({
                    maxEntries: 1000,
                    maxAgeSeconds: 60 * 60 * 24 * 30, // 30 Days
                }),
            ],
        }),
    );

    workbox.routing.registerRoute(
        /^https:\/\/fonts\.gstatic\.com/,
        new workbox.strategies.CacheFirst({
            cacheName: 'google-fonts-webfonts',
            plugins: [
                new workbox.cacheableResponse.Plugin({
                    statuses: [0, 200],
                }),
                new workbox.expiration.Plugin({
                    maxAgeSeconds: 60 * 60 * 24 * 365,
                    maxEntries: 30,
                }),
            ],
        })
    )

    workbox.routing.registerRoute(
        ({ url }) => url.pathname.startsWith('/api/media/'),
        new workbox.strategies.StaleWhileRevalidate({
            cacheName: 'thumbnails',
            plugins: [
                new workbox.cacheableResponse.Plugin({
                    statuses: [200],
                    headers: {
                        'X-Sw-Cache-Thumbnail': 'true',
                    },
                }),
                new workbox.expiration.Plugin({
                    maxEntries: 25000,
                    maxAgeSeconds: 60 * 60 * 24 * 90, // 90 Days
                }),
            ],
        }),
    );
}

self.addEventListener('message', (event) => {
    if (event.data && event.data.type === 'SKIP_WAITING') {
        self.skipWaiting()
    }
});

setInterval(() => {

    fetch('api/session').then(response => {
        response.json().then(data => {

            if (!data.isAuthenticated) {
                self.clients.matchAll().then(clients => {
                    clients.forEach(client => {
                        sendClientAction(client, { action: 'ROUTE', value: 'SessionExpired' })
                    });
                })
            }
        })
    })

}, 1000 * 60 * 3)

addEventListener('fetch', event => {

    const requestUrl = new URL(event.request.url);

    if (requestUrl.pathname.startsWith("/graphql")) {

        event.respondWith(
            fetch(event.request).then(function (response) {
                if (response.status === 403) {
                    self.clients.get(event.clientId).then(client => {
                        sendClientAction(client, { action: 'ROUTE', value: 'SessionExpired' })
                    });
                }

                return response;
            }));
    }
});

const sendClientAction = (client, message) => {
    client.postMessage(message);
}

const isGraphQLNotAuth = (data) => {

    if (data.errors && data.errors.length > 0) {

        if (data.errors[0].extensions.code === "AUTH_NOT_AUTHENTICATED") {
            return true;
        }
    }

    return false;
} 