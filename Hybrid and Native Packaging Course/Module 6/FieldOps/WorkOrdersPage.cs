using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using FieldOps.Model;
using FieldOps.Services;
using Wisej.Hybrid;
using Wisej.Web;

namespace FieldOps
{
    /// <summary>
    /// The work-order list on the desktop. Module 3 adds what the Hybrid client makes possible -
    /// a report written into the technician's Documents folder, an operating-system notification
    /// for a new order, a second window - each behind Device.Valid with a browser answer.
    /// The same file is compiled into FieldOps.Offline, the Local app, which shows the offline banner.
    /// </summary>
    public partial class WorkOrdersPage : Page
    {
        private readonly WorkOrderService _service = WorkOrderService.Current;
        private readonly ReportService _reports = new ReportService();
        private BindingList<WorkOrder> _orders = new BindingList<WorkOrder>();

        public WorkOrdersPage()
        {
            InitializeComponent();

            // Module 2: prompt() must run inside the click, so the click is handled in the page.
            btnInstall.AddClientEventListener("execute", "fieldOps.install(this)");

            // New orders arrive through Wisej.NET server push.
            OrderFeed.NewOrder += OnFeed;
        }

        private void WorkOrdersPage_Load(object sender, EventArgs e)
        {
            _technicianId = Session.Technician.Id;
            SignIn.ApplyTitle(Session.Technician);
            lblOffline.Visible = AppMode.IsLocal;

            // Module 6: the compatibility check runs at startup, BEFORE any work-order query.
            btnStartShift_Click(this, EventArgs.Empty);
        }

        #region Start shift: the server/client compatibility check (Device.Versioning)

        private void btnStartShift_Click(object sender, EventArgs e)
        {
            updatePanel.Visible = false;
            gridOrders.Visible = true;
            btnExportReport.Visible = btnOpenWindow.Visible = true;
            pnlDeviceActions.Visible = true;
            lblNote.Visible = false;

            if (!Device.Valid || AppMode.IsLocal) { LoadWorkOrders(); return; }   // browser or PWA: latest server

            ClientReleasePolicy policy;
            try
            {
                policy = ClientReleasePolicy.Load();
            }
            catch (Exception ex)
            {
                OpsLog.Error("loading ClientPolicy.json", ex);
                SetStatus("The release policy could not be read. Work orders are loaded; tell your administrator.", Ops.Warn);
                LoadWorkOrders();
                return;
            }

            var platform = Device.System.Platform.ToString(); // Android, iOS, MacCatalyst, WinUI
            Version client;
            try
            {
                client = ClientReleasePolicy.Normalize(Version.Parse(Device.Versioning.CurrentVersion));
            }
            catch (Exception ex)
            {
                OpsLog.Error($"reading the client version ({Device.Versioning.CurrentVersion})", ex);
                client = new Version(0, 0, 0);
            }

            Application.Session["ClientPlatform"] = platform;
            Application.Session["ClientVersion"] = client.ToString();
            OpsLog.Info($"version: session {Application.SessionId[..8]} {platform} client {client} (build {Device.Versioning.CurrentBuild}) against minimum {policy.Rule(platform).Minimum}, latest {policy.Rule(platform).Latest}, server {policy.ServerVersion}");

            if (client < policy.MinimumClient(platform)) { ShowUpdateRequired(platform, client, policy); return; }

            if (client < policy.LatestClient(platform)) ShowUpdateNotice(policy.Rule(platform).Latest);
            else ShowNote($"Admitted · client {client} is current", Ops.Good, dismissible: false);
            if (Device.Versioning.IsFirstLaunchForCurrentVersion) ShowWhatsNew(client, policy.WhatsNew);
            LoadWorkOrders();
        }

        /// <summary>Below the minimum: the update screen, and NO work order is loaded.</summary>
        private void ShowUpdateRequired(string platform, Version client, ClientReleasePolicy policy)
        {
            gridOrders.DataSource = null;
            gridOrders.Visible = false;
            pnlDeviceActions.Visible = false;
            btnExportReport.Visible = btnOpenWindow.Visible = false;   // nothing to export, nothing to open
            updatePanel.ShowFor(client.ToString(), policy.Rule(platform).Minimum, policy.Rule(platform));
            SetStatus($"Update required · {platform} {client} < minimum {policy.Rule(platform).Minimum}", Ops.Bad);
            OpsLog.Info($"version: session {Application.SessionId[..8]} BLOCKED {platform} {client}");
            UsageTelemetry.Record("version.check", "update-required");
        }

        private void ShowUpdateNotice(string latest)
        {
            ShowNote($"A newer FieldOps client ({latest}) is available — update when you can. ✕", Color.FromArgb(0xB4, 0x69, 0x0E), dismissible: true);
            UsageTelemetry.Record("version.check", "behind-latest");
        }

        private void ShowWhatsNew(Version client, string text)
        {
            ShowNote($"What's new in {client}: {text}", Ops.Accent, dismissible: true);
            OpsLog.Info($"version: session {Application.SessionId[..8]} first launch of {client}");
        }

        private bool _noteDismissible;

        private void ShowNote(string text, Color tone, bool dismissible)
        {
            lblNote.Text = text;
            lblNote.ForeColor = tone;
            lblNote.BackColor = Color.FromArgb(0x14, tone);
            _noteDismissible = dismissible;
            lblNote.Visible = true;
        }

        private void lblNote_Click(object sender, EventArgs e)
        {
            if (_noteDismissible) lblNote.Visible = false;
        }

        /// <summary>Take Photo, Signature and Navigate act on the selected order's detail page.</summary>
        private void DeviceAction_Click(object sender, EventArgs e)
        {
            var order = SelectedOrder;
            if (order == null) { SetStatus("Select a work order first.", Ops.Warn); return; }
            new WorkOrderDetail(order.Number).Show();
        }

        #endregion

        private void LoadWorkOrders()
        {
            if (!LoadOrders()) return;

            if (AppMode.IsLocal)
            {
                SetStatus(_orders.Count == 0
                    ? "Working offline — no orders saved on this laptop yet. Open FieldOps once while connected: the list syncs to the device for the offline app."
                    : $"Working offline — {_orders.Count} orders saved on this laptop", Ops.Muted);
                return;
            }

            if (Device.Valid)
            {
                SetStatus("Connected · " + new Uri(Application.Url).Authority, Ops.Good);
                SaveSnapshotToDevice();
                if (Application.Session["SyncOutcome"] is string sync)
                {
                    Application.Session["SyncOutcome"] = null;
                    SetStatus(sync, Ops.Good);
                }
                return;
            }

            // Browser and PWA (Module 2): the offline summary, the shell check, the install offer.
            Application.Eval("fieldOps.saveSummary(" + Summarize() + ")");
            CheckShellVersion();
            OnFirstSessionCompleted();
        }

        /// <summary>Re-reads the service into the bound list.</summary>
        public bool LoadOrders(string selectNumber = null)
        {
            try
            {
                _orders = new BindingList<WorkOrder>(_service.GetTodaysOrders(Session.Technician.Id).ToList());
                gridOrders.DataSource = _orders;
                UpdateFooter();
                Select(selectNumber);
                return true;
            }
            catch (Exception ex)
            {
                OpsLog.Error($"work-order refresh for {Session.Technician?.Id}", ex);
                SetStatus("Work orders could not be refreshed.", Ops.Bad);
                return false;
            }
        }

        private void Select(string number)
        {
            for (int i = 0; i < _orders.Count; i++)
                if (_orders[i].Number == number)
                    gridOrders.CurrentCell = gridOrders.Rows[i].Cells[0];
        }

        private void UpdateFooter()
        {
            string client =
                AppMode.IsLocal ? "Local app on this device" :
                !Device.Valid ? "browser" :
                Device.System.PackagingModel == AppPackagingModel.Packaged ? "Packaged · MSIX" : "Wisej.NET Hybrid client";
            lblFooter.Text = $"{_orders.Count} orders · {client}";
        }

        private void SetStatus(string text, Color tone)
        {
            lblExportStatus.Text = text;
            lblExportStatus.ForeColor = tone;
        }

        private WorkOrder SelectedOrder => gridOrders.CurrentRow?.DataBoundItem as WorkOrder;

        #region Export report: Device.FileSystem in the client, Application.Download in a browser

        private void btnExportReport_Click(object sender, EventArgs e)
        {
            byte[] pdf = _reports.BuildWorkOrderReport(_orders, Session.Technician.Name);
            string name = $"work-orders-{DateTime.Today:yyyy-MM-dd}.pdf";
            if (!Device.Valid)
            {
                Application.Download(new MemoryStream(pdf), name);
                SetStatus($"Downloaded {name} — your browser keeps it in Downloads", Ops.Good);
                return;
            }

            try
            {
                // Device.FileSystem writes on the TECHNICIAN'S machine. System.IO.File here would
                // write onto the FieldOps server's disk - the bug the review question is about.
                string folder = Path.Combine(Device.FileSystem.MyDocuments, "FieldOps");
                Device.FileSystem.CreateDirectory(folder);
                Device.FileSystem.WriteBytes(Path.Combine(folder, name), pdf);
                SetStatus(@"Saved to Documents\FieldOps\" + name, Ops.Good);
                OpsLog.Info($"export: {name} ({pdf.Length} bytes) written to the client's Documents\\FieldOps");
                Device.Launcher.Open("Open the report", name, pdf);
            }
            catch (Exception ex)
            {
                // e.g. a Documents folder the user cannot write to: a sentence, never a stack trace.
                OpsLog.Error($"export {name}", ex);
                SetStatus("Export failed: " + ex.Message, Ops.Bad);
            }
        }

        #endregion

        #region New orders: server push, then a local notification inside the client

        // Raised on the dispatcher's thread: hop into THIS session before touching anything.
        private void OnFeed(object sender, OrderEventArgs e)
        {
            if (e.Order.AssignedTechnician != _technicianId) return;
            Application.RunInContext(this, () => OrderFeed_NewOrder(sender, e));
        }

        // Captured at load: the feed runs outside the session, where Application.Session is not available.
        private string _technicianId;

        private void OrderFeed_NewOrder(object sender, OrderEventArgs e)
        {
            _orders.Insert(0, e.Order);                 // the grid is bound to it
            UpdateFooter();
            SetStatus($"New work order {e.Order.Number} · {e.Order.Customer}", Ops.Good);
            Application.Update(this);

            if (Device.Valid && !AppMode.IsLocal)
            {
                Device.LocalNotification.Schedule("New work order",
                    $"{e.Order.Number} · {e.Order.Customer}", DateTime.Now);
                OpsLog.Info($"notify: Device.LocalNotification.Schedule \"New work order\" {e.Order.Number} for session {Application.SessionId[..8]}");

                // Module 5: the appointment reminder, 30 minutes before, scheduled when it is assigned.
                ReminderService.ScheduleFor(e.Order);
            }
        }

        #endregion

        #region Second window: Device.Windows on the desktop, a new tab in a browser

        private async void btnOpenWindow_Click(object sender, EventArgs e)
        {
            var selected = SelectedOrder;
            if (selected == null) { SetStatus("Select a work order first.", Ops.Warn); return; }

            var url = OrderUrl(selected.Number);
            if (Device.Valid && Device.System.IsDesktop)
            {
                var id = await Device.Windows.TryOpenAsync(url, "FieldOps — " + selected.Number);
                SetStatus(id != null ? $"{selected.Number} opened in a second window" : "The second window could not be opened.", id != null ? Ops.Good : Ops.Bad);
                OpsLog.Info($"window: Device.Windows.TryOpenAsync {selected.Number} -> {id ?? "(null)"}");
            }
            else
            {
                Application.Navigate(url, "_blank");
                SetStatus($"{selected.Number} opened in a new tab", Ops.Good);
            }
            Application.Update(this);
        }

        private string OrderUrl(string number)
        {
            var root = new Uri(Application.Url).GetLeftPart(UriPartial.Authority) + "/";
            return $"{root}?order={Uri.EscapeDataString(number)}&tech={Uri.EscapeDataString(Session.Technician.Id)}";
        }

        #endregion

        #region Local fallback: today's orders saved on the device

        private void SaveSnapshotToDevice()
        {
            try
            {
                DeviceSync.SaveSnapshot(Session.Technician, _orders);
                OpsLog.Info($"sync: {_orders.Count} orders saved on the device for the offline app");
            }
            catch (Exception ex)
            {
                OpsLog.Error("saving the offline snapshot", ex);
            }
        }

        #endregion

        #region Module 2: the PWA's server-side half (browser only)

        private string Summarize()
        {
            var open = _orders.Where(o => o.Status != WorkOrderStatus.Completed).ToList();
            var next = open.FirstOrDefault(o => o.Status != WorkOrderStatus.OnSite);
            var synced = DateTime.Now;
            return JsonSerializer.Serialize(new
            {
                technician = Session.Technician.Name,
                openCount = open.Count,
                nextSite = next?.Customer ?? "—",
                dueBefore14 = open.Count(o => o.DueAt.Date == DateTime.Today && o.DueAt.Hour < 14),
                rows = open.Take(3).Select(o => new { number = o.Number, site = o.Customer, due = o.DueText }),
                syncedAt = synced.ToString("HH:mm"),
                syncedIso = synced.ToString("o"),
            });
        }

        private void CheckShellVersion()
        {
            Application.Eval("fieldOps.cachedShell", cached =>
            {
                var shell = cached as string;
                if (!string.IsNullOrEmpty(shell) && shell != ShellVersion.Current)
                {
                    OpsLog.Info($"session {Application.SessionId[..8]} runs shell {shell}, server ships {ShellVersion.Current}: asking the worker to update");
                    Application.Eval("fieldOps.checkForUpdate()");
                }
            });
        }

        private void OnFirstSessionCompleted()
        {
            if (Device.Valid) return;
            if (Application.Session["InstallOffered"] != null) return;
            Application.Session["InstallOffered"] = true;
            Application.Eval("fieldOps.showInstallButton()", available =>
            {
                if (available is bool yes && yes) btnInstall.Visible = true;
                else OpsLog.Info($"session {Application.SessionId[..8]}: install offer not available in this browser");
            });
        }

        #endregion

        private void gridOrders_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && SelectedOrder != null)
                new WorkOrderDetail(SelectedOrder.Number).Show();
        }

        private void gridOrders_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.RowIndex % 2 == 1) e.CellStyle.BackColor = Ops.RowAlt;

            if (e.ColumnIndex == colStatus.Index && e.Value is WorkOrderStatus status)
            {
                e.Value = Ops.StatusChip(status, 11);
                e.FormattingApplied = true;
            }
        }

        /// <summary>Phone width: Order, Site and Status only.</summary>
        private void WorkOrdersPage_Resize(object sender, EventArgs e)
        {
            colTask.Visible = Width >= 600;
        }
    }
}
