using System.Drawing;
using AccessOps.Services;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// The approval queue as Module 2 leaves it: every request to every signed-in employee, and
    /// no Approve action yet. Module 3 adds ApprovalService with its guard and the manager scope.
    /// </summary>
    internal sealed class ApprovalsView : UserControl
    {
        public ApprovalsView(IAreaHost host)
        {
            this.Name = "viewApprovals";
            this.BackColor = Color.White;

            var grid = new DataGridView { Name = "gridRequests", Dock = DockStyle.Fill };
            DataGridStyle.Apply(grid);
            grid.Columns.Add(DataGridStyle.Column("colRequest", "Request", nameof(ApprovalListItem.Request), 108));
            grid.Columns.Add(DataGridStyle.Column("colRequester", "Requester", nameof(ApprovalListItem.Requester), 168));
            grid.Columns.Add(DataGridStyle.Column("colSubject", "Subject", nameof(ApprovalListItem.Subject), 240, fill: true));
            grid.Columns.Add(DataGridStyle.Column("colDepartment", "Department", nameof(ApprovalListItem.Department), 132));
            grid.Columns.Add(DataGridStyle.Column("colAmount", "Amount", nameof(ApprovalListItem.Amount), 104));
            grid.Columns.Add(DataGridStyle.Column("colStatus", "State", nameof(ApprovalListItem.Status), 104));
            grid.Columns["colRequest"].DefaultCellStyle.ForeColor = Theme.Accent;
            grid.Columns["colRequest"].DefaultCellStyle.Font = Theme.Mono(12.5f, FontStyle.Bold);
            grid.Columns["colAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            var rows = AppHost.Get<PortalQueries>().ListApprovals();
            grid.DataSource = rows;
            this.Controls.Add(grid);

            host.Report($"Approval queue loaded · {rows.Count} requests · no scope rule yet", StatusTone.Idle, $"{rows.Count} requests");
        }
    }
}
