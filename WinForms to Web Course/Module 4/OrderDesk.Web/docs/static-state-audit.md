# Static-state audit — Module 4, lab steps 1–2

**Lab goal:** find and refactor shared static user state in LegacyOrderDesk into a session-aware
context service. Step 1 is the search, step 2 the classification; this file is the result.

## How the search was done

```
grep -rn "static " LegacyOrderDesk/*.cs LegacyOrderDesk/Legacy/*.cs OrderDesk.Web/Domain/*.cs OrderDesk.Web/Shared/*.cs
```

plus a read of every `Legacy/*.cs` file (the desktop plumbing) and of the shared `Domain/` folder,
because the business logic is reused as-is and any static there is now multi-user too. Singletons
(`OrderStore.Shared()`) count as statics: a private field behind a static accessor is still one slot per
process.

## The one question

> Would this value be different if two users opened the app at the same time?

If yes, it is per-user state and cannot live in a static. If no — the value is immutable (or guarded
by a lock and independent of any user) — it may stay static.

## Audit table

| Static | Where | Classification | Decision | Done in |
|---|---|---|---|---|
| `AppState.CurrentUser` | `Legacy/AppState.cs` | per-user | → `UserContext.User` (in `Application.Session`) | M4 |
| `AppState.CurrentCustomer` | `Legacy/AppState.cs` | per-user | → `UserContext.CurrentCustomer` | M4 |
| `AppState.ActiveFilter` (`string`) | `Legacy/AppState.cs` | per-user | → `UserContext.ActiveFilter` (`OrderStatus?`, typed) | M4 |
| `AppState.CurrentOrder` | `Legacy/AppState.cs` | per-user (workflow) | → passed to `EditOrderDialog` as a constructor parameter; never global | M3 |
| `UserPreferences.Get/Set` (HKCU `Software\LegacyOrderDesk`) | `Legacy/UserPreferences.cs` | per-user, machine-bound | → `UserSettingsStore` (`App_Data/users/<name>.json`) | M4 |
| `AppState.Countries` (`string[]`, readonly) | `Legacy/AppState.cs` | immutable lookup | **keep static** | – |
| `OrderStore.Shared()` (+ `Gate` lock) | `Domain/OrderStore.cs` | shared reference data, locked, user-independent | **keep static** (it is the stand-in database) | – |
| `OrderStore.Owners` (`IReadOnlyList<string>`) | `Domain/OrderStore.cs` | immutable lookup | keep | – |
| `OrderCalculator.*` (constants + pure functions) | `Domain/Order.cs` | stateless | keep | – |
| `Palette.*` colors | `Shared/Palette.cs` | immutable UI constants | keep | – |
| `EditOrderDialog.LiveInstances` (M3 lab counter) | `Dialogs/EditOrderDialog.cs` (Module 3) | process-wide diagnostic, not user data | keep, but document that it counts **all** sessions' dialogs | M3 |
| `Program` / `Application.MainPage` | `Program.cs` | per-session by construction (Wisej scopes `Application` to the session) | nothing to do — but never cache the page in a static | – |

Seven of these show up in the app's **Static-state audit** grid (the five per-user ones and the two
"keep" examples) so the classification is visible next to the running demo.

## What the classification buys

- The two "keep" rows stop an over-reaction: moving `Countries` or the order store into the session
  would copy reference data once per user for no benefit (the video: "don't move every static").
- The five "move" rows are exactly the values the LoginForm and OrdersForm wrote and read; they become
  one class, `Services/UserContext.cs`, and every caller goes through `UserContext.Current`.
- `CurrentOrder` was already removed in Module 3: the dialog receives the order it edits.

## Evidence (what the running app shows)

- On load the trace prints `• server Static-state audit  7 statics found · 5 per-user → session · 2 immutable/shared → keep`
  and the **Static-state audit** grid under the Orders grid lists the seven rows above.
- `Sign in` writes both stores and logs the static write as a failure path:
  `✖ fail AppState.CurrentUser = "kelly" · CurrentCustomer = Northwind Traders · ActiveFilter = Open   ✕ static: visible to every session`
  next to the ok line `✓ ok UserContext.Current  UserContext#xxxx ← kelly · Northwind Traders · Open   ✓ Application.Session of <session>`.
- `Corrupt from here ✕` proves the per-user classification of `CurrentCustomer` by making a second
  session's write appear in kelly's read-back; `Use UserContext ✓` proves the fix.
- `Registry preference ✕` / `Server profile ✓` do the same for the `UserPreferences` row.
