using System.Drawing;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// An area that exists in the navigation but asks the server for nothing yet. Empty is the
    /// honest state: no rows were loaded, so none are on the wire.
    /// </summary>
    internal sealed class EmptyAreaView : UserControl
    {
        public EmptyAreaView(MainPage shell, string area)
        {
            this.Name = "view" + area.Replace(" ", "");
            this.BackColor = Color.White;

            var title = new Label
            {
                Name = "lblEmptyTitle",
                Text = "Nothing loaded for this session",
                Dock = DockStyle.Top,
                AutoSize = false,
                Height = 24,
                Font = Theme.Px(14f, FontStyle.Bold),
                ForeColor = Theme.Muted,
                TextAlign = ContentAlignment.MiddleCenter
            };
            var detail = new Label
            {
                Name = "lblEmptyDetail",
                Text = "The server has not been asked for " + DescribeArea(area) + " yet.",
                Dock = DockStyle.Top,
                AutoSize = false,
                Height = 22,
                Font = Theme.Px(12.5f),
                ForeColor = ColorTranslator.FromHtml("#8a9aab"),
                TextAlign = ContentAlignment.MiddleCenter
            };

            this.Padding = new Padding(0, 70, 0, 0);
            this.Controls.Add(detail);
            this.Controls.Add(title);

            shell.WireNote = "nothing requested · 0 fields on the wire";
            shell.Report(area + " opened · nothing loaded", StatusTone.Idle, "0 rows");
        }

        private static string DescribeArea(string area) => area switch
        {
            "Departments" => "a department list",
            "Documents" => "a document list",
            "Approvals" => "an approval queue",
            "Admin console" => "an administration view",
            _ => "data"
        };
    }
}
