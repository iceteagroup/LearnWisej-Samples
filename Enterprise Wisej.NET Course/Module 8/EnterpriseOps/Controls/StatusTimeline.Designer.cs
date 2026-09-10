namespace EnterpriseOps.Controls
{
    partial class StatusTimeline
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
            this.pnlFrame = new Wisej.Web.Panel();
            this.lblCaption = new Wisej.Web.Label();
            this.lblSampleBadge = new Wisej.Web.Label();
            this.lblTypeName = new Wisej.Web.Label();
            this.pnlItems = new Wisej.Web.FlowLayoutPanel();
            this.lblEmpty = new Wisej.Web.Label();
            this.pnlFrame.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlFrame  (the white card; the UserControl itself has no border so it can sit on any background)
            //
            this.pnlFrame.BackColor = System.Drawing.Color.White;
            this.pnlFrame.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlFrame.Controls.Add(this.lblCaption);
            this.pnlFrame.Controls.Add(this.lblSampleBadge);
            this.pnlFrame.Controls.Add(this.lblTypeName);
            this.pnlFrame.Controls.Add(this.pnlItems);
            this.pnlFrame.Controls.Add(this.lblEmpty);
            this.pnlFrame.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlFrame.Location = new System.Drawing.Point(0, 0);
            this.pnlFrame.Name = "pnlFrame";
            this.pnlFrame.Size = new System.Drawing.Size(420, 210);
            //
            // lblCaption  (the Caption property writes here)
            //
            this.lblCaption.AutoSize = false;
            this.lblCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblCaption.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.lblCaption.Location = new System.Drawing.Point(12, 8);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(150, 18);
            this.lblCaption.Text = "STATUS TIMELINE";
            this.lblCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblSampleBadge  (only visible in sample mode — the amber pill the walkthrough shows in the Designer)
            //
            this.lblSampleBadge.AutoSize = false;
            this.lblSampleBadge.BackColor = System.Drawing.Color.FromArgb(255, 248, 236);
            this.lblSampleBadge.Font = new System.Drawing.Font("default", 7.5F, System.Drawing.FontStyle.Bold);
            this.lblSampleBadge.ForeColor = System.Drawing.Color.FromArgb(185, 119, 14);
            this.lblSampleBadge.Location = new System.Drawing.Point(166, 8);
            this.lblSampleBadge.Name = "lblSampleBadge";
            this.lblSampleBadge.Size = new System.Drawing.Size(132, 18);
            this.lblSampleBadge.Text = "DESIGN-TIME SAMPLE";
            this.lblSampleBadge.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSampleBadge.Visible = false;
            //
            // lblTypeName  (so a screenshot of the screen says which component drew this card)
            //
            this.lblTypeName.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblTypeName.AutoSize = false;
            this.lblTypeName.Font = new System.Drawing.Font("monospace", 7.5F);
            this.lblTypeName.ForeColor = System.Drawing.Color.FromArgb(154, 167, 180);
            this.lblTypeName.Location = new System.Drawing.Point(302, 8);
            this.lblTypeName.Name = "lblTypeName";
            this.lblTypeName.Size = new System.Drawing.Size(106, 18);
            this.lblTypeName.Text = "StatusTimeline";
            this.lblTypeName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // pnlItems  (one row control per TimelineItem, oldest first, built by RenderItems())
            //
            this.pnlItems.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlItems.AutoScroll = true;
            this.pnlItems.FlowDirection = Wisej.Web.FlowDirection.TopDown;
            this.pnlItems.Location = new System.Drawing.Point(12, 32);
            this.pnlItems.Name = "pnlItems";
            this.pnlItems.Size = new System.Drawing.Size(396, 166);
            this.pnlItems.WrapContents = false;
            //
            // lblEmpty  (what an empty component says instead of nothing at all)
            //
            this.lblEmpty.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblEmpty.AutoSize = false;
            this.lblEmpty.Font = new System.Drawing.Font("default", 9F);
            this.lblEmpty.ForeColor = System.Drawing.Color.FromArgb(138, 151, 164);
            this.lblEmpty.Location = new System.Drawing.Point(16, 40);
            this.lblEmpty.Name = "lblEmpty";
            this.lblEmpty.Size = new System.Drawing.Size(388, 24);
            this.lblEmpty.Text = "No history to show.";
            this.lblEmpty.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // StatusTimeline
            //
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.pnlFrame);
            this.Name = "StatusTimeline";
            this.Size = new System.Drawing.Size(420, 210);
            this.pnlFrame.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlFrame;
        private Wisej.Web.Label lblCaption;
        private Wisej.Web.Label lblSampleBadge;
        private Wisej.Web.Label lblTypeName;
        private Wisej.Web.FlowLayoutPanel pnlItems;
        private Wisej.Web.Label lblEmpty;
    }
}
