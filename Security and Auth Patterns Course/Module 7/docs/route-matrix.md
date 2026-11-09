# Route matrix · AccessOps (Module 7)

One build, two IIS applications, started per route with `AccessOps:Route` (in IIS: the
`AccessOps__Route` environment variable of each application). Both rows end in the same
application principal and the same `RecordPolicy.Read`.

| | Intranet route | Partner route |
|---|---|---|
| **Entry point** | `https://accessops.contoso.test/` (IIS app "Lab Intranet"); lab: `http://localhost:6408` | `https://partners.accessops.example/` (IIS app "Partner"); lab: `https://localhost:6407` (+ `http://localhost:6447`, Development only) |
| **Framework target** | `net10.0`, Wisej-4 4.1.7, `Microsoft.AspNetCore.Authentication.Negotiate` 10.0.14 (no net48 project: `Application.UserIdentity`, `"impersonate": true` and `ImpersonationBegin/End` are .NET Framework only) | `net10.0`, Wisej-4 4.1.7, Wisej-4-WebAuthn 4.1.7, `Microsoft.AspNetCore.Authentication.OpenIdConnect` 10.0.14 |
| **IIS mode** | Anonymous **off**, Windows Authentication **on** (providers Negotiate, NTLM; kernel mode) via `iis/windows-auth.location.xml` in applicationHost.config; ANCM in-process (`web.config` from the publish) | Anonymous **on**, Windows Authentication **off**; HTTPS binding only; ANCM in-process; behind the TLS proxy |
| **Challenge** | `401 WWW-Authenticate: Negotiate` before any Wisej.NET code (lab: Kestrel `FallbackPolicy = DefaultPolicy`; observed 401 → 200, provider NTLM) | none at the host; the app's own sign-in page; OpenID Connect challenge at `/account/signin` (code + PKCE) |
| **Host principal** | `WindowsIdentity` (Kerberos on the domain, NTLM here) as `Application.User` on the page load | anonymous (`IsAuthenticated` false) |
| **App principal** | `ApplicationPrincipal.Establish` in `Program.Main`: account mapped from the SID (`WindowsAccountMap`), tenant and a grant issued by `AccessGrants.FindForAccount` (Reader → read right; Visitor → none; unmapped → no grant); `amr = windows` | `ApplicationPrincipal.Establish` on the page load after `RefreshSessionId()`: password (service accounts), OpenID Connect hand-off or passkey; one grant per sign-in; `amr = password / oidc / passkey` |
| **Execution identity** | the app pool identity; the request's token only inside `WindowsIdentity.RunImpersonated` in `ScopedDocumentReader` for one approved file; remote shares refused (no delegation, F04) | the app pool identity, always (no impersonation) |
| **Revocation owner** | Directory team (account, group) **and** AccessOps admin (grant) | Partner IdP admin (external account) **and** AccessOps admin / the user (grant); Security (passkey credentials) |
| **Timeouts** | grant 8 h absolute · Wisej `sessionTimeout` 1200 s · Kerberos ticket (domain policy) | grant 8 h · host cookie 8 h non-sliding · hand-off handle 2 min · cookie hand-off 60 s · passkey challenge 2 min · Wisej `sessionTimeout` 1200 s |
| **Rollback switch** | stays on (the tested baseline) | ingress switch `Ingress:PartnerRoute = Off` (lab: `AccessOpsData/ingress.json`; production: the balancer rule / IIS site stop) |

## Role and tenant map

| Account (invented) | Route | Tenant | Role → grant | Reads |
|---|---|---|---|---|
| `reader1` (Windows account mapped by SID; lab: the PC's own account) | Intranet | A | Reader → `can_read` | tenant A records, DOC-201/202/203 scoped files |
| `visitor1` (`--AccessOps:CurrentWindowsUser=visitor1`) | Intranet | A | Visitor → grant without `can_read` | nothing (policy:role) |
| unmapped Windows account | Intranet | - | no grant | nothing (policy:missing) |
| Dana Ellis (`dana.ellis@contoso.example` at Contoso Identity, or a passkey) | Partner | A | Reader | tenant A records |
| Jo Newman (`jo.newman@contoso.example`) | Partner | A | Reader | tenant A records (account B of the passkey cases) |
| Kai Berg (`kai.berg@northwind.example`) | Partner | C (not approved) | none - refused at the mapping | nothing |
| `svc.reports` (password form, service accounts only) | Partner | A | Reader | tenant A records |

Tenants A and B both hold **record 17** (A: "Supplier contract"; B: "Supplier contract — Tailspin
logistics"); tenant C exists and is not approved.

## Release decision for the second hop (F04)

Unchanged from Module 3 (`Module 3/docs/second-hop-decision.md`): no Kerberos delegation. Remote
shares are refused in `DocumentCatalog.ResolveApprovedPath` unless a path is listed in
`AccessOps:DelegationTargets` (empty). A remote file AccessOps must serve is read by a restricted
service identity with the per-operation policy and an explicit audit event.
