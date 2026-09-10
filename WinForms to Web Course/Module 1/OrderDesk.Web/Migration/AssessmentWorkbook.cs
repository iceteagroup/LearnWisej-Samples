using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OrderDesk.Shared;

namespace OrderDesk.Migration
{
    /// <summary>One row of the assessment workbook: a form, feature or subsystem found in LegacyOrderDesk.</summary>
    public sealed class AssessmentItem
    {
        public string Feature { get; set; }
        public string Dependency { get; set; }
        public string RiskTag { get; set; }
        /// <summary>direct-port · adapt · redesign · defer · remove</summary>
        public string Verdict { get; set; }
        /// <summary>XS · S · M · L</summary>
        public string Effort { get; set; }
        /// <summary>Where it lives in LegacyOrderDesk.</summary>
        public string Source { get; set; }
        /// <summary>The migration-log line: why, and which module fixes it.</summary>
        public string Finding { get; set; }
    }

    /// <summary>
    /// The Module 1 deliverable as data: the inventory of LegacyOrderDesk with a risk tag, a verdict and
    /// an effort per item. docs/assessment-workbook.md is the same table in prose; the MainPage grid
    /// shows it live and the Replay button walks it one finding at a time.
    /// </summary>
    public static class AssessmentWorkbook
    {
        public static readonly IReadOnlyList<AssessmentItem> Items = new[]
        {
            new AssessmentItem
            {
                Feature = "OrdersForm · orders grid", Dependency = "DataGridView · full table load (GetAll)", RiskTag = "grid-volume",
                Verdict = "adapt", Effort = "M", Source = "OrdersForm.cs ReloadGrid()",
                Finding = "binds EVERY row — fine on a LAN desktop, a payload problem in the browser (Module 5: VirtualMode + server-side OrderQuery)",
            },
            new AssessmentItem
            {
                Feature = "OrdersForm · Print Invoice", Dependency = "PrintDocument → local printer", RiskTag = "report",
                Verdict = "redesign", Effort = "M", Source = "Legacy/InvoicePrinter.cs",
                Finding = "there is no user printer on the server — server-side PDF shown in PdfViewer / offered as Download (Module 6)",
            },
            new AssessmentItem
            {
                Feature = "OrdersForm · Export to Excel", Dependency = "Excel Interop (COM Excel.Application)", RiskTag = "office",
                Verdict = "redesign", Effort = "M", Source = "Legacy/ExcelExport.cs",
                Finding = "Office Automation is unsupported server-side — managed .xlsx writer + Application.Download (Module 6)",
            },
            new AssessmentItem
            {
                Feature = "AppState.CurrentUser / CurrentCustomer / ActiveFilter / CurrentOrder", Dependency = "static fields", RiskTag = "static-state",
                Verdict = "adapt", Effort = "S", Source = "Legacy/AppState.cs",
                Finding = "one process = one user on the desktop; one process = EVERY session on the server — Application.Session / UserContext (Module 4)",
            },
            new AssessmentItem
            {
                Feature = "UserPreferences (LastUser)", Dependency = @"HKCU registry Software\LegacyOrderDesk", RiskTag = "user-settings",
                Verdict = "adapt", Effort = "S", Source = "Legacy/UserPreferences.cs",
                Finding = "reads the SERVER's registry under the service account: wrong machine, wrong user — per-user server profile (Module 4)",
            },
            new AssessmentItem
            {
                Feature = @"LocalExport → C:\Orders\out.csv", Dependency = "local file path", RiskTag = "file-system",
                Verdict = "adapt", Effort = "S", Source = "Legacy/LocalExport.cs",
                Finding = "the file system is the server's, not the user's — Application.Download now (first slice), App_Data storage root later (Module 6)",
            },
            new AssessmentItem
            {
                Feature = "LoginForm", Dependency = "standard controls (ComboBox, Button)", RiskTag = "direct-port",
                Verdict = "direct-port", Effort = "S", Source = "LoginForm.cs",
                Finding = "System.Windows.Forms → Wisej.Web, same handlers; its registry default moves with UserPreferences",
            },
            new AssessmentItem
            {
                Feature = "EditOrderDialog", Dependency = "modal Form · ShowDialog · never disposed", RiskTag = "modal",
                Verdict = "adapt", Effort = "S", Source = "EditOrderDialog.cs",
                Finding = "ShowDialog keeps working; closed dialogs are NOT disposed on the server — using/Dispose, Toast instead of MessageBox (Module 3)",
            },
            new AssessmentItem
            {
                Feature = "MenuStrip / StatusStrip", Dependency = "ToolStrip family", RiskTag = "ui-shell",
                Verdict = "adapt", Effort = "S", Source = "OrdersForm.Designer.cs",
                Finding = "MenuStrip→MenuBar, StatusStrip→StatusBar, ToolStripMenuItem→MenuItem — the Module 2 compiler pass",
            },
            new AssessmentItem
            {
                Feature = "App.config", Dependency = "ConfigurationManager appSettings + connectionStrings", RiskTag = "config",
                Verdict = "adapt", Effort = "XS", Source = "App.config",
                Finding = "ExportFolder / ReportPrinter / LAN-SQL01 move to Web.config, read by AppConfig (Module 2)",
            },
            new AssessmentItem
            {
                Feature = "OrderService / OrderCalculator / OrderValidator / OrderStore", Dependency = "pure C# · no UI dependency", RiskTag = "business-logic",
                Verdict = "direct-port", Effort = "XS", Source = "Domain/*.cs (linked into both projects)",
                Finding = "reuse as-is — the same files are compiled by LegacyOrderDesk and OrderDesk.Web",
            },
            new AssessmentItem
            {
                Feature = "File → Exit", Dependency = "Form.Close() ends the process", RiskTag = "process-lifetime",
                Verdict = "remove", Effort = "XS", Source = "OrdersForm.cs exitMenuItem_Click",
                Finding = "the browser tab is the exit; the session ends on ApplicationExit / SessionTimeout (Module 4)",
            },
            new AssessmentItem
            {
                Feature = "Help → About", Dependency = "MessageBox with the version string", RiskTag = "direct-port",
                Verdict = "defer", Effort = "XS", Source = "OrdersForm.cs aboutMenuItem_Click",
                Finding = "harmless and not needed to prove the migration — after the first slice",
            },
        };

        /// <summary>The five verdicts, in the order the course names them.</summary>
        public static readonly string[] Verdicts = { "direct-port", "adapt", "redesign", "defer", "remove" };

        public static string Summary()
            => string.Join(" · ", Verdicts.Select(v => v + " " + Items.Count(i => i.Verdict == v)));

        /// <summary>Risk-tag colors as the walkthrough video paints them.</summary>
        public static Color TagColor(string tag)
        {
            switch (tag)
            {
                case "grid-volume": return Palette.Warn;                       // #e8a13c
                case "report": return Color.FromArgb(224, 90, 138);            // #e05a8a
                case "office": return Palette.Bad;                             // #e0563b
                case "static-state": return Palette.Bad;                       // #e0563b
                case "file-system": return Palette.Purple;                     // #7d5ae0
                case "user-settings": return Palette.Purple;
                case "direct-port": return Palette.Good;                       // #1f9d6b
                case "business-logic": return Palette.Good;
                case "modal": return Color.FromArgb(26, 134, 255);             // #1a86ff
                case "ui-shell": return Color.FromArgb(26, 134, 255);
                case "config": return Color.FromArgb(43, 181, 201);            // #2bb5c9
                default: return Palette.MutedText;                             // process-lifetime
            }
        }

        public static Color VerdictColor(string verdict)
        {
            switch (verdict)
            {
                case "direct-port": return Palette.Good;
                case "adapt": return Palette.Accent;
                case "redesign": return Palette.Warn;
                case "defer": return Palette.Purple;
                default: return Palette.MutedText;                             // remove
            }
        }
    }
}
