# Lifecycle, drafts and one way back

## What `WorkOrderPage` does

| Event / action | Handler | Effect |
|---|---|---|
| Page load (also after a `ReloadOnForeground` reload) | `RestoreDraft()` | Reopens the saved order with the saved reading; banner *Draft restored — WO-4821 · reading 412.8* |
| Typing in Meter reading / Notes | `Draft_Changed` → `SaveDraft()` | Saved on **every** change: server `DraftStore` (keyed by technician) + `Device.Preferences` `fieldops.wo` / `fieldops.draft` in the client |
| `Device.Lifecycle.Backgrounding` | `SaveDraft()` | Status *Background · draft saved* |
| `Device.Lifecycle.Stopped` | `SaveDraft()` | — |
| `Device.Lifecycle.Resumed` | `RestoreDraft()` | Back to *Remote · FieldOps server*; draft put back if the page survived (`ReloadOnForeground = false`) |
| `Device.Application.BackRequested` (Android back button and gesture) | `NavigateBack()` | |
| App-bar back arrow (iOS has no system back; the browser has none either) | `NavigateBack()` | |
| **Submit reading** | `WorkOrderService.SubmitReading` | Saved → draft cleared; invalid number / not On site → explained; conflict / failure → kept on screen, logged |

`NavigateBack`: an open dialog closes first; on the list there is nothing to pop; an unsent reading
asks **Leave WO-4821? / The reading 412.8 has not been submitted yet.** (Stay / Leave); otherwise the
detail page pops back to the list. The navigation state (`_open`, `_dirty`, `_dialog`) lives in the
Wisej.NET page because only FieldOps knows which screen is open — the client just reports "back".

Layout: `Device.System.IsPhone` → list, then detail full screen; `IsTablet` → side by side; a browser
narrower than 600 px gets the phone layout; `Device.StatusBar.BackColor` = the app-bar blue.

## Observed on the Windows Hybrid client (server log)

```
draft: restored WO-4821 reading=412.8 (session 4cf0f816)       <- new session, draft from DraftStore
lifecycle: Deactivated                                          <- window minimised
lifecycle: Stopped
lifecycle: Resumed                                              <- window restored
draft: restored WO-4821 reading=412.8
lifecycle: Activated
```

On Windows, minimising raised `Deactivated` and `Stopped` but **not** `Backgrounding`, and restoring
did **not** reload the WebView (the same session continued) — `ReloadOnForeground` and
`Backgrounding` are phone behaviours. That is exactly why the draft is also saved on every field
change and on `Stopped`, not only on `Backgrounding`. `BackRequested` was not raised on Windows;
on Android it needs an emulator or a device (not available here).

## Review answers

- **ReloadOnForeground at its default (true).** After a three-minute call and after an hour the client
  reloads FieldOps both times: a fresh page, the latest deploy, current data — and `RestoreDraft()`
  reopens WO-4821 with 412.8 because the draft was saved outside the controls. After an hour the old
  server session may have expired; nothing is lost because the draft is keyed by technician, not by
  session. With **false**, the three-minute return is instant and the screen is as it was (the session
  and the process survived); after an hour the bet is lost (session timeout or the OS reclaimed the
  app) and the same restore path is what saves the reading. What stays the same: the save points.
- **One NavigateBack for three clients.** Android calls it from `BackRequested`, iOS and the browser
  from the app-bar arrow. The stack lives in the Wisej.NET page because the client has no idea which
  FieldOps screen is open or whether a reading is unsent.
- **Client release vs server deploy.** Client build + store release: the property block, a new
  Android permission (and any `Info.plist` change, `FieldOpsServer` addresses). Live with the next
  server deploy: the lifecycle handlers, the draft logic, `NavigateBack` and the phone layout.
