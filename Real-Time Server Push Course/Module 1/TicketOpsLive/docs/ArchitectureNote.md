# Architecture note — how a server-generated event reaches the browser (Module 1 deliverable)

**One paragraph.** TicketOps Live is a server-side Wisej.NET application: the browser hosts the widgets, the server
owns the authoritative UI state, and user actions travel to the server as events while control changes travel back as
compact updates. A normal update is *in-request* — a click handler changes a label and the response of that click
carries the change. A real-time update is *out-of-bound*: work that outlives the click (a heartbeat, an import, a
hub event) runs on a task started with `Application.StartTask`, which keeps the session context, changes the
session's own controls, and calls `Application.Update(this)` to push the pending changes over the WebSocket. When
no WebSocket is available the server cannot push, so the session asks the browser to poll (`StartPolling(1000)`) for
the duration of the work and stops polling when it ends. State that belongs to one user lives in the page instance
(never in a static), every loop has a stop condition (`_running`, `IsDisposed`), exceptions are caught inside the
task and the UI is restored in `finally`, and the push cadence is chosen for humans (one per second for a status
strip), not for the CPU.

## The end-to-end path (lesson checkpoint)

```
   service changes data          service raises an event          a session receives it
 ┌─────────────────────┐        ┌───────────────────────┐        ┌────────────────────────────┐
 │  heartbeat / import │  ───►  │  TicketChanged / step │  ───►  │ MainPage (this session's   │
 │  hub / queue        │        │  (domain event)       │        │ context, captured or kept  │
 └─────────────────────┘        └───────────────────────┘        │ by StartTask)              │
                                                                 └─────────────┬──────────────┘
                                                                               │ updates its own controls
                                                                               ▼
                                                                 ┌────────────────────────────┐
                                                                 │ clockLabel.Text = …        │
                                                                 │ serverLoadBar.Value = …    │
                                                                 │ activityLabel.Text = …     │
                                                                 └─────────────┬──────────────┘
                                                                               │ Application.Update(this)
                                                                               ▼
                             WebSocket available ──────────────► browser widgets re-render now
                             no WebSocket        ──► StartPolling(1000) ──► next poll carries the pending changes
```

In Module 1 the "service" is the heartbeat loop itself (single session, page-owned). From Module 6 on it is the
global `TicketHub`, which raises the event and never touches a page; each session subscribes and updates its own
controls with `Application.Update(context, …)`.

## Ownership (the vocabulary of the lesson)

| Thing | Owner | In this sample |
|---|---|---|
| Session | one browser tab | one `MainPage` instance per tab; `Application.ClientId` / `SessionId` in SERVER STATE |
| Application context | the session | kept by `Application.StartTask`; needed by `Application.Update` |
| In-bound code | a browser request | `refreshButton_Click` |
| Out-of-bound code | a task | `HeartbeatLoop`, the 5-step server event, the cadence experiment |
| Push | server → browser over the WebSocket | every `→ push` trace line |
| Polling | browser → server, on a timer | only while a task runs without a socket (`BeginPush` / `EndPush`) |

## Evidence

- Load: `IsWebSocket=false` in the first trace line; the first event afterwards reports `true` (the socket opens after the first response).
- Heartbeat: one `→ push Application.Update(this, …) heartbeat #n` per second; SERVER STATE `pushes` grows with `beats`.
- Stop: `heartbeat stopped — stopped by operator after n beats (finally block)` within one second of the click.
- Fault: `heartbeat stopped — fault: Simulated sensor failure in heartbeat #n` + red banner; the detail is in the server console; Start works again.
- Fallback (`"enableWebSocket": false`): `StartPolling(1000) … fallback polling ON`, one `Wisej: Poll request.` per second in the browser console, beats delivered, `EndPolling() … fallback polling OFF` after Stop.
