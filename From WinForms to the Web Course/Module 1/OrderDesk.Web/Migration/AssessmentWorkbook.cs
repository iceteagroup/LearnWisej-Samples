using System.Collections.Generic;

namespace OrderDesk.Migration
{
    /// <summary>The four migration verdicts (plus "remove" for dead features).</summary>
    public enum Verdict
    {
        DirectPort,
        Adapt,
        Redesign,
        Defer,
        Remove
    }

    /// <summary>One row of the migration assessment workbook.</summary>
    public sealed class AssessmentItem
    {
        public string Area { get; set; }          // Forms & controls · Business logic · Data access · State · Desktop boundary · Deployment
        public string Feature { get; set; }       // the screen / feature
        public string Dependency { get; set; }    // what it depends on today
        public string RiskTag { get; set; }       // grid-volume · report · office · static-state · file-system · direct-port · registry · startup
        public Verdict Verdict { get; set; }
        public string Effort { get; set; }        // S · M · L
        public bool InFirstSlice { get; set; }
        public string Why { get; set; }
        public string Replacement { get; set; }   // the web-safe pattern (or "none needed")

        public string VerdictText => Verdict switch
        {
            Verdict.DirectPort => "Direct-port",
            Verdict.Adapt => "Port-with-adaptation",
            Verdict.Redesign => "Redesign",
            Verdict.Defer => "Defer",
            _ => "Remove"
        };

        public string SliceText => InFirstSlice ? "✓" : "";
    }

    /// <summary>
    /// The assessment workbook for LegacyOrderDesk — the Module 1 deliverable, as data so the
    /// app can show it, filter it and count it. The same rows are in docs/MigrationAssessmentWorkbook.md.
    /// </summary>
    public static class AssessmentWorkbook
    {
        public static readonly IReadOnlyList<AssessmentItem> Items = new List<AssessmentItem>
        {
            new AssessmentItem { Area = "Deployment", Feature = "Startup (Program.Main)", Dependency = "Application.Run / EnableVisualStyles", RiskTag = "startup", Verdict = Verdict.Adapt, Effort = "S", InFirstSlice = true,
                Why = "One .exe per user becomes one server for many sessions; Application.Run does not exist.",
                Replacement = "Default.json startup → Program.Main sets Application.MainPage (Module 2)." },
            new AssessmentItem { Area = "State", Feature = "Login → current user", Dependency = "static AppState.CurrentUser", RiskTag = "static-state", Verdict = Verdict.Adapt, Effort = "M", InFirstSlice = true,
                Why = "A static field is one slot for the whole server: the second user overwrites the first.",
                Replacement = "Application.Session behind a typed UserContext (Module 4)." },
            new AssessmentItem { Area = "Forms & controls", Feature = "Orders screen (grid + detail)", Dependency = "DataGridView · standard controls", RiskTag = "grid-volume", Verdict = Verdict.Adapt, Effort = "M", InFirstSlice = true,
                Why = "Ports cleanly, but the desktop habit loads every row (200k in production).",
                Replacement = "Same screen; server-side filter + virtual rows (Module 5)." },
            new AssessmentItem { Area = "Forms & controls", Feature = "Edit Order dialog", Dependency = "modal Form · DialogResult", RiskTag = "direct-port", Verdict = Verdict.DirectPort, Effort = "S", InFirstSlice = false,
                Why = "Ordinary handlers and a modal result — the event-driven model comes across.",
                Replacement = "None needed; add disposal (using / Dispose in the callback) and a Toast instead of MessageBox (Module 3)." },
            new AssessmentItem { Area = "Forms & controls", Feature = "Customer lookup", Dependency = "standard controls", RiskTag = "direct-port", Verdict = Verdict.DirectPort, Effort = "S", InFirstSlice = false,
                Why = "Standard controls, simple binding, no local dependency.",
                Replacement = "None needed." },
            new AssessmentItem { Area = "Business logic", Feature = "OrderService (totals, discounts, search)", Dependency = "none (plain C#)", RiskTag = "direct-port", Verdict = Verdict.DirectPort, Effort = "S", InFirstSlice = true,
                Why = "No UI or desktop assumption — moves unchanged.",
                Replacement = "None needed; copy the file." },
            new AssessmentItem { Area = "Desktop boundary", Feature = "Print Invoice", Dependency = "PrintDocument → local printer", RiskTag = "report", Verdict = Verdict.Redesign, Effort = "M", InFirstSlice = true,
                Why = "The server has no printer attached to the user's desk.",
                Replacement = "Server-generated PDF shown in PdfViewer or downloaded (Module 6)." },
            new AssessmentItem { Area = "Desktop boundary", Feature = "Export to Excel", Dependency = "Excel Interop · C:\\Orders\\out.xlsx", RiskTag = "office", Verdict = Verdict.Redesign, Effort = "M", InFirstSlice = true,
                Why = "Office Automation is not supported server-side; the path is on the user's PC.",
                Replacement = "Managed spreadsheet writer + Application.Download (Module 6)." },
            new AssessmentItem { Area = "Desktop boundary", Feature = "Attach file", Dependency = "OpenFileDialog · C:\\Orders\\Attachments", RiskTag = "file-system", Verdict = Verdict.Redesign, Effort = "M", InFirstSlice = false,
                Why = "File.Open no longer means the user's disk.",
                Replacement = "Upload control → server storage root (Module 6)." },
            new AssessmentItem { Area = "State", Feature = "Settings (grid density, export folder)", Dependency = "HKCU registry", RiskTag = "registry", Verdict = Verdict.Adapt, Effort = "S", InFirstSlice = false,
                Why = "HKCU on the server is the service account's registry, shared by everyone.",
                Replacement = "Per-user profile store on the server + browser storage for UI prefs (Module 4)." },
            new AssessmentItem { Area = "Data access", Feature = "Connection string", Dependency = "App.config (real system; the lab store is in-memory)", RiskTag = "startup", Verdict = Verdict.Adapt, Effort = "S", InFirstSlice = false,
                Why = "App.config is gone; configuration belongs to the web host.",
                Replacement = "Web.config / appsettings (Module 2)." },
            new AssessmentItem { Area = "Forms & controls", Feature = "Menu bar (File · View · Reports · Help)", Dependency = "MenuStrip", RiskTag = "direct-port", Verdict = Verdict.DirectPort, Effort = "S", InFirstSlice = false,
                Why = "Wisej.Web.MenuBar maps directly.",
                Replacement = "None needed (navigation reviewed in Module 3)." },
            new AssessmentItem { Area = "Forms & controls", Feature = "Window size restore", Dependency = "registry + Form.Size", RiskTag = "registry", Verdict = Verdict.Remove, Effort = "S", InFirstSlice = false,
                Why = "The browser window belongs to the user; the server should not resize it.",
                Replacement = "Remove; responsive layout instead (Module 7)." },
            new AssessmentItem { Area = "Deployment", Feature = "Installer / ClickOnce", Dependency = "per-PC install", RiskTag = "startup", Verdict = Verdict.Defer, Effort = "L", InFirstSlice = false,
                Why = "Not needed to prove the migration; the web app is deployed once.",
                Replacement = "IIS / Linux / container deployment checklist (Module 7)." },
        };
    }
}
