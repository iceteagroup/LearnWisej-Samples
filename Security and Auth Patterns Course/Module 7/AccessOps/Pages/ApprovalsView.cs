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
    /// The approvals screen from Module 6: Approve / Reject for the selected request, the number of
    /// audit rows written today, and every command run under its own correlation ID. Success shows
    /// the audit entry; a refusal says so without naming what it protects; an unexpected failure is
    /// one sentence plus the correlation ID as the reference for a support call.
    /// </summary>
    internal sealed class ApprovalsView : UserControl
    {
        private readonly IAreaHost _host;
        private readonly ApprovalService _approvals = AppHost.Get<ApprovalService>();
        private readonly AuditService _audit = AppHost.Get<AuditService>();

        private readonly Panel pnlToolbar = new Panel();
        private readonly Button btnApprove = new Button();
        private readonly Button btnReject = new Button();
        private readonly Label lblAudited = new Label();
        private readonly MessageStrip pnlBanner = new MessageStrip();
        private readonly DataGridView gridRequests = new DataGridView();
        private IReadOnlyList<ApprovalListItem> _rows = Array.Empty<ApprovalListItem>();

        public ApprovalsView(IAreaHost host)
        {
            _host = host;
            this.Name = "viewApprovals";
            this.BackColor = Color.White;
            var me = AppHost.Get<IAccessOpsSession>().Current;

            pnlToolbar.Name = "pnlToolbar";
            pnlToolbar.Dock = DockStyle.Top;
            pnlToolbar.Height = 52;
            pnlToolbar.BackColor = Theme.Strip;
            pnlToolbar.Padding = new Padding(16, 10, 16, 10);
            pnlToolbar.CssStyle = "border-bottom:1px solid #e4eaf1;";

            lblAudited.Name = "lblAudited";
            lblAudited.Dock = DockStyle.Right;
            lblAudited.AutoSize = true;
            lblAudited.Font = Theme.Mono(12f);
            lblAudited.ForeColor = Theme.Muted;
            lblAudited.TextAlign = ContentAlignment.MiddleRight;
            pnlToolbar.Controls.Add(lblAudited);

            btnReject.Name = "btnReject";
            btnReject.Text = "Reject";
            btnReject.Dock = DockStyle.Left;
            btnReject.Width = 84;
            Theme.StyleButton(btnReject, primary: false);
            btnReject.Click += (s, e) => Decide(approve: false);

            btnApprove.Name = "btnApprove";
            btnApprove.Text = "Approve";
            btnApprove.Dock = DockStyle.Left;
            btnApprove.Width = 96;
            Theme.StyleButton(btnApprove, primary: true);
            btnApprove.Click += (s, e) => Decide(approve: true);

            // reflects the permission set; ApprovalService decides again on every click
            btnApprove.Enabled = btnReject.Enabled = me.Has(Permission.ApproveRequest);

            pnlToolbar.Controls.Add(btnReject);
            pnlToolbar.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 12 });
            pnlToolbar.Controls.Add(btnApprove);

            pnlBanner.Name = "pnlBanner";

            gridRequests.Name = "gridRequests";
            gridRequests.Dock = DockStyle.Fill;
            DataGridStyle.Apply(gridRequests);
            gridRequests.Columns.Add(DataGridStyle.Column("colRequest", "Request", nameof(ApprovalListItem.Request), 96));
            gridRequests.Columns.Add(DataGridStyle.Column("colSubject", "Subject", nameof(ApprovalListItem.Subject), 250, fill: true));
            gridRequests.Columns.Add(DataGridStyle.Column("colDepartment", "Department", nameof(ApprovalListItem.Department), 150));
            gridRequests.Columns.Add(DataGridStyle.Column("colAmount", "Amount", nameof(ApprovalListItem.Amount), 110));
            gridRequests.Columns.Add(DataGridStyle.Column("colState", "State", nameof(ApprovalListItem.Status), 104));
            gridRequests.Columns["colRequest"].DefaultCellStyle.ForeColor = Theme.Accent;
            gridRequests.Columns["colRequest"].DefaultCellStyle.Font = Theme.Mono(12.5f, FontStyle.Bold);
            gridRequests.Columns["colAmount"].DefaultCellStyle.Font = Theme.Mono(12.5f);
            gridRequests.Columns["colAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gridRequests.CellFormatting += gridRequests_CellFormatting;
            gridRequests.SelectionChanged += (s, e) =>
            {
                if (Selected != null)
                    _host.Report(Selected.Request + " selected", StatusTone.Idle);
            };

            this.Controls.Add(gridRequests);
            this.Controls.Add(pnlBanner);
            this.Controls.Add(pnlToolbar);

            Reload();
            _host.Report("Ready", StatusTone.Idle, $"{_rows.Count(r => r.Status == "Pending")} pending");
        }

        private ApprovalListItem Selected => gridRequests.CurrentRow?.DataBoundItem as ApprovalListItem;

        private void Reload()
        {
            _rows = _approvals.ListAsync().GetAwaiter().GetResult();
            gridRequests.DataSource = _rows;
            lblAudited.Text = $"{_audit.CountToday()} audited today";
        }

        private async void Decide(bool approve)
        {
            var item = Selected;
            if (item == null)
            {
                _host.Report("Select a request first.", StatusTone.Idle);
                return;
            }

            // the correlation ID is minted here, where the command starts - not where it is logged
            using var command = Command.Begin(approve ? "Approval.Approve " + item.Request : "Approval.Reject " + item.Request);
            var reference = command.Context.CorrelationId;

            btnApprove.Text = approve ? "Approving…" : btnApprove.Text;
            Application.Update(this);
            try
            {
                var seq = approve
                    ? await _approvals.ApproveAsync(command.Context, item.Id, "Approved in AccessOps")
                    : await _approvals.RejectAsync(command.Context, item.Id, "Rejected in AccessOps");

                Reload();
                pnlBanner.Show($"Request {item.Request} {(approve ? "approved" : "rejected")}. Recorded as audit entry {seq}.", StatusTone.Ok);
                _host.Report((approve ? "Approved" : "Rejected") + " — audited", StatusTone.Ok, "reference " + reference);
            }
            catch (NotAuthorizedException ex)
            {
                // refused: one sentence that does not name what it protects
                pnlBanner.Show(ex.Message, StatusTone.Bad);
                _host.Report("Refused — audited", StatusTone.Bad, "reference " + reference);
            }
            catch (InvalidOperationException)
            {
                // a failure: one sentence plus the reference that finds every line of this command
                pnlBanner.Show($"That request could not be {(approve ? "approved" : "rejected")}. Reference {reference}.", StatusTone.Bad);
                _host.Report("Failed", StatusTone.Bad, "reference " + reference);
            }
            finally
            {
                btnApprove.Text = "Approve";
                lblAudited.Text = $"{_audit.CountToday()} audited today";
            }

            Application.Update(this);
        }

        private void gridRequests_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _rows.Count || gridRequests.Columns[e.ColumnIndex].Name != "colState")
                return;

            var status = _rows[e.RowIndex].Status;
            e.CellStyle.ForeColor = status == "Pending" ? ColorTranslator.FromHtml("#b9770e") : status == "Approved" ? Theme.Ok : Theme.Bad;
            e.CellStyle.Font = Theme.Px(11f, FontStyle.Bold);
        }
    }
}
