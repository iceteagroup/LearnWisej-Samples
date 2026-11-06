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
    /// The portal the user lands on once the SessionContext has been built: who signed in, how,
    /// and the documents in their scope. For an external sign-in the job title comes from the
    /// provider's HR directory, called with the access token (refreshed in the back channel).
    /// </summary>
    internal sealed class DashboardView : UserControl
    {
        private readonly IAreaHost _host;
        private readonly SessionContext _me;
        private readonly Label lblRole = new Label();

        public DashboardView(IAreaHost host, SessionContext me)
        {
            _host = host;
            _me = me;
            this.Name = "viewDashboard";
            this.BackColor = Color.White;
            this.Padding = new Padding(26, 22, 26, 22);

            var external = AppHost.Get<IExternalSignIn>().Current;
            var docs = me.Has(Permission.ViewDepartmentDocuments) || me.Has(Permission.ViewAllDocuments)
                ? AppHost.Get<DocumentService>().List()
                : new List<DocumentListItem>();

            // header: initials, name, role · department · how, and the chip
            var header = new Panel { Name = "pnlIdentity", Dock = DockStyle.Top, Height = 46 };
            var initials = new Label
            {
                Name = "lblInitials",
                Text = string.Concat(me.DisplayName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(p => p[0])).ToUpperInvariant(),
                Dock = DockStyle.Left,
                AutoSize = false,
                Width = 42,
                BackColor = Theme.AccentWash,
                ForeColor = Theme.Accent,
                Font = Theme.Px(16f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                CssStyle = "border-radius:999px; border:1px solid #c5ddff;"
            };
            var chip = new Label
            {
                Name = "lblContextChip",
                Text = external?.Provisioned == true ? "SessionContext built · provisioned, no roles" : "SessionContext built",
                Dock = DockStyle.Right,
                AutoSize = true,
                Padding = new Padding(13, 6, 13, 6),
                BackColor = ColorTranslator.FromHtml("#e8f6ee"),
                ForeColor = ColorTranslator.FromHtml("#1f7a52"),
                Font = Theme.Px(11.5f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                CssStyle = "border-radius:999px; border:1px solid #a9dcc2; margin:9px 0;"
            };
            var name = new Label { Name = "lblName", Text = me.DisplayName, Dock = DockStyle.Top, AutoSize = false, Height = 24, Font = Theme.Px(17f, FontStyle.Bold), ForeColor = Theme.Ink };
            lblRole.Name = "lblRole";
            lblRole.Dock = DockStyle.Top;
            lblRole.AutoSize = false;
            lblRole.Height = 20;
            lblRole.Font = Theme.Px(12.5f);
            lblRole.ForeColor = Theme.Muted;
            lblRole.Text = RoleLine(null, external);
            var text = new Panel { Dock = DockStyle.Fill, Padding = new Padding(14, 0, 0, 0) };
            text.Controls.Add(lblRole);
            text.Controls.Add(name);
            header.Controls.Add(text);
            header.Controls.Add(chip);
            header.Controls.Add(initials);

            var grid = new DataGridView { Name = "gridSessionDocuments", Dock = DockStyle.Fill };
            DataGridStyle.Apply(grid, 31);
            grid.Columns.Add(DataGridStyle.Column("colTitle", "Document", nameof(DocumentListItem.Title), 260, fill: true));
            grid.Columns.Add(DataGridStyle.Column("colDepartment", "Department", nameof(DocumentListItem.Department), 130));
            grid.Columns.Add(DataGridStyle.Column("colClass", "Classification", nameof(DocumentListItem.Classification), 130));
            grid.Columns["colTitle"].DefaultCellStyle.ForeColor = Theme.Accent;
            grid.Columns["colTitle"].DefaultCellStyle.Font = Theme.Px(12.5f, FontStyle.Bold);
            grid.DataSource = docs;

            this.Controls.Add(grid);
            this.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 18 });
            this.Controls.Add(header);

            var how = external == null ? "Signed in with a service account" : "Signed in via " + external.Provider;
            var subject = external == null ? "" : "sub " + Short(external.Subject) + " · ";
            host.Report(how, StatusTone.Ok, $"{subject}{docs.Count} documents in scope");

            if (external != null)
                this.Load += async (s, e) =>
                {
                    var (jobTitle, expired) = await AppHost.Get<HrProfileClient>().GetJobTitleAsync();
                    if (expired)
                    {
                        _host.Report("Your sign-in at " + external.Provider + " has expired. Signing out…", StatusTone.Bad);
                        Application.Update(this);
                        SignOutFlow.SignOut(SignOutReason.TokenExpired);
                        return;
                    }
                    lblRole.Text = RoleLine(jobTitle, external);
                    Application.Update(this);
                };
        }

        private string RoleLine(string jobTitle, ExternalSessionInfo external)
        {
            var role = jobTitle ?? (_me.Roles.Count == 0 ? "No role assigned yet" : string.Join(", ", _me.Roles));
            var via = external == null ? "service account password" : "signed in via " + external.Provider;
            return $"{role} · {_me.Department} · {via}";
        }

        private static string Short(string subject) => subject.Length <= 9 ? subject : subject.Substring(0, 9);
    }
}
