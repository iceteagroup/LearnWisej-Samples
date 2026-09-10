# Migration inventory table — current state → target state

Deliverable 1 of the lab. *Inventory before action*: nothing may be discovered halfway through the upgrade, so
every project, framework, package, startup file, theme, resource, custom component, authentication entry point,
deployment target and integration of the Intermediate **TicketOps Console** is listed here **before** any code
changes, together with what it becomes in the Advanced **EnterpriseOps** baseline.

Source of truth in the sample: `Data/MigrationInventoryStore.Inventory()` — shown in the app on the
*Inventory* tab of `MigrationDossierPage`.

| # | Category | Item | Current state (TicketOps, Wisej.NET 3.5) | Target state (EnterpriseOps, Wisej.NET 4.1) | Note |
|---|---|---|---|---|---|
| 1 | Project | TicketOps.Web (single project) | 1 project, folders by screen | EnterpriseOps: `UI` / `Domain` / `Services` / `Data` / `Security` folders | ADR-001 (Module 1) — folder-per-layer first, split later |
| 2 | Framework | Target framework | .NET Framework 4.8 | `net10.0-windows;net10.0` | Kestrel host on `net10.0`; the Windows target keeps the Designer |
| 3 | Package | Wisej-3 | 3.5.x | Wisej-4 4.1.0 | theme format and JS widget wrapper API changed |
| 4 | Package | Newtonsoft.Json | 12.0.3 (direct) | removed — `System.Text.Json` | only used by two DTOs |
| 5 | Package | EntityFramework 6 | 6.4.4 | EF Core 10 (Module 4) | out of scope for this step — the repository interface stays |
| 6 | Startup | `Global.asax` / `Web.config` | `Application_Start` + `appSettings` | `Program.Main` + `Startup.cs` + `Default.json` | the licence key stays in `Web.config` `appSettings` |
| 7 | Theme | Blue-2019 custom theme | `Themes/Blue-2019/*.json` (3.x) | `Themes/Blue-2019.mixin.json` over Bootstrap-4 | 3 tokens the screens depend on: accent, radius, priority colours |
| 8 | Resource | Embedded images (24) | `Resources/*.png` embedded | same, `EmbeddedResourceUseDependentUponConvention` | resource names change with the root namespace — map them |
| 9 | Resource | Priority enum labels | Low / Medium / High | Low / Normal / High / Critical | **Medium → Normal**; the grid shows the new label |
| 10 | Component | `PriorityBadge` (custom control) | `UserControl`, theme-coloured | same `UserControl`, colours from the mixin | flow 8 proves it |
| 11 | Component | JS interop widgets (2) | `Widget` with an inline init script | `Widget`: `_addListener` / `_getEventData` wiring | the widget event log is the proof |
| 12 | Authentication | Forms authentication | `FormsAuthentication` cookie + roles | same behaviour: `SessionContext` + `PermissionService` | flow 1 proves login + role checks |
| 13 | Deployment | Deployment target | IIS, xcopy publish | IIS + `/healthz` probe (Module 12) | the previous package is kept for rollback |
| 14 | Integration | Background import job | `System.Timers.Timer` in `Global.asax` | `Application.StartTask` + `Application.Update` | flow 10 proves progress reaches the UI |

## Why every line carries a *note*

The note is the part that saves the upgrade. "24 embedded images" is not a risk; "resource names change with the
root namespace" is. Two entries in this table are silent breakages that a compiler cannot see:

- **#9 Priority `Medium` → `Normal`.** The enum still compiles, the grid still binds, and every label the operators
  read changes. `Domain/WorkOrder.cs` records the rename; flow 8 checks the colour of each label.
- **#8 embedded resource names.** They are `RootNamespace.Folder.File.png`; changing the root namespace renames
  every one of them at build time and nothing fails until a screen asks for an image at runtime.

## Coverage check

Ten categories are inventoried: `Project, Framework, Package, Startup, Theme, Resource, Component,
Authentication, Deployment, Integration` (`Domain/InventoryItem.InventoryCategory`). Each maps onto at least one
dossier row and at least one regression flow — if a category had neither, it would be an unproven change.

## Evidence — what the running app shows

- Click **Build dossier**, then open the **Inventory** tab: the fourteen rows above, current state and target state
  side by side.
- The trace prints
  `Data:  MigrationInventoryStore → 7 dossier areas, 14 inventory items, 9 compatibility entries`
  before any risk is computed — inventory first, action second.
