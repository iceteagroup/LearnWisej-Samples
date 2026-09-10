# migration-log.md — LegacyOrderDesk → OrderDesk.Web

One dated line per accepted workaround or decision. Carried forward module by module; Module 3 adds its own at the end.

- 2026-09-10 · M1 · Assessment workbook created; screens tagged; first slice = startup + login/user context + Orders read-only grid + EditOrderDialog + CSV export via Download; acceptance = parity first, modernization second, deployment third.
- 2026-09-10 · M1 · Business logic (OrderService, OrderCalculator, OrderValidator, OrderStore) reused as-is — shared `Domain/*.cs`.
- 2026-09-10 · M2 · New Wisej.NET shell (`OrderDesk.Web`, Wisej-4 4.1.0, net10.0-windows;net10.0); `Application.Run` → `Program.Main` + `Default.json startup`; `App.config` → `Web.config` read via `AppConfig` (System.Xml.Linq).
- 2026-09-10 · M2 · Compiler errors classified: namespace swap, renamed types (MenuStrip→MenuBar, StatusStrip→StatusBar, ToolStripMenuItem→MenuItem), designer-only properties removed, startup rewritten.
- 2026-09-10 · M3 · Navigation = left nav + MenuBar + StatusBar; `EditOrderDialog` → `Wisej.Web.Form` shown with `ShowDialog` inside `using`; `MessageBox.Show("Saved.")` → `Toast`; validation MessageBox stays modal.
- 2026-09-10 · M3 · `OrdersForm` → `Pages/OrdersPage : Panel` hosted by `MainPage`; its MenuStrip/StatusStrip moved into the shell; Customers and Reports pages created once and shown by `Visible` (no new top-level Form per screen).
- 2026-09-10 · M3 · Layout: Anchor → Dock (header Top 36 · detail panel Right 220 · grid Fill); `ClientSize`/`MinimumSize`/`StartPosition`/`ISupportInitialize` dropped; TabIndex values preserved (grid 0, detail 1 → Customer 0, PO 1, Lines 2, New Order 3, Print 4, Export 5).
- 2026-09-10 · M3 · Dialog lifetime rule: `ShowDialog()` keeps the instance (reusable) — every transient dialog goes through `using`; a deliberately reused dialog must `ResetState()`; `EditOrderDialog.LiveInstances` + `Application.FindComponents` prove it. Non-modal `Show()` forms are disposed on close and are not affected.
- 2026-09-10 · M3 · Notification policy accepted: informational → Toast (`Notify.Saved`, BottomRight, 3.5 s) or AlertBox (TopRight); decisions, validation failures and user-requested dialogs (About) stay modal MessageBoxes.
- 2026-09-10 · M3 · `File › Exit` is not a web action (it ended the desktop process); logged as a finding, becomes Sign out in M4. Print Invoice / Export to Excel kept on the detail panel but disabled with tooltips → M6.
- 2026-09-10 · M3 · `AppState.CurrentOrder/CurrentCustomer` writes removed from `ShowDetail()`; New Order owner defaults to "Kelly" instead of `AppState.CurrentUser` until `UserContext` (M4). `CustomersPage` binds explicit columns only — `Customer.TaxId` never reaches the browser.
