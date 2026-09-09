# The three update mechanisms — what the sample shows

| Mechanism | Trigger | Code | Delivery | Cost |
|---|---|---|---|---|
| **1 · In-request** | a browser event (click, timer tick) | `refreshButton_Click`: `statusLabel.Text = "Refreshed at …"` | the response of that request | none beyond the request itself |
| **2 · Out-of-bound WebSocket push** | server-side work outside a request | `Application.StartTask(() => { …; Application.Update(this); })` | a WebSocket frame the moment `Update` is called | serialization + frame + browser work **per push** |
| **3 · Polling fallback** | no WebSocket (proxy, old browser, policy) | `Application.StartPolling(1000)` while a task runs, `EndPolling()` after | the next poll carries whatever is pending | one HTTP request per second **per session** while active |

## Push is event-driven, polling asks repeatedly

The trace makes the difference visible. With the WebSocket up, the five steps of the server event arrive 600 ms
apart with no browser request in between (`→ push` lines only). With `"enableWebSocket": false`, the browser console
prints `Wisej: Poll request.` every second and the same steps arrive on the next poll — the app still works, later and
at a higher cost. That is the lesson's rule: design for WebSocket first, then degrade predictably.

## Two ways to write the task

```csharp
// (a) the walkthrough's shape — direct control changes, then push
Application.StartTask(() =>
{
    for (int i = 1; i <= 5; i++)
    {
        pushStatusLabel.Text = $"Server event step {i}/5";
        Application.Update(this);
        Thread.Sleep(600);
    }
});

// (b) the heartbeat's shape — compute, then apply + push in one flush
Application.StartTask(() =>
{
    while (_running && !this.IsDisposed)
    {
        int load = random.Next(5, 95);                 // work, off the request thread
        Application.Update(this, () =>                 // apply in context, push once
        {
            clockLabel.Text = DateTime.Now.ToString("HH:mm:ss");
            serverLoadBar.Value = load;
            activityLabel.Text = $"Heartbeat #{n}";
        });
        Thread.Sleep(1000);
    }
});
```

Both work because `StartTask` keeps the session context. Shape (b) is the one to reuse when the code that raises the
change does **not** run inside `StartTask` (a global hub, a `System.Threading.Timer` of a service): capture
`Application.Current` while in context and call `Application.Update(context, () => …)` — Modules 2 and 6.

## Where `StartPolling` belongs — a finding

The lesson places `Application.StartPolling(1000)` in `MainPage_Load` with the comment *"ignored when WebSocket is
available"*. At runtime `Application.IsWebSocket` is **false during Load** — the socket is opened by the client right
after the first response — so the call is not ignored: the browser starts polling every second and keeps doing so
after the socket connects (verified: `Wisej: Poll request.` once per second, `POST app.wx` in the server log, while
`IsWebSocket` reads `true` on every later event). The sample therefore requests polling where the out-of-bound work
starts, only if `IsWebSocket` is false at that moment, and ends it when the last task finishes (`BeginPush` /
`EndPush` in `MainPage.cs`).

## Cadence experiment (verified numbers)

| Run | Model changes | Pushes | Wall time |
|---|---|---|---|
| Burst 100 @ 10 ms (anti-pattern) | 100 | **100** | ≈ 1.57 s |
| Batched (push every 10th) | 100 | **10** | ≈ 1.58 s |

Same result on screen, one tenth of the traffic. Rules the sample applies: push the final state immediately (the
`finally` block always pushes), push visible progress at a controlled interval, batch several control changes into
one `Application.Update`, never push inside a tight loop.

## Common mistakes the sample guards against

- Updating a control from a thread without the session context → only `StartTask` threads or `Application.Update(context, …)` touch controls.
- Pushing too often → the heartbeat pushes once per second; the experiment shows the cost of 10 ms.
- Static fields for session data → `_running`, `_heartbeat`, `_pushes` are instance fields; SERVER STATE prints the session ids to prove the page is per session.
- Silent failures in tasks → `catch` logs the exception to the server console and shows a safe message; `finally` restores the buttons.
- Loops without a stop condition → `_running && !this.IsDisposed`, `Disposed` handler, bounded experiments.
