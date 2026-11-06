// sw.js — the FieldOps service worker. It caches the SHELL and never the session.
//
// Bump SHELL in the same commit as any shell file, and keep it equal to ShellVersion.Current
// in ShellVersion.cs. Served with Cache-Control: no-cache (Startup.cs) so the browser notices.

const SHELL = 'fieldops-shell-v1';

// The application shell: everything the page needs before the server says anything.
const ASSETS = [
  '/Default.html',
  '/offline.html',
  '/manifest.webmanifest',
  '/icons/icon-192.png',
  '/icons/icon-512.png',
  '/icons/icon-maskable-192.png',
  '/icons/icon-maskable-512.png',
  '/icons/apple-touch-icon.png',
];

self.addEventListener('install', (e) => {
  e.waitUntil(caches.open(SHELL).then((c) => c.addAll(ASSETS)));
  self.skipWaiting();                                   // no waiting worker
});

self.addEventListener('activate', (e) => e.waitUntil(
  caches.keys().then((names) => Promise.all(
    names.filter((n) => n !== SHELL).map((n) => caches.delete(n))))   // old caches gone
    .then(() => self.clients.claim())));

self.addEventListener('fetch', (e) => {
  if (e.request.method !== 'GET') return;               // uploads and app.wx posts pass straight through
  const url = new URL(e.request.url);
  if (url.origin !== self.location.origin) return;

  // Shell files: cache-first, matched by name.
  if (ASSETS.includes(url.pathname))
    return e.respondWith(caches.match(e.request).then((hit) => hit || fetch(e.request)));

  // A page load that cannot reach the server: the honest fallback.
  if (e.request.mode === 'navigate')
    e.respondWith(fetch(e.request).catch(() => caches.match('/offline.html')));

  // Everything else - wisej.wx, resource.wx, app.wx, the WebSocket - untouched.
});
