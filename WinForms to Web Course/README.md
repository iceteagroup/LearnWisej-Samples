# From WinForms to the Web · lab samples

One runnable Wisej.NET 4 application per module, built from the course's lesson guide, lab guide
and walkthrough video. The course's worked example is **LegacyOrderDesk**, a single-user WinForms
order desk; every module folder is a snapshot of its migration into **OrderDesk.Web**, a Wisej.NET
web app, at the end of that module. Every sample follows the same layout: the migrated screen(s)
on the left, a **Server ⇄ Client migration trace** on the right, and a button bar that exercises the
success path, a progress path, at least one failure path (the desktop assumption breaking on the
server) and the web-safe recovery. Each folder has its own `README.md` (what to click, self-check
answers) and an `OrderDesk.Web/docs/` folder with the lab deliverables plus the running
`migration-log.md`.

Requirements already on this machine: .NET 10 SDK and the `Wisej-4` 4.1.0 NuGet package. Nothing is deployed anywhere.

| Module | Folder | What it builds | Run |
|---|---|---|---|
| 1 · Migration Discovery & the First Slice | `Module 1` | the WinForms `LegacyOrderDesk` (before) + the assessment workbook as a live screen, screen classification, the first vertical slice, desktop export ✕ vs `Application.Download` ✓ | `dotnet run -f net10.0 --urls http://localhost:5101` |
| 2 · Project Conversion | `Module 2` | the Wisej.NET shell (Default.html / Default.json / Program / Startup / Web.config), the first form ported (`OrdersPage`), compiler-error categories, App.config ✕ vs Web.config ✓ | `http://localhost:5102` |
| 3 · Forms, Navigation, Layout & Modal Workflow | `Module 3` | left nav + MenuBar + StatusBar, three pages, `EditOrderDialog` as a modal `Form` inside `using`, dialog-leak counter ✕ vs disposal ✓, `MessageBox` ✕ vs `Toast` ✓ | `http://localhost:5103` |
| 4 · Sessions, Statics & Multi-User Safety | `Module 4` | sign-in as kelly / sam, `AppState` statics ✕ vs `UserContext` in `Application.Session` ✓, HKCU registry ✕ vs server profile ✓, session cleanup hooks, two-session test | `http://localhost:5104` |
| 5 · DataGridView, Validation & Performance | `Module 5` | 200k-order store, naive full bind ✕ vs `VirtualMode` + server-side `OrderQuery` ✓, filter toolbar, summary row, `OrderValidator` + `ErrorProvider`, timing measurements | `http://localhost:5105` |
| 6 · Files, Reports & Browser Boundaries | `Module 6` | CSV `Upload` import into `App_Data`, Excel Interop ✕ vs managed `.xlsx` writer + `Download` ✓, PrintDocument ✕ vs server PDF in `PdfViewer` ✓, queued report jobs | `http://localhost:5106` |
| 7 · Modernize, Secure & Deploy the Capstone | `Module 7` | the OrderDesk operations dashboard: themes, `ClientProfiles.json` + responsive handler, `AllowHtml` review, guarded downloads, `/health`, deployment checklist, migration report | `http://localhost:5107` |

Run any module from its `OrderDesk.Web` project folder. The projects multi-target `net10.0-windows`
and `net10.0`, so `dotnet run` needs a framework (`-f net10.0`, or `-f net10.0-windows`), e.g.

```bash
cd "D:/Projects/LearnWisej-Samples/WinForms to Web Course/Module 3/OrderDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5103
```

or open the `OrderDesk.slnx` in the module folder with Visual Studio and press F5. Module 1 also
contains the WinForms `LegacyOrderDesk` project (`net10.0-windows`, `dotnet run` from its folder)
so the "before" app can be run next to the "after".

## `_template`

The scaffold every module was built from, plus `COOKBOOK.md`: the migration conventions and the
Wisej.NET API facts verified while building and running these samples (startup and configuration,
modal dialogs and disposal, `Application.Session`, download/upload, `PdfViewer`, `DataGridView`
virtual mode, `ErrorProvider`, responsive profiles, and the gotchas found along the way). Read it
before writing a new sample. The template holds the shared `Domain/` (order model, store, service,
validator, query — the business logic every module reuses unchanged), the `Shared/` lab props
(`TracePanel`, `Palette`, `Notify`) and the complete WinForms "before" app with its desktop
assumptions marked ✕ (`Legacy/`: static `AppState`, HKCU `UserPreferences`, `C:\Orders`
`LocalExport`, Excel Interop `ExcelExport` stub, `InvoicePrinter`).

## Stand-ins

The course ships no third-party packages, so the samples write the small pieces themselves: an
Interop-shaped Excel stub that fails the way real Office Automation fails on a server, a minimal
OpenXML `.xlsx` writer, a single-page PDF writer, an in-memory order store seeded with the orders
the videos show (1042 Northwind Traders … 1038 Globex Corp). Swap in a real database, a real
spreadsheet or PDF library and only the service classes change.
