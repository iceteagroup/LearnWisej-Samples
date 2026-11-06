# Secrets and rotation procedure · AccessOps Module 6

AccessOps holds three credentials. None is in a tracked file: `appsettings.json` holds structure
and harmless defaults, `Default.json` holds Wisej.NET settings only (and is never served - see
`app.UseWhen(… !.json …)` in [Startup.cs](../AccessOps/Startup.cs)). The application refuses to start
when any of the three is missing.

## Where each secret lives

Configuration is layered and later sources win: `appsettings.json` < `appsettings.{Environment}.json`
< user secrets (Development only) < environment variables < a vault provider, if one is added.

| Secret | Configuration key | Development | Deployment |
|---|---|---|---|
| Database connection string | `ConnectionStrings:AccessOps` | user secrets, id `accessops-security-course-app` | `ConnectionStrings__AccessOps` environment variable, or a vault reference |
| OIDC client secret (app side) | `Oidc:ClientSecret` | user secrets, id `accessops-security-course-app` | `Oidc__ClientSecret`, or vault; better still a certificate / managed identity credential (no stored secret) |
| OIDC client secret (test IdP side) | `Clients:accessops:Secret` | user secrets, id `accessops-security-course-test-idp` | n/a - the test identity provider is never deployed |
| Share-link signing key | `ShareLinks:SigningKey` (base64, ≥ 32 bytes); `ShareLinks:PreviousSigningKey` during a rotation only | user secrets, id `accessops-security-course-app` | `ShareLinks__SigningKey`, or vault |

Development setup (values live in the user profile, outside the repository):

```bash
cd "Module 6/AccessOps"
dotnet user-secrets set "ConnectionStrings:AccessOps" "Data Source=../AccessOpsData/accessops.db"
dotnet user-secrets set "Oidc:ClientSecret" "<a long random value>"
dotnet user-secrets set "ShareLinks:SigningKey" "<base64 of 32 random bytes>"

cd "../AccessOps.TestIdentityProvider"
dotnet user-secrets set "Clients:accessops:Secret" "<the same value as Oidc:ClientSecret>"
```

PowerShell for the random values:
`[Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(32))`.

## Rotation, per secret

The rule for all three: design for **two valid credentials at once**, so rotation is a rolling
restart and never an outage.

| | Connection string | OIDC client secret | Share-link signing key |
|---|---|---|---|
| Owner | Database administrator | Identity team (the app registration's owner) | AccessOps service owner |
| Expiry | 90 days (password login) - none with a managed identity | 180 days, set on the app registration | 1 year |
| Overlap | Two logins (`accessops_app_a`, `accessops_app_b`) with the same grants | The provider allows two active client secrets | `ShareLinks:PreviousSigningKey`: links signed with it still open, new links use the current key only |
| Steps | 1. Enable the idle login with a new password. 2. Update `ConnectionStrings__AccessOps` to it. 3. Rolling restart; confirm `Audit chain check at start-up: intact` on every instance. 4. Disable the old login. | 1. Add a second secret on the provider. 2. Update `Oidc__ClientSecret`. 3. Rolling restart; sign in once on each instance. 4. Delete the old secret on the provider. | 1. Generate a new key. 2. Set `ShareLinks__PreviousSigningKey` to the old key and `ShareLinks__SigningKey` to the new one; rolling restart. 3. Wait one link lifetime (60 minutes). 4. Remove `ShareLinks__PreviousSigningKey`; restart. |
| Verify | `Session.SignIn` rows keep arriving | A work-account sign-in completes | A link issued before step 2 still opens during the overlap; tampered links still 404 (test `During_a_key_rotation_links_signed_with_the_previous_key_still_open`) |

## When a secret has been committed

Deleting the line and pushing does not help: the value is in the history, in every clone and
fork, in CI caches and possibly in a search index. The incident is closed only when:

1. the secret is **rotated** (a new value issued) and the **old one revoked** at its issuer - the
   provider, the database, the key holder;
2. the access logs of the issuer are reviewed for use of the old value between the commit and the
   revocation (for the client secret: token requests from unknown addresses);
3. the new value lives only in the secret store, and a secret scanner runs on the repository so the
   next one is caught before the push.

Rewriting history is optional clean-up; it never replaces step 1.
