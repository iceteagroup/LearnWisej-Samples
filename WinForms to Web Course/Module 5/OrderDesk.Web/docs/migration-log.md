# migration-log.md — LegacyOrderDesk → OrderDesk.Web

One line per accepted workaround or decision, carried forward module by module. Module 5 adds the entries at the end.

- 2026-09-10 · M1: Assessment workbook created; screens tagged; first slice = startup + login/user context + Orders read-only grid + EditOrderDialog + CSV export via Download; acceptance = parity first, modernization second, deployment third.
- 2026-09-10 · M1: Business logic (OrderService, OrderCalculator, OrderValidator, OrderStore) reused as-is — shared `Domain/*.cs`.
- 2026-09-10 · M2: New Wisej.NET shell (`OrderDesk.Web`, Wisej-4 4.1.0, net10.0-windows;net10.0); `Application.Run` → `Program.Main` + `Default.json startup`; `App.config` → `Web.config` read via `AppConfig` (System.Xml.Linq).
- 2026-09-10 · M2: Compiler errors classified: namespace swap, renamed types (MenuStrip→MenuBar, StatusStrip→StatusBar, ToolStripMenuItem→MenuItem), designer-only properties removed, startup rewritten.
- 2026-09-10 · M3: Navigation = left nav + MenuBar + StatusBar; `EditOrderDialog` → `Wisej.Web.Form` shown with `ShowDialog` inside `using`; `MessageBox.Show("Saved.")` → `Toast`; validation MessageBox stays modal.
- 2026-09-10 · M4: `AppState` statics → `UserContext.Current` in `Application.Session`; `UserPreferences` (HKCU) → per-user server profile (`App_Data/users`); cleanup on `ApplicationExit` / `SessionTimeout`; two-session test passed.
- 2026-09-10 · M5: Orders grid → VirtualMode + server-side `OrderQuery` (default filter Open, BlockSize paging); naive full bind measured vs optimized; validation → `OrderValidator` + `ErrorProvider` field messages.
- 2026-09-10 · M5: Production-like test data = `OrderStore.Create(200000, 42)`, a private store seeded in `Application.StartTask` (page opens instantly, seed time traced); the 5,000-row `OrderStore.Shared()` of the other modules untouched.
- 2026-09-10 · M5: The naive port (`DataSource = service.GetAll()`) is kept in the sample only as the measurable "before" — capped at 20,000 rows by default, `Bind all 200k ✕` for the real number; it is not part of the product.
- 2026-09-10 · M5: Filter toolbar decided with the desk: Status defaults to Open; search covers order number, customer, owner, PO; sort is a query option — all server-side, nothing sorted or filtered in the browser.
- 2026-09-10 · M5: Summary row = Σ Total computed on the server over every matching row (footer); grid-side `AddSummaryRows` not used with virtual rows because it would only see loaded blocks.
- 2026-09-10 · M5: Validation MessageBox from M3 replaced in the edit workflow by `ErrorProvider.SetError` per field (`Customer`, `Owner`, `Lines`, `Total`); `OrderService.Save` still validates and throws `ValidationException` — the server rule is the boundary, the dialog is a courtesy.
- 2026-09-10 · M5: No live updates on the orders grid — no workflow benefit; server push reserved for screens that need it (M7 activity feed), pushing affected rows only.
- 2026-09-10 · M5: Performance notes recorded (`docs/performance-notes.md`, numbers from the Measure button, also written to `App_Data/performance-notes.md`): naive 20k / naive 200k / optimized — rows fetched, rows in grid, server ms, Δ heap, estimated payload; real bytes to be confirmed in DevTools → Network.
