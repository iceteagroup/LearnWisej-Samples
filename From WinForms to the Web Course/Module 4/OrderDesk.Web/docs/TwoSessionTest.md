# Two-session isolation test (lab step 5)

Lab step covered: *test two browser sessions with different users*. The common pitfall is *testing only one browser
session*: with one session the last writer of a static is always you, so the bug is invisible.

The Orders screen reads and writes its per-user state only through `SessionContext.Current` (`Application.Session`).
`buttonSignInKelly` signs in **kelly · Acme · Northwind Traders · filter Open**; `buttonSignInSam` signs in
**sam · Globex · Fabrikam Inc · filter Invoiced** — the two users of the video. `comboFilter` / `comboCustomer` write to
this session's context and re-filter the grid. `buttonReread` reads the context again. `buttonSecondSession` opens the
same URL in a new tab with `Application.Navigate(Application.Url, "_blank")` — a second browser session in the same
server process, the cheapest multi-user test there is.

The **Session** card shows this session's context the way the video's session cards do:

```
Session            a4f9c2e1   (2 open)
User               kelly
Current customer   Northwind Traders
Active filter      Open
Company            Acme
```

## Test 1 · isolation — expected outcome ✓

| # | Tab | Press | What you should see |
|---|---|---|---|
| 1 | A | `buttonSignInKelly` | The combos show **Open** / **Northwind Traders**; the grid shows `2 of 5 orders` (1042, 1040; Northwind highlighted); the Session card shows kelly · Northwind Traders · Open · Acme |
| 2 | A | `buttonSecondSession` | A new tab opens (session B); the Session card there says `(not signed in)` and `(2 open)` |
| 3 | B | `buttonSignInSam` | B shows **Invoiced** / **Fabrikam Inc**, `1 of 5 orders` (1039 Adventure Works) |
| 4 | B | `comboFilter` → *Shipped* | B's grid re-filters (1041 Contoso); B's Session card says `Active filter  Shipped` |
| 5 | A | `buttonReread` | A's combos **stay** at **Open** / **Northwind Traders**, the grid stays `2 of 5 orders`, the Session card still says kelly · Open |

Each session reads its own context; B's writes never reach A. This is the video's run scene: *B sets filter =
"Invoiced"*, *A filter unchanged = "Open" ✓ isolated*.

## Test 2 · logout must not leak either

| # | Tab | Press | What you should see |
|---|---|---|---|
| 1 | A | `buttonSignOut` | A's Session card says `(not signed in)`, the combos reset to **All** / **All customers**, the grid shows all 5 orders |
| 2 | B | `buttonReread` | B is still signed in as sam with its own filter — signing out in A cleared only A's context |

With the legacy `static AppState` both tests fail: the second sign-in overwrites the one slot for every session, and
a sign-out blanks it for everybody (see `StaticStateAudit.md` and `SessionContextService.md`).
