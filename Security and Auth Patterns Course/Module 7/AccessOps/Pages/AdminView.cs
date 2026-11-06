using System;
using System.Drawing;
using System.Linq;
using AccessOps.Services;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// The admin console (ManageUsers): accounts with their roles, claims and resolved permissions;
    /// the permission editor (grant / revoke, audited in one transaction with the change); force
    /// re-resolve; and, for ViewAuditTrail holders, the audit trail with its hash-chain check.
    /// </summary>
    internal sealed class AdminView : UserControl
    {
        private readonly IAreaHost _host;
        private readonly DataGridView gridAccounts = new DataGridView();
        private readonly ComboBox cboPermission = new ComboBox();
        private readonly DataGridView gridAudit = new DataGridView();

        public AdminView(IAreaHost host)
        {
            _host = host;
            this.Name = "viewAdmin";
            this.BackColor = Color.White;
            var me = AppHost.Get<IAccessOpsSession>().Current;

            var toolbar = Toolbar("pnlToolbar");
            cboPermission.Name = "cboPermission";
            cboPermission.Dock = DockStyle.Left;
            cboPermission.Width = 220;
            cboPermission.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (var p in Enum.GetValues<Permission>())
                cboPermission.Items.Add(p.ToString());
            cboPermission.SelectedIndex = 0;

            var btnGrant = ToolbarButton("btnGrant", "Grant", 80, primary: true, (s, e) => Change(grant: true));
            var btnRevoke = ToolbarButton("btnRevoke", "Revoke", 84, primary: false, (s, e) => Change(grant: false));
            var btnReResolve = ToolbarButton("btnForceReResolve", "Force re-resolve", 140, primary: false, btnReResolve_Click);
            toolbar.Controls.Add(btnReResolve);
            toolbar.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 20 });
            toolbar.Controls.Add(btnRevoke);
            toolbar.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 10 });
            toolbar.Controls.Add(btnGrant);
            toolbar.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 10 });
            toolbar.Controls.Add(cboPermission);

            gridAccounts.Name = "gridAccounts";
            gridAccounts.Dock = DockStyle.Fill;
            DataGridStyle.Apply(gridAccounts);
            gridAccounts.Columns.Add(DataGridStyle.Column("colUser", "Account", nameof(AccountListItem.UserName), 110));
            gridAccounts.Columns.Add(DataGridStyle.Column("colName", "Employee", nameof(AccountListItem.DisplayName), 150));
            gridAccounts.Columns.Add(DataGridStyle.Column("colDept", "Department", nameof(AccountListItem.Department), 110));
            gridAccounts.Columns.Add(DataGridStyle.Column("colRoles", "Roles", nameof(AccountListItem.Roles), 140));
            gridAccounts.Columns.Add(DataGridStyle.Column("colClaims", "Claims", nameof(AccountListItem.Claims), 200));
            gridAccounts.Columns.Add(DataGridStyle.Column("colPermissions", "Resolves to", nameof(AccountListItem.Permissions), 300, fill: true));

            var accountsPanel = new Panel { Name = "pnlAccounts", Dock = DockStyle.Fill };
            accountsPanel.Controls.Add(gridAccounts);
            accountsPanel.Controls.Add(toolbar);

            this.Controls.Add(accountsPanel);

            if (me.Has(Permission.ViewAuditTrail))
            {
                var auditToolbar = Toolbar("pnlAuditToolbar");
                auditToolbar.Controls.Add(new Label
                {
                    Name = "lblAuditTitle",
                    Text = "audit_entry · insert only · hash-chained",
                    Dock = DockStyle.Fill,
                    Font = Theme.Px(13f, FontStyle.Bold),
                    ForeColor = Theme.Ink,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(12, 0, 0, 0)
                });
                auditToolbar.Controls.Add(ToolbarButton("btnVerifyChain", "Verify chain", 110, primary: false, btnVerifyChain_Click));

                gridAudit.Name = "gridAudit";
                gridAudit.Dock = DockStyle.Fill;
                DataGridStyle.Apply(gridAudit, 28);
                gridAudit.Columns.Add(DataGridStyle.Column("colSeq", "Seq", nameof(AuditRow.Seq), 60));
                gridAudit.Columns.Add(DataGridStyle.Column("colAt", "UTC", nameof(AuditRow.AtUtc), 190));
                gridAudit.Columns.Add(DataGridStyle.Column("colAction", "Action", nameof(AuditRow.Action), 170));
                gridAudit.Columns.Add(DataGridStyle.Column("colActor", "Actor", nameof(AuditRow.Actor), 150));
                gridAudit.Columns.Add(DataGridStyle.Column("colTarget", "Target", nameof(AuditRow.Target), 170));
                gridAudit.Columns.Add(DataGridStyle.Column("colOutcome", "Outcome", nameof(AuditRow.Outcome), 120));
                gridAudit.Columns.Add(DataGridStyle.Column("colCorrelation", "Correlation", nameof(AuditRow.CorrelationId), 110, fill: true));
                gridAudit.Columns["colCorrelation"].DefaultCellStyle.Font = Theme.Mono(12f);

                var auditPanel = new Panel { Name = "pnlAudit", Dock = DockStyle.Bottom, Height = 330, CssStyle = "border-top:1px solid #e4eaf1;" };
                auditPanel.Controls.Add(gridAudit);
                auditPanel.Controls.Add(auditToolbar);
                this.Controls.Add(auditPanel);
            }

            ReloadAccounts();
            host.Report("Accounts loaded · ManageUsers checked by AdminService", StatusTone.Ok, $"{_accounts} accounts");
        }

        private int _accounts;

        private void ReloadAccounts()
        {
            // keep the selected account selected: the next Grant / Revoke must not land on another row
            var keep = SelectedAccount?.Id;
            var accounts = AppHost.Get<AdminService>().ListAccounts();
            _accounts = accounts.Count;
            gridAccounts.DataSource = accounts;
            var index = keep == null ? -1 : accounts.ToList().FindIndex(a => a.Id == keep);
            if (index >= 0)
                gridAccounts.CurrentCell = gridAccounts.Rows[index].Cells[0];
            if (AppHost.Get<IAccessOpsSession>().Current.Has(Permission.ViewAuditTrail))
                gridAudit.DataSource = AppHost.Get<AuditService>().Latest(40);
        }

        private AccountListItem SelectedAccount => gridAccounts.CurrentRow?.DataBoundItem as AccountListItem;

        private void Change(bool grant)
        {
            var account = SelectedAccount;
            if (account == null)
            {
                _host.Report("Select an account first.", StatusTone.Idle);
                return;
            }

            var permission = Enum.Parse<Permission>((string)cboPermission.SelectedItem);
            using var command = Command.Begin((grant ? "Permission.Grant " : "Permission.Revoke ") + account.Id);
            try
            {
                var editor = AppHost.Get<PermissionEditorService>();
                var seq = grant ? editor.Grant(command.Context, account.Id, permission) : editor.Revoke(command.Context, account.Id, permission);
                ReloadAccounts();
                _host.Report($"{(grant ? "Granted" : "Revoked")} {permission} for {account.DisplayName} · audit entry {seq} · takes effect at their next sign-in or re-resolve",
                    StatusTone.Ok, "reference " + command.Context.CorrelationId);
            }
            catch (NotAuthorizedException ex)
            {
                _host.Report(ex.Message, StatusTone.Bad, "reference " + command.Context.CorrelationId);
            }
        }

        private void btnReResolve_Click(object sender, EventArgs e)
        {
            var account = SelectedAccount;
            if (account == null)
            {
                _host.Report("Select an account first.", StatusTone.Idle);
                return;
            }

            AppHost.Get<PermissionRefreshService>().Request(account.Id);
            _host.Report($"{account.DisplayName}'s sessions re-resolve their permissions at their next action.", StatusTone.Ok);
        }

        private void btnVerifyChain_Click(object sender, EventArgs e)
        {
            var broken = AppHost.Get<AuditService>().VerifyChain();
            ReloadAccounts();
            _host.Report(broken == null ? "Audit chain verified · every entry matches" : $"Audit chain BROKEN at entry {broken}",
                broken == null ? StatusTone.Ok : StatusTone.Bad);
        }

        private static Panel Toolbar(string name) => new Panel
        {
            Name = name,
            Dock = DockStyle.Top,
            Height = 50,
            BackColor = Theme.Strip,
            Padding = new Padding(16, 10, 16, 10),
            CssStyle = "border-bottom:1px solid #e4eaf1;"
        };

        private static Button ToolbarButton(string name, string text, int width, bool primary, EventHandler click)
        {
            var b = new Button { Name = name, Text = text, Dock = name == "btnVerifyChain" ? DockStyle.Right : DockStyle.Left, Width = width };
            Theme.StyleButton(b, primary);
            b.Click += click;
            return b;
        }
    }
}
