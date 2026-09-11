# Security checklist — applied to the TicketOps Console (Module 11)

*Module 11 deliverable · sessions, cookies, CSP, logs, uploads, disabled controls, AllowHtml.*
Legend: ✔ implemented in this sample · ☐ configuration item documented for deployment (not code) · — not applicable to this lab.

## A. Trust boundary

| # | Check | State | Evidence in this sample |
|---|---|---|---|
| A1 | Every sensitive action re-checks authorization **inside the service** that performs it, never only in the UI | ✔ | `Services/TicketService.cs` → private `Authorize(permission, action, target)` runs first in `GetTicketsAsync`, `AddNoteAsync`, `CloseAsync`, `DeleteAsync`, before any repository call |
| A2 | Identity is read from the **server session**, never from a client-supplied value | ✔ | `TicketService` reads `IUserSession.User`; no method takes a user, role or id from the screen. `UserSession.SignIn` is called only by `AuthenticationService` |
| A3 | A disabled control is treated as a UX hint, and the sample proves it | ✔ | `WorkOrdersView.ApplyPermissionsToControls` disables Delete for a Technician; **Force-enable Delete** then **Delete** ends in `UnauthorizedAccessException` from the service and `[AUDIT] ⛔ DENIED DeleteTicket #2002` |
| A4 | Denial is **hard** (thrown, not a soft result) so it cannot be mistaken for success | ✔ | `UnauthorizedAccessException` thrown by `TicketService.Authorize`; `WorkOrdersView.ReportFailure` maps it to `Strings.AccessDenied` |
| A5 | Authentication and authorization are separate steps | ✔ | `Security/AuthenticationService` (once, at the gate) vs `Security/PermissionService` (per action) |

## B. Sessions & cookies

| # | Check | State | Evidence / note |
|---|---|---|---|
| B1 | The console starts closed: no screen before sign-in | ✔ | `AppComposition.CreateMainView()` returns `LoginView`; `WorkOrdersView` is created only from `LoginView.buttonSignIn_Click` after `result.Succeeded` |
| B2 | Identity bound to the server session, mirrored into the host | ✔ | `UserSession` (per session, created in `AppComposition`) + `Infrastructure/WisejSessionBinding` sets `Application.User` |
| B3 | No per-user state in a static | ✔ | `grep -rn "static" TicketOps/*.cs` → constants, `PermissionService.Grants` (constant table), `HtmlPolicy`, `SeedData`, `WisejSessionBinding` (stateless adapter). Nothing holds a user |
| B4 | Session cookie `HttpOnly` + `Secure`; TLS end to end (or terminated at a trusted proxy) | ☐ | Deployment: set on the host (`Web.config` / reverse proxy); check `Application.IsSecure` on the host |
| B5 | Idle sessions expire; sign-out clears the identity | ✔/☐ | `AuthenticationService.SignOut` → `UserSession.SignOut` → `Application.User = null`, audited. Timeout: `Default.json` `sessionTimeout` — document the chosen value (see `ProductionReadinessNote.md`) |
| B6 | New session id after login (session fixation) | ☐ | See `ThreatNotes.md` §4: the Wisej.NET session id is issued by the server and is not read from the URL; document the reverse-proxy cookie settings |

## C. Safe text / HTML

| # | Check | State | Evidence |
|---|---|---|---|
| C1 | `AllowHtml` stays `false` for anything a user can type | ✔ | `labelNotePlain.AllowHtml = false` (stated in code and Designer); `labelSignedIn`, `labelSelected` are defaults (false) |
| C2 | Every `AllowHtml = true` is a reviewed, commented decision | ✔ | Exactly one: `labelNoteAllowList` in `WorkOrdersView.Designer.cs` with a REVIEWED comment; it only ever receives `HtmlPolicy.RenderWithAllowList(note)` |
| C3 | Encode on output for every HTML-capable surface (labels, tooltips) | ✔ | `Security/HtmlPolicy.Encode`; a rejected user name is encoded before it is audited; `listAudit` receives `AuditLog.Format` output only (no user text) |
| C4 | The allow-list is documented and tiny | ✔ | `HtmlPolicy.AllowedTags = { b, i, br }` — see `SafeHtmlPolicy.md` |
| C5 | Content Security Policy header as a second layer | ☐ | Deployment (`ProductionReadinessNote.md`) |

## D. Errors & logs

| # | Check | State | Evidence |
|---|---|---|---|
| D1 | Users never see exception text, SQL, host names or stack traces | ✔ | `ReportFailure` in both views shows `Strings.ActionFailed` / `Strings.AccessDenied` / `Strings.SignInUnavailable`; exception details go to the server log only |
| D2 | Sign-in failure is one neutral message (no "user not found" vs "wrong password" leak) | ✔ | `AuthenticationService.SignInAsync` → `Strings.SignInFailed` for both; the distinction goes to the log and the audit trail only |
| D3 | No secrets in logs or audit: passwords, tokens, payloads | ✔ | the password is never logged; `AddNoteAsync` audits `N chars`, never the note body; `AuditEntry.Detail` is a short phrase |
| D4 | Audit trail: who / what / target / when / outcome, including denials, append-only | ✔ | `Services/IAuditService.cs` + `Infrastructure/AuditLog.cs` (no Remove/Clear); `[AUDIT]` lines in the server log; the in-app **Audit trail** list |

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
