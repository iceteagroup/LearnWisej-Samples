# Install and update checklist — FieldOps PWA

## Installability (all three must hold)

| Condition | How FieldOps meets it | How to tell it apart on the device |
|---|---|---|
| Secure context | HTTPS in production; `http://localhost` counts as secure while developing. | DevTools console: `isSecureContext` is `true`. On a phone, an `http://` LAN address is **not** secure — no worker, no offer. |
| A valid, reachable manifest | `manifest.webmanifest` linked from `Default.html`, served as `application/manifest+json` with `no-cache`; name, short_name, 192/512 icons + maskable variants, `display: standalone`, `start_url`/`scope` = `/`, theme and background `#1565d8`. | DevTools → Application → Manifest shows no errors; `fetch('/manifest.webmanifest')` returns 200. |
| A worker whose scope covers the start URL | `sw.js` at the application root, registered from `Default.html`; scope `/`. | DevTools → Application → Service workers: activated, scope `/`; `navigator.serviceWorker.controller` is set after one reload. |

Then the browser fires `beforeinstallprompt`. `Default.html` keeps it; the **server** decides when
`⊕ Install FieldOps` appears (after the first successful load of the list, once per session, never
where `Device.Valid` is true). If the browser never fires the event — already installed, opened in
the installed window, a browser without install support (iOS Safari uses Share → Add to Home
Screen) — the server gets `false` back, shows no button and logs
`install offer not available in this browser`.

## Shipping a new shell

1. Change the shell file(s).
2. Bump `SHELL` in `sw.js` **and** `ShellVersion.Current` in `ShellVersion.cs` in the same commit
   (`fieldops-shell-v1` → `fieldops-shell-v2`).
3. Deploy. `sw.js` is served `no-cache`, so the next navigation finds the changed worker.
4. The new worker installs into the new cache, `skipWaiting()`, then `activate` deletes every other
   cache and `clients.claim()` takes the open page.
5. The page's `controllerchange` listener shows **A new version of FieldOps is ready. — Reload**; the
   technician taps it when they are ready.
6. Safety net: at session start the list page compares the cache the browser holds
   (`fieldOps.cachedShell`) with `ShellVersion.Current` and asks the worker to update when it is older.
7. Verify: DevTools → Application → Cache storage shows exactly one `fieldops-shell-*` cache.

**The step that fails silently if the cache name never changes:** step 4. The new worker installs
into the *same* cache name, `activate` deletes nothing, and `caches.match` keeps returning the old
shell files — the technician runs last month's shell with no error anywhere.
