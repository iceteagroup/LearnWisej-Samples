namespace OrderDesk.Pages
{
    partial class CustomersPage
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
            this.components = new System.ComponentModel.Container();
            this.headerLabel = new Wisej.Web.Label();
            this.customersGrid = new Wisej.Web.DataGridView();
            this.colName = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCountry = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTier = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCreditLimit = new Wisej.Web.DataGridViewTextBoxColumn();
            this.footerLabel = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // headerLabel
            //
            this.headerLabel.AutoSize = false;
            this.headerLabel.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.headerLabel.Dock = Wisej.Web.DockStyle.Top;
            this.headerLabel.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.headerLabel.ForeColor = System.Drawing.Color.White;
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Padding = new Wisej.Web.Padding(16, 0, 0, 0);
            this.headerLabel.Size = new System.Drawing.Size(680, 36);
            this.headerLabel.TabStop = false;
            this.headerLabel.Text = "Customers";
            this.headerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // customersGrid  — AutoGenerateColumns = false: Customer.TaxId (sensitive) is never bound
            //
            this.customersGrid.AllowUserToAddRows = false;
            this.customersGrid.AllowUserToDeleteRows = false;
            this.customersGrid.AutoGenerateColumns = false;
            this.customersGrid.BorderStyle = Wisej.Web.BorderStyle.None;
            this.customersGrid.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colName, this.colCountry, this.colTier, this.colCreditLimit });
            this.customersGrid.Dock = Wisej.Web.DockStyle.Fill;
            this.customersGrid.MultiSelect = false;
            this.customersGrid.Name = "customersGrid";
            this.customersGrid.ReadOnly = true;
            this.customersGrid.RowHeadersVisible = false;
            this.customersGrid.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.customersGrid.TabIndex = 0;
            this.customersGrid.SelectionChanged += new System.EventHandler(this.customersGrid_SelectionChanged);
            //
            // columns
            //
            this.colName.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
            this.colName.DataPropertyName = "Name";
            this.colName.HeaderText = "Customer";
            this.colName.Name = "colName";
            this.colCountry.DataPropertyName = "Country";
            this.colCountry.HeaderText = "Country";
            this.colCountry.Name = "colCountry";
            this.colCountry.Width = 80;
            this.colTier.DataPropertyName = "Tier";
            this.colTier.HeaderText = "Tier";
            this.colTier.Name = "colTier";
            this.colTier.Width = 90;
            this.colCreditLimit.DataPropertyName = "CreditLimit";
            this.colCreditLimit.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            this.colCreditLimit.DefaultCellStyle.Format = "C0";
            this.colCreditLimit.HeaderText = "Credit limit";
            this.colCreditLimit.Name = "colCreditLimit";
            this.colCreditLimit.Width = 110;
            //
            // footerLabel
            //
            this.footerLabel.AutoSize = false;
            this.footerLabel.Dock = Wisej.Web.DockStyle.Bottom;
            this.footerLabel.Font = new System.Drawing.Font("default", 9F);
            this.footerLabel.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.footerLabel.Name = "footerLabel";
            this.footerLabel.Padding = new Wisej.Web.Padding(16, 0, 16, 0);
            this.footerLabel.Size = new System.Drawing.Size(680, 30);
            this.footerLabel.TabStop = false;
            this.footerLabel.Text = "TaxId is sensitive and is not bound to the grid (AutoGenerateColumns = false).";
            this.footerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // CustomersPage
            //
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.Controls.Add(this.customersGrid);
            this.Controls.Add(this.footerLabel);
            this.Controls.Add(this.headerLabel);
            this.Name = "CustomersPage";
            this.Size = new System.Drawing.Size(680, 436);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label headerLabel;
        private Wisej.Web.DataGridView customersGrid;
        private Wisej.Web.DataGridViewTextBoxColumn colName;
        private Wisej.Web.DataGridViewTextBoxColumn colCountry;
        private Wisej.Web.DataGridViewTextBoxColumn colTier;
        private Wisej.Web.DataGridViewTextBoxColumn colCreditLimit;
        private Wisej.Web.Label footerLabel;
    }
}
