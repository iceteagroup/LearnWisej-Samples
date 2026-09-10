# DashboardNotes.md — Operations Console, Module 6

## The question this dashboard answers

> **Are we on track for this month's ticket target?**

That sentence was written before a single control was dropped on the tab, and it is on the screen
(`lblQuestion`, under the section title) so nobody has to guess what the tab is for. Every control below had to
earn its place by helping answer it; anything that did not was left out. That is the whole method of this module:
*start from the question, then choose the control.*

The question has three parts, and the three main controls map onto them one to one:

| Part of the question | Control | Why that control |
|---|---|---|
| "…on track…" — is the pattern going the right way? | `chartTickets` (ChartJS · **Bar**) | A **pattern** over six months. Bars compare month against month at a glance; the series are *categories* (six discrete months), not a continuous signal, so bars read more honestly than a line. |
| "…this month's target?" — one number | `progressCompletion` (`ProgressBar`) | A **single status value**. A second chart for one number is clutter; a bar with `Minimum = 0`, `Maximum = 100` reads as a percentage without a legend, an axis or a tooltip. |
| "…and what is the evidence?" | `pdfPreview` (`PdfViewer`) + `htmlPreview` (`HtmlPanel`) + `uploadReport` (`Upload`) | **Content**: the SLA report the numbers came from, the model in words, and the way a new report gets to the server. |

## Control decisions, one by one

| Control | Decision | Alternative considered (and why it loses) |
|---|---|---|
| `chartTickets` — `Wisej.Web.Ext.ChartJS.ChartJS`, `ChartType.Bar` | Two `BarDataSet`s (Opened blue `21,101,216`, Closed green `31,157,87`), a legend at the top because there are two series, a **fixed y-axis 0–100 with a step of 20**, named axes ("Month", "Tickets"), grid lines off on x and quiet on y, tooltips on. All of it in one method, `ConfigureChart()`. | **Line chart** — fine for a trend, but it implies a continuous signal between months. **Pie/doughnut** — six months is not a part-to-whole relationship; the module lists that as a pitfall. **DataGridView** — the right control when the *records* matter, which is what the Orders tab from Module 5 is for. |
| Fixed axis `Min = 0`, `Max = 100` | A refresh brings new tickets, and an auto-scaled axis would silently rescale between two refreshes — the bars would look the same while the numbers changed. A fixed scale makes two screenshots comparable. | Auto-scale: less code, but it makes the chart lie about change. |
| `progressCompletion` — `ProgressBar` | `Minimum = 0`, `Maximum = 100`, `Value = model.CompletionPercent`, `BarColor` green at or above the 85 % target and amber below it. The bar is the whole indicator; the words under it come from the model. | A **gauge widget** (Module 7's territory): more pixels, a JavaScript dependency and a theming problem for a number a native control already shows. The control-library habit is native first. |
| `pdfPreview` — `PdfViewer` | `ViewerType = Auto`, fed from the model's document *reference*: `PdfSource = "wwwroot/sample-report.pdf"` for the report shipped with the lab, `PdfStream` over the stored bytes once a report has been uploaded. | **A download link** — makes the user leave the dashboard. **`WebBrowser`/`IFramePanel`** — right for arbitrary external content, too open for one known document. |
| `htmlPreview` — `HtmlPanel` | The model in words next to the bar (percentage, "50 of 64 closed in Sep", target gap, how many tickets were aggregated, which document is previewed, generation time). The markup is a fixed template in `Dashboard/PreviewHtml.cs`, and **every** value goes through `WebUtility.HtmlEncode`; no script, no external stylesheet, no link. | Concatenating model values straight into HTML — the pitfall the module names: a file called `<script>alert(1)</script>.pdf` would then execute in the preview instead of reading as text. |
| `uploadReport` — `Upload` | `AllowMultipleFiles = false`, `AllowedFileTypes = ".pdf"`, `MaxFileSize = 2 MB` — and all three values come from `DocumentStore`, so the browser filter and the server rule cannot drift apart. `Uploaded` is handled on the server, the file is stored through `DocumentStore`, and the preview is rebuilt **from the stored document**. | Asking for a path/filename in a `TextBox` — the server cannot open a path on the user's machine. That is the boundary mistake the module warns about. |
| `pnlMapPlaceholder` — reserved `Panel` | A dashed slot with the note "GoogleMaps needs an API key configured on the component" and, in the designer comment, the three lines that would create the component. Geography is a real question for a service desk; a key is not this lab's to ship (and it is billed per load). | Shipping a key, or dropping a screenshot in a `PictureBox` and calling it a map. |
| `tlpDashboard` — `TableLayoutPanel`, all percent | Five cards, 3 columns × 2 rows, chart spanning both rows. The page is hosted docked `Fill` inside the shell's TabPage, so nothing may depend on a fixed size. | Absolute `Location`/`Size` — breaks on the first resize. |

## One model, one refresh

```
DashboardService.GetDashboard()      // 293 ticket rows in, 12 numbers out
        ↓ DashboardModel (months, opened, closed, completion %, target %, document reference, generated at)
RefreshDashboard(model)              // the ONLY writer of chartTickets / progressCompletion / pdfPreview /
                                     // htmlPreview / lblLastRefreshed
```

- `chartTickets.Labels` and `chartTickets.DataSets` are assigned **together**, in `RefreshDashboard` and nowhere
  else. Labels are part of chart data: a data set without them has nothing to plot against.
- No click handler builds chart data. `btnRefresh_Click` is three lines: show the loading state, await one model,
  hand it to `RefreshDashboard`.
- The service returns **aggregates**. The ticket table is a `private sealed class Ticket` inside
  `DashboardService`; nothing outside can take a dependency on it, which is what keeps the payload small when the
  table grows.
- Every refresh stamps `lblLastRefreshed`, so the user always knows how old the numbers are.

## Browser ⇄ server boundary (the Upload workflow)

| Step | Where the file is | What runs |
|---|---|---|
| 1 · select | the user's disk | the browser's file dialog, filtered by `AllowedFileTypes` |
| 2 · upload | in flight | the browser posts the bytes; `MaxFileSize` can refuse before sending → `Upload.Error` |
| 3 · arrive | server memory | `uploadReport_Uploaded`, `e.Files.Get(0)` — `FileName`, `ContentLength`, `ContentType`, `InputStream` |
| 4 · validate | server | `DocumentStore.Validate(name, size)` **against the bytes that arrived**, not against what the client said |
| 5 · store | server memory | `DocumentStore.Store(...)` → `DOC-000001` |
| 6 · preview | server → browser | a fresh model references the stored document; `RefreshDashboard` streams it into `pdfPreview` |

The server never browses the user's disk — it only ever sees what the browser chose to send. That is why the
same size/type rule exists twice, and why the two "Simulate …" buttons call `DocumentStore.Validate` directly:
the browser normally blocks an oversized or non-PDF file first, and the server-side rule has to be visible anyway.

## Evidence (what the running app shows)

- **Ready / first paint** — opening the Dashboard section logs `DashboardPage · the question: Are we on track for
  this month's ticket target?` and `initial snapshot — 293 tickets aggregated into 6 monthly points (no service
  latency)`. The chart shows Apr–Sep opened vs closed, the bar sits at 78 %, `pdfPreview` shows
  `wwwroot/sample-report.pdf`, the caption reads `SLA report — sample · shipped in wwwroot/sample-report.pdf ·
  DOC-SAMPLE`, and the status is green: `Dashboard ready — 78% of the 85% target (50 of 64 tickets closed in Sep).`
- **Loading state** — pressing **Refresh** disables the button and shows its loader, puts a loader on the chart and
  the PdfViewer, disables the Upload and the two simulate buttons, writes `Refreshing…` over the stamp and logs
  `btnRefresh → loading state on, DashboardService.GetDashboard() (~650 ms)`.
- **Refreshed timestamp** — when the model arrives the log reads `✓ dashboard model 09:41:12 — 296 tickets
  aggregated into 6 monthly points …` followed by `RefreshDashboard(model) → chartTickets (6 labels, 2 DataSets),
  progressCompletion 80%, pdfPreview.PdfSource, htmlPreview, lblLastRefreshed`; the stamp becomes
  `Last refreshed 09:41:12`, a green Toast says *Dashboard refreshed*, and the bars for the current month grow a
  little on every press (the queue moves on).
- **Upload success** — choosing a PDF ≤ 2 MB logs `uploadReport.Uploaded → sla-report-sep.pdf (1.2 MB,
  application/pdf) — the bytes are on the server now`, then `✓ stored as DOC-000001 in DocumentStore (in memory,
  1.2 MB) — rebuilding the preview from the stored bytes`. The Upload card turns green, the preview swaps to the
  uploaded document, and the caption shows `… · uploaded 09:42:07 · DOC-000001`.
- **Upload rejected by the browser** — a file over 2 MB never leaves the browser: `Upload.Error` fires,
  the log gets `✗ uploadReport.Error → FileTooLarge · … · <framework message>`, and the user sees an amber
  AlertBox: *"… is 5 MB — reports have to be 2 MB or smaller."*
- **Upload rejected by the server (size)** — **Simulate oversized upload** logs
  `DocumentStore.Validate("sla-report-september-full.pdf", 5 MB) — the check that runs on the server whatever the
  browser allowed`, then `✗ rejected on the server before a byte was stored`; the status turns amber and the card
  says *Not accepted: That file is 5 MB — the limit is 2 MB.*
- **Upload rejected by the server (type)** — **Simulate wrong-type upload** does the same for
  `tickets-export.xlsx`: *Not accepted: Only .pdf reports are accepted — that file is a .xlsx file.*
- **Service failure** — tick **Simulate service failure** and press **Refresh**: the loading state appears, then
  `✗ GetDashboard failed — InvalidOperationException` plus the exception message go to the Event log **only**, the
  status turns red (*"The dashboard could not be refreshed — the numbers on screen are the last good ones."*), an
  AlertBox says the same in plain words, the previous numbers stay on screen, the stamp reads
  `Last refreshed 09:41:12 · retry failed`, and the checkbox resets itself.
- **Recovery** — press **Refresh** again: the model arrives, the status is green and the stamp is current.
- **No internals leak** — exception types, exception messages and framework error text appear in the Event log
  and never in a Toast, an AlertBox or a Label.
