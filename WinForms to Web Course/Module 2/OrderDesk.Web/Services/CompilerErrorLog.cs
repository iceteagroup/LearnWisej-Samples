using System.Collections.Generic;

namespace OrderDesk.Services
{
    /// <summary>One category of the compiler-error log kept while porting OrdersForm.</summary>
    public sealed class ErrorCategory
    {
        public string Category { get; set; }
        public string Example { get; set; }
        public string Fix { get; set; }
        /// <summary>Distinct fixes in the first build (text, because the namespace swap is counted in files).</summary>
        public string Count { get; set; }
        /// <summary>The bucket the video uses for the same errors.</summary>
        public string VideoBucket { get; set; }
    }

    /// <summary>
    /// The compiler-error log of the OrdersForm → OrdersPage port, as the lab asks for it: every
    /// error classified, not randomly edited. Counts are distinct fixes (14 → 6 → 0); the raw
    /// error list of the first build is longer because every designer line that touches a
    /// renamed type fails on its own (the video's funnel counts those: 38 → 0).
    /// </summary>
    public static class CompilerErrorLog
    {
        public const int FirstBuildErrors = 14;
        public const int AfterDirectFixes = 6;
        public const int FinalErrors = 0;

        public static readonly IReadOnlyList<ErrorCategory> Categories = new[]
        {
            new ErrorCategory
            {
                Category = "Namespace swap",
                Example = "using System.Windows.Forms;  ·  : Form",
                Fix = "using Wisej.Web;  ·  : UserControl (Page when it is the main view)",
                Count = "2 files",
                VideoBucket = "(before the first build)",
            },
            new ErrorCategory
            {
                Category = "Renamed types",
                Example = "MenuStrip, ToolStripMenuItem, ToolStripItem[], StatusStrip, ToolStripStatusLabel",
                Fix = "MenuBar, MenuItem, MenuItem[], StatusBar, StatusBarPanel",
                Count = "5",
                VideoBucket = "Control substitution",
            },
            new ErrorCategory
            {
                Category = "Designer-only properties",
                Example = "StartPosition, MainMenuStrip, BorderStyle.FixedSingle",
                Fix = "delete, delete, BorderStyle.Solid",
                Count = "3",
                VideoBucket = "Irrelevant styling",
            },
            new ErrorCategory
            {
                Category = "Missing members",
                Example = "menuStrip.Items, fileMenu.DropDownItems, statusStrip.Items, Close()",
                Fix = "MenuItems, MenuItems, Panels, session ends with the tab (Module 4)",
                Count = "4",
                VideoBucket = "Unsupported desktop op",
            },
            new ErrorCategory
            {
                Category = "Startup",
                Example = "Application.EnableVisualStyles(); Application.Run(new OrdersForm())",
                Fix = "Program.Main(NameValueCollection) → Application.MainPage; Default.json startup",
                Count = "2",
                VideoBucket = "Unsupported desktop op",
            },
        };
    }
}
