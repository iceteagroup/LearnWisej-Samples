# AccessOps identity matrix (Module 1)

Four columns, as the lab asks: **request**, **session**, **operation**, **outcome**. Filled from the
Debug build's identity trace (`AccessOps.IdentityTrace` log lines) and the identity diagnostics view,
run on 2026-11-09 against Wisej-4 4.1.7 on .NET 10 / Kestrel, `http://localhost:6401`, in two browser
sessions (two tabs of the Chromium browser: every tab is its own Wisej.NET session, because the session ID
lives in the tab's `sessionStorage`; two browser profiles behave the same way).

Session IDs are shown cut to their last four characters, account IDs as `acct …7f3a`. No raw session
ID, cookie or token is recorded anywhere.

## Profile 1 — `reader.a`, tenant A Reader

| Request | Session | Operation | Outcome |
|---|---|---|---|
| Startup · HTTP load | `Program.Main`, then `ApplicationStart` · new session `…a5cf` | sign-in screen built | no subject yet: `Application.User` is the host principal, AuthenticationType `(none)`, `IsAuthenticated` False |
| Sign-in click · WebSocket | same session `…a5cf` | `TestVerifier.Verify` → account parked under `AccessOps.PendingAccount` → `Application.Reload()` | nothing established yet; no `RefreshSessionId` on the click |
| Refresh · HTTP load (the reload) | `ApplicationRefresh` · same session state | `ApplicationPrincipal.Establish`: `RefreshSessionId()` then `Application.User` | **new ID `…4a3f`**, adopted by the browser (`sessionStorage["Wisej.SessionId"]` ends in `4a3f`); subject `acct …7f3a · tenant A`, AuthenticationType `Portal` |
| Click · WebSocket (Open A / 17) | same session `…4a3f` | `Application.StartTask` → `DocumentService.Read(subject, "A", 17)` | allowed · `ReadCount` +1 · "Record 17 opened · Supplier contract — Northwind renewal" |
| Click · WebSocket (Open B / 17, fixture F02) | same session | `DocumentService.Read(subject, "B", 17)` | denied · reason `tenant` (audit only) · `ReadCount` +0 · "Access denied." |
| Browser refresh · HTTP load (plain F5) | `ApplicationRefresh` · same session `…4a3f` | nothing pending, nothing rotated | same ID, subject kept, the screen keeps its state |
| Background · `Task.Run` (Export A / 17) | **no session** (`Application.SessionId` empty) | `ExportJob` → `DocumentService.Read(explicitSubject, "A", 17)` | explicit subject `acct …7f3a · tenant A`, grant rechecked when the job runs; after the revoke: denied · reason `grant` · `ReadCount` +0 |

## Profile 2 — not signed in, then `admin`

| Request | Session | Operation | Outcome |
|---|---|---|---|
| Startup · HTTP load | `Program.Main`, `ApplicationStart` · new session (different ID) | sign-in screen | no subject: AuthenticationType `(none)`, `IsAuthenticated` False, "No override: the getter falls back to the host principal." |
| Click · WebSocket (Admin console → Identity diagnostics, Debug build) | same session | read the identities | as above; Host request principal "not authenticated (Kestrel)"; Effective OS identity = the same process account as profile 1 |
| Sign-in click + reload | `ApplicationRefresh` | `Establish` for the admin account | new session ID, subject `acct …d4b7 · tenant A`, role Admin (no read right) |
| Click · WebSocket (Revoke grant …21) | same session | `GrantStore.SetRevoked(admin, grant, true)` | allowed (admin grant checked) · audit `grant.revoke` · profile 1's open Documents screen is **not** changed |

## Fields that are unavailable, and why

| Where | Field | Why |
|---|---|---|
| Raw `Task.Run` | `Application.SessionId`, the session's `Application.User` | The pool thread carries no Wisej.NET session ("Threads that are not started using StartTask don't have any knowledge of the current session"). Observed: `SessionId` empty, `Application.User.Identity.AuthenticationType` `(none)`, `IsAuthenticated` False — the subject has to arrive as data. |
| Raw `Task.Run` | `Application.IsWebSocket` | Reports false (the trace says "HTTP load"): there is no request at all, so the value means nothing on that thread. |
| `Application.StartTask` | — | Session restored: `Application.User` answered `Portal` / `acct …7f3a` inside the task. |
| Any request on Kestrel | Host request principal | Always "not authenticated (Kestrel)": Module 1 configures no host authentication (no IIS, no Negotiate). Module 2 adds the Windows route. |
| Any request | Effective Windows token per user | There is none: every request, both profiles and the background job run under the one process token (`<MACHINE>\<user> (dotnet process)`). Only impersonation (Module 3) changes it. |
| .NET 10 build | `Application.UserIdentity` | Declared only in the .NET Framework (net48) build of Wisej-4. |
| Everywhere | Raw session ID, cookies, tokens | Never shown or logged by design: IDs are cut to four characters; `document.cookie` is empty (Wisej keeps its session ID in `sessionStorage`). |
