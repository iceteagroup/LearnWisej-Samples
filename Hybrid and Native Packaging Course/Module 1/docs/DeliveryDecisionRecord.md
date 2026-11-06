# Delivery decision record — FieldOps

*Status: accepted · Module 1 · written before any client project exists*

## Context

Dispatchers work in the office in a browser. Technicians work from phones and laptops in plant
rooms and vans with patchy signal; they need photos, navigation and a home-screen app. FieldOps is
one Wisej.NET application (`FieldOps/`) with a work-order list, a detail page and a status update
flow, already complete in a plain browser. The options are compared in
[DeliveryModelComparison.md](DeliveryModelComparison.md).

## 1. Targets

| # | Target | Who | Why |
|---|---|---|---|
| 1 | Browser tab | Dispatchers, supervisors | Needed by the office; proves the app; no install. |
| 2 | Installed PWA | Supervisors on tablets | Cheap; icon + standalone window + an honest offline page (Module 2). |
| 3 | Hybrid client · Windows | Depot technicians, dispatchers on laptops | Export to Documents, notifications, second window (Module 3). |
| 4 | Hybrid client · Android | Field technicians | Camera, location, lifecycle, back gesture (Modules 4–5). |
| 5 | Hybrid client · iOS | Field technicians on iPhones | Same client project; needs a Mac build agent. |
| 6 | Hybrid client · macOS (Mac Catalyst) | Regional office | Same project built on a Mac. |

## 2. Hosting mode

**Remote Application with a Local fallback after `OfflineTimeout = 5000` ms**, limited to today's
assigned orders and status changes.

- Remote keeps the code on our server and makes every screen fix a server deploy.
- The Local fallback (`FieldOps.Offline`, registered with `UseWisejOffline<OfflineStartup>()`) keeps
  the status flow working in a basement.

The costs of Local, stated plainly:

1. **Data sync is ours.** Which orders are on the device (today's assigned orders only), how a
   status change made offline reaches the server (queued with the order's version, replayed on
   reconnect through `WorkOrderService.UpdateStatus`), and who wins a conflict (the server; the
   technician is told — the same `WorkOrderConflictException` path the browser already shows).
2. **Updates go through the stores.** The offline project ships inside the client, so a fix to the
   offline screens waits for a store release and review on every platform.
3. **The app's code ships on the device.** Business rules are in the technician's hands; no server
   secret, connection string or API key may live in the offline project.

What would change the decision: if technicians are offline for whole shifts, Local-only becomes the
better choice; if every site has reliable signal (or the release process cannot carry store
releases), Remote-only is enough.

## 3. Build order

1. Browser (done in Module 1).
2. PWA shell, offline page and update flow (Module 2).
3. Hybrid client on Windows, Remote + Local fallback, MSIX (Module 3).
4. Same client on Android (emulator, then supplied phones) (Module 4).
5. Device features: camera, signature, location, reminders (Module 5).
6. iOS and Mac Catalyst once a Mac build agent exists; signing and distribution (Module 6).

## 4. Exit criteria (checkable, one per target)

| Target | Exit criterion |
|---|---|
| Browser | Two browser windows: a status change saved in one is visible after refreshing the other; a backwards transition is refused in words; an Upload attaches a photo. |
| PWA | Installed from Chrome/Edge; with the server stopped the offline page shows the last synced summary; after a deploy the update bar appears and reloads into the new build. |
| Windows client | Export lands in `Documents\FieldOps` on the laptop; with the network pulled for 5 s the client switches to the offline app and says so on screen. |
| Android client | A technician completes a work order on a supplied phone, with the network switched off halfway. |
| iOS client | Same as Android, on a supplied iPhone, built by the Mac agent. |
| macOS client | Export and second window work on a MacBook build. |

## 5. Capability design

Every device feature sits behind `Device.Valid` and has a browser answer — see
[CapabilityCheckDesign.md](CapabilityCheckDesign.md). The architecture is drawn in
[Architecture.md](Architecture.md).
