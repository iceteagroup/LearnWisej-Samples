# Dependency audit · AccessOps Module 7

| | |
|---|---|
| Date | 2026-11-06 |
| Run by | the sample build (owner for follow-up: AccessOps service owner) |
| Tools | `dotnet list package --vulnerable --include-transitive`, `--outdated`, `--deprecated`, `--include-transitive` |
| Scope | `Module 7/AccessOps.slnx`: `AccessOps`, `AccessOps.Tests`, `AccessOps.TestIdentityProvider` |
| Advisory source | nuget.org (GitHub Advisory Database) |
| SDK / runtime | .NET SDK 10.0.303; the app runs on Microsoft.NETCore.App / Microsoft.AspNetCore.App **10.0.11** (latest 10.0 runtime on the build machine) |
| Next review | 2026-12-07, and on every build once the audit is in the pipeline |

## Packages

`AccessOps` - top level: `Wisej-4` **4.1.7**, `Microsoft.Data.Sqlite` **10.0.14**,
`Microsoft.AspNetCore.Authentication.OpenIdConnect` **10.0.14**. Transitive (21):
`Managed.System.Drawing` 4.1.7, `Microsoft.Bcl.Cryptography` 10.0.2, `Microsoft.Data.Sqlite.Core`
10.0.14, `Microsoft.IdentityModel.*` 8.19.2 (Abstractions, JsonWebTokens, Logging, Protocols,
Protocols.OpenIdConnect, Tokens), `System.IdentityModel.Tokens.Jwt` 8.19.2,
`Microsoft.Win32.SystemEvents` 10.0.0, `SQLitePCLRaw.*` 2.1.13 (bundle_e_sqlite3, core,
lib.e_sqlite3, provider.e_sqlite3), `System.Configuration.ConfigurationManager` 10.0.0,
`System.Drawing.Common` 10.0.0, `System.Formats.Nrbf` 10.0.0, `System.Resources.Extensions` 10.0.0,
`System.Runtime.Caching` 10.0.0, `System.Security.Cryptography.ProtectedData` 10.0.0.

`AccessOps.Tests` adds `Microsoft.NET.Test.Sdk` 17.14.1, `xunit` 2.9.3,
`xunit.runner.visualstudio` 3.1.4, `Microsoft.AspNetCore.TestHost` 10.0.14.
`AccessOps.TestIdentityProvider` has no package references beyond the shared framework.
41 distinct packages across the solution.

Third-party JavaScript in the web root: **none** (`Default.html` only; the Wisej.NET client is
served from the `Wisej-4` assembly as `resource.wx/…` and is covered by the package version).

## Results (real output, trimmed to the findings)

```
--vulnerable --include-transitive
The given project `AccessOps.TestIdentityProvider` has no vulnerable packages given the current sources.
The given project `AccessOps.Tests` has no vulnerable packages given the current sources.
The given project `AccessOps` has no vulnerable packages given the current sources.

--deprecated
Project `AccessOps.Tests` has the following deprecated packages
   Top-level Package      Requested   Resolved   Reason(s)   Alternative
   > xunit                2.9.3       2.9.3      Legacy      xunit.v3 >= 0.0.0

--outdated
The given project `AccessOps.TestIdentityProvider` has no updates given the current sources.
Project `AccessOps.Tests` has the following updates to its packages
   Top-level Package                Requested   Resolved   Latest
   > Microsoft.NET.Test.Sdk         17.14.1     17.14.1    18.11.0
   > xunit.runner.visualstudio      3.1.4       3.1.4      4.0.1
The given project `AccessOps` has no updates given the current sources.
```

## Findings and decisions

| # | Finding | Severity | Decision | Owner | Expiry / due |
|---|---|---|---|---|---|
| D1 | *(found in Module 1, fixed before this audit)* `Microsoft.Data.Sqlite` 10.0.0 pulled `SQLitePCLRaw.lib.e_sqlite3` 2.1.11, flagged `NU1903` (high, GHSA-2m69-gcr7-jv3q) at restore - the transitive package nobody chose | High | **Upgraded**: `Microsoft.Data.Sqlite` 10.0.14 → `SQLitePCLRaw` 2.1.13; restore and `--vulnerable` clean | service owner | done |
| D2 | `xunit` 2.9.3 is marked *Legacy* (successor `xunit.v3`) | Low - test-only, never deployed | **Accept** until the test project moves to xunit.v3 | service owner | 2027-03-31 |
| D3 | `Microsoft.NET.Test.Sdk` 18.11.0 and `xunit.runner.visualstudio` 4.0.1 available | Info - test-only | **Accept**; upgrade together with D2 (the 4.x runner targets xunit.v3) | service owner | 2027-03-31 |
| D4 | `Wisej-4` 4.1.7 is the latest stable release; Wisej.NET 4.2 (Passkey, OAuthClient) is not released | Info | **Track**: re-run this audit when 4.2 ships | service owner | on release |
| D5 | Runtime patch level is set by the machine/base image, not the project | Info | Record the runtime per deployment (here 10.0.11) and patch with the monthly .NET servicing release | platform team | monthly |

No vulnerable package, direct or transitive. Production dependencies (`AccessOps`) are all current.

## Putting it in the pipeline

```bash
dotnet list package --vulnerable --include-transitive 2>&1 | tee audit.txt
grep -q "has the following vulnerable packages" audit.txt && exit 1 || exit 0
```

and `<WarningsAsErrors>NU1901;NU1902;NU1903;NU1904</WarningsAsErrors>` in a deployment build turns a
new advisory into a failed restore.
