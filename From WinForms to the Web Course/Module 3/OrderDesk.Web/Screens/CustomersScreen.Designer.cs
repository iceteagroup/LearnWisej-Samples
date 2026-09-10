namespace OrderDesk.Screens
{
    partial class CustomersScreen
    {
        // Designer-owned file. Layout: heading Dock = Top, grid Dock = Fill — no fixed positions.
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.labelHeading = new Wisej.Web.Label();
            this.gridCustomers = new Wisej.Web.DataGridView();
            this.colName = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCity = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCountry = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDiscount = new Wisej.Web.DataGridViewTextBoxColumn();
            this.SuspendLayout();
            //
            // labelHeading
            //
            this.labelHeading.AutoSize = false;
            this.labelHeading.Dock = Wisej.Web.DockStyle.Top;
            this.labelHeading.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelHeading.Height = 30;
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelHeading.Text = "Customers · reference data (CustomerService, reused as-is)";
            this.labelHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // gridCustomers  (Dock = Fill)
            //
            this.gridCustomers.AllowUserToAddRows = false;
            this.gridCustomers.AllowUserToDeleteRows = false;
            this.gridCustomers.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colName, this.colCity, this.colCountry, this.colDiscount });
            this.gridCustomers.Dock = Wisej.Web.DockStyle.Fill;
            this.gridCustomers.MultiSelect = false;
            this.gridCustomers.Name = "gridCustomers";
            this.gridCustomers.ReadOnly = true;
            this.gridCustomers.RowHeadersVisible = false;
            this.gridCustomers.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridCustomers.TabIndex = 0;
            this.gridCustomers.SelectionChanged += new System.EventHandler(this.gridCustomers_SelectionChanged);
            this.colName.HeaderText = "Customer"; this.colName.Name = "colName"; this.colName.Width = 220; this.colName.ReadOnly = true;
            this.colCity.HeaderText = "City"; this.colCity.Name = "colCity"; this.colCity.Width = 140; this.colCity.ReadOnly = true;
            this.colCountry.HeaderText = "Country"; this.colCountry.Name = "colCountry"; this.colCountry.Width = 80; this.colCountry.ReadOnly = true;
            this.colDiscount.HeaderText = "Discount"; this.colDiscount.Name = "colDiscount"; this.colDiscount.Width = 90; this.colDiscount.ReadOnly = true;
            this.colDiscount.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            //
            // CustomersScreen
            //
            this.Controls.Add(this.gridCustomers);
            this.Controls.Add(this.labelHeading);
            this.Name = "Customers";
            this.Size = new System.Drawing.Size(616, 300);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label labelHeading;
        private Wisej.Web.DataGridView gridCustomers;
        private Wisej.Web.DataGridViewTextBoxColumn colName;
        private Wisej.Web.DataGridViewTextBoxColumn colCity;
        private Wisej.Web.DataGridViewTextBoxColumn colCountry;
        private Wisej.Web.DataGridViewTextBoxColumn colDiscount;
    }
}
