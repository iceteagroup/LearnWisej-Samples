# Enterprise Wisej.NET: Architecture to Cloud · lab samples

One runnable Wisej.NET 4 application per module of the advanced course, built from each module's lesson guide,
lab / exam guide and walkthrough video. Every sample is a slice of the same application — the **EnterpriseOps
Command Center**, a multi-tenant field-service work-order system — and each app shows only the screen(s) the
module's lab guide and walkthrough video build. Services log their decisions server-side (`Diagnostics/ActivityTrace`
→ `System.Diagnostics.Trace`); failures reach the user as safe messages with a correlation id. Each module folder
has its own `README.md` (run command, what to try, lab steps → code map) and, where the lab asks for written
deliverables, a `docs/` folder.

Requirements already on this machine: .NET 10 SDK and the `Wisej-4` 4.1.0 NuGet package (Module 4 also restores
`Microsoft.EntityFrameworkCore.Sqlite` 10.0.12). Nothing is deployed anywhere.

| Module | Folder | What it builds | Port |
|---|---|---|---|
| 1 · Architecture, Technical Leadership & Governance | `Module 1` | the solution baseline (folder-per-layer namespaces), the designable `CommandCenterDashboard` with a one-line handler, `ReviewGate` catching the 74-line legacy handler, ADR-001 + coding standards + review checklist | 5201 |
| 2 · Wisej.NET 4 Modernization & Migration Strategy | `Module 2` | the Migration Dossier (inventory, risk matrix, 7-step path, 10-flow regression harness, rollback) and the migrated `WorkOrdersPage` painted from the theme map | 5202 |
| 3 · Session, State, Tenant & Concurrency Design | `Module 3` | tenant-aware `SessionContext`, `CommandContext` + correlation id, `TenantGuard`, optimistic concurrency and the `ConflictDialog` (Reload · Compare · Cancel), a live static-state audit | 5203 |
| 4 · Real Data Architecture: EF Core, Transactions, Repositories & Commands | `Module 4` | EF Core on in-memory SQLite behind `IWorkOrderCommandService`: transactions, unique index, concurrency token, timeout, error mapping, the session-long DbContext anti-pattern, audit dialog | 5204 |
| 5 · High-Volume Data UX, Server Filtering & Batch Operations | `Module 5` | the Enterprise Work Queue: server paging over 6,000 rows, saved views, sort persistence, cross-page selection, batch reassignment with a per-row report, the load-everything anti-pattern measured | 5205 |
| 6 · Real-Time Systems, Background Pipelines, Notifications & Imports | `Module 6` | the Import Center: job queue, throttled progress observer, retry policy, cancellation, notifications, and re-attaching to a running job after the page is reopened | 5206 |
| 7 · Workflow UX: Wizards, Modal Orchestration & Compensation | `Module 7` | the six-step `EscalationWizard` over `EscalationWorkflow`: validation per step, directory timeout, notification failure with compensation, audit gap, the logic-in-the-page anti-pattern | 5207 |
| 8 · Custom Controls, Extensions, Widget Wrappers & Reusable Components | `Module 8` | `StatusTimeline` UserControl + `WorkOrderChartWidget` (own SVG vendor library, embedded resource package, typed events, fallback rendering), `ComponentApiGate` over a leaky screen | 5208 |
| 9 · JavaScript Object Model, Browser APIs & Secure Interop Contracts | `Module 9` | Ctrl+K command palette and browser capability panel through a typed interop contract; every command re-authorized server-side; the trust-the-client anti-pattern and its revert | 5209 |
| 10 · Security Architecture: Identity, SSO, Authorization, Audit & Secure Deployment | `Module 10` | simulated SSO sign-in gate, claims → roles → permissions, tenant guard, service-level `Demand`, export approval, the raw-HTML finding (it really runs) and the escape, an append-only audit log | 5210 |
| 11 · Observability, Diagnostics, Performance & Session Profiling | `Module 11` | `DiagnosticsPage`: structured JSON log, correlation ids end to end, performance budgets with a measured slow query, store failure, session-memory audit, health check | 5211 |
| 12 · Cloud, Containers, Load Balancing & Release Engineering | `Module 12` | `ReleaseDashboardPage`: environment configuration + fail-fast startup validation, `/healthz` probes, an 8-step runbook deploy with smoke tests and rollback, sticky-session simulation, Dockerfile / compose / nginx notes | 5212 |
| 13 · Hybrid, PWA, Offline & Device-Aware Applications | `Module 13` | Field Technician mode: `IDeviceServices`, offline cache + completion queue, reconnect sync with conflicts, revoked-permission rejection, device-writes-straight-to-store anti-pattern, phone/tablet/desktop layouts | 5213 |
| 14 · AI-Assisted Development, MCP-Ready Documentation & Capstone Delivery | `Module 14` | the capstone Command Center + Capstone Review: prompt library, a ten-rule generated-code review run live over an AI draft, MCP-shaped documentation index, demo script, deck outline, readiness statement | 5214 |

Run any module from its `EnterpriseOps` project folder. The projects multi-target `net10.0-windows` and `net10.0`,
so `dotnet run` needs a framework, e.g.

```bash
cd "D:/Projects/LearnWisej-Samples/Enterprise Wisej.NET Course/Module 5/EnterpriseOps"
dotnet run -f net10.0 --urls http://localhost:5205
```

or open the `EnterpriseOps.slnx` in the module folder with Visual Studio and press F5.

## `_template`

The scaffold every module was built from, plus `COOKBOOK.md`: the conventions (folder-per-layer namespaces, the
shared EnterpriseOps domain vocabulary), the framework facts verified while building
and reviewing these samples (dialogs, background pushes, data binding, widgets, health endpoints), and the gotchas
found on the way — `ComboBox.DisplayMember` needs properties, camel-casing stops at the first level of a Widget's
`Options`, static initializer order, and how to drive the buttons from the browser console when reviewing.

## Verified

Every module was built (`dotnet build`, both target frameworks, 0 warnings) and run in the browser on 2026-09-10,
clicking through its success, progress, failure and recovery paths. Defects found in that pass and fixed: the
approver list binding in Module 7, the chart segment casing in Module 8, the Production-by-default boot refusal
in Module 12, static initializer order in Modules 5 and 12, and a review-gate report type in Module 1.
