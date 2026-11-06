using System;
using System.Drawing;
using AccessOps.Services;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// The admin console (ManageUsers): every account with its roles, its claims and the permission
    /// set they resolve to, and "Force re-resolve" for an employee whose rights just changed.
    /// </summary>
    internal sealed class AdminView : UserControl
    {
        private readonly IAreaHost _host;
        private readonly DataGridView gridAccounts = new DataGridView();

        public AdminView(IAreaHost host)
        {
            _host = host;
            this.Name = "viewAdmin";
            this.BackColor = Color.White;

            var toolbar = new Panel
            {
                Name = "pnlToolbar",
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Theme.Strip,
                Padding = new Padding(16, 10, 16, 10),
                CssStyle = "border-bottom:1px solid #e4eaf1;"
            };
            var btnReResolve = new Button { Name = "btnForceReResolve", Text = "Force re-resolve", Dock = DockStyle.Left, Width = 140 };
            Theme.StyleButton(btnReResolve, primary: true);
            btnReResolve.Click += btnReResolve_Click;
            toolbar.Controls.Add(btnReResolve);

            gridAccounts.Name = "gridAccounts";
            gridAccounts.Dock = DockStyle.Fill;
            DataGridStyle.Apply(gridAccounts);
            gridAccounts.Columns.Add(DataGridStyle.Column("colUser", "Account", nameof(AccountListItem.UserName), 110));
            gridAccounts.Columns.Add(DataGridStyle.Column("colName", "Employee", nameof(AccountListItem.DisplayName), 150));
            gridAccounts.Columns.Add(DataGridStyle.Column("colDept", "Department", nameof(AccountListItem.Department), 110));
            gridAccounts.Columns.Add(DataGridStyle.Column("colRoles", "Roles", nameof(AccountListItem.Roles), 140));
            gridAccounts.Columns.Add(DataGridStyle.Column("colClaims", "Claims", nameof(AccountListItem.Claims), 170));
            gridAccounts.Columns.Add(DataGridStyle.Column("colPermissions", "Resolves to", nameof(AccountListItem.Permissions), 300, fill: true));
            var accounts = AppHost.Get<AdminService>().ListAccounts();
            gridAccounts.DataSource = accounts;

            this.Controls.Add(gridAccounts);
            this.Controls.Add(toolbar);

            host.Report("Accounts loaded · ManageUsers checked by AdminService", StatusTone.Ok, $"{accounts.Count} accounts");
        }

        private void btnReResolve_Click(object sender, EventArgs e)
        {
            if (gridAccounts.CurrentRow?.DataBoundItem is not AccountListItem account)
            {
                _host.Report("Select an account first.", StatusTone.Idle);
                return;
            }

            try
            {
                AppHost.Get<PermissionRefreshService>().Request(account.Id);
                _host.Report($"{account.DisplayName}'s sessions re-resolve their permissions at their next action.", StatusTone.Ok);
            }
            catch (NotAuthorizedException ex)
            {
                _host.Report(ex.Message, StatusTone.Bad);
            }
        }
    }
}
