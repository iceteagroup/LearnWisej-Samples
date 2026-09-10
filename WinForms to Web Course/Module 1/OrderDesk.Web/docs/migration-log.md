# migration-log.md — OrderDesk (LegacyOrderDesk → Wisej.NET)

One dated line per accepted workaround or decision. The videos keep this file open in a tab; every
module appends to it. Format: `date · module · decision`.

- 2026-09-10 · M1 · Assessment workbook created (13 items); screens tagged; first slice = startup + login/user context + Orders read-only grid + EditOrderDialog + CSV export via Download; acceptance = parity first, modernization second, deployment third.
- 2026-09-10 · M1 · Business logic (OrderService, OrderCalculator, OrderValidator, OrderStore) reused as-is — shared `Domain/*.cs`, linked into both `LegacyOrderDesk` and `OrderDesk.Web`.
- 2026-09-10 · M1 · Runtime shift accepted as the real change: one .exe for one user → one server, many sessions; `Program.Main` runs once per session, Kestrel owns the process.
- 2026-09-10 · M1 · `AppState.CurrentUser` (static) confirmed process-wide with two browser tabs — per-user state must leave statics (Module 4); `Countries` (immutable) may stay static.
- 2026-09-10 · M1 · Excel Interop fails on the server (`COMException 0x80040154`, no interactive desktop) — verdict redesign: managed writer + `Application.Download` (Module 6).
- 2026-09-10 · M1 · `LocalExport.WriteCsv` → `C:\Orders\out.csv` would land on the server's disk — the browser boundary is crossed with `Application.Download(stream, "orders.csv")`; `LocalExport.ToCsv` reused unchanged.
- 2026-09-10 · M1 · Print Invoice (`PrintDocument`) → redesign as server PDF (Module 6); `UserPreferences` (HKCU) → per-user server profile (Module 4); `App.config` → `Web.config` (Module 2); File → Exit removed; Help → About deferred.
