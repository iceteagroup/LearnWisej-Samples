using System;
using System.Collections.Generic;
using System.IO;
using OrderDesk.Services;
using Wisej.Web;

namespace OrderDesk.Diagnostics
{
    /// <summary>One item of the final readiness checklist with the evidence the console prints when it passes.</summary>
    public sealed class ReadinessItem
    {
        public int Number { get; set; }
        public string Title { get; set; }
        public Func<string> Evidence { get; set; }
    }

    /// <summary>
    /// The lesson's eight-item Final Readiness Checklist, as code. Each item produces evidence from the
    /// running application where it can (reflection, file system, session state) and from the module docs
    /// where the evidence is a test that was run earlier in the course (Module 5's 200,000-row grid).
    /// </summary>
    public static class ReadinessChecklist
    {
        public static IReadOnlyList<ReadinessItem> Items => new[]
        {
            new ReadinessItem { Number = 1, Title = "Starts through the Wisej.NET startup files and shows the main view",
                Evidence = () => $"Default.json startup → OrderDesk.Program.Main → Application.MainPage = MainPage; this session is on {Application.MainPage?.GetType().Name ?? "?"}" },
            new ReadinessItem { Number = 2, Title = "No per-user state in unsafe static fields",
                Evidence = () => "StaticStateAudit: " + StaticStateAudit.Summary() + " · user context lives in Application.Session (UserSessionContext)" },
            new ReadinessItem { Number = 3, Title = "Registry, local file, Office Automation and process-launch assumptions reviewed",
                Evidence = () => "registry → session/profile store (M4) · C:\\Orders → storage root " + AppConfig.StorageRoot + " (M6) · Excel Interop → CsvExport in memory (M1/M6) · Process.Start → none left (SecurityReview.md)" },
            new ReadinessItem { Number = 4, Title = "Large grids tested with production-like row counts",
                Evidence = () => "Module 5: VirtualMode + CellValueNeeded against a private InMemoryOrderRepository of 200,000 orders; paging via OrderQuery.Skip/Take — not re-run here, evidence in docs/ReadinessChecklist.md" },
            new ReadinessItem { Number = 5, Title = "Transient dialogs are disposed",
                Evidence = () => "InvoicePreviewForm: ShowDialog((form, result) => form.Dispose()) — the caller disposes in the callback (Module 3 rule); no blocking ShowDialog() return values in the project" },
            new ReadinessItem { Number = 6, Title = "AllowHtml only with trusted or sanitized content",
                Evidence = () => "1 label with AllowHtml = true on user data → fed only by HtmlSanitizer (whitelist b, i, br); the Raw label exists as the documented failure path and is off by default" },
            new ReadinessItem { Number = 7, Title = "Responsive behaviour tested at desktop, tablet and phone sizes",
                Evidence = () => $"ClientProfiles.json: Phone ≤600 · Tablet 601–1024 · Desktop ≥1025; ResponsiveLayout applies three layouts; active now: {ResponsiveLayout.Describe(Application.ActiveProfile)} — tested = what docs/ResponsiveProfiles.md lists, nothing more" },
            new ReadinessItem { Number = 8, Title = "Deployment configuration, secrets, logs, temp paths and health checks documented",
                Evidence = () => DeploymentDocsEvidence() },
        };

        private static string DeploymentDocsEvidence()
        {
            string root = AppConfig.BaseFolder;
            string[] files = { "docs/DeploymentChecklist.md", "deploy/iis/web.config", "deploy/docker/Dockerfile", "deploy/docker/docker-compose.yml", "deploy/appsettings.Production.notes.md" };
            int present = 0;
            foreach (var file in files)
                if (File.Exists(Path.Combine(root, file.Replace('/', Path.DirectorySeparatorChar)))) present++;
            return $"{present}/{files.Length} deployment files present · /health endpoint in Startup.cs · logs in {AppLog.LogFolder}";
        }
    }
}
