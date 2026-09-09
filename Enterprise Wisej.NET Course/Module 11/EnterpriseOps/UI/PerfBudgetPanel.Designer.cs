namespace EnterpriseOps.UI
{
    partial class PerfBudgetPanel
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.dgvBudgets = new Wisej.Web.DataGridView();
            this.colOperation = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colBudget = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colMeasured = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.SuspendLayout();
            //
            // dgvBudgets  (the performance budget table: Operation · Budget · Measured · Status)
            //
            this.dgvBudgets.AllowUserToAddRows = false;
            this.dgvBudgets.AllowUserToDeleteRows = false;
            this.dgvBudgets.AllowUserToResizeRows = false;
            this.dgvBudgets.AutoGenerateColumns = false;
            this.dgvBudgets.Columns.Add(this.colOperation);
            this.dgvBudgets.Columns.Add(this.colBudget);
            this.dgvBudgets.Columns.Add(this.colMeasured);
            this.dgvBudgets.Columns.Add(this.colStatus);
            this.dgvBudgets.Dock = Wisej.Web.DockStyle.Fill;
            this.dgvBudgets.Font = new System.Drawing.Font("default", 9F);
            this.dgvBudgets.MultiSelect = false;
            this.dgvBudgets.Name = "dgvBudgets";
            this.dgvBudgets.ReadOnly = true;
            this.dgvBudgets.RowHeadersVisible = false;
            this.dgvBudgets.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            //
            // colOperation
            //
            this.colOperation.HeaderText = "Operation";
            this.colOperation.Name = "colOperation";
            this.colOperation.ReadOnly = true;
            this.colOperation.Width = 400;
            //
            // colBudget
            //
            this.colBudget.HeaderText = "Budget";
            this.colBudget.Name = "colBudget";
            this.colBudget.ReadOnly = true;
            this.colBudget.Width = 110;
            //
            // colMeasured
            //
            this.colMeasured.HeaderText = "Measured";
            this.colMeasured.Name = "colMeasured";
            this.colMeasured.ReadOnly = true;
            this.colMeasured.Width = 110;
            //
            // colStatus
            //
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colStatus.Width = 110;
            //
            // PerfBudgetPanel
            //
            this.Controls.Add(this.dgvBudgets);
            this.Name = "PerfBudgetPanel";
            this.Size = new System.Drawing.Size(752, 140);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.DataGridView dgvBudgets;
        private Wisej.Web.DataGridViewTextBoxColumn colOperation;
        private Wisej.Web.DataGridViewTextBoxColumn colBudget;
        private Wisej.Web.DataGridViewTextBoxColumn colMeasured;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
    }
}
