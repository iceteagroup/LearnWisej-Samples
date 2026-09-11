# DashboardNotes.md — Operations Console, Module 6

## The question this dashboard answers

> **Are we on track for this month's ticket target?**

That sentence was written before a control was dropped on the tab, and it is on the screen (`lblQuestion`, in the
command row). Every control below had to earn its place by helping answer it.

| Part of the question | Control | Why that control |
|---|---|---|
| "…on track…" — is the pattern going the right way? | `chartTickets` (ChartJS · **Bar**) | Six discrete months compared at a glance; bars read more honestly than a line for categories. |
| "…this month's target?" — one number | `progressCompletion` (`ProgressBar`) | A single status value. `Minimum = 0`, `Maximum = 100` reads as a percentage without a legend or an axis. |
| "…and what is the evidence?" | `pdfPreview` (`PdfViewer`) + `htmlPreview` (`HtmlPanel`) + `uploadReport` (`Upload`) | The report the numbers came from, the model in words, and the way a new report reaches the server. |

## Control decisions

| Control | Decision | Alternative considered |
|---|---|---|
| `chartTickets` — `ChartType.Bar` | Two `BarDataSet`s (Opened blue, Closed green), a legend on top, a **fixed y-axis 0–100**, named axes, quiet grid lines, tooltips on — all in `ConfigureChart()`. | **Line** implies a continuous signal. **Pie** — six months is not a part-to-whole relationship. |
| Fixed axis 0–100 | An auto-scaled axis would silently rescale between two refreshes. | Auto-scale: less code, but it hides change. |
| `progressCompletion` — `ProgressBar` | `Value = model.CompletionPercent`, `BarColor` green at or above the 85 % target, amber below. | A gauge widget — a JavaScript dependency for a number a native control already shows. |
| `pdfPreview` — `PdfViewer` | `PdfSource = "wwwroot/sample-report.pdf"` for the shipped report, `PdfStream` over the stored bytes after an upload. | A download link — makes the user leave the dashboard. |
| `htmlPreview` — `HtmlPanel` | The completion value in words. A fixed template in `Dashboard/PreviewHtml.cs`; every value goes through `WebUtility.HtmlEncode`. | Concatenating values straight into HTML. |
| `uploadReport` — `Upload` | One file, `.pdf`, max 2 MB — values taken from `DocumentStore`, so the browser filter and the server rule cannot drift. | Asking for a path in a `TextBox` — the server cannot open a path on the user's machine. |
| `pnlMapPlaceholder` — reserved `Panel` | A dashed slot noting that GoogleMaps needs an API key; the designer comment shows how the component would be added. | Shipping a key. |
| `tlpDashboard` — `TableLayoutPanel`, all percent | Five cards, 3 × 2, chart spanning both rows. | Absolute positions. |

## One model, one refresh

```
DashboardService.GetDashboard()      // ticket rows in, 12 numbers out
        ↓ DashboardModel (months, opened, closed, completion %, target %, document reference, generated at)
RefreshDashboard(model)              // the only writer of chartTickets / progressCompletion / pdfPreview /
                                     // htmlPreview / lblLastRefreshed
```

- `chartTickets.Labels` and `chartTickets.DataSets` are assigned together, in `RefreshDashboard` and nowhere else.
- `btnRefresh_Click` is thin: show the loading state, await one model, hand it to `RefreshDashboard`.
- The service returns aggregates; the ticket table is a private class inside `DashboardService`.

## Browser ⇄ server boundary (the Upload workflow)

| Step | Where the file is | What runs |
|---|---|---|
| 1 · select | the user's disk | the browser's file dialog, filtered by `AllowedFileTypes` |
| 2 · upload | in flight | the browser posts the bytes; `MaxFileSize` can refuse before sending → `Upload.Error` |
| 3 · arrive | server memory | `uploadReport_Uploaded`, `e.Files.Get(0)` |
| 4 · validate | server | `DocumentStore.Validate(name, size)` against the bytes that arrived |
| 5 · store | server memory | `DocumentStore.Store(...)` → `DOC-000001` |
| 6 · preview | server → browser | a fresh model references the stored document; `RefreshDashboard` streams it into `pdfPreview` |

## Evidence (what the running app shows)

- **First paint** — the chart shows six months of opened vs closed tickets, the bar sits at 78 %, the preview shows
  the sample report, and the status reads "Dashboard ready — 78% of the 85% target (50 of 64 tickets closed in …)".
- **Loading state** — **Refresh** disables itself and shows its loader, the chart and the preview show loaders, and
  the stamp reads "Refreshing…".
- **Refreshed timestamp** — the stamp becomes `Last refreshed HH:mm:ss`, a Toast confirms, and the current month's
  bars grow a little on every refresh.
- **Upload success** — a PDF ≤ 2 MB is stored, the card turns green and the preview swaps to the uploaded document.
- **Upload rejected** — a file over 2 MB or not a PDF never leaves the browser; an amber AlertBox explains why.
- **Service failure** — tick **Simulate service failure** and press **Refresh**: red status, an AlertBox, the previous
  numbers stay on screen and the stamp says the refresh failed. Untick and refresh again to recover.
