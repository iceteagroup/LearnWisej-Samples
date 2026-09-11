# From WinForms to the Web Course · lab samples

One runnable Wisej.NET 4 application per module, built from the course's lesson guide, lab guide and walkthrough
video. The course migrates **LegacyOrderDesk** — a single-user WinForms order desk (login, orders grid, edit dialog,
Print Invoice, Export to Excel, Attach file, registry settings) — into **OrderDesk.Web**. Every module folder is a
snapshot of that migration at the end of the module: the screens the module's lab guide and walkthrough video build,
and nothing else. Each folder has its own `README.md` (what to try, self-check answers) and an `OrderDesk.Web/docs/`
folder with the lab deliverables and the running `migration-log.md`.

Requirements already on this machine: .NET 10 SDK and the `Wisej-4` 4.1.0 NuGet package. Nothing is deployed anywhere.

| Module | Folder | What it builds | Run |
|---|---|---|---|
| 1 · Migration Discovery and Risk Mapping | `Module 1` | the WinForms original (`LegacyOrderDesk`) + the assessment workbook (14 items, four verdicts) and the first vertical slice running in the browser: reused `OrderService`, server PDF instead of PrintDocument, download instead of Excel Interop, the static-user leak shown with two sessions | `dotnet run -f net10.0 --urls http://localhost:5601` |
| 2 · Project Conversion and Wisej.NET Startup | `Module 2` | the Wisej.NET shell (Default.html / Default.json / Program.cs / Startup.cs / Web.config), the first form ported `System.Windows.Forms → Wisej.Web`, the classified compiler-error log, App.config → Web.config | `http://localhost:5602` |
| 3 · Forms, Navigation, Layouts, and Modal Workflow | `Module 3` | MenuBar / ToolBar shell with three screens, the ported modal `EditOrderDialog` shown with `ShowDialogAsync` and disposed by the caller, MessageBox → Toast review, Dock/Anchor layouts | `http://localhost:5603` |
| 4 · Sessions, Statics, and Multi-User Safety | `Module 4` | static-state audit, `UserSessionContext` behind `Application.Session`, the two-session test, the registry decision, logout and timeout cleanup | `http://localhost:5604` |
| 5 · DataGridView, Validation, and Performance | `Module 5` | the orders grid over 200,000 generated orders: server-side filter + `VirtualMode` block cache, summary row, `OrderValidator` with `ErrorProvider` field messages | `http://localhost:5605` |
| 6 · Files, Reports, and Browser Boundaries | `Module 6` | file-boundary classification, CSV upload → server storage root, dependency-free .xlsx + PDF writers with `Application.Download` / `PdfViewer`, the process-wide report queue (progress, cancel, download) seen by every session | `http://localhost:5606` |
| 7 · Modernize, Secure, and Deploy the Capstone | `Module 7` | theme + mixin, `ClientProfiles.json` and responsive layouts, server-side auth + audit log + download guard, `AllowHtml` only through a sanitizer, operations dashboard, readiness checklist, IIS / Docker deployment assets, the migration report | `http://localhost:5607` |

Run any module from its `OrderDesk.Web` project folder. The projects multi-target `net10.0-windows` and `net10.0`,
so `dotnet run` needs a framework (`-f net10.0`, or `-f net10.0-windows`), e.g.

```bash
cd "D:/Projects/LearnWisej-Samples/From WinForms to the Web Course/Module 4/OrderDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5604
```

or open the `OrderDesk.slnx` in the module folder with Visual Studio and press F5 (Modules 1 and 2 also contain the
WinForms project `LegacyOrderDesk`, which runs on Windows only). Port = 5600 + module number.

## `_template`

The scaffold every module was built from (the shell, the shared `Domain/`, `Views/Ui.cs` + `Views/TracePanel.cs`, and
the WinForms original), plus `COOKBOOK.md`: the Wisej.NET migration conventions verified while building these samples
(startup files, non-blocking `ShowDialog`, `Application.Session`, `Download`, `PdfViewer`, `Upload`, `VirtualMode`,
`ErrorProvider`, `AllowHtml`, client profiles, themes) and the gotchas found along the way. Read it before writing a
new sample.

## The worked example

The same data runs through every module: orders 1042 Northwind Traders 4,820.00 Open · 1041 Contoso Ltd 1,290.50
Shipped · 1040 Fabrikam Inc 760.00 Open · 1039 Adventure Works 12,400.00 Invoiced · 1038 Globex Corp 3,090.00 Hold,
and the two users from the videos, **kelly** (Acme) and **sam** (Globex), whose second browser tab is the multi-user
test in Modules 1, 4, 6 and 7.
