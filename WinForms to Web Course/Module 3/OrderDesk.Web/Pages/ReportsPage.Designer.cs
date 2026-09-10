namespace OrderDesk.Pages
{
    partial class ReportsPage
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
            this.reportsList = new Wisej.Web.ListBox();
            this.noteLabel = new Wisej.Web.Label();
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
            this.headerLabel.Text = "Reports";
            this.headerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // reportsList
            //
            this.reportsList.BorderStyle = Wisej.Web.BorderStyle.None;
            this.reportsList.Dock = Wisej.Web.DockStyle.Fill;
            this.reportsList.Font = new System.Drawing.Font("default", 11F);
            this.reportsList.Items.AddRange(new object[] {
                "Open orders by customer",
                "Invoices this month",
                "Owner workload",
                "Orders on hold"});
            this.reportsList.Name = "reportsList";
            this.reportsList.TabIndex = 0;
            this.reportsList.SelectedIndexChanged += new System.EventHandler(this.reportsList_SelectedIndexChanged);
            //
            // noteLabel
            //
            this.noteLabel.AutoSize = false;
            this.noteLabel.Dock = Wisej.Web.DockStyle.Bottom;
            this.noteLabel.Font = new System.Drawing.Font("default", 9F);
            this.noteLabel.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.noteLabel.Name = "noteLabel";
            this.noteLabel.Padding = new Wisej.Web.Padding(16, 0, 16, 0);
            this.noteLabel.Size = new System.Drawing.Size(680, 48);
            this.noteLabel.TabStop = false;
            this.noteLabel.Text = "Placeholder list. Print Invoice (PrintDocument) and Export to Excel (Interop) are desktop boundaries; the server-side PDF, the managed .xlsx and the report queue arrive in Module 6.";
            this.noteLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // ReportsPage
            //
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.Controls.Add(this.reportsList);
            this.Controls.Add(this.noteLabel);
            this.Controls.Add(this.headerLabel);
            this.Name = "ReportsPage";
            this.Size = new System.Drawing.Size(680, 436);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label headerLabel;
        private Wisej.Web.ListBox reportsList;
        private Wisej.Web.Label noteLabel;
    }
}
