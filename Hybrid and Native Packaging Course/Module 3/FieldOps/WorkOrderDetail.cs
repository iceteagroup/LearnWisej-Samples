using System;
using System.Collections.Generic;
using System.Drawing;
using FieldOps.Model;
using FieldOps.Services;
using Wisej.Hybrid;
using Wisej.Web;

namespace FieldOps
{
    /// <summary>
    /// One work order and its status update flow - Accepted, On site, Completed. The page asks
    /// the service; the service decides. Every outcome is visible: the saved status with a
    /// confirmation, a rejected transition explained in words, and a friendly failure message
    /// with the detail written to the server log.
    /// </summary>
    public partial class WorkOrderDetail : Page
    {
        private readonly WorkOrderService _service = WorkOrderService.Current;
        private readonly string _number;
        private WorkOrder _order;

        public WorkOrderDetail(string number)
        {
            InitializeComponent();
            _number = number;
        }

        private void WorkOrderDetail_Load(object sender, EventArgs e)
        {
            // true only inside the Wisej.NET Hybrid client
            bool camera = Device.Valid && Device.Media.CaptureSupported;

            btnTakePhoto.Visible = camera;
            uploadPhoto.Visible = !camera;   // browser and PWA: Upload control

            Reload();
        }

        private void Reload()
        {
            try
            {
                _order = _service.Get(_number);
            }
            catch (Exception ex)
            {
                OpsLog.Error($"opening {_number}", ex);
                ShowMessage("This work order could not be opened. Go back to the list and try again.", Tone.Failure);
                return;
            }

            lblAppTitle.Text = $"FieldOps<span style=\"margin-left:9px;font-weight:500;opacity:.85;font-size:13px\">· {_order.Number}</span>";
            lblHeader.Text =
                $"<div style=\"display:flex;align-items:center;gap:10px\"><span style=\"font-family:Consolas,monospace;font-size:14px;font-weight:800;color:#1565d8\">{_order.Number}</span>{Ops.StatusChip(_order.Status, 11)}</div>" +
                $"<div style=\"font-size:18px;font-weight:700;color:#1f2d3a;margin-top:6px\">{Html(_order.Customer)}</div>";

            var tech = _service.FindTechnician(_order.AssignedTechnician);
            lblFacts.Text =
                Row("Site address", _order.SiteAddress) +
                Row("Fault", _order.Fault) +
                Row("Priority", _order.Priority.ToString(), _order.Priority == WorkOrderPriority.High ? "#c0392b" : null) +
                Row("Assigned", tech == null ? _order.AssignedTechnician : $"{tech.Van} · {tech.Name}");

            // Pre-select the next allowed step; the service still decides.
            rbAccepted.Checked = _order.Status == WorkOrderStatus.Assigned;
            rbOnSite.Checked = _order.Status == WorkOrderStatus.Accepted;
            rbCompleted.Checked = _order.Status >= WorkOrderStatus.OnSite;

            int photos = PhotoStore.Current.Count(_order.Number);
            lblPhotos.Text = photos == 0 ? "No photos yet" : $"{photos} photo{(photos == 1 ? "" : "s")} attached";
        }

        private WorkOrderStatus ChosenStatus =>
            rbCompleted.Checked ? WorkOrderStatus.Completed :
            rbOnSite.Checked ? WorkOrderStatus.OnSite : WorkOrderStatus.Accepted;

        private void btnSaveStatus_Click(object sender, EventArgs e)
        {
            var next = ChosenStatus;
            try
            {
                var before = _order;
                var saved = _service.UpdateStatus(_order.Number, next, _order.Version, Session.Technician.Id);
                if (AppMode.IsLocal)
                {
                    // Local app: saved on THIS device only. Recorded for the next Remote sign-in,
                    // and the screen says so - it must not look saved on the server.
                    DeviceSync.RecordPending(before, next);
                    Reload();
                    ShowMessage($"Saved on this device: {saved.Number} is now <b>{WorkOrder.StatusText(saved.Status)}</b>. " +
                                "It reaches the FieldOps server when you are back online.", Tone.Rejected);
                    return;
                }
                OpsLog.Info($"{saved.Number} -> {WorkOrder.StatusText(saved.Status)} by {Session.Technician.Id} (v{saved.Version})");
                Reload();
                ShowMessage($"Saved: {saved.Number} is now <b>{WorkOrder.StatusText(saved.Status)}</b>.", Tone.Success);
            }
            catch (StatusTransitionException ex)
            {
                // Validation: the rule said no. Explain it; nothing was changed.
                ShowMessage(Html(ex.Message), Tone.Rejected);
            }
            catch (WorkOrderConflictException ex)
            {
                OpsLog.Error($"status update {_order.Number} -> {next}", ex);
                Reload();
                ShowMessage($"{_order.Number} was changed somewhere else while you had it open. " +
                            $"It is now <b>{WorkOrder.StatusText(_order.Status)}</b> - check it and choose again.", Tone.Failure);
            }
            catch (Exception ex)
            {
                OpsLog.Error($"status update {_order.Number} -> {next}", ex);
                ShowMessage("The status could not be saved. Nothing was changed - please try again in a moment.", Tone.Failure);
            }
        }

        private async void btnTakePhoto_Click(object sender, EventArgs e)
        {
            try
            {
                ShowMessage(Html(await PhotoActions.CaptureAsync(_order.Number)), Tone.Success);
            }
            catch (Exception ex)
            {
                OpsLog.Error($"camera capture for {_order.Number}", ex);
                ShowMessage("The camera did not return a photo. Try again, or attach one later.", Tone.Failure);
            }
            Reload();
            Application.Update(this);
        }

        private void uploadPhoto_Uploaded(object sender, UploadedEventArgs e)
        {
            try
            {
                ShowMessage(Html(PhotoActions.SaveUploaded(_order.Number, e)), Tone.Success);
            }
            catch (Exception ex)
            {
                OpsLog.Error($"photo upload for {_order.Number}", ex);
                ShowMessage("The photo could not be saved. Your work order is unchanged.", Tone.Failure);
            }
            Reload();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            var list = Session.ListPage;
            if (list == null || list.IsDisposed)
                Session.ListPage = list = new WorkOrdersPage();
            list.Show();
            list.LoadOrders(_number);
            Dispose();
        }

        private enum Tone { Success, Rejected, Failure }

        private void ShowMessage(string html, Tone tone)
        {
            var (back, border, ink, mark) = tone switch
            {
                Tone.Success => ("#e6f6ee", "#1f9d6b", "#14603f", "✓"),
                Tone.Rejected => ("#fff8e6", "#d99b1c", "#6b4d09", "!"),
                _ => ("#fdecea", "#c0392b", "#7d2018", "✕"),
            };
            lblMessage.Text =
                $"<div style=\"display:flex;align-items:center;gap:11px;padding:10px 14px;margin-top:10px;background:{back};border:1px solid {border}55;border-left:4px solid {border};border-radius:9px\">" +
                $"<span style=\"flex:none;width:20px;height:20px;border-radius:999px;background:{border};color:#fff;display:flex;align-items:center;justify-content:center;font-size:12px;font-weight:800\">{mark}</span>" +
                $"<span style=\"font-size:13px;color:{ink};line-height:1.4\">{html}</span></div>";
            lblMessage.Visible = true;
        }

        private static string Row(string caption, string value, string color = null) =>
            $"<div style=\"display:flex;font-size:13px;padding:5px 0;border-bottom:1px solid #eef2f6\">" +
            $"<span style=\"width:120px;flex:none;color:#5a6b7d;font-weight:700\">{caption}</span>" +
            $"<span style=\"color:{color ?? "#1f2d3a"};{(color != null ? "font-weight:700" : "")}\">{Html(value)}</span></div>";

        private static string Html(string text) => System.Net.WebUtility.HtmlEncode(text ?? "");
    }
}
