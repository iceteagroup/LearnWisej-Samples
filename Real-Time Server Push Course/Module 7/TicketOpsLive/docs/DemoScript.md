# Demo script — the capstone in nine steps

Run it with two browser tabs (A and B).

## 0. Setup

```bash
cd "D:/Projects/LearnWisej-Samples/Real-Time Server Push Course/Module 7/TicketOpsLive"
dotnet run -f net10.0 --urls http://localhost:5307
```

Open <http://localhost:5307> (tab A), then **Session → Open second window** (tab B).

## 1. Open TicketOps Live in two browser sessions

Tab A's **Session** tab logs `Session xxxxxxxx joined · 2 live session(s)` without anyone clicking there: the global
registry raised an event on a thread-pool thread and tab A re-entered its own context to render it. Both tabs show
the same client id (the browser) and different session ids.

## 2. Start the heartbeat

**▶ Start heartbeat** in tab A: the clock, the load bar and `Heartbeat #n` move once per second with no request
from the browser. The button stays disabled while the loop runs.

## 3. Start an import and show progress

**Import monitor → Start Import**: the bar, `n/200` and the elapsed time move while the heartbeat keeps running;
the log shows "Import started" and "Imported 50/100/150 records".

## 4. Cancel one import

**Cancel Import** mid-run: "Import cancelled by user.", "Cancelled after record n" and the summary line. Start is
enabled again — by the `finally` block.

## 5. Start another import and force a simulated failure

**Fail at 87**: the import stops at record 87 with "Import failed. Review the server log."; the server console has
the full exception with the job id and the session id. The UI is usable and Start works again.

## 6. Generate new ticket events and show both sessions update

Both tabs on **Contoso** (**Ticket board**). In tab A click **Publish Event**, then **New ticket events (10)**: the
new rows and the notifications appear in tab A *and* tab B. **Escalate selected** also pops a toast in every Contoso
session.

## 7. Apply a filter in one session and show the other is independent

Switch tab B's tenant to **Northwind**: tab A's Contoso publishes no longer appear there. Tick **Escalated only** in
tab A: its board shrinks, tab B is unaffected. Same hub, same events, two different screens — the filter lives in
the session.

## 8. Disable live mode and explain polling/cadence behaviour

**Cadence → Live mode**: the model changes every 50 ms; `EVENTS RECEIVED` climbs about 20/s while `UPDATES APPLIED`
climbs once per tick. Change the cadence to 250 ms and 5 sec and compare. Untick live mode: the simulator and the
timer stop, and polling (if it was on) ends.

To show the fallback, stop the app, set `"enableWebSocket": false` in `Default.json`, restart: `connectionLabel`
shows *No WebSocket*, the Health tab reads *Fallback mode* / *Polling enabled* while work runs, and updates arrive
on the polls.

## 9. Present the production checklist

**Health · config**: *Push expected (WebSocket)* / *Polling off*, `Active subscriptions`, the measured
`updates/min` with the running loops, the effective `Default.json` values and the `HealthCheck.json` thresholds, and
the ten-point checklist. Then close tab B and show, in tab A, that the registry drops it and `Active subscriptions`
falls — every subscription had an unsubscribe.

## Troubleshooting during the demo

| Symptom | Cause | Fix |
|---|---|---|
| A "Session Timeout" dialog appears | `sessionTimeout` 180 s and no requests were made (pushes do not count as activity) | click OK; raise `sessionTimeout` for a long demo |
| The board does not update in the background tab | the browser paused the tab's rendering | bring the tab to the front; the server state was correct all along |
| Nothing arrives after "Unsubscribe" | the session has no subscription | click **Subscribe**; the snapshot reloads |
| The cadence looks slower than 250 ms in a background tab | browsers throttle timers in hidden tabs | keep the tab in front when comparing cadences |
