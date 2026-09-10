using System;
using System.IO;
using System.Linq;
using OrderDesk.Configuration;
using OrderDesk.Migration;
using OrderDesk.Views;
using Wisej.Web;

namespace OrderDesk
{
    /// <summary>
    /// Module 2 · Project conversion and Wisej.NET startup.
    ///
    /// Left, top:    the shell — the six files that replace Application.Run, with the REAL file of this
    ///               project previewed from Application.StartupPath.
    /// Left, bottom: the compiler error log of the port (OrdersForm + EditOrderDialog + the WinForms
    ///               Program.cs against Wisej-4 4.1.0), categorized, and the build funnel 15 → 24 → 10 → 0
    ///               replayed by a Timer.
    /// Right:        the migration log (live trace); underneath, App.config → Web.config (the desktop
    ///               ConfigurationManager pattern failing on the web host, then the XDocument read that
    ///               replaces it) and the ported OrdersForm opened as a floating window.
    /// </summary>
    public partial class MainPage : Page
    {
        private ErrorCategory? _categoryFilter;
        private int _pass;
        private OrdersForm _ordersForm;

        public MainPage()
        {
            InitializeComponent();
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            var configuration = Application.Configuration;
            trace.Add(TraceKind.Server, "startup", "Default.json → OrderDesk.Program.Main → Application.MainPage = new MainPage()");
            trace.Add(TraceKind.Server, "session", $"new browser session {Short(Application.SessionId)} · {Application.SessionCount} session(s) in this process");
            trace.Add(TraceKind.Server, "Application.Configuration", $"StartUp = {configuration.StartUp} · MainWindow = {Or(configuration.MainWindow, "(none)")} · ThemeName = {configuration.ThemeName} · Url = {configuration.Url}");
            trace.Add(TraceKind.Server, "Kestrel", $"{Application.ServerName}:{Application.ServerPort} · StartupPath = {Application.StartupPath}");
            trace.Add(TraceKind.Server, "Startup.cs", "app.UseWisej() — AddWisej() does not exist in Wisej-4 4.1.0, only UseWisej(); Kestrel owns the process, no Application.Run");
            trace.Add(TraceKind.Server, "namespace swap", "System.Windows.Forms → Wisej.Web: one search-and-replace over OrdersForm, OrdersForm.Designer, EditOrderDialog (+ Designer)");
            trace.Add(TraceKind.Server, "OrdersForm", "is still a Form — the port keeps the base class; docs/FormVsPage.md shows the one-line Page variant");
            trace.Add(TraceKind.Server, "CompilerErrorLog", $"{CompilerErrorLog.Total} error lines over {CompilerErrorLog.PassCount} passes: {PassTrail()} · {OpenItems()} open items → migration-log.md");

            BindShell();
            BindErrors();
            labelFunnelCount.Text = $"{CompilerErrorLog.Total} error lines";
            labelFunnelPass.Text = $"four build passes: {PassTrail()} — press ▶ to replay them";
            labelConfigValues.Text = "App.config   ✕ gone with the .exe — ConfigurationManager has nothing to read on the web host\n" +
                                     "Web.config   ✓ " + WebConfig.FilePath + "\n" +
                                     "press a button to read the OrderDesk connection string both ways";
            Ui.SetStatus(labelStatus, "shell running · OrdersForm ported · 0 compiler errors", Ui.Ok);
        }

        #region Card A · the Wisej.NET shell (deliverable 1)

        private void BindShell()
        {
            gridShell.Rows.Clear();
            foreach (var file in ShellAnatomy.Files)
            {
                int index = gridShell.Rows.Add(file.FileName, file.Replaces, file.Role);
                gridShell.Rows[index].Tag = file;
                gridShell.Rows[index].Cells[0].Style.Font = Ui.SmallBold;
            }
            if (gridShell.Rows.Count > 0)
            {
                gridShell.Rows[0].Selected = true;
                ShowShellPreview(gridShell.Rows[0].Tag as ShellFile, announce: false);
            }
        }

        private void gridShell_SelectionChanged(object sender, EventArgs e)
        {
            ShowShellPreview(gridShell.CurrentRow?.Tag as ShellFile, announce: true);
        }

        private void ShowShellPreview(ShellFile file, bool announce)
        {
            if (file == null)
            {
                labelShellPreview.Text = "";
                return;
            }

            // ShellAnatomy.Preview reads the real file from Application.StartupPath (the project folder under dotnet run).
            const int shown = 7;
            string preview = ShellAnatomy.Preview(file.FileName, out int totalLines);
            var lines = preview.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).Take(shown).ToList();
            if (totalLines > shown)
                lines.Add($"… ({totalLines - shown} more lines) · {ShellAnatomy.PathOf(file.FileName)}");
            labelShellPreview.Text = string.Join("\n", lines);

            if (announce)
                trace.Add(TraceKind.ToClient, "ShellAnatomy.Preview", $"{file.FileName} · {totalLines} lines · replaces {file.Replaces}");
        }

        #endregion

        #region Card B · compiler errors, categorized (deliverable 2) + the build funnel

        private void BindErrors()
        {
            var rows = CompilerErrorLog.Entries
                .Where(x => _categoryFilter == null || x.Category == _categoryFilter.Value)
                .ToList();

            gridErrors.Rows.Clear();
            foreach (var error in rows)
            {
                int index = gridErrors.Rows.Add(error.Symbol, error.Code, error.Count, error.CategoryText, error.Fix, error.Module, error.RemainingText);
                gridErrors.Rows[index].Tag = error;
                gridErrors.Rows[index].Cells[3].Style.ForeColor = CategoryColor(error.Category);
                gridErrors.Rows[index].Cells[3].Style.Font = Ui.SmallBold;
                if (error.Remaining)
                    gridErrors.Rows[index].Cells[6].Style.ForeColor = Ui.Warn;
            }

            labelErrorsCount.Text = _categoryFilter == null
                ? $"{CompilerErrorLog.Total} error lines over {CompilerErrorLog.PassCount} passes → 0 · {OpenItems()} open items"
                : $"{rows.Count} of {CompilerErrorLog.Entries.Count} rows · {CompilerErrorLog.CountFor(_categoryFilter)} error lines · {CompilerErrorLog.CategoryName(_categoryFilter.Value)}";

            if (gridErrors.Rows.Count > 0)
            {
                gridErrors.Rows[0].Selected = true;
                ShowErrorDetail(gridErrors.Rows[0].Tag as CompilerError);
            }
            else
            {
                ShowErrorDetail(null);
            }
        }

        private static System.Drawing.Color CategoryColor(ErrorCategory category) => category switch
        {
            ErrorCategory.Namespace => Ui.Muted,
            ErrorCategory.ControlSubstitution => Ui.Accent,
            ErrorCategory.IrrelevantStyling => Ui.Ok,
            ErrorCategory.UnsupportedDesktopOp => Ui.Warn,
            _ => Ui.Purple
        };

        private void gridErrors_SelectionChanged(object sender, EventArgs e)
        {
            ShowErrorDetail(gridErrors.CurrentRow?.Tag as CompilerError);
        }

        private void ShowErrorDetail(CompilerError error)
        {
            if (error == null)
            {
                labelErrorDetail.Text = "";
                return;
            }
            labelErrorDetail.Text = $"{error.Code} · {error.Message}\nfix: {error.Fix}";
        }

        private void buttonCatAll_Click(object sender, EventArgs e) => ApplyCategoryFilter(null, "all");
        private void buttonCatNamespace_Click(object sender, EventArgs e) => ApplyCategoryFilter(ErrorCategory.Namespace, "namespace/using");
        private void buttonCatControls_Click(object sender, EventArgs e) => ApplyCategoryFilter(ErrorCategory.ControlSubstitution, "control substitution");
        private void buttonCatStyling_Click(object sender, EventArgs e) => ApplyCategoryFilter(ErrorCategory.IrrelevantStyling, "irrelevant styling");
        private void buttonCatDesktop_Click(object sender, EventArgs e) => ApplyCategoryFilter(ErrorCategory.UnsupportedDesktopOp, "unsupported desktop op");
        private void buttonCatDeferred_Click(object sender, EventArgs e) => ApplyCategoryFilter(ErrorCategory.UnclearDeferred, "unclear/deferred");

        private void ApplyCategoryFilter(ErrorCategory? category, string name)
        {
            _categoryFilter = category;
            BindErrors();
            trace.Add(TraceKind.FromClient, "errors.filter", $"category = {name} → {gridErrors.Rows.Count} rows · {CompilerErrorLog.CountFor(category)} error lines");
        }

        private void buttonReplayFunnel_Click(object sender, EventArgs e)
        {
            // The progress path: a Wisej.Web.Timer walks the four build passes; every tick is pushed to the browser.
            _pass = 0;
            buttonReplayFunnel.Enabled = false;
            labelFunnelCount.ForeColor = Ui.Error;
            Ui.HideBanner(labelBanner);
            trace.Add(TraceKind.FromClient, "build.replay", $"categorize, fix, rebuild — {CompilerErrorLog.PassCount} passes, {timerBuild.Interval} ms each (Wisej.Web.Timer)");
            Ui.SetStatus(labelStatus, "rebuilding — pass 1 of 4", Ui.Warn);
            timerBuild.Start();
        }

        private void timerBuild_Tick(object sender, EventArgs e)
        {
            _pass++;
            int errors = CompilerErrorLog.ErrorsAtPass(_pass);
            labelFunnelCount.Text = $"{errors} error{(errors == 1 ? "" : "s")}";
            labelFunnelPass.Text = $"pass {_pass}/{CompilerErrorLog.PassCount} · {CompilerErrorLog.PassSummary(_pass)}";
            trace.Add(TraceKind.Server, $"build pass {_pass}/{CompilerErrorLog.PassCount}", $"{errors} errors — {CompilerErrorLog.PassSummary(_pass)}");

            if (_pass < CompilerErrorLog.PassCount)
            {
                Ui.SetStatus(labelStatus, $"rebuilding — pass {_pass + 1} of {CompilerErrorLog.PassCount}", Ui.Warn);
                return;
            }

            timerBuild.Stop();
            buttonReplayFunnel.Enabled = true;
            labelFunnelCount.ForeColor = Ui.Ok;
            labelFunnelCount.Text = "0 errors · build succeeded";
            trace.Add(TraceKind.ToClient, "build.replay", "Build succeeded — but compiling isn't done");
            Ui.ShowBanner(labelBanner, $"✓ Build succeeded — but compiling isn't done: {OpenItems()} items compile and behave differently (ShowDialog result, static AppState, OpenFileDialog on the server, MessageBox return values, load-all grid) or are commented stand-ins. They are logged in migration-log.md with the module that fixes each.", Ui.BannerKind.Ok);
            Ui.SetStatus(labelStatus, "build succeeded — runs in the browser, but compiling isn't done", Ui.Ok);
        }

        #endregion

        #region Card C · configuration: App.config → Web.config (deliverable 3)

        private void buttonLegacyConfig_Click(object sender, EventArgs e)
        {
            // The failure path: the desktop pattern, unchanged, on the web host.
            trace.Add(TraceKind.FromClient, "config.legacy", "ConfigurationManager.ConnectionStrings[\"OrderDesk\"] — the desktop pattern, unchanged");
            try
            {
                string value = Legacy.AppConfig.ConnectionString("OrderDesk");
                // Not reached on the web host — kept so the console stays honest if somebody drops an <exe>.config into bin.
                labelConfigValues.Text = $"looked for   {Legacy.AppConfig.ExeConfigPath}\nfound        {Or(WebConfig.Mask(value), "(no OrderDesk entry)")}";
                trace.Add(TraceKind.Server, "AppConfig.ConnectionString", $"unexpectedly found {Path.GetFileName(Legacy.AppConfig.ExeConfigPath)} next to the binary");
                Ui.ShowBanner(labelBanner, "An <exe>.config was found next to the binary — that is a desktop deployment artefact, not the web host's configuration. Delete it and use Web.config.", Ui.BannerKind.Warn);
                Ui.SetStatus(labelStatus, "exe config found — should not be there", Ui.Warn);
            }
            catch (FileNotFoundException ex)
            {
                labelConfigValues.Text = $"looked for   {ex.FileName}\nfound        nothing — FileNotFoundException\nconnection   —";
                trace.Add(TraceKind.Boundary, "ConfigurationManager", $"looked for {ex.FileName} — App.config was the .exe's file; the web host has Web.config in the content root → XDocument");
                Ui.ShowBanner(labelBanner, $"✕ FileNotFoundException: {ex.Message} ConfigurationManager reads <exe>.config next to the binary; on the web host the binary is OrderDesk.dll under Kestrel and configuration lives in Web.config. Press Web.config for the replacement.", Ui.BannerKind.Error);
                Ui.SetStatus(labelStatus, "App.config is gone — the desktop read fails on the web host", Ui.Error);
            }
        }

        private void buttonWebConfig_Click(object sender, EventArgs e)
        {
            // The recovery: the same value, read from the web host's configuration with System.Xml.Linq.
            trace.Add(TraceKind.FromClient, "config.web", "WebConfig.ConnectionString(\"OrderDesk\") — XDocument over Web.config in the content root");
            string value = WebConfig.ConnectionString("OrderDesk");
            trace.Add(TraceKind.Server, "WebConfig.ConnectionString", $"XDocument.Load({WebConfig.FilePath}) → connectionStrings/add[@name='OrderDesk'] → {(value == null ? "missing" : WebConfig.Mask(value))}");

            if (value == null)
            {
                labelConfigValues.Text = $"read         {WebConfig.FilePath}\nfound        no <add name=\"OrderDesk\"> under <connectionStrings>";
                Ui.ShowBanner(labelBanner, "Web.config has no OrderDesk connection string — add <connectionStrings><add name=\"OrderDesk\" connectionString=\"…\"/></connectionStrings>.", Ui.BannerKind.Warn);
                Ui.SetStatus(labelStatus, "Web.config read, entry missing", Ui.Warn);
                return;
            }

            labelConfigValues.Text = $"read         {WebConfig.FilePath}\nfound        connectionStrings/add[@name='OrderDesk']\nconnection   {WebConfig.Mask(value)}";
            Ui.ShowBanner(labelBanner, $"✓ Web.config: OrderDesk = {WebConfig.Mask(value)} — read with XDocument from Application.StartupPath, no System.Configuration reference, the password never reaches the browser.", Ui.BannerKind.Ok);
            Ui.SetStatus(labelStatus, "connection string read from Web.config", Ui.Ok);
        }

        #endregion

        #region The ported OrdersForm, opened as a floating window

        private void buttonOpenOrdersForm_Click(object sender, EventArgs e)
        {
            trace.Add(TraceKind.FromClient, "open OrdersForm", "the form ported by the namespace swap — still a Form");

            if (_ordersForm != null && !_ordersForm.IsDisposed)
            {
                _ordersForm.BringToFront();
                trace.Add(TraceKind.ToClient, "OrdersForm.BringToFront()", "already open — one instance per session, brought to front");
                Ui.SetStatus(labelStatus, "OrdersForm already open", Ui.Ok);
                return;
            }

            var form = new OrdersForm();
            // Every commented desktop call in the ported form raises BoundaryHit through its one-line stand-in.
            form.BoundaryHit += (name, message) => trace.Add(TraceKind.Boundary, name, message);
            form.FormClosed += (s, args) =>
            {
                // A form shown with Show() is disposed by Wisej.NET when it closes; guard so the caller's Dispose stays idempotent.
                if (!form.IsDisposed) form.Dispose();
                _ordersForm = null;
                trace.Add(TraceKind.Server, "OrdersForm.FormClosed", "closed and disposed — the Module 3 rule: the caller owns the lifetime");
                Ui.SetStatus(labelStatus, "OrdersForm closed", Ui.Ok);
            };
            _ordersForm = form;

            // Non-modal Show(): a Wisej.Web.Form floats over the page (StartPosition stays the designer's CenterScreen).
            trace.Add(TraceKind.ToClient, "OrdersForm.Show()", "floating window over the page — MenuBar, StatusBar, DataGridView bound to OrderService.Search, same handlers");
            form.Show();
            Ui.HideBanner(labelBanner);
            Ui.SetStatus(labelStatus, "OrdersForm running in the browser — a Form, not a Page", Ui.Ok);
        }

        #endregion

        private void buttonClear_Click(object sender, EventArgs e) => trace.Clear();

        private static string PassTrail() =>
            string.Join(" → ", Enumerable.Range(1, CompilerErrorLog.PassCount).Select(p => CompilerErrorLog.ErrorsAtPass(p)));

        private static int OpenItems() => CompilerErrorLog.Entries.Count(x => x.Remaining);

        private static string Or(string value, string fallback) => string.IsNullOrEmpty(value) ? fallback : value;

        private static string Short(string id) => string.IsNullOrEmpty(id) ? "?" : (id.Length > 8 ? id.Substring(0, 8) : id);
    }
}
