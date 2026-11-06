using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using FieldOps.Model;
using FieldOps.Services;
using Wisej.Hybrid;
using Wisej.Web;

namespace FieldOps
{
    /// <summary>
    /// The work-order list. Module 2 adds the PWA's server-side half: after every successful load
    /// it hands the browser a compact summary for the offline page, it decides WHEN the install
    /// offer appears (after the first good session, never inside the Hybrid client), and it
    /// compares the shell the browser cached with the one this server ships.
    /// </summary>
    public partial class WorkOrdersPage : Page
    {
        private readonly WorkOrderService _service = WorkOrderService.Current;
        private IReadOnlyList<WorkOrder> _orders = Array.Empty<WorkOrder>();
        private string _connection = "Connected";

        public WorkOrdersPage()
        {
            InitializeComponent();

            // prompt() must run inside the click, so the click is handled in the page.
            btnInstall.AddClientEventListener("execute", "fieldOps.install(this)");
        }

        private void WorkOrdersPage_Load(object sender, EventArgs e)
        {
            if (LoadOrders())
            {
                MeasureConnection();
                CheckShellVersion();
                OnFirstSessionCompleted();
            }
        }

        /// <summary>Re-reads the service. Returns false when the list could not be loaded.</summary>
        public bool LoadOrders(string selectNumber = null)
        {
            try
            {
                _orders = _service.GetOpenOrders(Session.Technician.Id);
                gridOrders.DataSource = _orders;

                var synced = DateTime.Now;
                lblSynced.Text = "synced " + synced.ToString("HH:mm");
                lblCount.Text = $"{_orders.Count} work orders · server-rendered";
                SetConnection(_connection, Ops.Good);

                for (int i = 0; i < _orders.Count; i++)
                {
                    if (_orders[i].Number == selectNumber)
                        gridOrders.CurrentCell = gridOrders.Rows[i].Cells[0];
                }

                // the summary the offline page will read back
                Application.Eval("fieldOps.saveSummary(" + Summarize(_orders, synced) + ")");
                return true;
            }
            catch (Exception ex)
            {
                OpsLog.Error($"work-order refresh for {Session.Technician?.Id}", ex);
                SetConnection("Work orders could not be refreshed.", Ops.Bad);
                return false;
            }
        }

        /// <summary>
        /// Technician, open count, next site, due-before-14:00 and the first rows, with the sync
        /// time - nothing more. It is a read-only snapshot, never something to edit offline.
        /// </summary>
        private string Summarize(IReadOnlyList<WorkOrder> orders, DateTime synced)
        {
            var next = orders.FirstOrDefault(o => o.Status != WorkOrderStatus.OnSite);
            var summary = new
            {
                technician = Session.Technician.Name,
                openCount = orders.Count,
                nextSite = next?.Customer ?? "—",
                dueBefore14 = orders.Count(o => o.DueAt.Date == DateTime.Today && o.DueAt.Hour < 14),
                rows = orders.Take(3).Select(o => new { number = o.Number, site = o.SiteAndAsset, due = o.DueText }),
                syncedAt = synced.ToString("HH:mm"),
                syncedIso = synced.ToString("o"),
            };
            return JsonSerializer.Serialize(summary);
        }

        /// <summary>One round trip through the session, timed: "Connected · 38 ms".</summary>
        private void MeasureConnection()
        {
            var watch = Stopwatch.StartNew();
            Application.Eval("1", _ =>
            {
                _connection = $"Connected · {watch.ElapsedMilliseconds} ms";
                SetConnection(_connection, Ops.Good);
            });
        }

        /// <summary>
        /// The build-stamp check. The page reports which shell cache the browser holds; if it is
        /// older than the one this server ships, the worker is asked to update, and the
        /// controllerchange listener in Default.html offers the reload.
        /// </summary>
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
            // Device.Valid is true only inside the Wisej.NET Hybrid client;
            // in a browser tab and in the installed PWA it is false.
            if (Device.Valid) return;
            if (Application.Session["InstallOffered"] != null) return;

            // Default.html kept the beforeinstallprompt event; the server only
            // decides when the in-page Install button becomes visible.
            Application.Session["InstallOffered"] = true;
            Application.Eval("fieldOps.showInstallButton()", available =>
            {
                if (available is bool yes && yes)
                {
                    btnInstall.Visible = true;
                    SetConnection(_connection = "Connected · install offered", Ops.Good);
                }
                else
                {
                    // Already installed, opened in the installed window, or a browser that does
                    // not offer installation: no button, and the status says why.
                    OpsLog.Info($"session {Application.SessionId[..8]}: install offer not available in this browser");
                }
            });
        }

        private WorkOrder SelectedOrder =>
            gridOrders.CurrentRow?.DataBoundItem as WorkOrder;

        private void SetConnection(string text, Color tone)
        {
            lblConnection.Text = text;
            lblConnection.ForeColor = tone;
        }

        private void OpenDetail()
        {
            var order = SelectedOrder;
            if (order == null)
            {
                SetConnection("Select a work order first.", Ops.Warn);
                return;
            }
            new WorkOrderDetail(order.Number).Show();
        }

        private void btnUpdateStatus_Click(object sender, EventArgs e) => OpenDetail();

        private void gridOrders_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) OpenDetail();
        }

        /// <summary>Start job = the technician arrives: the selected order moves to On site.</summary>
        private void btnStartJob_Click(object sender, EventArgs e)
        {
            var order = SelectedOrder;
            if (order == null) { SetConnection("Select a work order first.", Ops.Warn); return; }
            try
            {
                var saved = _service.UpdateStatus(order.Number, WorkOrderStatus.OnSite, order.Version, Session.Technician.Id);
                OpsLog.Info($"{saved.Number} -> On site by {Session.Technician.Id} (v{saved.Version})");
                LoadOrders(saved.Number);
                SetConnection($"{saved.Number} started - you are On site.", Ops.Good);
            }
            catch (StatusTransitionException ex)
            {
                SetConnection(ex.Message, Ops.Warn);
            }
            catch (WorkOrderConflictException ex)
            {
                OpsLog.Error($"start job {order.Number}", ex);
                LoadOrders(order.Number);
                SetConnection($"{order.Number} was changed somewhere else - the list is refreshed.", Ops.Bad);
            }
            catch (Exception ex)
            {
                OpsLog.Error($"start job {order.Number}", ex);
                SetConnection("The job could not be started. Nothing was changed.", Ops.Bad);
            }
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

        /// <summary>Phone width: Work order, Site / asset and Status only.</summary>
        private void WorkOrdersPage_Resize(object sender, EventArgs e)
        {
            bool compact = Width < 600;
            colPriority.Visible = !compact;
            colDue.Visible = !compact;
            colSite.AutoSizeMode = compact ? DataGridViewAutoSizeColumnMode.Fill : DataGridViewAutoSizeColumnMode.None;
            if (!compact) colSite.Width = 300;
        }
    }
}
