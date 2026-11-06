using System.Drawing;
using AccessOps.Services;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>Three figures and the documents this session can see.</summary>
    internal sealed class DashboardView : UserControl
    {
        public DashboardView(IAreaHost host, SessionContext me)
        {
            var queries = AppHost.Get<PortalQueries>();
            this.Name = "viewDashboard";
            this.BackColor = Color.White;
            this.Padding = new Padding(22, 18, 22, 18);

            var kpis = new TableLayoutPanel
            {
                Name = "tblKpis",
                Dock = DockStyle.Top,
                Height = 86,
                ColumnCount = 3,
                RowCount = 1
            };
            for (int i = 0; i < 3; i++)
                kpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3F));
            kpis.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            kpis.Controls.Add(Kpi("kpiApprovals", "OPEN APPROVALS", queries.CountPendingApprovals().ToString(), Theme.Accent), 0, 0);
            kpis.Controls.Add(Kpi("kpiDocuments", "DOCUMENTS", queries.CountDocuments().ToString(), Theme.Ok), 1, 0);
            kpis.Controls.Add(Kpi("kpiDepartment", "MY DEPARTMENT", me.Department, ColorTranslator.FromHtml("#7d5ae0")), 2, 0);

            var docs = queries.ListDocuments();
            var grid = new DataGridView { Name = "gridSessionDocuments", Dock = DockStyle.Fill };
            DataGridStyle.Apply(grid, 40);
            grid.Columns.Add(DataGridStyle.Column("colTitle", "Document", nameof(DocumentListItem.Title), 260, fill: true));
            grid.Columns.Add(DataGridStyle.Column("colDepartment", "Department", nameof(DocumentListItem.Department), 130));
            grid.Columns.Add(DataGridStyle.Column("colClass", "Classification", nameof(DocumentListItem.Classification), 118));
            grid.Columns["colTitle"].DefaultCellStyle.ForeColor = Theme.Accent;
            grid.Columns["colTitle"].DefaultCellStyle.Font = Theme.Px(12.5f, FontStyle.Bold);
            grid.DataSource = docs;

            var card = new Panel
            {
                Name = "pnlDocumentsCard",
                Dock = DockStyle.Fill,
                CssStyle = "border:1px solid #e4eaf1; border-radius:12px;"
            };
            card.Controls.Add(grid);
            card.Controls.Add(new Label
            {
                Name = "lblDocumentsTitle",
                Text = "Documents visible to this session",
                Dock = DockStyle.Top,
                AutoSize = false,
                Height = 42,
                Padding = new Padding(16, 0, 0, 0),
                Font = Theme.Px(13.5f, FontStyle.Bold),
                ForeColor = Theme.Ink,
                TextAlign = ContentAlignment.MiddleLeft
            });

            this.Controls.Add(card);
            this.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 16 });
            this.Controls.Add(kpis);

            host.Report($"Signed in as {me.DisplayName} — {me.Department}.", StatusTone.Ok,
                $"{docs.Count} documents · session started {me.SignedInUtc:HH:mm} UTC");
        }

        private static Control Kpi(string name, string caption, string value, Color tone)
        {
            var panel = new Panel
            {
                Name = name,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 14, 0),
                Padding = new Padding(16, 12, 16, 10),
                CssStyle = $"border:1px solid #e4eaf1; border-top:3px solid {ColorTranslator.ToHtml(tone)}; border-radius:12px;"
            };
            panel.Controls.Add(new Label
            {
                Text = value,
                Dock = DockStyle.Top,
                AutoSize = false,
                Height = 34,
                Font = Theme.Mono(24f, FontStyle.Bold),
                ForeColor = tone
            });
            panel.Controls.Add(new Label
            {
                Text = caption,
                Dock = DockStyle.Top,
                AutoSize = false,
                Height = 18,
                Font = Theme.Px(11.5f, FontStyle.Bold),
                ForeColor = Theme.Muted
            });
            return panel;
        }
    }
}
