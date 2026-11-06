# AccessOps - security control placement map

One row per control: the rule, the boundary it sits on, the class that enforces it, and the
evidence a reviewer can be shown. A rule that exists only in the UI is a suggestion; the same rule
inside a service is a control. The status column is updated as the modules land.

| # | Rule | Boundary | Enforcing class | Evidence for a reviewer | Module |
|---|---|---|---|---|---|
| C1 | The directory never sends pay or personal fields | Session → browser | `EmployeeDirectoryService.GetDirectory` (projection) | Browser data model holds 4 values per row (javascript check in README) | M1 ✓ |
| C2 | Configuration files are never served | Browser → process | `app.UseWhen(... !EndsWith(".json") ...)` in Startup.cs | `/Default.json` and `/appsettings.json` return 404 | M1 ✓ |
| C3 | A password is verified only against a salted slow hash, in one place | Browser → session | `PasswordService`, `LoginService.SignIn` | Unit of work: stored row has salt, iterations, hash, no password | M2 |
| C4 | Five failures lock the account; every failure gets the same sentence | Browser → session | `LoginThrottle`, `LoginService` | Six attempts in the audit table, one message on screen | M2 |
| C5 | Identity exists only after verification, only in the session, with a fresh session ID | Session | `SessionContext`, `Application.RefreshSessionId()` | Session ID changes across sign-in | M2 |
| C6 | Idle sessions end on the server | Session | `Application.SessionTimeout` handler → `Application.Exit()` | Idle tab lands on the login page; audit row "Idle" | M2 |
| C7 | Every service method checks a permission first | Session → process | `Authorize.Require` in `DocumentService`, `ApprovalService` | xUnit forged-request test | M3 |
| C8 | Department and manager scope live in the query | Process → database | `DocumentRepository`, `ApprovalRepository.GetForApproverAsync` | SQL text logged with the predicate | M3 |
| C9 | External identities are mapped, never trusted for permissions | Process ↔ IdP | External sign-in seam + `PermissionService` | First login provisioned with no rights | M4 |
| C10 | Downloads only through the service, never a URL | Session → browser | `DocumentService.Download` + `Application.Download` | Forged download test | M5 |
| C11 | Uploads gated on size, type and content, quarantined | Browser → session | `UploadGate` | Renamed executable rejected | M5 |
| C12 | Share links are signed and expire | Browser → process (no session) | `ShareLinkService`, `/share/{id}` endpoint | Tampered / expired link → same 404 | M5 |
| C13 | Every sensitive command is audited, append-only | Process → database | `AuditService`, triggers on `audit_entry` | UPDATE refused, chain check | M6 |
| C14 | Secrets come from the environment | Process | Configuration (user secrets / env) | Fresh clone refuses to start without them | M6 |
| C15 | Transport and headers hardened, login and share links rate limited | Browser → process | Startup.cs middleware, `LoginRateLimiter` | Header fetch, 429 | M7 |
