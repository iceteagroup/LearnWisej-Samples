# Demo script — the capstone in nine steps

The lab's demo script, with the trace lines to expect. Run it with **two browser tabs** (tab A and tab B, opened
with **Open another session ↗**). Both tabs are the same browser, so they share `ClientId` and differ in
`SessionId` — the Session tab shows both.

## 0. Setup

```bash
cd "D:/Projects/LearnWisej-Samples/Real-Time Server Push Course/Module 7/TicketOpsLive"
dotnet run -f net10.0 --urls http://localhost:5307
```

Open <http://localhost:5307>, then click **Open another session ↗** for tab B. In tab B switch the **Ticket board**
tenant to `Northwind`.

## 1. Open TicketOps Live in two browser sessions

Each tab logs its own load:

```
← request  MainPage_Load    session <B> of client <same> · IsWebSocket=false (the socket opens after this response)
• server   wiring           registry registered · hub subscribed · ApplicationExit + SessionTimeout armed · healthTimer started (2 s)
→ push     Application.Update(_context, …)   SessionRegistry: <B> joined → 2 live · pushed into session <A> …
```

The last line lands in **tab A** without anyone clicking there: a global service raised an event on a thread-pool
thread and tab A re-entered its own context to render it.

## 2. Start the heartbeat

**▶ Start heartbeat** in tab A. Once per second:

```
→ push  Application.Update(this, …)   heartbeat #12: clock, load 64%, activity — one flush
```

The clock, the load bar and the activity line move with no request from the browser. A second click on Start is
refused (`refused — the heartbeat is already running`).

## 3. Start an import and show progress

**Import monitor → ▶ Start import**. 200 records at 25 ms, pushed every tenth:

```
• server  Application.StartTask   job J-38C419: 200 records × 25 ms, push every 10 · the handler returns now
→ push    Application.Update(this)  job J-38C419 · 50/200 · 25% · 1.39 s
```

Note the heartbeat pushes interleaving with the import pushes in the same trace: two loops, one session, one
connection.

## 4. Cancel one import

**■ Cancel** mid-run:

```
← request  cancelImportButton_Click  _importCts.Cancel() → job J-… sees the token at its next record (≤ 25 ms)
• server   import                    job J-… cancelled after record 71 (OperationCanceledException caught inside the task)
→ push     Application.Update(this, …)  Job J-…: 71 records in 2.24 s — cancelled · 8 pushes (final state, finally block)
```

Start is enabled again — the `finally` block did that, not the happy path.

## 5. Start another import and force a simulated failure

Tick **Throw at record 87**, then **▶ Start import**:

```
• server  import   job J-38C419 FAILED on record 87: InvalidOperationException caught inside the task → server log, safe message
→ push    Application.Update(this, …)  Job J-38C419: 86 records in 2.41 s — failed · 9 pushes (final state, finally block)
```

On screen: *Import failed. Review the server log.* In the server console: the full exception with the job id and
the session id. The UI is usable and Start works again.

## 6. Generate new ticket events and show both sessions update

Tab A (Contoso) **Ticket board → Publish (my tenant)**, then **Escalate selected**:

```
• server  hub.AddOrUpdate  #4831 → TicketChanged (tenant Contoso, subscribers 2) · event c46df1fd · fanned out on a thread-pool thread
→ push    Application.Update(_context)  event c46df1fd · Added #4831 · published by THIS session — applied here
```

The **same event id** appears in every Contoso session's trace, marked *published by another session <id>* there.
Escalation is the one change type that also pops a toast (top right).

## 7. Apply a filter in one session and show the other is independent

Tab B is on `Northwind`, so tab A's Contoso publish arrives and is dropped there:

```
• server  filtered out  (tenant Contoso ≠ this session's Northwind) event … — dropped, nothing rendered
```

Tick **Escalated only** in tab A: its board shrinks, tab B is unaffected. Same hub, same events, two different
screens — because the filter lives in the session.

## 8. Disable live mode and explain polling/cadence behaviour

**Cadence** tab → tick **Live mode**. The model changes every 50 ms; the UI applies one update per tick:

```
← request  refreshTimer_Tick  applied model v266 · 266 event(s) coalesced into 14 update(s) → the changes ride back with THIS timer request
```

Change the cadence to 250 ms and to 5 sec and watch the ratio move. Untick live mode:

```
• server  stop  DashboardSimulator.Stop() + refreshTimer.Stop() — a timer nobody stops keeps a session busy forever
```

To show the fallback, stop the app, set `"enableWebSocket": false` in `Default.json`, restart: the strip stays
`○ HTTP only`, the Health tab reads *Fallback mode* / *Polling enabled*, and the same pushes arrive on the polls.

## 9. Present the production checklist

**Health · config** tab:

- `● Push expected (WebSocket)` / `○ Polling off`
- `active subscriptions: 2 (hub 1 + sessions 1)` — the number a leak would grow
- `63 updates/min · last 0.9 s ago · loops: heartbeat, dashboard simulator, refreshTimer @ 1 s`
- the effective `Default.json` values and the `HealthCheck.json` thresholds
- the ten-point checklist; ticking an item writes it to the trace

Then close tab B and show, in tab A, that the registry drops it and `active subscriptions` falls — every
subscription had an unsubscribe.

## Troubleshooting during the demo

| Symptom | Cause | Fix |
|---|---|---|
| A "Session Timeout" dialog appears | `sessionTimeout` 180 s and no requests were made (pushes do not count as activity) | click OK; raise `sessionTimeout` for a long demo |
| The board does not update in the background tab | the browser paused the tab's rendering | bring the tab to the front; the server state was correct all along |
| Nothing arrives after "Unsubscribe" | that is the point — the session has no subscription | click **Subscribe**; the snapshot reloads |
| The cadence looks slower than 250 ms in a background tab | browsers throttle timers in hidden tabs | keep the tab in front when comparing cadences |
