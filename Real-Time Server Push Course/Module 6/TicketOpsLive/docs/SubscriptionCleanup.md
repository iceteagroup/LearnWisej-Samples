# Subscription cleanup — every subscription has an unsubscribe

The course rule: **if you subscribe, you unsubscribe.** In a real-time app a leaked handler is not only a memory
problem — the service keeps calling a session that is gone, so it is a correctness problem too.

## Every subscription this page makes

| # | Subscription | Where it is made | Where it is removed | Guard |
|---|---|---|---|---|
| 1 | `TicketHub.Instance.TicketChanged += Hub_TicketChanged` | `Subscribe()`, called from `MainPage_Load` and from `subscribeButton_Click` | `Unsubscribe()`, called from `unsubscribeButton_Click`, `Application_ApplicationExit` and `MainPage_Disposed` | `_subscribed` makes both directions idempotent |
| 2 | `Application.ApplicationExit += Application_ApplicationExit` | `MainPage_Load`, once (`_exitHooked`) | `MainPage_Disposed` (`-=`, inside `try/catch`) | `_exitHooked` |
| 3 | `this.Disposed += MainPage_Disposed` | the constructor | never — it dies with the page, and it is the page's own event | — |
| 4 | designer event wiring (`Click`, `SelectedIndexChanged`, `SelectionChanged`, `Load`) | `InitializeComponent()` | never — the page owns those controls and disposes them with itself | — |

Only #1 crosses the session boundary, and it is the only one that can leak. #2 is a session-scoped static event, so
it is detached when the page goes away; #3 and #4 point from the page to the page.

## The three-part guard

```csharp
// (a) subscribe once
private void Subscribe(string reason)
{
    if (_subscribed) return;                       // a second subscription would render every event twice
    _hub.TicketChanged += Hub_TicketChanged;
    _subscribed = true;
}

// (b) unsubscribe once, from whichever path gets there first
private void Unsubscribe(string reason)
{
    if (!_subscribed) return;
    _hub.TicketChanged -= Hub_TicketChanged;
    _subscribed = false;
}

// (c) never touch a dead page, even if a race got past (a) and (b)
private void Hub_TicketChanged(object sender, TicketChangedEventArgs e)
{
    if (this.IsDisposed) return;
    …
    try { Application.Update(_context, …); } catch (ObjectDisposedException) { }
}
```

`(c)` is the belt to `(a)`/`(b)`'s braces. There is a genuine race: a session can be torn down between the
`IsDisposed` check and the push. `SafeUpdate` swallows the `ObjectDisposedException`, and even if it did not, the
hub's `Raise` catches whatever a handler throws, logs it and continues to the next subscriber.

## What happens on each lifecycle event

| Event | What fires | Result |
|---|---|---|
| **Unsubscribe clicked** | `unsubscribeButton_Click` → `Unsubscribe("operator")` | the handler is removed; `hub subscribers` drops by one; this tab receives nothing while the others keep updating; **Subscribe** re-enables |
| **Tab closed** | the session ends → `Application.ApplicationExit` → `Unsubscribe("ApplicationExit")` | `hub subscribers` drops by one, visible in the other tabs on their next event |
| **Browser refresh (F5)** | the old session ends (`ApplicationExit`) and a **new** session starts (`MainPage_Load` → `Subscribe`) | the count dips and comes back — a refresh is a new session with a new `SessionId` (the `ClientId` stays: it is the browser), not the same one |
| **Page replaced in a live session** | `MainPage.Disposed` → `Unsubscribe("page disposed")` + `ApplicationExit -=` | the trace list is already gone, so the line goes to the server console instead |
| **Session timeout** | Wisej ends the session → `ApplicationExit` | same as "tab closed" |
| **A subscriber throws** | `TicketHub.Raise` catches it | logged as `TicketHub: subscriber failed while handling event …`; the loop continues with the next session |

`Unsubscribe` is written so that the two shutdown paths (`ApplicationExit` and `Disposed`) can both run, in either
order, without double-decrementing anything: the second call sees `_subscribed == false` and returns. The hub's
`remove` accessor is defensive in the same way — it only decrements when the delegate really changed.

## What a leak would look like

`hub subscribers` is printed in every tab and refreshed on every event. Close a tab and watch the number in the
remaining tabs: if it did **not** go down, the hub would still be holding a handler that points at a dead
`MainPage` — the page, its grid, its `BindingList` and every `Ticket` in it would stay alive for as long as the
process, and every future publish would walk into a disposed page.

## Evidence

Three tabs: A and B on **Contoso**, C on **Northwind**.

1. All three loaded — each trace shows
   `• server hub.TicketChanged += subscribed (page load) · hub subscribers = 3`
   (each tab shows the count as it was when *it* subscribed: 1, 2, 3).
2. Tab B clicks **Unsubscribe**:
   `• server hub.TicketChanged -= unsubscribed (operator) · hub subscribers = 2 · this session now receives nothing`,
   `subscribersLabel` turns red and reads `hub subscribers: 2 (this session NOT subscribed)`.
3. Tab A publishes: A renders it, **B shows nothing at all** (no trace line, no notification, no counter movement),
   C counts it as filtered out. B's `hub subscribers` still reads 2 until B itself does something — it is a snapshot
   taken when the label was last rendered, which is exactly the point: an unsubscribed session gets no pushes.
4. Tab B clicks **Subscribe** → back to 3, and the next publish reaches it again.
5. Close tab B, then publish from A: A's trace reads `subscribers 2`, and the server console shows
   `[TicketOpsLive] … TicketHub: unsubscribed (ApplicationExit) · subscribers now 2`.

*(Expected behaviour — this sample was built and compiled, not executed; run it and confirm the counts.)*
