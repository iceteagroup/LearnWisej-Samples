using System.Collections.Generic;
using System.Linq;

namespace OrderDesk.Migration
{
    /// <summary>The five buckets every compiler error of the port fell into.</summary>
    public enum ErrorCategory
    {
        /// <summary>Dead or renamed usings / namespaces.</summary>
        Namespace,
        /// <summary>A WinForms control that has a differently named Wisej.Web counterpart.</summary>
        ControlSubstitution,
        /// <summary>Desktop rendering/styling calls that have no meaning in a browser.</summary>
        IrrelevantStyling,
        /// <summary>A call that needs the user's PC (printer, registry, Office, local disk, process startup).</summary>
        UnsupportedDesktopOp,
        /// <summary>Compiles, but the behaviour differs on the web — recorded, fixed in a later module.</summary>
        UnclearDeferred
    }

    /// <summary>One line of the compiler error log: a symbol the build stopped on (or a silent difference).</summary>
    public sealed class CompilerError
    {
        public ErrorCategory Category { get; set; }
        public string Symbol { get; set; }          // what the code said
        public string Code { get; set; }            // CS0246 … or "—" when it compiled
        public int Count { get; set; }              // error lines the compiler printed for this symbol (0 = compiled)
        public string Message { get; set; }         // the compiler's message, shortened
        public string Fix { get; set; }             // what replaced it
        public int SurfacedInPass { get; set; }     // the build pass that first reported it (1 or 2)
        public int FixedInPass { get; set; }        // the build pass after which it was gone (1..3); 0 = never an error
        public bool Remaining { get; set; }         // still open after Module 2 → migration-log.md
        public string Module { get; set; }          // where the real replacement lands

        public string CategoryText => CompilerErrorLog.CategoryName(Category);
        public string RemainingText => Remaining ? "open" : "";
    }

    /// <summary>
    /// The Module 2 deliverable as data: every compiler error the namespace swap produced, measured by
    /// building the copied OrdersForm, EditOrderDialog and the WinForms Program.cs against Wisej-4 4.1.0
    /// (the same rows are in docs/CompilerErrorLog.md). Four build passes: 15 → 24 → 10 → 0.
    /// The count RISES after pass 1 because the compiler stops at unresolved field types and only
    /// reports the method bodies once the declarations compile.
    /// </summary>
    public static class CompilerErrorLog
    {
        public const int PassCount = 4;

        public static string CategoryName(ErrorCategory category) => category switch
        {
            ErrorCategory.Namespace => "Namespace/using",
            ErrorCategory.ControlSubstitution => "Control substitution",
            ErrorCategory.IrrelevantStyling => "Irrelevant styling",
            ErrorCategory.UnsupportedDesktopOp => "Unsupported desktop op",
            _ => "Unclear/deferred"
        };

        public static readonly IReadOnlyList<CompilerError> Entries = new List<CompilerError>
        {
            // ── Namespace/using ─────────────────────────────────────────────────────────────────────
            new CompilerError { Category = ErrorCategory.Namespace, Symbol = "using LegacyOrderDesk.Reporting;", Code = "CS0246", Count = 1, SurfacedInPass = 1, FixedInPass = 1,
                Message = "The type or namespace name 'LegacyOrderDesk' could not be found", Fix = "Deleted — InvoicePrinter/ExcelExport are desktop-only and were not copied", Module = "Module 2" },
            new CompilerError { Category = ErrorCategory.Namespace, Symbol = "using LegacyOrderDesk.Settings;", Code = "CS0246", Count = 1, SurfacedInPass = 1, FixedInPass = 1,
                Message = "The type or namespace name 'LegacyOrderDesk' could not be found", Fix = "Deleted — RegistrySettings is desktop-only and was not copied", Module = "Module 2" },
            new CompilerError { Category = ErrorCategory.Namespace, Symbol = "using System.Windows.Forms; / namespace LegacyOrderDesk", Code = "—", Count = 0, SurfacedInPass = 0, FixedInPass = 0,
                Message = "Done during the copy: one search-and-replace to Wisej.Web and OrderDesk", Fix = "using Wisej.Web; namespace OrderDesk", Module = "Module 2" },

            // ── Control substitution ────────────────────────────────────────────────────────────────
            new CompilerError { Category = ErrorCategory.ControlSubstitution, Symbol = "MenuStrip", Code = "CS0234", Count = 1, SurfacedInPass = 1, FixedInPass = 1,
                Message = "'MenuStrip' does not exist in the namespace 'Wisej.Web'", Fix = "Wisej.Web.MenuBar, Dock = Top", Module = "Module 2" },
            new CompilerError { Category = ErrorCategory.ControlSubstitution, Symbol = "ToolStripMenuItem (×10)", Code = "CS0234", Count = 10, SurfacedInPass = 1, FixedInPass = 1,
                Message = "'ToolStripMenuItem' does not exist in the namespace 'Wisej.Web'", Fix = "Wisej.Web.MenuItem", Module = "Module 2" },
            new CompilerError { Category = ErrorCategory.ControlSubstitution, Symbol = "StatusStrip", Code = "CS0234", Count = 1, SurfacedInPass = 1, FixedInPass = 1,
                Message = "'StatusStrip' does not exist in the namespace 'Wisej.Web'", Fix = "Wisej.Web.StatusBar, Dock = Bottom, ShowPanels = true", Module = "Module 2" },
            new CompilerError { Category = ErrorCategory.ControlSubstitution, Symbol = "ToolStripStatusLabel", Code = "CS0234", Count = 1, SurfacedInPass = 1, FixedInPass = 1,
                Message = "'ToolStripStatusLabel' does not exist in the namespace 'Wisej.Web'", Fix = "Wisej.Web.StatusBarPanel, AutoSize = Spring", Module = "Module 2" },
            new CompilerError { Category = ErrorCategory.ControlSubstitution, Symbol = "ToolStripMenuItem.DropDownItems (×4)", Code = "CS1061", Count = 4, SurfacedInPass = 2, FixedInPass = 2,
                Message = "'MenuItem' does not contain a definition for 'DropDownItems'", Fix = "MenuItem.MenuItems.AddRange(…)", Module = "Module 2" },
            new CompilerError { Category = ErrorCategory.ControlSubstitution, Symbol = "ToolStripItem[] (×6)", Code = "CS0234", Count = 6, SurfacedInPass = 2, FixedInPass = 2,
                Message = "'ToolStripItem' does not exist in the namespace 'Wisej.Web'", Fix = "MenuItem[] for menus, StatusBarPanel[] for the status bar", Module = "Module 2" },
            new CompilerError { Category = ErrorCategory.ControlSubstitution, Symbol = "MenuStrip.Items", Code = "CS1061", Count = 1, SurfacedInPass = 2, FixedInPass = 2,
                Message = "'MenuBar' does not contain a definition for 'Items'", Fix = "MenuBar.MenuItems", Module = "Module 2" },
            new CompilerError { Category = ErrorCategory.ControlSubstitution, Symbol = "StatusStrip.Items", Code = "CS1061", Count = 1, SurfacedInPass = 2, FixedInPass = 2,
                Message = "'StatusBar' does not contain a definition for 'Items'", Fix = "StatusBar.Panels", Module = "Module 2" },
            new CompilerError { Category = ErrorCategory.ControlSubstitution, Symbol = "Form.MainMenuStrip", Code = "CS1061", Count = 1, SurfacedInPass = 2, FixedInPass = 2,
                Message = "'OrdersForm' does not contain a definition for 'MainMenuStrip'", Fix = "Deleted — a docked MenuBar needs no owner property", Module = "Module 2" },
            new CompilerError { Category = ErrorCategory.ControlSubstitution, Symbol = "DataGridView + TextBoxColumn, GroupBox, Label, Button, ComboBox, TextBox", Code = "—", Count = 0, SurfacedInPass = 0, FixedInPass = 0,
                Message = "Compiled unchanged: DataPropertyName, DefaultCellStyle.Format, Anchor, AutoSizeMode.Fill, DropDownStyle", Fix = "None needed", Module = "—" },

            // ── Irrelevant styling ──────────────────────────────────────────────────────────────────
            new CompilerError { Category = ErrorCategory.IrrelevantStyling, Symbol = "FormBorderStyle.FixedDialog", Code = "CS0117", Count = 1, SurfacedInPass = 2, FixedInPass = 2,
                Message = "'FormBorderStyle' does not contain a definition for 'FixedDialog'", Fix = "FormBorderStyle.Fixed (Wisej: None, Fixed, FixedToolWindow, Sizable, SizableToolWindow)", Module = "Module 2" },
            new CompilerError { Category = ErrorCategory.IrrelevantStyling, Symbol = "Application.EnableVisualStyles()", Code = "CS0117", Count = 1, SurfacedInPass = 2, FixedInPass = 3,
                Message = "'Application' does not contain a definition for 'EnableVisualStyles'", Fix = "Deleted with Program.cs — the theme comes from Default.json (\"theme\": \"Bootstrap-4\")", Module = "Module 2" },
            new CompilerError { Category = ErrorCategory.IrrelevantStyling, Symbol = "Application.SetCompatibleTextRenderingDefault(false)", Code = "CS0117", Count = 1, SurfacedInPass = 2, FixedInPass = 3,
                Message = "'Application' does not contain a definition for 'SetCompatibleTextRenderingDefault'", Fix = "Deleted — the browser renders text", Module = "Module 2" },
            new CompilerError { Category = ErrorCategory.IrrelevantStyling, Symbol = "ISupportInitialize.BeginInit/EndInit, PerformLayout, TabIndex, ClientSize, ColumnHeadersHeightSizeMode", Code = "—", Count = 0, SurfacedInPass = 0, FixedInPass = 0,
                Message = "Compiled — harmless in Wisej.Web, left in place so the designer file stays diff-able", Fix = "None needed", Module = "—" },

            // ── Unsupported desktop op ──────────────────────────────────────────────────────────────
            new CompilerError { Category = ErrorCategory.UnsupportedDesktopOp, Symbol = "RegistrySettings.Load/Save (×3)", Code = "CS0103", Count = 3, SurfacedInPass = 2, FixedInPass = 3, Remaining = true,
                Message = "The name 'RegistrySettings' does not exist in the current context", Fix = "Original lines commented ✕; window size is the browser's; grid density → per-user profile store", Module = "Module 4" },
            new CompilerError { Category = ErrorCategory.UnsupportedDesktopOp, Symbol = "InvoicePrinter.Print (PrintDocument, PrintPreviewDialog)", Code = "CS0103", Count = 1, SurfacedInPass = 2, FixedInPass = 3, Remaining = true,
                Message = "The name 'InvoicePrinter' does not exist in the current context", Fix = "Commented ✕ + stand-in; InvoiceDocument stays, delivery becomes a server PDF in PdfViewer", Module = "Module 6" },
            new CompilerError { Category = ErrorCategory.UnsupportedDesktopOp, Symbol = "ExcelExport.ExportOrders (Excel Interop, C:\\Orders)", Code = "CS0103", Count = 1, SurfacedInPass = 2, FixedInPass = 3, Remaining = true,
                Message = "The name 'ExcelExport' does not exist in the current context", Fix = "Commented ✕ + stand-in; managed writer + Application.Download", Module = "Module 6" },
            new CompilerError { Category = ErrorCategory.UnsupportedDesktopOp, Symbol = "SettingsForm (edits HKCU)", Code = "CS0246", Count = 1, SurfacedInPass = 2, FixedInPass = 3, Remaining = true,
                Message = "The type or namespace name 'SettingsForm' could not be found", Fix = "Commented ✕ + stand-in; ported with the per-user settings store", Module = "Module 4" },
            new CompilerError { Category = ErrorCategory.UnsupportedDesktopOp, Symbol = "Application.Run(new OrdersForm())", Code = "CS0117", Count = 1, SurfacedInPass = 2, FixedInPass = 3,
                Message = "'Application' does not contain a definition for 'Run'", Fix = "Program.Main(NameValueCollection) sets Application.MainPage; Default.json names it (\"startup\")", Module = "Module 2" },
            new CompilerError { Category = ErrorCategory.UnsupportedDesktopOp, Symbol = "LoginForm (in Program.Main)", Code = "CS0246", Count = 1, SurfacedInPass = 2, FixedInPass = 3, Remaining = true,
                Message = "The type or namespace name 'LoginForm' could not be found", Fix = "Not copied — the WinForms Program.cs is replaced by the shell's; login becomes the session sign-in", Module = "Module 4" },
            new CompilerError { Category = ErrorCategory.UnsupportedDesktopOp, Symbol = "[STAThread]", Code = "—", Count = 0, SurfacedInPass = 0, FixedInPass = 0,
                Message = "Compiles anywhere but means nothing on a server thread pool — removed with Program.cs", Fix = "Deleted", Module = "Module 2" },

            // ── Unclear/deferred (0 compiler errors — compiles, behaves differently) ───────────────
            new CompilerError { Category = ErrorCategory.UnclearDeferred, Symbol = "dialog.ShowDialog(this) == DialogResult.OK (×3)", Code = "—", Count = 0, SurfacedInPass = 0, FixedInPass = 0, Remaining = true,
                Message = "Compiles, but ShowDialog returns immediately on the web: the OK branch never runs", Fix = "Minimal fix now: ShowDialog(owner, (form, result) => …); the Module 3 rule is ShowDialogAsync + dispose in the caller", Module = "Module 3" },
            new CompilerError { Category = ErrorCategory.UnclearDeferred, Symbol = "using (var dialog = …) { dialog.ShowDialog(this) … }", Code = "—", Count = 0, SurfacedInPass = 0, FixedInPass = 0, Remaining = true,
                Message = "Compiles, but disposes the dialog while it is still open in the browser", Fix = "Dispose in the callback (form.Dispose()) — done here; formalised in Module 3", Module = "Module 3" },
            new CompilerError { Category = ErrorCategory.UnclearDeferred, Symbol = "new OpenFileDialog { Title = … }", Code = "—", Count = 0, SurfacedInPass = 0, FixedInPass = 0, Remaining = true,
                Message = "Compiles — Wisej.Web.OpenFileDialog exists, but it browses the SERVER's file system", Fix = "Commented ✕ + stand-in; Upload control + server storage root", Module = "Module 6" },
            new CompilerError { Category = ErrorCategory.UnclearDeferred, Symbol = "static AppState.CurrentUser / CurrentFilter / LastSearch", Code = "—", Count = 0, SurfacedInPass = 0, FixedInPass = 0, Remaining = true,
                Message = "Compiles — one static slot for every browser session (Module 1 showed the leak)", Fix = "Application.Session behind a typed UserContext", Module = "Module 4" },
            new CompilerError { Category = ErrorCategory.UnclearDeferred, Symbol = "MessageBox.Show(\"Saved.\", \"LegacyOrderDesk\")", Code = "—", Count = 0, SurfacedInPass = 0, FixedInPass = 0, Remaining = true,
                Message = "Works as-is for a notification; a Yes/No decision needs ShowAsync or the callback overload", Fix = "Keep; review every MessageBox whose return value is read", Module = "Module 3" },
            new CompilerError { Category = ErrorCategory.UnclearDeferred, Symbol = "ordersGrid.DataSource = list; CurrentRow.DataBoundItem", Code = "—", Count = 0, SurfacedInPass = 0, FixedInPass = 0, Remaining = true,
                Message = "Compiles — list binding with DataPropertyName columns; loads every row like the desktop did", Fix = "Verify in the browser; server-side paging + virtual mode for real volumes", Module = "Module 5" },
        };

        /// <summary>Errors the compiler printed for a category over all passes.</summary>
        public static int CountFor(ErrorCategory? category) =>
            Entries.Where(e => category == null || e.Category == category.Value).Sum(e => e.Count);

        /// <summary>Total error lines over all passes (the "N errors → 0" figure).</summary>
        public static int Total => CountFor(null);

        /// <summary>Error lines the compiler printed in build pass <paramref name="pass"/> (1-based); pass 4 is the clean build.</summary>
        public static int ErrorsAtPass(int pass) =>
            Entries.Where(e => e.Count > 0 && e.SurfacedInPass <= pass && e.FixedInPass >= pass).Sum(e => e.Count);

        /// <summary>What each pass fixed — the line the replay writes into the trace.</summary>
        public static string PassSummary(int pass) => pass switch
        {
            1 => "declarations only: MenuStrip, ToolStripMenuItem ×10, StatusStrip, ToolStripStatusLabel + 2 dead usings — the compiler stops at unresolved field types",
            2 => "the bodies surface: DropDownItems ×4, ToolStripItem[] ×6, .Items ×2, MainMenuStrip, FixedDialog — plus every desktop op the missing usings hid",
            3 => "only the desktop ops are left: RegistrySettings ×3, InvoicePrinter, ExcelExport, SettingsForm, Application.Run/EnableVisualStyles/SetCompatibleTextRenderingDefault, LoginForm",
            _ => "Build succeeded — runs in the browser, but compiling isn't done (5 deferred items in the log)"
        };
    }
}
