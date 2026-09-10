# Two-session test (lab step 5) — prove isolation before user acceptance testing

The bug this module removes is invisible with one browser tab. The test therefore always has two
sessions: **A = kelly** (Acme · Northwind Traders · Open) and **B = sam** (Globex · Fabrikam Inc ·
Invoiced), the same pair the video uses. The app offers the test three ways.

## 1. The real test — two browser tabs

Each tab is its own Wisej.NET session (`Application.SessionId` differs; `SessionCount` shows 2 in the
Sign-in card after a `Read back`).

| Step | Tab A (kelly) | Tab B (sam) | Expected |
|---|---|---|---|
| 1 | open `http://localhost:5104`, **Sign in** as kelly | | A: read-back kelly · Acme · Northwind Traders · Open |
| 2 | | open a second tab, **Sign in** as sam | B: sam · Globex · Fabrikam Inc · Invoiced |
| 3 | keep **Legacy static (AppState) ✕** selected, click **Read back** | | **corruption:** read-back shows `sam · Globex · Fabrikam Inc · Invoiced`; trace `✖ fail AppState.CurrentUser  "sam" but this session signed in as "kelly" — another session overwrote the static`; red banner |
| 4 | select **Session context (UserContext) ✓**, **Read back** | | **isolated:** read-back shows `kelly · Acme · Northwind Traders · Open`; trace `• server UserContext (session)  user = kelly · … · customer = Northwind Traders · filter = Open` |
| 5 | | in session mode change **Filter** to `Shipped` | B: `✓ ok UserContext.Current.ActiveFilter =  Shipped   ✓ UserContext#bbbb only` |
| 6 | **Read back** (session mode) | | A still `Open` — "changing the filter in one does not affect the other" |
| 7 | | in static mode change **Customer** to `Globex Corp` | |
| 8 | static mode, **Read back** | | A reads `Globex Corp` — the leak, once more |
| 9 | **Sign out** | | A: read-back `—` in session mode; B is still signed in (its own session) |

Pass criterion: steps 4, 6 and 9 show no cross-session effect through `UserContext`; steps 3 and 8
show the effect through `AppState` (the static is kept in the sample only to demonstrate this).

## 2. The single-tab simulation — `Corrupt from here ✕` / `Use UserContext ✓`

For a reviewer with one tab the two buttons replay the sequence in-process:

- `Corrupt from here ✕` re-seeds kelly's selection in the static, then an `Application.StartTask`
  plays session B: `AppState.CurrentCustomer = Fabrikam Inc; AppState.ActiveFilter = "Invoiced"`.
  Kelly's read-back (pushed with `Application.Update(this)`) shows Fabrikam / Invoiced and the banner
  says *sam's selection overwrote kelly's*. This is not a simulation of the **static** — it is the real
  static; only the second session is simulated.
- `Use UserContext ✓` does the same through `Application.Session`. Session B is stood in by a second
  `UserContext` object (what `Application.Session` hands the other tab); session A is read back from
  `UserContext.Current` **inside the background task**, which still resolves to kelly's object.
  Result: `✓ isolated — UserContext#aaaa ≠ UserContext#bbbb`.

## 3. The timed replay — `Two-session replay ▶`

A `Wisej.Web.Timer` (700 ms) writes the video's trace lines one by one:

```
• server  A   UserContext.Current → session a4f9…  · UserContext#aaaa · kelly · Northwind Traders · Open
• server  B   UserContext.Current → session (2nd tab, simulated) · UserContext#bbbb · sam · Fabrikam Inc · Open
• server  B   B sets filter = "Invoiced"
✓ ok      A   A filter unchanged = "Open" ✓ isolated
• server  B   same test against the static: B sets AppState.ActiveFilter = "Invoiced"
✖ fail    A   A reads AppState.ActiveFilter = "Invoiced" ✖ leaked across sessions
★ log     two-session test   UserContext isolated ✓ · AppState leaked ✕ — prove isolation with two real tabs before user acceptance testing
```

The State store toggle switches to the session store at step 4 and back to the static at step 6 so the
read-back panel shows both outcomes. Clicking the button while it runs stops it.

## What "passed" means for the migration log

`M4: … two-session test passed` is written only after variant 1 (real tabs) shows steps 4, 6 and 9
clean. The simulations exist so the argument can be made in a demo; the tabs are the proof.

## Evidence (what the running app shows)

- `SessionCount 2` in the Sign-in card after the second tab signs in and `Read back` is clicked.
- The static-mode `Read back` after step 2 prints the red banner
  `✖ AppState.CurrentUser is "sam" — this session signed in as "kelly": another session overwrote the shared static.`
- The session-mode `Read back` prints `✓ Both stores agree …` only while no other session has signed in;
  with sam signed in it prints kelly's own values from `UserContext` and the static discrepancy above.
