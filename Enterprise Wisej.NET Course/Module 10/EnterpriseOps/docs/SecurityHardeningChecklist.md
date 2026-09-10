# Security hardening checklist — EnterpriseOps

Deliverable 5 of Module 10. The whole surface of a Wisej.NET application, turned into review items that are
checked on every release rather than remembered occasionally.

The list also exists as code — `Security/HardeningChecklist.cs` — so the running application can count it and
the two cannot drift apart. Treat it as living code: add an item every time a review finds a gap, and record
who checked each item for each release in the table at the bottom.

State: **done** proven in this sample · **sample** demonstrated but only meaningful once hosted for real ·
**deployment** owned by the release runbook (Module 12).

## HTML

| # | Item | State | Evidence |
|---|---|:--:|---|
| H1 | Every control with `AllowHtml = true` is inventoried and its text source is known | done | **AllowHtml review** button — reflection over the live control tree, not a list someone typed |
| H2 | Untrusted text is escaped, or sanitized against an allow-list, before it reaches an HTML surface | done | `Security/HtmlText.cs`; the decision is taken in `NoteRenderService`, not in the screen |
| H3 | Grid columns, tooltips, toasts and list items count as HTML surfaces too | done | `SecurityReviewService` inspects `DataGridViewColumn` as well as controls |
| H4 | Sanitizing happens at render time, never at storage time | done | `InMemoryNoteStore` stores the bytes that arrived; encoding is chosen per surface |

### The AllowHtml review

In Wisej.NET an `AllowHtml` property exists on far more types than most teams expect: `Label`, `ButtonBase`,
`ListBox`, `ComboBox`, `GroupBox`, `TabPage`, `TreeNode`, `MenuItem`, `ToolTip`, `Toast`, `ListViewItem`,
`DataGridViewColumn` and `DataGridViewCell`, among others. That is why the review is automated rather than
written down: `SecurityReviewService.Review` walks the running screen, finds every surface with a public
`bool AllowHtml`, and pairs it with a declared text source.

Reflection cannot know where a surface's text comes from, so that judgement is declared once in
`SecurityReviewService.TextSources` and reviewed like code. Anything not declared is reported as
"constant text written by the developer" — so a control added later still appears in the report for judgement.

Surfaces on this screen whose text is **not** author-written:

| Surface | Text source | Protection |
|---|---|---|
| `lblNote` | a customer note from a public portal | `AllowHtml = false` by default; the allow-list sanitizer when formatting is wanted |
| `lblNoteSource` | note author and origin | `AllowHtml = false` |
| `lstTrace` | service text quoting provider group names and user ids | `AllowHtml = false` |
| `colDetail`, `colUser` | audit detail and user ids | `AllowHtml = false` |
| `lblUser`, `lblTenant` | the `name` and `tid` claims | `AllowHtml = false` |
| `lblBanner` | service messages quoting permission names | `AllowHtml = false` |

The sanitizer in `HtmlText.Sanitize` is an allow-list of `b, i, em, strong, br` with **all attributes dropped**,
so an `onerror`, a `style` or a `javascript:` href cannot survive it. It is small enough to read in a lab; in
production use a maintained library (HtmlSanitizer / AngleSharp) instead of a regular expression.

## Uploads

| # | Item | State | Evidence |
|---|---|:--:|---|
| U1 | Size limit enforced server-side, before the file is read | deployment | no upload surface in this module; the item stays so the next screen inherits it |
| U2 | Content-type **and** extension checked against an allow-list | deployment | — |
| U3 | The server chooses the stored name; the original name is never used as a path | deployment | — |
| U4 | Files are stored outside the web root and never served back by original name | deployment | Wisej's static file server serves the project folder — anything written there is public |

## Cookies

| # | Item | State | Evidence |
|---|---|:--:|---|
| C1 | Session cookie is `Secure` | deployment | set on the host; verified per environment |
| C2 | Session cookie is `HttpOnly` | deployment | — |
| C3 | `SameSite=Lax` (or `Strict` where no cross-site POST is needed) | deployment | — |
| C4 | Session is invalidated server-side on sign-out, not only cleared client-side | done (pattern) | `SessionContext.SignOut` drops the identity; every later `BeginCommand` throws |

## Response headers

| # | Item | State | Evidence |
|---|---|:--:|---|
| S1 | `Content-Security-Policy` — and it must still allow Wisej's own `*.wx` endpoints and inline theme styles | deployment | Module 12's `Startup.cs`; test the app after tightening, a broken CSP breaks the framework |
| S2 | `Strict-Transport-Security` once HTTPS is terminated for the domain | deployment | — |
| S3 | `X-Content-Type-Options: nosniff` | deployment | — |
| S4 | `Referrer-Policy: strict-origin-when-cross-origin` | deployment | — |

## Logs

| # | Item | State | Evidence |
|---|---|:--:|---|
| L1 | No password, token, cookie, claim value or full personal record in any log line | done | `ActivityTrace` writes subject ids, tenants and permission names only; `NoteRenderService.Preview` truncates payloads |
| L2 | Every sensitive command writes an audit entry with a correlation id, including denials | done | `PermissionService.Demand` + `AuditLog` — visible in `dgvAudit` |
| L3 | Audit storage is durable, append-only and tamper-evident (hash chain or signed sequence) | deployment | the in-memory log is none of these — `AuditLogScreen.md` says what is missing |
| L4 | Repeated denials from one subject raise an alert | deployment | filter **Result: DENIED** shows the data the alert would watch |

## Secrets

| # | Item | State | Evidence |
|---|---|:--:|---|
| X1 | Connection strings, keys and client secrets come from protected configuration or a vault | sample | this sample has no secret at all — the simulated provider needs none, which is the point |
| X2 | Nothing secret is in `Default.json` or any file the static file server can reach | done | `Startup.cs` refuses to serve `*.json`; `Default.json` holds theme and startup only |
| X3 | No secret in source control, and rotation is possible without a rebuild | deployment | — |

## Identity

| # | Item | State | Evidence |
|---|---|:--:|---|
| I1 | The gate sits in front of every entry point, not only the main page | done | `SessionContext.BeginCommand` throws when unauthenticated, so no service can be called without it |
| I2 | Services read identity from the session context — never from a control, query string or hidden field | done | every service method takes a `CommandContext` and nothing else identity-shaped |
| I3 | Authorization is enforced in services, not in the UI | done | **Break the UI: enable Export** proves it |
| I4 | A long-running session can reload roles without a new login | done | `SessionContext.RefreshPermissions` |
| I5 | An unknown directory group grants nothing and is reported | done | sign in as `t.novak` |
| I6 | Tenant isolation is checked before any role lookup | done | `TenantGuard`, and **Fail: cross-tenant approve** |

## Reverse proxy

| # | Item | State | Evidence |
|---|---|:--:|---|
| P1 | The proxy forwards `X-Forwarded-Proto` and `X-Forwarded-For`, and the app is configured to trust them | deployment | otherwise every audit entry records the proxy's address and every redirect drops to HTTP |
| P2 | WebSocket upgrade is forwarded — Wisej.NET pushes over it | deployment | a proxy that drops `Upgrade` turns `Application.Update` into silence |
| P3 | Only the proxy can reach the app port | deployment | — |

## Release sign-off

| Release | Date | Checked by | Items failed | Notes |
|---|---|---|---|---|
| *(sample)* | — | — | — | The deployment-owned items are inherited by Module 12's release runbook. |
