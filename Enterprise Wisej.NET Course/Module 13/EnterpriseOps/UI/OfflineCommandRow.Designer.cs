namespace EnterpriseOps.UI
{
    partial class OfflineCommandRow
    {
        /// <summary>Required designer variable.</summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>Clean up any resources being used.</summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.lblRowCode = new Wisej.Web.Label();
            this.lblRowSummary = new Wisej.Web.Label();
            this.lblRowState = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // lblRowCode  (WO-1041)
            //
            this.lblRowCode.AutoSize = false;
            this.lblRowCode.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.lblRowCode.ForeColor = System.Drawing.Color.FromArgb(11, 92, 196);
            this.lblRowCode.Location = new System.Drawing.Point(10, 7);
            this.lblRowCode.Name = "lblRowCode";
            this.lblRowCode.Size = new System.Drawing.Size(78, 18);
            this.lblRowCode.Text = "WO-0000";
            //
            // lblRowSummary  (what the technician did, plus the server's answer once it synced)
            //
            this.lblRowSummary.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblRowSummary.AutoSize = false;
            this.lblRowSummary.Font = new System.Drawing.Font("default", 9F);
            this.lblRowSummary.ForeColor = System.Drawing.Color.FromArgb(70, 88, 106);
            this.lblRowSummary.Location = new System.Drawing.Point(94, 7);
            this.lblRowSummary.Name = "lblRowSummary";
            this.lblRowSummary.Size = new System.Drawing.Size(184, 18);
            this.lblRowSummary.Text = "";
            //
            // lblRowState  (the SyncState pill)
            //
            this.lblRowState.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblRowState.AutoSize = false;
            this.lblRowState.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.lblRowState.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.lblRowState.Location = new System.Drawing.Point(284, 5);
            this.lblRowState.Name = "lblRowState";
            this.lblRowState.Size = new System.Drawing.Size(96, 22);
            this.lblRowState.Text = "PendingSync";
            this.lblRowState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // OfflineCommandRow
            //
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.Controls.Add(this.lblRowCode);
            this.Controls.Add(this.lblRowSummary);
            this.Controls.Add(this.lblRowState);
            this.Name = "OfflineCommandRow";
            this.Size = new System.Drawing.Size(390, 32);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblRowCode;
        private Wisej.Web.Label lblRowSummary;
        private Wisej.Web.Label lblRowState;
    }
}
