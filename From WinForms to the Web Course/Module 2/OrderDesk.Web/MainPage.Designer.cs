namespace OrderDesk
{
    partial class MainPage
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelShell = new Wisej.Web.Panel();
            this.labelShellTitle = new Wisej.Web.Label();
            this.labelStatus = new Wisej.Web.Label();
            this.gridShell = new Wisej.Web.DataGridView();
            this.colShellFile = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colShellReplaces = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colShellRole = new Wisej.Web.DataGridViewTextBoxColumn();
            this.labelShellPreview = new Wisej.Web.Label();
            this.panelErrors = new Wisej.Web.Panel();
            this.labelErrorsTitle = new Wisej.Web.Label();
            this.labelErrorsCount = new Wisej.Web.Label();
            this.buttonCatAll = new Wisej.Web.Button();
            this.buttonCatNamespace = new Wisej.Web.Button();
            this.buttonCatControls = new Wisej.Web.Button();
            this.buttonCatStyling = new Wisej.Web.Button();
            this.buttonCatDesktop = new Wisej.Web.Button();
            this.buttonCatDeferred = new Wisej.Web.Button();
            this.gridErrors = new Wisej.Web.DataGridView();
            this.colErrSymbol = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colErrCode = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colErrCount = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colErrCategory = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colErrFix = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colErrModule = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colErrOpen = new Wisej.Web.DataGridViewTextBoxColumn();
            this.labelErrorDetail = new Wisej.Web.Label();
            this.buttonReplayFunnel = new Wisej.Web.Button();
            this.labelFunnelCount = new Wisej.Web.Label();
            this.labelFunnelPass = new Wisej.Web.Label();
            this.timerBuild = new Wisej.Web.Timer(this.components);
            this.trace = new OrderDesk.Views.TracePanel();
            this.panelConfig = new Wisej.Web.Panel();
            this.labelConfigTitle = new Wisej.Web.Label();
            this.buttonLegacyConfig = new Wisej.Web.Button();
            this.buttonWebConfig = new Wisej.Web.Button();
            this.buttonOpenOrdersForm = new Wisej.Web.Button();
            this.buttonClear = new Wisej.Web.Button();
            this.labelConfigValues = new Wisej.Web.Label();
            this.labelBanner = new Wisej.Web.Label();
            this.panelShell.SuspendLayout();
            this.panelErrors.SuspendLayout();
            this.panelConfig.SuspendLayout();
            this.SuspendLayout();
            //
            // panelShell  (deliverable 1: the Wisej.NET shell, file by file — the real files of this project)
            //
            this.panelShell.BackColor = System.Drawing.Color.White;
            this.panelShell.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelShell.Controls.Add(this.labelShellTitle);
            this.panelShell.Controls.Add(this.labelStatus);
            this.panelShell.Controls.Add(this.gridShell);
            this.panelShell.Controls.Add(this.labelShellPreview);
            this.panelShell.Location = new System.Drawing.Point(30, 30);
            this.panelShell.Name = "panelShell";
            this.panelShell.Size = new System.Drawing.Size(640, 290);
            //
            // labelShellTitle
            //
            this.labelShellTitle.AutoSize = false;
            this.labelShellTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelShellTitle.Location = new System.Drawing.Point(20, 14);
            this.labelShellTitle.Name = "labelShellTitle";
            this.labelShellTitle.Size = new System.Drawing.Size(390, 30);
            this.labelShellTitle.Text = "Wisej.NET shell · what replaces Application.Run";
            //
            // labelStatus
            //
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelStatus.Location = new System.Drawing.Point(410, 18);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(212, 24);
            this.labelStatus.Text = "● loading";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // gridShell  (File | Replaces | Role)
            //
            this.gridShell.AllowUserToAddRows = false;
            this.gridShell.AllowUserToDeleteRows = false;
            this.gridShell.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colShellFile, this.colShellReplaces, this.colShellRole });
            this.gridShell.Location = new System.Drawing.Point(20, 50);
            this.gridShell.MultiSelect = false;
            this.gridShell.Name = "gridShell";
            this.gridShell.ReadOnly = true;
            this.gridShell.RowHeadersVisible = false;
            this.gridShell.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridShell.Size = new System.Drawing.Size(602, 112);
            this.gridShell.SelectionChanged += new System.EventHandler(this.gridShell_SelectionChanged);
            this.colShellFile.HeaderText = "File"; this.colShellFile.Name = "colShellFile"; this.colShellFile.Width = 120; this.colShellFile.ReadOnly = true;
            this.colShellReplaces.HeaderText = "Replaces"; this.colShellReplaces.Name = "colShellReplaces"; this.colShellReplaces.Width = 200; this.colShellReplaces.ReadOnly = true;
            this.colShellRole.HeaderText = "Role"; this.colShellRole.Name = "colShellRole"; this.colShellRole.Width = 280; this.colShellRole.ReadOnly = true;
            //
            // labelShellPreview  (the first lines of the selected file, read from Application.StartupPath)
            //
            this.labelShellPreview.AutoSize = false;
            this.labelShellPreview.BackColor = System.Drawing.Color.FromArgb(246, 248, 250);
            this.labelShellPreview.Font = new System.Drawing.Font("monospace", 9F);
            this.labelShellPreview.ForeColor = System.Drawing.Color.FromArgb(40, 52, 66);
            this.labelShellPreview.Location = new System.Drawing.Point(20, 170);
            this.labelShellPreview.Name = "labelShellPreview";
            this.labelShellPreview.Padding = new Wisej.Web.Padding(8, 4, 8, 4);
            this.labelShellPreview.Size = new System.Drawing.Size(602, 106);
            this.labelShellPreview.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelErrors  (deliverable 2: the compiler error log, categorized, and the build funnel)
            //
            this.panelErrors.BackColor = System.Drawing.Color.White;
            this.panelErrors.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelErrors.Controls.Add(this.labelErrorsTitle);
            this.panelErrors.Controls.Add(this.labelErrorsCount);
            this.panelErrors.Controls.Add(this.buttonCatAll);
            this.panelErrors.Controls.Add(this.buttonCatNamespace);
            this.panelErrors.Controls.Add(this.buttonCatControls);
            this.panelErrors.Controls.Add(this.buttonCatStyling);
            this.panelErrors.Controls.Add(this.buttonCatDesktop);
            this.panelErrors.Controls.Add(this.buttonCatDeferred);
            this.panelErrors.Controls.Add(this.gridErrors);
            this.panelErrors.Controls.Add(this.labelErrorDetail);
            this.panelErrors.Controls.Add(this.buttonReplayFunnel);
            this.panelErrors.Controls.Add(this.labelFunnelCount);
            this.panelErrors.Controls.Add(this.labelFunnelPass);
            this.panelErrors.Location = new System.Drawing.Point(30, 334);
            this.panelErrors.Name = "panelErrors";
            this.panelErrors.Size = new System.Drawing.Size(640, 320);
            //
            // labelErrorsTitle / labelErrorsCount
            //
            this.labelErrorsTitle.AutoSize = false;
            this.labelErrorsTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelErrorsTitle.Location = new System.Drawing.Point(20, 12);
            this.labelErrorsTitle.Name = "labelErrorsTitle";
            this.labelErrorsTitle.Size = new System.Drawing.Size(300, 28);
            this.labelErrorsTitle.Text = "Compiler errors · categorized";
            this.labelErrorsCount.AutoSize = false;
            this.labelErrorsCount.Font = new System.Drawing.Font("default", 9F);
            this.labelErrorsCount.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelErrorsCount.Location = new System.Drawing.Point(320, 14);
            this.labelErrorsCount.Name = "labelErrorsCount";
            this.labelErrorsCount.Size = new System.Drawing.Size(302, 24);
            this.labelErrorsCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // category filter buttons: All + the five categories
            //
            this.buttonCatAll.Location = new System.Drawing.Point(20, 46);
            this.buttonCatAll.Name = "buttonCatAll";
            this.buttonCatAll.Size = new System.Drawing.Size(50, 28);
            this.buttonCatAll.Text = "All";
            this.buttonCatAll.Click += new System.EventHandler(this.buttonCatAll_Click);
            this.buttonCatNamespace.Location = new System.Drawing.Point(76, 46);
            this.buttonCatNamespace.Name = "buttonCatNamespace";
            this.buttonCatNamespace.Size = new System.Drawing.Size(96, 28);
            this.buttonCatNamespace.Text = "Namespace";
            this.buttonCatNamespace.Click += new System.EventHandler(this.buttonCatNamespace_Click);
            this.buttonCatControls.Location = new System.Drawing.Point(178, 46);
            this.buttonCatControls.Name = "buttonCatControls";
            this.buttonCatControls.Size = new System.Drawing.Size(80, 28);
            this.buttonCatControls.Text = "Controls";
            this.buttonCatControls.Click += new System.EventHandler(this.buttonCatControls_Click);
            this.buttonCatStyling.Location = new System.Drawing.Point(264, 46);
            this.buttonCatStyling.Name = "buttonCatStyling";
            this.buttonCatStyling.Size = new System.Drawing.Size(74, 28);
            this.buttonCatStyling.Text = "Styling";
            this.buttonCatStyling.Click += new System.EventHandler(this.buttonCatStyling_Click);
            this.buttonCatDesktop.Location = new System.Drawing.Point(344, 46);
            this.buttonCatDesktop.Name = "buttonCatDesktop";
            this.buttonCatDesktop.Size = new System.Drawing.Size(96, 28);
            this.buttonCatDesktop.Text = "Desktop op";
            this.buttonCatDesktop.Click += new System.EventHandler(this.buttonCatDesktop_Click);
            this.buttonCatDeferred.Location = new System.Drawing.Point(446, 46);
            this.buttonCatDeferred.Name = "buttonCatDeferred";
            this.buttonCatDeferred.Size = new System.Drawing.Size(84, 28);
            this.buttonCatDeferred.Text = "Deferred";
            this.buttonCatDeferred.Click += new System.EventHandler(this.buttonCatDeferred_Click);
            //
            // gridErrors  (Symbol | Code | # | Category | Fix | Module | open)
            //
            this.gridErrors.AllowUserToAddRows = false;
            this.gridErrors.AllowUserToDeleteRows = false;
            this.gridErrors.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colErrSymbol, this.colErrCode, this.colErrCount, this.colErrCategory, this.colErrFix, this.colErrModule, this.colErrOpen });
            this.gridErrors.Location = new System.Drawing.Point(20, 80);
            this.gridErrors.MultiSelect = false;
            this.gridErrors.Name = "gridErrors";
            this.gridErrors.ReadOnly = true;
            this.gridErrors.RowHeadersVisible = false;
            this.gridErrors.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridErrors.Size = new System.Drawing.Size(602, 136);
            this.gridErrors.SelectionChanged += new System.EventHandler(this.gridErrors_SelectionChanged);
            this.colErrSymbol.HeaderText = "Symbol"; this.colErrSymbol.Name = "colErrSymbol"; this.colErrSymbol.Width = 220; this.colErrSymbol.ReadOnly = true;
            this.colErrCode.HeaderText = "Code"; this.colErrCode.Name = "colErrCode"; this.colErrCode.Width = 62; this.colErrCode.ReadOnly = true;
            this.colErrCount.HeaderText = "#"; this.colErrCount.Name = "colErrCount"; this.colErrCount.Width = 34; this.colErrCount.ReadOnly = true;
            this.colErrCount.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            this.colErrCategory.HeaderText = "Category"; this.colErrCategory.Name = "colErrCategory"; this.colErrCategory.Width = 150; this.colErrCategory.ReadOnly = true;
            this.colErrFix.HeaderText = "Fix"; this.colErrFix.Name = "colErrFix"; this.colErrFix.Width = 360; this.colErrFix.ReadOnly = true;
            this.colErrModule.HeaderText = "Module"; this.colErrModule.Name = "colErrModule"; this.colErrModule.Width = 72; this.colErrModule.ReadOnly = true;
            this.colErrOpen.HeaderText = "open"; this.colErrOpen.Name = "colErrOpen"; this.colErrOpen.Width = 46; this.colErrOpen.ReadOnly = true;
            //
            // labelErrorDetail  (message + fix of the selected row)
            //
            this.labelErrorDetail.AutoSize = false;
            this.labelErrorDetail.Font = new System.Drawing.Font("monospace", 9F);
            this.labelErrorDetail.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelErrorDetail.Location = new System.Drawing.Point(20, 220);
            this.labelErrorDetail.Name = "labelErrorDetail";
            this.labelErrorDetail.Size = new System.Drawing.Size(602, 30);
            this.labelErrorDetail.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // the build funnel: a Timer walks the four passes (15 → 24 → 10 → 0)
            //
            this.buttonReplayFunnel.Location = new System.Drawing.Point(20, 256);
            this.buttonReplayFunnel.Name = "buttonReplayFunnel";
            this.buttonReplayFunnel.Size = new System.Drawing.Size(200, 32);
            this.buttonReplayFunnel.Text = "▶ Replay the build funnel";
            this.buttonReplayFunnel.ToolTipText = "Categorize, fix, rebuild: four passes, ~900 ms each, driven by a Wisej.Web.Timer.";
            this.buttonReplayFunnel.Click += new System.EventHandler(this.buttonReplayFunnel_Click);
            this.labelFunnelCount.AutoSize = false;
            this.labelFunnelCount.Font = new System.Drawing.Font("default", 18F, System.Drawing.FontStyle.Bold);
            this.labelFunnelCount.ForeColor = System.Drawing.Color.FromArgb(224, 86, 59);
            this.labelFunnelCount.Location = new System.Drawing.Point(232, 250);
            this.labelFunnelCount.Name = "labelFunnelCount";
            this.labelFunnelCount.Size = new System.Drawing.Size(390, 40);
            this.labelFunnelCount.Text = "";
            this.labelFunnelCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelFunnelPass.AutoSize = false;
            this.labelFunnelPass.Font = new System.Drawing.Font("default", 9F);
            this.labelFunnelPass.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelFunnelPass.Location = new System.Drawing.Point(20, 292);
            this.labelFunnelPass.Name = "labelFunnelPass";
            this.labelFunnelPass.Size = new System.Drawing.Size(602, 24);
            this.labelFunnelPass.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.timerBuild.Interval = 900;
            this.timerBuild.Tick += new System.EventHandler(this.timerBuild_Tick);
            //
            // trace  (the migration log)
            //
            this.trace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.trace.Location = new System.Drawing.Point(690, 30);
            this.trace.Name = "trace";
            this.trace.Size = new System.Drawing.Size(628, 400);
            //
            // panelConfig  (deliverable 3: App.config → Web.config, and the ported form)
            //
            this.panelConfig.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelConfig.BackColor = System.Drawing.Color.White;
            this.panelConfig.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelConfig.Controls.Add(this.labelConfigTitle);
            this.panelConfig.Controls.Add(this.buttonLegacyConfig);
            this.panelConfig.Controls.Add(this.buttonWebConfig);
            this.panelConfig.Controls.Add(this.buttonOpenOrdersForm);
            this.panelConfig.Controls.Add(this.buttonClear);
            this.panelConfig.Controls.Add(this.labelConfigValues);
            this.panelConfig.Controls.Add(this.labelBanner);
            this.panelConfig.Location = new System.Drawing.Point(690, 444);
            this.panelConfig.Name = "panelConfig";
            this.panelConfig.Size = new System.Drawing.Size(628, 210);
            //
            // labelConfigTitle
            //
            this.labelConfigTitle.AutoSize = false;
            this.labelConfigTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelConfigTitle.Location = new System.Drawing.Point(20, 12);
            this.labelConfigTitle.Name = "labelConfigTitle";
            this.labelConfigTitle.Size = new System.Drawing.Size(590, 28);
            this.labelConfigTitle.Text = "Configuration · App.config → Web.config  ·  the ported form";
            //
            // config buttons
            //
            this.buttonLegacyConfig.Location = new System.Drawing.Point(20, 46);
            this.buttonLegacyConfig.Name = "buttonLegacyConfig";
            this.buttonLegacyConfig.Size = new System.Drawing.Size(196, 32);
            this.buttonLegacyConfig.Text = "Legacy: ConfigurationManager";
            this.buttonLegacyConfig.ToolTipText = "The desktop pattern: ConfigurationManager.ConnectionStrings[\"OrderDesk\"] — looks for <exe>.config next to the binary.";
            this.buttonLegacyConfig.Click += new System.EventHandler(this.buttonLegacyConfig_Click);
            this.buttonWebConfig.Location = new System.Drawing.Point(222, 46);
            this.buttonWebConfig.Name = "buttonWebConfig";
            this.buttonWebConfig.Size = new System.Drawing.Size(100, 32);
            this.buttonWebConfig.Text = "Web.config";
            this.buttonWebConfig.ToolTipText = "XDocument.Load(Application.StartupPath + Web.config) → connectionStrings/add[@name='OrderDesk'].";
            this.buttonWebConfig.Click += new System.EventHandler(this.buttonWebConfig_Click);
            this.buttonOpenOrdersForm.Location = new System.Drawing.Point(328, 46);
            this.buttonOpenOrdersForm.Name = "buttonOpenOrdersForm";
            this.buttonOpenOrdersForm.Size = new System.Drawing.Size(216, 32);
            this.buttonOpenOrdersForm.Text = "Open the ported OrdersForm ↗";
            this.buttonOpenOrdersForm.ToolTipText = "new OrdersForm().Show() — still a Form, floating over the page; every commented desktop call reports a boundary here.";
            this.buttonOpenOrdersForm.Click += new System.EventHandler(this.buttonOpenOrdersForm_Click);
            this.buttonClear.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.buttonClear.Location = new System.Drawing.Point(552, 46);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(58, 32);
            this.buttonClear.Text = "Clear";
            this.buttonClear.ToolTipText = "Clear the trace.";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            //
            // labelConfigValues  (what each read looked for and found)
            //
            this.labelConfigValues.AutoSize = false;
            this.labelConfigValues.Font = new System.Drawing.Font("monospace", 9F);
            this.labelConfigValues.Location = new System.Drawing.Point(20, 86);
            this.labelConfigValues.Name = "labelConfigValues";
            this.labelConfigValues.Size = new System.Drawing.Size(590, 56);
            this.labelConfigValues.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // labelBanner
            //
            this.labelBanner.AutoSize = false;
            this.labelBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelBanner.Location = new System.Drawing.Point(20, 146);
            this.labelBanner.Name = "labelBanner";
            this.labelBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelBanner.Size = new System.Drawing.Size(590, 54);
            this.labelBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelBanner.Visible = false;
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelShell);
            this.Controls.Add(this.panelErrors);
            this.Controls.Add(this.trace);
            this.Controls.Add(this.panelConfig);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1348, 684);
            this.Text = "OrderDesk — Project Conversion and Startup";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.panelShell.ResumeLayout(false);
            this.panelErrors.ResumeLayout(false);
            this.panelConfig.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelShell;
        private Wisej.Web.Label labelShellTitle;
        private Wisej.Web.Label labelStatus;
        private Wisej.Web.DataGridView gridShell;
        private Wisej.Web.DataGridViewTextBoxColumn colShellFile;
        private Wisej.Web.DataGridViewTextBoxColumn colShellReplaces;
        private Wisej.Web.DataGridViewTextBoxColumn colShellRole;
        private Wisej.Web.Label labelShellPreview;
        private Wisej.Web.Panel panelErrors;
        private Wisej.Web.Label labelErrorsTitle;
        private Wisej.Web.Label labelErrorsCount;
        private Wisej.Web.Button buttonCatAll;
        private Wisej.Web.Button buttonCatNamespace;
        private Wisej.Web.Button buttonCatControls;
        private Wisej.Web.Button buttonCatStyling;
        private Wisej.Web.Button buttonCatDesktop;
        private Wisej.Web.Button buttonCatDeferred;
        private Wisej.Web.DataGridView gridErrors;
        private Wisej.Web.DataGridViewTextBoxColumn colErrSymbol;
        private Wisej.Web.DataGridViewTextBoxColumn colErrCode;
        private Wisej.Web.DataGridViewTextBoxColumn colErrCount;
        private Wisej.Web.DataGridViewTextBoxColumn colErrCategory;
        private Wisej.Web.DataGridViewTextBoxColumn colErrFix;
        private Wisej.Web.DataGridViewTextBoxColumn colErrModule;
        private Wisej.Web.DataGridViewTextBoxColumn colErrOpen;
        private Wisej.Web.Label labelErrorDetail;
        private Wisej.Web.Button buttonReplayFunnel;
        private Wisej.Web.Label labelFunnelCount;
        private Wisej.Web.Label labelFunnelPass;
        private Wisej.Web.Timer timerBuild;
        private OrderDesk.Views.TracePanel trace;
        private Wisej.Web.Panel panelConfig;
        private Wisej.Web.Label labelConfigTitle;
        private Wisej.Web.Button buttonLegacyConfig;
        private Wisej.Web.Button buttonWebConfig;
        private Wisej.Web.Button buttonOpenOrdersForm;
        private Wisej.Web.Button buttonClear;
        private Wisej.Web.Label labelConfigValues;
        private Wisej.Web.Label labelBanner;
    }
}
