# Lifetimes and session safety · no static DbContext anywhere

Deliverable 5 of the Module 1 lab: the four lifetimes of a Wisej.NET + EF Core application, where each object
of the Support Desk console lives, and the proof that nothing shares a `DbContext`.

## The four lifetimes

| Lifetime | Owner | In this solution | Lives for |
|---|---|---|---|
| **Session state** | Wisej.NET | `Application.Session`, `Application.SessionId`, the `TicketBrowserPage` created by `Program.Main` | the browser tab — minutes to hours |
| **UI object** | the Page | `TicketBrowserPage`, its labels, the trace `ListBox`, the `_loading` flag, later the `BindingSource` and the selected row | as long as the session keeps the page |
| **Request / thread** | one click | the `countButton_Click` handler; after an `await` the continuation may resume on any thread | milliseconds to seconds |
| **Unit of work** | `DbContext` | one `SupportDeskContext` created from the factory inside `TicketQueryService.CountTicketsAsync` | one operation — created and disposed inside the service call |

The page prints this table live (**Four lifetimes on one server**): the session id, the page instance, the number of
handler runs and the contexts created versus disposed. The last column is always `0 alive between clicks`.

## What is shared and what is not

Two users open the console at the same time and both click **Count**:

| Object | Per session or shared | Why it is safe |
|---|---|---|
| `TicketBrowserPage`, `statusLabel`, `listTrace`, `_loading` | per session | Wisej.NET creates one page per session; handlers of one session run one at a time |
| `SupportDeskContext` | per operation | created by `CreateDbContextAsync` inside the click, disposed before the handler continues |
| `IDbContextFactory<SupportDeskContext>` and its options | shared (singleton) | immutable after startup, thread-safe by contract |
| `TicketQueryService` | transient (one per page) | holds only the factory — no context, no entities, no UI state |
| `DevelopmentOutageSwitch` | shared (singleton lab prop) | one boolean, development only; a production app has no such object |
| The SQLite file | shared | the database is the shared state by design; EF Core's connection handling serialises access |

## The proof

1. **Search the solution.** `grep -rn "static.*SupportDeskContext\|static.*DbContext" --include=*.cs` finds nothing.
   Every `SupportDeskContext` in the solution is created by `_dbFactory.CreateDbContextAsync(...)` or, in the tests, by
   `SqliteTestFactory.CreateDbContext()`, and sits in an `await using`.
2. **Watch the trace.** Every statement is bracketed by `◦ context #n created` and `◦ context #n disposed (0 tracked
   entities released)` with the same number. Two clicks never share a number.
3. **Run the anti-pattern on purpose.** The **Two ops, one context** button takes one context and runs two counts on it
   from two threads — what a `static SupportDeskContext` would do when two sessions click at once. EF Core throws
   `InvalidOperationException: A second operation was started on this context instance before a previous operation
   completed`. The demo lives in `SupportDesk.Services/SharedContextAntiPattern.cs`, named for what it is, and nothing
   in the real code path can reach that state.
4. **The tests.** `Each_operation_creates_and_disposes_its_own_context` asserts two contexts created and two disposed
   for two calls.

## Why the alternatives were rejected

- **A context per page (session).** Hours of life, a change tracker that keeps every entity it ever loaded, a stale
  view of the database, and — because Wisej.NET delivers a second click while an awaited operation is pending — two
  operations on one instance. The anti-pattern button shows the last point.
- **A context per DI scope (`AddDbContext`).** There is no Microsoft DI scope that matches "one click" in Wisej.NET;
  `[Inject]` resolves through the root provider (a `Scoped` registration fails in Development, see
  [DependencyInjectionBridge.md](DependencyInjectionBridge.md)). A form-scoped context is acceptable only for a bounded
  edit workflow with no concurrent operations and explicit disposal (Module 4 discusses it and still chooses per operation).
- **A static context.** One instance for every user: cross-session data leaks, the exception above, and one broken
  connection taking the whole application down.

## Evidence

```
09:47:05.149 • countButton_Click TicketQueryService.CountTicketsAsync()
09:47:05.151 ◦ context #2 created (SupportDeskContext from the factory)
09:47:05.189 → SQL SELECT COUNT(*) FROM "Tickets" AS "t" (0.1 ms)
09:47:05.190 ◦ context #2 disposed (0 tracked entities released)
09:47:05.190 ← result 12 tickets · 1 statement(s) · 0.1 ms in the database · 1 context created, 1 disposed

• anti-pattern one context, two concurrent CountAsync calls — what a static/shared DbContext does when two sessions use it
• caught       InvalidOperationException: A second operation was started on this context instance before a previous operation completed. (round 1)
```

The lifetimes table after the runs above: `unit of work  DbContext  3 created · 3 disposed · 0 alive between clicks`.
