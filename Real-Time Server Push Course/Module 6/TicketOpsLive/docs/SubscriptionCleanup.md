# Subscription cleanup — every subscription has an unsubscribe

If you subscribe, you unsubscribe. In a real-time app a leaked handler is not only a memory problem — the service
keeps calling a session that is gone, so it is a correctness problem too.

## Every subscription this page makes

| # | Subscription | Where it is made | Where it is removed | Guard |
|---|---|---|---|---|
| 1 | `TicketHub.Instance.TicketChanged += Hub_TicketChanged` | `Subscribe()`, from `MainPage_Load` and `subscribeButton_Click` | `Unsubscribe()`, from `unsubscribeButton_Click`, `Application_ApplicationExit` and `MainPage_Disposed` | `_subscribed` makes both directions idempotent |
| 2 | `Application.ApplicationExit += Application_ApplicationExit` | `MainPage_Load`, once (`_exitHooked`) | `MainPage_Disposed` (`-=`, inside `try/catch`) | `_exitHooked` |
| 3 | `this.Disposed += MainPage_Disposed` | the constructor | never — it is the page's own event | — |
| 4 | designer event wiring (`Click`, `SelectedIndexChanged`, `Load`) | `InitializeComponent()` | never — the page owns those controls | — |

Only #1 crosses the session boundary, and it is the only one that can leak.

## The three-part guard

```csharp
// (a) subscribe once
private void Subscribe()
{
    if (_subscribed) return;                       // a second subscription would render every event twice
    _hub.TicketChanged += Hub_TicketChanged;
    _subscribed = true;
}

// (b) unsubscribe once, from whichever path gets there first
private void Unsubscribe()
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

A session can be torn down between the `IsDisposed` check and the push: `SafeUpdate` swallows the
`ObjectDisposedException`, and the hub's `Raise` catches whatever a handler throws, logs it and continues.

## What happens on each lifecycle event

| Event | What fires | Result |
|---|---|---|
| **Unsubscribe clicked** | `unsubscribeButton_Click` → `Unsubscribe()` | the handler is removed; this tab receives nothing while the others keep updating |
| **Tab closed / session timeout** | the session ends → `Application.ApplicationExit` → `Unsubscribe()` | the hub no longer calls this session |
| **Browser refresh (F5)** | the old session ends (`ApplicationExit`) and a new one starts (`MainPage_Load` → `Subscribe()`) | a new session with a new subscription |
| **Page replaced in a live session** | `MainPage.Disposed` → `Unsubscribe()` + `ApplicationExit -=` | same cleanup, from the other end |
| **A subscriber throws** | `TicketHub.Raise` catches it | logged on the server console; the loop continues with the next session |

`ApplicationExit` and `Disposed` can both run, in either order: the second call sees `_subscribed == false` and returns.

## What a leak would look like

Without the `-=`, the hub would hold a handler that points at a dead `MainPage`: the page, its grid, its
`BindingList` and every `Ticket` in it would stay alive for as long as the process, and every future publish would
walk into a disposed page.
