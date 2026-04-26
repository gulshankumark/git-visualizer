# PWA Service Worker & Offline Functionality - Manual Testing Guide

## Story 4-1: Service Worker, Offline Shell & Asset Caching

This document provides step-by-step instructions for manually testing the PWA offline functionality, cache behavior, and Lighthouse PWA audit compliance.

---

## Prerequisites

1. **Build the application for release:**
   ```bash
   dotnet publish -c Release
   ```

2. **Run the app locally or deploy to GitHub Pages:**
   - For local testing: `dotnet run` (uses Debug configuration, service worker will be in passthrough mode)
   - For GitHub Pages deployment: Push to main branch and wait for GitHub Actions

3. **Browser requirements:**
   - Chrome 90+ or Edge 90+
   - DevTools available (F12)

---

## Test 1: Service Worker Registration

### Objective
Verify that the service worker is properly registered and active.

### Steps

1. Open the app in Chrome/Edge
2. Press F12 to open DevTools
3. Go to the **Application** tab
4. In the left sidebar, click **Service Workers**
5. Verify the following:
   - Service worker `service-worker.js` is listed
   - Status shows **"active and running"**
   - Registration scope is correct (should match app base URL)

### Expected Result
✅ Service worker is active and responsive

---

## Test 2: Cache Storage Inspection

### Objective
Verify that all critical assets are pre-cached with the correct version manifest.

### Steps

1. Open DevTools → **Application** tab
2. In the left sidebar, click **Cache Storage**
3. You should see a cache named something like `offline-cache-<hash>`
4. Click on the cache name to expand it
5. Verify it contains the following types of assets:
   - `index.html` (entry point)
   - Blazor WASM runtime files (`*.wasm`, `*.js` in `_framework/`)
   - JS interop modules (`git-interop.js`, `gitgraph-renderer.js`)
   - MudBlazor CSS/JS (`_content/MudBlazor/MudBlazor.min.css`, etc.)
   - App CSS (`css/app.css`)
   - Configuration (`appsettings.json`)
   - Icons and manifest

### Expected Result
✅ All critical assets are present in the cache

---

## Test 3: Offline Mode - App Shell Loading

### Objective
Verify that the app loads completely when offline, without any network errors.

### Steps

1. Open DevTools → **Network** tab
2. Locate the **Offline** checkbox (usually near top-right of Network tab)
3. Click the **Offline** checkbox to enable offline mode
4. Reload the page (Ctrl+R or F5)
5. Monitor the Network tab for any failed requests
6. Check the Console tab for any errors

### Expected Result
✅ App loads completely
✅ No red (failed) requests in Network tab
✅ No error messages in Console tab
✅ App shell (UI, navigation) is fully interactive

---

## Test 4: Offline Functionality - Git Commands

### Objective
Verify that all git operations work correctly while offline.

### Steps

1. **Ensure offline mode is still enabled** (from Test 3)
2. In the app's terminal panel, execute the following commands in order:

   a. **Initialize a repository:**
      ```
      git init
      ```
      ✓ Should show success message

   b. **Create a test file:**
      ```
      echo "# Hello World" > README.md
      ```
      ✓ Should complete without error

   c. **Stage the file:**
      ```
      git add README.md
      ```
      ✓ Should complete silently (success)

   d. **Commit with a message:**
      ```
      git commit -m "Initial commit"
      ```
      ✓ Should show commit hash and message

   e. **View commit log:**
      ```
      git log
      ```
      ✓ Should show the commit you just created with:
      - Author info
      - Commit hash
      - Commit message
      - Date

3. **Execute additional commands:**
   ```
   git add README.md
   git commit -m "Second commit"
   git log
   ```
   ✓ Should show both commits in the log

### Expected Result
✅ All git commands execute successfully
✅ Terminal shows expected output
✅ No network errors in browser console

---

## Test 5: Offline Functionality - Graph Rendering

### Objective
Verify that the commit graph renders correctly with offline data.

### Steps

1. **Ensure you have commits** (from Test 4)
2. Look for the commit graph panel (usually right side of screen)
3. Verify the graph shows:
   - Visual representation of commits
   - Commit hashes or short hashes
   - Branch connections (if multiple branches)
4. The graph should update in real-time as you create new commits

### Expected Result
✅ Commit graph renders correctly
✅ Graph is interactive and responsive
✅ No rendering errors in Console

---

## Test 6: Online Reconnection

### Objective
Verify the app continues to work after reconnecting to the network.

### Steps

1. **While offline**, execute: `git status`
2. **Re-enable network:**
   - Uncheck the **Offline** checkbox in DevTools Network tab
3. **After reconnecting**, execute: `git log` again
4. The app should function normally

### Expected Result
✅ App remains stable after reconnection
✅ Previous offline changes are preserved
✅ App can communicate with network if needed

---

## Test 7: Cache Cleanup on Update

### Objective
Verify that old cache versions are cleaned up when a new version is deployed.

### Steps

1. **Note the current cache version:**
   - DevTools → Application → Cache Storage
   - Write down the cache name (e.g., `offline-cache-abc123`)

2. **Simulate a version update:**
   - In a real scenario, this happens when you deploy a new version
   - For testing: You can modify a file and republish

3. **Clear browser cache and hard-refresh:**
   - Press Ctrl+Shift+R (hard refresh)
   - Wait for new service worker to activate

4. **Verify the old cache is gone:**
   - DevTools → Application → Cache Storage
   - The old cache name should no longer appear
   - A new cache with a different hash should exist

### Expected Result
✅ Old cache versions are automatically deleted
✅ New cache version is created
✅ No orphaned cache entries

---

## Test 8: Update Detection (If Deploying New Version)

### Objective
Verify that update detection and notifications work when a new version is deployed.

### Steps

1. **Deploy a new version** to GitHub Pages or staging server
2. **In the browser with the old version:**
   - Ensure you're still running the old version
   - Leave the app open or visit it again
3. **Wait for update detection:**
   - Should take up to 5 minutes (periodic check interval)
   - Or manually trigger: DevTools → Service Workers → Update (if available)
4. **Look for the snackbar notification:**
   - At the bottom of the screen
   - Should say: "A new version is available. Reloading in 3 seconds..."
5. **Observe the reload:**
   - Page should reload automatically after 3 seconds
   - New version should be loaded

### Expected Result
✅ Update notification appears
✅ Page reloads with new version
✅ New service worker is active

---

## Test 9: Lighthouse PWA Audit

### Objective
Verify PWA compliance and get audit score ≥90.

### Steps

1. **Deploy the app** (GitHub Pages or staging)
2. **Open the app URL** in Chrome
3. **Open DevTools** (F12)
4. Go to the **Lighthouse** tab (may need to enable if not visible)
5. **Configure audit settings:**
   - Category: Check only "PWA"
   - Device: Mobile (for strictest testing)
6. **Click "Analyze page load"**
7. **Wait for audit to complete** (1-2 minutes)

### Expected Results

**PWA Score:**
✅ Overall PWA score ≥ 90

**Specific Audits (should all pass):**
- ✅ "Installable" - Passed
- ✅ "Registers a service worker" - Passed
- ✅ "Responds with a 200 when offline" - Passed
- ✅ "Contains a valid web app manifest" - Passed
- ✅ "Web app is installable" - Passed
- ✅ "Icons are masked-icon format for PWA" - Passed
- ✅ "Shortcuts are screenshots-based PWA" - Passed

**Performance Metrics (secondary check):**
- First Contentful Paint (FCP): < 3 seconds
- Largest Contentful Paint (LCP): < 4 seconds
- Cumulative Layout Shift (CLS): < 0.1

---

## Test 10: Browser Console Verification

### Objective
Ensure no network-related errors appear in the console.

### Steps

1. **Open DevTools** → **Console** tab
2. **Reload the page while offline** (from Test 3)
3. **Scan the console for errors:**
   - Look for red error messages
   - Filter for "network" or "offline" keywords
   - Check for 404 errors

### Expected Result
✅ No network-related error messages
✅ No 404 errors
✅ Service worker logs show cache hits
   - Messages like: "Service worker: Cache hit" or similar

---

## Acceptance Criteria Mapping

| Test | AC Coverage |
|------|-------------|
| 1    | AC1 (pre-cache config) |
| 2    | AC1 (asset listing) |
| 3-5  | AC2 & AC3 (offline fallback & functionality) |
| 6    | AC3 (no console errors) |
| 7    | AC5 (cache cleanup) |
| 8    | AC4 (update detection) |
| 9    | AC6 (Lighthouse ≥90) |
| 10   | AC7 (no network errors) |

---

## Troubleshooting

### App doesn't load offline

1. Check Service Workers registration (Test 1)
2. Verify cache exists (Test 2)
3. Check browser console for errors
4. Try hard refresh (Ctrl+Shift+R) while online
5. Clear site data and reload

### Service worker not activating

1. Ensure no other tabs have the old service worker active
2. Close all tabs of the app
3. Wait 30 seconds
4. Reload the app in a new tab

### Git commands not working offline

1. Verify isomorphic-git library is cached
2. Check Console for JavaScript errors
3. Verify IndexedDB still has file system data
4. Try resetting the sandbox (Ctrl+Shift+R)

### Lighthouse audit failing

1. Ensure service worker is active
2. Verify manifest.json is valid (Test 2)
3. Check icons are present and correct sizes
4. Ensure all required metadata in manifest
5. Try different network throttling settings

---

## Documentation References

- [MDN Service Worker API](https://developer.mozilla.org/en-US/docs/Web/API/Service_Worker_API)
- [MDN Cache API](https://developer.mozilla.org/en-US/docs/Web/API/Cache)
- [Lighthouse PWA Audits](https://developers.google.com/web/tools/lighthouse/audits/pwa)
- [Web App Manifest Spec](https://www.w3.org/TR/appmanifest/)

---

## Sign-Off

After completing all 10 tests successfully, the PWA implementation is ready for production deployment.

- [ ] Test 1: Service Worker Registration - PASS
- [ ] Test 2: Cache Storage Inspection - PASS
- [ ] Test 3: Offline App Shell - PASS
- [ ] Test 4: Git Commands Offline - PASS
- [ ] Test 5: Graph Rendering - PASS
- [ ] Test 6: Online Reconnection - PASS
- [ ] Test 7: Cache Cleanup - PASS
- [ ] Test 8: Update Detection - PASS
- [ ] Test 9: Lighthouse PWA Audit ≥90 - PASS
- [ ] Test 10: Console Verification - PASS

**Tested by:** ___________________  
**Date:** ___________________  
**Environment:** ___________________  
