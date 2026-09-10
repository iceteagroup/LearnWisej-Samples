namespace TicketOps.Diagnostics
{
    partial class ActivityTracePanel
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
            this.labelTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.labelFooter = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // labelTitle
            //
            this.labelTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTitle.AutoSize = false;
            this.labelTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(20, 16);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(460, 26);
            this.labelTitle.Text = "Activity trace · UI → Service → Data";
            //
            // listTrace
            //
            this.listTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.listTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.listTrace.Location = new System.Drawing.Point(20, 50);
            this.listTrace.Name = "listTrace";
            this.listTrace.Size = new System.Drawing.Size(460, 456);
            //
            // labelFooter
            //
            this.labelFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelFooter.AutoSize = false;
            this.labelFooter.Font = new System.Drawing.Font("default", 8.5F);
            this.labelFooter.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelFooter.Location = new System.Drawing.Point(20, 512);
            this.labelFooter.Name = "labelFooter";
            this.labelFooter.Size = new System.Drawing.Size(460, 34);
            this.labelFooter.Text = "UI collects input · SVC decides · DATA persists · ⚠ handled · ✖ failure (details stay in the log, the user sees a safe message)";
            //
            // ActivityTracePanel
            //
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.listTrace);
            this.Controls.Add(this.labelFooter);
            this.Name = "ActivityTracePanel";
            this.Size = new System.Drawing.Size(500, 560);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label labelTitle;
        private Wisej.Web.ListBox listTrace;
        private Wisej.Web.Label labelFooter;
    }
}
