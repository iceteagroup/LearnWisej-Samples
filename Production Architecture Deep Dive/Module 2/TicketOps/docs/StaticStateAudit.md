# Static-state audit — applied to the TicketOps Console

*Module 2 deliverable · lab steps 7 and 8*

**The rule.** A `static` field belongs to the class, and there is one class per server process — not one
per session. In WinForms one process was one user, so a static "current customer" was per-user by
accident. In Wisej.NET one process serves every connected session, so the same field becomes a shared
blackboard: session A writes ticket 42, session B writes 99, A saves to 99. Static is only for data that is
**the same for everyone and read-only (or thread-safe)**; anything per operator or per workflow is
session-scoped.

## Checklist

For every value the app keeps between clicks: *how long should it survive, and who may see it?*

| # | Check | Result | Where it lives now | Evidence |
|---|---|---|---|---|
| 1 | No **selected record id** in a static | ✔ | `SessionContext.SelectedTicketId` (session) | select a row: *Selected ticket* changes in this tab; the other tab's panel still says `(none)` |
| 2 | No **current user / identity** in a static | ✔ | `SessionContext.CurrentUser` (session), set only by `SessionService.SignInAsync` | change the operator in tab A, press ↻ in tab B: tab B keeps its user |
| 3 | No **tenant** in a static | ✔ | `SessionContext.Tenant` (session), guarded by `UserAccount.IsMemberOf` | switch the tenant in tab A: tab A's grid changes, tab B's does not |
| 4 | No **temporary filter** in a static | ✔ | the tenant filter the grid uses is read from the injected context inside `TicketService.GetOpenTicketsAsync` — no field at all | the grid title shows the tab's own tenant |
| 5 | No **theme choice** in a static | ✔ | `SessionContext.Theme` + `Application.Theme` (both per session) | pick Material-3 in tab A: only tab A restyles |
| 6 | No **pending upload / working object** in a static | ✔ (n/a) | nothing of the kind in this module; the place for it is `SessionContext` (implement `IDisposable` when it holds a handle) | — |
| 7 | No **per-session service** in a static | ✔ | `SessionService`, `TicketService`, `DiagnosticsService`, `InMemoryUserDirectory`, `InMemoryTicketRepository`, `ActivityLog` are all created by `AppComposition`, once per session | each tab shows its own *Session id* |
| 8 | **Configuration** is read once and never mutated | ✔ | `ProcessScope.Settings` → immutable `AppSettings` (get-only properties, no setter anywhere) | both tabs show the same *Application* panel |
| 9 | **Shared mutable state** is thread-safe | ✔ | `SharedTicketStore` (one `lock`, copies out), `SharedCounters` (`Interlocked`) | *Sessions started (this process)* grows by one per tab, in every tab |
| 10 | Shared state holds **no per-user meaning** | ✔ | `SharedTicketStore` holds tickets (each carries its own tenant/author); `SharedCounters` holds a total | inspect the two classes: no "current", no "selected", no "user" member |
| 11 | Nothing static holds a reference **back into a session** | ✔ | `ProcessScope`, `SharedTicketStore`, `SharedCounters`, `ThemeCatalog` reference no `SessionContext`, no Form, no `ActivityLog` | a session can be collected when it ends; no leak across the process lifetime |

## Every `static` left in the project, and why it is allowed

`grep -rn "static " --include=*.cs TicketOps` (excluding `bin/`, `obj/`, comments):

| Member | Kind | Verdict |
|---|---|---|
| `SharedTicketStore.Instance` | process-wide store, `lock`-protected, returns copies | allowed — shared durable data, thread-safe |
| `SharedCounters._contextsCreated` / `ContextsCreated` / `ContextCreated()` | `Interlocked` counter | allowed — a process total, thread-safe, no per-user meaning |
| `ProcessScope._settings` (`Lazy<AppSettings>`), `Settings`, `SettingsAlreadyLoaded` | read once, immutable result | allowed — configuration, read-only after construction |
| `SeedData.Tickets()`, `OperationResult.Ok/Fail`, `StatusBanner.ColorFor`, `AppSettings.Load`, `ThemeCatalog.Load`, `WisejRuntimeInfo.ReadSessionId`, `SharedTicketStore.Clone` | pure functions / factories | allowed — no state |
| `Strings.*` | `const` text | allowed — immutable |
| `Program.Main` | entry point | required by Wisej.NET; it composes, it stores nothing |

The lab's auto-check (`static\s+\w+\s+(_?session|_?user|_?current|_?selected)`) finds nothing in the project.

## What "session-scoped" means here (lab step 3)

`AppComposition` is instantiated by `Program.Main`, which Wisej.NET calls once per browser session. Every
`new` in its constructor therefore runs once per session — that is the registration with session lifetime,
written by hand. The objects are dropped when the session ends (timeout, tab closed) because nothing static
references them. Module 8 expresses the same thing declaratively with `Application.Services` and
`ServiceLifetime.Session`.

## Evidence (what the running app shows)

1. Open <http://localhost:5102> in two browser tabs (A and B). Each shows a different **Session id**. (item 7)
2. Tab A: operator → *Bob Chen*, **Apply to this session**. Tab A's panel: User = Bob Chen; window caption
   changes. Tab B: press **↻** — User is still Alice Rivera. (item 2)
3. Tab A: tenant → *Fabrikam* (Alice is a member), Apply. Tab A's grid shows the Fabrikam tickets; tab B's grid
   is unchanged after ↻. (items 3, 4)
4. Tab A: theme → *Material-3*, Apply. Only tab A restyles. (item 5)
5. Tab A: select ticket 1003. Tab B after ↻: *Selected ticket = (none)*. (item 1)
6. Both tabs after ↻: the *Application* panel is identical, and *Sessions started (this process)* counts both
   tabs. (items 8, 9, 10)
