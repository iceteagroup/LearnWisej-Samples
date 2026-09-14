namespace WisejPerfLab.Forms
{
    partial class TicketDetailForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblNumber = new Wisej.Web.Label();
            this.lblCustomer = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.lblAge = new Wisej.Web.Label();
            this.gridComments = new Wisej.Web.DataGridView();
            this.bindingSource1 = new Wisej.Web.BindingSource(this.components);
            this.btnClose = new Wisej.Web.Button();
            this.lblRoots = new Wisej.Web.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridComments)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            //
            // lblNumber
            //
            this.lblNumber.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.lblNumber.Location = new System.Drawing.Point(18, 14);
            this.lblNumber.Name = "lblNumber";
            this.lblNumber.Size = new System.Drawing.Size(300, 26);
            this.lblNumber.TabIndex = 0;
            //
            // lblCustomer
            //
            this.lblCustomer.Location = new System.Drawing.Point(20, 46);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(460, 18);
            this.lblCustomer.TabIndex = 1;
            //
            // lblStatus
            //
            this.lblStatus.Location = new System.Drawing.Point(20, 68);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(460, 18);
            this.lblStatus.TabIndex = 2;
            //
            // lblAge
            //
            this.lblAge.Font = new System.Drawing.Font("monospace", 9F);
            this.lblAge.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblAge.Location = new System.Drawing.Point(20, 90);
            this.lblAge.Name = "lblAge";
            this.lblAge.Size = new System.Drawing.Size(460, 18);
            this.lblAge.TabIndex = 3;
            //
            // gridComments
            //
            this.gridComments.AllowUserToAddRows = false;
            this.gridComments.AllowUserToDeleteRows = false;
            this.gridComments.AutoGenerateColumns = false;
            this.gridComments.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridComments.Location = new System.Drawing.Point(20, 118);
            this.gridComments.Name = "gridComments";
            this.gridComments.ReadOnly = true;
            this.gridComments.Size = new System.Drawing.Size(460, 180);
            this.gridComments.TabIndex = 4;
            //
            // btnClose
            //
            this.btnClose.Location = new System.Drawing.Point(380, 316);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 34);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close";
            this.btnClose.Click += this.btnClose_Click;
            //
            // lblRoots
            //
            this.lblRoots.Font = new System.Drawing.Font("monospace", 8F);
            this.lblRoots.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblRoots.Location = new System.Drawing.Point(20, 320);
            this.lblRoots.Name = "lblRoots";
            this.lblRoots.Size = new System.Drawing.Size(350, 28);
            this.lblRoots.TabIndex = 6;
            //
            // TicketDetailForm
            //
            this.AcceptButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(500, 364);
            this.Controls.Add(this.lblRoots);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.gridComments);
            this.Controls.Add(this.lblAge);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblCustomer);
            this.Controls.Add(this.lblNumber);
            this.Name = "TicketDetailForm";
            this.ShowInTaskbar = false;
            this.StartPosition = Wisej.Web.FormStartPosition.CenterParent;
            this.Text = "Ticket";
            ((System.ComponentModel.ISupportInitialize)(this.gridComments)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblNumber;
        private Wisej.Web.Label lblCustomer;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Label lblAge;
        private Wisej.Web.DataGridView gridComments;
        private Wisej.Web.BindingSource bindingSource1;
        private Wisej.Web.Button btnClose;
        private Wisej.Web.Label lblRoots;
    }
}
