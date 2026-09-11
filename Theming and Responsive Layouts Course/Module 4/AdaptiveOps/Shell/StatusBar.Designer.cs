namespace AdaptiveOps.Shell
{
    partial class StatusBar
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
            this.lblStatus = new Wisej.Web.Label();
            this.widthLabel = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // lblStatus
            //
            this.lblStatus.AutoEllipsis = true;
            this.lblStatus.AutoSize = false;
            this.lblStatus.Dock = Wisej.Web.DockStyle.Left;
            this.lblStatus.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(360, 22);
            this.lblStatus.Text = "Ready";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // widthLabel
            //
            this.widthLabel.AutoEllipsis = true;
            this.widthLabel.AutoSize = false;
            this.widthLabel.Dock = Wisej.Web.DockStyle.Fill;
            this.widthLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.widthLabel.Name = "widthLabel";
            this.widthLabel.Text = "Width: –";
            this.widthLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // StatusBar
            //
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.Controls.Add(this.widthLabel);
            this.Controls.Add(this.lblStatus);
            this.Name = "StatusBar";
            this.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.Size = new System.Drawing.Size(1332, 24);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Label widthLabel;
    }
}
