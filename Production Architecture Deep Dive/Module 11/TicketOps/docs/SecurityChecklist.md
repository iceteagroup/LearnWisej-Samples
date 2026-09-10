# Security checklist — applied to the TicketOps Console (Module 11)

*Module 11 deliverable · sessions, cookies, CSP, logs, uploads, disabled controls, AllowHtml.*
Legend: ✔ implemented in this sample · ☐ configuration item documented for deployment (not code) · — not applicable to this lab.

## A. Trust boundary

| # | Check | State | Evidence in this sample |
|---|---|---|---|
| A1 | Every sensitive action re-checks authorization **inside the service** that performs it, never only in the UI | ✔ | `Services/TicketService.cs` → private `Authorize(permission, action, target)` runs first in `GetTicketsAsync`, `AddNoteAsync`, `CloseAsync`, `DeleteAsync`, before any repository call |
| A2 | Identity is read from the **server session**, never from a client-supplied value | ✔ | `TicketService` reads `IUserSession.User`; no method takes a user, role or id from the screen. `UserSession.SignIn` is called only by `AuthenticationService` |
| A3 | A hidden/disabled control is treated as a UX hint, and the sample proves it | ✔ | `WorkOrdersView.ApplyPermissionsToControls` hides Delete for a Technician; **Call DeleteAsync directly** and **Force-enable Delete** both end in `[SVC] ⚠ denied before any read or write → throw UnauthorizedAccessException` and `[AUDIT] ⛔ DENIED DeleteTicket #2002` |
| A4 | Denial is **hard** (thrown, not a soft result) so it cannot be mistaken for success | ✔ | `UnauthorizedAccessException` thrown by `TicketService.Authorize`; `WorkOrdersView.ReportFailure` maps it to `Strings.AccessDenied` |
| A5 | Authentication and authorization are separate steps | ✔ | `Security/AuthenticationService` (once, at the gate) vs `Security/PermissionService` (per action) |

## B. Sessions & cookies

| # | Check | State | Evidence / note |
|---|---|---|---|
| B1 | The console starts closed: no screen before sign-in | ✔ | `AppComposition.CreateMainView()` returns `LoginView`; `WorkOrdersView` is created only from `LoginView.SignInAsync` after `result.Succeeded` |
| B2 | Identity bound to the server session, mirrored into the host | ✔ | `UserSession` (per session, created in `AppComposition`) + `Infrastructure/WisejSessionBinding` sets `Application.User`; trace shows `[SESSION] Application.IsAuthenticated = …` |
| B3 | No per-user state in a static | ✔ | `grep -rn "static" TicketOps/*.cs` → constants, `PermissionService.Grants` (constant table), `HtmlPolicy`, `SeedData`, `WisejSessionBinding` (stateless adapter). Nothing holds a user |
| B4 | Session cookie `HttpOnly` + `Secure`; TLS end to end (or terminated at a trusted proxy) | ☐ | Deployment: set on the host (`Web.config` / reverse proxy). `Application.IsSecure` is logged at load so the trace shows whether the session runs on https/wss |
| B5 | Idle sessions expire; sign-out clears the identity | ✔/☐ | `AuthenticationService.SignOut` → `UserSession.SignOut` → `Application.User = null`, audited. Timeout: `Default.json` `sessionTimeout` — document the chosen value (see `ProductionReadinessNote.md`) |
| B6 | New session id after login (session fixation) | ☐ | See `ThreatNotes.md` §4: the Wisej.NET session id is issued by the server and is not read from the URL; document the reverse-proxy cookie settings. The sample logs only a 6-character prefix of the id, never the full value |

## C. Safe text / HTML

| # | Check | State | Evidence |
|---|---|---|---|
| C1 | `AllowHtml` stays `false` for anything a user can type | ✔ | `labelNotePlain.AllowHtml = false` (stated in code and Designer); `labelIdentity`, `labelSignedIn`, `labelSelected` are defaults (false) |
| C2 | Every `AllowHtml = true` is a reviewed, commented decision | ✔ | Exactly one: `labelNoteAllowList` in `WorkOrdersView.Designer.cs` with a REVIEWED comment; it only ever receives `HtmlPolicy.RenderWithAllowList(note)` |
| C3 | Encode on output for every HTML-capable surface (labels, the trace/audit `ListBox`, tooltips) | ✔ | `Security/HtmlPolicy.Encode`; user text is encoded before it is logged (`TicketService.AddNoteAsync`, `AuthenticationService.SignInAsync`); `listAudit` receives `AuditLog.Format` output only (no user text) |
| C4 | The allow-list is documented and tiny | ✔ | `HtmlPolicy.AllowedTags = { b, i, br }` — see `SafeHtmlPolicy.md` |
| C5 | Content Security Policy header as a second layer | ☐ | Deployment (`ProductionReadinessNote.md`) |

## D. Errors & logs

| # | Check | State | Evidence |
|---|---|---|---|
| D1 | Users never see exception text, SQL, host names or stack traces | ✔ | `ReportFailure` in both views shows `Strings.ActionFailed` / `Strings.AccessDenied` / `Strings.SignInUnavailable`; `sql01:1433` and `dc01.ticketops.local:636` appear in the trace only |
| D2 | Sign-in failure is one neutral message (no "user not found" vs "wrong password" leak) | ✔ | `AuthenticationService.SignInAsync` → `Strings.SignInFailed` for both; the distinction goes to the log only |
| D3 | No secrets in logs or audit: passwords, tokens, payloads | ✔ | `SignInAsync` logs `password: N chars, not logged`; `AddNoteAsync` audits `N chars`, never the note body; `AuditEntry.Detail` is a short phrase |
| D4 | Audit trail: who / what / target / when / outcome, including denials, append-only | ✔ | `Services/IAuditService.cs` + `Infrastructure/AuditLog.cs` (no Remove/Clear); `[AUDIT]` lines in the trace; the in-app **Audit trail** list |

## E. Uploads & inputs

| # | Check | State | Evidence / note |
|---|---|---|---|
| E1 | Server-side length/shape validation of user input | ✔ | `TicketService.AddNoteAsync` (`MaxNoteLength = 500`, trimmed); `TextBox.MaxLength` in the Designer is the UI courtesy |
| E2 | File uploads validated server-side (type, size, content), stored outside the web root, sent file name never trusted | — | No upload in this module; rule recorded for the capstone |

## F. Dependencies & deployment

| # | Check | State | Evidence / note |
|---|---|---|---|
| F1 | Dependency inventory | ☐ | `TicketOps.csproj`: `Wisej-4 4.1.0` only; record in the release notes (Module 12) |
| F2 | Reverse-proxy awareness (TLS termination, forwarded headers) | ☐ | `ProductionReadinessNote.md` |
| F3 | Real identity provider replaces `AuthenticationService` + `InMemoryUserStore` | ☐ | The rest of the app depends only on `IUserSession` / `IUserContext` / `IPermissionService` |

## Evidence — what the running app shows

- **Login gate**: the first window is *TicketOps — Sign in*; the identity read-out shows `IUserSession.User = —` and `Application.IsAuthenticated = False`. After **Use m.weber** → **Sign in**: `[SVC] password compared on the server (fixed-time) → match`, `[SESSION] UserSession.SignIn — identity bound…`, `[SESSION] WisejSessionBinding — Application.User ← m.weber · Application.IsAuthenticated = True`, `[INFRA] AuditLog — [AUDIT] ✓ SignIn session — m.weber (Supervisor)`, and the Work Orders window opens.
- **Wrong password / Unknown user**: same orange banner *The user name or password is incorrect.*; the trace distinguishes them (`rejected: wrong password` / `rejected: unknown user`) and the audit trail gets `[AUDIT] ⛔ DENIED SignIn l.romero — (anonymous): wrong password`.
- **Bypass as l.romero (Technician)**: Delete is hidden; **Call DeleteAsync directly** → `[SVC] ⚠ PermissionService.Can — l.romero [Technician] DeleteTicket → denied (needs Supervisor or Admin)`, `[AUDIT] ⛔ DENIED DeleteTicket #2002`, red banner *You are not allowed to perform this action.*; #2002 is still in the grid.
- **Safe text**: a note `Pump failed <b>again</b> <img src=x onerror=alert(1)>` shows verbatim (escaped) in the left box, and as bold "again" + literal `<img …>` text in the allow-list box; no script runs; the trace line carries the note **encoded**.
- **Outages**: `✖ [DATA] … sql01:1433` / `… dc01.ticketops.local:636` in the trace; the user sees the safe sentence only; clicking the same button again recovers.
