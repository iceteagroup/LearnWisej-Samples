# Disposal checklist

One rule, then the list: **whatever a screen subscribes to, starts, binds or allocates, the same screen
releases — in its own `Dispose(bool disposing)`, in the same class.** Not in the caller, not in a
`FormClosed` handler, not "when the session ends".

`Dispose(bool)` is the single place because every way of closing arrives there: the Close button, the
window **X**, `Application.Exit`, and the session timing out. A `FormClosed` handler is a second path
that one of those will eventually miss.

```csharp
protected override void Dispose(bool disposing)
{
    if (disposing && !_released)
    {
        _released = true;

        GlobalTicketBus.TicketChanged -= OnTicketChanged;   // 1. static events
        if (_refreshTimer != null)                          // 2. timers and background work
        {
            _refreshTimer.Stop();
            _refreshTimer.Elapsed -= OnRefreshTimerElapsed;
            _refreshTimer.Dispose();
            _refreshTimer = null;
        }
        bindingSource1.DataSource = null;                   // 3. bindings and the rows behind them
        _largeImage?.Dispose();                             // 4. images and other IDisposables
        _largeImage = null;
        _attachmentBuffer = null;                           // 5. large buffers
        components?.Dispose();                              // 6. the designer container
    }

    base.Dispose(disposing);
}
```

## The list

| What | Why it roots the screen | What to write |
|---|---|---|
| **Static events** (`GlobalTicketBus.TicketChanged`) | the invocation list lives for the process; every delegate holds its target | `-=` the exact same method you `+=`d |
| **Events on longer-lived objects** (a service, a cache, the shell) | the publisher outlives the subscriber | same rule; if the publisher is a singleton, treat it as static |
| **Timers** (`System.Timers.Timer`, `Wisej.Web.Timer`) | a running timer is rooted by the runtime, and its callback captures `this` | `Stop()`, unsubscribe the handler, `Dispose()`, null the field |
| **Background work** (`Application.StartTask`, `Task.Run`, polling loops) | the captured closure holds the form until the work finishes | cancel it (a `CancellationTokenSource` the form owns) and dispose the source |
| **Bindings** (`BindingSource.DataSource`) | the binding source holds the list, the list holds every row | set `DataSource = null` before the base call |
| **Images, streams, fonts, brushes** | unmanaged handles that a collection alone will not release | `Dispose()`, then null |
| **Large buffers** (`byte[]`, cached DataTables) | they are the megabytes in "MB per session" | null the field |
| **The designer container** (`components`) | it owns the non-visual components on the form | `components?.Dispose()` — the designer's own `Dispose` does exactly this, and when you override `Dispose(bool)` yourself you take over that job |
| **Static caches keyed by session or user** | nothing ever removes the entry | do not write them; if you must, use a bounded cache with eviction and prove the bound |

## The three checks the lab asks for

- **The timer callback checks `IsDisposed` before touching controls.** Not to hide a bug — the timer is
  stopped in `Dispose`, so it can fire at most once more, in the window between the last tick and the
  stop. The check turns that race into a no-op instead of an unobserved `ObjectDisposedException` on a
  thread nobody is watching.
- **Closing while a refresh is in flight does not throw.** The `_released` flag is set first, so the
  callback returns immediately; the `ObjectDisposedException` catch is the belt to that braces.
- **The window X and the Close button follow the same path.** They both end in `Dispose(bool)`. There is
  nothing in `btnClose_Click` except `Close()` — anything else there would be a second path that the X
  does not take.

## What to check in a review

1. Search the class for `+=` and for `new System.Timers.Timer` / `StartTask` / `Task.Run`. Every hit
   needs a matching line in `Dispose`.
2. Search the project for `static` events and static collections. Each one is a permanent root until
   something removes the entry.
3. Open and close the screen fifty times, take two snapshots and compare the instance count.
   Fifty instances still there is a leak; "forms disposed 50" is not evidence of anything.
