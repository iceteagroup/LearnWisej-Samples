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
            this.lblTenant = new Wisej.Web.Label();
            this.lblUserCaption = new Wisej.Web.Label();
            this.cboUser = new Wisej.Web.ComboBox();
            this.lblCorrelation = new Wisej.Web.Label();
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
            this.lblBudgetsHint = new Wisej.Web.Label();
            this.perfBudgetPanel = new EnterpriseOps.UI.PerfBudgetPanel();
            this.pnlLog = new Wisej.Web.Panel();
            this.lblLogTitle = new Wisej.Web.Label();
            this.lstStructuredLog = new Wisej.Web.ListBox();
            this.pnlTrace = new Wisej.Web.Panel();
            this.lblTraceTitle = new Wisej.Web.Label();
            this.lstTrace = new Wisej.Web.ListBox();
            this.lblBanner = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.lblTraceFooter = new Wisej.Web.Label();
            this.pnlActions = new Wisej.Web.Panel();
            this.btnRunQuery = new Wisej.Web.Button();
            this.btnSlowQuery = new Wisej.Web.Button();
            this.btnFixPageSize = new Wisej.Web.Button();
            this.btnThrow = new Wisej.Web.Button();
            this.btnLeakSession = new Wisej.Web.Button();
            this.btnReleaseLeak = new Wisej.Web.Button();
            this.btnLive = new Wisej.Web.Button();
            this.btnHealth = new Wisej.Web.Button();
            this.btnClearTrace = new Wisej.Web.Button();
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
            this.pnlTrace.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader  (slim header bar: screen · tenant · user · correlation id)
            //
            this.pnlHeader.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlHeader.Controls.Add(this.lblScreenName);
            this.pnlHeader.Controls.Add(this.lblTenant);
            this.pnlHeader.Controls.Add(this.lblUserCaption);
            this.pnlHeader.Controls.Add(this.cboUser);
            this.pnlHeader.Controls.Add(this.lblCorrelation);
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1348, 44);
            //
            // lblScreenName
            //
            this.lblScreenName.AutoSize = false;
            this.lblScreenName.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblScreenName.ForeColor = System.Drawing.Color.White;
            this.lblScreenName.Location = new System.Drawing.Point(20, 10);
            this.lblScreenName.Name = "lblScreenName";
            this.lblScreenName.Size = new System.Drawing.Size(300, 24);
            this.lblScreenName.Text = "EnterpriseOps — Diagnostics";
            //
            // lblTenant
            //
            this.lblTenant.AutoSize = false;
            this.lblTenant.Font = new System.Drawing.Font("default", 10F);
            this.lblTenant.ForeColor = System.Drawing.Color.White;
            this.lblTenant.Location = new System.Drawing.Point(330, 12);
            this.lblTenant.Name = "lblTenant";
            this.lblTenant.Size = new System.Drawing.Size(150, 20);
            this.lblTenant.Text = "tenant fabrikam";
            //
            // lblUserCaption
            //
            this.lblUserCaption.AutoSize = false;
            this.lblUserCaption.Font = new System.Drawing.Font("default", 10F);
            this.lblUserCaption.ForeColor = System.Drawing.Color.White;
            this.lblUserCaption.Location = new System.Drawing.Point(490, 12);
            this.lblUserCaption.Name = "lblUserCaption";
            this.lblUserCaption.Size = new System.Drawing.Size(40, 20);
            this.lblUserCaption.Text = "user";
            //
            // cboUser  (switching to ben.tech is the role-check failure path)
            //
            this.cboUser.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboUser.Location = new System.Drawing.Point(530, 8);
            this.cboUser.Name = "cboUser";
            this.cboUser.Size = new System.Drawing.Size(190, 28);
            this.cboUser.ToolTipText = "Who is looking at the page. Technicians are denied by DiagnosticsAccessPolicy, and the attempt is audited.";
            this.cboUser.SelectedIndexChanged += new System.EventHandler(this.cboUser_SelectedIndexChanged);
            //
            // lblCorrelation
            //
            this.lblCorrelation.AutoSize = false;
            this.lblCorrelation.Font = new System.Drawing.Font("monospace", 9F);
            this.lblCorrelation.ForeColor = System.Drawing.Color.White;
            this.lblCorrelation.Location = new System.Drawing.Point(740, 12);
            this.lblCorrelation.Name = "lblCorrelation";
            this.lblCorrelation.Size = new System.Drawing.Size(320, 20);
            this.lblCorrelation.Text = "correlation —";
            //
            // pnlSnapshot  (Diagnostics · DiagnosticSnapshot bound to four cards)
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
            // lblRole  (chip: the DiagnosticsAccessPolicy decision)
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
            // lblRedacted  (chip: what DiagnosticsService kept out of the snapshot)
            //
            this.lblRedacted.AutoSize = false;
            this.lblRedacted.BackColor = System.Drawing.Color.FromArgb(255, 248, 236);
            this.lblRedacted.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblRedacted.ForeColor = System.Drawing.Color.FromArgb(122, 82, 16);
            this.lblRedacted.Location = new System.Drawing.Point(370, 14);
            this.lblRedacted.Name = "lblRedacted";
            this.lblRedacted.Size = new System.Drawing.Size(406, 22);
            this.lblRedacted.Text = "secrets redacted";
            this.lblRedacted.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblRedacted.ToolTipText = "The full redaction list is in the trace: what was excluded by type and what was masked.";
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
            this.lblVersionCaption.AutoSize = false;
            this.lblVersionCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblVersionCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblVersionCaption.Location = new System.Drawing.Point(12, 6);
            this.lblVersionCaption.Name = "lblVersionCaption";
            this.lblVersionCaption.Size = new System.Drawing.Size(158, 16);
            this.lblVersionCaption.Text = "VERSION";
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
            this.lblEnvironmentCaption.AutoSize = false;
            this.lblEnvironmentCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblEnvironmentCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblEnvironmentCaption.Location = new System.Drawing.Point(12, 6);
            this.lblEnvironmentCaption.Name = "lblEnvironmentCaption";
            this.lblEnvironmentCaption.Size = new System.Drawing.Size(158, 16);
            this.lblEnvironmentCaption.Text = "ENVIRONMENT";
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
            this.lblNodeCaption.AutoSize = false;
            this.lblNodeCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblNodeCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblNodeCaption.Location = new System.Drawing.Point(12, 6);
            this.lblNodeCaption.Name = "lblNodeCaption";
            this.lblNodeCaption.Size = new System.Drawing.Size(158, 16);
            this.lblNodeCaption.Text = "NODE";
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
            this.lblThemeCaption.AutoSize = false;
            this.lblThemeCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblThemeCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblThemeCaption.Location = new System.Drawing.Point(12, 6);
            this.lblThemeCaption.Name = "lblThemeCaption";
            this.lblThemeCaption.Size = new System.Drawing.Size(152, 16);
            this.lblThemeCaption.Text = "THEME";
            this.lblTheme.AutoSize = false;
            this.lblTheme.AutoEllipsis = true;
            this.lblTheme.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblTheme.Location = new System.Drawing.Point(12, 24);
            this.lblTheme.Name = "lblTheme";
            this.lblTheme.Size = new System.Drawing.Size(152, 24);
            this.lblTheme.Text = "—";
            //
            // pnlHealth  (Session & health · live)
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
            this.lblHealthTitle.Text = "Session & health  ·  live";
            //
            // lblHealth  (chip: HEALTHY / DEGRADED / UNHEALTHY from HealthCheck.Run)
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
            // lblSessions / lblUptime / lblMemory / lblFlags
            //
            this.lblSessions.AutoSize = false;
            this.lblSessions.Font = new System.Drawing.Font("monospace", 9F);
            this.lblSessions.Location = new System.Drawing.Point(24, 40);
            this.lblSessions.Name = "lblSessions";
            this.lblSessions.Size = new System.Drawing.Size(300, 18);
            this.lblSessions.Text = "sessions —";
            this.lblUptime.AutoSize = false;
            this.lblUptime.Font = new System.Drawing.Font("monospace", 9F);
            this.lblUptime.Location = new System.Drawing.Point(24, 58);
            this.lblUptime.Name = "lblUptime";
            this.lblUptime.Size = new System.Drawing.Size(300, 18);
            this.lblUptime.Text = "uptime —";
            this.lblMemory.AutoSize = false;
            this.lblMemory.AutoEllipsis = true;
            this.lblMemory.Font = new System.Drawing.Font("monospace", 9F);
            this.lblMemory.Location = new System.Drawing.Point(330, 40);
            this.lblMemory.Name = "lblMemory";
            this.lblMemory.Size = new System.Drawing.Size(446, 18);
            this.lblMemory.Text = "memory —";
            this.lblFlags.AutoSize = false;
            this.lblFlags.AutoEllipsis = true;
            this.lblFlags.Font = new System.Drawing.Font("monospace", 9F);
            this.lblFlags.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblFlags.Location = new System.Drawing.Point(330, 58);
            this.lblFlags.Name = "lblFlags";
            this.lblFlags.Size = new System.Drawing.Size(446, 18);
            this.lblFlags.Text = "flags —";
            //
            // pnlBudgets  (Performance budgets — live)
            //
            this.pnlBudgets.BackColor = System.Drawing.Color.White;
            this.pnlBudgets.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlBudgets.Controls.Add(this.lblBudgetsTitle);
            this.pnlBudgets.Controls.Add(this.lblBudgetsHint);
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
            this.lblBudgetsTitle.Text = "Performance budgets  ·  live";
            //
            // lblBudgetsHint
            //
            this.lblBudgetsHint.AutoSize = false;
            this.lblBudgetsHint.Font = new System.Drawing.Font("default", 8F);
            this.lblBudgetsHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblBudgetsHint.Location = new System.Drawing.Point(350, 12);
            this.lblBudgetsHint.Name = "lblBudgetsHint";
            this.lblBudgetsHint.Size = new System.Drawing.Size(426, 22);
            this.lblBudgetsHint.Text = "measured ≤ budget → OK ✓   ·   over budget → the row turns red";
            this.lblBudgetsHint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // perfBudgetPanel  (PerfBudgetPanel : UserControl hosting dgvBudgets)
            //
            this.perfBudgetPanel.Location = new System.Drawing.Point(24, 42);
            this.perfBudgetPanel.Name = "perfBudgetPanel";
            this.perfBudgetPanel.Size = new System.Drawing.Size(752, 136);
            //
            // pnlLog  (Structured log · JSON lines)
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
            this.lblLogTitle.Size = new System.Drawing.Size(600, 26);
            this.lblLogTitle.Text = "Structured log  ·  JSON lines (fields, not prose)";
            //
            // lstStructuredLog
            //
            this.lstStructuredLog.Font = new System.Drawing.Font("monospace", 8F);
            this.lstStructuredLog.Location = new System.Drawing.Point(24, 40);
            this.lstStructuredLog.Name = "lstStructuredLog";
            this.lstStructuredLog.Size = new System.Drawing.Size(752, 84);
            //
            // pnlTrace  (Server · live activity trace)
            //
            this.pnlTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlTrace.BackColor = System.Drawing.Color.White;
            this.pnlTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlTrace.Controls.Add(this.lblTraceTitle);
            this.pnlTrace.Controls.Add(this.lstTrace);
            this.pnlTrace.Controls.Add(this.lblBanner);
            this.pnlTrace.Controls.Add(this.lblStatus);
            this.pnlTrace.Controls.Add(this.lblTraceFooter);
            this.pnlTrace.Location = new System.Drawing.Point(858, 60);
            this.pnlTrace.Name = "pnlTrace";
            this.pnlTrace.Size = new System.Drawing.Size(460, 550);
            //
            // lblTraceTitle
            //
            this.lblTraceTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblTraceTitle.AutoSize = false;
            this.lblTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTraceTitle.Location = new System.Drawing.Point(20, 12);
            this.lblTraceTitle.Name = "lblTraceTitle";
            this.lblTraceTitle.Size = new System.Drawing.Size(420, 26);
            this.lblTraceTitle.Text = "Server  ·  live activity trace";
            //
            // lstTrace
            //
            this.lstTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstTrace.Font = new System.Drawing.Font("monospace", 8F);
            this.lstTrace.Location = new System.Drawing.Point(20, 44);
            this.lstTrace.Name = "lstTrace";
            this.lstTrace.Size = new System.Drawing.Size(420, 384);
            //
            // lblBanner  (failure / recovery banner)
            //
            this.lblBanner.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblBanner.AutoSize = false;
            this.lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.lblBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.lblBanner.Location = new System.Drawing.Point(20, 436);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Padding = new Wisej.Web.Padding(10, 4, 10, 4);
            this.lblBanner.Size = new System.Drawing.Size(420, 62);
            this.lblBanner.Text = "";
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBanner.Visible = false;
            //
            // lblStatus
            //
            this.lblStatus.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblStatus.AutoSize = false;
            this.lblStatus.AutoEllipsis = true;
            this.lblStatus.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Location = new System.Drawing.Point(20, 504);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(420, 22);
            this.lblStatus.Text = "● starting…";
            //
            // lblTraceFooter
            //
            this.lblTraceFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblTraceFooter.AutoSize = false;
            this.lblTraceFooter.Font = new System.Drawing.Font("default", 8F);
            this.lblTraceFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblTraceFooter.Location = new System.Drawing.Point(20, 526);
            this.lblTraceFooter.Name = "lblTraceFooter";
            this.lblTraceFooter.Size = new System.Drawing.Size(420, 18);
            this.lblTraceFooter.Text = "UI →  ·  Service:  ·  Data:  ·  Diagnostics:  ·  Security:  ·  Log:  ·  Job:";
            //
            // pnlActions  (bottom bar: success · failure · recovery · progress · clear)
            //
            this.pnlActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlActions.Controls.Add(this.btnRunQuery);
            this.pnlActions.Controls.Add(this.btnSlowQuery);
            this.pnlActions.Controls.Add(this.btnFixPageSize);
            this.pnlActions.Controls.Add(this.btnThrow);
            this.pnlActions.Controls.Add(this.btnLeakSession);
            this.pnlActions.Controls.Add(this.btnReleaseLeak);
            this.pnlActions.Controls.Add(this.btnLive);
            this.pnlActions.Controls.Add(this.btnHealth);
            this.pnlActions.Controls.Add(this.btnClearTrace);
            this.pnlActions.Location = new System.Drawing.Point(30, 626);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(1288, 44);
            //
            // btnRunQuery  (success path)
            //
            this.btnRunQuery.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.btnRunQuery.Location = new System.Drawing.Point(0, 4);
            this.btnRunQuery.Name = "btnRunQuery";
            this.btnRunQuery.Size = new System.Drawing.Size(150, 36);
            this.btnRunQuery.Text = "Run query · 50";
            this.btnRunQuery.ToolTipText = "SearchWorkOrders with the current page size, timed by OperationTimer, judged against the budget.";
            this.btnRunQuery.Click += new System.EventHandler(this.btnRunQuery_Click);
            //
            // btnSlowQuery  (failure path 1: the bypassed paged query)
            //
            this.btnSlowQuery.Location = new System.Drawing.Point(158, 4);
            this.btnSlowQuery.Name = "btnSlowQuery";
            this.btnSlowQuery.Size = new System.Drawing.Size(170, 36);
            this.btnSlowQuery.Text = "Slow query · 5000";
            this.btnSlowQuery.ToolTipText = "Overrides the page size to 5,000 and runs the same handler: 2,340 ms, over budget, and the log names the cause.";
            this.btnSlowQuery.Click += new System.EventHandler(this.btnSlowQuery_Click);
            //
            // btnFixPageSize  (recovery 1)
            //
            this.btnFixPageSize.Location = new System.Drawing.Point(336, 4);
            this.btnFixPageSize.Name = "btnFixPageSize";
            this.btnFixPageSize.Size = new System.Drawing.Size(130, 36);
            this.btnFixPageSize.Text = "Fix page size";
            this.btnFixPageSize.ToolTipText = "Back to pageSize 50 and run again: the budget row turns green.";
            this.btnFixPageSize.Click += new System.EventHandler(this.btnFixPageSize_Click);
            //
            // btnThrow  (failure path 2: an operation that throws)
            //
            this.btnThrow.Location = new System.Drawing.Point(474, 4);
            this.btnThrow.Name = "btnThrow";
            this.btnThrow.Size = new System.Drawing.Size(150, 36);
            this.btnThrow.Text = "Store failure";
            this.btnThrow.ToolTipText = "The store throws a connection error on the next call: full detail goes to the log with the correlation id; the user gets a safe message.";
            this.btnThrow.Click += new System.EventHandler(this.btnThrow_Click);
            //
            // btnLeakSession  (failure path 3: session memory growth)
            //
            this.btnLeakSession.Location = new System.Drawing.Point(632, 4);
            this.btnLeakSession.Name = "btnLeakSession";
            this.btnLeakSession.Size = new System.Drawing.Size(170, 36);
            this.btnLeakSession.Text = "Leak: cache 50k rows";
            this.btnLeakSession.ToolTipText = "ReportCacheService keeps a 50,000-row report in a session field; the session-memory audit flags it.";
            this.btnLeakSession.Click += new System.EventHandler(this.btnLeakSession_Click);
            //
            // btnReleaseLeak  (recovery 3)
            //
            this.btnReleaseLeak.Location = new System.Drawing.Point(810, 4);
            this.btnReleaseLeak.Name = "btnReleaseLeak";
            this.btnReleaseLeak.Size = new System.Drawing.Size(110, 36);
            this.btnReleaseLeak.Text = "Release cache";
            this.btnReleaseLeak.ToolTipText = "Drops the cached report and re-runs the audit.";
            this.btnReleaseLeak.Click += new System.EventHandler(this.btnReleaseLeak_Click);
            //
            // btnLive  (progress path: a Timer refreshes the live numbers every second)
            //
            this.btnLive.Location = new System.Drawing.Point(928, 4);
            this.btnLive.Name = "btnLive";
            this.btnLive.Size = new System.Drawing.Size(120, 36);
            this.btnLive.Text = "▶ Live refresh";
            this.btnLive.ToolTipText = "timerLive (1 s): session count, memory, uptime and health, each tick timed as 'Background job tick'.";
            this.btnLive.Click += new System.EventHandler(this.btnLive_Click);
            //
            // btnHealth
            //
            this.btnHealth.Location = new System.Drawing.Point(1056, 4);
            this.btnHealth.Name = "btnHealth";
            this.btnHealth.Size = new System.Drawing.Size(110, 36);
            this.btnHealth.Text = "Health check";
            this.btnHealth.ToolTipText = "HealthCheck.Run: store, background job, budgets, session memory, structured log.";
            this.btnHealth.Click += new System.EventHandler(this.btnHealth_Click);
            //
            // btnClearTrace
            //
            this.btnClearTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnClearTrace.Location = new System.Drawing.Point(1188, 4);
            this.btnClearTrace.Name = "btnClearTrace";
            this.btnClearTrace.Size = new System.Drawing.Size(100, 36);
            this.btnClearTrace.Text = "Clear trace";
            this.btnClearTrace.Click += new System.EventHandler(this.btnClearTrace_Click);
            //
            // timerLive  (a Component with no visual surface — stopped in DiagnosticsPage_Disposed)
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
            this.Controls.Add(this.pnlTrace);
            this.Controls.Add(this.pnlActions);
            this.Name = "DiagnosticsPage";
            this.Size = new System.Drawing.Size(1348, 680);
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
            this.pnlTrace.ResumeLayout(false);
            this.pnlActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblScreenName;
        private Wisej.Web.Label lblTenant;
        private Wisej.Web.Label lblUserCaption;
        private Wisej.Web.ComboBox cboUser;
        private Wisej.Web.Label lblCorrelation;
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
        private Wisej.Web.Label lblBudgetsHint;
        private EnterpriseOps.UI.PerfBudgetPanel perfBudgetPanel;
        private Wisej.Web.Panel pnlLog;
        private Wisej.Web.Label lblLogTitle;
        private Wisej.Web.ListBox lstStructuredLog;
        private Wisej.Web.Panel pnlTrace;
        private Wisej.Web.Label lblTraceTitle;
        private Wisej.Web.ListBox lstTrace;
        private Wisej.Web.Label lblBanner;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Label lblTraceFooter;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.Button btnRunQuery;
        private Wisej.Web.Button btnSlowQuery;
        private Wisej.Web.Button btnFixPageSize;
        private Wisej.Web.Button btnThrow;
        private Wisej.Web.Button btnLeakSession;
        private Wisej.Web.Button btnReleaseLeak;
        private Wisej.Web.Button btnLive;
        private Wisej.Web.Button btnHealth;
        private Wisej.Web.Button btnClearTrace;
        private Wisej.Web.Timer timerLive;
    }
}
