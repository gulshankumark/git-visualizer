// storage-monitor-interop.js
// Wraps navigator.storage.estimate() for IndexedDB quota monitoring

/**
 * Gets the current storage usage and quota estimates.
 * Returns {usage, quota} in bytes, or {usage: 0, quota: 0} if API unavailable.
 */
export async function getStorageEstimate() {
    try {
        if (navigator.storage && navigator.storage.estimate) {
            const estimate = await navigator.storage.estimate();
            return {
                usage: estimate.usage ?? 0,
                quota: estimate.quota ?? 50_000_000  // 50MB default if quota is unavailable
            };
        } else {
            console.warn("navigator.storage.estimate() not supported");
            return {
                usage: 0,
                quota: 50_000_000
            };
        }
    } catch (error) {
        console.error("Error calling navigator.storage.estimate():", error);
        return {
            usage: 0,
            quota: 50_000_000
        };
    }
}
