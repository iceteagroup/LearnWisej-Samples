using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OrderDesk.Diagnostics;
using OrderDesk.Domain;
using OrderDesk.Reporting;
using OrderDesk.Security;
using OrderDesk.Services;
using OrderDesk.Views;
using Wisej.Core;
using Wisej.Web;

namespace OrderDesk
{
    /// <summary>
    /// The capstone: the migrated Orders screen after parity — themed (Default.json + the orderdesk
    /// mixin), responsive (ClientProfiles.json → three layouts), secured (server-side sign-in and a
    /// permission check on every action, audited) — and the operations dashboard every session sees.
    /// Nothing in Domain/ changed: modernization came after parity, never before.
    /// </summary>
    public partial class MainPage : Page
    {
        private const string ExportFile = "exports/orders.csv";

        private readonly OrderService _orderService = new OrderService();
        private readonly CustomerService _customerService = new CustomerService();
        private readonly OrderStatus[] _chartStatuses = { OrderStatus.Open, OrderStatus.InProgress, OrderStatus.Shipped, OrderStatus.Invoiced, OrderStatus.Hold };
        private ResponsiveLayout _layout;
        private string _dashboardSignature = "";
        private bool _updatingActions;
        private bool _profileEventAttached;

        public MainPage()
        {
            InitializeComponent();
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            AppLog.Info($"session {Short(Application.SessionId)} started · {Application.Browser?.Type} {Application.Browser?.Version} on {Application.Browser?.OS}");
            AuditLog.Record("session-start", $"{Application.Browser?.Type} {Application.Browser?.Version} · {Application.Browser?.OS}", allowed: true);

            _layout = new ResponsiveLayout(gridOrders, labelDetailTitle, labelDetail, toolBar, comboActions, textSearch);
            BindOrders();
            ApplyProfile(Application.ActiveProfile);
            Application.ResponsiveProfileChanged += Application_ResponsiveProfileChanged;
            _profileEventAttached = true;

            RefreshAuthLabel();
            RefreshLiveData(force: true);
            timerAudit.Start();
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
            if (tabs.SelectedTab == tabDashboard)
                DrawChart();
        }

        #region Orders

        private void BindOrders(int? selectId = null, string search = null)
        {
            IList<Order> orders = string.IsNullOrWhiteSpace(search)
                ? _orderService.GetOrders()
                : _orderService.Search(new OrderFilter { Text = search });
            gridOrders.Rows.Clear();
            foreach (var order in orders)
            {
                int index = gridOrders.Rows.Add(order.Id, order.CustomerName, order.Total, order.Status.ToString());
                gridOrders.Rows[index].Tag = order;
                gridOrders.Rows[index].Cells[3].Style.ForeColor = StatusColor(order.Status);
            }

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

        private void textSearch_TextChanged(object sender, EventArgs e) => BindOrders(null, textSearch.Text);

        private void toolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e) => RunAction(e.Button.Name);

        private void comboActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_updatingActions || comboActions.SelectedIndex < 0) return;
            string action = comboActions.SelectedItem?.ToString() ?? "";
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
                default: BindOrders(CurrentOrder?.Id, textSearch.Text); break;
            }
        }

        private void NewOrder()
        {
            var customer = _customerService.Find(8);   // Litware Inc
            var order = new Order
            {
                Customer = customer, CustomerId = customer.Id, Owner = AuthService.Current?.UserName ?? "",
                PoNumber = "LW-" + DateTime.Now.ToString("HHmmss"), Status = OrderStatus.Open, CreatedOn = DateTime.Today
            };
            order.Lines.Add(new OrderLine { Sku = "WJ-DEV-SEAT", Description = "Developer seat", Quantity = 5, UnitPrice = 190m });
            order.Lines.Add(new OrderLine { Sku = "WJ-SUP-GOLD", Description = "Gold support, 1 year", Quantity = 1, UnitPrice = 330m });

            var saved = _orderService.Save(order);
            AuditLog.Record("order.save", $"order {saved.Id} · {saved.CustomerName} · {saved.Total:N2}", allowed: true);
            BindOrders(saved.Id);
            Ui.Toast($"Order {saved.Id} saved — {saved.Total:N2}.");
            RefreshLiveData(force: true);
        }

        private void PrintInvoice()
        {
            var order = CurrentOrder;
            if (order == null) return;

            var pdf = InvoicePdfWriter.Write(InvoiceDocument.Build(order, _orderService), $"Invoice {order.Id}");
            AuditLog.Record("print", $"invoice {order.Id} ({pdf.Length:N0} bytes)", allowed: true);

            var preview = new InvoicePreviewForm(pdf, $"Invoice-{order.Id}.pdf");
            preview.ShowDialog((form, result) => form.Dispose());
        }

        private void ExportOrders()
        {
            // The server authorizes every action again: the click is all the browser can reach.
            try
            {
                AuthService.Demand(AuthService.PermissionExport);
                var orders = _orderService.GetOrders();
                DownloadGuard.Store(ExportFile, CsvExport.Orders(orders));
                DownloadGuard.Download(ExportFile);
                AuditLog.Record("export", $"{ExportFile} ({orders.Count} orders)", allowed: true);
            }
            catch (UnauthorizedAccessException ex)
            {
                Ui.Toast("Export is not allowed: " + ex.Message, MessageBoxIcon.Warning);
            }
            RefreshLiveData(force: true);
        }

        #endregion

        #region Responsive: ClientProfiles.json → three layouts

        private void Application_ResponsiveProfileChanged(object sender, ResponsiveProfileChangedEventArgs e)
        {
            if (IsDisposed) return;
            ApplyProfile(e.CurrentProfile);
        }

        private void ApplyProfile(ClientProfile profile)
        {
            _layout.Apply(ResponsiveLayout.FromProfile(profile));
            ShowOrderDetail(CurrentOrder);
        }

        #endregion

        #region Security: sign-in, AllowHtml

        private void RefreshAuthLabel()
        {
            var user = AuthService.Current;
            labelAuth.Text = user == null ? "Not signed in" : $"{user.UserName} · {user.Role} of {user.Company}";
        }

        // Demo accounts (Security/AuthService.cs): the password is verified against a salted hash on the server.
        private void buttonSignInKelly_Click(object sender, EventArgs e) => SignIn("kelly", "northwind-2026");
        private void buttonSignInSam_Click(object sender, EventArgs e) => SignIn("sam", "fabrikam-2026");

        private void SignIn(string user, string password)
        {
            if (AuthService.SignIn(user, password) == null)
                Ui.Toast($"Sign-in failed for {user}.", MessageBoxIcon.Warning);
            RefreshAuthLabel();
            RefreshLiveData(force: true);
        }

        private void buttonSignOut_Click(object sender, EventArgs e)
        {
            AuthService.SignOut();
            RefreshAuthLabel();
            RefreshLiveData(force: true);
        }

        private void buttonSecondSession_Click(object sender, EventArgs e)
        {
            Application.Navigate(Application.Url, "_blank");
        }

        /// <summary>User-entered text reaches a label with AllowHtml = true only through the whitelist sanitizer.</summary>
        private void textNotes_TextChanged(object sender, EventArgs e)
        {
            labelSanitized.Text = HtmlSanitizer.Sanitize(textNotes.Text).Html;
        }

        #endregion

        #region Dashboard: KPIs, chart, activity

        private void timerAudit_Tick(object sender, EventArgs e)
        {
            if (IsDisposed) return;
            RefreshLiveData(force: false);
        }

        /// <summary>Redraws the activity feed, the KPIs and the chart — only when something changed.</summary>
        private void RefreshLiveData(bool force)
        {
            long version = AuditLog.Version;
            int sessions = Application.SessionCount;
            string signature = version + "|" + sessions;
            if (!force && signature == _dashboardSignature) return;
            _dashboardSignature = signature;

            gridActivity.Rows.Clear();
            foreach (var entry in AuditLog.Snapshot(12))
            {
                int index = gridActivity.Rows.Add(entry.WhenUtc.ToLocalTime().ToString("HH:mm:ss"), entry.User, entry.Session, entry.Action, entry.Detail);
                if (!entry.Allowed) gridActivity.Rows[index].Cells[3].Style.ForeColor = Ui.Error;
            }

            labelLive.Text = $"● live · {sessions} session{(sessions == 1 ? "" : "s")}";
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

        private void canvasChart_Redraw(object sender, EventArgs e) => DrawChart();

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

        private static string Short(string id) => string.IsNullOrEmpty(id) ? "?" : (id.Length > 8 ? id.Substring(0, 8) : id);
    }
}
