# AccessOps - asset and entry point inventory

## Assets

| # | Asset | Where it lives | Sensitivity | Why someone wants it |
|---|---|---|---|---|
| A1 | Employee records (name, department, role, manager) | `employees` table | Internal | Org mapping, phishing targets |
| A2 | Employee personal and pay data (salary band, review note, address, phone, date of birth) | `employees` table, same rows as A1 | Confidential | Pay comparison, disciplinary history, identity theft |
| A3 | Confidential documents (contracts, salary letters, board packs) | Document store outside the web root (Module 5) | Confidential / Restricted | The reason an attacker comes |
| A4 | Approval decisions | `approval_requests` (Module 3) | Internal, high integrity | Self-approval, fraud |
| A5 | Audit trail | `audit_entry` (Module 6) | Internal, high integrity | Erasing the evidence |
| A6 | Credentials: password hashes, the OIDC client secret, the share-link signing key, the connection string | Database (hashes), environment / user secrets (keys) | Secret | Becoming anyone |
| A7 | Session identity (`SessionContext` in `Application.Session`) | Server memory, per session | Secret while live | Riding someone else's session |

## Entry points

| # | Entry point | Reaches | Authenticated? | Input that must be distrusted |
|---|---|---|---|---|
| E1 | Login page (`LoginPage.btnSignIn_Click`) | Credential check | No | User name, password, timing of attempts |
| E2 | Every screen's event handlers (grid selection, buttons) | All services | Session | Row ids, selected record, typed text |
| E3 | `Upload` control (`Uploaded` event) | Document store | Session | File name, extension, declared type, content, size |
| E4 | Downloads (`Application.Download` from a service) | Document store | Session | The document id the screen asks for |
| E5 | ASP.NET Core endpoints beside the app (share link, OIDC callback) mapped in `Startup.cs` | Document store, sign-in | **No Wisej session** | Every query value, the signature, the expiry |
| E6 | JavaScript interop (`Application.Call`, `Application.Eval`) | The browser | Session | Any user value placed in script |
| E7 | The AI assistant prompt (planned feature) | Whatever tools it is given | Session | The prompt itself, and every document it reads |
| E8 | Static files in the web root | Files under `./` | No | The path; `.json` files are excluded in Startup.cs |
| E9 | Configuration (`Default.json`, `appsettings.json`, environment) | Startup | Deployment | Whoever can write it |

## Not in scope here

The host, reverse proxy and network (TLS termination, DoS protection) are the platform's; the
threat table records them as accepted or as host controls rather than inventing an application
control for them.
