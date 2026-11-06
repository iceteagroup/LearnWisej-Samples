using System.Drawing;
using AccessOps.Services;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// The document list as Module 2 leaves it: every document to every signed-in employee.
    /// Module 3 moves the department rule into the query.
    /// </summary>
    internal sealed class DocumentsView : UserControl
    {
        public DocumentsView(IAreaHost host)
        {
            this.Name = "viewDocuments";
            this.BackColor = Color.White;

            var grid = new DataGridView { Name = "gridDocuments", Dock = DockStyle.Fill };
            DataGridStyle.Apply(grid);
            grid.Columns.Add(DataGridStyle.Column("colTitle", "Document", nameof(DocumentListItem.Title), 300, fill: true));
            grid.Columns.Add(DataGridStyle.Column("colDepartment", "Department", nameof(DocumentListItem.Department), 140));
            grid.Columns.Add(DataGridStyle.Column("colClass", "Class", nameof(DocumentListItem.Classification), 120));
            grid.Columns.Add(DataGridStyle.Column("colSize", "Size", nameof(DocumentListItem.Size), 90));
            grid.Columns.Add(DataGridStyle.Column("colUpdated", "Updated", nameof(DocumentListItem.Updated), 110));
            grid.Columns["colTitle"].DefaultCellStyle.ForeColor = Theme.Accent;
            grid.Columns["colTitle"].DefaultCellStyle.Font = Theme.Px(12.5f, FontStyle.Bold);

            var rows = AppHost.Get<PortalQueries>().ListDocuments();
            grid.DataSource = rows;
            this.Controls.Add(grid);

            host.Report($"Documents loaded · {rows.Count} rows · no department rule yet", StatusTone.Idle, $"{rows.Count} documents");
        }
    }
}
