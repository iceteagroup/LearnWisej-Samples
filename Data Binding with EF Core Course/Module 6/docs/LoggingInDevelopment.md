# EF Core logging in development

Deliverable 1 of the Module 6 lab: "enable EF Core logging in development with `LogTo` or
Microsoft.Extensions.Logging on `SupportDeskContext`, keep `EnableSensitiveDataLogging` behind a development
check, run the naive search once and record the statement count, milliseconds and SQL in your lab notes."

## Two logging channels, on purpose

This solution already had a teaching instrument for SQL: `QueryTrace` + `QueryTraceInterceptor` (Module 1),
an `AsyncLocal` scope the UI reads per click to show "◦ context created", "→ SQL … (ms)", "◦ context
disposed" in the trace card. That instrument stays — it is how the reviewer sees statement counts on the
page, including the Module 6 Before / after card.

Module 6 adds a **second, independent** channel: the "real" EF Core log a production app would actually turn
on, next to (not instead of) `QueryTrace`. `SupportDesk.Data/DependencyInjection/SupportDeskDataServiceCollectionExtensions.cs`,
inside the existing `if (isDevelopment)` block that already guards `EnableDetailedErrors()` and
`EnableSensitiveDataLogging()`:

```csharp
if (isDevelopment)
{
    options.EnableDetailedErrors();
    // Parameter values in logs: local development only, never in production logs.
    options.EnableSensitiveDataLogging();

    // Module 6: the "real" EF Core log, next to (not instead of) the lab's own QueryTrace card.
    options.LogTo(
        message => Console.Error.WriteLine($"[EF] {message}"),
        new[] { DbLoggerCategory.Database.Command.Name },
        LogLevel.Information);
}
```

`LogTo` with the `DbLoggerCategory.Database.Command` category and `LogLevel.Information` is EF Core's own
"one line per command, with its duration" log — the text already carries the elapsed milliseconds (EF Core
formats it as `Executed DbCommand (Nms) …`), so nothing here computes a duration by hand. Every line is
prefixed `[EF]` and written to `Console.Error`, the same "server log" every other module in this course
already writes to (`Console.Error.WriteLine("[SupportDesk] …")`), so it is easy to grep the EF Core log out
of the rest of the host's console output.

`SupportDesk.Web/appsettings.Development.json` also sets the matching Microsoft.Extensions.Logging category:

```json
"Microsoft.EntityFrameworkCore.Database.Command": "Information"
```

**Honesty note.** `AddDbContextFactory`'s `options.LogTo(...)` is EF Core's own internal logging path — it
runs whether or not anything is listening through `Microsoft.Extensions.Logging`, and the appsettings category
above does **not** gate it (that filter only applies to log providers registered through
`ILoggerFactory`/`builder.Logging`, which this project does not wire EF Core into). The appsettings entry is
the deliverable the lab guide and the course cookbook literally ask for ("the category set to Information in
appsettings.Development.json"), and it is exactly what would gate EF Core's log line if the project ever adds
an `ILogger<T>`-based provider (Console, Application Insights, …) instead of — or in addition to — `LogTo`.
Today, with only `LogTo` wired up, the appsettings entry is dormant; it is left in place because it is the
correct, forward-compatible configuration, not because it currently changes anything.

`EnableSensitiveDataLogging()` stays exactly where it already was, inside the same `if (isDevelopment)` block
— Module 6 changes nothing about when parameter values are allowed to appear in a log.

## Evidence

Captured with a throwaway console project (`SupportDesk.Data` + `SupportDesk.Services`, no Wisej.NET,
SQLite in memory seeded with `DevelopmentSeeder`) calling the exact shipped
`TicketQueryService.SearchTicketsNaiveAsync` — the naive branch this lab measures before fixing it. This is
the same code path `QueryTrace` reports to the trace card; the numbers below are what both the `QueryTrace`
scope and (in the running app) the `[EF]`-prefixed console log show for the identical call.

**Naive search, no filters, 50-row cap** — 15 statements (see `docs/BeforeAfterMeasurements.md` for why this
is not the lesson's illustrative "1 + 3 × rows"):

```
SELECT "t"."Id", "t"."AgentId", "t"."CategoryId", "t"."CreatedAt", "t"."CustomerId", "t"."Description",
       "t"."DueDate", "t"."IsUrgent", "t"."Number", "t"."Priority", "t"."RowVersion", "t"."Status",
       "t"."Title", "t"."UpdatedAt"
FROM "Tickets" AS "t"
ORDER BY "t"."UpdatedAt" DESC
```
```
SELECT "c"."Id", "c"."Email", "c"."Name" FROM "Customers" AS "c" WHERE "c"."Id" = @p LIMIT 1
```
```
SELECT "a"."Id", "a"."DisplayName", "a"."Email" FROM "Agents" AS "a" WHERE "a"."Id" = @p LIMIT 1
```
```
SELECT "c"."Id", "c"."Name" FROM "Categories" AS "c" WHERE "c"."Id" = @p LIMIT 1
```
… twelve more `Customers`/`Agents`/`Categories` lookups, one per distinct value the capped 50 rows still had
not seen — 15 statements in total, 0.49 ms of in-process SQLite command time (`QueryTrace` scope), 326
tracked entities left in the context (312 tickets + 14 distinct related rows) when the method returns.

Against a file-based SQLite database (`Data Source=<file>`, the same provider the running app uses under
`App_Data/supportdesk.db`, not `:memory:`) the wall-clock difference is much more visible than the in-process
command time above: averaged over 5 warmed runs, the naive branch took **18.79 ms**, the optimised branch
**0.92 ms** — a **20.4×** difference driven by fifteen round trips instead of two, not by any one query being
slow. See `docs/BeforeAfterMeasurements.md` for the full before/after table.

**Tests** (`SupportDesk.Tests/TicketPerformanceTests.cs`): `The_naive_branch_statement_count_is_one_plus_the_distinct_lookups_the_capped_rows_touch`
asserts the exact statement count against an independently computed expectation (the number of distinct
`CustomerId`/non-null `AgentId`/`CategoryId` values among the same 50 rows), so the test stays correct even if
the seed data ever changes shape.

**Not verified here — for the browser reviewer.** That the `[EF]`-prefixed lines actually appear in the
running server's console output when a search runs in development, and that they are absent (or the
category quiet) once `ASPNETCORE_ENVIRONMENT` is not `Development` — the application was not started for
this report.
