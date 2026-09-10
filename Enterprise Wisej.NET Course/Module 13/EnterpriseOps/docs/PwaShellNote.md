# PWA shell note — and why it is not wired in

The lesson lists the **PWA shell** (manifest + service worker) among the module's key concepts. Both
files are written here as a deliverable:

* [`pwa/manifest.webmanifest`](pwa/manifest.webmanifest)
* [`pwa/service-worker.js`](pwa/service-worker.js)

**Neither is registered by the running sample.** `Default.html` links no manifest and registers no
service worker, and nothing under `docs/` is referenced by the application.

## Why not

1. **A service worker in front of a Wisej.NET session is a deployment decision, not a screen decision.**
   Wisej.NET serves the application over `wisej.wx` and a WebSocket; a worker that caches or replays
   those requests changes session behaviour globally. It belongs with the release runbook (Module 12),
   reviewed once, not switched on inside a lab.
2. **A cached shell hides the thing this module teaches.** If the browser can silently serve a stale
   shell, "offline" stops being an explicit application state with a queue and a boundary and becomes an
   invisible cache. The Field Technician prototype must show connectivity as a state the application
   owns.
3. **It would make the sample unrunnable-by-accident.** A registered worker survives a rebuild and keeps
   serving old assets from `http://localhost:5213` until it is manually unregistered — a bad trap in a
   teaching sample the reviewer runs once.

## What the two files are for

The **manifest** is the honest part of the PWA story and is safe to ship as written: it declares the
installable identity (name, icons, `display: standalone`, `start_url`, theme colours, an orientation and
a shortcut into field mode). Installing an app from the browser needs nothing else.

The **service worker** shows the only caching strategy that is defensible in front of a stateful server
application:

* **precache the shell only** — `Default.html`, the icons, nothing session-bound;
* **never** intercept `wisej.wx`, the WebSocket upgrade, or anything with a query string;
* **network-first with a shell fallback** for navigations, so a cold offline start shows the application
  frame and its own "offline" state instead of a browser error page;
* an explicit `SKIP_WAITING` message so a deploy can take effect without users hunting through devtools.

The comments in the file say the same thing at each decision point.

The icons the manifest names (`pwa/icons/icon-192.png`, `icon-512.png`, `icon-maskable-512.png`) are
**not** in the repository — this lab ships no binary assets. A real build adds them; until then the
manifest is a specification, and the worker's `addAll` would fail (which it handles, and which is
harmless because nothing registers it).

## How to try it (deliberately manual)

```html
<!-- Default.html, inside <head> — for an experiment only, never committed -->
<link rel="manifest" href="docs/pwa/manifest.webmanifest">
<script>
  if ('serviceWorker' in navigator)
    navigator.serviceWorker.register('docs/pwa/service-worker.js', { scope: '/' });
</script>
```

Then, before touching the sample again: DevTools → Application → Service Workers → **Unregister**, and
Storage → **Clear site data**. The rest of this lab assumes no worker is installed.

## Evidence

There is nothing to see in the running app, and that is the claim being made. Check it: `Default.html`
contains only the Wisej.NET bootstrap `<script src="wisej.wx">`, and a search for `serviceWorker` or
`manifest` in the project's served files returns only this note and the two files under `docs/pwa/`.
