using System;
using System.Drawing;
using AccessOps.Services;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// The employee directory. The grid is bound to <see cref="EmployeeListItem"/>, the projection
    /// every viewer may see, so a salary band or a review note cannot reach the browser from this
    /// screen: it is never put on a control. Hiding a column would only change what is drawn.
    /// </summary>
    internal sealed class EmployeesView : UserControl
    {
        private readonly MainPage _shell;
        private readonly EmployeeDirectoryService _employees = AppHost.Get<EmployeeDirectoryService>();

        private readonly DataGridView gridEmployees = new DataGridView();
        private readonly Panel pnlActions = new Panel();
        private readonly Button btnOpenRecord = new Button();
        private readonly Button btnDownloadContract = new Button();
        private readonly Button btnApproveRequest = new Button();

        public EmployeesView(MainPage shell)
        {
            _shell = shell;
            this.Name = "viewEmployees";
            this.BackColor = Color.White;

            gridEmployees.Name = "gridEmployees";
            gridEmployees.Dock = DockStyle.Fill;
            DataGridStyle.Apply(gridEmployees);
            gridEmployees.Columns.Add(DataGridStyle.Column("colEmployee", "Employee", nameof(EmployeeListItem.DisplayName), 250));
            gridEmployees.Columns.Add(DataGridStyle.Column("colDepartment", "Department", nameof(EmployeeListItem.Department), 180));
            gridEmployees.Columns.Add(DataGridStyle.Column("colRole", "Role", nameof(EmployeeListItem.Role), 180));
            gridEmployees.Columns.Add(DataGridStyle.Column("colManager", "Manager", nameof(EmployeeListItem.ManagerName), 150, fill: true));
            gridEmployees.Columns["colEmployee"].DefaultCellStyle.ForeColor = Theme.Accent;
            gridEmployees.Columns["colEmployee"].DefaultCellStyle.Font = Theme.Px(12.5f, FontStyle.Bold);

            pnlActions.Name = "pnlActions";
            pnlActions.Dock = DockStyle.Bottom;
            pnlActions.Height = 54;
            pnlActions.Padding = new Padding(14, 11, 14, 11);
            pnlActions.CssStyle = "border-top:1px solid #e4eaf1;";

            SetupButton(btnApproveRequest, "btnApproveRequest", "Approve request", 140, primary: false);
            SetupButton(btnDownloadContract, "btnDownloadContract", "Download contract", 150, primary: false);
            SetupButton(btnOpenRecord, "btnOpenRecord", "Open record", 112, primary: true);

            // Nothing behind these two in Module 1: there are no contracts and no approvals yet.
            btnDownloadContract.Enabled = false;
            btnApproveRequest.Enabled = false;
            btnOpenRecord.Click += btnOpenRecord_Click;

            pnlActions.Controls.Add(btnApproveRequest);
            pnlActions.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 10 });
            pnlActions.Controls.Add(btnDownloadContract);
            pnlActions.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 10 });
            pnlActions.Controls.Add(btnOpenRecord);

            this.Controls.Add(gridEmployees);
            this.Controls.Add(pnlActions);

            LoadDirectory();
        }

        private static void SetupButton(Button button, string name, string text, int width, bool primary)
        {
            button.Name = name;
            button.Text = text;
            button.Dock = DockStyle.Left;
            button.Width = width;
            Theme.StyleButton(button, primary);
        }

        private void LoadDirectory()
        {
            // the server decides what leaves the process: a projection, not the employee table
            var rows = _employees.GetDirectory();
            gridEmployees.DataSource = rows;

            _shell.WireNote = "Directory projection · name, department, role, manager";
            _shell.Report(
                $"Directory loaded · {rows.Count} rows, {gridEmployees.Columns.Count} fields each",
                StatusTone.Ok,
                $"{rows.Count} rows");
        }

        private void btnOpenRecord_Click(object sender, EventArgs e)
        {
            if (gridEmployees.CurrentRow?.DataBoundItem is not EmployeeListItem item)
            {
                _shell.Report("Select an employee first.", StatusTone.Idle);
                return;
            }

            _shell.Report(
                $"Record {item.Id} opened · {item.DisplayName} · {item.Department} · {item.Role} — directory fields only",
                StatusTone.Ok);
        }
    }
}
