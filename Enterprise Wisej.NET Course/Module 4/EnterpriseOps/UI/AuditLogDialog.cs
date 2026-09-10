using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Services.Queries;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// Read-only view of what the commands recorded. It is handed a list of
    /// <see cref="AuditLogRow"/> projections — it never queries anything itself, and it never sees an
    /// <c>AuditEntry</c> entity or a DbContext.
    ///
    /// Opened with <c>await dialog.ShowDialogAsync()</c> from an async handler (never a blocking
    /// <c>ShowDialog()</c>, which does not exist in a Wisej.NET session).
    /// </summary>
    public partial class AuditLogDialog : Form
    {
        public AuditLogDialog(string tenantId, List<AuditLogRow> rows)
        {
            InitializeComponent();

            rows = rows ?? new List<AuditLogRow>();
            int rejected = rows.Count(r => r.Outcome == "Rejected");

            lblAuditTitle.Text = $"Audit log · tenant {tenantId} · {rows.Count} rows ({rows.Count - rejected} committed, {rejected} rejected)";
            dgvAudit.DataSource = new BindingSource { DataSource = rows };
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
