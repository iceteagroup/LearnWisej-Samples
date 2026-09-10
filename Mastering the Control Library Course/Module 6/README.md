# OperationsConsole · Mastering the Control Library · Module 6

Local lab build for **Module 6 · Charts, Dashboards, Content, Media, and Documents**. It follows the lesson
guide, the lab / exam guide and the walkthrough video: the **Dashboard** section of the Operations Console is no
longer a placeholder but a real dashboard tab — a ChartJS bar chart of tickets opened vs closed, a `ProgressBar`
for this month's completion, a `PdfViewer` document preview, an `HtmlPanel` summary, an `Upload` workflow that
stores the file on the server, a reserved map slot — and **one** `RefreshDashboard(DashboardModel)` method that
writes to all of them from a single model returned by `DashboardService.GetDashboard()`.

The question the tab answers is written down first and shown on the page: *are we on track for this month's
ticket target?* Every control had to earn its place by helping answer it.

The course is cumulative: the shell (`MainPage`, `Shell/IConsoleShell`, `ConsoleLog`, `ISection`, the Event log
card) comes from Module 1 and is not touched here; this module replaces only `Sections/DashboardPage` and adds its
own models, services and documents.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Mastering the Control Library Course/Module 6/OperationsConsole"
dotnet run -f net10.0 --urls http://localhost:5706
```

Then open <http://localhost:5706> and click **Dashboard** in the navigation. (Visual Studio: open
`OperationsConsole.slnx` in this folder, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 and **`Wisej-4-ChartJS` 4.1.0** NuGet
packages. (ChartJS 4.1.2 exists but requires `Wisej-4` ≥ 4.1.2; its public API is identical to 4.1.0, so the
sample stays on the 4.1.0 pair the rest of the course uses.)

## What to try

| Action | Path | What you should see |
|---|---|---|
| Open **Dashboard** | first paint | chart Apr–Sep (Opened blue / Closed green), bar at **78 %**, `wwwroot/sample-report.pdf` in the preview, log `DashboardPage · the question: …` + `initial snapshot — 293 tickets aggregated into 6 monthly points (no service latency)`, green status `Dashboard ready — 78% of the 85% target (50 of 64 tickets closed in Sep).` |
| **Refresh** | loading → success | button disabled with a loader, loaders on the chart and the preview, `Refreshing…` over the stamp, then `✓ dashboard model HH:mm:ss — … aggregated into 6 monthly points`, `RefreshDashboard(model) → chartTickets (6 labels, 2 DataSets), progressCompletion …%, pdfPreview.PdfSource, htmlPreview, lblLastRefreshed`, `Last refreshed HH:mm:ss`, a green Toast, and the current month's bars a little taller than before |
| Choose a **PDF ≤ 2 MB** in *Upload report* | upload success | log `uploadReport.Uploaded → <name> (<size>, application/pdf) — the bytes are on the server now` then `✓ stored as DOC-000001 in DocumentStore (in memory, …)`; the card turns green, the preview swaps to the uploaded document, the caption reads `… · uploaded HH:mm:ss · DOC-000001`, status `Report <name> uploaded — preview updated.` |
| Choose a **PDF over 2 MB** | validation (browser) | the file never leaves the browser: `✗ uploadReport.Error → FileTooLarge · … · <framework message>` in the log, amber AlertBox *"… is 5 MB — reports have to be 2 MB or smaller."*, amber status |
| **Simulate oversized upload** | validation (server) | `DocumentStore.Validate("sla-report-september-full.pdf", 5 MB) — the check that runs on the server whatever the browser allowed`, `✗ rejected on the server before a byte was stored`, card: *Not accepted: That file is 5 MB — the limit is 2 MB.* |
| **Simulate wrong-type upload** | validation (server) | the same for `tickets-export.xlsx`: *Not accepted: Only .pdf reports are accepted — that file is a .xlsx file.* |
| Tick **Simulate service failure**, press **Refresh** | failure | loading state, then `✗ GetDashboard failed — InvalidOperationException` + the message **in the Event log only**, red status *"The dashboard could not be refreshed — the numbers on screen are the last good ones."*, an AlertBox in plain words, the previous numbers still on screen, stamp `Last refreshed HH:mm:ss · retry failed`, checkbox resets |
| Press **Refresh** again | recovery | the model arrives, green status, fresh stamp |
| Tick **Simulate service failure**, then upload a PDF | failure (store) | `✗ DocumentStore.Store failed — InvalidOperationException`, red status *"The report could not be stored — the previous preview is still shown."*, the preview unchanged |
| **Refresh** in the shell's command area | shell → section | the shell calls `DashboardPage.RefreshSection()`, which is the page's own Refresh — one refresh path, not two |
| Resize the browser / the section | layout | the five cards are a percent `TableLayoutPanel`; the chart keeps its share, nothing is positioned absolutely |

## Where things live

```
OperationsConsole/
├─ Sections/DashboardPage.cs / .Designer.cs   THIS MODULE — the dashboard tab: five cards in a percent
│                                             TableLayoutPanel, ConfigureChart(), ConfigureUpload(),
│                                             RefreshDashboard(model), btnRefresh_Click, upload handlers
├─ Models/DashboardModel.cs                   the view model (+ PreviewDocument): months, opened, closed,
│                                             completion %, target %, document reference, generated at
├─ Services/DashboardService.cs               the in-memory ticket table and GetDashboard() → aggregates only
├─ Services/DocumentStore.cs                  in-memory document store: Validate() (server-side rule), Store(),
│                                             OpenRead() — what PdfViewer.PdfStream is fed with
├─ Dashboard/PreviewHtml.cs                   the fixed HTML template for htmlPreview; every value HtmlEncoded
├─ wwwroot/sample-report.pdf                  the document PdfViewer shows until something is uploaded
├─ docs/DashboardNotes.md                     deliverable: the question, every control decision, the boundary,
│                                             and the Evidence for each path
├─ docs/ControlSelection.md                   Module 1's deliverable (unchanged)
├─ MainPage.cs / .Designer.cs                 Module 1's shell (unchanged)
├─ Shell/                                     IConsoleShell / ConsoleLog / ISection (Module 1, unchanged)
└─ Sections/*Page.cs                          the other five sections (placeholders until their module runs)
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| Add the ChartJS extension package through NuGet | `OperationsConsole.csproj` — `<PackageReference Include="Wisej-4-ChartJS" Version="4.1.0" />` |
| Write down the question the dashboard answers | `docs/DashboardNotes.md` (first line) and `lblQuestion` on the page; the string itself is `DashboardService.Question` |
| `DashboardModel` record (months, opened, closed, completion %, preview document, generated at) | `Models/DashboardModel.cs` (+ `PreviewDocument`) |
| `DashboardService.GetDashboard()` returning aggregates, never the raw ticket table | `Services/DashboardService.cs` — `GetDashboard()` / `BuildModel()`; the table is the private `Ticket` class |
| ChartJS `chartTickets`, line or bar, axes / legend / min-max / colours set deliberately | `Sections/DashboardPage.cs` → `ConfigureChart()`; the control is created in `DashboardPage.Designer.cs` with `ChartType = Bar` |
| `Labels` and `DataSets` filled together from the model in one place | `Sections/DashboardPage.cs` → `RefreshDashboard()` + `BuildSeries()` |
| `ProgressBar progressCompletion` with Minimum / Maximum bound to the completion value | `DashboardPage.Designer.cs` (`Minimum = 0`, `Maximum = 100`) + `RefreshDashboard()` step 2 |
| `PdfViewer pdfPreview` / `HtmlPanel htmlPreview` fed from the model, HTML constrained | `RefreshDashboard()` step 3 → `ShowPreview()` and `Dashboard/PreviewHtml.cs` |
| Map / content placeholder with the GoogleMaps API-key note | `DashboardPage.Designer.cs` → `pnlMapPlaceholder` (comment) + `lblMapNote` |
| `Upload` with allowed file types and a maximum size, `Uploaded` handled on the server, stored through a service, preview updated from the result | `ConfigureUpload()`, `uploadReport_Uploaded()`, `Services/DocumentStore.cs` |
| One `RefreshDashboard(DashboardModel)` updating chart, indicator, preview and `lblLastRefreshed` | `Sections/DashboardPage.cs` → `RefreshDashboard()` |
| Thin `btnRefresh_Click` with a loading state and the button disabled while it runs | `btnRefresh_Click()` → `RefreshFromServiceAsync()` + `SetLoading()` |
| Show every path: loading, refreshed timestamp, upload validation rejection, service failure | `SetLoading()`, `lblLastRefreshed`, `RejectUpload()` / `ValidateOnServer()` / `uploadReport_Error()`, the `catch` in `RefreshFromServiceAsync()` |
| Short note explaining each control decision | `docs/DashboardNotes.md` |
| Stretch: diagnostic panel (selected control, record id, last refresh) | the page reports through `ConsoleLog.Control(...)` / `ConsoleLog.Record(model.PreviewDocument.Id)`; the panel itself is Module 1's |

## Deliverables

1. **Dashboard tab with a ChartJS bar chart** — `Sections/DashboardPage.cs` / `.Designer.cs` (`chartTickets`)
2. **Chart labels and DataSets built from a dashboard view model** — `RefreshDashboard()` + `Models/DashboardModel.cs`
3. **ProgressBar indicator for one status value** — `progressCompletion`
4. **PdfViewer / HtmlPanel preview plus an Upload workflow** — `pdfPreview`, `htmlPreview`, `uploadReport`, `Services/DocumentStore.cs`
5. **One refresh method that updates every dashboard control** — `RefreshDashboard(DashboardModel)`
6. **The control-decision note** — [`OperationsConsole/docs/DashboardNotes.md`](OperationsConsole/docs/DashboardNotes.md)

## Self-check answers (lab / exam guide)

- **Which control on your dashboard tab is doing the most important work, and what question would go unanswered
  if you removed it?**
  `progressCompletion`. The chart is the most *visible* control, but the question on the page is "are we on track
  **for this month's target**", and only the ProgressBar answers it: the bars say tickets are being closed, not
  whether 50 out of 64 is enough with three weeks gone. Remove the chart and the tab still answers the question,
  minus the context of whether this month is unusual; remove the indicator and the tab answers a different, vaguer
  question ("how has the queue been moving?") and leaves the actual one to mental arithmetic.
- **What would break first if the ticket data set became ten times larger, and why does returning aggregates from
  the service protect the chart?**
  Nothing on the client — and that is the point. The first thing to feel it would be `DashboardService`'s
  aggregation loop (a few thousand rows instead of a few hundred, and in a real system the SQL behind it), which
  is exactly where you want the cost, because it is one server-side pass you can index, cache or push into the
  database. The payload does not grow: `GetDashboard()` returns six labels, twelve numbers, one percentage and one
  document reference whatever the table size, so the chart keeps drawing 12 points, the browser keeps receiving a
  few hundred bytes, and `RefreshDashboard` does not change at all. Return the raw table instead and every one of
  those grows tenfold — serialisation, transfer, the Chart.js render loop and the memory each session holds.
- **Where does the uploaded file exist at each step of the Upload workflow, and why can the server never open a
  path the user typed in?**
  (1) On the user's disk, where only the browser can reach it; (2) in flight, as bytes the browser posts —
  `MaxFileSize` can refuse before this; (3) in server memory, as `e.Files.Get(0)` with its `FileName`,
  `ContentLength`, `ContentType` and `InputStream`; (4) still on the server after `DocumentStore.Validate` accepted
  it and `Store` copied it to a `byte[]` as `DOC-000001`; (5) back in the browser as a rendered preview, streamed
  out of that copy through `pdfPreview.PdfStream`. The server can never open a typed path because the server is a
  different machine from the browser: `C:\Users\...\report.pdf` names a file on the *user's* computer, and the only
  thing that ever crosses the boundary is a file the user explicitly chose and the browser explicitly sent. A path
  is a promise the server cannot keep — which is also why the size and type rules are enforced again on the bytes
  that actually arrived, not on what the client said about them.

## Known simplifications

- `DocumentStore` keeps uploaded files in memory for the life of the session — no disk, no database, no virus
  scan. A real store would stream to disk or blob storage and hand back a URL.
- The ticket table is generated in memory and drifts a little on every refresh (a few tickets opened and closed)
  so the chart, the gauge and the stamp visibly change; a real service would query.
- The map slot is a reserved `Panel`, not a `GoogleMaps` component: that extension needs an API key configured on
  the component, and the key is billed per load.
- `ProgressBar` is used as the "gauge-like indicator" the lab allows; a semicircular gauge would be a Widget, and
  the control-library habit is to reach for the native control first.
