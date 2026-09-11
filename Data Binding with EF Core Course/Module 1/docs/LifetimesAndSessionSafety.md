# Lifetimes and session safety · no static DbContext anywhere

Deliverable 5 of the Module 1 lab: the four lifetimes of a Wisej.NET + EF Core application, where each object
of the Support Desk console lives, and the proof that nothing shares a `DbContext`.

## The four lifetimes

| Lifetime | Owner | In this solution | Lives for |
|---|---|---|---|
| **Session state** | Wisej.NET | `Application.Session`, `Application.SessionId`, the `TicketBrowserPage` created by `Program.Main` | the browser tab: minutes to hours |
| **UI object** | the Page | `TicketBrowserPage`, `countButton`, `statusLabel`, the `_loading` flag, later the `BindingSource` and the selected row | as long as the session keeps the page |
| **Request / thread** | one click | the `countButton_Click` handler; after an `await` the continuation may resume on any thread | milliseconds to seconds |
| **Unit of work** | `DbContext` | one `SupportDeskContext` created from the factory inside `TicketQueryService.CountTicketsAsync` | one operation: created and disposed inside the service call |

## What is shared and what is not

Two users open the console at the same time and both click **Count**:

| Object | Per session or shared | Why it is safe |
|---|---|---|
| `TicketBrowserPage`, `statusLabel`, `_loading` | per session | Wisej.NET creates one page per session; handlers of one session run one at a time |
| `SupportDeskContext` | per operation | created by `CreateDbContextAsync` inside the click, disposed before the handler continues |
| `IDbContextFactory<SupportDeskContext>` and its options | shared (singleton) | immutable after startup, thread-safe by contract |
| `TicketQueryService` | transient (one per page) | holds only the factory: no context, no entities, no UI state |
| The SQLite file | shared | the database is the shared state by design |

## The proof

1. **Search the solution.** `grep -rn "static.*SupportDeskContext\|static.*DbContext" --include=*.cs` finds nothing.
   Every `SupportDeskContext` in the solution is created by `_dbFactory.CreateDbContextAsync(...)` or, in the tests, by
   `SqliteTestFactory.CreateDbContext()`, and sits in an `await using`.
2. **The tests.** `Each_operation_creates_and_disposes_its_own_context` asserts two contexts created and two disposed
   for two calls.
3. **Two sessions.** Open the page in two browser tabs and click **Count tickets** in both: each tab gets its own
   result; a second click in one tab while its count runs does nothing.

## Why the alternatives were rejected

- **A context per page (session).** Hours of life, a change tracker that keeps every entity it ever loaded, a stale
  view of the database, and, because Wisej.NET delivers a second click while an awaited operation is pending, two
  operations on one instance. EF Core refuses that with `InvalidOperationException: A second operation was started on
  this context instance before a previous operation completed`.
- **A context per DI scope (`AddDbContext`).** There is no Microsoft DI scope that matches "one click" in Wisej.NET;
  `[Inject]` resolves through the root provider (a `Scoped` registration fails in Development, see
  [DependencyInjectionBridge.md](DependencyInjectionBridge.md)). A form-scoped context is acceptable only for a bounded
  edit workflow with no concurrent operations and explicit disposal (Module 4 discusses it and still chooses per operation).
- **A static context.** One instance for every user: cross-session data leaks, the exception above, and one broken
  connection taking the whole application down.
