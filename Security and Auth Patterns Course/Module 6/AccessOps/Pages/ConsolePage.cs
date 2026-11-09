using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using AccessOps.Documents;
using AccessOps.Identity;
using AccessOps.Security;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// The partner portal of Module 5 ("AccessOps · Records"): the person (name, role, tenant and
    /// how they signed in - all from AccessOps data and the session principal), the grid
    /// Record · Tenant · Class, <b>Export record</b>, <b>Sign out</b> and the status strip. It holds
    /// no identity of its own: every click reads <c>Application.User</c> and
    /// <see cref="ExportService"/> rechecks the grant.
    /// </summary>
    public class ConsolePage : Page
    {
        private readonly DocumentService _documents = AppHost.Get<DocumentService>();
        private readonly ExportService _export = AppHost.Get<ExportService>();
        private readonly IExternalSignIn _external = AppHost.Get<IExternalSignIn>();

        private readonly DataGridView grdRecords = new DataGridView();
        private readonly Button btnExport = new Button();
        private readonly Button btnSignOut = new Button();
        private readonly Button btnMyPasskeys = new Button();
        private readonly Banner _banner = new Banner();
        private readonly StatusStrip _status = new StatusStrip();

        /// <summary>Creates the portal for the session's principal.</summary>
        public ConsolePage()
        {
            this.Name = "ConsolePage";
            this.Text = "AccessOps";
            this.BackColor = Color.White;

            var principal = Application.User as ClaimsPrincipal;
            var subject = Subject.From(principal);
            var name = principal?.FindFirst(ClaimTypes.Name)?.Value ?? "";
            var method = principal?.FindFirst(ApplicationPrincipal.MethodClaim)?.Value;
            var role = subject.Roles.FirstOrDefault() ?? "Member";
            var via = method == "oidc" ? "signed in via " + _external.ProviderName
                : method == "passkey" ? "signed in with a passkey"
                : "signed in with a service account";

            var appBar = new AppBar { Screen = "Records" };
            var body = new Panel { Name = "pnlBody", Dock = DockStyle.Left, Width = 640, Padding = new Padding(24, 20, 24, 0) };

            // the person
            var who = new Panel { Name = "pnlWho", Dock = DockStyle.Top, Height = 42 };
            var avatar = new Label
            {
                Name = "lblAvatar",
                Text = Initials(name),
                Dock = DockStyle.Left,
                Width = 40,
                Font = Theme.Px(15f, FontStyle.Bold),
                ForeColor = Theme.Accent,
                BackColor = Theme.AccentWash,
                TextAlign = ContentAlignment.MiddleCenter,
                CssStyle = "border:1px solid #c5ddff; border-radius:999px;"
            };
            var lines = new Panel { Dock = DockStyle.Fill, Padding = new Padding(14, 0, 0, 0) };
            var lblName = new Label { Name = "lblName", Text = name, Dock = DockStyle.Top, Height = 22, Font = Theme.Px(16.5f, FontStyle.Bold), ForeColor = Theme.Ink };
            var lblWho = new Label { Name = "lblWho", Text = $"{role} · Tenant {subject.Tenant} · {via}", Dock = DockStyle.Top, Height = 20, Font = Theme.Px(12.5f), ForeColor = Theme.Muted };
            lines.Controls.Add(lblWho);
            lines.Controls.Add(lblName);
            who.Controls.Add(lines);
            who.Controls.Add(avatar);

            grdRecords.Name = "grdRecords";
            grdRecords.Dock = DockStyle.Top;
            grdRecords.Height = 26 + 3 * 30 + 2;
            DataGridStyle.Apply(grdRecords, rowHeight: 30);
            grdRecords.ColumnHeadersHeight = 26;
            grdRecords.Columns.Add(DataGridStyle.Column("colRecord", "Record", nameof(GridRow.RecordTitle), 200, fill: true));
            grdRecords.Columns["colRecord"].DefaultCellStyle.ForeColor = Theme.Accent;
            grdRecords.Columns["colRecord"].DefaultCellStyle.Font = Theme.Px(12.5f, FontStyle.Bold);
            grdRecords.Columns.Add(DataGridStyle.Column("colTenant", "Tenant", nameof(GridRow.TenantName), 100));
            var cls = DataGridStyle.Column("colClass", "Class", nameof(GridRow.ClassPill), 120);
            cls.AllowHtml = true;   // fixed markup from a fixed list, never input
            grdRecords.Columns.Add(cls);

            var buttons = new Panel { Name = "pnlButtons", Dock = DockStyle.Top, Height = 32 };
            btnExport.Name = "btnExport";
            btnExport.Text = "Export record";
            Theme.StyleButton(btnExport, primary: true, size: 12.5f);
            btnExport.Dock = DockStyle.Left;
            btnExport.Width = 116;
            btnExport.Click += btnExport_Click;
            btnSignOut.Name = "btnSignOut";
            btnSignOut.Text = "Sign out";
            Theme.StyleButton(btnSignOut, primary: false, size: 12.5f);
            btnSignOut.Dock = DockStyle.Left;
            btnSignOut.Width = 84;
            btnSignOut.Click += btnSignOut_Click;
            btnMyPasskeys.Name = "btnMyPasskeys";
            btnMyPasskeys.Text = "My passkeys";
            Theme.StyleButton(btnMyPasskeys, primary: false, size: 12.5f);
            btnMyPasskeys.Dock = DockStyle.Left;
            btnMyPasskeys.Width = 104;
            btnMyPasskeys.Click += (s, e) => Program.Show(new CredentialReviewView());
            buttons.Controls.Add(btnMyPasskeys);
            buttons.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 10 });
            buttons.Controls.Add(btnSignOut);
            buttons.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 10 });
            buttons.Controls.Add(btnExport);

            _banner.Dock = DockStyle.Top;
            body.Controls.Add(_banner);
            body.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 14 });
            body.Controls.Add(buttons);
            body.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 14 });
            body.Controls.Add(grdRecords);
            body.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 16 });
            body.Controls.Add(who);

            this.Controls.Add(body);
            this.Controls.Add(_status);
            this.Controls.Add(appBar);

            var rows = _documents.List(subject);
            if (rows == null)
            {
                _status.Show(DocumentResult.EndedMessage, StatusTone.Bad, "grant not active");
                return;
            }
            grdRecords.DataSource = rows.Select(r => new GridRow(r)).ToList();
            if (grdRecords.Rows.Count > 0)
                grdRecords.CurrentCell = grdRecords.Rows[0].Cells[0];
            if (method == "passkey" && Application.Session["PasskeyReference"] is string reference)
            {
                // the page load right after a passkey sign-in (the session ID was just renewed)
                Application.Session["PasskeyReference"] = null;
                _banner.Show($"Signed in as {name}. Your session was renewed.", ok: true);
                _status.Show("Passkey verified · grant issued", StatusTone.Ok, "reference " + reference);
                return;
            }
            _status.Show(method == "oidc" ? "Signed in via " + _external.ProviderName
                    : method == "passkey" ? "Signed in with a passkey" : "Signed in · service account",
                StatusTone.Ok, $"grant {Grant.Shorten(subject.GrantId)} · tenant from AccessOps");
        }

        private static string Initials(string name)
            => string.Concat((name ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(p => char.ToUpperInvariant(p[0])));

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (grdRecords.CurrentRow?.DataBoundItem is not GridRow row)
                return;

            var result = _export.Export(Application.User, row.TenantId, row.Record);
            if (!result.IsAllowed)
            {
                var status = result.Reason == "grant" ? "Export denied · grant revoked" : result.Message;
                _status.Show(status, StatusTone.Bad, result.Reason == "grant" || result.Reason == "tenant" || result.Reason == "role" ? "repository reads: 0" : "");
                return;
            }

            var d = result.Document;
            var text = $"{d.Title}\r\nTenant {d.Tenant} · record {d.Id} · {d.Classification}\r\n\r\n{d.Body}\r\n";
            Application.Download(new MemoryStream(Encoding.UTF8.GetBytes(text)), $"record-{d.Tenant}-{d.Id}.txt");
            _status.Show($"Record {d.Id} exported", StatusTone.Ok, "repository reads: 1");
        }

        // Sign-out ends two sessions. Here: revoke the grants, clear the identity, expire the host
        // cookie, end the Wisej.NET session. Then Default.html posts to /account/signout for a
        // federated sign-in, which ends the provider session.
        private async void btnSignOut_Click(object sender, EventArgs e)
        {
            var principal = Application.User as ClaimsPrincipal;
            var subject = Subject.From(principal);
            var external = principal?.FindFirst(ApplicationPrincipal.MethodClaim)?.Value == "oidc";

            AppHost.Get<SignOutService>().SignOut(subject, Application.UserHostAddress);
            Application.User = null;
            Application.Session[SignInHandoff.PendingKey] = null;

            try
            {
                await Application.CallAsync("AccessOpsAuth.signOut", external ? "signout-external" : "signout");
            }
            catch (Exception)
            {
                // the grant is already revoked; the host cookie's own grant check rejects it anyway
            }

            Application.Exit();   // "autoReload": true -> the tab reloads
        }

        /// <summary>A grid row: display values only.</summary>
        private sealed class GridRow
        {
            public GridRow(DocumentListItem item)
            {
                this.Record = item.Record;
                this.RecordTitle = $"Record {item.Record} · {item.Title}";
                this.Classification = item.Classification;
                this.TenantId = item.Tenant;
            }

            public int Record { get; }
            public string RecordTitle { get; }
            public string Classification { get; }
            public string TenantId { get; }
            public string TenantName => "Tenant " + this.TenantId;

            public string ClassPill
            {
                get
                {
                    var confidential = this.Classification == "Confidential";
                    var style = confidential
                        ? "color:#b8611a;background:#fdf1e3;border:1px solid #eccaa4"
                        : "color:#5a6b7d;background:#f1f4f8;border:1px solid #dde4ec";
                    return $"<span style=\"font-size:11px;font-weight:800;border-radius:999px;padding:2px 9px;{style}\">{WebUtility.HtmlEncode(this.Classification)}</span>";
                }
            }
        }
    }
}
