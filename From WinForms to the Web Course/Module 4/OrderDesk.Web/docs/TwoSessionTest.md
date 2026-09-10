# Two-session isolation test (lab step 5)

Lab step covered: *test two browser sessions with different users*. Deliverable 3 of the storyboard: "two-session
isolation test — proven before user acceptance testing". The common pitfall the storyboard names is *testing only one
browser session*: with one session the last writer of a static is always you, so the bug is invisible.

Card B of the console (**Two-session isolation test · Orders**) runs the same Orders screen on two stores, chosen with
two radio buttons:

| Control | Store | Comment in code |
|---|---|---|
| `radioLegacy` — *Legacy statics* | `Legacy.AppState.CurrentUser / CurrentCompany / CurrentCustomer / CurrentFilter` | ✕ one slot for the whole server |
| `radioContext` — *UserContext (session)* | `SessionContext.Current` (`Application.Session`) | ✓ one typed context per browser session |

`buttonSignInKelly` signs in **kelly · Acme · Northwind Traders · filter Open**; `buttonSignInSam` signs in
**sam · Globex · Fabrikam Inc · filter Invoiced** — the two users of the videos. `comboFilter` / `comboCustomer` write
to the active store and re-filter the grid. `buttonReread` reads the active store again and compares it with what
**this page** last wrote (the page instance is per session, so that reference is per session too). `buttonSecondSession`
opens the same URL in a new tab with `Application.Navigate(Application.Url, "_blank")` — a second browser session in the
same server process, the cheapest multi-user test there is.

The **stores** label under the grid always shows three things: the static slot, this session's context, and what this
page last wrote — e.g.

```
session a4f9c2e1 · 2 live · Legacy statics
static AppState (one slot per server)
  sam · Globex · Fabrikam Inc · filter Invoiced
UserContext.Current (session a4f9c2e1)
  (not signed in)
this page last wrote
  kelly · Acme · Northwind Traders · filter Open
```

## Test 1 · the corruption (Legacy statics) — expected outcome ✕

| # | Tab | Press | What you should see |
|---|---|---|---|
| 1 | A | `radioLegacy` | Trace `← JS→.NET mode  Legacy statics — the screen reads/writes Legacy.AppState.*  (✕ one slot for the server)`; banner hidden |
| 2 | A | `buttonSignInKelly` | Trace `← JS→.NET sign in  kelly · Acme · Northwind Traders · filter Open` · `• server workspace  App_Data\tmp\a4f9c2e1 created — temp files this session must release on logout/timeout` · `• server AppState.CurrentUser (static)  = kelly   ← one slot: EVERY session on the server now reads kelly` · `• server static slot  = kelly · Acme · Northwind Traders · filter Open — matches this page (no second session has written yet)`. Combos show **Open** / **Northwind Traders**; grid `2 of 5 orders · filter Open · Northwind Traders` (1042, 1040; Northwind highlighted). Green banner: `✓ Static slot and this page agree ("kelly · Acme · Northwind Traders · filter Open") — for now. Open a second session, switch it to Legacy statics, sign in as the other user, then Re-read here.` Status `● signed in as kelly · Legacy.AppState (static)` |
| 3 | A | `buttonSecondSession` | Trace `→ .NET→JS Application.Navigate  same URL, target _blank → a second browser session in this process`; a new tab opens, its load trace says `browser session 7e0d… · 2 session(s) share this process` |
| 4 | B | `radioLegacy` | Same *mode* trace as step 1 — the new page starts on the context store, switch it |
| 5 | B | `buttonSignInSam` | `• server AppState.CurrentUser (static)  = sam   ← one slot: EVERY session on the server now reads sam`; B's stores label shows the static slot `sam · Globex · Fabrikam Inc · filter Invoiced`; green "agree — for now" banner for sam |
| 5b | B | (optional) `comboFilter` → *Shipped* | `← JS→.NET filter  7e0d… sets filter = Shipped → Legacy.AppState (static)` — any write from B lands in the one slot |
| 6 | A | `buttonReread` | Trace `← JS→.NET re-read state  Legacy.AppState (static) — compare with what this page last wrote`. The combos **change by themselves** to **Invoiced** / **Fabrikam Inc** (or whatever B set), the grid re-filters to `1 of 5 orders · filter Invoiced · Fabrikam Inc` (1039 Adventure Works), and the **red** banner reads: `✕ Static slot corrupted: this session (a4f9c2e1, kelly) last wrote "kelly · Acme · Northwind Traders · filter Open" but AppState now holds "sam · Globex · Fabrikam Inc · filter Invoiced" — another session overwrote the one slot the whole server shares, and the grid now shows the OTHER user's filter. Single-user testing never sees this: with one session the last writer is always you.` Trace `• server ✕ corrupted  static filter = Invoiced ≠ this page wrote Open — kelly's selection silently replaced by sam`. Status `● static state overwritten by another session` (red) |

kelly never touched anything; her screen now shows sam's customer and sam's filter. This is the scene of the video
("sam's selection overwrote the one shared slot — kelly now sees the wrong customer").

## Test 2 · the isolation (UserContext) — expected outcome ✓

| # | Tab | Press | What you should see |
|---|---|---|---|
| 1 | A | `radioContext` | Trace `← JS→.NET mode  UserContext — the screen reads/writes SessionContext.Current  (✓ one context per browser session)`; banner hidden |
| 2 | A | `buttonSignInKelly` | `• server UserContext.Current  → session a4f9c2e1 · kelly · Acme · Northwind Traders · filter Open · culture en-US   ← only this session` · `• server ✓ isolated  filter unchanged = Open (UserContext.Current → session a4f9c2e1)`. Green banner: `✓ Isolated: UserContext.Current → session a4f9c2e1 still says "kelly · Acme · Northwind Traders · filter Open". Open a second session, sign in as the other user and change its filter, then Re-read here: nothing changes.` Status `● signed in as kelly · UserContext (session)` |
| 3 | A | `buttonSecondSession` | New tab (session B). A fresh tab starts on the context store; if B was already used for Test 1 it is still on the legacy store — press `radioContext` in B first |
| 4 | B | `buttonSignInSam` | `• server UserContext.Current  → session 7e0d… · sam · Globex · Fabrikam Inc · filter Invoiced · culture en-US   ← only this session`; B's grid `1 of 5 orders · filter Invoiced · Fabrikam Inc` |
| 5 | B | `comboFilter` → *Shipped* | `← JS→.NET filter  7e0d… sets filter = Shipped → UserContext (session)`; B's grid `1 of 5 orders · filter Shipped · Fabrikam Inc` (1041 Contoso) |
| 6 | A | `buttonReread` | Trace `← JS→.NET re-read state  UserContext (session) — compare with what this page last wrote`. Combos **stay** at **Open** / **Northwind Traders**, grid stays `2 of 5 orders · filter Open · Northwind Traders`. Green banner: `✓ Isolated: UserContext.Current → session a4f9c2e1 still says "kelly · Acme · Northwind Traders · filter Open"` — followed by ` while the static slot meanwhile says "sam · Globex · Fabrikam Inc · filter Invoiced" — another session's writes never reached this context.` if Test 1 ran first (the legacy slot still holds sam), otherwise by `. Open a second session, sign in as the other user and change its filter, then Re-read here: nothing changes.` Trace `• server ✓ isolated  filter unchanged = Open (UserContext.Current → session a4f9c2e1)`. Status `● signed in as kelly · isolated per session` |

Each session reads its own context; B's writes never reach A. The corresponding lines of the video's session trace are
`UserContext.Current → session a4f9`, `UserContext.Current → session 7e0d`, `B sets filter = "Invoiced"`,
`A filter unchanged = "Open" ✓ isolated`.

## Test 3 · logout must not leak either

| # | Tab | Press | What you should see |
|---|---|---|---|
| 1 | A | `radioLegacy`, `buttonSignInKelly`, then `buttonSignOut` | Trace `⚠ boundary AppState cleared  ✕ the statics are one slot — this sign-out signed out EVERY session on the server`; amber banner ending in ` In Legacy statics mode this logout also blanked AppState — for EVERY session on the server.` — a `buttonReread` in tab B (on Legacy statics) now shows `(not signed in)` in the static slot |
| 2 | A | `radioContext`, `buttonSignInKelly`, then `buttonSignOut` | `• server cleanup  UserContext: SessionContext.Reset() → session a4f9c2e1 is anonymous again (logout)`; green banner `✓ Signed out: SessionContext.Reset() cleared this session's context …`; tab B's context is untouched (`buttonReread` there still says sam) |

## How the console decides "corrupted" vs "isolated"

`MainPage.RefreshStores` compares the **active store** (a snapshot of `Legacy.AppState`, or `SessionContext.Current`)
with **what this page last wrote** (`_lastWritten`, a per-page and therefore per-session dictionary, one entry per
store). A difference is only possible on the static store — another session wrote the slot — so that branch prints the
red banner; the context branch prints the green one and, for extra evidence, quotes the static slot when it disagrees.
`SameState` compares user, company, customer id and filter.

## Evidence

The two banners above, the `✕ corrupted` / `✓ isolated` trace lines and the stores label are the deliverable: a
reproducible before/after with the same screen, the same two users and the same clicks. Run Test 1 and Test 2 back to
back in the same two tabs to see both outcomes within a minute; `buttonClear` empties the trace between runs.
