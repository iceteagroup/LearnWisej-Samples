# Session cleanup — timeout and logout requirements (lab step 6)

On the desktop nothing had to be cleaned up: closing the window ended the process and Windows freed
everything. On the server the process lives on; a browser tab can be abandoned, a laptop can sleep, a
network can drop. Whatever a user left behind must be released either when the workflow completes or
when the session ends.

## Session lifecycle in Wisej.NET (what the app relies on)

| Moment | Wisej.NET | OrderDesk hook |
|---|---|---|
| Browser opens the app | `Program.Main(NameValueCollection)` runs **once per session** | creates `MainPage`, subscribes the two events below (closure over the page — never a static) |
| User works | requests + WebSocket keep the session alive (built-in keep-alive while the tab is connected) | – |
| Tab idle / disconnected past the timeout | `Application.SessionTimeout` fires first (`HandledEventArgs`; the default shows a "prolong session?" dialog, `e.Handled = true` suppresses it) | `SessionLifecycle.OnSessionTimeout` — traces `Application.SessionTimeout  session <id> about to expire · e.Handled = false → Wisej shows its prolong dialog` |
| Session ends (timeout elapsed, tab closed for good, `Application.Exit()`) | `Application.ApplicationExit` | `SessionLifecycle.OnApplicationExit` — `UserContext.Clear()` + `Console.WriteLine` + trace (if the page is still there) |
| User clicks Sign out | – | `UserContext.Clear()`, UI back to the Sign-in card, toast "Signed out — this session only." |
| Page refresh | `Application.ApplicationRefresh`; the session survives | nothing to clear; `MainPage_Load` restores the signed-in user from `UserContext.Current` |

## Requirements checklist

**On logout (Sign out)**
1. `UserContext.Clear()` — user, company, customer, filter, last search are gone; the next
   `UserContext.Current` creates an empty context. (Video pitfall: "letting session state survive
   logout or user switching".)
2. Do **not** touch `AppState` statics on logout — setting `AppState.CurrentUser = null` would sign out
   every session. The trace says so on every Sign out:
   `★ log AppState.CurrentUser  left as "kelly": setting the static to null would sign out EVERY session`.
3. Release anything the user holds: open `EditOrderDialog` instances (Module 3: `using` blocks already
   dispose them), pessimistic locks on orders, half-finished imports (Module 6).
4. Return to the Sign-in card; do not reuse controls that show the previous user's data (the read-back
   panel is refreshed from the now-empty store and shows `—`).

**On timeout**
1. Trace/log it (done) — operations need to see abandoned sessions.
2. Decide per app whether to suppress the built-in prolong dialog (`e.Handled = true`) — OrderDesk keeps
   it: a clerk on the phone should not lose an edit.
3. Nothing user-visible can happen after this point that needs the browser; anything pending must
   already be server-side (report jobs → `ReportQueue` in Module 6).

**On ApplicationExit (the session is gone)**
1. `UserContext.Clear()` (done) — releases the references held by the session bag.
2. Release server resources keyed by session id: temp files under `App_Data/tmp/<session>` (none in
   this module), locks, background tasks started with `Application.StartTask` (check `IsDisposed`
   before touching controls — every task in `MainPage.cs` does).
3. Never trace/update the UI without guarding: the page may already be disposed, so
   `MainPage.TraceLifecycle` checks `IsDisposed` and swallows the failure; the console line is the
   durable record.

**Keep-alive / affinity notes**
- The WebSocket keeps a connected tab alive indefinitely; the timeout only starts when the tab is
  closed or the connection drops. Test the timeout by closing the tab and watching the console.
- With more than one server instance, sessions need sticky routing (Module 7 deployment checklist);
  `Application.Session` lives in that process' memory.

## Evidence (what the running app shows)

- On load: `• server Program.Main  Application.ApplicationExit += … UserContext.Clear()   Application.SessionTimeout += … trace`.
- `Sign out`: the three lines quoted above, the read-back resets to `—` in session mode, and in static
  mode it still shows kelly — the static did not notice the logout (which is the point).
- Closing the tab prints `[OrderDesk] ApplicationExit · session <id> · UserContext cleared, per-user state released`
  in the server console (the page is gone, so no trace line). `SessionTimeout` prints the corresponding
  console line and, if the page is still alive, the trace line — this path is wired and compile-checked
  but was not exercised while building the sample (see README, "Notes for the reviewer").
