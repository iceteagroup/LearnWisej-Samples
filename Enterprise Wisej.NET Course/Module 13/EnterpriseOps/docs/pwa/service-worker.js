/*
 * EnterpriseOps — Field Technician mode · PWA shell worker (DELIVERABLE ONLY)
 *
 * ┌──────────────────────────────────────────────────────────────────────────────────────────┐
 * │  THIS FILE IS NOT REGISTERED BY THE SAMPLE.                                               │
 * │  Default.html links no manifest and registers no worker. See ../PwaShellNote.md for why:  │
 * │  a worker in front of a Wisej.NET session is a deployment decision, and a silently cached │
 * │  shell would hide the explicit offline state this module exists to teach.                 │
 * └──────────────────────────────────────────────────────────────────────────────────────────┘
 *
 * What it does show is the only caching strategy that is defensible in front of a stateful server
 * application: precache the shell, never touch the session, fall back to the shell for navigations.
 */

const SHELL_CACHE = 'enterpriseops-field-shell-v1';

/* The shell: what the browser needs to draw the application frame with no network.
   Nothing session-bound, nothing user-specific, nothing that changes per deploy without a new
   SHELL_CACHE name. */
const SHELL_ASSETS = [
  '/',
  '/Default.html',
  '/docs/pwa/manifest.webmanifest',
  '/docs/pwa/icons/icon-192.png',
  '/docs/pwa/icons/icon-512.png',
];

/* Requests the worker must never handle. Wisej.NET's session traffic, its WebSocket upgrade and any
   query-string request are the application's own state: caching or replaying them corrupts the session
   in ways that are very hard to debug from a technician's phone. */
function isSessionTraffic(url) {
  return url.pathname.endsWith('.wx')          // wisej.wx and friends: the whole client-server protocol
      || url.pathname.startsWith('/wisej')     // framework resources served per session
      || url.search.length > 0;                // anything parameterised is not a shell asset
}

self.addEventListener('install', (event) => {
  event.waitUntil(
    caches.open(SHELL_CACHE)
      .then((cache) => cache.addAll(SHELL_ASSETS))
      // Do NOT skipWaiting automatically: a technician mid-shift should not have the shell swapped
      // under them. The page asks for it explicitly (see the message handler below).
      .catch((error) => console.warn('[field-sw] shell precache failed', error))
  );
});

self.addEventListener('activate', (event) => {
  event.waitUntil(
    caches.keys()
      .then((names) => Promise.all(
        names.filter((name) => name.startsWith('enterpriseops-field-shell-') && name !== SHELL_CACHE)
             .map((name) => caches.delete(name))
      ))
      .then(() => self.clients.claim())
  );
});

self.addEventListener('fetch', (event) => {
  const request = event.request;

  // Only GETs, only this origin, never the session protocol.
  if (request.method !== 'GET') return;

  const url = new URL(request.url);
  if (url.origin !== self.location.origin) return;
  if (isSessionTraffic(url)) return;

  // Navigations: network first, so a connected technician always gets the current shell; the cached
  // shell is the fallback, and the application then reports its own offline state — the queue, the
  // cache age and the OFFLINE pill — instead of the browser showing a dinosaur.
  if (request.mode === 'navigate') {
    event.respondWith(
      fetch(request).catch(() => caches.match('/Default.html').then((r) => r || caches.match('/')))
    );
    return;
  }

  // Shell assets: cache first (they are versioned by SHELL_CACHE), network as the fallback.
  event.respondWith(
    caches.match(request).then((cached) => cached || fetch(request))
  );
});

/* An explicit hand-over, triggered by the page after telling the user, so a deploy can take effect
   without anyone hunting through devtools. */
self.addEventListener('message', (event) => {
  if (event.data === 'SKIP_WAITING') self.skipWaiting();
});

/*
 * Deliberately NOT here:
 *   • Background Sync for the completion queue. The queue is the application's, it lives in the local
 *     store with an auditable state machine, and it replays through WorkOrderService — not through a
 *     worker that has no identity, no permission snapshot and no way to show a conflict to a person.
 *   • Caching of API or session responses. See isSessionTraffic().
 *   • Push notifications. They need a server, a subscription store and a consent decision; none of
 *     that belongs in a shell worker.
 */
