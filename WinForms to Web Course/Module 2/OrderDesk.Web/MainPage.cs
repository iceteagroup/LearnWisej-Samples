using System;
using System.Collections.Generic;
using System.IO;
using OrderDesk.Pages;
using OrderDesk.Services;
using OrderDesk.Shared;
using Wisej.Web;
using LegacyAppConfig = OrderDesk.Legacy.AppConfig;

namespace OrderDesk
{
    /// <summary>
    /// Module 2 — Project conversion: the Wisej.NET shell. The lab page hosts the first ported
    /// form (Pages/OrdersPage), shows the shell files with their live values, keeps the
    /// compiler-error log of the port and exercises the App.config → Web.config move.
    /// Buttons: Open OrdersPage ✓ (success) · Replay conversion (progress, Timer) ·
    /// App.config lookup ✕ (failure) · Web.config lookup ✓ (recovery).
    /// </summary>
    public partial class MainPage : Page
    {
        private OrdersPage _ordersPage;
        private readonly List<Label> _shellValues = new List<Label>();
        private readonly List<Action> _replaySteps = new List<Action>();
        private int _replayIndex;

        public MainPage()
        {
            InitializeComponent();
            BuildShellRows();
            BuildReplaySteps();
        }

        // ───────────────────────── startup ─────────────────────────

        private void MainPage_Load(object sender, EventArgs e)
        {
            var sid = ShortSession();
            labelSession.Text = "session " + sid + " · " + ShellAnatomy.TargetFramework + " · Wisej.Framework " + ShellAnatomy.WisejVersion;

            trace.Server("Program.Main", "Application.MainPage = new MainPage()  · session " + sid);
            trace.Server("Startup.cs", "Kestrel owns the process · app.UseWisej() · no EnableVisualStyles, no Application.Run");
            trace.Server("Default.json", "\"startup\": \"OrderDesk.Program.Main, OrderDesk\" — the method that replaced Application.Run");
            RefreshShellAnatomy(log: true);
            trace.Finding("shell", "OrderDesk.Web = Wisej-4 4.1.0 · net10.0-windows;net10.0 · Path B: clean shell, forms moved in one at a time");
            SetStatus(StatusKind.Idle);
        }

        // ───────────────────────── success path ─────────────────────────

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            trace.In("click", "Open OrdersPage ✓");
            OpenOrdersPage();
        }

        private void OpenOrdersPage()
        {
            SetStatus(StatusKind.Working);
            HideBanner();

            if (_ordersPage == null)
            {
                trace.Finding("namespace swap", "System.Windows.Forms → Wisej.Web · OrdersForm : Form → OrdersPage : UserControl (hosted; : Page when it is the main view)");
                trace.Server("new OrdersPage()", "InitializeComponent() — the migrated .Designer.cs: MenuBar, DataGridView ×4 columns, detail Panel, StatusBar");
                _ordersPage = new OrdersPage { Dock = DockStyle.Fill };
                _ordersPage.Activity += (name, payload) => trace.Server("OrdersPage." + name, payload);
                labelOrdersPlaceholder.Visible = false;
                panelOrdersHost.Controls.Add(_ordersPage);   // Load fires here → ReloadGrid()
            }
            else
            {
                trace.Server("OrdersPage.ReloadGrid()", "already open — reloading (idempotent)");
                _ordersPage.ReloadGrid();
            }

            trace.Out("ordersGrid.DataSource", "List<Order> ×" + _ordersPage.RowCount + " → DataGridView · rows stream to the browser in blocks (the full-table habit is the Module 5 problem)");
            trace.Ok("OrdersPage", "renders in the browser · MenuBar File/Edit/View/Reports/Help · StatusBar 'Ready · " + _ordersPage.RowCount + " orders · single-user desktop'");
            trace.Finding("parity, not done", "compiles + renders ≠ migrated: statics (M4), full-table grid (M5), C:\\Orders / Interop / printer (M6) still there");
            ShowBanner("✓ OrdersPage open — the first form runs in the browser: same OrderService, same handlers, MessageBox.Show still works (try Help → About).", BannerKind.Ok);
            SetStatus(StatusKind.Idle);
        }

        // ───────────────────────── progress path (Timer) ─────────────────────────

        private void buttonReplay_Click(object sender, EventArgs e)
        {
            trace.In("click", "Replay conversion");
            timerReplay.Stop();
            gridErrors.Rows.Clear();
            labelErrorsCount.Text = "building…";
            labelErrorsCount.ForeColor = Palette.MutedText;
            HideBanner();
            SetStatus(StatusKind.Working);
            _replayIndex = 0;
            trace.Server("replay", "Lab 2 · " + _replaySteps.Count + " steps · Wisej.Web.Timer every " + timerReplay.Interval + " ms");
            timerReplay.Start();
        }

        private void timerReplay_Tick(object sender, EventArgs e)
        {
            if (_replayIndex >= _replaySteps.Count)
            {
                timerReplay.Stop();
                return;
            }
            _replaySteps[_replayIndex++]();
            if (_replayIndex >= _replaySteps.Count)
                timerReplay.Stop();
        }

        private void BuildReplaySteps()
        {
            var cats = CompilerErrorLog.Categories;
            _replaySteps.Clear();
            _replaySteps.Add(() => trace.Server("git", "switch -c migration/m2-shell · tag legacy-3.2 = the backup — the desktop build stays intact"));
            _replaySteps.Add(() => trace.Server("dotnet new", "Wisej-4 4.1.0 web app → OrderDesk.Web · <TargetFrameworks>net10.0-windows;net10.0</TargetFrameworks> · Path B: clean shell"));
            _replaySteps.Add(() => trace.Server("copy", "OrdersForm.cs + OrdersForm.Designer.cs → Pages/OrdersPage.cs + .Designer.cs · Domain/*.cs already shared"));
            _replaySteps.Add(() =>
            {
                trace.Finding("namespace swap", cats[0].Example + "  →  " + cats[0].Fix);
                AddErrorRow(cats[0]);
            });
            _replaySteps.Add(() =>
            {
                trace.Fail("dotnet build", CompilerErrorLog.FirstBuildErrors + " errors (distinct fixes; every designer line on a renamed type fails on its own — the video's raw funnel is 38 → 0)");
                SetErrorCount(CompilerErrorLog.FirstBuildErrors + " errors", Palette.Bad);
            });
            for (int i = 1; i < cats.Count; i++)
            {
                var c = cats[i];
                _replaySteps.Add(() =>
                {
                    trace.Finding("classify · " + c.Category.ToLowerInvariant(), c.Count + " — " + c.Example + "  →  " + c.Fix + "  [" + c.VideoBucket + "]");
                    AddErrorRow(c);
                });
            }
            _replaySteps.Add(() =>
            {
                trace.Server("fix direct differences", "rename 5 types · delete 3 designer-only properties (StartPosition, MainMenuStrip, FixedSingle → Solid)");
                trace.Fail("dotnet build", CompilerErrorLog.AfterDirectFixes + " errors left — the ones that need a decision, not a rename");
                SetErrorCount(CompilerErrorLog.AfterDirectFixes + " errors", Palette.Warn);
            });
            _replaySteps.Add(() =>
            {
                trace.Server("fix the rest", "Items/DropDownItems → MenuItems · statusStrip.Items → Panels · Close() → session ends with the tab · Program.Main(NameValueCollection) + Default.json startup");
                trace.Ok("dotnet build", "0 errors · 0 warnings · net10.0-windows + net10.0");
                SetErrorCount("0 errors ✓", Palette.Good);
            });
            _replaySteps.Add(() =>
            {
                trace.Ok("dotnet run", "http://localhost:5102 → Program.Main → MainPage → OrdersPage renders in the browser");
                trace.Finding("business rule preserved", "OrderService.GetAll / Save, OrderCalculator, OrderValidator untouched — 0 edits in Domain/*.cs");
                OpenOrdersPage();
                ShowBanner("✓ Replay done: branch → clean shell → OrdersForm copied → namespace swap → 14 → 6 → 0 errors → runs. Compiling is not \"done\": M4 statics, M5 grid, M6 files.", BannerKind.Ok);
            });
        }

        private void AddErrorRow(ErrorCategory c)
        {
            gridErrors.Rows.Add(c.Category, c.Example, c.Fix, c.Count);
        }

        private void SetErrorCount(string text, System.Drawing.Color color)
        {
            labelErrorsCount.Text = text;
            labelErrorsCount.ForeColor = color;
        }

        // ───────────────────────── failure path ─────────────────────────

        private void buttonAppConfig_Click(object sender, EventArgs e)
        {
            trace.In("click", "App.config lookup ✕");
            SetStatus(StatusKind.Working);
            HideBanner();
            trace.Server("Legacy.AppConfig", "ConfigurationManager-style: read <entry assembly>.config next to the exe — on the desktop " + LegacyAppConfig.DesktopConfigPath);
            trace.Server("Legacy.AppConfig", "on this server the entry assembly is " + Path.GetFileName(LegacyAppConfig.ServerConfigPath).Replace(".config", "") + " → looking for " + LegacyAppConfig.ServerConfigPath);
            try
            {
                var folder = LegacyAppConfig.GetAppSetting("ExportFolder");
                var cs = LegacyAppConfig.GetConnectionString("OrderDesk");
                // Only reachable if someone drops an OrderDesk.dll.config into bin/ — still the wrong place for web configuration.
                trace.Fail("App.config", "found next to the assembly (ExportFolder=" + folder + ", OrderDesk=" + cs + ") — a desktop file in a server folder; not versioned, not per environment");
                ShowBanner("✖ App.config found next to the server assembly — configuration moved to Web.config; delete the copy.", BannerKind.Fail);
            }
            catch (FileNotFoundException ex)
            {
                trace.Fail("FileNotFoundException", ex.Message);
                trace.Fail("App.config", "ExportFolder = C:\\Orders and connectionStrings[OrderDesk] → LAN-SQL01 were never deployed: they lived in the user's Program Files");
                trace.Finding("config", "App.config → Web.config: appSettings (OrderDesk.StorageRoot replaces ExportFolder) + connectionStrings · read via Services/AppConfig (System.Xml.Linq)");
                ShowBanner("✖ configuration moved to Web.config — " + Path.GetFileName(LegacyAppConfig.ServerConfigPath) + " is not next to the server assembly (there is no exe on the web).", BannerKind.Fail);
            }
            catch (Exception ex)
            {
                trace.Fail(ex.GetType().Name, ex.Message);
                ShowBanner("✖ configuration moved to Web.config — " + ex.Message, BannerKind.Fail);
            }
            SetStatus(StatusKind.Alarm);
        }

        // ───────────────────────── recovery ─────────────────────────

        private void buttonWebConfig_Click(object sender, EventArgs e)
        {
            trace.In("click", "Web.config lookup ✓");
            SetStatus(StatusKind.Working);
            HideBanner();
            try
            {
                var path = AppConfig.ResolvePath();
                trace.Server("Services.AppConfig.Load", "XDocument.Load(" + path + ")  · Application.StartupPath = " + SafeStartupPath());
                var cfg = AppConfig.Load();
                trace.Ok("appSettings", "OrderDesk.StorageRoot = " + cfg.StorageRoot + " · Wisej.DefaultTheme = " + cfg.DefaultTheme + " · Wisej.LicenseKey = " + (string.IsNullOrEmpty(cfg.Get("Wisej.LicenseKey")) ? "(empty)" : "(set)"));
                var cs = cfg.Connection("OrderDesk");
                if (cs != null)
                    trace.Ok("connectionStrings", cs.Name + " → " + cs.ConnectionString + " (" + cs.ProviderName + ")");
                else
                    trace.Fail("connectionStrings", "no entry named OrderDesk");
                trace.Ok("storage root", cfg.StorageRootPath + " (relative to the web root; created on demand in Module 6)");
                trace.Finding("config", "ExportFolder=C:\\Orders → OrderDesk.StorageRoot=" + cfg.StorageRoot + " · LAN-SQL01 → " + (cs?.Server ?? "?") + " · one file per deployment, no per-user copy");
                ShowBanner("✓ Web.config: OrderDesk.StorageRoot = " + cfg.StorageRoot + " · connectionStrings[" + (cs?.Name ?? "?") + "] → " + (cs?.Server ?? "?") + " · theme " + cfg.DefaultTheme + " (read with System.Xml.Linq).", BannerKind.Ok);
                RefreshShellAnatomy(log: false);
                SetStatus(StatusKind.Idle);
            }
            catch (Exception ex)
            {
                trace.Fail(ex.GetType().Name, ex.Message);
                ShowBanner("✖ Web.config could not be read: " + ex.Message, BannerKind.Fail);
                SetStatus(StatusKind.Alarm);
            }
        }

        // ───────────────────────── shell anatomy card ─────────────────────────

        private void BuildShellRows()
        {
            _shellValues.Clear();
            panelShellRows.Controls.Clear();
            var parts = ShellAnatomy.Read();
            int y = 4;
            foreach (var part in parts)
            {
                var key = new Label
                {
                    AutoSize = false,
                    Location = new System.Drawing.Point(12, y),
                    Size = new System.Drawing.Size(304, 14),
                    Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold),
                    ForeColor = Palette.MutedText,
                    Text = part.File + "  ·  " + part.Role,
                    TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                    AutoEllipsis = true,
                };
                var value = new Label
                {
                    AutoSize = false,
                    Location = new System.Drawing.Point(12, y + 14),
                    Size = new System.Drawing.Size(304, 18),
                    Font = new System.Drawing.Font("monospace", 8.5F),
                    ForeColor = Palette.Ink,
                    Text = part.LiveValue,
                    ToolTipText = part.LiveValue,
                    TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                    AutoEllipsis = true,
                };
                panelShellRows.Controls.Add(key);
                panelShellRows.Controls.Add(value);
                _shellValues.Add(value);
                y += 33;
            }
        }

        private void RefreshShellAnatomy(bool log)
        {
            var parts = ShellAnatomy.Read();
            for (int i = 0; i < parts.Count && i < _shellValues.Count; i++)
            {
                _shellValues[i].Text = parts[i].LiveValue;
                _shellValues[i].ToolTipText = parts[i].LiveValue;
                if (log) trace.Server(parts[i].File, parts[i].LiveValue);
            }
        }

        // ───────────────────────── status + banner ─────────────────────────

        private enum StatusKind { Idle, Working, Alarm }
        private enum BannerKind { Ok, Warn, Fail }

        private void SetStatus(StatusKind kind)
        {
            switch (kind)
            {
                case StatusKind.Working:
                    labelStatus.Text = "● working";
                    labelStatus.ForeColor = Palette.Warn;
                    break;
                case StatusKind.Alarm:
                    labelStatus.Text = "● alarm";
                    labelStatus.ForeColor = Palette.Bad;
                    break;
                default:
                    labelStatus.Text = "● idle";
                    labelStatus.ForeColor = Palette.Good;
                    break;
            }
        }

        private void ShowBanner(string text, BannerKind kind)
        {
            switch (kind)
            {
                case BannerKind.Fail:
                    labelBanner.BackColor = Palette.BadSoft;
                    labelBanner.ForeColor = Palette.Bad;
                    break;
                case BannerKind.Warn:
                    labelBanner.BackColor = Palette.WarnSoft;
                    labelBanner.ForeColor = Palette.Warn;
                    break;
                default:
                    labelBanner.BackColor = Palette.GoodSoft;
                    labelBanner.ForeColor = Palette.Good;
                    break;
            }
            labelBanner.Text = text;
            labelBanner.ToolTipText = text;
            labelBanner.Visible = true;
        }

        private void HideBanner() => labelBanner.Visible = false;

        private static string ShortSession()
        {
            try
            {
                var id = Application.SessionId ?? "";
                return id.Length > 8 ? id.Substring(0, 8) : id;
            }
            catch { return "?"; }
        }

        private static string SafeStartupPath()
        {
            try { return Application.StartupPath; } catch (Exception ex) { return "(" + ex.GetType().Name + ")"; }
        }
    }
}
