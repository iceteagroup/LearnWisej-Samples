# Where state lives (Module 2 deliverable)

Four legitimate places, one trap. The lesson's table, applied to every value this sample holds.

## The four places

| Place | Scope | Good for | Cost / risk |
|---|---|---|---|
| **Control state** | one page of one session | presentation state: a label's text, a selected row, a progress value, a list of log lines | dies with the page; do not use it as the source of truth for domain data |
| **`Application.Session`** | one session | lightweight session metadata: a counter, the selected tenant, a preference, a small per-session object | dynamic bag — a typo compiles; no type safety; gone at exit/timeout/refresh |
| **Session services** | one session | the same thing with structure and testability (a per-session object created once and reused) | needs a creation/ownership convention |
| **Global services** | the whole server process | queues, hubs, caches, watchers, "who is online" | must be thread-safe, must not hold pages or contexts, must have a subscription lifecycle |

## The static-field trap

A `static` field is a global service without the discipline of one. It is shared by every session, it is never
cleaned up, and nothing in the code says so. The lesson lists the three classic bugs:

- `static string CurrentTicketId` — every user overwrites every other user's current ticket.
- `static List<MainPage> Pages` — the server keeps sessions alive after the users leave (a memory leak that also
  keeps pushing to dead UIs).
- `static CancellationTokenSource _cts` — one user cancels another user's job.

This sample ships the trap on purpose, next to the correct version, so it can be *seen*:

```csharp
private static int _sharedCounter;                 // ✖ ONE field for the whole server
…
Application.Session.Counter = (int)Application.Session.Counter + 1;   // ✓ one value per session
```

`Static counter +1 (trap)` and `Session counter +1` sit side by side in the action bar, and both values are printed
in the inspector and in **SERVER STATE**. Open two tabs, click the static button in one, and the other tab's number
moves too. Refresh: the session counter resets to 0, the static one keeps counting for as long as the server process
lives. That is the entire lesson in two labels.

Use `static` for a *truly global* service (`SessionRegistry.Instance` here — thread-safe, values only, with a
subscription lifecycle), never for a per-user value.

## Every value in this sample

| Value | Lives in | Why there | Dies when |
|---|---|---|---|
| `counterLabel` value | `Application.Session.Counter` | per-session metadata, exactly the lab's requirement | the session ends (exit, timeout, F5) |
| `sharedCounterLabel` value | `static int _sharedCounter` | **the trap**, shown deliberately | the server process stops |
| lifecycle log lines | control state (`lifecycleListBox`) | presentation only; nobody else needs them | the page is disposed |
| push trace lines | control state (`listTrace`) | presentation only | the page is disposed |
| banner / status text | control state (`labelBanner`, `labelStatus`) | presentation only | the page is disposed |
| `_context` (`Application.Current`) | instance field of `MainPage` | it addresses *this* session; a static one would push into the wrong UI | the page is disposed |
| `_clientId`, `_registered`, `_cleanedUp`, `_tasks`, `_pushes`, `_pushers`, `_polling` | instance fields of `MainPage` | per-session bookkeeping — one `MainPage` per browser tab | the page is disposed |
| the live-session list | `SessionRegistry` (global service) | it is genuinely server-wide state, and it is values only | the process stops (rows leave on unregister) |
| `SessionInfo` (session id + start time) | inside the registry | a value object; never a page, never a context | its session unregisters |

## Reading the registry back into a UI

A global service must not know about controls, so the flow is inverted: the service raises an event carrying values,
and each session decides what to do with it *in its own context*.

```
SessionRegistry (global, locked dictionary)
        │  Register / Unregister
        ▼
   SessionsChanged  ──► Task.Run  ──► subscriber (a thread-pool thread, NO session context)
                                          │  if (IsDisposed) return;
                                          ▼
                                Application.Update(_context, () => { liveSessionsLabel.Text = …; })
                                          │
                                          ▼
                                   push over the WebSocket
```

## Evidence (expected; not verified in a browser)

- SERVER STATE line 2: `session.Counter=3 static _sharedCounter=7 (server-wide!) liveSessions=2 …` — in the second
  tab the same line reads `session.Counter=0 static _sharedCounter=7 …`. Same static number, different session number.
- After F5: `session.Counter=0`, `static _sharedCounter=7` unchanged, and the session id in line 3 is a new one (the client id stays — it is the browser, not the session).
- `liveSessionsLabel`: `live sessions: 2 · a1b2c3d4 (this one)@10:12:03  e5f6a7b8@10:12:41`.
