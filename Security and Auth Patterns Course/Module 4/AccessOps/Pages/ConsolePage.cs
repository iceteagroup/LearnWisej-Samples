using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using AccessOps.Documents;
using AccessOps.Identity;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// The signed-in partner console of the walkthrough ("AccessOps · Documents"): the side strip
    /// Documents · Approvals · Sign out, the Documents heading with Export, the grid
    /// ID · Title · Tenant · Class and the banner. It replaces the login view on the page load
    /// that established the principal. It holds no identity of its own: every click reads
    /// <c>Application.User</c> and <see cref="ExportService"/> rechecks the grant.
    /// </summary>
    public class ConsolePage : Page
    {
        private readonly DocumentService _documents = AppHost.Get<DocumentService>();
        private readonly ExportService _export = AppHost.Get<ExportService>();

        private readonly AppBar _appBar = new AppBar();
        private readonly Dictionary<string, Label> _nav = new Dictionary<string, Label>();
        private readonly Label lblHeading = new Label();
        private readonly Button btnExport = new Button();
        private readonly DataGridView grdDocuments = new DataGridView();
        private readonly Label lblBanner = new Label();

        /// <summary>Creates the console for the session's principal.</summary>
        public ConsolePage()
        {
            this.Name = "ConsolePage";
            this.Text = "AccessOps";
            this.BackColor = Color.White;

            // side strip
            var side = new Panel
            {
                Name = "pnlNav",
                Dock = DockStyle.Left,
                Width = 150,
                BackColor = Theme.Strip,
                Padding = new Padding(10, 14, 10, 14),
                CssStyle = "border-right:1px solid #e4eaf1;"
            };
            foreach (var item in new[] { "Sign out", "Approvals", "Documents" })   // docked top, added bottom-up
            {
                var label = new Label
                {
                    Name = "nav" + item.Replace(" ", ""),
                    Text = item,
                    Dock = DockStyle.Top,
                    Height = 34,
                    Padding = new Padding(12, 0, 12, 0),
                    Font = Theme.Px(13f),
                    ForeColor = Theme.Muted,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Cursor = Cursors.Hand,
                    CssStyle = "border-radius:7px;"
                };
                label.Click += (s, e) => OnNav(item);
                _nav[item] = label;
                side.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 4 });
                side.Controls.Add(label);
            }

            // main column
            var main = new Panel { Name = "pnlMain", Dock = DockStyle.Fill, Padding = new Padding(16, 14, 16, 14) };

            var header = new Panel { Name = "pnlHeader", Dock = DockStyle.Top, Height = 32 };
            lblHeading.Name = "lblHeading";
            lblHeading.Dock = DockStyle.Fill;
            lblHeading.Font = Theme.Px(16f, FontStyle.Bold);
            lblHeading.ForeColor = Theme.Ink;
            lblHeading.TextAlign = ContentAlignment.MiddleLeft;
            btnExport.Name = "btnExport";
            btnExport.Text = "Export";
            Theme.StyleButton(btnExport, primary: true, size: 13f);
            btnExport.Dock = DockStyle.Right;
            btnExport.Width = 84;
            btnExport.Click += btnExport_Click;
            header.Controls.Add(lblHeading);
            header.Controls.Add(btnExport);

            grdDocuments.Name = "grdDocuments";
            grdDocuments.Dock = DockStyle.Top;
            grdDocuments.Height = 26 + 3 * 30 + 4;
            DataGridStyle.Apply(grdDocuments, rowHeight: 30);
            grdDocuments.ColumnHeadersHeight = 26;
            grdDocuments.CssStyle = "border:1px solid #e4eaf1; border-radius:8px;";
            grdDocuments.Columns.Add(DataGridStyle.MonoColumn("colId", "ID", nameof(GridRow.Record), 54, Theme.Accent, bold: true));
            grdDocuments.Columns.Add(DataGridStyle.Column("colTitle", "Title", nameof(GridRow.Title), 200, fill: true));
            grdDocuments.Columns.Add(DataGridStyle.Column("colTenant", "Tenant", nameof(GridRow.TenantName), 90));
            var cls = DataGridStyle.Column("colClass", "Class", nameof(GridRow.ClassBadge), 110);
            cls.AllowHtml = true;   // the markup is built here from a fixed list, never from input
            grdDocuments.Columns.Add(cls);

            lblBanner.Name = "lblBanner";
            lblBanner.Dock = DockStyle.Top;
            lblBanner.Height = 38;
            lblBanner.Padding = new Padding(12, 0, 12, 0);
            lblBanner.Font = Theme.Px(13f, FontStyle.Bold);
            lblBanner.TextAlign = ContentAlignment.MiddleLeft;
            lblBanner.Visible = false;

            main.Controls.Add(lblBanner);
            main.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 10 });
            main.Controls.Add(grdDocuments);
            main.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 10 });
            main.Controls.Add(header);

            this.Controls.Add(main);
            this.Controls.Add(side);
            this.Controls.Add(_appBar);

            OnNav("Documents");
        }

        private void OnNav(string item)
        {
            if (item == "Sign out")
            {
                SignOut();
                return;
            }

            foreach (var pair in _nav)
            {
                var active = pair.Key == item;
                pair.Value.ForeColor = active ? Theme.Accent : Theme.Muted;
                pair.Value.BackColor = active ? Theme.AccentWash : Color.Transparent;
                pair.Value.Font = Theme.Px(13f, active ? FontStyle.Bold : FontStyle.Regular);
            }

            _appBar.Screen = item;
            lblHeading.Text = item;
            Theme.ShowBanner(lblBanner, null, BannerTone.Info);
            var documents = item == "Documents";
            btnExport.Visible = documents;
            grdDocuments.Visible = documents;
            if (documents)
                LoadDocuments();
        }

        private void LoadDocuments()
        {
            var rows = _documents.List(Subject.From(Application.User));
            if (rows == null)
            {
                grdDocuments.DataSource = null;
                Theme.ShowBanner(lblBanner, DocumentResult.EndedMessage, BannerTone.Bad);
                return;
            }

            grdDocuments.DataSource = rows.Select(r => new GridRow(r)).ToList();
            if (grdDocuments.Rows.Count > 0)
                grdDocuments.CurrentCell = grdDocuments.Rows[0].Cells[0];
        }

        // Export: the screen only asks. ExportService checks the grant from the principal's
        // grant claim, the tenant and the record before the repository runs.
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (grdDocuments.CurrentRow?.DataBoundItem is not GridRow row)
            {
                Theme.ShowBanner(lblBanner, "Select a document.", BannerTone.Info);
                return;
            }

            var result = _export.Export(Application.User, row.TenantId, row.Record);
            if (!result.IsAllowed)
            {
                Theme.ShowBanner(lblBanner, result.Message, result.Decision == "failed" ? BannerTone.Info : BannerTone.Bad);
                return;
            }

            var d = result.Document;
            var text = $"{d.Title}\r\nTenant {d.Tenant} · record {d.Id} · {d.Classification}\r\n\r\n{d.Body}\r\n";
            Application.Download(new MemoryStream(Encoding.UTF8.GetBytes(text)), $"record-{d.Tenant}-{d.Id}.txt");
            Theme.ShowBanner(lblBanner, $"Record {d.Id} exported · {d.Title}", BannerTone.Ok);
        }

        // Sign-out: revoke the grants (every tab), clear the session identity, expire the host
        // cookie through the HTTPS endpoint, then end the Wisej.NET session.
        private async void SignOut()
        {
            var subject = Subject.From(Application.User);
            AppHost.Get<SignOutService>().SignOut(subject, Application.UserHostAddress);
            Application.User = null;
            Application.Session[SignInHandoff.PendingKey] = null;

            try
            {
                // POST /auth/signout with an antiforgery token; the page function keeps the word
                // "signout" for the next load. Awaited, so it runs before the session ends.
                await Application.CallAsync("AccessOpsAuth.signOut", "signout");
            }
            catch (Exception)
            {
                // the grant is already revoked; the host cookie's own grant check rejects it anyway
            }

            Application.Exit();   // "autoReload": true -> the tab reloads into a new session
        }

        /// <summary>A grid row: the list item plus display values. No body, no internal keys.</summary>
        private sealed class GridRow
        {
            public GridRow(DocumentListItem item)
            {
                this.Record = item.Record;
                this.Title = item.Title;
                this.Classification = item.Classification;
                this.TenantId = item.Tenant;
            }

            public int Record { get; }
            public string Title { get; }
            public string Classification { get; }
            public string TenantId { get; }
            public string TenantName => "Tenant " + this.TenantId;

            public string ClassBadge
            {
                get
                {
                    var color = this.Classification == "Confidential" ? "#b42318" : "#1565d8";
                    return $"<span style=\"font-size:10.5px;font-weight:800;color:{color}\">{WebUtility.HtmlEncode(this.Classification)}</span>";
                }
            }
        }
    }
}
