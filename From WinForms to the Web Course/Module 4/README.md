# OrderDesk.Web · From WinForms to the Web · Module 4

Local lab build for **Module 4 · Sessions, Statics, and Multi-User Safety**. Every static field and singleton of
**LegacyOrderDesk** is found and classified (*keep static* vs *move to session / profile store / browser storage*), the
five per-user statics of `AppState` become a typed `UserSessionContext` behind `Application.Session`, and the Orders
screen is tested with **two browser sessions**: changing the filter in one does not affect the other. Sign out runs
the one cleanup routine the lesson asks for.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/From WinForms to the Web Course/Module 4/OrderDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5604
```

Then open <http://localhost:5604>. (Visual Studio: open `OrderDesk.slnx`, F5.)

## What to try

| Action | What you should see |
|---|---|
| Page load | The Orders card with all 5 orders and the Session card: `User (not signed in)`, `Active filter All` |
| **Sign in as kelly** | The context becomes kelly · Acme · Northwind Traders · filter Open: the combos follow, the grid shows `2 of 5 orders` (Northwind highlighted), the Session card shows the four values |
| **Sign in as sam** | sam · Globex · Fabrikam Inc · filter Invoiced: `1 of 5 orders` |
| Filter / customer combos | Write to this session's context and re-filter the grid |
| **Open second session ↗** → in the new tab **Sign in as sam** and change its filter → back in the first tab **Re-read state** | The first tab still shows kelly's customer and filter: each session reads its own `UserContext` (steps in `docs/TwoSessionTest.md`) |
| **Sign out** | `SessionCleanup.Run` deletes the session's temp folder and `SessionContext.Reset()` clears only this session's context; the other tab is untouched |

## Where things live

```
Module 4/
└─ OrderDesk.Web/                     the Wisej.NET 4 app (net10.0-windows;net10.0), port 5604
   ├─ Program.cs / Startup.cs         session entry point (Application.MainPage + the ApplicationExit cleanup) / Kestrel host
   ├─ Default.html / Default.json / Web.config
   ├─ Domain/                         the reused business logic (Order, Customer, OrderService, CustomerService, InvoiceDocument, SampleData)
   ├─ Services/UserSessionContext.cs  the typed context: plain data, no Wisej reference
   ├─ Services/SessionContext.cs      Current (lazy, per browser session in Application.Session) + Reset()
   ├─ Services/SessionCleanup.cs      the one routine logout and ApplicationExit share
   ├─ Services/StorageRoot.cs         Web.config OrderDesk.StorageRoot (default App_Data)
   ├─ Views/Ui.cs                     colours + toast helper
   ├─ MainPage.cs / .Designer.cs      the Orders screen + the Session card
   ├─ App_Data/                       created at run time: tmp/<session>/report-job.txt
   └─ docs/                           the lab deliverables + migration-log.md
```

## Deliverables

| # | Deliverable | Document | Code |
|---|---|---|---|
| 1 | Static-state audit (lab steps 1 + 2) | [`OrderDesk.Web/docs/StaticStateAudit.md`](OrderDesk.Web/docs/StaticStateAudit.md) | — |
| 2 | Static user state → session services (lab steps 3 + 4) | [`OrderDesk.Web/docs/SessionContextService.md`](OrderDesk.Web/docs/SessionContextService.md) | [`Services/UserSessionContext.cs`](OrderDesk.Web/Services/UserSessionContext.cs), [`Services/SessionContext.cs`](OrderDesk.Web/Services/SessionContext.cs) |
| 3 | Two-session isolation test (lab step 5) | [`OrderDesk.Web/docs/TwoSessionTest.md`](OrderDesk.Web/docs/TwoSessionTest.md) | `MainPage.cs` |
| 4 | Cleanup requirements for timeout and logout (lab step 6) | [`OrderDesk.Web/docs/SessionCleanup.md`](OrderDesk.Web/docs/SessionCleanup.md) | [`Services/SessionCleanup.cs`](OrderDesk.Web/Services/SessionCleanup.cs), `Program.cs` |
| 5 | Migration log | [`OrderDesk.Web/docs/migration-log.md`](OrderDesk.Web/docs/migration-log.md) | — |

## Self-check answers (lab guide + video)

- **Which business logic was reused as-is?** Everything under `Domain/`, unchanged since Module 1. The grid is filtered
  through `OrderService.Search(new OrderFilter { Status, Text })` — the same call `OrdersForm` made; only *where the
  filter value comes from* changed (a static → the session context).
- **Which desktop boundary was replaced with a web-safe pattern?** The process-per-user assumption: per-user state
  moved from process-wide statics to `Application.Session`. The registry decision (browser storage for UI preferences,
  a server profile for settings that must roam, a database for audited ones) is recorded in `StaticStateAudit.md` and
  `migration-log.md`.
- **How is per-user state kept out of static fields?** The five per-user statics of `AppState` became properties of
  `UserSessionContext`, one instance per browser session stored in `Application.Session` and reached only through
  `SessionContext.Current`. Callers do not know the storage; `Reset()` clears it on logout. The statics that are
  immutable and user-independent (`Countries`, `SampleData.Customers`, the locked repository) stayed static.
- **What was tested before calling the migrated feature complete?** The two-session test in `TwoSessionTest.md`: kelly in
  tab A, sam in tab B changes his filter, tab A is unchanged; signing out in one tab leaves the other signed in.
- **Can this static stay: is it immutable and user-independent?** Ask *would the value differ if two users opened the
  app at the same time?* `Countries`, `SampleData.Customers`, `InMemoryOrderRepository.Shared` stay; `CurrentUser`,
  `CurrentCompany`, `CurrentCustomer`, `CurrentFilter`, `LastSearch` move to the session. 3 of 11 stay.
- **Should this setting roam, be audited, or stay on the device?** `GridDensity` may differ per device → browser
  storage. `ExportFolder` must roam and server code must read it → a server profile; audited → a database table.
  `WindowWidth/Height` → neither; the browser window belongs to the user.

## Runtime facts

- `Application.Session` is a **dynamic bag** (also a `Dictionary<string, object>`): a member that was never set reads
  as `null`. `SessionContext.Current` wraps exactly that and creates the context lazily.
- `Application.ApplicationExit` is a **static event**: subscribed once per session in `Program.Main`, it runs even
  after the page is disposed. It has no UI to push to — log to the host, do not touch controls.
- `Application.Navigate(Application.Url, "_blank")` opens a second session in the same server process;
  `Application.SessionCount` counts them; `Application.SessionId` identifies this one.
- `Application.StartupPath` is the project folder under `dotnet run`, so `App_Data/tmp` is created next to the sources;
  delete `App_Data/` to reset the sample.
- The projects multi-target `net10.0-windows;net10.0`, so `dotnet run` needs `-f net10.0` (or `-f net10.0-windows`).
