namespace OperationsConsole.Shell
{
    partial class StatusStrip
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
            this.lblStrip = new Wisej.Web.Label();
            this.btnStripRefresh = new Wisej.Web.Button();
            this.SuspendLayout();
            //
            // lblStrip
            //
            this.lblStrip.AutoSize = false;
            this.lblStrip.Dock = Wisej.Web.DockStyle.Fill;
            this.lblStrip.Font = new System.Drawing.Font("monospace", 9F);
            this.lblStrip.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.lblStrip.Name = "lblStrip";
            this.lblStrip.Padding = new Wisej.Web.Padding(14, 0, 8, 0);
            this.lblStrip.Text = "Section · 0 records · never refreshed";
            this.lblStrip.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnStripRefresh
            //
            this.btnStripRefresh.AccessibleName = "Refresh this section";
            this.btnStripRefresh.Dock = Wisej.Web.DockStyle.Right;
            this.btnStripRefresh.Name = "btnStripRefresh";
            this.btnStripRefresh.Size = new System.Drawing.Size(44, 30);
            this.btnStripRefresh.TabIndex = 1;
            this.btnStripRefresh.Text = "↻";
            this.btnStripRefresh.Click += new System.EventHandler(this.btnStripRefresh_Click);
            //
            // StatusStrip
            //
            this.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.Controls.Add(this.lblStrip);
            this.Controls.Add(this.btnStripRefresh);
            this.Name = "StatusStrip";
            this.Size = new System.Drawing.Size(720, 30);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblStrip;
        private Wisej.Web.Button btnStripRefresh;
    }
}
