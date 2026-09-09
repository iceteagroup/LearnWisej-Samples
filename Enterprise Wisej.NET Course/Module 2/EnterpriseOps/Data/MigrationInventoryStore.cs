using System.Collections.Generic;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// The dossier's raw material: the seven dossier areas, the current/target inventories, the compatibility
    /// matrix, the seven steps and the ten key flows — everything the assessment service reasons about.
    /// Factory methods return fresh instances so each session owns its own mutable copies.
    /// </summary>
    public class MigrationInventoryStore
    {
        /// <summary>The seven rows of docs/migration/MigrationDossier.md. Risk is left empty on purpose: the service computes it.</summary>
        public List<DossierRow> DossierRows() => new List<DossierRow>
        {
            new DossierRow { Area = "Target framework",        Current = ".NET Framework 4.8",  Target = ".NET 10",              RegressionProof = "full build + smoke run",   RollbackPoint = "git tag pre-fx",      BreakingChange = true,  UserVisible = false, ProvedByStep = 1 },
            new DossierRow { Area = "Wisej.NET version",       Current = "3.5",                 Target = "4.1",                  RegressionProof = "10 key screen flows",      RollbackPoint = "package pin 3.5",     BreakingChange = true,  UserVisible = true,  ProvedByStep = 3 },
            new DossierRow { Area = "Startup / configuration", Current = "Global.asax",         Target = "Program.cs + Startup", RegressionProof = "session + auth flows",     RollbackPoint = "config backup",       BreakingChange = true,  UserVisible = false, ProvedByStep = 2 },
            new DossierRow { Area = "Themes / resources",      Current = "custom Blue-2019",    Target = "mapped theme mixin",   RegressionProof = "visual diff per screen",   RollbackPoint = "theme folder copy",   BreakingChange = true,  UserVisible = true,  ProvedByStep = 4 },
            new DossierRow { Area = "Custom widgets",          Current = "2 JS interop widgets", Target = "re-tested on 4",      RegressionProof = "widget event log",         RollbackPoint = "feature flag off",    BreakingChange = true,  UserVisible = false, ProvedByStep = 5 },
            new DossierRow { Area = "Authentication",          Current = "forms auth",          Target = "same behavior",        RegressionProof = "login + role checks",      RollbackPoint = "standalone gate",     BreakingChange = true,  UserVisible = true,  ProvedByStep = 5 },
            new DossierRow { Area = "Deployment target",       Current = "IIS",                 Target = "IIS + health checks",  RegressionProof = "staging deploy",           RollbackPoint = "previous package",    BreakingChange = false, UserVisible = false, ProvedByStep = 6 },
        };

        /// <summary>Current-state and target-state inventory, one line per thing the lesson says to list before touching code.</summary>
        public List<InventoryItem> Inventory() => new List<InventoryItem>
        {
            new InventoryItem { Category = InventoryCategory.Project,        Name = "TicketOps.Web (single project)",      CurrentState = "1 project, folders by screen",            TargetState = "EnterpriseOps: UI / Domain / Services / Data / Security folders", Note = "ADR-001 (Module 1) — folder-per-layer first, split later" },
            new InventoryItem { Category = InventoryCategory.Framework,      Name = "Target framework",                    CurrentState = ".NET Framework 4.8",                       TargetState = "net10.0-windows;net10.0",                                        Note = "Kestrel host on net10.0; Windows target keeps the Designer" },
            new InventoryItem { Category = InventoryCategory.Package,        Name = "Wisej-3",                             CurrentState = "3.5.x",                                    TargetState = "Wisej-4 4.1.0",                                                   Note = "theme format and JS widget wrapper API changed" },
            new InventoryItem { Category = InventoryCategory.Package,        Name = "Newtonsoft.Json",                     CurrentState = "12.0.3 (direct)",                          TargetState = "removed — System.Text.Json",                                     Note = "only used by two DTOs" },
            new InventoryItem { Category = InventoryCategory.Package,        Name = "EntityFramework 6",                   CurrentState = "6.4.4",                                    TargetState = "EF Core 10 (Module 4)",                                           Note = "out of scope for this step — repository interface stays" },
            new InventoryItem { Category = InventoryCategory.Startup,        Name = "Global.asax / Web.config",            CurrentState = "Application_Start + appSettings",          TargetState = "Program.Main + Startup.cs + Default.json",                        Note = "license key stays in Web.config appSettings" },
            new InventoryItem { Category = InventoryCategory.Theme,          Name = "Blue-2019 custom theme",              CurrentState = "Themes/Blue-2019/*.json (3.x)",            TargetState = "Themes/Blue-2019.mixin.json over Bootstrap-4",                    Note = "3 tokens the screens depend on: accent, radius, priority colors" },
            new InventoryItem { Category = InventoryCategory.Resource,       Name = "Embedded images (24)",                CurrentState = "Resources/*.png embedded",                 TargetState = "same, EmbeddedResourceUseDependentUponConvention",               Note = "resource names change with the root namespace — map them" },
            new InventoryItem { Category = InventoryCategory.Resource,       Name = "Priority enum labels",                CurrentState = "Low / Medium / High",                      TargetState = "Low / Normal / High / Critical",                                  Note = "Medium → Normal; grid shows the new label" },
            new InventoryItem { Category = InventoryCategory.Component,      Name = "PriorityBadge (custom control)",      CurrentState = "UserControl, theme-colored",               TargetState = "same UserControl, colors from the mixin",                         Note = "flow 8 proves it" },
            new InventoryItem { Category = InventoryCategory.Component,      Name = "JS interop widgets (2)",              CurrentState = "Widget with inline init script",           TargetState = "Widget: _addListener / _getEventData wiring",                     Note = "widget event log is the proof" },
            new InventoryItem { Category = InventoryCategory.Authentication, Name = "Forms authentication",                CurrentState = "FormsAuthentication cookie + roles",       TargetState = "same behavior: SessionContext + PermissionService",                Note = "flow 1 proves login + role checks" },
            new InventoryItem { Category = InventoryCategory.Deployment,     Name = "Deployment target",                   CurrentState = "IIS, xcopy publish",                       TargetState = "IIS + /healthz probe (Module 12)",                                Note = "previous package kept for rollback" },
            new InventoryItem { Category = InventoryCategory.Integration,    Name = "Background import job",               CurrentState = "System.Timers.Timer in Global.asax",       TargetState = "Application.StartTask + Application.Update",                      Note = "flow 10 proves progress reaches the UI" },
        };

        /// <summary>The compatibility matrix: framework × Wisej.NET × packages, decided before any code changes.</summary>
        public List<CompatibilityEntry> CompatibilityMatrix() => new List<CompatibilityEntry>
        {
            new CompatibilityEntry { Component = "Wisej-4 4.1.0",                 CurrentVersion = "Wisej-3 3.5", TargetVersion = "net10.0-windows;net10.0", Supported = true,  Risk = RiskLevel.High,   Mitigation = "10 key flows after every step" },
            new CompatibilityEntry { Component = "Wisej.NET Designer",            CurrentVersion = "VS2019 / 3.5", TargetVersion = "VS2022 / 4.1",            Supported = true,  Risk = RiskLevel.Medium, Mitigation = "open every screen once (net10.0-windows)" },
            new CompatibilityEntry { Component = "Blue-2019 theme (3.x format)",  CurrentVersion = "3.x json",     TargetVersion = "4.x engine",              Supported = false, Risk = RiskLevel.High,   Mitigation = "port to a mixin; visual diff per screen" },
            new CompatibilityEntry { Component = "JS widget wrappers",            CurrentVersion = "3.x wrapper",  TargetVersion = "4.x wrapper",             Supported = true,  Risk = RiskLevel.Medium, Mitigation = "re-wire events with _addListener" },
            new CompatibilityEntry { Component = "Newtonsoft.Json 12",            CurrentVersion = "12.0.3",       TargetVersion = "net10.0",                 Supported = true,  Risk = RiskLevel.Low,    Mitigation = "replace with System.Text.Json" },
            new CompatibilityEntry { Component = "EntityFramework 6.4",           CurrentVersion = "6.4.4",        TargetVersion = "net10.0",                 Supported = true,  Risk = RiskLevel.Low,    Mitigation = "keep until Module 4 (EF Core)" },
            new CompatibilityEntry { Component = "Forms authentication",          CurrentVersion = "System.Web",   TargetVersion = "Kestrel",                 Supported = false, Risk = RiskLevel.High,   Mitigation = "SessionContext + PermissionService, same rules" },
            new CompatibilityEntry { Component = "Global.asax startup",           CurrentVersion = "System.Web",   TargetVersion = "Program.cs + Startup",    Supported = false, Risk = RiskLevel.Medium, Mitigation = "config backup; session + auth flows" },
            new CompatibilityEntry { Component = "IIS hosting",                   CurrentVersion = "IIS 10",       TargetVersion = "IIS 10 + ASP.NET Core module", Supported = true, Risk = RiskLevel.Low, Mitigation = "staging deploy; previous package" },
        };

        /// <summary>The seven verifiable steps, each with its check and its fallback point.</summary>
        public List<MigrationStep> Steps() => new List<MigrationStep>
        {
            new MigrationStep { Number = 1, Name = "Compile",             Check = "dotnet build both targets, 0 errors",          FallbackPoint = "git tag pre-fx" },
            new MigrationStep { Number = 2, Name = "Run",                 Check = "session starts, MainPage loads",               FallbackPoint = "config backup" },
            new MigrationStep { Number = 3, Name = "Compare behavior",    Check = "flows 1–5 (security + behavior)",             FallbackPoint = "package pin 3.5" },
            new MigrationStep { Number = 4, Name = "Check themes",        Check = "full harness, 10 flows incl. visual diff",     FallbackPoint = "theme folder copy" },
            new MigrationStep { Number = 5, Name = "Verify sessions",     Check = "flows 9–10 (session + background task)",      FallbackPoint = "feature flag off" },
            new MigrationStep { Number = 6, Name = "Review deployment",   Check = "staging deploy answers /healthz",             FallbackPoint = "previous package" },
            new MigrationStep { Number = 7, Name = "Measure performance", Check = "work queue query under the 3.5 budget",       FallbackPoint = "previous package" },
        };

        /// <summary>The ten key flows of docs/migration/RegressionPlan.md.</summary>
        public List<RegressionFlow> Flows() => new List<RegressionFlow>
        {
            new RegressionFlow { Id = 1,  Name = "Login + role check",                     Screen = "Login / any",       Category = FlowCategory.Security,       Expected = "ana.ops may approve, ben.tech may not" },
            new RegressionFlow { Id = 2,  Name = "Open work queue (Open tab)",             Screen = "WorkOrdersPage",    Category = FlowCategory.Behavior,       Expected = "only New/Assigned/OnHold rows of the tenant, due date ascending" },
            new RegressionFlow { Id = 3,  Name = "Filter In progress (server-side)",       Screen = "WorkOrdersPage",    Category = FlowCategory.Behavior,       Expected = "filter runs on the server; only InProgress/Escalated rows" },
            new RegressionFlow { Id = 4,  Name = "New work order — validation",            Screen = "WorkOrdersPage",    Category = FlowCategory.Behavior,       Expected = "blank title rejected with a message, nothing saved" },
            new RegressionFlow { Id = 5,  Name = "Approve with stale version",             Screen = "WorkOrdersPage",    Category = FlowCategory.Behavior,       Expected = "stale Version rejected; current Version accepted" },
            new RegressionFlow { Id = 6,  Name = "Theme: accent color on header + tabs",   Screen = "WorkOrdersPage",    Category = FlowCategory.Theme,          Expected = "accent #1565D8 as on 3.5" },
            new RegressionFlow { Id = 7,  Name = "Theme: corner radius on tabs + buttons", Screen = "WorkOrdersPage",    Category = FlowCategory.Theme,          Expected = "radius 7 px as on 3.5" },
            new RegressionFlow { Id = 8,  Name = "Theme: priority colors in the grid",     Screen = "WorkOrdersPage",    Category = FlowCategory.Theme,          Expected = "High red, Normal amber, Low green" },
            new RegressionFlow { Id = 9,  Name = "Session context survives navigation",    Screen = "Dossier ⇄ WorkOrders", Category = FlowCategory.Session,     Expected = "same SessionContext instance, no static user state" },
            new RegressionFlow { Id = 10, Name = "Background import progress reaches UI",  Screen = "ImportCenterPage",  Category = FlowCategory.BackgroundTask, Expected = "3 progress ticks observed" },
        };
    }
}
