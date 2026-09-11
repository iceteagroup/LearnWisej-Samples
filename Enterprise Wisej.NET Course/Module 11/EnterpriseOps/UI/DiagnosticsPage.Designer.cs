namespace EnterpriseOps.UI
{
    partial class DiagnosticsPage
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
            this.pnlHeader = new Wisej.Web.Panel();
            this.lblScreenName = new Wisej.Web.Label();
            this.pnlSnapshot = new Wisej.Web.Panel();
            this.lblSnapshotTitle = new Wisej.Web.Label();
            this.lblRole = new Wisej.Web.Label();
            this.lblRedacted = new Wisej.Web.Label();
            this.pnlCardVersion = new Wisej.Web.Panel();
            this.lblVersionCaption = new Wisej.Web.Label();
            this.lblVersion = new Wisej.Web.Label();
            this.pnlCardEnvironment = new Wisej.Web.Panel();
            this.lblEnvironmentCaption = new Wisej.Web.Label();
            this.lblEnvironment = new Wisej.Web.Label();
            this.pnlCardNode = new Wisej.Web.Panel();
            this.lblNodeCaption = new Wisej.Web.Label();
            this.lblNode = new Wisej.Web.Label();
            this.pnlCardTheme = new Wisej.Web.Panel();
            this.lblThemeCaption = new Wisej.Web.Label();
            this.lblTheme = new Wisej.Web.Label();
            this.pnlHealth = new Wisej.Web.Panel();
            this.lblHealthTitle = new Wisej.Web.Label();
            this.lblHealth = new Wisej.Web.Label();
            this.lblSessions = new Wisej.Web.Label();
            this.lblUptime = new Wisej.Web.Label();
            this.lblMemory = new Wisej.Web.Label();
            this.lblFlags = new Wisej.Web.Label();
            this.pnlBudgets = new Wisej.Web.Panel();
            this.lblBudgetsTitle = new Wisej.Web.Label();
            this.btnRunQuery = new Wisej.Web.Button();
            this.btnSlowQuery = new Wisej.Web.Button();
            this.btnFixPageSize = new Wisej.Web.Button();
            this.perfBudgetPanel = new EnterpriseOps.UI.PerfBudgetPanel();
            this.pnlLog = new Wisej.Web.Panel();
            this.lblLogTitle = new Wisej.Web.Label();
            this.lstStructuredLog = new Wisej.Web.ListBox();
            this.lblStatus = new Wisej.Web.Label();
            this.timerLive = new Wisej.Web.Timer(this.components);
            this.pnlHeader.SuspendLayout();
            this.pnlSnapshot.SuspendLayout();
            this.pnlCardVersion.SuspendLayout();
            this.pnlCardEnvironment.SuspendLayout();
            this.pnlCardNode.SuspendLayout();
            this.pnlCardTheme.SuspendLayout();
            this.pnlHealth.SuspendLayout();
            this.pnlBudgets.SuspendLayout();
            this.pnlLog.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlHeader.Controls.Add(this.lblScreenName);
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(860, 44);
            //
            // lblScreenName
            //
            this.lblScreenName.AutoSize = false;
            this.lblScreenName.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblScreenName.ForeColor = System.Drawing.Color.White;
            this.lblScreenName.Location = new System.Drawing.Point(20, 10);
            this.lblScreenName.Name = "lblScreenName";
            this.lblScreenName.Size = new System.Drawing.Size(400, 24);
            this.lblScreenName.Text = "EnterpriseOps — Diagnostics";
            //
            // pnlSnapshot
            //
            this.pnlSnapshot.BackColor = System.Drawing.Color.White;
            this.pnlSnapshot.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlSnapshot.Controls.Add(this.lblSnapshotTitle);
            this.pnlSnapshot.Controls.Add(this.lblRole);
            this.pnlSnapshot.Controls.Add(this.lblRedacted);
            this.pnlSnapshot.Controls.Add(this.pnlCardVersion);
            this.pnlSnapshot.Controls.Add(this.pnlCardEnvironment);
            this.pnlSnapshot.Controls.Add(this.pnlCardNode);
            this.pnlSnapshot.Controls.Add(this.pnlCardTheme);
            this.pnlSnapshot.Location = new System.Drawing.Point(30, 60);
            this.pnlSnapshot.Name = "pnlSnapshot";
            this.pnlSnapshot.Size = new System.Drawing.Size(800, 118);
            //
            // lblSnapshotTitle
            //
            this.lblSnapshotTitle.AutoSize = false;
            this.lblSnapshotTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblSnapshotTitle.Location = new System.Drawing.Point(24, 12);
            this.lblSnapshotTitle.Name = "lblSnapshotTitle";
            this.lblSnapshotTitle.Size = new System.Drawing.Size(140, 26);
            this.lblSnapshotTitle.Text = "Diagnostics";
            //
            // lblRole
            //
            this.lblRole.AutoSize = false;
            this.lblRole.BackColor = System.Drawing.Color.FromArgb(240, 249, 243);
            this.lblRole.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblRole.ForeColor = System.Drawing.Color.FromArgb(15, 122, 58);
            this.lblRole.Location = new System.Drawing.Point(170, 14);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(190, 22);
            this.lblRole.Text = "Role: —";
            this.lblRole.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblRedacted
            //
            this.lblRedacted.AutoSize = false;
            this.lblRedacted.BackColor = System.Drawing.Color.FromArgb(255, 248, 236);
            this.lblRedacted.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblRedacted.ForeColor = System.Drawing.Color.FromArgb(122, 82, 16);
            this.lblRedacted.Location = new System.Drawing.Point(370, 14);
            this.lblRedacted.Name = "lblRedacted";
            this.lblRedacted.Size = new System.Drawing.Size(220, 22);
            this.lblRedacted.Text = "secrets redacted";
            this.lblRedacted.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // pnlCardVersion
            //
            this.pnlCardVersion.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.pnlCardVersion.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlCardVersion.Controls.Add(this.lblVersionCaption);
            this.pnlCardVersion.Controls.Add(this.lblVersion);
            this.pnlCardVersion.Location = new System.Drawing.Point(24, 48);
            this.pnlCardVersion.Name = "pnlCardVersion";
            this.pnlCardVersion.Size = new System.Drawing.Size(182, 56);
            //
            // lblVersionCaption
            //
            this.lblVersionCaption.AutoSize = false;
            this.lblVersionCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblVersionCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblVersionCaption.Location = new System.Drawing.Point(12, 6);
            this.lblVersionCaption.Name = "lblVersionCaption";
            this.lblVersionCaption.Size = new System.Drawing.Size(158, 16);
            this.lblVersionCaption.Text = "VERSION";
            //
            // lblVersion
            //
            this.lblVersion.AutoSize = false;
            this.lblVersion.AutoEllipsis = true;
            this.lblVersion.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.lblVersion.Location = new System.Drawing.Point(12, 24);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(158, 24);
            this.lblVersion.Text = "—";
            //
            // pnlCardEnvironment
            //
            this.pnlCardEnvironment.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.pnlCardEnvironment.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlCardEnvironment.Controls.Add(this.lblEnvironmentCaption);
            this.pnlCardEnvironment.Controls.Add(this.lblEnvironment);
            this.pnlCardEnvironment.Location = new System.Drawing.Point(216, 48);
            this.pnlCardEnvironment.Name = "pnlCardEnvironment";
            this.pnlCardEnvironment.Size = new System.Drawing.Size(182, 56);
            //
            // lblEnvironmentCaption
            //
            this.lblEnvironmentCaption.AutoSize = false;
            this.lblEnvironmentCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblEnvironmentCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblEnvironmentCaption.Location = new System.Drawing.Point(12, 6);
            this.lblEnvironmentCaption.Name = "lblEnvironmentCaption";
            this.lblEnvironmentCaption.Size = new System.Drawing.Size(158, 16);
            this.lblEnvironmentCaption.Text = "ENVIRONMENT";
            //
            // lblEnvironment
            //
            this.lblEnvironment.AutoSize = false;
            this.lblEnvironment.AutoEllipsis = true;
            this.lblEnvironment.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblEnvironment.Location = new System.Drawing.Point(12, 24);
            this.lblEnvironment.Name = "lblEnvironment";
            this.lblEnvironment.Size = new System.Drawing.Size(158, 24);
            this.lblEnvironment.Text = "—";
            //
            // pnlCardNode
            //
            this.pnlCardNode.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.pnlCardNode.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlCardNode.Controls.Add(this.lblNodeCaption);
            this.pnlCardNode.Controls.Add(this.lblNode);
            this.pnlCardNode.Location = new System.Drawing.Point(408, 48);
            this.pnlCardNode.Name = "pnlCardNode";
            this.pnlCardNode.Size = new System.Drawing.Size(182, 56);
            //
            // lblNodeCaption
            //
            this.lblNodeCaption.AutoSize = false;
            this.lblNodeCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblNodeCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblNodeCaption.Location = new System.Drawing.Point(12, 6);
            this.lblNodeCaption.Name = "lblNodeCaption";
            this.lblNodeCaption.Size = new System.Drawing.Size(158, 16);
            this.lblNodeCaption.Text = "NODE";
            //
            // lblNode
            //
            this.lblNode.AutoSize = false;
            this.lblNode.AutoEllipsis = true;
            this.lblNode.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblNode.Location = new System.Drawing.Point(12, 24);
            this.lblNode.Name = "lblNode";
            this.lblNode.Size = new System.Drawing.Size(158, 24);
            this.lblNode.Text = "—";
            //
            // pnlCardTheme
            //
            this.pnlCardTheme.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.pnlCardTheme.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlCardTheme.Controls.Add(this.lblThemeCaption);
            this.pnlCardTheme.Controls.Add(this.lblTheme);
            this.pnlCardTheme.Location = new System.Drawing.Point(600, 48);
            this.pnlCardTheme.Name = "pnlCardTheme";
            this.pnlCardTheme.Size = new System.Drawing.Size(176, 56);
            //
            // lblThemeCaption
            //
            this.lblThemeCaption.AutoSize = false;
            this.lblThemeCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblThemeCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblThemeCaption.Location = new System.Drawing.Point(12, 6);
            this.lblThemeCaption.Name = "lblThemeCaption";
            this.lblThemeCaption.Size = new System.Drawing.Size(152, 16);
            this.lblThemeCaption.Text = "THEME";
            //
            // lblTheme
            //
            this.lblTheme.AutoSize = false;
            this.lblTheme.AutoEllipsis = true;
            this.lblTheme.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblTheme.Location = new System.Drawing.Point(12, 24);
            this.lblTheme.Name = "lblTheme";
            this.lblTheme.Size = new System.Drawing.Size(152, 24);
            this.lblTheme.Text = "—";
            //
            // pnlHealth
            //
            this.pnlHealth.BackColor = System.Drawing.Color.White;
            this.pnlHealth.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlHealth.Controls.Add(this.lblHealthTitle);
            this.pnlHealth.Controls.Add(this.lblHealth);
            this.pnlHealth.Controls.Add(this.lblSessions);
            this.pnlHealth.Controls.Add(this.lblUptime);
            this.pnlHealth.Controls.Add(this.lblMemory);
            this.pnlHealth.Controls.Add(this.lblFlags);
            this.pnlHealth.Location = new System.Drawing.Point(30, 186);
            this.pnlHealth.Name = "pnlHealth";
            this.pnlHealth.Size = new System.Drawing.Size(800, 84);
            //
            // lblHealthTitle
            //
            this.lblHealthTitle.AutoSize = false;
            this.lblHealthTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblHealthTitle.Location = new System.Drawing.Point(24, 10);
            this.lblHealthTitle.Name = "lblHealthTitle";
            this.lblHealthTitle.Size = new System.Drawing.Size(320, 26);
            this.lblHealthTitle.Text = "Session & health";
            //
            // lblHealth
            //
            this.lblHealth.AutoSize = false;
            this.lblHealth.BackColor = System.Drawing.Color.FromArgb(240, 249, 243);
            this.lblHealth.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblHealth.ForeColor = System.Drawing.Color.FromArgb(15, 122, 58);
            this.lblHealth.Location = new System.Drawing.Point(640, 10);
            this.lblHealth.Name = "lblHealth";
            this.lblHealth.Size = new System.Drawing.Size(136, 26);
            this.lblHealth.Text = "—";
            this.lblHealth.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblSessions
            //
            this.lblSessions.AutoSize = false;
            this.lblSessions.Font = new System.Drawing.Font("monospace", 9F);
            this.lblSessions.Location = new System.Drawing.Point(24, 40);
            this.lblSessions.Name = "lblSessions";
            this.lblSessions.Size = new System.Drawing.Size(300, 18);
            this.lblSessions.Text = "sessions —";
            //
            // lblUptime
            //
            this.lblUptime.AutoSize = false;
            this.lblUptime.Font = new System.Drawing.Font("monospace", 9F);
            this.lblUptime.Location = new System.Drawing.Point(24, 58);
            this.lblUptime.Name = "lblUptime";
            this.lblUptime.Size = new System.Drawing.Size(300, 18);
            this.lblUptime.Text = "uptime —";
            //
            // lblMemory
            //
            this.lblMemory.AutoSize = false;
            this.lblMemory.AutoEllipsis = true;
            this.lblMemory.Font = new System.Drawing.Font("monospace", 9F);
            this.lblMemory.Location = new System.Drawing.Point(330, 40);
            this.lblMemory.Name = "lblMemory";
            this.lblMemory.Size = new System.Drawing.Size(446, 18);
            this.lblMemory.Text = "memory —";
            //
            // lblFlags
            //
            this.lblFlags.AutoSize = false;
            this.lblFlags.AutoEllipsis = true;
            this.lblFlags.Font = new System.Drawing.Font("monospace", 9F);
            this.lblFlags.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblFlags.Location = new System.Drawing.Point(330, 58);
            this.lblFlags.Name = "lblFlags";
            this.lblFlags.Size = new System.Drawing.Size(446, 18);
            this.lblFlags.Text = "flags —";
            //
            // pnlBudgets
            //
            this.pnlBudgets.BackColor = System.Drawing.Color.White;
            this.pnlBudgets.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlBudgets.Controls.Add(this.lblBudgetsTitle);
            this.pnlBudgets.Controls.Add(this.btnRunQuery);
            this.pnlBudgets.Controls.Add(this.btnSlowQuery);
            this.pnlBudgets.Controls.Add(this.btnFixPageSize);
            this.pnlBudgets.Controls.Add(this.perfBudgetPanel);
            this.pnlBudgets.Location = new System.Drawing.Point(30, 278);
            this.pnlBudgets.Name = "pnlBudgets";
            this.pnlBudgets.Size = new System.Drawing.Size(800, 190);
            //
            // lblBudgetsTitle
            //
            this.lblBudgetsTitle.AutoSize = false;
            this.lblBudgetsTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblBudgetsTitle.Location = new System.Drawing.Point(24, 10);
            this.lblBudgetsTitle.Name = "lblBudgetsTitle";
            this.lblBudgetsTitle.Size = new System.Drawing.Size(320, 26);
            this.lblBudgetsTitle.Text = "Performance budgets — live";
            //
            // btnRunQuery
            //
            this.btnRunQuery.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.btnRunQuery.Location = new System.Drawing.Point(360, 8);
            this.btnRunQuery.Name = "btnRunQuery";
            this.btnRunQuery.Size = new System.Drawing.Size(130, 28);
            this.btnRunQuery.Text = "Run query · 50";
            this.btnRunQuery.Click += new System.EventHandler(this.btnRunQuery_Click);
            //
            // btnSlowQuery
            //
            this.btnSlowQuery.Location = new System.Drawing.Point(498, 8);
            this.btnSlowQuery.Name = "btnSlowQuery";
            this.btnSlowQuery.Size = new System.Drawing.Size(150, 28);
            this.btnSlowQuery.Text = "Slow query · 5000";
            this.btnSlowQuery.Click += new System.EventHandler(this.btnSlowQuery_Click);
            //
            // btnFixPageSize
            //
            this.btnFixPageSize.Location = new System.Drawing.Point(656, 8);
            this.btnFixPageSize.Name = "btnFixPageSize";
            this.btnFixPageSize.Size = new System.Drawing.Size(120, 28);
            this.btnFixPageSize.Text = "Fix page size";
            this.btnFixPageSize.Click += new System.EventHandler(this.btnFixPageSize_Click);
            //
            // perfBudgetPanel
            //
            this.perfBudgetPanel.Location = new System.Drawing.Point(24, 42);
            this.perfBudgetPanel.Name = "perfBudgetPanel";
            this.perfBudgetPanel.Size = new System.Drawing.Size(752, 136);
            //
            // pnlLog
            //
            this.pnlLog.BackColor = System.Drawing.Color.White;
            this.pnlLog.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlLog.Controls.Add(this.lblLogTitle);
            this.pnlLog.Controls.Add(this.lstStructuredLog);
            this.pnlLog.Location = new System.Drawing.Point(30, 476);
            this.pnlLog.Name = "pnlLog";
            this.pnlLog.Size = new System.Drawing.Size(800, 134);
            //
            // lblLogTitle
            //
            this.lblLogTitle.AutoSize = false;
            this.lblLogTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblLogTitle.Location = new System.Drawing.Point(24, 10);
            this.lblLogTitle.Name = "lblLogTitle";
            this.lblLogTitle.Size = new System.Drawing.Size(320, 26);
            this.lblLogTitle.Text = "Structured log";
            //
            // lstStructuredLog
            //
            this.lstStructuredLog.Font = new System.Drawing.Font("monospace", 8F);
            this.lstStructuredLog.Location = new System.Drawing.Point(24, 40);
            this.lstStructuredLog.Name = "lstStructuredLog";
            this.lstStructuredLog.Size = new System.Drawing.Size(752, 84);
            //
            // lblStatus
            //
            this.lblStatus.AutoEllipsis = true;
            this.lblStatus.AutoSize = false;
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.lblStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblStatus.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(159, 192, 232);
            this.lblStatus.Location = new System.Drawing.Point(0, 626);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new Wisej.Web.Padding(16, 0, 16, 0);
            this.lblStatus.Size = new System.Drawing.Size(860, 30);
            this.lblStatus.Text = "Ready";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // timerLive
            //
            this.timerLive.Interval = 1000;
            this.timerLive.Tick += new System.EventHandler(this.timerLive_Tick);
            //
            // DiagnosticsPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlSnapshot);
            this.Controls.Add(this.pnlHealth);
            this.Controls.Add(this.pnlBudgets);
            this.Controls.Add(this.pnlLog);
            this.Controls.Add(this.lblStatus);
            this.Name = "DiagnosticsPage";
            this.Size = new System.Drawing.Size(860, 656);
            this.Text = "EnterpriseOps — Diagnostics";
            this.Load += new System.EventHandler(this.DiagnosticsPage_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlSnapshot.ResumeLayout(false);
            this.pnlCardVersion.ResumeLayout(false);
            this.pnlCardEnvironment.ResumeLayout(false);
            this.pnlCardNode.ResumeLayout(false);
            this.pnlCardTheme.ResumeLayout(false);
            this.pnlHealth.ResumeLayout(false);
            this.pnlBudgets.ResumeLayout(false);
            this.pnlLog.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblScreenName;
        private Wisej.Web.Panel pnlSnapshot;
        private Wisej.Web.Label lblSnapshotTitle;
        private Wisej.Web.Label lblRole;
        private Wisej.Web.Label lblRedacted;
        private Wisej.Web.Panel pnlCardVersion;
        private Wisej.Web.Label lblVersionCaption;
        private Wisej.Web.Label lblVersion;
        private Wisej.Web.Panel pnlCardEnvironment;
        private Wisej.Web.Label lblEnvironmentCaption;
        private Wisej.Web.Label lblEnvironment;
        private Wisej.Web.Panel pnlCardNode;
        private Wisej.Web.Label lblNodeCaption;
        private Wisej.Web.Label lblNode;
        private Wisej.Web.Panel pnlCardTheme;
        private Wisej.Web.Label lblThemeCaption;
        private Wisej.Web.Label lblTheme;
        private Wisej.Web.Panel pnlHealth;
        private Wisej.Web.Label lblHealthTitle;
        private Wisej.Web.Label lblHealth;
        private Wisej.Web.Label lblSessions;
        private Wisej.Web.Label lblUptime;
        private Wisej.Web.Label lblMemory;
        private Wisej.Web.Label lblFlags;
        private Wisej.Web.Panel pnlBudgets;
        private Wisej.Web.Label lblBudgetsTitle;
        private Wisej.Web.Button btnRunQuery;
        private Wisej.Web.Button btnSlowQuery;
        private Wisej.Web.Button btnFixPageSize;
        private EnterpriseOps.UI.PerfBudgetPanel perfBudgetPanel;
        private Wisej.Web.Panel pnlLog;
        private Wisej.Web.Label lblLogTitle;
        private Wisej.Web.ListBox lstStructuredLog;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Timer timerLive;
    }
}
