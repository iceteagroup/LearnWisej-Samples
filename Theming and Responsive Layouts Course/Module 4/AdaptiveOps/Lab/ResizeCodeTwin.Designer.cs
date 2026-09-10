namespace AdaptiveOps.Lab
{
    partial class ResizeCodeTwin
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
            this.twinToolbar = new Wisej.Web.Panel();
            this.lblTwinCounter = new Wisej.Web.Label();
            this.twinRail = new Wisej.Web.Panel();
            this.lblTwinRail = new Wisej.Web.Label();
            this.twinWorkspace = new Wisej.Web.Panel();
            this.lblTwinWorkspace = new Wisej.Web.Label();
            this.twinDetails = new Wisej.Web.Panel();
            this.lblTwinDetails = new Wisej.Web.Label();
            this.btnTwinSave = new Wisej.Web.Button();
            this.twinStatus = new Wisej.Web.Panel();
            this.lblTwinStatus = new Wisej.Web.Label();
            this.twinToolbar.SuspendLayout();
            this.twinRail.SuspendLayout();
            this.twinWorkspace.SuspendLayout();
            this.twinDetails.SuspendLayout();
            this.twinStatus.SuspendLayout();
            this.SuspendLayout();
            //
            // NOTE: none of the five panels has a Dock or an Anchor. Their Bounds are assigned in
            // ResizeCodeTwin_Resize (ResizeCodeTwin.cs) - the anti-pattern this module removes.
            // The Location/Size values below are only the designer's initial placement.
            //
            // twinToolbar
            //
            this.twinToolbar.BackColor = System.Drawing.Color.White;
            this.twinToolbar.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.twinToolbar.Controls.Add(this.lblTwinCounter);
            this.twinToolbar.Location = new System.Drawing.Point(0, 0);
            this.twinToolbar.Name = "twinToolbar";
            this.twinToolbar.Size = new System.Drawing.Size(740, 36);
            this.lblTwinCounter.AutoEllipsis = true;
            this.lblTwinCounter.AutoSize = false;
            this.lblTwinCounter.Dock = Wisej.Web.DockStyle.Fill;
            this.lblTwinCounter.Font = new System.Drawing.Font("monospace", 9F);
            this.lblTwinCounter.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.lblTwinCounter.Name = "lblTwinCounter";
            this.lblTwinCounter.Padding = new Wisej.Web.Padding(10, 0, 10, 0);
            this.lblTwinCounter.Text = "toolbar · Bounds set in Resize";
            this.lblTwinCounter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // twinRail
            //
            this.twinRail.BackColor = System.Drawing.Color.White;
            this.twinRail.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.twinRail.Controls.Add(this.lblTwinRail);
            this.twinRail.Location = new System.Drawing.Point(0, 40);
            this.twinRail.Name = "twinRail";
            this.twinRail.Size = new System.Drawing.Size(120, 220);
            this.lblTwinRail.AutoSize = false;
            this.lblTwinRail.Dock = Wisej.Web.DockStyle.Fill;
            this.lblTwinRail.ForeColor = System.Drawing.Color.FromArgb(103, 112, 133);
            this.lblTwinRail.Name = "lblTwinRail";
            this.lblTwinRail.Padding = new Wisej.Web.Padding(8);
            this.lblTwinRail.Text = "rail\nBounds = (0, 40, 120, h)";
            this.lblTwinRail.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // twinWorkspace  (fixed width: correct at the width the developer tested, wrong elsewhere)
            //
            this.twinWorkspace.BackColor = System.Drawing.Color.White;
            this.twinWorkspace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.twinWorkspace.Controls.Add(this.lblTwinWorkspace);
            this.twinWorkspace.Location = new System.Drawing.Point(124, 40);
            this.twinWorkspace.Name = "twinWorkspace";
            this.twinWorkspace.Size = new System.Drawing.Size(380, 220);
            this.lblTwinWorkspace.AutoSize = false;
            this.lblTwinWorkspace.Dock = Wisej.Web.DockStyle.Fill;
            this.lblTwinWorkspace.Name = "lblTwinWorkspace";
            this.lblTwinWorkspace.Padding = new Wisej.Web.Padding(8);
            this.lblTwinWorkspace.Text = "ticket grid\nBounds = (124, 40, 380, h) — width fixed at the tested size";
            this.lblTwinWorkspace.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // twinDetails  ("anchored right" by hand: x = w - 220)
            //
            this.twinDetails.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.twinDetails.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.twinDetails.Controls.Add(this.lblTwinDetails);
            this.twinDetails.Controls.Add(this.btnTwinSave);
            this.twinDetails.Location = new System.Drawing.Point(520, 40);
            this.twinDetails.Name = "twinDetails";
            this.twinDetails.Size = new System.Drawing.Size(220, 220);
            this.lblTwinDetails.AutoSize = false;
            this.lblTwinDetails.Location = new System.Drawing.Point(8, 8);
            this.lblTwinDetails.Name = "lblTwinDetails";
            this.lblTwinDetails.Size = new System.Drawing.Size(204, 160);
            this.lblTwinDetails.Text = "details\nBounds = (w − 220, 40, 220, h)\n\nSave sits at a fixed y = 180:\nit drops below the panel when the browser is short.";
            this.lblTwinDetails.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnTwinSave.Location = new System.Drawing.Point(12, 180);
            this.btnTwinSave.Name = "btnTwinSave";
            this.btnTwinSave.Size = new System.Drawing.Size(90, 32);
            this.btnTwinSave.Text = "Save";
            //
            // twinStatus
            //
            this.twinStatus.BackColor = System.Drawing.Color.White;
            this.twinStatus.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.twinStatus.Controls.Add(this.lblTwinStatus);
            this.twinStatus.Location = new System.Drawing.Point(0, 264);
            this.twinStatus.Name = "twinStatus";
            this.twinStatus.Size = new System.Drawing.Size(740, 24);
            this.lblTwinStatus.AutoEllipsis = true;
            this.lblTwinStatus.AutoSize = false;
            this.lblTwinStatus.Dock = Wisej.Web.DockStyle.Fill;
            this.lblTwinStatus.ForeColor = System.Drawing.Color.FromArgb(103, 112, 133);
            this.lblTwinStatus.Name = "lblTwinStatus";
            this.lblTwinStatus.Padding = new Wisej.Web.Padding(10, 0, 10, 0);
            this.lblTwinStatus.Text = "status · Bounds = (0, h − 24, w, 24)";
            this.lblTwinStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // ResizeCodeTwin
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.BorderStyle = Wisej.Web.BorderStyle.None;
            this.Controls.Add(this.twinToolbar);
            this.Controls.Add(this.twinRail);
            this.Controls.Add(this.twinWorkspace);
            this.Controls.Add(this.twinDetails);
            this.Controls.Add(this.twinStatus);
            this.Name = "ResizeCodeTwin";
            this.Size = new System.Drawing.Size(740, 288);
            this.Resize += new System.EventHandler(this.ResizeCodeTwin_Resize);
            this.twinToolbar.ResumeLayout(false);
            this.twinRail.ResumeLayout(false);
            this.twinWorkspace.ResumeLayout(false);
            this.twinDetails.ResumeLayout(false);
            this.twinStatus.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel twinToolbar;
        private Wisej.Web.Label lblTwinCounter;
        private Wisej.Web.Panel twinRail;
        private Wisej.Web.Label lblTwinRail;
        private Wisej.Web.Panel twinWorkspace;
        private Wisej.Web.Label lblTwinWorkspace;
        private Wisej.Web.Panel twinDetails;
        private Wisej.Web.Label lblTwinDetails;
        private Wisej.Web.Button btnTwinSave;
        private Wisej.Web.Panel twinStatus;
        private Wisej.Web.Label lblTwinStatus;
    }
}
