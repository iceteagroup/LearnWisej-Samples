# TicketOps · Production Architecture Deep Dive · Module 11

Local lab build for **Module 11 · Security, Authentication, Authorization & Safe Server Boundaries**. It
follows the walkthrough video: the TicketOps Console starts **closed** behind a login gate; a signed-in
identity is bound to the server session; `TicketService` checks the permission **itself** before closing or
deleting a work order, so hiding a button is never the only guard; user-typed notes are rendered through a
safe text/HTML policy; and every sensitive action — allowed or denied — lands in an append-only audit trail.
Two bottom-bar buttons prove the boundary: *Call DeleteAsync directly (button hidden)* and *Force-enable
Delete (DevTools)* both end in an audited denial for a Technician.

Nothing here is deployed anywhere and nothing is a real credential: three demo accounts in an in-memory
store, password `demo` for all, compared on the server. Production swaps that for an identity provider.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Production Architecture Deep Dive\Module 11\TicketOps"
dotnet run -f net10.0 --urls http://localhost:5111
```

Then open <http://localhost:5111>. (Visual Studio: open `TicketOps.slnx`, press F5 — the port is in
`Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.
`dotnet build -nologo -v q` passes with no warnings for both targets (`net10.0-windows`, `net10.0`).

## Demo accounts (lab only)

| User | Role | May |
|---|---|---|
| `l.romero` | Technician | view work orders, add notes — **Delete is hidden** |
| `m.weber` | Supervisor | + close, delete |
| `s.okafor` | Admin | everything |

Password for all three: `demo` (the *Use …* buttons on the sign-in screen fill it in).

## What to click — Sign in window (the login gate)

The left card is the gate; the right card is the **Activity trace · UI → Service → Data · [SESSION] · [AUDIT]**.

| Button | Path | What you should see |
|---|---|---|
| **Use m.weber · Supervisor** then **Sign in** (or Enter) | success | `[UI] LoginView.buttonSignIn_Click → IAuthenticationService.SignInAsync("m.weber", password: not logged)`, `[SVC] → IUserStore.FindAsync`, `[DATA] "m.weber" found (roles: Supervisor)`, `[SVC] password compared on the server (fixed-time) → match`, `[SESSION] UserSession.SignIn — identity bound to this session: m.weber [Supervisor]`, `[SESSION] WisejSessionBinding — Application.User ← m.weber · Application.IsAuthenticated = True`, `[INFRA] AuditLog — [AUDIT] ✓ SignIn session — m.weber (Supervisor)`; the **Work Orders** window opens |
| **Wrong password** | failure 1 | `l.romero` with `guess-1234`: `[DATA] "l.romero" found`, `[SVC] ⚠ rejected: wrong password → the user sees the same neutral message either way`, `[AUDIT] ⛔ DENIED SignIn l.romero — (anonymous): wrong password`; orange banner **The user name or password is incorrect.**; no identity bound |
| **Unknown user** | failure 2 | `j.doe`: `[DATA] not found`, `[SVC] ⚠ rejected: unknown user`; the **same** banner text — the UI never says which half was wrong |
| **Simulate user store outage** | error path | `✖ [DATA] InMemoryUserStore — outage: LDAP lookup … dc01.ticketops.local:636 unreachable` stays in the trace; the user sees the red banner **Sign-in is temporarily unavailable…** and a toast; status **● unavailable** |
| **Recover the user store** (same button) | recovery | the directory answers again, `l.romero` signs in and Work Orders opens |
| **Clear trace** | — | empties the right-hand card (the audit trail is append-only and untouched) |

The *Server-side identity* box shows `IUserSession.User`, `Application.IsAuthenticated`, `Application.User`,
`Application.IsSecure` and a 6-character session prefix — what the server knows, none of it held by the browser.

## What to click — Work Orders window

Sign in as **l.romero (Technician)** first: Delete is hidden, Close is disabled. Then repeat as **m.weber**.

| Button | Path | What you should see |
|---|---|---|
| (load) | — | `[UI] WorkOrdersView.Load — screen shown for l.romero [Technician] → IPermissionService.PermissionsOf decides what to SHOW`, `[SVC] PermissionService.Can — l.romero [Technician] CloseTicket → denied…`, `[UI] ApplyPermissions — buttonDelete.Visible = False … display only, TicketService re-checks`, then `GetTicketsAsync` → `[DATA] 6 rows`; chip **Signed in: L. Romero · Technician · session …** |
| select **#2002**, type `Pump failed <b>again</b> <img src=x onerror=alert(1)>`, **Render ticket note** | success + safe text | `[SVC] TicketService.AddNote — #2002 → IPermissionService.Can(l.romero, AddNote)` → allowed, `[SVC] AddNoteAsync — #2002 note (53 chars, contains markup — stored as text): Pump failed &lt;b&gt;…` (encoded), `[DATA] #2002 written`, `[AUDIT] ✓ AddNote #2002 — l.romero (Technician): 53 chars`, `[UI] RenderNote — note contains markup → rendered as text (AllowHtml = false) and through the allow-list…`; the left box shows the markup **literally**, the right box shows **again** in bold and the `<img …>` as text; **no alert** |
| **Call DeleteAsync directly (button hidden)** as Technician | failure (permission) | `[UI] calling ITicketService.DeleteAsync(#2002) directly — buttonDelete.Visible = False, irrelevant to the service`, `[SVC] TicketService.DeleteTicket — #2002 → IPermissionService.Can(l.romero, DeleteTicket)`, `[SVC] ⚠ PermissionService.Can — … DeleteTicket → denied (needs Supervisor or Admin)`, `[INFRA] ⚠ AuditLog — [AUDIT] ⛔ DENIED DeleteTicket #2002 — l.romero (Technician): needs Supervisor or Admin`, `[SVC] ⚠ denied before any read or write → throw UnauthorizedAccessException`, `[UI] ⚠ UnauthorizedAccessException from the service — denied where it executes; user sees Strings.AccessDenied`; red banner **You are not allowed to perform this action.**, toast, status **● access denied**; #2002 still in the grid; the audit list gains the ⛔ line |
| **Force-enable Delete (DevTools)** then **Delete ticket** as Technician | failure (permission) | `⚠ [CLIENT] simulating a client that re-enables the hidden Delete button…`; the button appears; clicking it: `[UI] buttonDelete_Click → ITicketService.DeleteAsync(#2002) — no permission check here; TicketService authorizes` and the same denial as above — *the UI obeys, the server won't* |
| **Close ticket** as Technician (button is disabled — use Supervisor) | rule | as m.weber on #2002: `[DOMAIN] Ticket.Close — #2002 status → Closed by m.weber`, `[AUDIT] ✓ CloseTicket #2002 — m.weber (Supervisor)`; on #2006 (already closed): `[DOMAIN] ⚠ Ticket.CanClose — #2006 rejected: The ticket is already closed.` — a domain rule is a *result*, a permission denial is an *exception* |
| **Call DeleteAsync directly** as **m.weber** | success | `[SVC] PermissionService.Can — m.weber [Supervisor] DeleteTicket → allowed`, `[DATA] #2002 deleted (5 rows left)`, `[AUDIT] ✓ DeleteTicket #2002 — m.weber (Supervisor)`; status **● Ticket #2002 deleted.** — the decision is the role's, not the button's |
| **▶ Add 20 notes** | progress | a `Timer` adds 5 notes per tick through the same `AddNoteAsync`; progress bar + **● adding n/20**; 20 `[AUDIT] ✓ AddNote` lines; the Note column fills |
| **Simulate data outage** | error path | `✖ [DATA] … timeout connecting to sql01:1433 …` stays in the trace; the user sees **The action could not be completed. Check the log for details.**; status **● failed** |
| **Recover the data store** (same button) | recovery | the grid reloads, status **● ready** |
| **Sign out** | — | `[SESSION] UserSession.SignOut — identity cleared (l.romero)`, `[SESSION] Application.User ← (anonymous) · Application.IsAuthenticated = False`, `[AUDIT] ✓ SignOut session — l.romero`; back to the login gate |
| **Clear trace** | — | empties the right-hand card |

## Deliverables (lab guide)

| # | Deliverable | Where |
|---|---|---|
| 1 | Login gate | `Views/LoginView.cs` + `.Designer.cs`; `Security/IAuthenticationService.cs` + `AuthenticationService.cs`; `Data/IUserStore.cs` + `InMemoryUserStore.cs`; `Security/IUserSession.cs` + `UserSession.cs`; `Security/IUserContext.cs` + `UserContext.cs` (also an `IPrincipal`); `Infrastructure/WisejSessionBinding.cs` (mirrors into `Application.User`) |
| 2 | Permission service | `Security/Permission.cs`, `Security/IPermissionService.cs` + `PermissionService.cs` — see [`docs/PermissionMatrix.md`](TicketOps/docs/PermissionMatrix.md) |
| 3 | Service-level authorization checks | `Services/TicketService.cs` (`Authorize` before every read/write; denial → audited + `UnauthorizedAccessException`); proof buttons in `Views/WorkOrdersView.cs` (`buttonBypass_Click`, `buttonForceEnable_Click`) |
| 4 | Safe text/HTML policy | `Security/HtmlPolicy.cs` (encode + `{b, i, br}` allow-list); `WorkOrdersView.RenderNote`; the one reviewed `AllowHtml = true` in `WorkOrdersView.Designer.cs` — see [`docs/SafeHtmlPolicy.md`](TicketOps/docs/SafeHtmlPolicy.md) |
| 5 | Audit log | `Services/IAuditService.cs` + `Infrastructure/AuditLog.cs` (`[AUDIT]` lines in the trace, in-app **Audit trail** list) |
| 6 | Security checklist (applied) | [`docs/SecurityChecklist.md`](TicketOps/docs/SecurityChecklist.md) |
| 7 | Threat notes | [`docs/ThreatNotes.md`](TicketOps/docs/ThreatNotes.md) — forged events, hidden-button bypass, HTML injection, session fixation |
| 8 | Every path visible without leaking internals | `ReportFailure` / `ShowResult` in both views, `Resources/Strings.cs` (`AccessDenied`, `SignInFailed`, `SignInUnavailable`) |
| 9 | Production-readiness note | [`docs/ProductionReadinessNote.md`](TicketOps/docs/ProductionReadinessNote.md) |

## Where things live

```
TicketOps/
├─ Views/
│  ├─ LoginView.cs / .Designer.cs        the login gate: read the form, SignInAsync, open Work Orders on success
│  └─ WorkOrdersView.cs / .Designer.cs   grid, note editor + two safe renderings, Close/Delete (shown by permission), audit list
├─ Security/
│  ├─ Permission.cs                      Permission enum + Roles constants
│  ├─ IUserContext.cs / UserContext.cs   the identity (immutable; also IPrincipal/IIdentity for Application.User)
│  ├─ IUserSession.cs / UserSession.cs   per-session holder of the identity — the only source services read
│  ├─ IPermissionService.cs / PermissionService.cs   role → permission table; Can(user, permission)
│  ├─ IAuthenticationService.cs / AuthenticationService.cs   server-side credential check, binds the session, audits
│  └─ HtmlPolicy.cs                      Encode, RenderWithAllowList, AllowedTags
├─ Services/
│  ├─ ITicketService.cs / TicketService.cs   authorizes ITSELF before every action; denial → audit + throw
│  └─ IAuditService.cs                   AuditEntry / AuditOutcome / IAuditService
├─ Domain/
│  ├─ Ticket.cs                          work order + CanClose/Close (no permission logic here)
│  └─ OperationResult.cs
├─ Data/
│  ├─ IUserStore.cs / InMemoryUserStore.cs         3 demo accounts, SimulateOutage (LDAP-style message stays in the log)
│  └─ ITicketRepository.cs / InMemoryTicketRepository.cs   never checks permissions — the service already did
├─ Infrastructure/
│  ├─ AuditLog.cs                        IAuditService: append-only list + "[AUDIT]" trace lines
│  ├─ WisejSessionBinding.cs             the one place that touches Application.User / IsAuthenticated / IsSecure / SessionId
│  ├─ ILog.cs / ActivityLog.cs           cross-cutting logging
│  └─ AppComposition.cs                  one object graph per session; CreateMainView() = the login gate
├─ Resources/Strings.cs                  safe user-facing messages (AccessDenied, SignInFailed, …)
├─ Controls/StatusBanner, Diagnostics/ActivityTracePanel   shared UI pieces
├─ docs/                                 SecurityChecklist, ThreatNotes, SafeHtmlPolicy, PermissionMatrix, ProductionReadinessNote
├─ Program.cs                            session entry point → AppComposition.CreateMainView().Show()
└─ Startup.cs                            Kestrel host (app.UseWisej())
```

## Self-check answers (lesson guide)

- **Where is authorization actually enforced — in a service, or only by a hidden button?**
  In `TicketService.Authorize`, called first by every method that reads or changes a work order, using the
  identity from `IUserSession`. The hidden Delete button is a courtesy: *Call DeleteAsync directly* and
  *Force-enable Delete* both reach the service and both are refused for a Technician.
- **Which controls or messages allow HTML, and is user content sanitized before it reaches them?**
  One label (`labelNoteAllowList`) has `AllowHtml = true`, commented as reviewed; it only receives
  `HtmlPolicy.RenderWithAllowList(note)`. The trace and audit `ListBox`es are HTML-capable, so user text
  is encoded before it is logged and never enters the audit trail. Everything else is plain text.
- **If someone called a sensitive service method directly, would they be stopped?**
  Yes: `DeleteAsync` / `CloseAsync` throw `UnauthorizedAccessException` before touching the repository and
  write `[AUDIT] ⛔ DENIED …` first — the *Call DeleteAsync directly* button is exactly that call.
- **What sensitive data could accidentally end up in a log or an error message shown to the user?**
  None by design: the password is logged as a length only, sign-in failure is one neutral sentence, notes
  are audited by length and logged encoded, and every exception is mapped to a `Strings.*` sentence
  while `sql01:1433` / `dc01.ticketops.local:636` stay in the trace.
