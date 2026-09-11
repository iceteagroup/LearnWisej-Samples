# Production-readiness note — Module 11 (security)

*What this sample proves, what it fakes, and what must be configured before the TicketOps Console faces real users.*

## Proven in code (keep as is)

- Authorization is enforced **inside `TicketService`**, per action, from the session identity; the UI only
  disables. A force-enabled Delete still ends in an audited denial. (`docs/PermissionMatrix.md`, `docs/ThreatNotes.md` §1–2)
- Authentication is a separate step at a **login gate**; the console has no screen before it. (`Views/LoginView`)
- User text is rendered as text by default; the one `AllowHtml = true` surface is allow-listed and commented. (`docs/SafeHtmlPolicy.md`)
- Errors and denials show `Strings.*` only; internals stay in the log. Sign-in failure is one neutral message.
- Audit trail: who / what / target / when / outcome, including denials, append-only, no secrets. (`Infrastructure/AuditLog.cs`)

## Faked for the lab (replace before production)

| Fake | Replace with | What else changes |
|---|---|---|
| `Data/InMemoryUserStore` (three accounts, password `secret123`) + `Security/AuthenticationService` (fixed-time compare of a constant) | An identity provider: OpenID Connect (Entra ID, Keycloak, …) or Windows authentication on the host; roles/groups from its claims | `AuthenticationService` becomes an adapter that turns claims into `UserContext`; **nothing else** — screens and services depend on `IUserSession`, `IUserContext`, `IPermissionService` only |
| `Infrastructure/AuditLog` (in-memory list + log lines) | A durable, append-only sink: audit table with insert-only rights, or a SIEM forwarder | Same `IAuditService`; add retention and access rules for the audit data |
| `Data/InMemoryTicketRepository` | The real repository (Module 4/12) | none — the repository never checks permissions; the service already did |
| Audit list shown to every role | Gate it with `Permission.ViewAuditTrail` (already in the matrix) | `WorkOrdersView.ApplyPermissionsToControls` hides the list; a dedicated audit screen for Supervisors/Admins |

## Configure on deployment (documented, not code)

| Item | Setting | Why |
|---|---|---|
| Transport | HTTPS everywhere; WebSocket over WSS; HSTS | `Application.IsSecure` must be `True` on the host |
| Session cookie | `HttpOnly`, `Secure`, `SameSite=Lax` (or `Strict`), set by the host/reverse proxy | blunts theft via XSS and cross-site replay (`ThreatNotes.md` §4) |
| Session timeout | `Default.json` → `sessionTimeout` (seconds) sized for the desk workflow (e.g. 20 min); handle `Application.SessionTimeout` to warn | idle sessions expire; the identity dies with the session |
| Content Security Policy | `Content-Security-Policy: default-src 'self'; script-src 'self' …; object-src 'none'; frame-ancestors 'none'` — test against the Wisej.NET client bundle first (it uses inline styles; start with `report-only`) | second layer against injected script |
| Reverse proxy | Forward `X-Forwarded-Proto` / `-For`; terminate TLS at the proxy only if the hop to Kestrel is on a trusted network; keep WebSocket upgrade enabled | so `IsSecure`, origin and client address are trustworthy |
| Secrets | No connection strings or keys in `Default.json`, `Default.html` or anything under the web root; use environment / a secret store | anything shipped to the browser can be read |
| Dependency inventory | Record `Wisej-4 4.1.0` and every other package in the release notes; subscribe to advisories | find known-vulnerable versions quickly |
| Logs | Ship `ILog` output to the central log with the same "no secrets" rule; separate audit from diagnostics | audit must be reviewable without exposing internals |
| Uploads (capstone) | Validate type/size/content on the server, store outside the web root, generate the stored file name | rule recorded in `SecurityChecklist.md` E2 |

## Wisej.NET APIs used for security in this module (verify on the deployment host)

`Application.User` (get/set `IPrincipal`), `Application.IsAuthenticated`, `Application.IsSecure`,
`Label.AllowHtml` (default `false`), `TextBox.PasswordChar`, `Form.AcceptButton`.

## Sign-off questions (from the lesson)

1. Where is authorization actually enforced? → `TicketService.Authorize`, on the server, per action.
2. Which controls allow HTML, and is user content sanitized before them? → one label, allow-listed; everything else text.
3. If someone called a sensitive service method directly, would they be stopped? → yes, and audited (force-enable Delete as a Technician).
4. What sensitive data could end up in a log or a user message? → none: passwords and note bodies are never logged or audited, users see `Strings.*`.
