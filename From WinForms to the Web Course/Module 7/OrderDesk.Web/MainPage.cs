using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using OrderDesk.Diagnostics;
using OrderDesk.Domain;
using OrderDesk.Reporting;
using OrderDesk.Security;
using OrderDesk.Services;
using OrderDesk.Views;
using Wisej.Core;
using HealthCheck = OrderDesk.Diagnostics.HealthCheck;
using Wisej.Web;

namespace OrderDesk
{
    /// <summary>
    /// Module 7 · Modernize, secure and deploy the capstone.
    ///
    /// Left (tabs):  Orders — the migrated screen after parity: theme + mixin, tool buttons, watermark,
    ///                        three responsive layouts driven by ClientProfiles.json (or simulated);
    ///               Security — server-side auth per action, the download guard, AllowHtml encoded /
    ///                        sanitized / raw, and the process-wide audit log every session sees;
    ///               Dashboard — KPIs, an orders-by-status Canvas chart and the live activity feed;
    ///               Readiness &amp; deploy — health check, GET /health, the eight-item readiness checklist,
    ///                        the static-state audit and the deployment assets under deploy/.
    /// Right:        the migration log (live trace), status, banner, second session, clear.
    ///
    /// Nothing in Domain/ changed in this module: modernization came after parity, never before.
    /// </summary>
    public partial class MainPage : Page
    {
        private readonly OrderService _orderService = new OrderService();
        private readonly CustomerService _customerService = new CustomerService();
        private ResponsiveLayout _layout;
        private string _dashboardSignature = "";
        private long _auditVersionShown = -1;
        private bool _updatingActions;
        private bool _profileEventAttached;

        private readonly OrderStatus[] _chartStatuses = { OrderStatus.Open, OrderStatus.InProgress, OrderStatus.Shipped, OrderStatus.Invoiced, OrderStatus.Hold };

        public MainPage()
        {
            InitializeComponent();
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            trace.Add(TraceKind.Server, "startup", "Default.json → OrderDesk.Program.Main → Application.MainPage = new MainPage()");
            trace.Add(TraceKind.Server, "session", $"new browser session {Short(Application.SessionId)} · {Application.SessionCount} session(s) in this process");
            AppLog.Info($"session {Short(Application.SessionId)} started · {Application.Browser?.Type} {Application.Browser?.Version} on {Application.Browser?.OS}");
            AuditLog.Record("session-start", $"{Application.Browser?.Type} {Application.Browser?.Version} · {Application.Browser?.OS}", allowed: true);

            // Theme: Default.json loads Bootstrap-4; every Themes/*.mixin.theme is merged at startup.
            trace.Add(TraceKind.Server, "theme", $"{ThemeService.CurrentName} · button radius = {ThemeService.ButtonRadius() ?? "(none)"} (Themes/orderdesk.mixin.theme merged at startup)");

            // The file the download guard serves: written under the storage root, never a user path.
            try
            {
                var orders = _orderService.GetOrders();
                var relative = DownloadGuard.Store("exports/orders.csv", CsvExport.Orders(orders));
                trace.Add(TraceKind.Server, "DownloadGuard.Store", $"{relative} · {orders.Count} orders · under {AppConfig.StorageRoot}");
            }
            catch (Exception ex)
            {
                trace.Add(TraceKind.Server, "DownloadGuard.Store", "✕ " + ex.GetType().Name + ": " + ex.Message);
            }

            _layout = new ResponsiveLayout(gridOrders, labelDetailTitle, labelDetail, toolBar, comboActions, textSearch);
            BindOrders();
            ApplyProfile(Application.ActiveProfile, "Application.ActiveProfile");
            Application.ResponsiveProfileChanged += Application_ResponsiveProfileChanged;
            _profileEventAttached = true;
            trace.Add(TraceKind.Server, "ResponsiveProfileChanged", "subscribed — fires when the browser width crosses a ClientProfiles.json boundary (resize the window)");

            RefreshThemeLabel();
            RefreshAuthLabel();
            RefreshDeployFiles();
            RefreshLiveData(force: true);

            timerAudit.Start();
            trace.Add(TraceKind.Server, "timerAudit", "1 s · refreshes audit grid, activity feed, KPIs and chart only when AuditLog.Version or SessionCount changes");
            Ui.SetStatus(labelStatus, "capstone running · signed in: nobody · sign in on the Security tab", Ui.Ok);
        }

        private void DetachApplicationEvents()
        {
            if (_profileEventAttached)
            {
                Application.ResponsiveProfileChanged -= Application_ResponsiveProfileChanged;
                _profileEventAttached = false;
            }
        }

        private void tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            trace.Add(TraceKind.FromClient, "tabs", $"selected \"{tabs.SelectedTab?.Text}\"");
            if (tabs.SelectedTab == tabDashboard)
                DrawChart();
        }

        #region Orders · the migrated screen (reused business logic, modernized shell)

        private void BindOrders(int? selectId = null, string search = null)
        {
            IList<Order> orders = string.IsNullOrWhiteSpace(search)
                ? _orderService.GetOrders()                                      // ✓ business logic reused as-is
                : _orderService.Search(new OrderFilter { Text = search });       // ✓ the same Search the desktop used
            gridOrders.Rows.Clear();
            foreach (var order in orders)
            {
                int index = gridOrders.Rows.Add(order.Id, order.CustomerName, order.Total, order.Status.ToString());
                gridOrders.Rows[index].Tag = order;
                gridOrders.Rows[index].Cells[3].Style.ForeColor = StatusColor(order.Status);
            }
            trace.Add(TraceKind.Server, string.IsNullOrWhiteSpace(search) ? "OrderService.GetOrders" : "OrderService.Search",
                string.IsNullOrWhiteSpace(search) ? $"{orders.Count} orders from the shared repository" : $"\"{search}\" → {orders.Count} orders");

            int select = 0;
            if (selectId.HasValue)
                for (int i = 0; i < gridOrders.Rows.Count; i++)
                    if ((gridOrders.Rows[i].Tag as Order)?.Id == selectId.Value) select = i;
            if (gridOrders.Rows.Count > 0)
            {
                gridOrders.Rows[select].Selected = true;
                ShowOrderDetail(gridOrders.Rows[select].Tag as Order);
            }
            else
            {
                ShowOrderDetail(null);
            }
        }

        private static Color StatusColor(OrderStatus status) => status switch
        {
            OrderStatus.Shipped => Ui.Ok,
            OrderStatus.Invoiced => Ui.Purple,
            OrderStatus.Hold => Ui.Warn,
            _ => Ui.Accent
        };

        private Order CurrentOrder => gridOrders.CurrentRow?.Tag as Order;

        private void gridOrders_SelectionChanged(object sender, EventArgs e) => ShowOrderDetail(CurrentOrder);

        private void ShowOrderDetail(Order order)
        {
            if (order == null)
            {
                labelDetailTitle.Text = "Order";
                labelDetail.Text = "";
                return;
            }
            labelDetailTitle.Text = $"Order {order.Id}";
            labelDetail.Text = _layout.FormatDetail(order.CustomerName, order.PoNumber, string.IsNullOrEmpty(order.Owner) ? "—" : order.Owner,
                order.Lines.Count, order.Lines.Sum(l => l.Quantity), order.Total);
        }

        private void textSearch_TextChanged(object sender, EventArgs e)
        {
            trace.Add(TraceKind.FromClient, "textSearch", $"\"{textSearch.Text}\"");
            BindOrders(null, textSearch.Text);
        }

        private void toolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            trace.Add(TraceKind.FromClient, "toolBar.ButtonClick", $"{e.Button.Name} \"{e.Button.Text}\"");
            RunAction(e.Button.Name);
        }

        private void comboActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_updatingActions || comboActions.SelectedIndex < 0) return;
            string action = comboActions.SelectedItem?.ToString() ?? "";
            trace.Add(TraceKind.FromClient, "comboActions (phone)", $"\"{action}\" — the ComboBox replaces the toolbar on ≤600 px");
            _updatingActions = true;
            try { comboActions.SelectedIndex = -1; } finally { _updatingActions = false; }
            RunAction(action switch
            {
                "New Order" => "toolNew",
                "Print Invoice" => "toolPrint",
                "Export" => "toolExport",
                _ => "toolRefresh"
            });
        }

        private void RunAction(string name)
        {
            switch (name)
            {
                case "toolNew": NewOrder(); break;
                case "toolPrint": PrintInvoice(); break;
                case "toolExport": ExportOrders(); break;
                default: RefreshOrders(); break;
            }
        }

        private void NewOrder()
        {
            // The same OrderService call LegacyOrderDesk makes — no UI, no desktop assumption, no change in Module 7.
            var customer = _customerService.Find(8);   // Litware Inc
            var order = new Order
            {
                Customer = customer, CustomerId = customer.Id, Owner = AuthService.Current?.UserName ?? "",
                PoNumber = "LW-" + DateTime.Now.ToString("HHmmss"), Status = OrderStatus.Open, CreatedOn = DateTime.Today
            };
            order.Lines.Add(new OrderLine { Sku = "WJ-DEV-SEAT", Description = "Developer seat", Quantity = 5, UnitPrice = 190m });
            order.Lines.Add(new OrderLine { Sku = "WJ-SUP-GOLD", Description = "Gold support, 1 year", Quantity = 1, UnitPrice = 330m });

            var saved = _orderService.Save(order);
            trace.Add(TraceKind.Server, "OrderService.Save", $"order {saved.Id} · CalculateOrderTotal = {saved.Total:N2}  (business logic reused, unchanged since Module 1)");
            AuditLog.Record("order.save", $"order {saved.Id} · {saved.CustomerName} · {saved.Total:N2}", allowed: true);
            BindOrders(saved.Id);
            Ui.Toast($"Order {saved.Id} saved — {saved.Total:N2}.");
            trace.Add(TraceKind.ToClient, "Ui.Toast", "AlertBox top-right, auto-close — not a blocking MessageBox (Module 3 rule)");
            Ui.SetStatus(labelStatus, $"order {saved.Id} saved through the reused OrderService", Ui.Ok);
            Ui.HideBanner(labelBanner);
            RefreshLiveData(force: true);
        }

        private void PrintInvoice()
        {
            var order = CurrentOrder;
            if (order == null) return;

            var lines = InvoiceDocument.Build(order, _orderService);
            var pdf = InvoicePdfWriter.Write(lines, $"Invoice {order.Id}");
            trace.Add(TraceKind.Boundary, "Print Invoice", $"PrintDocument → local printer  ⇒  server PDF ({pdf.Length:N0} bytes) → PdfViewer");
            trace.Add(TraceKind.ToClient, "InvoicePreviewForm", $"Invoice-{order.Id}.pdf  (PdfViewer.PdfStream, modal)");
            AuditLog.Record("print", $"invoice {order.Id} ({pdf.Length:N0} bytes)", allowed: true);

            var preview = new InvoicePreviewForm(pdf, $"Invoice-{order.Id}.pdf");
            preview.ShowDialog((form, result) =>
            {
                form.Dispose();     // Module 3 rule: the caller disposes a transient dialog when it closes
                trace.Add(TraceKind.Server, "InvoicePreviewForm", "closed and disposed");
            });
            Ui.SetStatus(labelStatus, $"invoice {order.Id} rendered on the server", Ui.Ok);
        }

        private void ExportOrders()
        {
            // Security review item 1: the server authorizes every action again — the click is all the browser can reach.
            try
            {
                AuthService.Demand(AuthService.PermissionExport);
                trace.Add(TraceKind.Server, "AuthService.Demand", $"✓ '{AuthService.PermissionExport}' — {AuthService.Current}");

                var orders = _orderService.GetOrders();
                var stream = CsvExport.OrdersStream(orders);
                trace.Add(TraceKind.Boundary, "Export", $"Excel.Application + C:\\Orders\\out.xlsx  ⇒  {stream.Length:N0} bytes → Application.Download(\"orders.csv\")");
                Application.Download(stream, "orders.csv");
                AuditLog.Record("export", $"orders.csv ({stream.Length:N0} bytes, {orders.Count} orders)", allowed: true);
                trace.Add(TraceKind.ToClient, "Application.Download", "orders.csv (the browser saves it — no server path involved)");
                Ui.SetStatus(labelStatus, "export streamed to the browser", Ui.Ok);
                Ui.HideBanner(labelBanner);
            }
            catch (UnauthorizedAccessException ex)
            {
                trace.Add(TraceKind.Server, "AuthService.Demand", "✕ " + ex.Message);
                Ui.ShowBanner(labelBanner, $"✕ Export denied on the server: {ex.Message} The button was clickable — on the web the browser is the untrusted side, so the permission is checked per action, not per screen. The denial is in the audit log.", Ui.BannerKind.Error);
                Ui.SetStatus(labelStatus, "export denied — audited", Ui.Error);
            }
            RefreshLiveData(force: true);
        }

        private void RefreshOrders()
        {
            BindOrders(CurrentOrder?.Id, textSearch.Text);
            Ui.SetStatus(labelStatus, "orders reloaded from the shared repository", Ui.Ok);
        }

        #endregion

        #region Responsive · ClientProfiles.json → three layouts

        private void Application_ResponsiveProfileChanged(object sender, ResponsiveProfileChangedEventArgs e)
        {
            if (IsDisposed) return;
            trace.Add(TraceKind.FromClient, "ResponsiveProfileChanged", $"{e.PreviousProfile?.Name ?? "Default"} → {e.CurrentProfile?.Name ?? "Default"} · {ResponsiveLayout.Describe(e.CurrentProfile)}");
            ApplyProfile(e.CurrentProfile, "ResponsiveProfileChanged");
        }

        private void ApplyProfile(ClientProfile profile, string source)
        {
            var kind = ResponsiveLayout.FromProfile(profile);
            string description = _layout.Apply(kind);
            ShowOrderDetail(CurrentOrder);
            labelProfileBadge.Text = ResponsiveLayout.Describe(profile);
            labelProfile.Text = $"{source}: {ResponsiveLayout.Describe(profile)} → {kind}";
            trace.Add(TraceKind.ToClient, "ResponsiveLayout.Apply", $"{kind} ← {source} ({profile?.Name ?? "Default"}) · {description}");
        }

        private void buttonSimDesktop_Click(object sender, EventArgs e) => Simulate(LayoutKind.Desktop);
        private void buttonSimTablet_Click(object sender, EventArgs e) => Simulate(LayoutKind.Tablet);
        private void buttonSimPhone_Click(object sender, EventArgs e) => Simulate(LayoutKind.Phone);

        private void Simulate(LayoutKind kind)
        {
            trace.Add(TraceKind.FromClient, "simulate", $"{kind} — the same Apply a real {kind} profile triggers");
            string description = _layout.Apply(kind);
            ShowOrderDetail(CurrentOrder);
            labelProfile.Text = $"simulated: {kind} (real profile now: {ResponsiveLayout.Describe(Application.ActiveProfile)})";
            trace.Add(TraceKind.ToClient, "ResponsiveLayout.Apply", $"{kind} · {description}");
            Ui.SetStatus(labelStatus, $"layout {kind} applied — only the three tested layouts are claimed", Ui.Ok);
            Ui.HideBanner(labelBanner);
        }

        #endregion

        #region Theme + mixin · restyle after parity

        private void RefreshThemeLabel()
        {
            labelTheme.Text = $"theme {ThemeService.CurrentName} · button radius {ThemeService.ButtonRadius() ?? "(none)"} · mixin {AppConfig.ThemeMixin}.mixin.theme";
        }

        private void buttonThemeBootstrap_Click(object sender, EventArgs e) => ApplyTheme("Bootstrap-4");
        private void buttonThemeMaterial_Click(object sender, EventArgs e) => ApplyTheme("Material-3");
        private void buttonThemeFluent_Click(object sender, EventArgs e) => ApplyTheme("FluentDark-5");

        private void ApplyTheme(string name)
        {
            string before = ThemeService.CurrentName;
            string radiusBefore = ThemeService.ButtonRadius() ?? "(none)";
            trace.Add(TraceKind.FromClient, "theme", $"{name}");
            try
            {
                ThemeService.Apply(name);
                string radiusAfter = ThemeService.ButtonRadius() ?? "(none)";
                trace.Add(TraceKind.Server, "Application.LoadTheme", $"{before} → {ThemeService.CurrentName} · button radius {radiusBefore} → {radiusAfter}");
                trace.Add(TraceKind.ToClient, "theme", "every session in this process is restyled — zero lines of form code changed");
                AuditLog.Record("theme", $"{before} → {name}", allowed: true);
                Ui.SetStatus(labelStatus, $"theme {ThemeService.CurrentName} · same workflow, same C# logic", Ui.Ok);
                Ui.HideBanner(labelBanner);
            }
            catch (Exception ex)
            {
                trace.Add(TraceKind.Server, "Application.LoadTheme", "✕ " + ex.GetType().Name + ": " + ex.Message);
                Ui.ShowBanner(labelBanner, $"✕ LoadTheme(\"{name}\") failed: {ex.Message}", Ui.BannerKind.Error);
                Ui.SetStatus(labelStatus, "theme not applied", Ui.Error);
            }
            RefreshThemeLabel();
        }

        private void buttonMixin_Click(object sender, EventArgs e)
        {
            string radiusBefore = ThemeService.ButtonRadius() ?? "(none)";
            trace.Add(TraceKind.FromClient, "mixin", $"{AppConfig.ThemeMixin} · button radius before = {radiusBefore}");
            try
            {
                ThemeService.ApplyMixin(AppConfig.ThemeMixin);
                string radiusAfter = ThemeService.ButtonRadius() ?? "(none)";
                trace.Add(TraceKind.Server, "Application.LoadTheme(mixins)", $"{ThemeService.CurrentName} + [{AppConfig.ThemeMixin}] · button radius {radiusBefore} → {radiusAfter} (Themes/{AppConfig.ThemeMixin}.mixin.theme says 14)");
                Ui.SetStatus(labelStatus, $"mixin {AppConfig.ThemeMixin} merged over {ThemeService.CurrentName} · button radius {radiusAfter}", Ui.Ok);
                Ui.HideBanner(labelBanner);
            }
            catch (Exception ex)
            {
                trace.Add(TraceKind.Server, "Application.LoadTheme(mixins)", "✕ " + ex.GetType().Name + ": " + ex.Message);
                Ui.ShowBanner(labelBanner, $"✕ Mixin failed: {ex.Message}", Ui.BannerKind.Error);
                Ui.SetStatus(labelStatus, "mixin not applied", Ui.Error);
            }
            RefreshThemeLabel();
        }

        #endregion

        #region Security · server-side auth, download guard, AllowHtml, audit log

        private void RefreshAuthLabel()
        {
            var user = AuthService.Current;
            labelAuth.Text = user == null
                ? $"AuthService.Current = (nobody) · session {Short(Application.SessionId)} · every action below asks the server again"
                : $"AuthService.Current = {user} · session {Short(Application.SessionId)} · signed in {user.SignedInUtc.ToLocalTime():HH:mm:ss}";
        }

        private void buttonSignInKelly_Click(object sender, EventArgs e) => SignIn("kelly", "northwind-2026");
        private void buttonSignInSam_Click(object sender, EventArgs e) => SignIn("sam", "fabrikam-2026");
        private void buttonSignInBad_Click(object sender, EventArgs e) => SignIn("kelly", "wrong-password");

        private void SignIn(string user, string password)
        {
            trace.Add(TraceKind.FromClient, "sign in", $"user = {user} (the password travels to the server and is verified against a salted hash there)");
            var context = AuthService.SignIn(user, password);
            if (context == null)
            {
                trace.Add(TraceKind.Server, "AuthService.SignIn", $"✕ denied for '{user}' — audited, nothing stored in the session");
                Ui.ShowBanner(labelBanner, $"✕ Sign-in denied for {user}: the salted SHA-256 hash did not match. The server recorded the denial in the audit log (✕ row) and the session still has no user — nothing the browser sends is trusted until this check passes.", Ui.BannerKind.Error);
                Ui.SetStatus(labelStatus, $"sign-in denied for {user} — audited", Ui.Error);
            }
            else
            {
                trace.Add(TraceKind.Server, "AuthService.SignIn", $"✓ {context} → Application.Session.UserContext (this session only)");
                Ui.ShowBanner(labelBanner, $"✓ Signed in as {context.UserName} · {context.Role} of {context.Company}. Permissions: {(context.Role == "Manager" ? "read, export, download, delete" : "read")}. Only this browser session holds the context — a second tab starts as nobody.", Ui.BannerKind.Ok);
                Ui.SetStatus(labelStatus, $"signed in as {context.UserName} · {context.Role}", Ui.Ok);
            }
            RefreshAuthLabel();
            RefreshLiveData(force: true);
        }

        private void buttonSignOut_Click(object sender, EventArgs e)
        {
            var user = AuthService.Current?.UserName ?? "nobody";
            trace.Add(TraceKind.FromClient, "sign out", user);
            AuthService.SignOut();
            trace.Add(TraceKind.Server, "AuthService.SignOut", $"{user} → Application.Session.UserContext = null");
            Ui.SetStatus(labelStatus, "signed out · this session has no user", Ui.Warn);
            Ui.HideBanner(labelBanner);
            RefreshAuthLabel();
            RefreshLiveData(force: true);
        }

        private void buttonDownload_Click(object sender, EventArgs e) => GuardedDownload(textDownload.Text);

        private void buttonDownloadTraversal_Click(object sender, EventArgs e)
        {
            textDownload.Text = @"..\Web.config";
            trace.Add(TraceKind.FromClient, "traversal attempt", @"textDownload = ..\Web.config — a name that walks out of the storage root");
            GuardedDownload(textDownload.Text);
        }

        private void GuardedDownload(string requested)
        {
            trace.Add(TraceKind.FromClient, "download", $"\"{requested}\" as {AuthService.Current?.UserName ?? "nobody"}");
            try
            {
                string full = DownloadGuard.Download(requested);
                trace.Add(TraceKind.Server, "DownloadGuard.Download", $"✓ permission files.download · resolved under {AppConfig.StorageRoot} · exists");
                trace.Add(TraceKind.ToClient, "Application.Download", $"{Path.GetFileName(full)} — the resolved server path, never the browser's string");
                Ui.ShowBanner(labelBanner, $"✓ {requested} resolved to {full} and streamed to the browser. Audited with the byte count.", Ui.BannerKind.Ok);
                Ui.SetStatus(labelStatus, $"download {requested} allowed", Ui.Ok);
            }
            catch (UnauthorizedAccessException ex)
            {
                bool traversal = ex.Message.IndexOf("traversal", StringComparison.OrdinalIgnoreCase) >= 0
                              || ex.Message.IndexOf("Rooted", StringComparison.OrdinalIgnoreCase) >= 0
                              || ex.Message.IndexOf("outside", StringComparison.OrdinalIgnoreCase) >= 0;
                trace.Add(TraceKind.Server, "DownloadGuard.Download", "✕ " + ex.Message);
                if (traversal)
                    Ui.ShowBanner(labelBanner, $"✕ Rejected by the download guard: {ex.Message} On the desktop this string would have been a path on the user's own disk; on the server it points at the application's Web.config. The name is resolved under the storage root and anything with '..' or a root is refused before File.Exists is even asked. Audited as ✕.", Ui.BannerKind.Error);
                else
                    Ui.ShowBanner(labelBanner, $"✕ Download denied: {ex.Message} The permission is checked on the server before the path is even resolved (sam is a Clerk; nobody has no role). Audited as ✕.", Ui.BannerKind.Error);
                Ui.SetStatus(labelStatus, traversal ? "path traversal rejected — audited" : "download denied — audited", Ui.Error);
            }
            catch (FileNotFoundException ex)
            {
                trace.Add(TraceKind.Server, "DownloadGuard.Download", "✕ " + ex.Message);
                Ui.ShowBanner(labelBanner, $"✕ {ex.Message}", Ui.BannerKind.Warn);
                Ui.SetStatus(labelStatus, "file not found under the storage root", Ui.Warn);
            }
            catch (Exception ex)
            {
                trace.Add(TraceKind.Server, "DownloadGuard.Download", "✕ " + ex.GetType().Name + ": " + ex.Message);
                Ui.ShowBanner(labelBanner, $"✕ {ex.GetType().Name}: {ex.Message}", Ui.BannerKind.Error);
                Ui.SetStatus(labelStatus, "download failed", Ui.Error);
            }
            RefreshLiveData(force: true);
        }

        private void buttonRender_Click(object sender, EventArgs e)
        {
            string text = textNotes.Text ?? "";
            trace.Add(TraceKind.FromClient, "render notes", $"{text.Length} chars of user-entered text");

            // ✓ the default: Label.AllowHtml = false — Wisej encodes the text, the markup is shown literally.
            labelEncoded.Text = text;
            trace.Add(TraceKind.ToClient, "labelEncoded", "AllowHtml = false (default) — the tags are displayed as text, nothing renders");

            // ✓ the one allowed AllowHtml = true path on user data: the whitelist sanitizer feeds it.
            var result = HtmlSanitizer.Sanitize(text);
            labelSanitized.Text = result.Html;
            trace.Add(TraceKind.Server, "HtmlSanitizer.Sanitize", $"kept {result.TagsKept} tag(s) (b, i, br) · removed {result.TagsRemoved}: {string.Join(" ", result.Removed)}");
            trace.Add(TraceKind.ToClient, "labelSanitized", "AllowHtml = true, fed only by the sanitizer — bold and italic render, the <img onerror> is gone");
            labelRaw.Text = "";
            Ui.ShowBanner(labelBanner, $"✓ Encoded shows the markup as text (the default); sanitized keeps <b>/<i>/<br> and removed {result.TagsRemoved} tag(s): {string.Join(" ", result.Removed)}. Raw stays empty until you press the unsafe button.", Ui.BannerKind.Ok);
            Ui.SetStatus(labelStatus, "notes rendered: encoded + sanitized", Ui.Ok);
        }

        private void buttonRenderRaw_Click(object sender, EventArgs e)
        {
            string text = textNotes.Text ?? "";
            trace.Add(TraceKind.FromClient, "render raw", "AllowHtml = true with the user's text as-is (the failure path)");

            // ✕ the injection the lesson warns about: user-provided data straight into a label with AllowHtml = true.
            labelRaw.Text = text;
            trace.Add(TraceKind.Boundary, "AllowHtml", "desktop Label.Text was inert text  ⇒  web Label with AllowHtml = true is innerHTML: <img src=x onerror=\"alert(1)\"> becomes a DOM element");
            AuditLog.Record("allowhtml.raw", "user text rendered with AllowHtml = true (demo of the injection)", allowed: false);
            Ui.ShowBanner(labelBanner, "✕ The note is now part of the page DOM. The <img src=x> fails to load and the browser runs its onerror handler — if an alert(1) just popped up, that was user-entered text executing in your session; a real payload would read the session cookie or post as you. Wisej encodes Label.Text by default; AllowHtml = true is the one switch that turns that off, so it is allowed only after HtmlSanitizer (item 6 of the readiness checklist).", Ui.BannerKind.Error);
            Ui.SetStatus(labelStatus, "unsafe render — AllowHtml on user data", Ui.Error);
            RefreshLiveData(force: true);
        }

        #endregion

        #region Dashboard · KPIs, chart, live activity (the timer is the progress path and the multi-user proof)

        private void timerAudit_Tick(object sender, EventArgs e)
        {
            if (IsDisposed) return;
            RefreshLiveData(force: false);
        }

        /// <summary>Redraws the audit grid, the activity feed, the KPIs and the chart — only when something changed.</summary>
        private void RefreshLiveData(bool force)
        {
            long version = AuditLog.Version;
            int sessions = Application.SessionCount;
            string signature = version + "|" + sessions;
            if (!force && signature == _dashboardSignature) return;
            bool versionChanged = version != _auditVersionShown;
            bool sessionsChanged = !_dashboardSignature.EndsWith("|" + sessions, StringComparison.Ordinal);
            _dashboardSignature = signature;

            var entries = AuditLog.Snapshot(40);
            string mine = Short(Application.SessionId, 6);

            // The multi-user proof: entries recorded by another browser session appear here without any push from them.
            if (versionChanged && _auditVersionShown >= 0)
            {
                int fresh = (int)Math.Min(version - _auditVersionShown, entries.Count);
                var others = entries.Take(fresh).Where(a => a.Session != mine).ToList();
                if (others.Count > 0)
                    trace.Add(TraceKind.Server, "AuditLog (other session)", $"{others.Count} new entr{(others.Count == 1 ? "y" : "ies")} from session {string.Join(", ", others.Select(a => a.Session).Distinct())}: {string.Join(" · ", others.Select(a => a.Action + " " + a.User))}");
            }
            if (sessionsChanged && _auditVersionShown >= 0)
                trace.Add(TraceKind.Server, "Application.SessionCount", $"{sessions} session(s) in this process");
            _auditVersionShown = version;

            gridAudit.Rows.Clear();
            foreach (var entry in entries)
            {
                int index = gridAudit.Rows.Add(entry.WhenUtc.ToLocalTime().ToString("HH:mm:ss"), entry.Allowed ? "✓" : "✕", entry.User, entry.Session, entry.Action, entry.Detail);
                gridAudit.Rows[index].Cells[1].Style.ForeColor = entry.Allowed ? Ui.Ok : Ui.Error;
                if (entry.Session == mine) gridAudit.Rows[index].Cells[3].Style.Font = Ui.SmallBold;
            }

            gridActivity.Rows.Clear();
            foreach (var entry in entries.Take(12))
            {
                int index = gridActivity.Rows.Add(entry.WhenUtc.ToLocalTime().ToString("HH:mm:ss"), entry.User, entry.Session, entry.Action, entry.Detail);
                if (!entry.Allowed) gridActivity.Rows[index].Cells[3].Style.ForeColor = Ui.Error;
            }

            labelLive.Text = $"● {sessions} session{(sessions == 1 ? "" : "s")} · {AuditLog.Count} audit entries";
            labelSession.Text = $"session {mine} · {sessions} live in this process · user {AuthService.Current?.UserName ?? "nobody"} · audit v{version}";
            RefreshKpis();
            DrawChart();
        }

        private void RefreshKpis()
        {
            var orders = _orderService.GetOrders();
            int open = orders.Count(o => o.Status == OrderStatus.Open);
            decimal today = orders.Where(o => o.CreatedOn.Date == DateTime.Today).Sum(o => o.Total);
            int invoiced = orders.Count(o => o.Status == OrderStatus.Invoiced);
            double onTime = orders.Count == 0 ? 0 : 100.0 * orders.Count(o => o.Status != OrderStatus.Hold) / orders.Count;
            labelKpiOpen.Text = $"Open orders\n{open}";
            labelKpiRevenue.Text = $"Revenue today\n{today:N2}";
            labelKpiInvoiced.Text = $"Invoiced\n{invoiced}";
            labelKpiOnTime.Text = $"On-time %\n{onTime:0.0}";
        }

        private void canvasChart_Redraw(object sender, EventArgs e)
        {
            // The browser asks for the full picture (first render, resize): re-issue every drawing command.
            DrawChart();
        }

        /// <summary>"Orders by status" bars. Drawing commands are queued on the server and executed by the browser's canvas.</summary>
        private void DrawChart()
        {
            var canvas = canvasChart;
            int width = canvas.Width, height = canvas.Height;
            if (width <= 0 || height <= 0) return;

            var orders = _orderService.GetOrders();
            var counts = _chartStatuses.Select(s => orders.Count(o => o.Status == s)).ToArray();
            int max = Math.Max(1, counts.Max());

            canvas.ClearRect(0, 0, width, height);
            canvas.FillStyle = Color.White;
            canvas.FillRect(0, 0, width, height);
            canvas.TextFont = new Font("default", 9F, FontStyle.Bold);
            canvas.TextAlign = CanvasTextAlign.Left;
            canvas.FillStyle = Ui.Muted;
            canvas.FillText($"Orders by status · {orders.Count} orders", 12, 18);

            int left = 24, bottom = height - 28, top = 40;
            int slot = (width - 2 * left) / _chartStatuses.Length;
            int barWidth = slot - 36;
            canvas.TextAlign = CanvasTextAlign.Center;
            for (int i = 0; i < _chartStatuses.Length; i++)
            {
                int x = left + i * slot + 18;
                int barHeight = (int)Math.Round((bottom - top) * (counts[i] / (double)max));
                canvas.FillStyle = StatusColor(_chartStatuses[i]);
                canvas.FillRect(x, bottom - barHeight, barWidth, Math.Max(barHeight, 2));
                canvas.FillStyle = Ui.Muted;
                canvas.FillText(_chartStatuses[i].ToString(), x + barWidth / 2, bottom + 16);
                canvas.FillText(counts[i].ToString(), x + barWidth / 2, bottom - barHeight - 6);
            }
        }

        #endregion

        #region Readiness & deploy

        private void buttonHealth_Click(object sender, EventArgs e)
        {
            trace.Add(TraceKind.FromClient, "health check", "HealthCheck.Report(includeSession: true)");
            var items = HealthCheck.Report(includeSession: true);
            labelHealth.Text = HealthCheck.Text(includeSession: true);
            foreach (var item in items)
                trace.Add(TraceKind.Server, "health · " + item.Name, item.ToString());
            int failed = items.Count(i => !i.Ok);
            if (failed == 0)
            {
                Ui.ShowBanner(labelBanner, $"✓ {items.Count} health items, all ✓ — the same report the /health endpoint serves, plus the session facts a load balancer never sees (sessions, theme, profile, web socket).", Ui.BannerKind.Ok);
                Ui.SetStatus(labelStatus, "healthy", Ui.Ok);
            }
            else
            {
                Ui.ShowBanner(labelBanner, $"✕ {failed} health item(s) failed — see the trace. A load balancer would take this instance out of rotation.", Ui.BannerKind.Error);
                Ui.SetStatus(labelStatus, $"{failed} health item(s) failed", Ui.Error);
            }
        }

        private void buttonHealthHttp_Click(object sender, EventArgs e)
        {
            const string url = "http://localhost:5607/health";
            trace.Add(TraceKind.FromClient, "GET /health", "open the JSON in a new tab + fetch it server-side like a load balancer would");
            trace.Add(TraceKind.ToClient, "Application.Navigate", "/health, _blank — a plain ASP.NET Core endpoint, no Wisej session is created for it");
            Application.Navigate("/health", "_blank");
            Ui.SetStatus(labelStatus, "GET /health in flight…", Ui.Warn);

            // The progress path: the fetch runs in a background task; the result is pushed back into this session.
            Application.StartTask(() =>
            {
                string json = null;
                Exception error = null;
                try
                {
                    using (var http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) })
                        json = http.GetStringAsync(url).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    error = ex;
                }

                Application.Update(this, () =>
                {
                    if (IsDisposed) return;
                    if (error == null)
                    {
                        trace.Add(TraceKind.Server, "HttpClient GET", $"{url} → {json.Length} bytes");
                        trace.Add(TraceKind.Server, "/health JSON", json);
                        labelHealth.Text = "GET " + url + "\n" + json.Replace("},\"", "},\n\"");
                        Ui.ShowBanner(labelBanner, $"✓ {url} answered {json.Length} bytes of JSON: {(json.Contains("\"ok\":true") ? "\"ok\": true" : "\"ok\": false")}. Docker's HEALTHCHECK and the IIS/load-balancer probe call exactly this URL.", Ui.BannerKind.Ok);
                        Ui.SetStatus(labelStatus, "GET /health ok", Ui.Ok);
                    }
                    else
                    {
                        trace.Add(TraceKind.Server, "HttpClient GET", $"✕ {url} → {error.GetType().Name}: {error.Message}");
                        Ui.ShowBanner(labelBanner, $"✕ GET {url} failed: {error.Message} — the app is listening on {Application.Url}; the probe URL is hard-wired to port 5607 the way a deployment's health probe is, so a different --urls port is exactly the misconfiguration the readiness checklist is for.", Ui.BannerKind.Warn);
                        Ui.SetStatus(labelStatus, "GET /health failed — port differs?", Ui.Warn);
                    }
                });
            });
        }

        private void buttonReadiness_Click(object sender, EventArgs e)
        {
            trace.Add(TraceKind.FromClient, "readiness checklist", $"{ReadinessChecklist.Items.Count} items (lesson: Final Readiness Checklist)");
            gridReadiness.Rows.Clear();
            int pass = 0;
            foreach (var item in ReadinessChecklist.Items)
            {
                string evidence;
                bool ok = true;
                try { evidence = item.Evidence(); }
                catch (Exception ex) { evidence = ex.GetType().Name + ": " + ex.Message; ok = false; }
                if (ok) pass++;
                int index = gridReadiness.Rows.Add(item.Number, item.Title, evidence);
                gridReadiness.Rows[index].Cells[0].Style.ForeColor = ok ? Ui.Ok : Ui.Error;
                trace.Add(TraceKind.Server, $"readiness {item.Number}/{ReadinessChecklist.Items.Count}", $"{(ok ? "✓" : "✕")} {item.Title} — {evidence}");
            }
            Ui.ShowBanner(labelBanner, $"✓ Readiness checklist: {pass}/{ReadinessChecklist.Items.Count} items with evidence from the running app (items 4 and 8 point at docs and files: the 200,000-row grid test lives in Module 5, the deployment assets in deploy/).", pass == ReadinessChecklist.Items.Count ? Ui.BannerKind.Ok : Ui.BannerKind.Warn);
            Ui.SetStatus(labelStatus, $"readiness {pass}/{ReadinessChecklist.Items.Count}", pass == ReadinessChecklist.Items.Count ? Ui.Ok : Ui.Warn);
        }

        private void buttonStaticAudit_Click(object sender, EventArgs e)
        {
            trace.Add(TraceKind.FromClient, "static-state audit", "StaticStateAudit.Run() — reflection over every static field in the OrderDesk assembly");
            var findings = StaticStateAudit.Run();
            int open = StaticStateAudit.OpenItems(findings);
            foreach (var finding in findings)
                trace.Add(TraceKind.Server, "static", finding.ToString());
            trace.Add(TraceKind.Server, "StaticStateAudit", StaticStateAudit.Summary());
            if (open == 0)
            {
                Ui.ShowBanner(labelBanner, $"✓ 0 open items · {findings.Count} static fields scanned. The only writable static is AuditLog._version, a counter mutated inside AuditLog's lock; everything else is readonly shared state (the repository, the audit list, the account table). Module 1's AppState.CurrentUser would have been listed as ✕ writable.", Ui.BannerKind.Ok);
                Ui.SetStatus(labelStatus, "static-state audit: 0 open items", Ui.Ok);
            }
            else
            {
                Ui.ShowBanner(labelBanner, $"✕ {open} open item(s): writable static fields a session can overwrite for every other session — see the ✕ lines in the trace. Move them to Application.Session or make them readonly shared state behind a lock.", Ui.BannerKind.Error);
                Ui.SetStatus(labelStatus, $"static-state audit: {open} open item(s)", Ui.Error);
            }
        }

        private void RefreshDeployFiles()
        {
            string root = AppConfig.BaseFolder;
            string[] files =
            {
                "deploy/iis/web.config", "deploy/docker/Dockerfile", "deploy/docker/docker-compose.yml", "deploy/appsettings.Production.notes.md",
                "docs/DeploymentChecklist.md", "docs/SecurityReview.md", "docs/ResponsiveProfiles.md", "docs/CapstoneReport.md"
            };
            var lines = new List<string> { $"deployment assets under {root}" };
            int present = 0;
            foreach (var file in files)
            {
                bool exists = File.Exists(Path.Combine(root, file.Replace('/', Path.DirectorySeparatorChar)));
                if (exists) present++;
                lines.Add($"{(exists ? "✓" : "✕")} {file}");
            }
            lines.Add($"/health endpoint: Startup.cs MapGet · logs: {AppLog.LogFolder}");
            labelDeploy.Text = string.Join("\n", lines);
            trace.Add(TraceKind.Server, "deploy assets", $"{present}/{files.Length} files present under {root}");
        }

        #endregion

        #region Session · second session, clear

        private void buttonSecondSession_Click(object sender, EventArgs e)
        {
            trace.Add(TraceKind.ToClient, "Application.Navigate", "same URL, target _blank → a second browser session in this process; sign in as the other user there and watch the audit grid here");
            Application.Navigate(Application.Url, "_blank");
        }

        private void buttonClear_Click(object sender, EventArgs e) => trace.Clear();

        #endregion

        private static string Short(string id, int length = 8) => string.IsNullOrEmpty(id) ? "?" : (id.Length > length ? id.Substring(0, length) : id);
    }
}
