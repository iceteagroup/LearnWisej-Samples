# Sanitized production configuration · AccessOps (Module 7)

Everything below is in the sample's code or configuration; values that would be secrets in
production are placeholders here and never appear in a tracked file.

## 1. Pinned versions (what was tested)

`AccessOps/AccessOps.csproj`:

```xml
<TargetFramework>net10.0</TargetFramework>
<PackageReference Include="Wisej-4" Version="4.1.7" />
<PackageReference Include="Wisej-4-WebAuthn" Version="4.1.7" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.OpenIdConnect" Version="10.0.14" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.Negotiate" Version="10.0.14" />
<PackageReference Include="Microsoft.Data.Sqlite" Version="10.0.14" />
```

Runtime observed: .NET 10.0.11 (PolicyHarness header). The Wisej.NET 4.2 `Passkey` / `OAuthClient`
components are not used and not referenced. Two known 4.1.7 WebAuthn defects are worked around in
code (`Identity/PasskeyService.cs` flags from `AuthenticatorData.Base64`, `Identity/Es256Signature.cs`).

## 2. One build, two IIS applications

`tools/Publish-Release.ps1 -Output <folder>` publishes once (`-c Release`) and checks the output.
Deploy the same folder to both applications; each sets its route:

| IIS application | Authentication | Environment |
|---|---|---|
| Lab Intranet (`accessops.contoso.test`) | Anonymous off, Windows on (`iis/windows-auth.location.xml` → applicationHost.config) | `AccessOps__Route=Intranet` |
| Partner (`partners.accessops.example`) | Anonymous on, Windows off | `AccessOps__Route=Partner`, `Oidc__ClientSecret=<from the vault>` |

Both: `ASPNETCORE_ENVIRONMENT=Production`, `AccessOps__DataRoot=<shared, ACL'd path>`,
`DataProtection__KeyPath=<shared key ring path>`. `AccessOps:LocalHttpVerification` and the fault
injection are refused / compiled out outside Development + Debug.

## 3. Startup identical on every node (`AccessOps/Startup.cs`)

```csharp
// forwarded headers from the known proxies only - the loopback defaults are cleared first
builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
    o.ForwardLimit = 1;
    o.KnownProxies.Clear();
    o.KnownIPNetworks.Clear();
    foreach (var proxy in config.GetSection("Hosting:KnownProxies").Get<string[]>() ?? []) o.KnownProxies.Add(IPAddress.Parse(proxy));
    foreach (var net in config.GetSection("Hosting:KnownNetworks").Get<string[]>() ?? []) o.KnownIPNetworks.Add(IPNetwork.Parse(net));
    foreach (var host in config.GetSection("Hosting:AllowedHosts").Get<string[]>() ?? []) o.AllowedHosts.Add(host);
});

// one key ring and one application name for every node
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(sharedKeyPath))   // DataProtection:KeyPath
    .SetApplicationName("AccessOps");                            // DataProtection:ApplicationName

app.UseForwardedHeaders();                 // first
// Intranet: ChallengeTrace, UseAuthentication (Negotiate, FallbackPolicy = DefaultPolicy), UseAuthorization
// Partner:  IngressSwitch, WebSocket origin guard, OpenID Connect + cookie endpoints, UseAuthorization
app.UseWisej();
app.UseWhen(cx => !cx.Request.Path.Value.EndsWith(".json", StringComparison.OrdinalIgnoreCase),
    a => a.UseFileServer());               // no downloadable .json (observed: /Default.json, /appsettings.json, /ingress.json → 404)
```

Production values (`appsettings.Production.json` or the environment, not in this repository):

```json
{
  "Hosting": { "KnownProxies": [ "10.20.0.5" ], "KnownNetworks": [ ], "AllowedHosts": [ "partners.accessops.example" ] },
  "DataProtection": { "ApplicationName": "AccessOps", "KeyPath": "\\\\fs01\\accessops-keys" },
  "AccessOps": { "AllowedOrigins": [ "https://partners.accessops.example" ], "DataRoot": "D:\\AccessOpsData" },
  "WebAuthn": { "RpId": "partners.accessops.example", "AllowedOrigins": [ "https://partners.accessops.example" ] },
  "Oidc": { "Authority": "https://login.contoso.example", "RequireHttpsMetadata": true }
}
```

Protect the key ring at rest in production (`ProtectKeysWithCertificate(...)` or DPAPI-NG for a
group of servers): the lab persists it unprotected and ASP.NET Core says so at startup
(`No XML encryptor configured`). The key-ring folder's ACL allows the app pool identities only.

## 4. Session affinity and the WebSocket

Wisej.NET keeps a session in one node's memory: the balancer must route every request of a session
to the same node (sticky cookie or client-IP hash) and pass the WebSocket upgrade (`/ws.wx`); some
balancers need TCP (layer-4) mode for WebSockets. A lost node loses its sessions - nothing that
must survive lives in `Application.Session`:

| Shared store (survives a node) | In the node (lost with it) |
|---|---|
| grants and revocations (`grants`) | the Wisej.NET session and its controls |
| sign-in hand-off handles, hashed (`signin_handoffs`) | `Application.Session` values (the pending password sign-in for one reload) |
| passkey challenges (`passkey_challenges`) and credentials (`passkey_credentials`) | |
| audit (`audit`, `login_audit`), the Data Protection key ring, the ingress switch | |

The lab's shared store is one SQLite file both processes open (WAL + `busy_timeout`); production
uses a server database - the code only needs single-statement consume (`DELETE … RETURNING` or an
equivalent conditional delete) and a conditional update for the passkey commit.

## 5. `"secure": true` in the published Default.json

The project's `Default.json` has no `secure` key (the lab runs `http://localhost`). The publish step
writes it into the output:

```powershell
$config = Get-Content $out\Default.json -Raw | ConvertFrom-Json
$config | Add-Member -NotePropertyName secure -NotePropertyValue $true -Force
$config | ConvertTo-Json | Set-Content $out\Default.json
```

Observed output (`tools/Publish-Release.ps1`, 2026-11-09):

```
Default.json "secure": true                OK
no appsettings.Development.json            OK
web.config present (IIS in-process)        OK
no ReleaseReviewView in AccessOps.dll      OK
no demo password in AccessOps.dll          OK
no fault injection in AccessOps.dll        OK
```

## 6. The grant is read on every call

`Security/RecordPolicy.cs` - the only place a protected read is decided, on both routes:

```csharp
grant = _grants.Find(subject?.GrantId);           // durable store, every call
missing / account / revoked / expired / role / tenant  → _audit.Deny("policy", …) and return
_records.Read(tenantId, recordId);                // only now
```

Nothing caches a grant: not the page, not the claims (they carry the grant ID only), not the session.

## 7. Diagnostics and synthetic data

`#if DEBUG` + Development only: the release review (`Pages/ReleaseReviewView.cs`, `Review/*`), the
software authenticator, the demo password hashes, the display passkeys and the fault injection. A
Release build seeds the accounts without passwords. The test identity provider is a separate project
that is never published with AccessOps.
