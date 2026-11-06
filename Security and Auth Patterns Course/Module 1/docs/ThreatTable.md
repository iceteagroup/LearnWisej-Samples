# AccessOps - threat table (STRIDE)

Every entry point from the inventory, walked through the six STRIDE categories. Likelihood and
impact are Low / Med / High for an internal audience: a curious colleague is likelier and milder
than an outsider; a departing employee taking the document store is plausible and severe.
The last column names the module of this course that delivers the mitigation. A row without one
is written down as an accepted risk.

| # | Category | Entry point | What could happen | Likelihood | Impact | Mitigation | Module |
|---|---|---|---|---|---|---|---|
| T1 | Spoofing | E1 Login page | Someone signs in as the finance manager (guessed or reused password) | Med | High | Salted PBKDF2 hash, lockout after 5 failures, generic message, dummy verification for unknown users | M2 |
| T2 | Spoofing | E1 Login page | Session fixation: an ID planted before login is reused after it | Low | High | `Application.RefreshSessionId()` after verification | M2 |
| T3 | Spoofing | E5 OIDC callback | A forged or replayed ID token / authorization code is accepted | Low | High | Authorization code + PKCE, state and nonce, ID-token validation by the OpenID Connect handler | M4 |
| T4 | Tampering / Elevation | E2 Approve button | A non-manager raises the approve event for their own request | Med | High | `Authorize.Require` in `ApprovalService`, manager scope in the repository query | M3 |
| T5 | Information disclosure | E2 Employee grid | Salary bands ride along in the widget state (hidden column) | High | High | Bind a projection; never put pay data on a control (verified in this module) | M1 |
| T6 | Information disclosure | E2 Documents list | An employee lists another department's documents | Med | High | Department predicate inside the document query | M3 |
| T7 | Tampering | E4 Download | A document id is edited to fetch another one | Med | High | `DocumentService.Download` authorizes the id for this caller before streaming | M5 |
| T8 | Tampering | E5 Share link | Expiry or id edited by hand | Med | High | HMAC over id and expiry, fixed-time comparison, one-hour lifetime | M5 |
| T9 | Information disclosure | E3 Upload | An executable renamed `.pdf` becomes a "document" | Med | Med | Size limit, extension allow-list, content signature, quarantine | M5 |
| T10 | Elevation | E6 Interop / output | User text rendered as markup (`AllowHtml`, `MessageBox.Show` default) or script | Med | High | Text encoded by default; `allowHtml: false`; `Application.Call` with arguments | M5 |
| T11 | Tampering | E2 Search box | SQL injection through a search term | Low | High | Parameterised commands only | M5 |
| T12 | Repudiation | E2 Approvals | Nobody can prove who approved what, when, from where | Med | Med | Append-only, hash-chained audit table with correlation IDs | M6 |
| T13 | Information disclosure | E9 Configuration | Client secret or connection string committed to git, or Default.json downloaded | Med | High | Secrets in user secrets / environment; `.json` excluded from the file server | M1, M6 |
| T14 | Information disclosure | Logs | Passwords, tokens or personal data written to log files | Med | Med | Structured logging by reference, redaction at the sink | M6 |
| T15 | Denial of service | E1 Login | Password guessing floods the login | High | Med | Lockout (M2) and a token bucket in the login service (M7); volumetric floods are the host's | M2, M7 |
| T16 | Denial of service | E3 Upload | Disk-filling uploads | Med | Med | `Upload.MaxFileSize` + host request limit | M5, M7 |
| T17 | Denial of service | Whole app | Volumetric flood, connection exhaustion | Med | High | **Accepted for the application**: the Wisej.NET docs state DoS protection is not a web framework's responsibility. Owned by the host / proxy / gateway. `maxSessionsPerClient` is only secondary - behind a proxy every client can share one address. | Host |
| T18 | Spoofing | E5 / E1 Transport | Credentials or session ID read on the network | Low | High | HTTPS, HSTS, `secure: true` in Default.json | M7 |
| T19 | Elevation | E7 AI assistant | A prompt (or a document it reads) instructs the assistant to fetch another department's data | Med | High | **Gap, accepted for now**: the assistant is not built in this course. Rule recorded: it may only call the same guarded services as the UI, with the caller's own SessionContext. | - |
| T20 | Information disclosure | Session ↔ process | A "current user" kept in a static field leaks between sessions | Med | High | Identity only in `Application.Session` | M2 |

## Accepted risks, in writing

- **T17** belongs to the host. Revisit when the hosting design is written.
- **T19** is an open gap until the AI assistant exists; whoever builds it inherits this row.
