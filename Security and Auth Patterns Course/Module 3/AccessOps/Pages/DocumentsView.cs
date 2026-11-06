using System.Drawing;
using AccessOps.Services;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// The documents of the caller's department (or all, with ViewAllDocuments). The rows come from
    /// <see cref="DocumentService"/>, whose query already carries the department rule. The upload
    /// button exists only for UploadDocument holders; the document store itself arrives in Module 5.
    /// </summary>
    internal sealed class DocumentsView : UserControl
    {
        public DocumentsView(IAreaHost host)
        {
            this.Name = "viewDocuments";
            this.BackColor = Color.White;

            var me = AppHost.Get<IAccessOpsSession>().Current;
            var rows = AppHost.Get<DocumentService>().List();

            var toolbar = new Panel
            {
                Name = "pnlToolbar",
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Theme.Strip,
                Padding = new Padding(16, 10, 16, 10),
                CssStyle = "border-bottom:1px solid #e4eaf1;"
            };
            toolbar.Controls.Add(new Label
            {
                Name = "lblScope",
                Text = me.Has(Permission.ViewAllDocuments) ? "scope: every department" : "scope: " + me.Department + " only · filtered in the query",
                Dock = DockStyle.Right,
                AutoSize = true,
                Font = Theme.Mono(12f),
                ForeColor = Theme.Muted,
                TextAlign = ContentAlignment.MiddleRight
            });

            // hidden, not disabled: without UploadDocument this user can never upload in AccessOps
            if (me.Has(Permission.UploadDocument))
            {
                var btnUpload = new Button { Name = "btnUpload", Text = "Upload document", Dock = DockStyle.Left, Width = 140, Enabled = false };
                Theme.StyleButton(btnUpload, primary: true);
                toolbar.Controls.Add(btnUpload);
            }

            var grid = new DataGridView { Name = "gridDocuments", Dock = DockStyle.Fill };
            DataGridStyle.Apply(grid);
            grid.Columns.Add(DataGridStyle.Column("colTitle", "Document", nameof(DocumentListItem.Title), 300, fill: true));
            grid.Columns.Add(DataGridStyle.Column("colDepartment", "Department", nameof(DocumentListItem.Department), 140));
            grid.Columns.Add(DataGridStyle.Column("colClass", "Class", nameof(DocumentListItem.Classification), 120));
            grid.Columns.Add(DataGridStyle.Column("colSize", "Size", nameof(DocumentListItem.Size), 90));
            grid.Columns.Add(DataGridStyle.Column("colUpdated", "Updated", nameof(DocumentListItem.Updated), 110));
            grid.Columns["colTitle"].DefaultCellStyle.ForeColor = Theme.Accent;
            grid.Columns["colTitle"].DefaultCellStyle.Font = Theme.Px(12.5f, FontStyle.Bold);
            grid.DataSource = rows;

            this.Controls.Add(grid);
            this.Controls.Add(toolbar);

            host.Report($"{rows.Count} documents · authorized by DocumentService", StatusTone.Ok, $"{rows.Count} documents");
        }
    }
}
