# Security & Auth Patterns · lab samples

One runnable solution per module of the **Security & Auth Patterns** course, all built around the
same application, **AccessOps** - an internal portal for departments, employees, confidential
documents and approvals. Each module is cumulative: it starts from the previous module's code and
adds that module's controls. Every module folder has its own `README.md` (what to try, lab
deliverables → code, what was verified, answers to the review questions, where the sample differs
from the lesson) and a `docs/` folder with the written lab deliverables.

> **Wisej.NET version - read this first.** These samples target the released **Wisej-4 4.1.7** from
> nuget.org. The course teaches the `Passkey` and `OAuthClient` components introduced in
> **Wisej.NET 4.2**; they do not exist in 4.1.7. So **passkeys are not implemented** (Module 2), and
> **external sign-in uses ASP.NET Core's OpenID Connect handler** (cookie scheme + OpenID Connect,
> authorization code flow with PKCE) behind one small seam, `IExternalSignIn`, so that replacing it
> with `OAuthClient` later is a contained change (Module 4). Modules 5-7 apply their controls to the
> password sign-in and the OpenID Connect sign-in. Each affected module README names the lab
> deliverables this changes.

| Module | Folder | What it builds | App | Test IdP |
|---|---|---|---|---|
| 1 · The Wisej.NET security model | `Module 1` | Employees directory bound to a server projection; trust boundaries, asset/entry-point inventory, threat table, control placement map | 6401 | - |
| 2 · Authentication & session identity | `Module 2` | Login with PBKDF2 hashing, lockout, `RefreshSessionId`, `SessionContext`, idle timeout, sign-out with `Application.Exit()`, login audit | 6402 | - |
| 3 · Authorization: roles, claims, permissions | `Module 3` | `PermissionService` (role map + claims, denial wins), `IAuthorize` in every service, department and manager rules, forged-request xUnit tests | 6403 | - |
| 4 · OAuth & OpenID Connect | `Module 4` | Work-account sign-in through ASP.NET Core OIDC + a local test identity provider, claim mapping, first-login provisioning, service-account fallback, sign-out of both sessions, token refresh | 6404 | 6414 |
| 5 · Data, uploads, downloads & handlers | `Module 5` | Document store outside the web root, upload gate, downloads through the service, signed expiring share-link endpoint, injection/output review | 6405 | 6415 |
| 6 · Auditing, secrets & configuration | `Module 6` | Append-only hash-chained audit trail, correlation ID per command on every log line, secrets in user secrets / environment, rotation procedure, redacting log sink | 6406 | 6416 |
| 7 · Hardening checklist & capstone | `Module 7` | HTTPS/HSTS, security headers + CSP, cookie policy, request limits, rate limits on sign-in and share links, dependency audit, seven-module review | 6407 | 6417 |

## Run a module

```bash
# Modules 4-7: start the module's test identity provider first (a TEST DOUBLE - never deploy)
cd "Security and Auth Patterns Course/Module 5/AccessOps.TestIdentityProvider"
dotnet run --urls http://localhost:6415

cd "Security and Auth Patterns Course/Module 5/AccessOps"
dotnet run -f net10.0 --urls http://localhost:6405

# Modules 3-7: the xUnit tests
cd "Security and Auth Patterns Course/Module 5"
dotnet test
```

Or open `Module N/AccessOps.slnx` in Visual Studio. Requirements: the .NET 10 SDK; packages restore
from nuget.org. Runtime data (SQLite database, documents, logs) is created on first run in
`Module N/AccessOpsData/`, outside the web root and ignored by git.

**Modules 6 and 7 need secrets before they start** - the connection string, the OIDC client secret
and the share-link signing key are no longer in any tracked file. Set them once with
`dotnet user-secrets` (commands in the Module 6 README); both modules share the ids.

All people, passwords and keys in the samples are invented demo values for local use only. Demo
sign-in details are in each module's seed files and the test identity provider's `Data/users.json`.

## `_template`

`COOKBOOK.md`: the conventions and the Wisej.NET 4.1.7 / ASP.NET Core facts verified while building
and running these samples - `RefreshSessionId` only on a page load, `Application.Exit()` and
`autoReload`, `Application.Cookies` with HttpOnly cookies, the OpenID Connect hand-off into a Wisej.NET
session, what a `DataGridView` serializes to the browser, `.json` and the file server, the Upload
control's events, the CSP the client runs under, and more.
