namespace EnterpriseOps.UI
{
    partial class SyncConflictPanel
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
            this.lblConflictTitle = new Wisej.Web.Label();
            this.lblConflictSubtitle = new Wisej.Web.Label();
            this.pnlLocalVersion = new Wisej.Web.Panel();
            this.lblLocalHeader = new Wisej.Web.Label();
            this.lblLocalBody = new Wisej.Web.Label();
            this.pnlServerVersion = new Wisej.Web.Panel();
            this.lblServerHeader = new Wisej.Web.Label();
            this.lblServerBody = new Wisej.Web.Label();
            this.flpConflictActions = new Wisej.Web.FlowLayoutPanel();
            this.btnKeepServer = new Wisej.Web.Button();
            this.btnApplyMine = new Wisej.Web.Button();
            this.pnlLocalVersion.SuspendLayout();
            this.pnlServerVersion.SuspendLayout();
            this.flpConflictActions.SuspendLayout();
            this.SuspendLayout();
            //
            // lblConflictTitle
            //
            this.lblConflictTitle.AutoSize = false;
            this.lblConflictTitle.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblConflictTitle.ForeColor = System.Drawing.Color.FromArgb(156, 47, 47);
            this.lblConflictTitle.Location = new System.Drawing.Point(12, 10);
            this.lblConflictTitle.Name = "lblConflictTitle";
            this.lblConflictTitle.Size = new System.Drawing.Size(330, 22);
            this.lblConflictTitle.Text = "Sync conflict";
            //
            // lblConflictSubtitle
            //
            this.lblConflictSubtitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblConflictSubtitle.AutoSize = false;
            this.lblConflictSubtitle.Font = new System.Drawing.Font("default", 9F);
            this.lblConflictSubtitle.ForeColor = System.Drawing.Color.FromArgb(70, 88, 106);
            this.lblConflictSubtitle.Location = new System.Drawing.Point(12, 32);
            this.lblConflictSubtitle.Name = "lblConflictSubtitle";
            this.lblConflictSubtitle.Size = new System.Drawing.Size(352, 18);
            this.lblConflictSubtitle.Text = "";
            //
            // pnlLocalVersion  (what the technician did, on the device)
            //
            this.pnlLocalVersion.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlLocalVersion.BackColor = System.Drawing.Color.FromArgb(246, 249, 252);
            this.pnlLocalVersion.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlLocalVersion.Controls.Add(this.lblLocalHeader);
            this.pnlLocalVersion.Controls.Add(this.lblLocalBody);
            this.pnlLocalVersion.Location = new System.Drawing.Point(12, 54);
            this.pnlLocalVersion.Name = "pnlLocalVersion";
            this.pnlLocalVersion.Size = new System.Drawing.Size(352, 46);
            //
            // lblLocalHeader
            //
            this.lblLocalHeader.AutoSize = false;
            this.lblLocalHeader.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.lblLocalHeader.ForeColor = System.Drawing.Color.FromArgb(11, 92, 196);
            this.lblLocalHeader.Location = new System.Drawing.Point(10, 4);
            this.lblLocalHeader.Name = "lblLocalHeader";
            this.lblLocalHeader.Size = new System.Drawing.Size(330, 16);
            this.lblLocalHeader.Text = "YOUR LOCAL CHANGE";
            //
            // lblLocalBody
            //
            this.lblLocalBody.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblLocalBody.AutoSize = false;
            this.lblLocalBody.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblLocalBody.ForeColor = System.Drawing.Color.FromArgb(31, 45, 58);
            this.lblLocalBody.Location = new System.Drawing.Point(10, 22);
            this.lblLocalBody.Name = "lblLocalBody";
            this.lblLocalBody.Size = new System.Drawing.Size(330, 18);
            this.lblLocalBody.Text = "";
            //
            // pnlServerVersion  (what the dispatcher did, on the server)
            //
            this.pnlServerVersion.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlServerVersion.BackColor = System.Drawing.Color.FromArgb(253, 246, 246);
            this.pnlServerVersion.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlServerVersion.Controls.Add(this.lblServerHeader);
            this.pnlServerVersion.Controls.Add(this.lblServerBody);
            this.pnlServerVersion.Location = new System.Drawing.Point(12, 106);
            this.pnlServerVersion.Name = "pnlServerVersion";
            this.pnlServerVersion.Size = new System.Drawing.Size(352, 46);
            //
            // lblServerHeader
            //
            this.lblServerHeader.AutoSize = false;
            this.lblServerHeader.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.lblServerHeader.ForeColor = System.Drawing.Color.FromArgb(156, 47, 47);
            this.lblServerHeader.Location = new System.Drawing.Point(10, 4);
            this.lblServerHeader.Name = "lblServerHeader";
            this.lblServerHeader.Size = new System.Drawing.Size(330, 16);
            this.lblServerHeader.Text = "SERVER VERSION";
            //
            // lblServerBody
            //
            this.lblServerBody.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblServerBody.AutoSize = false;
            this.lblServerBody.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblServerBody.ForeColor = System.Drawing.Color.FromArgb(31, 45, 58);
            this.lblServerBody.Location = new System.Drawing.Point(10, 22);
            this.lblServerBody.Name = "lblServerBody";
            this.lblServerBody.Size = new System.Drawing.Size(330, 18);
            this.lblServerBody.Text = "";
            //
            // flpConflictActions  (wraps to two rows on a phone-width layout)
            //
            this.flpConflictActions.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.flpConflictActions.Controls.Add(this.btnKeepServer);
            this.flpConflictActions.Controls.Add(this.btnApplyMine);
            this.flpConflictActions.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.flpConflictActions.Location = new System.Drawing.Point(12, 158);
            this.flpConflictActions.Name = "flpConflictActions";
            this.flpConflictActions.Size = new System.Drawing.Size(352, 82);
            this.flpConflictActions.WrapContents = true;
            //
            // btnKeepServer
            //
            this.btnKeepServer.Location = new System.Drawing.Point(3, 3);
            this.btnKeepServer.Name = "btnKeepServer";
            this.btnKeepServer.Size = new System.Drawing.Size(214, 36);
            this.btnKeepServer.Text = "Keep server — attach my notes";
            this.btnKeepServer.Click += new System.EventHandler(this.btnKeepServer_Click);
            //
            // btnApplyMine
            //
            this.btnApplyMine.Location = new System.Drawing.Point(3, 3);
            this.btnApplyMine.Name = "btnApplyMine";
            this.btnApplyMine.Size = new System.Drawing.Size(180, 36);
            this.btnApplyMine.Text = "Apply my completion…";
            this.btnApplyMine.Click += new System.EventHandler(this.btnApplyMine_Click);
            //
            // SyncConflictPanel
            //
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.Controls.Add(this.lblConflictTitle);
            this.Controls.Add(this.lblConflictSubtitle);
            this.Controls.Add(this.pnlLocalVersion);
            this.Controls.Add(this.pnlServerVersion);
            this.Controls.Add(this.flpConflictActions);
            this.Name = "SyncConflictPanel";
            this.Size = new System.Drawing.Size(376, 248);
            this.Visible = false;
            this.pnlLocalVersion.ResumeLayout(false);
            this.pnlServerVersion.ResumeLayout(false);
            this.flpConflictActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblConflictTitle;
        private Wisej.Web.Label lblConflictSubtitle;
        private Wisej.Web.Panel pnlLocalVersion;
        private Wisej.Web.Label lblLocalHeader;
        private Wisej.Web.Label lblLocalBody;
        private Wisej.Web.Panel pnlServerVersion;
        private Wisej.Web.Label lblServerHeader;
        private Wisej.Web.Label lblServerBody;
        private Wisej.Web.FlowLayoutPanel flpConflictActions;
        private Wisej.Web.Button btnKeepServer;
        private Wisej.Web.Button btnApplyMine;
    }
}
