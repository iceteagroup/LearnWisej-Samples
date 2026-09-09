namespace WisejTrainingApp.Views
{
    partial class CustomersView
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
            this.lblPageTitle = new Wisej.Web.Label();
            this.lblPageDescription = new Wisej.Web.Label();
            this.cardCustomers = new Wisej.Web.Panel();
            this.lblCustomersTitle = new Wisej.Web.Label();
            this.lstCustomers = new Wisej.Web.ListBox();
            this.lblCustomersSummary = new Wisej.Web.Label();
            this.cardCustomers.SuspendLayout();
            this.SuspendLayout();
            //
            // lblPageTitle
            //
            this.lblPageTitle.AutoSize = false;
            this.lblPageTitle.Font = new System.Drawing.Font("default", 18F, System.Drawing.FontStyle.Bold);
            this.lblPageTitle.Location = new System.Drawing.Point(32, 24);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Size = new System.Drawing.Size(600, 34);
            this.lblPageTitle.Text = "Customers";
            //
            // lblPageDescription
            //
            this.lblPageDescription.AutoSize = false;
            this.lblPageDescription.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblPageDescription.Location = new System.Drawing.Point(32, 60);
            this.lblPageDescription.Name = "lblPageDescription";
            this.lblPageDescription.Size = new System.Drawing.Size(1000, 22);
            this.lblPageDescription.Text = "Who the tickets belong to — derived from the ticket queue, same padding and card width as every other page.";
            //
            // cardCustomers  (524 × 340 at 32,100)
            //
            this.cardCustomers.BackColor = System.Drawing.Color.White;
            this.cardCustomers.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardCustomers.Controls.Add(this.lblCustomersTitle);
            this.cardCustomers.Controls.Add(this.lstCustomers);
            this.cardCustomers.Controls.Add(this.lblCustomersSummary);
            this.cardCustomers.Location = new System.Drawing.Point(32, 100);
            this.cardCustomers.Name = "cardCustomers";
            this.cardCustomers.Size = new System.Drawing.Size(524, 340);
            //
            // lblCustomersTitle
            //
            this.lblCustomersTitle.AutoSize = false;
            this.lblCustomersTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblCustomersTitle.Location = new System.Drawing.Point(20, 14);
            this.lblCustomersTitle.Name = "lblCustomersTitle";
            this.lblCustomersTitle.Size = new System.Drawing.Size(484, 28);
            this.lblCustomersTitle.Text = "Customers  ·  tickets per customer";
            //
            // lstCustomers
            //
            this.lstCustomers.Font = new System.Drawing.Font("monospace", 9F);
            this.lstCustomers.Location = new System.Drawing.Point(20, 52);
            this.lstCustomers.Name = "lstCustomers";
            this.lstCustomers.Size = new System.Drawing.Size(484, 236);
            //
            // lblCustomersSummary
            //
            this.lblCustomersSummary.AutoSize = false;
            this.lblCustomersSummary.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCustomersSummary.Location = new System.Drawing.Point(20, 296);
            this.lblCustomersSummary.Name = "lblCustomersSummary";
            this.lblCustomersSummary.Size = new System.Drawing.Size(484, 24);
            this.lblCustomersSummary.Text = "";
            //
            // CustomersView
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.lblPageTitle);
            this.Controls.Add(this.lblPageDescription);
            this.Controls.Add(this.cardCustomers);
            this.Name = "CustomersView";
            this.Size = new System.Drawing.Size(1148, 620);
            this.cardCustomers.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblPageTitle;
        private Wisej.Web.Label lblPageDescription;
        private Wisej.Web.Panel cardCustomers;
        private Wisej.Web.Label lblCustomersTitle;
        private Wisej.Web.ListBox lstCustomers;
        private Wisej.Web.Label lblCustomersSummary;
    }
}
