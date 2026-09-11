# Deliverable 3 — Progress observer UI

> **How often does the UI update?** At most **1000 / 400 = 2.5 times per second**, whatever the job does.
> The number is a constant in one file: `JobProgressObserver.MinPushIntervalMs`.

## The rule

Wisej.NET can change the browser without a request. `Application.StartTask` runs work on a background
thread that keeps the session context; `Application.Update(component)` flushes pending control changes over
the WebSocket from that thread. Used carelessly, the same two calls turn a two-minute import into a flood of
round trips.

So the sample splits the responsibility in two:

- **The job publishes milestones.** `queued · started · validated · every completed batch · failed ·
  canceled · completed` — meaningful state, a dozen events for a thousand rows.
- **The observer decides when to push.** It never pushes because an event arrived; it pushes because the
  screen is stale and enough time has passed.

## The implementation

`Services/Jobs/JobProgressObserver.cs`

```csharp
_store.Changed           += (s, e) => _dirty = true;   // worker thread
_notifications.Published += (s, e) => _dirty = true;
```

Those handlers run on the queue worker, which has **no session**. They set a flag. That is all they are
allowed to do.

```csharp
Application.StartTask(PushLoop);            // started from a request thread → inherits this session

private void PushLoop()
{
    while (!_disposed && !_target.IsDisposed)
    {
        if (_dirty)
        {
            _dirty = false;
            _refresh();                     // rebind the grid, bar, status, detail, bell
            Application.Update(_target);    // one flush
        }
        else if (idle.Elapsed > IdleTimeout) break;
        Thread.Sleep(MinPushIntervalMs);
    }
}
```

Details that matter in production:

- **Coalescing, not queueing.** `_dirty` is a bool. A thousand row events between two ticks produce one
  push, so the cost of the UI is bounded by the clock and not by the job.
- **`IsDisposed` checks and `catch (ObjectDisposedException)`.** The page can vanish between two pushes;
  the loop exits quietly and the job never notices.
- **It stops when there is nothing to watch.** After 20 s with no change the loop ends; any user action
  (`Start()` / `Invalidate()`) restarts it. No session keeps a thread spinning overnight.
- **One observer per page**, disposed by the page's `Dispose` (which also unsubscribes from the store).

## What the observer paints

`ImportCenterPage.RefreshAll()`:

| Control | Source |
|---|---|
| `dgvJobs` | `ImportService.ListJobRows` — a `JobQueueRow` projection, tenant-scoped |
| `prgJob` / `lblPercent` | `JobRecord.Percent` of the active job |
| `lblStatus` (status bar) / `lblBanner` | `JobRecord.Status` + `Message` |
| `pnlJobDetail` | `JobRecord.History` + `Result` (repainted only when the history grew) |
| `lstNotifications` / `btnBell` | the notification records for this user |

## Evidence in the running app

- Start `contoso_q2.csv` and watch `prgJob`: it moves in ten steps (one per batch), not in a thousand.
- The job detail's status history gains one `Job:` line per batch — the milestones the observer pushed.
