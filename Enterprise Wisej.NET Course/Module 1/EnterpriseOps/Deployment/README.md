# Deployment — what belongs in this folder

The lab baseline asks for a place for deployment assets next to UI, services, data, integrations, security,
diagnostics and resources. In Module 1 the folder holds this note and nothing is deployed anywhere; the
assets arrive with the modules that need them.

| Asset | Where | Module |
|---|---|---|
| Local launch profile (port 5201) | `Properties/launchSettings.json` | 1 |
| Startup configuration (`startup`, `theme`, `debug`) | `Default.json` — never served, never copied to output | 1 |
| Kestrel host, static file server | `Startup.cs` | 1 |
| Environment settings (`appsettings.{Environment}.json`) | this folder → `builder.Configuration` | 12 |
| Health probe (`/healthz`) | `Startup.cs` (`app.MapGet` before `app.Run()`) | 12 |
| Dockerfile, compose file, release runbook | this folder | 12 |

Rule (docs/CodingStandards.md, D-3): nothing in `UI/`, `Services/` or `Data/` reads a connection string, a host
name or an environment name from a literal. `Architecture/Samples/OrderEntryLegacy.cs.txt` shows the violation
(`Server=ops-sql01;…` inside a click handler) that this folder exists to prevent.

Run locally:

```bash
cd "D:\Projects\LearnWisej-Samples\Enterprise Wisej.NET Course\Module 1\EnterpriseOps"
dotnet run -f net10.0 --urls http://localhost:5201
```
