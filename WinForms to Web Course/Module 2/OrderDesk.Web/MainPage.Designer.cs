namespace OrderDesk
{
    partial class MainPage
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

        // Layout for the reviewer's 1400×760 pane: app bar 44 · status/banner row · OrdersPage card
        // (790×330) · Shell anatomy (330×228) + Compiler-error log (450×228) · button bar at y 658;
        // the TracePanel is docked right, 560 wide.
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.appBar = new Wisej.Web.Panel();
            this.labelAppTitle = new Wisej.Web.Label();
            this.labelSession = new Wisej.Web.Label();
            this.labelStatus = new Wisej.Web.Label();
            this.labelBanner = new Wisej.Web.Label();
            this.cardOrders = new Wisej.Web.Panel();
            this.labelOrdersTitle = new Wisej.Web.Label();
            this.panelOrdersHost = new Wisej.Web.Panel();
            this.labelOrdersPlaceholder = new Wisej.Web.Label();
            this.cardShell = new Wisej.Web.Panel();
            this.labelShellTitle = new Wisej.Web.Label();
            this.panelShellRows = new Wisej.Web.Panel();
            this.cardErrors = new Wisej.Web.Panel();
            this.labelErrorsTitle = new Wisej.Web.Label();
            this.labelErrorsCount = new Wisej.Web.Label();
            this.gridErrors = new Wisej.Web.DataGridView();
            this.colCategory = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colExample = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colFix = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCount = new Wisej.Web.DataGridViewTextBoxColumn();
            this.panelButtons = new Wisej.Web.Panel();
            this.buttonOpen = new Wisej.Web.Button();
            this.buttonReplay = new Wisej.Web.Button();
            this.buttonAppConfig = new Wisej.Web.Button();
            this.buttonWebConfig = new Wisej.Web.Button();
            this.trace = new OrderDesk.Shared.TracePanel();
            this.timerReplay = new Wisej.Web.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.gridErrors)).BeginInit();
            this.appBar.SuspendLayout();
            this.cardOrders.SuspendLayout();
            this.panelOrdersHost.SuspendLayout();
            this.cardShell.SuspendLayout();
            this.cardErrors.SuspendLayout();
            this.labelErrorsTitle.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            //
            // appBar
            //
            this.appBar.BackColor = OrderDesk.Shared.Palette.Accent;
            this.appBar.Controls.Add(this.labelAppTitle);
            this.appBar.Controls.Add(this.labelSession);
            this.appBar.Dock = Wisej.Web.DockStyle.Top;
            this.appBar.Name = "appBar";
            this.appBar.Size = new System.Drawing.Size(1400, 44);
            //
            // labelAppTitle
            //
            this.labelAppTitle.AutoSize = false;
            this.labelAppTitle.Font = new System.Drawing.Font("default", 13F, System.Drawing.FontStyle.Bold);
            this.labelAppTitle.ForeColor = System.Drawing.Color.White;
            this.labelAppTitle.Location = new System.Drawing.Point(16, 0);
            this.labelAppTitle.Name = "labelAppTitle";
            this.labelAppTitle.Size = new System.Drawing.Size(760, 44);
            this.labelAppTitle.Text = "OrderDesk — Module 2 · Project conversion: the Wisej.NET shell";
            this.labelAppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelSession
            //
            this.labelSession.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.labelSession.AutoSize = false;
            this.labelSession.Font = new System.Drawing.Font("monospace", 9F);
            this.labelSession.ForeColor = System.Drawing.Color.White;
            this.labelSession.Location = new System.Drawing.Point(964, 0);
            this.labelSession.Name = "labelSession";
            this.labelSession.Size = new System.Drawing.Size(420, 44);
            this.labelSession.Text = "session …";
            this.labelSession.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // labelStatus
            //
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = OrderDesk.Shared.Palette.MutedText;
            this.labelStatus.Location = new System.Drawing.Point(16, 50);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(150, 28);
            this.labelStatus.Text = "● idle";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelBanner
            //
            this.labelBanner.AutoEllipsis = true;
            this.labelBanner.AutoSize = false;
            this.labelBanner.BackColor = OrderDesk.Shared.Palette.GoodSoft;
            this.labelBanner.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.labelBanner.Font = new System.Drawing.Font("default", 9F);
            this.labelBanner.ForeColor = OrderDesk.Shared.Palette.Good;
            this.labelBanner.Location = new System.Drawing.Point(172, 50);
            this.labelBanner.Name = "labelBanner";
            this.labelBanner.Padding = new Wisej.Web.Padding(10, 0, 10, 0);
            this.labelBanner.Size = new System.Drawing.Size(634, 28);
            this.labelBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelBanner.Visible = false;
            //
            // cardOrders  (the ported form's host)
            //
            this.cardOrders.BackColor = OrderDesk.Shared.Palette.CardBackground;
            this.cardOrders.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardOrders.Controls.Add(this.panelOrdersHost);
            this.cardOrders.Controls.Add(this.labelOrdersTitle);
            this.cardOrders.Location = new System.Drawing.Point(16, 84);
            this.cardOrders.Name = "cardOrders";
            this.cardOrders.Size = new System.Drawing.Size(790, 330);
            //
            // labelOrdersTitle
            //
            this.labelOrdersTitle.AutoSize = false;
            this.labelOrdersTitle.Dock = Wisej.Web.DockStyle.Top;
            this.labelOrdersTitle.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Bold);
            this.labelOrdersTitle.ForeColor = OrderDesk.Shared.Palette.Ink;
            this.labelOrdersTitle.Name = "labelOrdersTitle";
            this.labelOrdersTitle.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelOrdersTitle.Size = new System.Drawing.Size(788, 26);
            this.labelOrdersTitle.Text = "OrdersPage — LegacyOrderDesk.OrdersForm ported  ·  Pages/OrdersPage.cs + OrdersPage.Designer.cs  ·  Wisej.Web.UserControl";
            this.labelOrdersTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // panelOrdersHost
            //
            this.panelOrdersHost.BackColor = OrderDesk.Shared.Palette.PanelBackground;
            this.panelOrdersHost.Controls.Add(this.labelOrdersPlaceholder);
            this.panelOrdersHost.Dock = Wisej.Web.DockStyle.Fill;
            this.panelOrdersHost.Name = "panelOrdersHost";
            //
            // labelOrdersPlaceholder
            //
            this.labelOrdersPlaceholder.AutoSize = false;
            this.labelOrdersPlaceholder.Dock = Wisej.Web.DockStyle.Fill;
            this.labelOrdersPlaceholder.Font = new System.Drawing.Font("default", 10F);
            this.labelOrdersPlaceholder.ForeColor = OrderDesk.Shared.Palette.MutedText;
            this.labelOrdersPlaceholder.Name = "labelOrdersPlaceholder";
            this.labelOrdersPlaceholder.Text = "The ported form is not loaded yet — click  Open OrdersPage ✓  (or  Replay conversion  to watch the port first).";
            this.labelOrdersPlaceholder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // cardShell
            //
            this.cardShell.BackColor = OrderDesk.Shared.Palette.CardBackground;
            this.cardShell.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardShell.Controls.Add(this.panelShellRows);
            this.cardShell.Controls.Add(this.labelShellTitle);
            this.cardShell.Location = new System.Drawing.Point(16, 422);
            this.cardShell.Name = "cardShell";
            this.cardShell.Size = new System.Drawing.Size(330, 228);
            //
            // labelShellTitle
            //
            this.labelShellTitle.AutoSize = false;
            this.labelShellTitle.Dock = Wisej.Web.DockStyle.Top;
            this.labelShellTitle.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Bold);
            this.labelShellTitle.ForeColor = OrderDesk.Shared.Palette.Ink;
            this.labelShellTitle.Name = "labelShellTitle";
            this.labelShellTitle.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelShellTitle.Size = new System.Drawing.Size(328, 26);
            this.labelShellTitle.Text = "Shell anatomy  ·  live values";
            this.labelShellTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // panelShellRows  (rows are created in MainPage.BuildShellRows)
            //
            this.panelShellRows.Dock = Wisej.Web.DockStyle.Fill;
            this.panelShellRows.Name = "panelShellRows";
            //
            // cardErrors
            //
            this.cardErrors.BackColor = OrderDesk.Shared.Palette.CardBackground;
            this.cardErrors.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardErrors.Controls.Add(this.gridErrors);
            this.cardErrors.Controls.Add(this.labelErrorsTitle);
            this.cardErrors.Location = new System.Drawing.Point(356, 422);
            this.cardErrors.Name = "cardErrors";
            this.cardErrors.Size = new System.Drawing.Size(450, 228);
            //
            // labelErrorsTitle
            //
            this.labelErrorsTitle.AutoSize = false;
            this.labelErrorsTitle.Controls.Add(this.labelErrorsCount);
            this.labelErrorsTitle.Dock = Wisej.Web.DockStyle.Top;
            this.labelErrorsTitle.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Bold);
            this.labelErrorsTitle.ForeColor = OrderDesk.Shared.Palette.Ink;
            this.labelErrorsTitle.Name = "labelErrorsTitle";
            this.labelErrorsTitle.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelErrorsTitle.Size = new System.Drawing.Size(448, 26);
            this.labelErrorsTitle.Text = "Compiler-error log  ·  first build of the port";
            this.labelErrorsTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelErrorsCount
            //
            this.labelErrorsCount.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.labelErrorsCount.AutoSize = false;
            this.labelErrorsCount.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.labelErrorsCount.ForeColor = OrderDesk.Shared.Palette.MutedText;
            this.labelErrorsCount.Location = new System.Drawing.Point(308, 0);
            this.labelErrorsCount.Name = "labelErrorsCount";
            this.labelErrorsCount.Size = new System.Drawing.Size(128, 26);
            this.labelErrorsCount.Text = "not built yet";
            this.labelErrorsCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // gridErrors
            //
            this.gridErrors.AllowUserToAddRows = false;
            this.gridErrors.AllowUserToDeleteRows = false;
            this.gridErrors.AutoGenerateColumns = false;
            this.gridErrors.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colCategory, this.colExample, this.colFix, this.colCount });
            this.gridErrors.Dock = Wisej.Web.DockStyle.Fill;
            this.gridErrors.Font = new System.Drawing.Font("default", 8.5F);
            this.gridErrors.MultiSelect = false;
            this.gridErrors.Name = "gridErrors";
            this.gridErrors.ReadOnly = true;
            this.gridErrors.RowHeadersVisible = false;
            this.gridErrors.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.colCategory.HeaderText = "Category";
            this.colCategory.Width = 108;
            this.colExample.HeaderText = "Example";
            this.colExample.Width = 128;
            this.colFix.HeaderText = "Fix";
            this.colFix.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
            this.colCount.HeaderText = "Count";
            this.colCount.Width = 52;
            this.colCount.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            //
            // panelButtons  (bottom button bar, y ≤ 700)
            //
            this.panelButtons.BackColor = OrderDesk.Shared.Palette.PageBackground;
            this.panelButtons.Controls.Add(this.buttonOpen);
            this.panelButtons.Controls.Add(this.buttonReplay);
            this.panelButtons.Controls.Add(this.buttonAppConfig);
            this.panelButtons.Controls.Add(this.buttonWebConfig);
            this.panelButtons.Location = new System.Drawing.Point(16, 658);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(790, 36);
            //
            // buttonOpen  (success path)
            //
            this.buttonOpen.Location = new System.Drawing.Point(0, 0);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(180, 34);
            this.buttonOpen.Text = "Open OrdersPage ✓";
            this.buttonOpen.ToolTipText = "Load the ported OrdersForm (Pages/OrdersPage) with the 5,000 orders of the same OrderService";
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            //
            // buttonReplay  (progress path)
            //
            this.buttonReplay.Location = new System.Drawing.Point(190, 0);
            this.buttonReplay.Name = "buttonReplay";
            this.buttonReplay.Size = new System.Drawing.Size(170, 34);
            this.buttonReplay.Text = "Replay conversion";
            this.buttonReplay.ToolTipText = "Timer: branch → new Wisej.NET project → copy OrdersForm → namespace swap → build 14 errors → classify → 6 → 0 → run";
            this.buttonReplay.Click += new System.EventHandler(this.buttonReplay_Click);
            //
            // buttonAppConfig  (failure path)
            //
            this.buttonAppConfig.Location = new System.Drawing.Point(370, 0);
            this.buttonAppConfig.Name = "buttonAppConfig";
            this.buttonAppConfig.Size = new System.Drawing.Size(190, 34);
            this.buttonAppConfig.Text = "App.config lookup ✕";
            this.buttonAppConfig.ToolTipText = "Legacy/AppConfig: ConfigurationManager-style lookup of <exe>.config next to the server assembly";
            this.buttonAppConfig.Click += new System.EventHandler(this.buttonAppConfig_Click);
            //
            // buttonWebConfig  (recovery)
            //
            this.buttonWebConfig.Location = new System.Drawing.Point(570, 0);
            this.buttonWebConfig.Name = "buttonWebConfig";
            this.buttonWebConfig.Size = new System.Drawing.Size(190, 34);
            this.buttonWebConfig.Text = "Web.config lookup ✓";
            this.buttonWebConfig.ToolTipText = "Services/AppConfig: Web.config appSettings + connectionStrings via System.Xml.Linq";
            this.buttonWebConfig.Click += new System.EventHandler(this.buttonWebConfig_Click);
            //
            // trace
            //
            this.trace.Dock = Wisej.Web.DockStyle.Right;
            this.trace.Name = "trace";
            this.trace.Size = new System.Drawing.Size(560, 716);
            //
            // timerReplay
            //
            this.timerReplay.Interval = 650;
            this.timerReplay.Tick += new System.EventHandler(this.timerReplay_Tick);
            //
            // MainPage
            //
            this.BackColor = OrderDesk.Shared.Palette.PageBackground;
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.labelBanner);
            this.Controls.Add(this.cardOrders);
            this.Controls.Add(this.cardShell);
            this.Controls.Add(this.cardErrors);
            this.Controls.Add(this.panelButtons);
            this.Controls.Add(this.trace);
            this.Controls.Add(this.appBar);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1400, 760);
            this.Text = "OrderDesk — Module 2";
            this.Load += new System.EventHandler(this.MainPage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridErrors)).EndInit();
            this.appBar.ResumeLayout(false);
            this.cardOrders.ResumeLayout(false);
            this.panelOrdersHost.ResumeLayout(false);
            this.cardShell.ResumeLayout(false);
            this.cardErrors.ResumeLayout(false);
            this.labelErrorsTitle.ResumeLayout(false);
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel appBar;
        private Wisej.Web.Label labelAppTitle;
        private Wisej.Web.Label labelSession;
        private Wisej.Web.Label labelStatus;
        private Wisej.Web.Label labelBanner;
        private Wisej.Web.Panel cardOrders;
        private Wisej.Web.Label labelOrdersTitle;
        private Wisej.Web.Panel panelOrdersHost;
        private Wisej.Web.Label labelOrdersPlaceholder;
        private Wisej.Web.Panel cardShell;
        private Wisej.Web.Label labelShellTitle;
        private Wisej.Web.Panel panelShellRows;
        private Wisej.Web.Panel cardErrors;
        private Wisej.Web.Label labelErrorsTitle;
        private Wisej.Web.Label labelErrorsCount;
        private Wisej.Web.DataGridView gridErrors;
        private Wisej.Web.DataGridViewTextBoxColumn colCategory;
        private Wisej.Web.DataGridViewTextBoxColumn colExample;
        private Wisej.Web.DataGridViewTextBoxColumn colFix;
        private Wisej.Web.DataGridViewTextBoxColumn colCount;
        private Wisej.Web.Panel panelButtons;
        private Wisej.Web.Button buttonOpen;
        private Wisej.Web.Button buttonReplay;
        private Wisej.Web.Button buttonAppConfig;
        private Wisej.Web.Button buttonWebConfig;
        private OrderDesk.Shared.TracePanel trace;
        private Wisej.Web.Timer timerReplay;
    }
}
