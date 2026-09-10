namespace OperationsConsole.Shell
{
    partial class RecordHeader
    {
        private System.ComponentModel.IContainer components = null;

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
            this.lblTitle = new Wisej.Web.Label();
            this.lblCount = new Wisej.Web.Label();
            this.lblRefreshed = new Wisej.Web.Label();
            this.btnRefresh = new Wisej.Web.Button();
            this.SuspendLayout();
            //
            // lblTitle  (private — a consumer sets RecordHeader.Title, never this label)
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 13F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(16, 14);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(230, 30);
            this.lblTitle.Text = "Section";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblCount  (private — a consumer sets RecordHeader.RecordCount)
            //
            this.lblCount.AutoSize = false;
            this.lblCount.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblCount.ForeColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.lblCount.Location = new System.Drawing.Point(254, 18);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(130, 22);
            this.lblCount.Text = "0 records";
            this.lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblRefreshed  (private — a consumer sets RecordHeader.LastRefresh)
            //
            this.lblRefreshed.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblRefreshed.AutoSize = false;
            this.lblRefreshed.Font = new System.Drawing.Font("monospace", 9F);
            this.lblRefreshed.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblRefreshed.Location = new System.Drawing.Point(394, 18);
            this.lblRefreshed.Name = "lblRefreshed";
            this.lblRefreshed.Size = new System.Drawing.Size(190, 22);
            this.lblRefreshed.Text = "never refreshed";
            this.lblRefreshed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // btnRefresh  (private — a consumer handles RecordHeader.RefreshRequested)
            //
            this.btnRefresh.AccessibleName = "Refresh this section";
            this.btnRefresh.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnRefresh.Location = new System.Drawing.Point(596, 13);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(108, 32);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.ToolTipText = "Raises RecordHeader.RefreshRequested — the consuming page decides what that means.";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // RecordHeader
            //
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.lblRefreshed);
            this.Controls.Add(this.btnRefresh);
            this.Name = "RecordHeader";
            this.Size = new System.Drawing.Size(720, 58);
            this.ResumeLayout(false);
        }

        #endregion

        // every child control is private: the public surface of this UserControl is
        // Title, RecordCount, LastRefresh and RefreshRequested — nothing else.
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblCount;
        private Wisej.Web.Label lblRefreshed;
        private Wisej.Web.Button btnRefresh;
    }
}
