# The chosen first vertical slice

**Lab 1 deliverable 3.** Small but honest: not a static form (proves almost nothing), not every
feature (a hidden rewrite). The slice must prove the architecture, the conventions and the workflow,
and it must cross at least one real browser boundary.

## What the slice touches

| # | Touch point | In this module | Later |
|---|---|---|---|
| 1 | **Startup** | `Startup.cs` (Kestrel + `app.UseWisej()`), `Default.json` `startup` → `Program.Main`, which runs **once per session** and sets `Application.MainPage` | Module 2 explains the shell file by file |
| 2 | **User context / login** | the copied `AppState.CurrentUser` static set to `kelly` — shown to be process-wide (the finding) | Module 4: `UserContext.Current` in `Application.Session` |
| 3 | **One read-only screen** | the Orders card | Module 2: the ported `OrdersPage` |
| 4 | **One edit dialog** | *planned* — `EditOrderDialog` is in the slice, ported in Module 3 (`ShowDialog` inside `using`) | Module 3 |
| 5 | **One grid** | `DataGridView` with the five walkthrough orders from `OrderService.GetAll()` | Module 5: `VirtualMode` |
| 6 | **One file / report** | `orders.csv` via `Application.Download` (the `LocalExport.ToCsv` formatting reused) | Module 6: `.xlsx`, PDF, report queue |
| 7 | **One deployment boundary** | Excel Interop and `C:\Orders` fail on the server; the download replaces them | Module 6/7 |

## Acceptance criteria — parity first, modernization second, deployment third

1. **Parity first.** The Orders workflow of LegacyOrderDesk works in the browser with the same
   `OrderService` calls, the same five orders (1042 Northwind Traders $4,820.00 Open · 1041 Contoso Ltd
   $1,290.50 Shipped · 1040 Fabrikam Inc $760.00 Open · 1039 Adventure Works $12,400.00 Invoiced ·
   1038 Globex Corp $3,090.00 Open) and the same CSV bytes. No business rule is rewritten.
2. **Modernization second.** No new layout, no dashboard, no theme work until (1) is proven. The
   Module 7 operations dashboard is the payoff, not the starting point.
3. **Deployment third.** The slice runs on a host with no printer, no Excel and no `C:\Orders`:
   every desktop boundary it meets is either replaced (`Application.Download`) or logged as an explicit
   task in `migration-log.md`.

**Done means:** the slice is exercised with **two concurrent sessions** (two browser tabs). The video's
production caution: the report/export crossed a browser boundary, and the second session proves the
static user object cannot be shared — test with two users before marking the slice done.

## Why this slice and not another

- It touches the three explicit migration tasks of the video (static user, local file path, Excel
  Interop) without porting a single extra form.
- It reuses `OrderService.GetAll()` and `LocalExport.ToCsv()` unchanged — proof that the business logic
  moves as-is.
- It fails visibly where the desktop assumptions break (a `COMException`, a path on the wrong machine)
  instead of hiding them behind a happy-path form.

## Evidence (what the running app shows)

The **First vertical slice** card starts with seven `○` items and lights up:

| Button | Items that change | Trace |
|---|---|---|
| Run first slice ✓ | `✓ Startup — Startup.cs + Program.Main` · `✓ One read-only screen — Orders` · `✓ One grid — DataGridView, 5 rows` | `• server  Startup.cs`, `• server  Program.Main  runs once per SESSION…`, `• server  OrderService.GetAll()  5000 orders in the store…`, `→ .NET→JS  gridOrders.Rows  5 rows → browser: 1042 Northwind Traders $4,820.00 Open · …`, `✓ ok  first slice  startup ✓ · read-only screen ✓ · grid ✓ — parity first…` |
| Second session | `✓ User context / login — kelly (static ✕ → session, Module 4)` (amber) | `• server  AppState.CurrentUser = kelly`, `• server  read back  … the same static instance for every session in this process`, `★ log  static-state  open a second browser tab…` |
| Export (desktop way) ✕ | `✖ One deployment boundary — Excel Interop + C:\Orders hit the SERVER` (red) | `✖ fail  COMException 0x80040154 …`, `✖ fail  would write C:\Orders\out.csv on the SERVER …` |
| Export (Download) ✓ | `✓ One file / report — orders.csv via Application.Download` · `✓ One deployment boundary — server → browser download` | `→ .NET→JS  Application.Download  orders.csv (… bytes) → browser download · nothing written on the server`, `✓ ok  browser boundary crossed …` |

`○ One edit dialog — EditOrderDialog (ported in Module 3)` stays open on purpose: it is part of the
slice's definition and the first thing Module 3 delivers.
