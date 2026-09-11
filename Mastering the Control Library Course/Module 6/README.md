# OperationsConsole · Mastering the Control Library · Module 6

Lab build for **Module 6 · Charts, Dashboards, Content, Media, and Documents**: the **Dashboard** tab — a ChartJS bar
chart of tickets opened vs closed, a `ProgressBar` for this month's completion, a `PdfViewer` preview, an `HtmlPanel`
summary, an `Upload` workflow that stores the file on the server, a reserved map slot, and one
`RefreshDashboard(DashboardModel)` method that writes to all of them from the model `DashboardService.GetDashboard()`
returns. The question the tab answers is on the page: *are we on track for this month's ticket target?*

This module fills `Sections/DashboardPage` and adds its models, services and documents. It also restores the
`Wisej-4-ChartJS` 4.1.0 NuGet package (4.1.2 needs `Wisej-4` ≥ 4.1.2).

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Mastering the Control Library Course/Module 6/OperationsConsole"
dotnet run -f net10.0 --urls http://localhost:5706
```

Then open <http://localhost:5706> and select **Dashboard**.

## What to try

| Action | What you should see |
|---|---|
| Open **Dashboard** | the chart (Opened blue, Closed green), the bar at 78 %, the sample PDF, green status "Dashboard ready — …" |
| **Refresh** | the button disables with a loader, loaders on the chart and preview, then `Last refreshed HH:mm:ss`, a Toast, and slightly taller bars for the current month |
| Choose a PDF ≤ 2 MB in **Upload report** | the card turns green, the preview swaps to the uploaded document |
| Choose a file over 2 MB, or not a PDF | the file never leaves the browser; amber AlertBox and status explain why |
| Tick **Simulate service failure**, **Refresh** | red status, an AlertBox, the previous numbers stay, the stamp says the refresh failed. Untick and refresh to recover |
| **Refresh** on the ToolBar | runs the page's own Refresh |

## Where things live

```
OperationsConsole/
├─ Sections/DashboardPage.cs / .Designer.cs   the tab: ConfigureChart(), ConfigureUpload(), RefreshDashboard(model)
├─ Models/DashboardModel.cs                   the view model (+ PreviewDocument)
├─ Services/DashboardService.cs               in-memory ticket table, GetDashboard() → aggregates only
├─ Services/DocumentStore.cs                  Validate() (server-side rule), Store(), OpenRead()
├─ Dashboard/PreviewHtml.cs                   the fixed, encoded HTML template for htmlPreview
├─ wwwroot/sample-report.pdf                  the document shown until something is uploaded
└─ docs/DashboardNotes.md                     the question, the control decisions, the Upload boundary
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| Add the ChartJS package through NuGet | `OperationsConsole.csproj` — `Wisej-4-ChartJS` 4.1.0 |
| Write down the question; `DashboardModel`; `DashboardService.GetDashboard()` returning aggregates | `docs/DashboardNotes.md`, `lblQuestion`; `Models/DashboardModel.cs`; `Services/DashboardService.cs` |
| ChartJS `chartTickets` with axes, legend, min/max, colours; `Labels` and `DataSets` filled together | `ConfigureChart()`; `RefreshDashboard()` + `BuildSeries()` |
| `ProgressBar progressCompletion` with Minimum / Maximum | the designer + `RefreshDashboard()` |
| `PdfViewer pdfPreview` / `HtmlPanel htmlPreview` from the model; map placeholder with the API-key note | `ShowPreview()`, `Dashboard/PreviewHtml.cs`; `pnlMapPlaceholder` |
| `Upload` with allowed types and max size, handled on the server, stored through a service, preview updated | `ConfigureUpload()`, `uploadReport_Uploaded()`, `Services/DocumentStore.cs` |
| One `RefreshDashboard(DashboardModel)`; thin `btnRefresh_Click` with a loading state | `RefreshDashboard()`; `btnRefresh_Click()` → `RefreshFromServiceAsync()` + `SetLoading()` |
| Loading, refreshed timestamp, upload rejection, service failure | `SetLoading()`, `lblLastRefreshed`, `uploadReport_Error()` / `RejectUpload()`, the `catch` in `RefreshFromServiceAsync()` |

## Self-check answers

- **Which control does the most important work?** `progressCompletion`: only it answers "this month's target"; the
  chart gives context, the bar gives the answer.
- **What breaks first at ten times the data, and why do aggregates protect the chart?** The service's aggregation loop
  (in production, the query behind it) — one server-side pass you can index or cache. The payload stays six labels
  and twelve numbers, so the chart and the browser do not notice.
- **Where is the uploaded file at each step, and why can the server never open a typed path?** User's disk → in flight
  → server memory (`e.Files.Get(0)`) → validated and stored as `DOC-000001` → streamed back to `pdfPreview`. A typed
  path names a file on the user's machine; only what the browser sends ever crosses the boundary.

## Known simplifications

- Uploaded files stay in memory for the session.
- The ticket table drifts a little on every refresh so the screen visibly changes.
- The map slot is a reserved `Panel`: GoogleMaps needs an API key.
