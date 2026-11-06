using System;
using System.Drawing;
using System.Net;
using FieldOps.Services;
using Wisej.Web;

namespace FieldOps
{
    /// <summary>
    /// One work order on its own: what the second desktop window shows
    /// (Device.Windows.TryOpenAsync) and what a browser tab shows (Application.Navigate "_blank").
    /// Built in code: two controls, nothing for a designer to do.
    /// </summary>
    public class WorkOrderWindow : Page
    {
        private readonly Label lblAppTitle = new Label();
        private readonly Label lblFields = new Label();

        public WorkOrderWindow(string number)
        {
            Name = "WorkOrderWindow";
            Text = "FieldOps — " + number;
            BackColor = Color.White;

            lblFields.Name = "lblFields";
            lblFields.AllowHtml = true;
            lblFields.AutoSize = false;
            lblFields.Dock = DockStyle.Fill;
            lblFields.Padding = new Padding(16, 12, 16, 12);

            lblAppTitle.Name = "lblAppTitle";
            lblAppTitle.AllowHtml = true;
            lblAppTitle.AutoSize = false;
            lblAppTitle.Dock = DockStyle.Top;
            lblAppTitle.Height = 34;
            lblAppTitle.Padding = new Padding(14, 0, 0, 0);
            lblAppTitle.BackColor = Ops.Accent;
            lblAppTitle.ForeColor = Color.White;
            lblAppTitle.Font = Ops.Px(13, FontStyle.Bold);
            lblAppTitle.TextAlign = ContentAlignment.MiddleLeft;

            Controls.Add(lblFields);
            Controls.Add(lblAppTitle);

            Render(number);
        }

        private void Render(string number)
        {
            lblAppTitle.Text = $"{Enc(number)}<span style=\"margin-left:9px;font-weight:500;opacity:.85;font-size:12px\">· Work order</span>";
            try
            {
                var service = WorkOrderService.Current;
                var o = service.Get(number);
                if (o.AssignedTechnician != Session.Technician?.Id)
                    throw new UnauthorizedAccessException($"{number} is not assigned to {Session.Technician?.Id}.");
                var tech = service.FindTechnician(o.AssignedTechnician);
                lblFields.Text =
                    Row("Site", o.Customer) + Row("Task", o.Fault) + Row("Priority", o.Priority.ToString()) +
                    Row("Assigned", tech == null ? o.AssignedTechnician : $"{tech.Van} · {tech.Name}") +
                    Row("Status", Model.WorkOrder.StatusText(o.Status));
            }
            catch (Exception ex)
            {
                OpsLog.Error($"second window for {number}", ex);
                lblFields.Text = "<div style=\"font-size:13px;color:#7d2018\">This work order cannot be shown here.</div>";
            }
        }

        private static string Row(string caption, string value) =>
            $"<div style=\"display:flex;font-size:12.5px;padding:6px 0;border-bottom:1px solid #eef2f6\">" +
            $"<span style=\"width:92px;color:#5a6b7d;font-weight:700\">{caption}</span><span style=\"color:#1f2d3a\">{Enc(value)}</span></div>";

        private static string Enc(string s) => WebUtility.HtmlEncode(s ?? "");
    }
}
