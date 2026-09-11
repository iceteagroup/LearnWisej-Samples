namespace OrderDesk
{
    partial class MainPage
    {
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
            this.components = new System.ComponentModel.Container();
            this.panelOrders = new Wisej.Web.Panel();
            this.labelOrdersTitle = new Wisej.Web.Label();
            this.labelOrdersCount = new Wisej.Web.Label();
            this.comboFilter = new Wisej.Web.ComboBox();
            this.comboCustomer = new Wisej.Web.ComboBox();
            this.gridOrders = new Wisej.Web.DataGridView();
            this.colOrder = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTotal = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.panelSession = new Wisej.Web.Panel();
            this.labelSessionTitle = new Wisej.Web.Label();
            this.labelSession = new Wisej.Web.Label();
            this.buttonSignInKelly = new Wisej.Web.Button();
            this.buttonSignInSam = new Wisej.Web.Button();
            this.buttonSignOut = new Wisej.Web.Button();
            this.buttonReread = new Wisej.Web.Button();
            this.buttonSecondSession = new Wisej.Web.Button();
            this.panelOrders.SuspendLayout();
            this.panelSession.SuspendLayout();
            this.SuspendLayout();
            //
            // panelOrders
            //
            this.panelOrders.BackColor = System.Drawing.Color.White;
            this.panelOrders.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelOrders.Controls.Add(this.labelOrdersTitle);
            this.panelOrders.Controls.Add(this.labelOrdersCount);
            this.panelOrders.Controls.Add(this.comboFilter);
            this.panelOrders.Controls.Add(this.comboCustomer);
            this.panelOrders.Controls.Add(this.gridOrders);
            this.panelOrders.Location = new System.Drawing.Point(20, 20);
            this.panelOrders.Name = "panelOrders";
            this.panelOrders.Size = new System.Drawing.Size(660, 330);
            //
            // labelOrdersTitle
            //
            this.labelOrdersTitle.AutoSize = false;
            this.labelOrdersTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelOrdersTitle.Location = new System.Drawing.Point(20, 12);
            this.labelOrdersTitle.Name = "labelOrdersTitle";
            this.labelOrdersTitle.Size = new System.Drawing.Size(200, 28);
            this.labelOrdersTitle.Text = "Orders";
            //
            // labelOrdersCount
            //
            this.labelOrdersCount.AutoSize = false;
            this.labelOrdersCount.Font = new System.Drawing.Font("default", 9F);
            this.labelOrdersCount.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelOrdersCount.Location = new System.Drawing.Point(340, 52);
            this.labelOrdersCount.Name = "labelOrdersCount";
            this.labelOrdersCount.Size = new System.Drawing.Size(300, 24);
            this.labelOrdersCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // comboFilter
            //
            this.comboFilter.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboFilter.Location = new System.Drawing.Point(20, 50);
            this.comboFilter.Name = "comboFilter";
            this.comboFilter.Size = new System.Drawing.Size(130, 28);
            this.comboFilter.SelectedIndexChanged += new System.EventHandler(this.comboFilter_SelectedIndexChanged);
            //
            // comboCustomer
            //
            this.comboCustomer.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboCustomer.Location = new System.Drawing.Point(160, 50);
            this.comboCustomer.Name = "comboCustomer";
            this.comboCustomer.Size = new System.Drawing.Size(180, 28);
            this.comboCustomer.SelectedIndexChanged += new System.EventHandler(this.comboCustomer_SelectedIndexChanged);
            //
            // gridOrders
            //
            this.gridOrders.AllowUserToAddRows = false;
            this.gridOrders.AllowUserToDeleteRows = false;
            this.gridOrders.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colOrder, this.colCustomer, this.colTotal, this.colStatus });
            this.gridOrders.Location = new System.Drawing.Point(20, 88);
            this.gridOrders.MultiSelect = false;
            this.gridOrders.Name = "gridOrders";
            this.gridOrders.ReadOnly = true;
            this.gridOrders.RowHeadersVisible = false;
            this.gridOrders.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridOrders.Size = new System.Drawing.Size(620, 222);
            this.colOrder.HeaderText = "Order"; this.colOrder.Name = "colOrder"; this.colOrder.Width = 70; this.colOrder.ReadOnly = true;
            this.colCustomer.HeaderText = "Customer"; this.colCustomer.Name = "colCustomer"; this.colCustomer.Width = 220; this.colCustomer.ReadOnly = true;
            this.colTotal.HeaderText = "Total"; this.colTotal.Name = "colTotal"; this.colTotal.Width = 110; this.colTotal.ReadOnly = true;
            this.colTotal.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            this.colTotal.DefaultCellStyle.Format = "N2";
            this.colStatus.HeaderText = "Status"; this.colStatus.Name = "colStatus"; this.colStatus.Width = 110; this.colStatus.ReadOnly = true;
            //
            // panelSession
            //
            this.panelSession.BackColor = System.Drawing.Color.White;
            this.panelSession.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelSession.Controls.Add(this.labelSessionTitle);
            this.panelSession.Controls.Add(this.labelSession);
            this.panelSession.Controls.Add(this.buttonSignInKelly);
            this.panelSession.Controls.Add(this.buttonSignInSam);
            this.panelSession.Controls.Add(this.buttonSignOut);
            this.panelSession.Controls.Add(this.buttonReread);
            this.panelSession.Controls.Add(this.buttonSecondSession);
            this.panelSession.Location = new System.Drawing.Point(20, 364);
            this.panelSession.Name = "panelSession";
            this.panelSession.Size = new System.Drawing.Size(660, 196);
            //
            // labelSessionTitle
            //
            this.labelSessionTitle.AutoSize = false;
            this.labelSessionTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelSessionTitle.Location = new System.Drawing.Point(20, 12);
            this.labelSessionTitle.Name = "labelSessionTitle";
            this.labelSessionTitle.Size = new System.Drawing.Size(300, 28);
            this.labelSessionTitle.Text = "Session";
            //
            // labelSession
            //
            this.labelSession.AutoSize = false;
            this.labelSession.Font = new System.Drawing.Font("monospace", 9F);
            this.labelSession.Location = new System.Drawing.Point(20, 44);
            this.labelSession.Name = "labelSession";
            this.labelSession.Size = new System.Drawing.Size(620, 90);
            this.labelSession.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // buttonSignInKelly
            //
            this.buttonSignInKelly.Location = new System.Drawing.Point(20, 144);
            this.buttonSignInKelly.Name = "buttonSignInKelly";
            this.buttonSignInKelly.Size = new System.Drawing.Size(116, 32);
            this.buttonSignInKelly.Text = "Sign in as kelly";
            this.buttonSignInKelly.Click += new System.EventHandler(this.buttonSignInKelly_Click);
            //
            // buttonSignInSam
            //
            this.buttonSignInSam.Location = new System.Drawing.Point(142, 144);
            this.buttonSignInSam.Name = "buttonSignInSam";
            this.buttonSignInSam.Size = new System.Drawing.Size(116, 32);
            this.buttonSignInSam.Text = "Sign in as sam";
            this.buttonSignInSam.Click += new System.EventHandler(this.buttonSignInSam_Click);
            //
            // buttonSignOut
            //
            this.buttonSignOut.Location = new System.Drawing.Point(264, 144);
            this.buttonSignOut.Name = "buttonSignOut";
            this.buttonSignOut.Size = new System.Drawing.Size(90, 32);
            this.buttonSignOut.Text = "Sign out";
            this.buttonSignOut.Click += new System.EventHandler(this.buttonSignOut_Click);
            //
            // buttonReread
            //
            this.buttonReread.Location = new System.Drawing.Point(360, 144);
            this.buttonReread.Name = "buttonReread";
            this.buttonReread.Size = new System.Drawing.Size(110, 32);
            this.buttonReread.Text = "Re-read state";
            this.buttonReread.Click += new System.EventHandler(this.buttonReread_Click);
            //
            // buttonSecondSession
            //
            this.buttonSecondSession.Location = new System.Drawing.Point(476, 144);
            this.buttonSecondSession.Name = "buttonSecondSession";
            this.buttonSecondSession.Size = new System.Drawing.Size(164, 32);
            this.buttonSecondSession.Text = "Open second session ↗";
            this.buttonSecondSession.Click += new System.EventHandler(this.buttonSecondSession_Click);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelOrders);
            this.Controls.Add(this.panelSession);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(700, 580);
            this.Text = "OrderDesk — Orders";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.panelOrders.ResumeLayout(false);
            this.panelSession.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelOrders;
        private Wisej.Web.Label labelOrdersTitle;
        private Wisej.Web.Label labelOrdersCount;
        private Wisej.Web.ComboBox comboFilter;
        private Wisej.Web.ComboBox comboCustomer;
        private Wisej.Web.DataGridView gridOrders;
        private Wisej.Web.DataGridViewTextBoxColumn colOrder;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomer;
        private Wisej.Web.DataGridViewTextBoxColumn colTotal;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.Panel panelSession;
        private Wisej.Web.Label labelSessionTitle;
        private Wisej.Web.Label labelSession;
        private Wisej.Web.Button buttonSignInKelly;
        private Wisej.Web.Button buttonSignInSam;
        private Wisej.Web.Button buttonSignOut;
        private Wisej.Web.Button buttonReread;
        private Wisej.Web.Button buttonSecondSession;
    }
}
