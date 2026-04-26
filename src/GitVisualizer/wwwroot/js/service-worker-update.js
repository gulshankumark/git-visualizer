// Service Worker Update Detection Module
// Detects when a new version of the service worker is available
// and notifies the client to reload

let swUpdateCallbackReference = null;
let swController = null;
let swWaitingWorker = null;

/**
 * Initialize service worker update detection.
 * Must be called once during app initialization.
 * 
 * @param {DotNetObject} callbackReference - Reference to the C# callback method
 */
window.initServiceWorkerUpdateDetection = function(callbackReference) {
    console.info('[SW Update] Initializing service worker update detection');
    swUpdateCallbackReference = callbackReference;

    if (!navigator.serviceWorker) {
        console.warn('[SW Update] Service Worker API not available');
        return;
    }

    // Listen for new service worker activation
    navigator.serviceWorker.addEventListener('controllerchange', () => {
        console.info('[SW Update] Service worker controller changed - new version is now active');
    });

    // Get the current controller
    if (navigator.serviceWorker.controller) {
        swController = navigator.serviceWorker.controller;
        setupUpdateListener(swController);
    }

    // Listen for any new service worker registration
    navigator.serviceWorker.ready.then(registration => {
        console.info('[SW Update] Service Worker is ready');

        // Check for updates periodically (every 5 minutes)
        setInterval(() => {
            console.info('[SW Update] Checking for service worker updates...');
            registration.update().catch(err => {
                console.error('[SW Update] Error checking for updates:', err);
            });
        }, 5 * 60 * 1000);

        // Setup listeners for updates
        if (registration.installing) {
            setupUpdateListener(registration.installing);
        }
        if (registration.waiting) {
            swWaitingWorker = registration.waiting;
            notifyUpdateAvailable();
        }

        // Listen for new installations
        registration.addEventListener('updatefound', () => {
            console.info('[SW Update] updatefound event - new service worker installing');
            const newWorker = registration.installing;
            if (newWorker) {
                setupUpdateListener(newWorker);
            }
        });
    }).catch(err => {
        console.error('[SW Update] Error getting ready service worker:', err);
    });
};

/**
 * Set up update listeners on a specific service worker
 * 
 * @param {ServiceWorker} worker - The service worker to listen to
 */
function setupUpdateListener(worker) {
    worker.addEventListener('statechange', () => {
        console.info(`[SW Update] Service worker state changed: ${worker.state}`);

        if (worker.state === 'installed' && navigator.serviceWorker.controller) {
            // New service worker installed while we have an active controller
            // This means an update is ready
            console.info('[SW Update] Update available - new service worker installed');
            swWaitingWorker = worker;
            notifyUpdateAvailable();
        }

        if (worker.state === 'activated') {
            console.info('[SW Update] Service worker activated');
        }
    });
}

/**
 * Notify the client that an update is available
 */
function notifyUpdateAvailable() {
    console.info('[SW Update] Notifying client of available update');
    if (swUpdateCallbackReference) {
        swUpdateCallbackReference.invokeMethodAsync('OnUpdateDetected')
            .catch(err => console.error('[SW Update] Error notifying client:', err));
    }
}

/**
 * Reload the page with the new service worker version.
 * Tells the waiting service worker to skip waiting and take control.
 */
window.reloadWithNewServiceWorkerVersion = function() {
    console.info('[SW Update] Reloading with new service worker version');

    if (!navigator.serviceWorker || !navigator.serviceWorker.controller) {
        console.warn('[SW Update] No service worker controller - performing standard reload');
        location.reload();
        return;
    }

    // Get the waiting service worker
    navigator.serviceWorker.ready.then(registration => {
        if (registration.waiting) {
            console.info('[SW Update] Telling waiting service worker to skip waiting');
            registration.waiting.postMessage({ type: 'SKIP_WAITING' });

            // Listen for the controller change, then reload
            let onControllerChange = null;
            onControllerChange = () => {
                console.info('[SW Update] Controller changed, reloading page');
                navigator.serviceWorker.removeEventListener('controllerchange', onControllerChange);
                window.location.reload();
            };
            navigator.serviceWorker.addEventListener('controllerchange', onControllerChange);

            // Failsafe: if controller doesn't change after 2 seconds, reload anyway
            setTimeout(() => {
                if (onControllerChange) {
                    navigator.serviceWorker.removeEventListener('controllerchange', onControllerChange);
                    console.warn('[SW Update] Controller change timeout, reloading anyway');
                    window.location.reload();
                }
            }, 2000);
        } else {
            console.info('[SW Update] No waiting service worker, performing standard reload');
            location.reload();
        }
    }).catch(err => {
        console.error('[SW Update] Error in reload process:', err);
        location.reload();
    });
};

console.info('[SW Update] Service Worker Update Detection module loaded');
