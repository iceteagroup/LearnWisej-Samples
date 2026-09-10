# Deliverable 2 — Environment configuration table

**The contract:** the same artifact runs in every environment, so nothing environment-specific may live in the
code or in the image. Every setting below has a value per environment, a source, and an owner who may change it.

Configuration is layered by `builder.Configuration` in `Startup.cs`, lowest priority first:

```
appsettings.json                      base — committed, never secret
appsettings.{ASPNETCORE_ENVIRONMENT}.json   per environment — committed, never secret
environment variables                 EnterpriseOps__Section__Key — injected at deploy time (vault / platform)
```

A double underscore is the separator for nested keys: `EnterpriseOps:IdentityProvider:ClientSecret` is supplied as
`EnterpriseOps__IdentityProvider__ClientSecret`.

## The table

| Setting | Key | Development | Staging (test) | Production | Secret? | Source in prod | Owner |
|---|---|---|---|---|---|---|---|
| Connection string | `EnterpriseOps:ConnectionString` | `(localdb)\MSSQLLocalDB`, integrated security | `sql-staging`, integrated security | *absent from files* | **YES** | environment variable from the vault | platform |
| Logging level | `EnterpriseOps:Logging:Level` | `Debug` | `Information` | `Information` | no | `appsettings.Production.json` | ops |
| Upload limit (MB) | `EnterpriseOps:UploadLimitMb` | 100 | 50 | 25 | no | `appsettings.Production.json` | product |
| Feature flags | `EnterpriseOps:FeatureFlags` | `all-on` | `release-set` | `release-set` | no | `appsettings.Production.json` | product |
| Identity provider | `EnterpriseOps:IdentityProvider:Authority` | dev stub on localhost | staging IdP | corporate SSO | **PARTLY** — the authority is public, the secret is not | file + vault | security |
| IdP client secret | `EnterpriseOps:IdentityProvider:ClientSecret` | `dev-only-not-a-secret` | *absent from files* | *absent from files* | **YES** | environment variable from the vault | security |
| Allowed hosts | `EnterpriseOps:AllowedHosts` | `*` | `staging.enterpriseops.example` | `ops.enterpriseops.example` | no | `appsettings.Production.json` | security |
| Release version | `EnterpriseOps:Release:Version` | `2.4.2` | set by the pipeline | set by the pipeline | no | pipeline variable | release engineering |
| Previous version | `EnterpriseOps:Release:PreviousVersion` | `2.4.1` | set by the pipeline | set by the pipeline | no | pipeline variable | release engineering |
| Node name | `EnterpriseOps:Release:NodeName` | `dev-box` | per instance | per instance | no | environment variable set by the host | platform |
| Storage root | `EnterpriseOps:Storage:Root` | `./_storage` | mounted volume | object-storage mount | no | deployment manifest | platform |

The dashboard renders the first nine rows live: `ConfigurationService.BuildTable()` reads the **real**
`appsettings*.json` files, so the table in the app cannot drift from the files in the repository.

## The rules the table encodes

1. **No secret is ever committed.** `appsettings.Production.json` deliberately has no `ConnectionString` and no
   `ClientSecret`. A leaked repository exposes host names and log levels, nothing that grants access.
2. **A missing setting is a startup failure, not a runtime surprise.** `StartupValidation.Validate` runs in
   `Startup.cs` *before* `builder.Build()` and throws. A node that cannot work never joins the load balancer.
3. **Production-only rules exist.** `AllowedHosts` may not be `*` in Production (host-header spoofing), and
   `Logging:Level` may not be `Debug` (log volume and data exposure).
4. **Owners are named.** "Who may change the upload limit" has one answer, and it is not "whoever edits the server".

## Evidence in the running sample

| Action | What you see |
|---|---|
| The page loads | The configuration card lists every setting for all three environments; secrets show `●●●●●●●●` or `→ environment variable`, never a value. |
| Pick **Production** in the environment combo | Red banner: `missing EnterpriseOps:ConnectionString — expected from environment variable EnterpriseOps__ConnectionString (vault / platform secret store)`. The trace shows `StartupValidation REJECTED:` once per broken rule. This is the failure a clean checkout would hit on a real production host. |
| Press **Recover: inject platform secrets** | The same preview with the two environment variables supplied: `startup validation passed (… rules)` — the node would boot and join the balancer. |
| Pick **Staging** | One error, not four: the test database uses integrated security so its connection string is not a secret, but `IdentityProvider:ClientSecret` is still expected from the environment. |
| Pick **Development** | Green: a developer machine has every value locally, and none of them is real. |
