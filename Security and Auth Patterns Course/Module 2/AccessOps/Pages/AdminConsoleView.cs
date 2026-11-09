using System;
using System.Drawing;
using AccessOps.Documents;
using AccessOps.Identity;
using AccessOps.Security;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// The admin console: the grants (revoke / restore), the audit trail and the lab's
    /// read-counting repository. Only a subject whose current grant allows administration sees
    /// it; the check runs in <see cref="GrantStore"/> on every load and every click.
    /// In Debug builds it also opens the identity diagnostics view; that button and that view do
    /// not exist in a Release build.
    /// </summary>
    public class AdminConsoleView : UserControl
    {
        private readonly MainPage _page;
        private readonly GrantStore _grants = AppHost.Get<GrantStore>();
        private readonly AuditLog _audit = AppHost.Get<AuditLog>();
        private readonly CountingDocumentRepository _repository = AppHost.Get<CountingDocumentRepository>();

        private readonly Button btnRevoke = new Button();
        private readonly Button btnRestore = new Button();
        private readonly Button btnRefresh = new Button();
        private readonly Button btnFailNextRead = new Button();
        private readonly Label lblReadCount = new Label();
        private readonly DataGridView grdGrants = new DataGridView();
        private readonly DataGridView grdAudit = new DataGridView();
        private readonly Panel pnlBody = new Panel();
        private readonly StatusLine lblStatus = new StatusLine();

        public AdminConsoleView(MainPage page)
        {
            _page = page;
            this.Name = "AdminConsoleView";
            this.BackColor = Color.White;

            // action bar
            var bar = new FlowLayoutPanel
            {
                Name = "pnlAdminActions",
                Dock = DockStyle.Top,
                Height = 54,
                Padding = new Padding(16, 12, 16, 12),
                WrapContents = false,
                CssStyle = "border-bottom:1px solid #e8edf3;"
            };
            btnRevoke.Name = "btnRevoke";
            btnRevoke.Text = "Revoke";
            btnRevoke.Width = 80;
            btnRevoke.Margin = new Padding(0);
            Theme.StyleButton(btnRevoke, primary: true);
            btnRevoke.Click += (s, e) => SetRevoked(true);
            btnRestore.Name = "btnRestore";
            btnRestore.Text = "Restore";
            btnRestore.Width = 80;
            btnRestore.Margin = new Padding(8, 0, 0, 0);
            Theme.StyleButton(btnRestore, primary: false);
            btnRestore.Click += (s, e) => SetRevoked(false);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Text = "Refresh";
            btnRefresh.Width = 80;
            btnRefresh.Margin = new Padding(8, 0, 0, 0);
            Theme.StyleButton(btnRefresh, primary: false);
            btnRefresh.Click += (s, e) => LoadConsole();
            bar.Controls.AddRange(new Control[] { btnRevoke, btnRestore, btnRefresh });
#if DEBUG
            var btnDiagnostics = new Button { Name = "btnDiagnostics", Text = "Identity diagnostics", Width = 150, Margin = new Padding(8, 0, 0, 0) };
            Theme.StyleButton(btnDiagnostics, primary: false);
            btnDiagnostics.Click += (s, e) => _page.ShowScreen("Identity diagnostics", new IdentityDiagnosticsView());
            bar.Controls.Add(btnDiagnostics);
#endif

            // grants
            grdGrants.Name = "grdGrants";
            grdGrants.Dock = DockStyle.Top;
            grdGrants.Height = 28 + 3 * 32 + 2;
            DataGridStyle.Apply(grdGrants);
            grdGrants.Columns.Add(DataGridStyle.MonoColumn("colGrant", "Grant", nameof(GrantRow.Grant), 90, Theme.Accent, bold: true));
            grdGrants.Columns.Add(DataGridStyle.Column("colAccount", "Account", nameof(GrantRow.Account), 160));
            grdGrants.Columns.Add(DataGridStyle.MonoColumn("colGrantTenant", "Tenant", nameof(GrantRow.Tenant), 70, Theme.Body));
            grdGrants.Columns.Add(DataGridStyle.Column("colRole", "Role", nameof(GrantRow.Role), 110));
            grdGrants.Columns.Add(DataGridStyle.Column("colState", "State", nameof(GrantRow.State), 100, fill: true));

            // repository strip
            var repo = new FlowLayoutPanel
            {
                Name = "pnlRepository",
                Dock = DockStyle.Top,
                Height = 46,
                Padding = new Padding(16, 8, 16, 8),
                WrapContents = false,
                BackColor = Theme.Strip,
                CssStyle = "border-top:1px solid #e0e7ef; border-bottom:1px solid #e0e7ef;"
            };
            var repoLabel = Theme.FieldLabel("CountingDocumentRepository.ReadCount");
            repoLabel.Margin = new Padding(0, 7, 12, 0);
            lblReadCount.Name = "lblReadCount";
            lblReadCount.AutoSize = true;
            lblReadCount.Font = Theme.Mono(15f, FontStyle.Bold);
            lblReadCount.ForeColor = Theme.Text;
            lblReadCount.Margin = new Padding(0, 5, 24, 0);
            btnFailNextRead.Name = "btnFailNextRead";
            btnFailNextRead.Text = "Fail next read";
            btnFailNextRead.Width = 120;
            Theme.StyleButton(btnFailNextRead, primary: false);
            btnFailNextRead.Click += btnFailNextRead_Click;
            repo.Controls.AddRange(new Control[] { repoLabel, lblReadCount, btnFailNextRead });

            // audit
            grdAudit.Name = "grdAudit";
            grdAudit.Dock = DockStyle.Fill;
            DataGridStyle.Apply(grdAudit, rowHeight: 28);
            grdAudit.Columns.Add(DataGridStyle.MonoColumn("colTime", "Time", nameof(AuditRow.Time), 80, Theme.Body));
            grdAudit.Columns.Add(DataGridStyle.MonoColumn("colSubject", "Subject", nameof(AuditRow.Subject), 130, Theme.Body));
            grdAudit.Columns.Add(DataGridStyle.Column("colAction", "Action", nameof(AuditRow.Action), 110));
            grdAudit.Columns.Add(DataGridStyle.MonoColumn("colResource", "Resource", nameof(AuditRow.Resource), 110, Theme.Body));
            grdAudit.Columns.Add(DataGridStyle.Column("colDecision", "Decision", nameof(AuditRow.Decision), 90));
            grdAudit.Columns.Add(DataGridStyle.Column("colReason", "Reason", nameof(AuditRow.Reason), 110));
            grdAudit.Columns.Add(DataGridStyle.MonoColumn("colCorrelation", "Correlation", nameof(AuditRow.Correlation), 120, Theme.Body, false));
            grdAudit.Columns["colCorrelation"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            pnlBody.Name = "pnlAdminBody";
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Controls.Add(grdAudit);
            pnlBody.Controls.Add(repo);
            pnlBody.Controls.Add(grdGrants);

            this.Controls.Add(pnlBody);
            this.Controls.Add(lblStatus);
            this.Controls.Add(bar);

            LoadConsole();
        }

        private Subject Administrator => _page.SignedInUser.Subject;

        private bool LoadConsole()
        {
            var rows = _grants.List(this.Administrator);
            var allowed = rows != null;
            pnlBody.Visible = allowed;
            btnRevoke.Visible = btnRestore.Visible = btnRefresh.Visible = allowed;
            if (!allowed)
            {
                lblStatus.Show(DocumentResult.DeniedMessage, StatusTone.Deny);
                return false;
            }

            grdGrants.DataSource = rows;
            grdAudit.DataSource = _audit.Recent(12);
            lblReadCount.Text = _repository.ReadCount.ToString();
            lblStatus.Show($"{rows.Count} grants · reads {_repository.ReadCount}", StatusTone.Idle);
            return true;
        }

        private void SetRevoked(bool revoked)
        {
            if (grdGrants.CurrentRow?.DataBoundItem is not GrantRow row)
            {
                lblStatus.Show("Select a grant.", StatusTone.Idle);
                return;
            }

            var changed = _grants.SetRevoked(this.Administrator, row.GrantId, revoked);
            var selected = row.GrantId;
            if (!LoadConsole())
                return;
            Reselect(selected);

            if (!changed)
                lblStatus.Show(DocumentResult.DeniedMessage, StatusTone.Deny);
            else
                lblStatus.Show($"{row.Role} grant {row.Grant} {(revoked ? "revoked" : "restored")}.", revoked ? StatusTone.Warn : StatusTone.Ok);
        }

        private void btnFailNextRead_Click(object sender, EventArgs e)
        {
            if (!_grants.CanAdminister(this.Administrator))
            {
                LoadConsole();
                return;
            }
            _repository.FailNextRead = true;
            lblStatus.Show("The next repository read will fail.", StatusTone.Warn);
        }

        private void Reselect(string grantId)
        {
            foreach (DataGridViewRow r in grdGrants.Rows)
            {
                if (r.DataBoundItem is GrantRow g && g.GrantId == grantId)
                {
                    grdGrants.CurrentCell = r.Cells[0];
                    return;
                }
            }
        }
    }
}
