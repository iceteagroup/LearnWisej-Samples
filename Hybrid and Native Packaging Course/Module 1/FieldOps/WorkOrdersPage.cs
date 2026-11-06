using System;
using System.Collections.Generic;
using System.Drawing;
using FieldOps.Model;
using FieldOps.Services;
using Wisej.Web;

namespace FieldOps
{
    /// <summary>
    /// The work-order list: the signed-in technician's open orders, the status update entry point
    /// and the photo action. Nothing on it names a platform.
    /// </summary>
    public partial class WorkOrdersPage : Page
    {
        private readonly WorkOrderService _service = WorkOrderService.Current;
        private IReadOnlyList<WorkOrder> _orders = Array.Empty<WorkOrder>();

        public WorkOrdersPage()
        {
            InitializeComponent();
        }

        private void WorkOrdersPage_Load(object sender, EventArgs e)
        {
            // The capability gate: Take photo only inside the Hybrid client with a camera,
            // the Upload control everywhere else (browser tab, installed PWA).
            bool camera = Capabilities.CanTakePhoto;
            btnTakePhoto.Visible = camera;
            uploadPhoto.Visible = !camera;

            LoadOrders();
        }

        /// <summary>Re-reads the service. Called on load and whenever this page is shown again.</summary>
        public void LoadOrders(string selectNumber = null)
        {
            try
            {
                _orders = _service.GetOpenOrders(Session.Technician.Id);
                gridOrders.DataSource = _orders;
                SetStatus($"Connected · {_orders.Count} open orders", Ops.Good);

                for (int i = 0; i < _orders.Count; i++)
                {
                    if (_orders[i].Number == selectNumber)
                        gridOrders.CurrentCell = gridOrders.Rows[i].Cells[0];
                }
            }
            catch (Exception ex)
            {
                OpsLog.Error($"loading orders for {Session.Technician?.Id}", ex);
                SetStatus("Your work orders could not be loaded. Try again in a moment.", Ops.Bad);
            }
        }

        private WorkOrder SelectedOrder =>
            gridOrders.CurrentRow?.DataBoundItem as WorkOrder;

        private void SetStatus(string text, Color tone)
        {
            lblConnection.Text = text;
            lblConnection.ForeColor = tone;
            lblDot.Text = $"<span style=\"display:inline-block;width:8px;height:8px;border-radius:999px;background:{ColorTranslator.ToHtml(tone)}\"></span>";
        }

        private void OpenDetail()
        {
            var order = SelectedOrder;
            if (order == null)
            {
                SetStatus("Select a work order first.", Ops.Warn);
                return;
            }
            new WorkOrderDetail(order.Number).Show();
        }

        private void btnUpdateStatus_Click(object sender, EventArgs e) => OpenDetail();

        private void gridOrders_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) OpenDetail();
        }

        private async void btnTakePhoto_Click(object sender, EventArgs e)
        {
            var order = SelectedOrder;
            if (order == null) { SetStatus("Select a work order first.", Ops.Warn); return; }
            try
            {
                SetStatus(await PhotoActions.CaptureAsync(order.Number), Ops.Good);
            }
            catch (Exception ex)
            {
                OpsLog.Error($"camera capture for {order.Number}", ex);
                SetStatus("The camera did not return a photo. Try again, or attach one later.", Ops.Bad);
            }
            Application.Update(this);
        }

        private void uploadPhoto_Uploaded(object sender, UploadedEventArgs e)
        {
            var order = SelectedOrder;
            if (order == null) { SetStatus("Select a work order first.", Ops.Warn); return; }
            try
            {
                SetStatus(PhotoActions.SaveUploaded(order.Number, e), Ops.Good);
            }
            catch (Exception ex)
            {
                OpsLog.Error($"photo upload for {order.Number}", ex);
                SetStatus("The photo could not be saved. Your work order is unchanged.", Ops.Bad);
            }
        }

        private void gridOrders_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.RowIndex % 2 == 1) e.CellStyle.BackColor = Ops.RowAlt;

            if (e.ColumnIndex == colStatus.Index && e.Value is WorkOrderStatus status)
            {
                e.Value = Ops.StatusChip(status);
                e.FormattingApplied = true;
            }
            else if (e.ColumnIndex == colPriority.Index && e.Value is WorkOrderPriority priority)
            {
                e.CellStyle.ForeColor = priority == WorkOrderPriority.High ? Ops.High : Ops.Body;
            }
        }

        /// <summary>Phone width: Order, Site and Status only - the compact list of the walkthrough.</summary>
        private void WorkOrdersPage_Resize(object sender, EventArgs e)
        {
            bool compact = Width < 600;
            colFault.Visible = !compact;
            colPriority.Visible = !compact;
            colSite.AutoSizeMode = compact ? DataGridViewAutoSizeColumnMode.Fill : DataGridViewAutoSizeColumnMode.None;
            if (!compact) colSite.Width = 210;
        }
    }
}
