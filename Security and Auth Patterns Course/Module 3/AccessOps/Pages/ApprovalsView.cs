using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AccessOps.Services;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// The approval queue of the caller's department. The Action column reflects the permission set
    /// and the manager scope - "Approve", "Approve · not your report", "— no approval rights",
    /// "Approved" - and is never the control: a click on any pending row goes to
    /// <see cref="ApprovalService.ApproveAsync"/>, which decides again on the server. A click on a row
    /// the UI already shows as out of scope is therefore refused by the service, with one message
    /// on screen and a DENIED line in the server log.
    /// </summary>
    internal sealed class ApprovalsView : UserControl
    {
        private readonly IAreaHost _host;
        private readonly ApprovalService _approvals = AppHost.Get<ApprovalService>();
        private readonly MessageStrip pnlRefusal = new MessageStrip();
        private readonly DataGridView gridRequests = new DataGridView();
        private IReadOnlyList<ApprovalListItem> _rows;

        public ApprovalsView(IAreaHost host)
        {
            _host = host;
            this.Name = "viewApprovals";
            this.BackColor = Color.White;

            pnlRefusal.Name = "pnlRefusal";
            pnlRefusal.CssStyle = "";

            gridRequests.Name = "gridRequests";
            gridRequests.Dock = DockStyle.Fill;
            DataGridStyle.Apply(gridRequests, 42);
            gridRequests.Columns.Add(DataGridStyle.Column("colRequest", "Request", nameof(ApprovalListItem.Request), 108));
            gridRequests.Columns.Add(DataGridStyle.Column("colRequester", "Requester", nameof(ApprovalListItem.Requester), 168));
            gridRequests.Columns.Add(DataGridStyle.Column("colDepartment", "Department", nameof(ApprovalListItem.Department), 132));
            gridRequests.Columns.Add(DataGridStyle.Column("colAmount", "Amount", nameof(ApprovalListItem.Amount), 104));
            gridRequests.Columns.Add(DataGridStyle.Column("colAction", "Action", nameof(ApprovalListItem.Action), 230));
            gridRequests.Columns.Add(DataGridStyle.Column("colSubject", "Subject", nameof(ApprovalListItem.Subject), 200, fill: true));
            gridRequests.Columns["colRequest"].DefaultCellStyle.ForeColor = Theme.Accent;
            gridRequests.Columns["colRequest"].DefaultCellStyle.Font = Theme.Mono(12.5f, FontStyle.Bold);
            gridRequests.Columns["colRequester"].DefaultCellStyle.Font = Theme.Px(12.5f, FontStyle.Bold);
            gridRequests.Columns["colAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gridRequests.CellClick += gridRequests_CellClick;
            gridRequests.CellFormatting += gridRequests_CellFormatting;

            this.Controls.Add(gridRequests);
            this.Controls.Add(pnlRefusal);

            Reload(null);
        }

        private void Reload(string status)
        {
            _rows = _approvals.ListAsync().GetAwaiter().GetResult();
            gridRequests.DataSource = _rows;

            var mine = _rows.Count(r => r.CanApprove);
            var outside = _rows.Count(r => r.Status == "Pending" && r.WhyNot == "not your report");
            var me = AppHost.Get<IAccessOpsSession>().Current;
            var defaultStatus = me.Has(Permission.ApproveRequest)
                ? $"{mine} in your reports · {outside} outside them"
                : "Own department only · no approval rights";
            _host.Report(status ?? defaultStatus, status == null ? StatusTone.Idle : StatusTone.Ok, $"{_rows.Count} requests in scope");
        }

// the Action column's look follows the same flags the service computed - a courtesy only
        private void gridRequests_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _rows.Count || gridRequests.Columns[e.ColumnIndex].Name != "colAction")
                return;

            var item = _rows[e.RowIndex];
            if (item.Status != "Pending")
            {
                e.CellStyle.ForeColor = Theme.Ok;
                e.CellStyle.Font = Theme.Px(11.5f, FontStyle.Bold);
            }
            else if (item.CanApprove)
            {
                e.CellStyle.BackColor = Theme.Accent;
                e.CellStyle.ForeColor = Color.White;
                e.CellStyle.Font = Theme.Px(12f, FontStyle.Bold);
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            else if (item.WhyNot == "no approval rights")
            {
                e.CellStyle.ForeColor = ColorTranslator.FromHtml("#8a9aab");
                e.CellStyle.Font = Theme.Mono(11.5f);
            }
            else
            {
                e.CellStyle.ForeColor = ColorTranslator.FromHtml("#9aa7b4");
                e.CellStyle.BackColor = ColorTranslator.FromHtml("#eef1f4");
                e.CellStyle.Font = Theme.Px(11.5f);
            }
        }

        private async void gridRequests_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || gridRequests.Columns[e.ColumnIndex].Name != "colAction")
                return;

            var item = _rows[e.RowIndex];
            if (item.Status != "Pending" || item.WhyNot == "no approval rights")
                return;

            try
            {
                await _approvals.ApproveAsync(item.Id, "Approved in AccessOps");
                pnlRefusal.Clear();
                Reload($"{item.Request} approved · recorded in the server log");
            }
            catch (NotAuthorizedException ex)
            {
                pnlRefusal.Show(ex.Message, StatusTone.Bad);
                _host.Report("Approval refused — logged", StatusTone.Bad);
            }

            Application.Update(this);
        }
    }
}
