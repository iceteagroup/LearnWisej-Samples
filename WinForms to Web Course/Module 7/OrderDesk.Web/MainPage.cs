using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using OrderDesk.Domain;
using OrderDesk.Pages;
using OrderDesk.Services;
using OrderDesk.Shared;
using Wisej.Core;
using Wisej.Web;

namespace OrderDesk
{
    /// <summary>
    /// Module 7 — the capstone: OrderDesk · Operations. The migrated app after parity, now modernized
    /// (theme switch, responsive client profiles), secured (AllowHtml review, guarded downloads, session
    /// lifecycle, audit log) and deployable (/health, readiness checklist, configuration with environment
    /// overrides). Left = nav rail + views (Dashboard · Orders · Customers · Reports · Settings), right = the
    /// migration trace. The bottom bar exercises the lab paths:
    ///
    ///   Refresh dashboard ✓   success   DashboardMetrics.Compute → KPI cards, chart, recent grid + audit entry
    ///   Simulate activity     progress  Application.StartTask plays sam's session B into the feed with Application.Update(page)
    ///   Responsive check      success   Application.ActiveProfile / Browser.Size logged, layout re-applied
    ///   Guarded download      failure ✕ kelly (Clerk) → 403 customers-with-taxid.csv needs Manager
    ///                         recovery ✓ dana (Manager) → Application.Download
    ///   Health check          success   the /health JSON built in-process (open /health in a tab for the real probe)
    ///   Readiness check ✓     success   the automated part of the final checklist (N/8)
    ///
    /// The Settings view holds the Security card (Render encoded ✓ / Render AllowHtml ✕ / Render sanitized ✓)
    /// and the Deployment card (checklist, /health, Show config); Preview phone/tablet/desktop force a layout.
    /// </summary>
    public partial class MainPage : Page
    {
        /// <summary>What a careless import put into order 1038's Notes: formatting plus an event-handler payload.</summary>
        public const string NotesPayload = "<b>Rush</b> <img src=x onerror=\"document.title='pwned'\">";

        private readonly OrderService _service = new OrderService();
        private readonly string _sessionId;
        private readonly HashSet<LayoutMode> _modesSeen = new HashSet<LayoutMode>();
        private LayoutMode _mode = LayoutMode.Desktop;
        private LayoutMode? _forcedMode;
        private string _currentTheme = "Bootstrap-4";
        private string _currentView = "Dashboard";
        private bool _simulating;
        private bool _sanitizedShown;
        private bool _healthChecked;
        private bool _configShown;
        private bool _profileEventSeen;
        private readonly string[] _checkNotes = new string[8];
        private readonly bool[] _checks = new bool[8];

        public MainPage()
        {
            InitializeComponent();
            _sessionId = Application.SessionId;

            // Security card tool buttons (SettingsView exposes them; behaviour lives here).
            settingsView.ButtonRenderEncoded.Click += buttonRenderEncoded_Click;
            settingsView.ButtonRenderAllowHtml.Click += buttonRenderAllowHtml_Click;
            settingsView.ButtonRenderSanitized.Click += buttonRenderSanitized_Click;
            settingsView.ButtonPreviewPhone.Click += (s, e) => ForceLayout(LayoutMode.Phone);
            settingsView.ButtonPreviewTablet.Click += (s, e) => ForceLayout(LayoutMode.Tablet);
            settingsView.ButtonPreviewDesktop.Click += (s, e) => ForceLayout(LayoutMode.Desktop);
            settingsView.ButtonAuto.Click += (s, e) => { _forcedMode = null; trace.In("Auto", "preview off — following Application.ActiveProfile again"); ApplyLayout(ModeFor(ProfileName()), "Auto"); };
            settingsView.ButtonShowConfig.Click += buttonShowConfig_Click;
            settingsView.HealthLink.LinkClicked += (s, e) => OpenHealthTab();

            // Process-wide feed and session-wide responsive events (unsubscribed in Dispose).
            AuditLog.Added += this.AuditLog_Added;
            Application.ResponsiveProfileChanged += this.Application_ResponsiveProfileChanged;
            Application.BrowserSizeChanged += this.Application_BrowserSizeChanged;
        }

        // ------------------------------------------------------------------ startup ------------------------

        private void MainPage_Load(object sender, EventArgs e)
        {
            trace.Server("Program.Main", "Application.MainPage = new MainPage()  · session " + Short(_sessionId) + " · sessions " + Application.SessionCount);
            trace.Server("Default.json", "startup OrderDesk.Program.Main · theme " + _currentTheme + " · Web.config theme " + (AppConfig.Get("Wisej.DefaultTheme") ?? "-"));
            trace.Server("Startup.cs", "app.UseWisej() · app.MapGet(\"/health\") · UseFileServer (never *.json)");

            // The payload the security review is about: user-entered notes on the shared order 1038 (idempotent).
            var o1038 = _service.Find(1038);
            if (o1038 != null && o1038.Notes != NotesPayload)
            {
                o1038.Notes = NotesPayload;
                trace.Finding("order 1038 Notes", "user-entered text now contains markup + an onerror handler — the AllowHtml review target");
            }
            settingsView.RawNotes.Text = o1038?.Notes ?? "";

            // Users: this session starts as kelly (Clerk). Per session, in Application.Session — never a static.
            comboUser.SelectedIndex = 0;
            comboTheme.SelectedIndex = 0;
            UserContext.Current.User = UserContext.KnownUsers[0];
            trace.Server("UserContext.Current", "kelly · Clerk · Application.Session (Module 4 pattern)");

            // Session lifecycle (Program.Main subscribed the events; here we only report the configuration).
            int timeout = -1;
            try { timeout = Application.Configuration.SessionTimeout; } catch { }
            trace.Server("session lifecycle", "Application.SessionTimeout + ApplicationExit subscribed in Program.Main · Configuration.SessionTimeout = " + (timeout < 0 ? "?" : timeout + " s") + " · IsWebSocket " + Application.IsWebSocket);
            settingsView.SessionLine.Text = "Session " + Short(_sessionId) + " · SessionTimeout/ApplicationExit audit-logged (Program.Main) · timeout " + (timeout < 0 ? "?" : timeout + " s") + " · secure " + Application.Browser.IsSecure;

            // Responsive: what the framework loaded and what it matched for this browser.
            LogClientProfiles();
            string profile = ProfileName();
            trace.Server("Application.ActiveProfile", "\"" + profile + "\" · browser " + SizeText(Application.Browser.Size) + " · device " + Application.Browser.Device + " · screen " + SizeText(Application.Browser.ScreenSize));
            ApplyLayout(ModeFor(profile), "MainPage_Load");
            settingsView.ShowTheme("Theme: " + _currentTheme + " (Default.json) — switch in the app bar: Application.LoadTheme(name)");

            // Data.
            RefreshDashboard("MainPage_Load", audit: false);
            ordersView.Load(_service, 200);
            trace.Server("OrderService.Search", "take 200 of " + _service.Store.Count + " → Orders view (server-side query, Module 5)");
            customersView.Load(_service.Customers);
            trace.Server("OrderService.Customers", _service.Customers.Count + " rows · TaxId not bound (sensitive)");
            dashboardView.LoadActivity(AuditLog.Recent(50));
            trace.Server("AuditLog.Recent(50)", AuditLog.Count + " entries process-wide · feed subscribed (AuditLog.Added)");

            // Checklist item 1 is proven by being here.
            SetCheck(0, true, "Default.json startup → Program.Main → MainPage");
            AuditLog.Add(UserContext.Current.UserName, _sessionId, "opened OrderDesk · Operations (session " + Short(_sessionId) + ")");
            SetStatus("idle");
            trace.Ok("MainPage_Load", "dashboard ready · " + dashboardView.DescribeLayout());
        }

        private void LogClientProfiles()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "ClientProfiles.json");
            if (!File.Exists(path))
            {
                trace.Fail("ClientProfiles.json", "✖ not found next to the assembly (" + path + ") — the csproj copies it with PreserveNewest");
                return;
            }
            try
            {
                using (var doc = System.Text.Json.JsonDocument.Parse(File.ReadAllText(path)))
                {
                    var names = new List<string>();
                    foreach (var p in doc.RootElement.GetProperty("profiles").EnumerateArray())
                    {
                        string n = p.GetProperty("name").GetString();
                        string min = p.TryGetProperty("minWidth", out var mn) ? mn.GetInt32().ToString() : "0";
                        string max = p.TryGetProperty("maxWidth", out var mx) ? mx.GetInt32().ToString() : "∞";
                        names.Add(n + " [" + min + "–" + max + "]");
                    }
                    trace.Server("ClientProfiles.json", string.Join(" · ", names) + " · merged with the built-in Phone (Landscape), Tablet (Landscape), Small Desktop · loaded from bin");
                }
            }
            catch (Exception ex)
            {
                trace.Fail("ClientProfiles.json", ex.Message);
            }
        }

        // ------------------------------------------------------------------ navigation ---------------------

        private void buttonNav_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            ShowView((string)button.Tag, "nav click");
        }

        private void ShowView(string name, string reason)
        {
            _currentView = name;
            dashboardView.Visible = name == "Dashboard";
            ordersView.Visible = name == "Orders";
            customersView.Visible = name == "Customers";
            reportsView.Visible = name == "Reports";
            settingsView.Visible = name == "Settings";
            foreach (var b in new[] { buttonNavDashboard, buttonNavOrders, buttonNavCustomers, buttonNavReports, buttonNavSettings })
            {
                bool active = (string)b.Tag == name;
                b.BackColor = active ? Palette.AccentSoft : System.Drawing.Color.Empty;
                b.ForeColor = active ? Palette.Accent : System.Drawing.Color.Empty;
            }
            trace.In("nav → " + name, reason);
        }

        // ------------------------------------------------------------------ dashboard -----------------------

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            trace.In("Refresh dashboard ✓", "click");
            ShowView("Dashboard", "Refresh dashboard");
            RefreshDashboard("button", audit: true);
            Notify.Saved("Dashboard refreshed.");
        }

        private void RefreshDashboard(string reason, bool audit)
        {
            SetStatus("working");
            var m = DashboardMetrics.Compute(_service);
            trace.Server("OrderService.GetAll()", m.TotalOrders + " orders · business date " + m.BusinessDate.ToString("yyyy-MM-dd"));
            trace.Server("OrderService.CountByStatus()", "Open " + m.Count(OrderStatus.Open) + " · In progress " + m.Count(OrderStatus.InProgress) + " · Shipped " + m.Count(OrderStatus.Shipped) + " · Invoiced " + m.Count(OrderStatus.Invoiced) + " · Hold " + m.Count(OrderStatus.Hold));
            dashboardView.ShowMetrics(m);
            var recent = _service.Search(new OrderQuery { Take = 5 });
            dashboardView.ShowRecent(recent);
            trace.Out("KPI cards", "Open orders " + m.OpenOrders + " (+" + m.OrdersToday + " today) · Revenue today " + m.RevenueToday.ToString("C2", ViewBase.Us) + " · Invoiced " + m.Invoiced + " · On-time " + m.OnTimePercent.ToString("0.0", ViewBase.Us) + "% (SLA 95%)");
            trace.Out("Recent orders", string.Join(" · ", recent.Select(o => o.Id + " " + o.CustomerName + " " + o.Total.ToString("C2", ViewBase.Us) + " " + ViewBase.StatusText(o.Status))));
            if (audit)
            {
                AuditLog.Add(UserContext.Current.UserName, _sessionId, "refreshed the dashboard (" + m.OpenOrders + " open · " + m.RevenueToday.ToString("C0", ViewBase.Us) + " today)");
                trace.Ok("dashboard", "KPIs, chart and recent orders recomputed from OrderService — same C# the WinForms app called");
            }
            SetStatus("idle");
        }

        // ------------------------------------------------------------------ activity feed -------------------

        /// <summary>
        /// Called on the WRITER's thread for every audit entry, in every open page of every session.
        /// Own session: the entry is appended directly (the request or StartTask that wrote it will push it).
        /// Another session: hop into this page's context with Application.StartTask + Application.Update(page, …)
        /// so kelly's tab shows what sam did the moment he did it.
        /// </summary>
        private void AuditLog_Added(AuditEntry entry)
        {
            if (this.IsDisposed) return;
            string writer = null;
            try { writer = Application.SessionId; } catch { }
            if (writer == _sessionId)
            {
                dashboardView.AppendActivity(entry);
                return;
            }
            var page = this;
            Application.StartTask(() =>
            {
                if (page.IsDisposed) return;
                Application.Update(page, () =>
                {
                    if (page.IsDisposed) return;
                    page.dashboardView.AppendActivity(entry);
                    page.trace.Out("Application.Update(page)", "activity from session " + entry.ShortSession + " pushed into this tab: " + entry.User + " " + entry.Action);
                });
            });
        }

        private void buttonSimulate_Click(object sender, EventArgs e)
        {
            trace.In("Simulate activity", "click");
            if (_simulating)
            {
                trace.Server("Simulate activity", "already running — ignored (idempotent)");
                return;
            }
            ShowView("Dashboard", "Simulate activity");
            _simulating = true;
            SetStatus("working 0/5");
            trace.Server("Application.StartTask", "session B (sam) plays 5 actions, 700 ms apart; each AuditLog.Add → feed → Application.Update(page)");

            var steps = new[]
            {
                "signed in (Globex · Manager)",
                "opened Orders · filter Invoiced",
                "viewed order 1041 Contoso Ltd $1,290.50",
                "downloaded orders.xlsx (managed XlsxWriter)",
                "signed out",
            };
            var page = this;
            Application.StartTask(() =>
            {
                try
                {
                    for (int i = 0; i < steps.Length; i++)
                    {
                        Thread.Sleep(700);
                        if (page.IsDisposed) return;
                        var entry = AuditLog.Add("sam", "B-simulated-" + Short(_sessionId), steps[i]);
                        int done = i + 1;
                        Application.Update(page, () =>
                        {
                            page.SetStatus("working " + done + "/5");
                            page.trace.Out("Application.Update(page)", "feed +1 · " + entry);
                        });
                    }
                    Application.Update(page, () =>
                    {
                        page._simulating = false;
                        page.SetStatus("idle");
                        page.trace.Ok("Simulate activity", "5 entries from another session appeared without a click — the feed is multi-user (open a second tab to see the real thing)");
                    });
                }
                catch (Exception ex)
                {
                    Application.Update(page, () =>
                    {
                        page._simulating = false;
                        page.SetStatus("alarm");
                        page.trace.Fail("Simulate activity", ex.GetType().Name + ": " + ex.Message);
                    });
                }
            });
        }

        // ------------------------------------------------------------------ responsive ----------------------

        private void Application_ResponsiveProfileChanged(object sender, ResponsiveProfileChangedEventArgs e)
        {
            _profileEventSeen = true;
            string prev = e.PreviousProfile?.Name ?? "(none)";
            string cur = e.CurrentProfile?.Name ?? ProfileName();
            trace.In("Application.ResponsiveProfileChanged", "\"" + prev + "\" → \"" + cur + "\" · Application.Browser.Size " + SizeText(Application.Browser.Size));
            if (_forcedMode.HasValue)
            {
                trace.Server("preview active", "profile change logged but not applied — click Auto to follow the browser again");
                return;
            }
            ApplyLayout(ModeFor(cur), "Application.ResponsiveProfileChanged");
        }

        private void Application_BrowserSizeChanged(object sender, EventArgs e)
        {
            labelProfile.Text = "profile " + ProfileName() + " · " + SizeText(Application.Browser.Size);
            settingsView.ShowProfile("Profile: " + ProfileName() + " · browser " + SizeText(Application.Browser.Size) + " · device " + Application.Browser.Device + (_forcedMode.HasValue ? " · preview " + _forcedMode : ""));
        }

        private void buttonResponsive_Click(object sender, EventArgs e)
        {
            trace.In("Responsive check", "click");
            string profile = ProfileName();
            trace.Server("Application.ActiveProfile.Name", "\"" + profile + "\" · Application.Browser.Size " + SizeText(Application.Browser.Size) + " · device " + Application.Browser.Device + " · screen " + SizeText(Application.Browser.ScreenSize) + " · ResponsiveProfileChanged seen: " + (_profileEventSeen ? "yes" : "not yet — resize the pane below 1024 / 600 px"));
            if (_forcedMode.HasValue)
                trace.Server("preview", "layout forced to " + _forcedMode + " — Auto returns to the profile");
            ApplyLayout(_forcedMode ?? ModeFor(profile), "Responsive check");
            SetCheck(6, _modesSeen.Count == 3, "seen " + string.Join(", ", _modesSeen.OrderBy(m => (int)m)) + (_modesSeen.Count == 3 ? "" : " — preview or resize to the missing size(s)"));
        }

        private void ForceLayout(LayoutMode mode)
        {
            _forcedMode = mode;
            trace.In("Preview " + mode.ToString().ToLowerInvariant(), "layout forced (the browser is still " + SizeText(Application.Browser.Size) + ", profile \"" + ProfileName() + "\")");
            ApplyLayout(mode, "Preview " + mode);
            SetCheck(6, _modesSeen.Count == 3, "seen " + string.Join(", ", _modesSeen.OrderBy(m => (int)m)));
        }

        /// <summary>Profile name → layout family. Small Desktop (a built-in profile that stays in the merged list) behaves like Tablet.</summary>
        internal static LayoutMode ModeFor(string profileName)
        {
            if (string.IsNullOrEmpty(profileName)) return LayoutMode.Desktop;
            if (profileName.StartsWith("Phone", StringComparison.OrdinalIgnoreCase)) return LayoutMode.Phone;
            if (profileName.StartsWith("Tablet", StringComparison.OrdinalIgnoreCase) || profileName.StartsWith("Small Desktop", StringComparison.OrdinalIgnoreCase)) return LayoutMode.Tablet;
            return LayoutMode.Desktop;
        }

        /// <summary>
        /// The responsive properties, in code: one method, one table (docs/client-profiles.md).
        /// Desktop: rail with labels 150 px · trace 560 · KPI 4×1 · Owner visible · app-bar chips visible.
        /// Tablet:  rail icon-only 56 px · trace 300 · KPI 2×2 · Owner hidden.
        /// Phone:   rail icon-only · trace hidden · KPI 2×2, chart over grid · Owner, Date, PO hidden · app-bar chips hidden.
        /// </summary>
        private void ApplyLayout(LayoutMode mode, string reason)
        {
            _mode = mode;
            _modesSeen.Add(mode);
            bool labels = mode == LayoutMode.Desktop;

            panelNav.Width = labels ? 150 : 56;
            SetNavText(buttonNavDashboard, "⌂", "Dashboard", labels);
            SetNavText(buttonNavOrders, "≡", "Orders", labels);
            SetNavText(buttonNavCustomers, "☺", "Customers", labels);
            SetNavText(buttonNavReports, "▤", "Reports", labels);
            SetNavText(buttonNavSettings, "⚙", "Settings", labels);

            trace.Visible = mode != LayoutMode.Phone;
            trace.Width = mode == LayoutMode.Desktop ? 560 : 300;

            comboTheme.Visible = mode != LayoutMode.Phone;
            comboUser.Visible = mode != LayoutMode.Phone;
            labelProfile.Visible = mode != LayoutMode.Phone;

            dashboardView.ApplyLayout(mode);
            ordersView.ApplyLayout(mode);
            customersView.ApplyLayout(mode);
            reportsView.ApplyLayout(mode);
            settingsView.ApplyLayout(mode);

            string profile = ProfileName();
            labelProfile.Text = "profile " + profile + " · " + SizeText(Application.Browser.Size) + (_forcedMode.HasValue ? " · preview" : "");
            settingsView.ShowProfile("Profile: " + profile + " · browser " + SizeText(Application.Browser.Size) + " · device " + Application.Browser.Device + " · layout " + mode + (_forcedMode.HasValue ? " (preview)" : ""));
            trace.Out("ApplyLayout(" + mode + ")", reason + " · nav " + (labels ? "labels 150" : "icons 56") + " · trace " + (trace.Visible ? trace.Width + " px" : "hidden") + " · " + dashboardView.DescribeLayout());
        }

        private static void SetNavText(Button b, string glyph, string label, bool withLabel)
        {
            b.Text = withLabel ? glyph + "  " + label : glyph;
            b.Width = withLabel ? 132 : 40;
            b.TextAlign = withLabel ? System.Drawing.ContentAlignment.MiddleLeft : System.Drawing.ContentAlignment.MiddleCenter;
            b.ToolTipText = label;
        }

        // ------------------------------------------------------------------ theme / user --------------------

        private void comboTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            var name = comboTheme.SelectedItem as string;
            if (string.IsNullOrEmpty(name) || name == _currentTheme) return;
            trace.In("theme → " + name, "ComboBox.SelectedIndexChanged");
            try
            {
                Application.LoadTheme(name);
                string loaded = null;
                try { loaded = Application.Theme?.Name; } catch { }
                _currentTheme = name;
                settingsView.ShowTheme("Theme: " + name + " — Application.LoadTheme(\"" + name + "\") · Application.Theme.Name = " + (loaded ?? "?"));
                trace.Ok("Application.LoadTheme", "\"" + name + "\" applied live · Application.Theme.Name = " + (loaded ?? "?") + " · no form rewritten, OrderService untouched");
                AuditLog.Add(UserContext.Current.UserName, _sessionId, "switched theme to " + name);
                HideBanner();
            }
            catch (Exception ex)
            {
                trace.Fail("Application.LoadTheme", ex.GetType().Name + ": " + ex.Message);
                ShowBanner("✖ Theme \"" + name + "\" could not be loaded — " + ex.Message);
            }
        }

        private void comboUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = comboUser.SelectedIndex;
            if (index < 0 || index >= UserContext.KnownUsers.Length) return;
            var user = UserContext.KnownUsers[index];
            if (UserContext.Current.User == user) return;
            UserContext.Current.User = user;
            trace.In("user → " + user.UserName, "ComboBox.SelectedIndexChanged");
            trace.Server("UserContext.Current.User", user.UserName + " · " + user.Role + " · stored in Application.Session of " + Short(_sessionId) + " (another tab keeps its own user)");
            AuditLog.Add(user.UserName, _sessionId, "signed in as " + user.UserName + " (" + user.Role + ")");
            HideBanner();
        }

        // ------------------------------------------------------------------ security ------------------------

        private void buttonRenderEncoded_Click(object sender, EventArgs e)
        {
            trace.In("Render encoded ✓", "click");
            var notes = _service.Find(1038)?.Notes ?? "";
            settingsView.Rendered.AllowHtml = false;
            settingsView.Rendered.Text = notes;
            settingsView.SecurityResult.Text = "Label.AllowHtml = false (default): the browser shows the markup as text. Nothing is parsed, nothing runs.";
            settingsView.SecurityResult.ForeColor = Palette.Good;
            trace.Server("Label.AllowHtml = false", "Text = order 1038 Notes → encoded by Wisej.NET; the user sees the literal <b> and <img …> characters");
            trace.Ok("Render encoded", "safe by default — this is why AllowHtml stays off for user data");
            AuditLog.Add(UserContext.Current.UserName, _sessionId, "rendered order 1038 notes encoded");
            HideBanner();
            SetStatus("idle");
        }

        private void buttonRenderAllowHtml_Click(object sender, EventArgs e)
        {
            trace.In("Render AllowHtml ✕", "click");
            var notes = _service.Find(1038)?.Notes ?? "";
            // Deliberately NOT executed: AllowHtml = true with the raw text would inject the <img onerror> into
            // every viewer's page. The lab shows the raw markup and explains what the browser would do.
            settingsView.Rendered.AllowHtml = false;
            settingsView.Rendered.Text = "[blocked by the lab] AllowHtml = true would parse the raw notes shown above";
            settingsView.SecurityResult.Text = "✖ AllowHtml = true on user-entered text: <img src=x> fails to load → onerror runs document.title='pwned' in every browser that opens order 1038. Stored XSS, courtesy of a formatting flag.";
            settingsView.SecurityResult.ForeColor = Palette.Bad;
            trace.Fail("Label.AllowHtml = true (NOT applied)", "✖ would inject <img src=x onerror=\"document.title='pwned'\"> — the browser requests x, fails, and runs the handler; the script comes from an order note");
            trace.Finding("AllowHtml rule", "only trusted or sanitized content; encoded text is the default and stays the default for user data");
            AuditLog.Add(UserContext.Current.UserName, _sessionId, "attempted AllowHtml render of order 1038 notes — blocked by the lab");
            ShowBanner("✖ AllowHtml on user-entered notes would inject <img onerror=…> into every viewer's browser — blocked here; use Render sanitized ✓.");
            SetStatus("alarm");
            _ = notes;
        }

        private void buttonRenderSanitized_Click(object sender, EventArgs e)
        {
            trace.In("Render sanitized ✓", "click");
            var notes = _service.Find(1038)?.Notes ?? "";
            var clean = HtmlSanitizer.Strip(notes);
            trace.Server("HtmlSanitizer.Strip", HtmlSanitizer.Describe(notes) + " → " + clean);
            settingsView.Rendered.AllowHtml = true;
            settingsView.Rendered.Text = clean;
            settingsView.SecurityResult.Text = "AllowHtml = true with SANITIZED text: allow-list b/i/u/strong/em/br, no attributes, no other tags. \"Rush\" is bold; the <img onerror> is gone.";
            settingsView.SecurityResult.ForeColor = Palette.Good;
            trace.Ok("Render sanitized", "AllowHtml = true only after HtmlSanitizer.Strip — formatting kept, payload removed");
            AuditLog.Add(UserContext.Current.UserName, _sessionId, "rendered order 1038 notes sanitized (AllowHtml + HtmlSanitizer)");
            _sanitizedShown = true;
            SetCheck(5, true, "Render sanitized ✓ · " + CountAllowHtmlLabels() + " Label(s) with AllowHtml, all sanitized");
            HideBanner();
            SetStatus("idle");
        }

        private void buttonGuardedDownload_Click(object sender, EventArgs e)
        {
            trace.In("Guarded download", "click");
            var ctx = UserContext.Current;
            const string file = "customers-with-taxid.csv";
            var decision = DownloadGuard.Authorize(ctx, "Manager", file);
            trace.Server("DownloadGuard.Authorize", "user " + ctx.UserName + " (" + ctx.Role + ") · " + file + " requires Manager · server-side, every request");
            if (!decision.Allowed)
            {
                trace.Fail("download denied", "✖ " + decision.StatusCode + " " + file + " " + decision.Reason);
                AuditLog.Add(ctx.UserName, _sessionId, "download " + file + " denied (" + decision.StatusCode + ")");
                ShowBanner("✖ " + decision.StatusCode + " " + file + " " + decision.Reason + " — switch the user to dana · Manager in the app bar and retry.");
                SetStatus("alarm");
                return;
            }
            var csv = DownloadGuard.CustomersWithTaxIdCsv(_service.Customers);
            var bytes = Encoding.UTF8.GetBytes(csv);
            Application.Download(new MemoryStream(bytes), file);
            trace.Out("Application.Download", file + " · " + bytes.Length + " bytes · " + _service.Customers.Count + " customers WITH TaxId");
            trace.Ok("download granted", decision.ToString());
            AuditLog.Add(ctx.UserName, _sessionId, "downloaded " + file + " (Manager)");
            HideBanner();
            SetStatus("idle");
        }

        // ------------------------------------------------------------------ deployment ----------------------

        private void buttonHealth_Click(object sender, EventArgs e)
        {
            trace.In("Health check", "click");
            ShowView("Settings", "Health check");
            var report = HealthReport.Build();
            string json = report.ToJson();
            int wisejSessions = -1;
            try { wisejSessions = Application.SessionCount; } catch { }
            trace.Server("GET /health", json);
            trace.Server("sessions", "SessionRegistry " + report.sessions + " · Application.SessionCount " + (wisejSessions < 0 ? "?" : wisejSessions.ToString()) + " (the probe runs without a session, so it reads the registry)");
            settingsView.HealthLine.Text = "GET /health → " + json;
            _healthChecked = true;
            SetCheck(7, _healthChecked && _configShown, _configShown ? "/health + config shown · docs/deployment-checklist.md" : "/health ok — run Show config too");
            trace.Ok("health probe", "status " + report.status + " · sessions " + report.sessions + " · orders " + report.orders + " · " + report.version + " — open " + HealthUrl() + " in a tab for the real endpoint");
            AuditLog.Add(UserContext.Current.UserName, _sessionId, "ran the health check (" + report.sessions + " sessions, " + report.orders + " orders)");
            HideBanner();
            SetStatus("idle");
        }

        private void OpenHealthTab()
        {
            string url = HealthUrl();
            trace.In("Open /health ↗", "Application.Navigate(\"" + url + "\", \"_blank\")");
            Application.Navigate(url, "_blank");
        }

        private static string HealthUrl()
        {
            try
            {
                var uri = new Uri(Application.Url);
                return uri.GetLeftPart(UriPartial.Authority) + "/health";
            }
            catch
            {
                return "/health";
            }
        }

        private void buttonShowConfig_Click(object sender, EventArgs e)
        {
            trace.In("Show config", "click");
            const string key = "OrderDesk.StorageRoot";
            string fileValue = AppConfig.Get(key);
            string env = AppConfig.EnvironmentVariableName(key);
            string envValue = AppConfig.Override(key);
            string effective = AppConfig.Effective(key, "App_Data");
            string root = AppConfig.StorageRootPath(Application.StartupPath);
            trace.Server("AppConfig (Web.config)", AppConfig.LoadedFrom);
            trace.Server(key, "Web.config \"" + (fileValue ?? "(missing)") + "\" · env " + env + " = " + (envValue ?? "(not set)") + " → effective \"" + effective + "\"");
            trace.Server("storage root", root + (Directory.Exists(root) ? " (exists)" : " (created on demand by Module 6's import/report code)"));
            trace.Server("connectionStrings", string.Join(", ", AppConfig.ConnectionStringNames) + " · Wisej.DefaultTheme " + (AppConfig.Get("Wisej.DefaultTheme") ?? "-") + " · Wisej.LicenseKey " + (string.IsNullOrEmpty(AppConfig.Get("Wisej.LicenseKey")) ? "(empty — secrets come from the environment, never from a committed file)" : "(set)"));
            trace.Finding("deployment", "same build on IIS / Linux / container: ORDERDESK_STORAGEROOT overrides Web.config; Linux paths are case-sensitive; containers are ephemeral → point the root at a mounted volume");
            settingsView.ConfigLine.Text = key + " → Web.config \"" + (fileValue ?? "-") + "\" · " + env + " " + (envValue == null ? "not set" : "= " + envValue) + " · effective " + root;
            _configShown = true;
            SetCheck(7, _healthChecked && _configShown, _healthChecked ? "/health + config shown · docs/deployment-checklist.md" : "config shown — run Health check too");
            trace.Ok("Show config", "effective storage root " + root);
            SetStatus("idle");
        }

        private void buttonReadiness_Click(object sender, EventArgs e)
        {
            trace.In("Readiness check ✓", "click");
            ShowView("Settings", "Readiness check");
            SetStatus("working");

            // 2. mutable statics
            var statics = StaticStateAudit.Scan();
            foreach (var f in statics)
            {
                if (f.Allowed) trace.Server("static field", f.ToString());
                else trace.Fail("static field", f.ToString());
            }
            bool staticsOk = statics.All(f => f.Allowed);
            SetCheck(1, staticsOk, statics.Count + " mutable static(s) found, " + statics.Count(f => f.Allowed) + " allowed (reference data / config caches) · per-user state → UserContext in Application.Session");

            // 3. desktop boundaries
            var desktopRefs = StaticStateAudit.DesktopReferences();
            bool legacy = StaticStateAudit.LegacyNamespaceCompiled();
            bool boundariesOk = desktopRefs.Count == 0 && !legacy;
            trace.Server("referenced assemblies", desktopRefs.Count == 0 ? "no System.Windows.Forms / System.Drawing.Printing / Microsoft.Win32.Registry / Office Interop" : "✖ " + string.Join(", ", desktopRefs));
            SetCheck(2, boundariesOk, (legacy ? "OrderDesk.Legacy still compiled" : "no OrderDesk.Legacy types") + " · " + (desktopRefs.Count == 0 ? "no desktop assemblies referenced" : string.Join(", ", desktopRefs)) + " · registry → App_Data/users (M4), C:\\Orders → storage root (M6), Excel/printer → XlsxWriter/PDF (M6)");

            // 4. grid volume
            int count = _service.Store.Count;
            SetCheck(3, true, "store " + count + " rows here · 200,000 rows measured in Module 5 (performance-notes.md) · grids use OrderQuery paging");
            trace.Server("OrderStore.Shared().Count", count + " · Orders view binds 200 (server-side query)");

            // 5. dialogs
            int openForms = -1;
            try { openForms = Application.OpenForms.Count; } catch { }
            SetCheck(4, openForms == 0, "Application.OpenForms.Count = " + (openForms < 0 ? "?" : openForms.ToString()) + " · EditOrderDialog shown inside using (Module 3)");
            trace.Server("Application.OpenForms", (openForms < 0 ? "?" : openForms.ToString()) + " open form(s) in this session");

            // 6. AllowHtml
            int allowHtml = CountAllowHtmlLabels();
            bool allowOk = _sanitizedShown ? allowHtml <= 1 : allowHtml == 0;
            SetCheck(5, allowOk, allowHtml + " Label(s) with AllowHtml = true" + (_sanitizedShown ? " (the sanitized one)" : "") + (_sanitizedShown ? "" : " — run Render sanitized ✓ to prove the sanitizer path"));
            if (!_sanitizedShown) SetCheck(5, false, allowHtml + " Label(s) with AllowHtml — pending: run Render sanitized ✓");
            trace.Server("AllowHtml scan", allowHtml + " Label(s) with AllowHtml = true in the page tree");

            // 7. responsive
            SetCheck(6, _modesSeen.Count == 3, "seen " + string.Join(", ", _modesSeen.OrderBy(m => (int)m)) + (_modesSeen.Count == 3 ? "" : " — pending: Preview phone/tablet or resize"));

            // 8. deployment docs
            SetCheck(7, _healthChecked && _configShown, _healthChecked && _configShown ? "/health + config shown · docs/deployment-checklist.md" : "pending: " + (_healthChecked ? "" : "Health check ") + (_configShown ? "" : "Show config"));

            int passed = _checks.Count(c => c);
            var pending = Enumerable.Range(0, 8).Where(i => !_checks[i]).Select(i => (i + 1) + ". " + SettingsView.ChecklistItems[i]).ToList();
            if (passed == 8)
            {
                trace.Ok("Readiness", "8/8 — deployable build: parity first, modernization second, deployment third");
                HideBanner();
                SetStatus("idle");
            }
            else
            {
                trace.Server("Readiness", passed + "/8 · pending: " + string.Join(" · ", pending));
                ShowBanner("Readiness " + passed + "/8 — pending: " + string.Join(" · ", pending), warn: true);
                SetStatus("idle");
            }
            AuditLog.Add(UserContext.Current.UserName, _sessionId, "ran the readiness check (" + passed + "/8)");
        }

        private int CountAllowHtmlLabels()
        {
            int n = 0;
            void Walk(Control c)
            {
                if (c is Label l && l.AllowHtml) n++;
                foreach (Control child in c.Controls) Walk(child);
            }
            Walk(this);
            return n;
        }

        // ------------------------------------------------------------------ helpers -------------------------

        private void SetCheck(int index, bool ok, string note)
        {
            _checks[index] = ok;
            _checkNotes[index] = note;
            settingsView.SetCheck(index, ok, note);
        }

        private void SetStatus(string state)
        {
            labelStatus.Text = "● " + state;
            labelStatus.ForeColor = state.StartsWith("alarm") ? Palette.WarnSoft : (state.StartsWith("working") ? Palette.Warn : System.Drawing.Color.White);
        }

        private void ShowBanner(string text, bool warn = false)
        {
            labelBanner.Text = text;
            labelBanner.BackColor = warn ? Palette.WarnSoft : Palette.BadSoft;
            labelBanner.ForeColor = warn ? Palette.Warn : Palette.Bad;
            labelBanner.Visible = true;
            trace.Out("banner", text);
        }

        private void HideBanner()
        {
            if (labelBanner.Visible) labelBanner.Visible = false;
        }

        private static string ProfileName()
        {
            try { return Application.ActiveProfile?.Name ?? ClientProfile.Default?.Name ?? "Default"; }
            catch { return "Default"; }
        }

        private static string SizeText(System.Drawing.Size s) => s.Width + "×" + s.Height;

        private static string Short(string sessionId) => string.IsNullOrEmpty(sessionId) ? "--------" : (sessionId.Length > 8 ? sessionId.Substring(0, 8) : sessionId);
    }
}
