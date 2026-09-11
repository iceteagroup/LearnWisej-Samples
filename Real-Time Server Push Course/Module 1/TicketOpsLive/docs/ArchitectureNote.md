# Architecture note — how a server-generated event reaches the browser

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

## The end-to-end path

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

## The three mechanisms in this screen

| Mechanism | In this sample |
|---|---|
| In-request update | `refreshButton_Click` sets `statusLabel`; the response carries it |
| Out-of-bound WebSocket push | the server event (`pushStatusLabel`, 5 steps) and the heartbeat, both on `Application.StartTask` + `Application.Update` |
| Polling fallback | `BeginPush` / `EndPush`: `StartPolling(1000)` only while a task runs without a WebSocket |
