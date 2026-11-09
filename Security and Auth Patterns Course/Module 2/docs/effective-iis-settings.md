# Effective authentication settings — intranet and public routes (Module 2 deliverable 1)

**Status:** the IIS columns are the *target* settings to verify with `appcmd list config` in the lab
environment (`iis-publish-and-rollback.md`, step 3). IIS is not installed on this machine, so no IIS
value below was observed here. The Kestrel column was observed on 2026-11-09.

| Setting | Intranet route — IIS site `Lab Intranet` | Public route — IIS site `AccessOps Public` (Module 4/5) | Intranet route on this machine (Kestrel, observed) |
|---|---|---|---|
| Binding | `https *:443 accessops.contoso.test` | `https *:443 partners.contoso.test` (separate host name) | `http://localhost:6402` |
| Application pool | `AccessOps Intranet`, No Managed Code, `ApplicationPoolIdentity` | its own pool | the `dotnet` process of the signed-in user `ITGLLC2\matte` |
| Target framework | `net10.0`, ASP.NET Core in-process (ANCM V2) | same | `net10.0`, Kestrel |
| `anonymousAuthentication` | `enabled="false"` | `enabled="true"` | no anonymous access: `FallbackPolicy = DefaultPolicy` |
| `windowsAuthentication` | `enabled="true" useKernelMode="true"` | `enabled="false"` | Negotiate handler (`Microsoft.AspNetCore.Authentication.Negotiate` 10.0.14) |
| Providers (order) | `Negotiate`, `NTLM` | — | first 401 offers `WWW-Authenticate: Negotiate` only |
| `authPersistNonNTLM` / `authPersistSingleRequest` | default (`true` / `false`) — record what you find | — | Kestrel keeps the NTLM result per connection (observed: later requests on the connection carry no `Authorization` header) |
| `useAppPoolCredentials` | `false` (kernel mode decrypts with the machine account; set `true` only when `HTTP/accessops.contoso.test` is registered to a custom pool account) | — | — |
| `extendedProtection` | `tokenChecking="None"` unless TLS ends at IIS and every client supports channel binding; then `Allow`/`Require` + SPN list | — | — |
| Where it is configured | `applicationHost.config` `<location path="Lab Intranet">` (`iis/windows-auth.location.xml`) | `applicationHost.config` / server defaults | `AccessOps/Startup.cs`, switch `AccessOps:WindowsAuthentication` |

Rollback values (baseline): a site created by the publish step inherits the server defaults —
Anonymous `enabled="true"`, Windows `enabled="false"`. The saved `appcmd` backup
`accessops-m2-before` restores them in one step.

Why two applications: the public sign-in page and (Module 5) the OpenID Connect callback must be
reachable before the app knows the partner, so that application keeps Anonymous transport and the
Wisej.NET app enforces access. Sharing one application would either challenge the external callback
for Windows credentials or open the intranet screens.
