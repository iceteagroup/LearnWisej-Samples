# SupportDesk · Data Binding with EF Core · Module 6

Local lab build for **Module 6 · Async Loads, Related Data and Performance**: the ticket browser with EF Core
logging on in Development, the grid query as a no-tracking projection with paging of 50 in SQL, and the
editor's related data loaded deliberately (the last five comments by a filtered no-tracking read, the full
history by explicit loading on demand). The naive branch the lab starts from is kept in
`TicketQueryService.SearchTicketsNaiveAsync` as the measured baseline for the tests and the lab notes.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Data Binding with EF Core Course/Module 6/SupportDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5406
```

In Development every EF Core command is written to the console with an `[EF]` prefix (`LogTo`), with its
SQL and elapsed time. Tests: `dotnet test SupportDesk.Tests` (`TicketPerformanceTests` compares the two branches).

## What to try

- **Search / Next / Previous**: the console shows two statements per page of 50, a `COUNT` and one
  `SELECT` that joins the customer, agent and category names. The grid shows Number, Title, Customer, Agent,
  Category, Status, Due and Updated. Search, Next and Previous are disabled while a page loads.
- **Empty result, failed query**: *No tickets match these filters*; a failure shows a friendly `AlertBox`.
- **Edit Ticket**: the comments list shows the last five comments; **Show full history** loads them all.

## Deliverables

| # | Deliverable | Where |
|---|---|---|
| 1 | EF Core logging in development with the unoptimised SQL and timing recorded | [`docs/LoggingInDevelopment.md`](docs/LoggingInDevelopment.md) · `SupportDeskDataServiceCollectionExtensions.cs`, `appsettings.Development.json` |
| 2 | Grid query as a no-tracking projection | [`docs/NoTrackingProjection.md`](docs/NoTrackingProjection.md) · `TicketQueryService.SearchTicketsAsync` |
| 3 | Paging with a page size of 50 and indexed filters | [`docs/PagingAndIndexedFilters.md`](docs/PagingAndIndexedFilters.md) · `TicketQueryService.ApplyFilters`, `RunSearchAsync` |
| 4 | No lazy-loading N+1 behaviour in display code | [`docs/RelatedDataDecisions.md`](docs/RelatedDataDecisions.md) · `TicketDetailService.cs`, `TicketEditorForm.RenderComments` |
| 5 | Measured before-and-after results | [`docs/BeforeAfterMeasurements.md`](docs/BeforeAfterMeasurements.md) |

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| 1 · The naive branch: tracked tickets, no paging, names through navigations | `TicketQueryService.SearchTicketsNaiveAsync` (kept as the baseline) |
| 2 · Logging with `LogTo`, sensitive-data logging behind the Development check | `SupportDeskDataServiceCollectionExtensions.AddSupportDeskData` |
| 3 · No-tracking projection into `TicketListItem` | `TicketQueryService.RunSearchAsync` |
| 4 · Paging of 50: `CountAsync`, `Skip`/`Take`, `PagedResult<TicketListItem>` | `RunSearchAsync`, `TicketBrowserPage.PageSize` |
| 5 · Index-friendly filters | `TicketQueryService.ApplyFilters` (`StartsWith` on Number; Status, CustomerId, UpdatedAt indexes) |
| 6 · No navigation access in display code; related data by filtered read or explicit load | `TicketDetailService`, `TicketEditorForm.LoadRecentCommentsAsync` / `btnShowFullHistory_Click` |
| 7 · The async guard; Search, Next, Previous disabled while loading | `TicketBrowserPage.LoadTicketsAsync`, `SetBusy` |
| 8 · Loading, empty page, no results and failure visible | `LoadTicketsAsync` |

## Self-check answers

- **The log showed many statements for one screen of 50 tickets. Which code caused the extra ones, and why did nothing warn you?**
  The per-row `Reference(...).LoadAsync()` calls in the naive branch, the same queries a `CellFormatting`
  handler would trigger under lazy loading. Each call succeeds and looks like ordinary code; only a statement
  log shows the cost. On this seed EF Core's reference fix-up keeps the count at 15 instead of the lesson's
  151 (only 14 distinct related rows); forcing independent lookups measures 131. See
  [`docs/BeforeAfterMeasurements.md`](docs/BeforeAfterMeasurements.md).
- **Which loading strategy serves the ticket with its customer and category, the last five comments, and the full history?**
  `Include` for the fixed-size aggregate (`TicketDetailService.LoadForEditorAsync`), a filtered no-tracking
  read for the last five comments, and explicit loading for the full history on demand. Projection is wrong
  for the editor because the save needs a tracked entity loaded by key.
- **A teammate proposes compiled queries and context pooling first. What would you measure?**
  The statement count and milliseconds in the log. Here the optimised search sends two statements in about a
  millisecond; the problem was round trips, which neither compiled queries nor pooling reduce. Add them only
  when the log shows translation or context construction dominating under real load.
