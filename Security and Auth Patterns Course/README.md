# Security & Auth Patterns · lab samples

One runnable solution per module of the **Security & Auth Patterns** course, all built around the
same application, **AccessOps** - an internal operations portal (departments, employees,
confidential documents, approvals and an admin console) with an **intranet Windows route** for staff
and a **public partner route** for partners, and one shared resource policy behind both. Every
module folder has its own `README.md` (what the screen shows, how to run it, what to try, lab
deliverables → code, what was verified here and what only the lab environment can show, where the
sample differs from the lesson or video) and a `docs/` folder with the written lab deliverables.

The modules fork after Module 1 and meet again in Module 7:

```
M1 Identity Boundaries ──┬── M2 IIS / Windows sign-in ── M3 Scoped impersonation ──┐
                         └── M4 Custom sign-in ── M5 OpenID Connect ── M6 Passkeys ─┴── M7 Capstone
```

| Module | Folder | Port | What it builds |
|---|---|---|---|
| 1 · Identity Boundaries in Wisej.NET | Module 1 | 6401 | The AccessOps sign-in and identity diagnostics: host principal vs. Wisej.NET application principal, `ApplicationPrincipal.Establish` after `RefreshSessionId`, `DocumentService.Read` behind a read-counting repository, identity matrix and release check |
| 2 · IIS and Browser Integrated Authentication | Module 2 | 6402 | The intranet route: Windows sign-in through the Negotiate challenge (Kestrel here, IIS in the lab), the grant mapped from the account SID, `HostFaultPage`, redacted challenge trace, IIS location file, publish and rollback steps |
| 3 · Windows Impersonation and Delegation | Module 3 | 6403 | The confidential-documents reader: `DocumentCatalog.ResolveApprovedPath`, `ScopedDocumentReader` with `WindowsIdentity.RunImpersonated`, before / inside / after identity trace, ACL fixtures, forced-failure reversion, second-hop decision |
| 4 · Custom Sign In and Session Security | Module 4 | 6404 | The partner route's own sign-in: PBKDF2 passwords, throttling, opaque revocable grants in `GrantStore`, session rotation on the page load, host cookie with antiforgery and origin checks, stale-tab export denied |
| 5 · Federation with OpenID Connect | Module 5 | 6405 | Partner federation with ASP.NET Core's OpenID Connect handler and the local test identity provider: issuer + subject mapping, one grant per callback, unapproved tenant and replay refused, sign-out of both sessions |
| 6 · Passkeys with the WebAuthn Extension | Module 6 | 6406 | Passkey enrollment and sign-in with `Wisej-4-WebAuthn`: one-time challenges, server-side assertion checks, compare-and-swap credential store, credential review page, the two 4.1.7 extension defects worked around |
| 7 · Production Design and Capstone | Module 7 | 6407 | Both routes from one build (`AccessOps:Route`), one `RecordPolicy` behind both, shared store for grants, hand-offs and passkey state, the Release Review screen running the 12-case evidence matrix, rollback rehearsal, runbook, trust boundaries and route matrix |

## Run a module

```powershell
# every module: build, harness, and (from Module 4) the tests
cd "Security and Auth Patterns Course\Module 3"
dotnet build AccessOps.slnx
dotnet run --project PolicyHarness

# Modules 1-3: http on the module's port (Modules 2-3: open it in Edge/Chrome on the same PC -
# Negotiate answers silently for localhost)
dotnet run --project AccessOps --urls http://localhost:6403

# Modules 5-7: secrets first (no secret is in any tracked file), then the test identity provider
cd "Security and Auth Patterns Course\Module 6"
$secret = "lab-only-" + [guid]::NewGuid().ToString("N")
dotnet user-secrets set "Clients:accessops:Secret" $secret --project AccessOps.TestIdentityProvider
dotnet user-secrets set "Oidc:ClientSecret" $secret --project AccessOps
dotnet run --project AccessOps.TestIdentityProvider --urls http://localhost:6416
dotnet run --project AccessOps --urls https://localhost:6406
dotnet test AccessOps.slnx
```

Modules 4-7 run on `https://localhost:640N` with the ASP.NET Core development certificate. A browser
that does not trust it (the in-app Chromium browser, for example) can use the **Development-only http
listener** on `http://localhost:644N`: `AccessOps:LocalHttpVerification=true` plus the listener's
origin in `AccessOps:AllowedOrigins` (and, for passkeys, `WebAuthn:AllowedOrigins`) and the test IdP's
redirect URIs - the module READMEs give the exact commands. The switch is refused outside the
Development environment. Module 7 starts one build twice - partner route
(`--AccessOps:Route=Partner`, 6407 / 6447) and intranet route (`--AccessOps:Route=Intranet`, 6408) -
next to its identity provider on 6417.

Requirements: the .NET 10 SDK; packages restore from nuget.org. Runtime data (SQLite database,
audit key, Data Protection keys, confidential-file fixtures) is created on first run in
`Module N/AccessOpsData/`, outside the web root and ignored by git.

**What is real and what is lab-only.** The samples target the released **Wisej.NET 4.1.7**
(`Wisej-4`, `Wisej-4-WebAuthn`): federation uses ASP.NET Core's OpenID Connect handler and passkeys
the WebAuthn extension - the Wisej.NET 4.2 `Passkey` and `OAuthClient` components are not used. The
Windows route was verified on a workgroup PC with **Kestrel and the Negotiate handler** (NTLM for a
local account); **IIS Windows Authentication, Kerberos, SPNs and the second hop** are shipped as
configuration files and documented steps that can only be verified in the lab environment (a domain
and an IIS server). **Passkeys need Windows Hello, a security key or a phone** (or Chrome's virtual
authenticator); the unit tests sign their assertions with a software authenticator. Each module
README says exactly what was observed and what was not.

All people, passwords, tenants and keys in the samples are invented demo values for local use only.
Demo sign-in details are in each module README, the seed code and the test identity provider's
`Data/users.json`.

## `_template`

`_template/COOKBOOK.md` collects the Wisej.NET 4.1.7 / ASP.NET Core facts established by building and
running these samples, by topic: structure and ports, packages, session and principal
(`RefreshSessionId` on a page load, `Application.User` on WebSocket events), Negotiate on Kestrel,
`WindowsIdentity.RunImpersonated`, cookies / antiforgery / origin checks and the ingress switch, the
OpenID Connect handler (including a restart in the middle of a callback), the WebAuthn extension and
its two 4.1.7 defects with their workarounds, the shared store across processes, logging correlation,
grid and UI gotchas, and embedded-browser testing. Each entry says how it was established.
