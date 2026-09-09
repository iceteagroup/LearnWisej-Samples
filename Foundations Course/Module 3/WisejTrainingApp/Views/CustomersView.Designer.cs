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
            this.components = new System.ComponentModel.Container();
            this.lblViewTitle = new Wisej.Web.Label();
            this.lblCustomersSummary = new Wisej.Web.Label();
            this.pnlCustomers = new Wisej.Web.Panel();
            this.lblListHeader = new Wisej.Web.Label();
            this.lstCustomers = new Wisej.Web.ListBox();
            this.btnAddCustomer = new Wisej.Web.Button();
            this.btnViewSelected = new Wisej.Web.Button();
            this.lblPermissionNote = new Wisej.Web.Label();
            this.pnlCustomers.SuspendLayout();
            this.SuspendLayout();
            //
            // lblViewTitle
            //
            this.lblViewTitle.AutoSize = false;
            this.lblViewTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.lblViewTitle.Location = new System.Drawing.Point(0, 0);
            this.lblViewTitle.Name = "lblViewTitle";
            this.lblViewTitle.Size = new System.Drawing.Size(400, 30);
            this.lblViewTitle.Text = "Customers  ·  CustomersView (UserControl)";
            //
            // lblCustomersSummary
            //
            this.lblCustomersSummary.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblCustomersSummary.AutoSize = false;
            this.lblCustomersSummary.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCustomersSummary.Location = new System.Drawing.Point(780, 0);
            this.lblCustomersSummary.Name = "lblCustomersSummary";
            this.lblCustomersSummary.Size = new System.Drawing.Size(300, 30);
            this.lblCustomersSummary.Text = "";
            this.lblCustomersSummary.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // pnlCustomers  (the list card)
            //
            this.pnlCustomers.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlCustomers.BackColor = System.Drawing.Color.White;
            this.pnlCustomers.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlCustomers.Controls.Add(this.lblListHeader);
            this.pnlCustomers.Controls.Add(this.lstCustomers);
            this.pnlCustomers.Controls.Add(this.btnAddCustomer);
            this.pnlCustomers.Controls.Add(this.btnViewSelected);
            this.pnlCustomers.Controls.Add(this.lblPermissionNote);
            this.pnlCustomers.Location = new System.Drawing.Point(0, 44);
            this.pnlCustomers.Name = "pnlCustomers";
            this.pnlCustomers.Size = new System.Drawing.Size(1080, 492);
            //
            // lblListHeader
            //
            this.lblListHeader.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblListHeader.AutoSize = false;
            this.lblListHeader.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.lblListHeader.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblListHeader.Location = new System.Drawing.Point(20, 16);
            this.lblListHeader.Name = "lblListHeader";
            this.lblListHeader.Size = new System.Drawing.Size(1040, 24);
            this.lblListHeader.Text = " #  Name                   Contact          City       Open tickets";
            this.lblListHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lstCustomers
            //
            this.lstCustomers.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstCustomers.Font = new System.Drawing.Font("monospace", 9F);
            this.lstCustomers.Location = new System.Drawing.Point(20, 44);
            this.lstCustomers.Name = "lstCustomers";
            this.lstCustomers.Size = new System.Drawing.Size(1040, 372);
            //
            // btnAddCustomer  (Manager only — see PermissionService.CanEditCustomers)
            //
            this.btnAddCustomer.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.btnAddCustomer.Location = new System.Drawing.Point(20, 436);
            this.btnAddCustomer.Name = "btnAddCustomer";
            this.btnAddCustomer.Size = new System.Drawing.Size(150, 36);
            this.btnAddCustomer.Text = "Add Customer";
            this.btnAddCustomer.ToolTipText = "Disabled for a Support Agent; the handler checks the role again on the server.";
            this.btnAddCustomer.Click += new System.EventHandler(this.btnAddCustomer_Click);
            //
            // btnViewSelected
            //
            this.btnViewSelected.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.btnViewSelected.Location = new System.Drawing.Point(180, 436);
            this.btnViewSelected.Name = "btnViewSelected";
            this.btnViewSelected.Size = new System.Drawing.Size(150, 36);
            this.btnViewSelected.Text = "View Selected";
            this.btnViewSelected.ToolTipText = "Shows the selected customer in a toast; with no selection the shell logs a warning.";
            this.btnViewSelected.Click += new System.EventHandler(this.btnViewSelected_Click);
            //
            // lblPermissionNote
            //
            this.lblPermissionNote.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.lblPermissionNote.AutoSize = false;
            this.lblPermissionNote.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblPermissionNote.ForeColor = System.Drawing.Color.FromArgb(232, 161, 60);
            this.lblPermissionNote.Location = new System.Drawing.Point(500, 436);
            this.lblPermissionNote.Name = "lblPermissionNote";
            this.lblPermissionNote.Size = new System.Drawing.Size(560, 36);
            this.lblPermissionNote.Text = "";
            this.lblPermissionNote.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // CustomersView
            //
            this.Controls.Add(this.lblViewTitle);
            this.Controls.Add(this.lblCustomersSummary);
            this.Controls.Add(this.pnlCustomers);
            this.Name = "CustomersView";
            this.Size = new System.Drawing.Size(1080, 536);
            this.Load += new System.EventHandler(this.CustomersView_Load);
            this.pnlCustomers.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblViewTitle;
        private Wisej.Web.Label lblCustomersSummary;
        private Wisej.Web.Panel pnlCustomers;
        private Wisej.Web.Label lblListHeader;
        private Wisej.Web.ListBox lstCustomers;
        private Wisej.Web.Button btnAddCustomer;
        private Wisej.Web.Button btnViewSelected;
        private Wisej.Web.Label lblPermissionNote;
    }
}
