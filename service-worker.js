/* Manifest version: 3lzZJPzg */
// Service Worker for git-visualizer PWA
// Provides offline support with cache-first strategy for pre-cached assets
// and offline fallback for uncached requests.
// See https://aka.ms/blazor-offline-considerations

self.importScripts('./service-worker-assets.js');

self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', event => event.respondWith(onFetch(event)));

// Cache versioning and naming
const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;

// Asset filters for pre-caching
const offlineAssetsInclude = [
    /\.dll$/,           // Blazor runtime
    /\.pdb$/,           // Debugging info
    /\.wasm/,           // WebAssembly runtime
    /\.html$/,          // HTML pages
    /\.js$/,            // JavaScript modules
    /\.json$/,          // Configuration files
    /\.css$/,           // Stylesheets
    /\.woff$/,          // Web fonts
    /\.woff2$/,         // Web fonts
    /\.ttf$/,           // TrueType fonts
    /\.png$/,           // Images
    /\.jpe?g$/,         // JPEG images
    /\.gif$/,           // GIF images
    /\.ico$/,           // Favicons
    /\.blat$/,          // Blazor artifacts
    /\.dat$/,           // Data files
    /\.webmanifest$/    // Web app manifest
];

const offlineAssetsExclude = [
    /^service-worker\.js$/  // Exclude dev service worker
];

// Base path for asset resolution (GitHub Pages deployment)
// This should match the <base href> in index.html
const base = self.registration.scope.endsWith('/git-visualizer/') ? '/git-visualizer/' : '/';
const baseUrl = new URL(base, self.origin);
const manifestUrlList = self.assetsManifest.assets.map(asset => new URL(asset.url, baseUrl).href);

/**
 * Install event: Pre-cache all required assets
 * Cache-First strategy means these assets will be served from cache on every request
 */
async function onInstall(event) {
    console.info('Service worker: Install event - pre-caching assets');
    
    try {
        // Fetch and cache all matching items from the assets manifest
        const assetsRequests = self.assetsManifest.assets
            .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
            .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
            .map(asset => new Request(asset.url, { integrity: asset.hash, cache: 'no-cache' }));
        
        console.info(`Service worker: Pre-caching ${assetsRequests.length} assets`);
        await caches.open(cacheName).then(cache => cache.addAll(assetsRequests));
        
        // Immediately activate new service worker (for faster updates)
        self.skipWaiting();
        console.info('Service worker: Install complete');
    } catch (error) {
        console.error('Service worker: Install failed', error);
    }
}

/**
 * Activate event: Clean up old cache versions and claim all clients
 */
async function onActivate(event) {
    console.info('Service worker: Activate event - cleaning up old caches');
    
    // Delete unused caches
    const cacheKeys = await caches.keys();
    await Promise.all(cacheKeys
        .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
        .map(key => {
            console.info(`Service worker: Deleting old cache '${key}'`);
            return caches.delete(key);
        }));
    
    // Claim all clients immediately (makes the new SW active for all tabs)
    await self.clients.claim();
    console.info('Service worker: Activate complete');
}

/**
 * Fetch event: Implement Cache-First strategy with offline fallback
 * - Serve pre-cached assets from cache (fast, offline-ready)
 * - For uncached requests, try network first
 * - If network fails, fall back to index.html (offline shell)
 */
async function onFetch(event) {
    // Only handle GET requests
    if (event.request.method !== 'GET') {
        return;
    }

    try {
        const cache = await caches.open(cacheName);
        
        // Check if this is a navigation request (page load)
        const isNavigateRequest = event.request.mode === 'navigate';
        
        // Determine if we should serve from cache or network
        const shouldServeIndexHtml = isNavigateRequest
            && !manifestUrlList.some(url => url === event.request.url);
        
        // Resolve the actual request to serve
        const resolvedRequest = shouldServeIndexHtml ? 'index.html' : event.request;
        
        // Try to get from cache first (Cache-First strategy)
        let cachedResponse = await cache.match(resolvedRequest);
        if (cachedResponse) {
            return cachedResponse;
        }
        
        // Cache miss, try the network
        try {
            const networkResponse = await fetch(event.request);
            
            // Cache successful responses for later offline use
            if (networkResponse && networkResponse.status === 200 && event.request.method === 'GET') {
                const clonedResponse = networkResponse.clone();
                cache.put(event.request, clonedResponse).catch(err => {
                    console.warn('Service worker: Failed to cache response', event.request.url, err);
                });
            }
            
            return networkResponse;
        } catch (networkError) {
            // Network request failed, return cached fallback or offline shell
            console.warn('Service worker: Network request failed, using offline fallback', event.request.url, networkError);
            
            // For navigation requests, serve the offline shell (index.html)
            if (isNavigateRequest) {
                const fallback = await cache.match('index.html');
                if (fallback) {
                    return fallback;
                }
            }
            
            // If we can't serve anything, return a generic offline response
            return new Response('Offline - resource not available', {
                status: 503,
                statusText: 'Service Unavailable',
                headers: new Headers({
                    'Content-Type': 'text/plain'
                })
            });
        }
    } catch (error) {
        console.error('Service worker: Fetch handler error', error);
        return fetch(event.request);
    }
}

/**
 * Message event handler for communicating with the client
 * Allows the client to trigger cache updates or other SW actions
 */
self.addEventListener('message', event => {
    console.info('Service worker: Message received', event.data);
    
    if (event.data && event.data.type === 'SKIP_WAITING') {
        self.skipWaiting();
    } else if (event.data && event.data.type === 'GET_VERSION') {
        event.ports[0].postMessage({
            type: 'VERSION',
            version: self.assetsManifest.version
        });
    }
});
