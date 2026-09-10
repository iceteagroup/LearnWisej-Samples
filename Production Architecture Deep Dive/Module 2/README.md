# TicketOps · Production Architecture Deep Dive · Module 2

Local lab build for **Module 2 · Startup, Configuration, Session State & Lifetime**. It follows the
walkthrough video *Build a SessionContext & diagnostics page*: the per-user values the TicketOps Console
kept reaching for (operator, tenant, theme, client profile, the selected ticket) move out of static fields
into a **session-scoped `SessionContext`** that is created once per browser session by `AppComposition`
and constructor-injected into the Form and every service. A **Session diagnostics** page shows, side by
side, what is shared by the whole app (configuration read once per process, the shared ticket store, a
process counter) and what belongs to just this session. A static-state audit is applied to the project
(`docs/StaticStateAudit.md`), with one deliberately quarantined "legacy static" left in as a probe so you can
watch the leak happen between two tabs.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Production Architecture Deep Dive\Module 2\TicketOps"
dotnet run -f net10.0 --urls http://localhost:5102
```

Then open <http://localhost:5102>. (Visual Studio: open `TicketOps.slnx`, press F5 — the port is in
`Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.
`dotnet build -nologo -v q` passes with no warnings for both targets (`net10.0-windows`, `net10.0`).

## What to click in the Session diagnostics window

Left card: the **Application** panel (blue — identical in every session) and the **This session** panel
(green — from the injected `SessionContext`), the per-session selectors, and the open tickets of this
session's tenant. Right card: the **Activity trace · UI → Service → Session / Data** — every click is logged
as it crosses a boundary (`[UI]` → `[SVC]` → `[SESSION]` / `[DATA]` → `[UI]`, plus `[INFRA]` for the
once-per-process work and `[CLIENT]` for the theme).

| Button | Path | What you should see |
|---|---|---|
| *(on load)* | startup | `[INFRA] AppComposition — composing the session object graph`, `[INFRA] ProcessScope.Settings — AppSettings loaded now — first session of this process — from …appsettings.json` (or `reused (loaded once per process …)` in every later tab), `[SESSION] AppComposition — new SessionContext {session:…, user:"Alice Rivera", tenant:Contoso, …}`, `[SESSION] Application.Session — … same instance`; both panels filled; 5 Contoso tickets; status **● ready** |
| **Apply to this session** (change operator / tenant / theme first) | success | `[SVC] SessionService.SignInAsync → IUserDirectory.FindAsync`, `[DATA] InMemoryUserDirectory.FindAsync — Bob Chen [Contoso]`, `[SESSION] SessionContext — CurrentUser = "Bob Chen", Tenant = Contoso — session xxxxxxxx… only`; for a theme: `[CLIENT] Application.Theme = Material-3 — this session only`; the green panel and the window caption update; the grid reloads for the tenant; status **● Signed in as Bob Chen · Contoso.** |
| **↻** | — | `[UI] → IDiagnosticsService.GetSnapshot()` and both panels re-read (use it in the *other* tab during the two-tab test) |
| *select a ticket row* | success | `[UI] gridTickets_SelectionChanged — row #1003 → ISessionService.SelectTicket`, `[SESSION] SessionContext — SelectedTicketId = 1003 — this session only`; label **Ticket 1003 selected for this session.**; green panel row *Selected ticket = #1003* |
| **Stamp a ticket for this session** | success | `[SVC] TicketService.CreateForCurrentUserAsync — stamped Tenant=Contoso, Author="Alice Rivera" from SessionContext → ITicketRepository.UpsertAsync`, `[DATA] INSERT INTO Tickets → #9001 written … (shared store, 11 total)`; the row appears in the grid; *Tickets in the shared store* goes up in the blue panel; status **● Ticket #9001 created for Contoso.** |
| **▶ Simulate 20 sessions** | progress | a `Timer` creates two `SessionContext`s per tick: `[SESSION] SessionService.SimulateAnotherSession — #01 → new SessionContext 3f9a12c0… for load-user-01 · contexts on this server: 3 · this session still xxxxxxxx…/Alice Rivera` ×20; the progress bar and **● simulating n/20** advance; ends with *SessionContexts created (this process)* +20 in the blue panel while the green panel is unchanged |
| **Switch to a forbidden tenant** | failure (rule) | `[DOMAIN] ⚠ UserAccount.IsMemberOf — rejected: Alice Rivera is not a member of tenant Northwind (member of Contoso, Fabrikam)`; no `[SESSION]` line follows; orange banner **Alice Rivera is not a member of Northwind.**; status **● not applied**; the tenant did not change |
| **⚠ Write user to legacy static** | the audit probe | `[INFRA] ⚠ StaticLeakProbe.Write — static LastWriter = "Alice Rivera" — ONE copy per server process: refresh the OTHER tab and it reads this value`; blue panel row *⚠ Legacy static probe* shows it — in **every** tab (see the two-tab test) |
| **Simulate directory outage** | error path | `[DATA] ✖ InMemoryUserDirectory — outage: LDAP query … failed — LDAP://dc01.ticketops.local:636 did not answer` stays in the trace; `[UI] ✖ caught DirectoryOutageException — user sees the safe message`; the user sees only the red banner **The user directory is not available right now. Try again in a moment.** and a toast; status **● failed**; the green panel still shows the same user — the context survived the failed call |
| **Recover the directory** (same button) | recovery | the directory answers again: `[SESSION] SessionContext — CurrentUser = …`; status **● Signed in as …** |
| **Clear trace** | — | empties the right-hand card |

### The two-tab test (lab step 9: "run two sessions and confirm they don't leak state into each other")

Open <http://localhost:5102> in a **second browser tab** (or a second browser). Each tab is its own Wisej.NET
session — like a second launch of a desktop app. Call them **A** and **B**.

| Step | In tab A | Then in tab B | Expected |
|---|---|---|---|
| 1 | read *Session id* | read *Session id*; look at the trace | different ids; B's trace says `AppSettings reused (loaded once per process …)`; *Active sessions (Application.SessionCount)* = 2 in both after **↻** |
| 2 | operator → **Bob Chen**, tenant stays Contoso, **Apply to this session** | press **↻** | A: *User = Bob Chen*, caption `… Contoso / Bob Chen`. B: *User = Alice Rivera* — **no leak** |
| 3 | tenant → **Fabrikam** is refused for Bob (member of Contoso only) — pick operator **Alice Rivera** again, tenant **Fabrikam**, Apply | press **↻** | A's grid shows the 3 Fabrikam tickets, A's *Tenant = Fabrikam*. B still shows Contoso and its 5 tickets — **no leak** |
| 4 | theme → **Material-3**, Apply | look | only A restyles; B's *Theme rendering now (Application.Theme)* still says Bootstrap-4 — **no leak** (the per-session `Application.Theme`, not the global `LoadTheme`) |
| 5 | click ticket **2001** | press **↻** | A: *Selected ticket = #2001*. B: *(none)* — **no leak** |
| 6 | **Stamp a ticket for this session** (A is in Fabrikam) | press **↻** | B (Contoso) does not see the row in its grid, but *Tickets in the shared store* went up by one in **both** tabs — durable data is shared on purpose; switch B to Fabrikam and it sees the row |
| 7 | compare the blue panels | — | *Environment / build*, *Dispatch API base URL*, *Upload limit*, *Logging level*, *Idle session timeout (Default.json)*, *Default theme (Default.json)*, *Server* are identical — the **deliberately labelled shared read-only settings** both tabs must see |
| 8 | **⚠ Write user to legacy static** | press **↻** | B's *⚠ Legacy static probe* shows **Alice Rivera (written by session <A's id>)** — B is reading A's user. This is the bug every other row above avoids |
| 9 | press **F5** in A | — | Wisej.NET keeps the session: the same *Session id*, and the trace gains `[SESSION] Application.ApplicationRefresh — browser refresh — same session …: SessionContext, log and services are kept` |

## Deliverables (lab guide)

| # | Deliverable | Where |
|---|---|---|
| 1 | Project runs locally without missing references | `dotnet build -nologo -v q` — 0 warnings, 0 errors, both targets |
| 2 | `SessionContext` class: session id, selected user, tenant, theme, active client profile (+ selected ticket, start time) | `Services/SessionContext.cs` — plain C#, no Wisej.NET type |
| 3 | Registered per session (session lifetime) | `Infrastructure/AppComposition.cs` — one composition per `Program.Main` call, i.e. per session; `CreateSessionContext` copies `Application.SessionId`, `Application.Theme.Name`, `Application.ActiveProfile.Name` in; also parked in `Application.Session` |
| 4 | Injected into Forms and services instead of static fields | `Views/SessionDiagnostics(SessionContext, …)`, `Services/SessionService(SessionContext, …)`, `Services/TicketService(SessionContext, …)`, `Diagnostics/DiagnosticsService(…, SessionContext, …)` |
| 5 | Diagnostics page separating global settings from per-session values | `Views/SessionDiagnostics` (blue *Application* panel / green *This session* panel), `Diagnostics/DiagnosticsService.cs`, `Diagnostics/DiagnosticsSnapshot.cs`, `Diagnostics/IRuntimeInfo.cs` + `Infrastructure/WisejRuntimeInfo.cs` |
| 6 | Shows session id, user, tenant, theme, client profile | the green panel rows (plus *Selected ticket*, *Session started*, *Same instance in Application.Session?*) |
| 7 | Static-state audit checklist applied | [`docs/StaticStateAudit.md`](TicketOps/docs/StaticStateAudit.md) |
| 8 | Unsafe per-user state moved into the session-scoped service; shared state read-only or thread-safe | `SessionContext` + `SessionService`; `Infrastructure/AppSettings.cs` (immutable, read once by `ProcessScope`), `Data/SharedTicketStore.cs` (lock), `Infrastructure/SharedCounters.cs` (Interlocked) |
| 9 | Production-readiness note; two sessions do not leak | [`docs/ProductionReadinessNote.md`](TicketOps/docs/ProductionReadinessNote.md); the two-tab test above |
| — | Configuration as a deployable contract | [`docs/ConfigurationContract.md`](TicketOps/docs/ConfigurationContract.md), `appsettings.json`, `Default.json` |

## Where things live

```
TicketOps/
├─ Views/
│  ├─ SessionDiagnostics.cs          the screen: injected SessionContext; thin handlers; ShowResult / ReportFailure
│  └─ SessionDiagnostics.Designer.cs GENERATED-style layout (two panels, selectors, grid, bottom bar) — no logic
├─ Controls/StatusBanner             reusable "● state" + banner UserControl (display only)
├─ Services/
│  ├─ SessionContext.cs              ONE PER SESSION: session id, user, tenant, theme, client profile, selected ticket
│  ├─ ISessionService / SessionService.cs   sign-in, tenant switch (membership rule), theme choice, ticket selection
│  └─ ITicketService / TicketService.cs     tenant-filtered list; CreateForCurrentUser stamps tenant + author from the context
├─ Domain/
│  ├─ UserAccount.cs                 identity + tenant membership (the user scope), IsMemberOf rule
│  ├─ Ticket.cs                      record with Tenant and Author
│  └─ OperationResult.cs             success / safe explanation handed back to the screen
├─ Data/
│  ├─ SharedTicketStore.cs           PROCESS-WIDE, thread-safe ticket store (stands in for the database)
│  ├─ InMemoryTicketRepository.cs    per-session door to the shared store (logs into this session's trace)
│  └─ IUserDirectory / InMemoryUserDirectory.cs  fake user store; SimulateOutage throws like an LDAP client would
├─ Infrastructure/
│  ├─ AppComposition.cs              who gets what: one object graph per session; where SessionContext is born
│  ├─ ProcessScope.cs                the "once per process" moment: Lazy<AppSettings>
│  ├─ AppSettings.cs                 appsettings.json → immutable object (the deployable contract)
│  ├─ SharedCounters.cs              the one mutable shared value (Interlocked)
│  ├─ WisejRuntimeInfo.cs            IRuntimeInfo over Application.* (SessionCount, Configuration, Theme, ActiveProfile, Session)
│  ├─ ThemeCatalog.cs                builds a per-session ClientTheme from the framework's embedded theme JSON
│  └─ ILog.cs / ActivityLog.cs       cross-cutting logging (one log per session)
├─ Diagnostics/
│  ├─ DiagnosticsService.cs          builds the two panels: application rows vs session rows
│  ├─ DiagnosticsSnapshot.cs, IDiagnosticsService.cs, IRuntimeInfo.cs
│  ├─ StaticLeakProbe.cs             ⚠ the bug, quarantined as a probe for the two-tab test
│  └─ ActivityTracePanel             the live trace card
├─ Resources/Strings.cs              safe user-facing messages
├─ docs/                             StaticStateAudit.md · ProductionReadinessNote.md · ConfigurationContract.md
├─ appsettings.json                  the application-wide settings (read once per process)
├─ Default.json                      Wisej.NET config: startup, theme, sessionTimeout (1200 s)
├─ Program.cs                        Wisej.NET session entry point → AppComposition (once per session)
└─ Startup.cs                        Kestrel host (app.UseWisej()) — once per process
```

## Self-check answers (lesson guide)

- **Which fields would be unsafe if static?**
  Anything that differs per connected operator: the current user, the tenant, the selected ticket id, the
  theme they switched to, a filter, a pending upload. In this app they are all properties of
  `SessionContext`. The ⚠ probe shows what happens otherwise: press it in one tab and the other tab reads
  your user. Safe statics are the ones left in the project: immutable `AppSettings`, the lock-protected
  `SharedTicketStore`, the `Interlocked` counter, pure functions and constants.
- **What should happen when a browser refreshes?**
  The session survives: Wisej.NET reconnects the same session (same *Session id*), so the `SessionContext`,
  the trace and the services are kept and `Application.ApplicationRefresh` fires — nothing is recomposed.
  A new tab, on the other hand, is a new session with a fresh composition. Durable data (the stamped
  tickets) survives both because it lives in the repository, not in the session.
- **Where does authentication end and session state begin?**
  Authentication answers *who is this person* (the user scope — `UserAccount`, in production
  `Application.User`); it can span many sessions. Session state begins when that identity is attached to
  one running instance: `SessionService.SignInAsync` copies the user's name into `SessionContext.CurrentUser`
  and picks a tenant they belong to. From then on the app reads the session, not the identity provider.
- **Can you name, for each field, whether it is application, session or request scope?**
  Yes — the diagnostics page is that list: the blue panel is application scope, the green panel is session
  scope, and the handler locals (`user`, `tenant`, `theme`, `result` in `buttonApply_Click`) are request scope.
- **Can you point to the one place per-session state lives?**
  `Services/SessionContext.cs`, created in `AppComposition` — and nowhere else.
- **Which startup code runs once per process versus once per session?**
  `Startup.cs` and `ProcessScope.LoadSettings` once per process (`[INFRA] AppSettings loaded now` appears
  in the first tab only); `Program.Main` → `AppComposition` once per session (`[INFRA] composing the session
  object graph` appears in every tab).
