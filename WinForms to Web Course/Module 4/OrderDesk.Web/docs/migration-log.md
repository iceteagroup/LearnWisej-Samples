# migration-log.md — LegacyOrderDesk → OrderDesk.Web

One line per accepted workaround or decision. Carried forward module by module; Module 4 adds the
entries at the bottom.

- 2026-09-10 · M1: Assessment workbook created; screens tagged; first slice = startup + login/user context + Orders read-only grid + EditOrderDialog + CSV export via Download; acceptance = parity first, modernization second, deployment third.
- 2026-09-10 · M1: Business logic (OrderService, OrderCalculator, OrderValidator, OrderStore) reused as-is — shared `Domain/*.cs`.
- 2026-09-10 · M2: New Wisej.NET shell (`OrderDesk.Web`, Wisej-4 4.1.0, net10.0-windows;net10.0); `Application.Run` → `Program.Main` + `Default.json startup`; `App.config` → `Web.config` read via `AppConfig` (System.Xml.Linq).
- 2026-09-10 · M2: Compiler errors classified: namespace swap, renamed types (MenuStrip→MenuBar, StatusStrip→StatusBar, ToolStripMenuItem→MenuItem), designer-only properties removed, startup rewritten.
- 2026-09-10 · M3: Navigation = left nav + MenuBar + StatusBar; `EditOrderDialog` → `Wisej.Web.Form` shown with `ShowDialog` inside `using`; `MessageBox.Show("Saved.")` → `Toast`; validation MessageBox stays modal.
- 2026-09-10 · M4: `AppState` statics → `UserContext.Current` in `Application.Session`; `UserPreferences` (HKCU) → per-user server profile (`App_Data/users`); cleanup on `ApplicationExit` / `SessionTimeout`; two-session test passed.
- 2026-09-10 · M4: Static-state audit: 5 per-user statics moved (CurrentUser, CurrentCustomer, ActiveFilter, CurrentOrder, UserPreferences); `AppState.Countries` and `OrderStore.Shared()` stay static (immutable / locked reference data) — do not move every static.
- 2026-09-10 · M4: `LoginForm.ShowDialog()` before `Application.Run` → Sign-in card writing `UserContext.Current`; `Program.Main` returns immediately and runs once per session.
- 2026-09-10 · M4: `AppState.ActiveFilter` string → `UserContext.ActiveFilter` as `OrderStatus?` (typed; the string was parsed at every use).
- 2026-09-10 · M4: Sign out = `UserContext.Clear()` for this session only; the static is never nulled on logout (it would sign out every session). Built-in `SessionTimeout` prolong dialog kept (`e.Handled = false`), event traced.
- 2026-09-10 · M4: `AppState` and `UserPreferences` kept in `OrderDesk.Web/Legacy/` only as the ✕ side of the demo; delete with the last caller once the two-session test is part of the regression suite.
