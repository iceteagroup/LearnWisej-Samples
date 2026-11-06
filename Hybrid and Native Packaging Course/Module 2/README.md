# FieldOps · Hybrid & Native Packaging · Module 2

Lab build for **Module 2 · Progressive Web App Setup**. FieldOps from Module 1, made installable.
The list is the walkthrough's screen: the blue app bar **FieldOps · My work orders**, the action
strip **Start job · Update status · ⊕ Install FieldOps** with **synced hh:mm** on the right, the grid
Work order · Site / asset · Priority · Status · Due with WO-4821 … WO-4836, and the session strip
**SESSION  Connected · 38 ms … 5 work orders · server-rendered**. Behind it: `manifest.webmanifest`
with icons and colours, `sw.js` caching the shell and never the session, the install offer the
server reveals after the first successful session, `offline.html` with the last synced summary, and
the update bar.

## Run it

```bash
cd "Hybrid and Native Packaging Course/Module 2/FieldOps"
dotnet run -f net10.0 --urls http://localhost:6302
```

Open <http://localhost:6302> in Chrome or Edge. `localhost` is a secure context, so the worker
registers over plain HTTP while developing; anywhere else FieldOps must be served over HTTPS
(`dotnet dev-certs https` + an `https://` URL in `--urls`).

## What to try

| Action | Expected result |
|---|---|
| Open the app | The list loads; DevTools → Application shows the manifest (FieldOps, standalone, `#1565d8`) and `sw.js` activated with scope `/`; Cache storage holds `fieldops-shell-v1` with 8 shell files. |
| Wait for the first load in Chrome/Edge | **⊕ Install FieldOps** appears and the strip reads *Connected · install offered*. Click it: the browser's install dialog; accept and FieldOps opens in its own window. |
| Same in a browser that offers no install (or the installed window) | No button; the server log says `install offer not available in this browser`. |
| Select WO-4824 (Assigned), press **Start job** | Status strip: *WO-4824 must be Accepted before it can be On site.* — the Module 1 rule still decides. |
| Stop the server, reload | `offline.html`: the not-connected banner, *LAST SYNCED hh:mm · TODAY · DANA WHITFIELD*, Open work orders / Next site **Kilmore Depot** / Due before 14:00, the first three rows, **Retry connection**. |
| **Retry connection** while the server is down | *Still no connection.* Start the server and press it again: a fresh session from `/`. |
| Bump `fieldops-shell-v1` → `v2` in `sw.js` **and** `ShellVersion.cs`, rebuild, restart, reload | **A new version of FieldOps is ready. — Reload**; the server log notes the stale shell; after Reload only `fieldops-shell-v2` remains. |

## Lab tasks → where in the code

| Lab step / deliverable | Implementation |
|---|---|
| Web app manifest with icons and colours, linked with an apple-touch-icon | [manifest.webmanifest](FieldOps/manifest.webmanifest), [icons/](FieldOps/icons), `<link>`s in [Default.html](FieldOps/Default.html) |
| Service worker: versioned cache, precache, activate deletes older caches | [sw.js](FieldOps/sw.js) `install` / `activate` |
| Fetch handler: shell cache-first, session untouched, non-GET ignored, navigation → offline page | [sw.js](FieldOps/sw.js) `fetch` |
| `no-cache` for `sw.js` and the manifest | [Startup.cs](FieldOps/Startup.cs) `OnPrepareResponse` |
| `beforeinstallprompt` kept in `Default.html`; Install revealed by the server via `Application.Eval` after the first good load, only where `Device.Valid` is false | `fieldOps._keep` / `showInstallButton` / `install` in Default.html; `OnFirstSessionCompleted` in [WorkOrdersPage.cs](FieldOps/WorkOrdersPage.cs) |
| Summary to browser storage after every successful load | `Summarize` + `fieldOps.saveSummary` |
| Offline page with banner, last-synced summary and Retry | [offline.html](FieldOps/offline.html) |
| Update flow: `skipWaiting`, `clients.claim`, `controllerchange` → reload bar, build-stamp check | sw.js; Default.html; `CheckShellVersion` + [ShellVersion.cs](FieldOps/ShellVersion.cs) |
| Note on what the PWA does not do offline, and when it is a Hybrid Local Application | [docs/WhatThePwaDoesNotDoOffline.md](docs/WhatThePwaDoesNotDoOffline.md) |
| Install conditions and the update procedure | [docs/InstallAndUpdateChecklist.md](docs/InstallAndUpdateChecklist.md) |

## What was verified, and how

- `dotnet build`: 0 errors, 0 warnings (`Wisej-4` 4.1.7, `Wisej-4-Hybrid` 4.1.7).
- Browser pane (`fieldops-m2`, port 6302), read back with `javascript_tool`: secure context;
  `sw.js` registered, activated, controlling the page, scope `/`; one cache `fieldops-shell-v1`
  holding exactly the 8 shell files; manifest served as `application/manifest+json` with
  `no-cache`, `sw.js` with `no-cache`; the summary in `localStorage` (5 open, next site Kilmore
  Depot, 2 due before 14:00, synced time).
- Start job on WO-4824 was refused by the service rule with the sentence above.
- Server stopped, page reloaded: `offline.html` rendered the summary; Retry reported *Still no
  connection*.
- "Deployment": `SHELL`/`ShellVersion.Current` bumped to v2, rebuilt, restarted, page reloaded: the
  reload bar appeared, the server logged `runs shell fieldops-shell-v1, server ships
  fieldops-shell-v2: asking the worker to update`, and after **Reload** exactly one cache
  (`fieldops-shell-v2`) remained. The source was put back to v1 afterwards.
- `app.wx` traffic was never answered from the cache (non-zero transfer sizes; the cache holds only
  the 8 shell files).
- **Not verified end to end: the install prompt.** The course's browser pane is an embedded Chromium
  that never fires `beforeinstallprompt`, so the server took the *not available* path (logged). The
  page half was exercised by dispatching a synthetic `beforeinstallprompt`: `Default.html`
  prevented the default and `fieldOps.showInstallButton()` turned `true`. Installing for real needs
  Chrome or Edge.

## Where this sample differs from the lesson

- **Status names.** The walkthrough's grid shows *Open / Assigned / In progress*; FieldOps keeps the
  Module 1 status flow (Assigned → Accepted → On site → Completed), so WO-4821 shows *On site* and
  the others *Assigned*. The lab's rules win over the mock's labels.
- **The update bar overlays the action strip** (a fixed element under the app bar) rather than
  pushing the grid down: it belongs to the shell, not to the Wisej.NET page, and disappears on Reload.
- **No window glyphs in the app bar.** The installed-window mock draws minimise/maximise glyphs
  inside the app bar; in a real standalone window the operating system draws them, so the page does not.
- **"Connected · NN ms"** is a real round trip (`Application.Eval` timed on the server), so the number
  differs from the video's 38/42 ms.
- **HTTPS.** The lab asks to serve over HTTPS locally; the launch entry uses `http://localhost`, which
  browsers treat as a secure context, so every PWA feature here works without a certificate change.
- **Precache list.** The lesson lists "client scripts and theme" in the shell. Wisej's own scripts
  (`wisej.wx`, `resource.wx/*`) are left to the network on purpose: they are produced by the
  running Wisej.NET server for its own version, and a stale cached loader paired with an upgraded
  server is exactly the failure the update flow exists to avoid. Without the server they are useless
  anyway — the app cannot start.
  The cached shell is the page, the offline page, the manifest and the icons.
