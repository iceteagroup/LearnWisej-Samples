# TicketOps · Production Architecture Deep Dive · Module 11

Local lab build for **Module 11 · Security, Authentication, Authorization & Safe Server Boundaries**. It
follows the walkthrough video: the TicketOps Console starts **closed** behind a login gate; the signed-in
identity is bound to the server session; `TicketService` checks the permission **itself** before closing or
deleting a work order; user-typed notes go through a safe text/HTML policy; and every sensitive action —
allowed or denied — lands in an append-only audit trail.

Nothing here is a real credential: three demo accounts in an in-memory store, compared on the server.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Production Architecture Deep Dive\Module 11\TicketOps"
dotnet run -f net10.0 --urls http://localhost:5111
```

Then open <http://localhost:5111> (or open `TicketOps.slnx` in Visual Studio and press F5).

## Demo accounts (lab only, password `secret123` for all)

| User | Role | May |
|---|---|---|
| `l.romero` | Technician | view work orders, add notes |
| `m.weber` | Supervisor | + close, delete |
| `s.okafor` | Admin | everything |

## The screens

- **Sign in**: user name, password, **Sign in**. A wrong password or an unknown user gets the same
  neutral message.
- **Work Orders**: the signed-in user and **Sign out**, the work-order grid, a note box with
  **Render ticket note** and its two renderings (as text, and with the `b`/`i`/`br` allow-list),
  **Close ticket**, **Delete ticket** (disabled for a Technician), **Force-enable Delete** and the
  **audit trail**.

The lab asks you to show that an unauthorized action cannot run even when its control is enabled manually:
sign in as `l.romero`, select #2002, click **Force-enable Delete**, then **Delete ticket**. The service
refuses, the banner says *You are not allowed to perform this action.*, and the audit trail gains a
`⛔ DENIED DeleteTicket #2002` line. The video does the same from the browser console
(`btnDelete.disabled = false`); the button here is `buttonDelete`.

## Deliverables (lab guide)

| # | Deliverable | Where |
|---|---|---|
| 1 | Login gate | `Views/LoginView`, `Security/AuthenticationService.cs`, `Data/InMemoryUserStore.cs`, `Security/UserSession.cs`, `Infrastructure/WisejSessionBinding.cs` |
| 2 | Permission service | `Security/Permission.cs`, `Security/PermissionService.cs` — [`docs/PermissionMatrix.md`](TicketOps/docs/PermissionMatrix.md) |
| 3 | Service-level authorization checks | `Services/TicketService.cs` (`Authorize` before every read or write; a denial is audited, then thrown) |
| 4 | Safe text/HTML policy | `Security/HtmlPolicy.cs`, `WorkOrdersView.RenderNote`, the one reviewed `AllowHtml = true` in `WorkOrdersView.Designer.cs` — [`docs/SafeHtmlPolicy.md`](TicketOps/docs/SafeHtmlPolicy.md) |
| 5 | Security checklist | [`docs/SecurityChecklist.md`](TicketOps/docs/SecurityChecklist.md) |
| — | Audit log | `Services/IAuditService.cs`, `Infrastructure/AuditLog.cs` |
| — | Threat notes | [`docs/ThreatNotes.md`](TicketOps/docs/ThreatNotes.md) |
| — | Production-readiness note | [`docs/ProductionReadinessNote.md`](TicketOps/docs/ProductionReadinessNote.md) |

## Where things live

```
TicketOps/
├─ Views/            LoginView, WorkOrdersView
├─ Security/         Permission, UserContext, UserSession, PermissionService, AuthenticationService, HtmlPolicy
├─ Services/         ITicketService / TicketService (authorizes itself), IAuditService
├─ Domain/           Ticket (CanClose / Close), OperationResult
├─ Data/             IUserStore / InMemoryUserStore, ITicketRepository / InMemoryTicketRepository
├─ Infrastructure/   AuditLog, WisejSessionBinding, ILog/ActivityLog, AppComposition
├─ Resources/        Strings.cs
└─ docs/             SecurityChecklist, ThreatNotes, SafeHtmlPolicy, PermissionMatrix, ProductionReadinessNote
```
