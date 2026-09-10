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

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelAppBar = new Wisej.Web.Panel();
            this.labelAppTitle = new Wisej.Web.Label();
            this.trace = new OrderDesk.Shared.TracePanel();
            this.panelWorkbook = new Wisej.Web.Panel();
            this.labelWorkbookTitle = new Wisej.Web.Label();
            this.labelStatus = new Wisej.Web.Label();
            this.gridWorkbook = new Wisej.Web.DataGridView();
            this.colFeature = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDependency = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTag = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colVerdict = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colEffort = new Wisej.Web.DataGridViewTextBoxColumn();
            this.panelSlice = new Wisej.Web.Panel();
            this.labelSliceTitle = new Wisej.Web.Label();
            this.labelSliceStartup = new Wisej.Web.Label();
            this.labelSliceUser = new Wisej.Web.Label();
            this.labelSliceScreen = new Wisej.Web.Label();
            this.labelSliceDialog = new Wisej.Web.Label();
            this.labelSliceGrid = new Wisej.Web.Label();
            this.labelSliceFile = new Wisej.Web.Label();
            this.labelSliceBoundary = new Wisej.Web.Label();
            this.panelOrders = new Wisej.Web.Panel();
            this.labelOrdersTitle = new Wisej.Web.Label();
            this.labelOrdersHint = new Wisej.Web.Label();
            this.gridOrders = new Wisej.Web.DataGridView();
            this.colOrderId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colOrderCustomer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colOrderTotal = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colOrderStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.labelBanner = new Wisej.Web.Label();
            this.buttonRunSlice = new Wisej.Web.Button();
            this.buttonSecondSession = new Wisej.Web.Button();
            this.buttonExportDesktop = new Wisej.Web.Button();
            this.buttonExportDownload = new Wisej.Web.Button();
            this.buttonReplay = new Wisej.Web.Button();
            this.timerReplay = new Wisej.Web.Timer(this.components);
            this.panelAppBar.SuspendLayout();
            this.panelWorkbook.SuspendLayout();
            this.panelSlice.SuspendLayout();
            this.panelOrders.SuspendLayout();
            this.SuspendLayout();
            //
            // panelAppBar  (the blue OrderDesk app bar)
            //
            this.panelAppBar.BackColor = OrderDesk.Shared.Palette.Accent;
            this.panelAppBar.Controls.Add(this.labelAppTitle);
            this.panelAppBar.Dock = Wisej.Web.DockStyle.Top;
            this.panelAppBar.Name = "panelAppBar";
            this.panelAppBar.Size = new System.Drawing.Size(1400, 44);
            //
            // labelAppTitle
            //
            this.labelAppTitle.AutoSize = false;
            this.labelAppTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.labelAppTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelAppTitle.ForeColor = System.Drawing.Color.White;
            this.labelAppTitle.Name = "labelAppTitle";
            this.labelAppTitle.Padding = new Wisej.Web.Padding(20, 0, 0, 0);
            this.labelAppTitle.Text = "OrderDesk — Module 1 · Migration discovery && the first slice";
            this.labelAppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // trace  (Server ⇄ Client migration trace, docked right)
            //
            this.trace.Dock = Wisej.Web.DockStyle.Right;
            this.trace.Name = "trace";
            this.trace.Size = new System.Drawing.Size(560, 716);
            //
            // panelWorkbook  (the assessment workbook card)
            //
            this.panelWorkbook.BackColor = OrderDesk.Shared.Palette.CardBackground;
            this.panelWorkbook.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelWorkbook.Controls.Add(this.labelWorkbookTitle);
            this.panelWorkbook.Controls.Add(this.labelStatus);
            this.panelWorkbook.Controls.Add(this.gridWorkbook);
            this.panelWorkbook.Location = new System.Drawing.Point(20, 58);
            this.panelWorkbook.Name = "panelWorkbook";
            this.panelWorkbook.Size = new System.Drawing.Size(800, 336);
            //
            // labelWorkbookTitle
            //
            this.labelWorkbookTitle.AutoSize = false;
            this.labelWorkbookTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelWorkbookTitle.ForeColor = OrderDesk.Shared.Palette.Ink;
            this.labelWorkbookTitle.Location = new System.Drawing.Point(20, 10);
            this.labelWorkbookTitle.Name = "labelWorkbookTitle";
            this.labelWorkbookTitle.Size = new System.Drawing.Size(560, 26);
            this.labelWorkbookTitle.Text = "Assessment workbook · LegacyOrderDesk";
            this.labelWorkbookTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelStatus  (● idle / working / alarm)
            //
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = OrderDesk.Shared.Palette.Good;
            this.labelStatus.Location = new System.Drawing.Point(620, 12);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(160, 22);
            this.labelStatus.Text = "● idle";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // gridWorkbook  (Form / feature · Dependency · Risk tag · Verdict · Effort)
            //
            this.gridWorkbook.AllowUserToAddRows = false;
            this.gridWorkbook.AllowUserToDeleteRows = false;
            this.gridWorkbook.AllowUserToResizeRows = false;
            this.gridWorkbook.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.gridWorkbook.BorderStyle = Wisej.Web.BorderStyle.None;
            this.gridWorkbook.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colFeature, this.colDependency, this.colTag, this.colVerdict, this.colEffort });
            this.gridWorkbook.Location = new System.Drawing.Point(0, 44);
            this.gridWorkbook.MultiSelect = false;
            this.gridWorkbook.Name = "gridWorkbook";
            this.gridWorkbook.ReadOnly = true;
            this.gridWorkbook.RowHeadersVisible = false;
            this.gridWorkbook.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridWorkbook.Size = new System.Drawing.Size(798, 290);
            this.colFeature.HeaderText = "Form / feature";
            this.colFeature.Name = "colFeature";
            this.colFeature.ReadOnly = true;
            this.colFeature.Width = 240;
            this.colDependency.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
            this.colDependency.HeaderText = "Dependency";
            this.colDependency.Name = "colDependency";
            this.colDependency.ReadOnly = true;
            this.colTag.HeaderText = "Risk tag";
            this.colTag.Name = "colTag";
            this.colTag.ReadOnly = true;
            this.colTag.Width = 110;
            this.colVerdict.HeaderText = "Verdict";
            this.colVerdict.Name = "colVerdict";
            this.colVerdict.ReadOnly = true;
            this.colVerdict.Width = 90;
            this.colEffort.HeaderText = "Effort";
            this.colEffort.Name = "colEffort";
            this.colEffort.ReadOnly = true;
            this.colEffort.Width = 60;
            this.colEffort.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleCenter;
            //
            // panelSlice  (the first vertical slice checklist)
            //
            this.panelSlice.BackColor = OrderDesk.Shared.Palette.CardBackground;
            this.panelSlice.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelSlice.Controls.Add(this.labelSliceTitle);
            this.panelSlice.Controls.Add(this.labelSliceStartup);
            this.panelSlice.Controls.Add(this.labelSliceUser);
            this.panelSlice.Controls.Add(this.labelSliceScreen);
            this.panelSlice.Controls.Add(this.labelSliceDialog);
            this.panelSlice.Controls.Add(this.labelSliceGrid);
            this.panelSlice.Controls.Add(this.labelSliceFile);
            this.panelSlice.Controls.Add(this.labelSliceBoundary);
            this.panelSlice.Location = new System.Drawing.Point(20, 404);
            this.panelSlice.Name = "panelSlice";
            this.panelSlice.Size = new System.Drawing.Size(390, 212);
            //
            // labelSliceTitle
            //
            this.labelSliceTitle.AutoSize = false;
            this.labelSliceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelSliceTitle.ForeColor = OrderDesk.Shared.Palette.Ink;
            this.labelSliceTitle.Location = new System.Drawing.Point(20, 10);
            this.labelSliceTitle.Name = "labelSliceTitle";
            this.labelSliceTitle.Size = new System.Drawing.Size(350, 24);
            this.labelSliceTitle.Text = "First vertical slice · small but honest";
            this.labelSliceTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelSliceStartup … labelSliceBoundary  (○ → ✓ as the buttons run)
            //
            this.labelSliceStartup.AutoSize = false;
            this.labelSliceStartup.Font = new System.Drawing.Font("default", 10F);
            this.labelSliceStartup.ForeColor = OrderDesk.Shared.Palette.MutedText;
            this.labelSliceStartup.Location = new System.Drawing.Point(20, 40);
            this.labelSliceStartup.Name = "labelSliceStartup";
            this.labelSliceStartup.Size = new System.Drawing.Size(350, 22);
            this.labelSliceStartup.Text = "○  Startup";
            this.labelSliceStartup.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelSliceUser.AutoSize = false;
            this.labelSliceUser.Font = new System.Drawing.Font("default", 10F);
            this.labelSliceUser.ForeColor = OrderDesk.Shared.Palette.MutedText;
            this.labelSliceUser.Location = new System.Drawing.Point(20, 64);
            this.labelSliceUser.Name = "labelSliceUser";
            this.labelSliceUser.Size = new System.Drawing.Size(350, 22);
            this.labelSliceUser.Text = "○  User context / login";
            this.labelSliceUser.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelSliceScreen.AutoSize = false;
            this.labelSliceScreen.Font = new System.Drawing.Font("default", 10F);
            this.labelSliceScreen.ForeColor = OrderDesk.Shared.Palette.MutedText;
            this.labelSliceScreen.Location = new System.Drawing.Point(20, 88);
            this.labelSliceScreen.Name = "labelSliceScreen";
            this.labelSliceScreen.Size = new System.Drawing.Size(350, 22);
            this.labelSliceScreen.Text = "○  One read-only screen";
            this.labelSliceScreen.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelSliceDialog.AutoSize = false;
            this.labelSliceDialog.Font = new System.Drawing.Font("default", 10F);
            this.labelSliceDialog.ForeColor = OrderDesk.Shared.Palette.MutedText;
            this.labelSliceDialog.Location = new System.Drawing.Point(20, 112);
            this.labelSliceDialog.Name = "labelSliceDialog";
            this.labelSliceDialog.Size = new System.Drawing.Size(350, 22);
            this.labelSliceDialog.Text = "○  One edit dialog — EditOrderDialog (ported in Module 3)";
            this.labelSliceDialog.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelSliceGrid.AutoSize = false;
            this.labelSliceGrid.Font = new System.Drawing.Font("default", 10F);
            this.labelSliceGrid.ForeColor = OrderDesk.Shared.Palette.MutedText;
            this.labelSliceGrid.Location = new System.Drawing.Point(20, 136);
            this.labelSliceGrid.Name = "labelSliceGrid";
            this.labelSliceGrid.Size = new System.Drawing.Size(350, 22);
            this.labelSliceGrid.Text = "○  One grid";
            this.labelSliceGrid.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelSliceFile.AutoSize = false;
            this.labelSliceFile.Font = new System.Drawing.Font("default", 10F);
            this.labelSliceFile.ForeColor = OrderDesk.Shared.Palette.MutedText;
            this.labelSliceFile.Location = new System.Drawing.Point(20, 160);
            this.labelSliceFile.Name = "labelSliceFile";
            this.labelSliceFile.Size = new System.Drawing.Size(350, 22);
            this.labelSliceFile.Text = "○  One file / report";
            this.labelSliceFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelSliceBoundary.AutoSize = false;
            this.labelSliceBoundary.Font = new System.Drawing.Font("default", 10F);
            this.labelSliceBoundary.ForeColor = OrderDesk.Shared.Palette.MutedText;
            this.labelSliceBoundary.Location = new System.Drawing.Point(20, 184);
            this.labelSliceBoundary.Name = "labelSliceBoundary";
            this.labelSliceBoundary.Size = new System.Drawing.Size(350, 22);
            this.labelSliceBoundary.Text = "○  One deployment boundary";
            this.labelSliceBoundary.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // panelOrders  (the read-only orders grid of the first slice)
            //
            this.panelOrders.BackColor = OrderDesk.Shared.Palette.CardBackground;
            this.panelOrders.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelOrders.Controls.Add(this.labelOrdersTitle);
            this.panelOrders.Controls.Add(this.labelOrdersHint);
            this.panelOrders.Controls.Add(this.gridOrders);
            this.panelOrders.Location = new System.Drawing.Point(420, 404);
            this.panelOrders.Name = "panelOrders";
            this.panelOrders.Size = new System.Drawing.Size(400, 212);
            //
            // labelOrdersTitle
            //
            this.labelOrdersTitle.AutoSize = false;
            this.labelOrdersTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelOrdersTitle.ForeColor = OrderDesk.Shared.Palette.Ink;
            this.labelOrdersTitle.Location = new System.Drawing.Point(20, 10);
            this.labelOrdersTitle.Name = "labelOrdersTitle";
            this.labelOrdersTitle.Size = new System.Drawing.Size(360, 24);
            this.labelOrdersTitle.Text = "Orders · read-only (first slice)";
            this.labelOrdersTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelOrdersHint
            //
            this.labelOrdersHint.AutoSize = false;
            this.labelOrdersHint.Font = new System.Drawing.Font("default", 10F);
            this.labelOrdersHint.ForeColor = OrderDesk.Shared.Palette.MutedText;
            this.labelOrdersHint.Location = new System.Drawing.Point(20, 44);
            this.labelOrdersHint.Name = "labelOrdersHint";
            this.labelOrdersHint.Size = new System.Drawing.Size(360, 80);
            this.labelOrdersHint.Text = "Run first slice ✓ calls OrderService.GetAll() — the same call OrdersForm.ReloadGrid() made — and shows the first five orders here.";
            this.labelOrdersHint.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // gridOrders  (Order · Customer · Total · Status)
            //
            this.gridOrders.AllowUserToAddRows = false;
            this.gridOrders.AllowUserToDeleteRows = false;
            this.gridOrders.AllowUserToResizeRows = false;
            this.gridOrders.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.gridOrders.BorderStyle = Wisej.Web.BorderStyle.None;
            this.gridOrders.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colOrderId, this.colOrderCustomer, this.colOrderTotal, this.colOrderStatus });
            this.gridOrders.Location = new System.Drawing.Point(0, 40);
            this.gridOrders.MultiSelect = false;
            this.gridOrders.Name = "gridOrders";
            this.gridOrders.ReadOnly = true;
            this.gridOrders.RowHeadersVisible = false;
            this.gridOrders.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridOrders.Size = new System.Drawing.Size(398, 170);
            this.gridOrders.Visible = false;
            this.colOrderId.HeaderText = "Order";
            this.colOrderId.Name = "colOrderId";
            this.colOrderId.ReadOnly = true;
            this.colOrderId.Width = 70;
            this.colOrderCustomer.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
            this.colOrderCustomer.HeaderText = "Customer";
            this.colOrderCustomer.Name = "colOrderCustomer";
            this.colOrderCustomer.ReadOnly = true;
            this.colOrderTotal.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            this.colOrderTotal.DefaultCellStyle.Format = "C2";
            this.colOrderTotal.HeaderText = "Total";
            this.colOrderTotal.Name = "colOrderTotal";
            this.colOrderTotal.ReadOnly = true;
            this.colOrderTotal.Width = 100;
            this.colOrderStatus.HeaderText = "Status";
            this.colOrderStatus.Name = "colOrderStatus";
            this.colOrderStatus.ReadOnly = true;
            this.colOrderStatus.Width = 90;
            //
            // labelBanner  (finding / alarm banner — hidden until something happens)
            //
            this.labelBanner.AutoSize = false;
            this.labelBanner.BackColor = OrderDesk.Shared.Palette.BadSoft;
            this.labelBanner.Font = new System.Drawing.Font("default", 10F);
            this.labelBanner.ForeColor = OrderDesk.Shared.Palette.Bad;
            this.labelBanner.Location = new System.Drawing.Point(20, 626);
            this.labelBanner.Name = "labelBanner";
            this.labelBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelBanner.Size = new System.Drawing.Size(800, 30);
            this.labelBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelBanner.Visible = false;
            //
            // buttonRunSlice  (success path)
            //
            this.buttonRunSlice.Location = new System.Drawing.Point(20, 664);
            this.buttonRunSlice.Name = "buttonRunSlice";
            this.buttonRunSlice.Size = new System.Drawing.Size(150, 30);
            this.buttonRunSlice.Text = "Run first slice ✓";
            this.buttonRunSlice.ToolTipText = "Program.Main → OrderService.GetAll() → a read-only orders grid";
            this.buttonRunSlice.Click += new System.EventHandler(this.buttonRunSlice_Click);
            //
            // buttonSecondSession  (finding: static state is process-wide)
            //
            this.buttonSecondSession.Location = new System.Drawing.Point(180, 664);
            this.buttonSecondSession.Name = "buttonSecondSession";
            this.buttonSecondSession.Size = new System.Drawing.Size(130, 30);
            this.buttonSecondSession.Text = "Second session";
            this.buttonSecondSession.ToolTipText = "Application.SessionCount + the copied AppState.CurrentUser static set to kelly";
            this.buttonSecondSession.Click += new System.EventHandler(this.buttonSecondSession_Click);
            //
            // buttonExportDesktop  (failure path)
            //
            this.buttonExportDesktop.Location = new System.Drawing.Point(320, 664);
            this.buttonExportDesktop.Name = "buttonExportDesktop";
            this.buttonExportDesktop.Size = new System.Drawing.Size(170, 30);
            this.buttonExportDesktop.Text = "Export (desktop way) ✕";
            this.buttonExportDesktop.ToolTipText = "Excel Interop → COMException; LocalExport → C:\\Orders on the SERVER";
            this.buttonExportDesktop.Click += new System.EventHandler(this.buttonExportDesktop_Click);
            //
            // buttonExportDownload  (recovery)
            //
            this.buttonExportDownload.Location = new System.Drawing.Point(500, 664);
            this.buttonExportDownload.Name = "buttonExportDownload";
            this.buttonExportDownload.Size = new System.Drawing.Size(160, 30);
            this.buttonExportDownload.Text = "Export (Download) ✓";
            this.buttonExportDownload.ToolTipText = "LocalExport.ToCsv reused → Application.Download(stream, \"orders.csv\")";
            this.buttonExportDownload.Click += new System.EventHandler(this.buttonExportDownload_Click);
            //
            // buttonReplay  (progress path: Timer)
            //
            this.buttonReplay.Location = new System.Drawing.Point(670, 664);
            this.buttonReplay.Name = "buttonReplay";
            this.buttonReplay.Size = new System.Drawing.Size(150, 30);
            this.buttonReplay.Text = "Replay assessment";
            this.buttonReplay.ToolTipText = "A Wisej.Web.Timer reveals the workbook one finding at a time";
            this.buttonReplay.Click += new System.EventHandler(this.buttonReplay_Click);
            //
            // timerReplay
            //
            this.timerReplay.Interval = 450;
            this.timerReplay.Tick += new System.EventHandler(this.timerReplay_Tick);
            //
            // MainPage
            //
            this.BackColor = OrderDesk.Shared.Palette.PageBackground;
            this.Controls.Add(this.panelWorkbook);
            this.Controls.Add(this.panelSlice);
            this.Controls.Add(this.panelOrders);
            this.Controls.Add(this.labelBanner);
            this.Controls.Add(this.buttonRunSlice);
            this.Controls.Add(this.buttonSecondSession);
            this.Controls.Add(this.buttonExportDesktop);
            this.Controls.Add(this.buttonExportDownload);
            this.Controls.Add(this.buttonReplay);
            this.Controls.Add(this.trace);
            this.Controls.Add(this.panelAppBar);
            this.Name = "MainPage";
            this.Text = "OrderDesk — Module 1";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.panelAppBar.ResumeLayout(false);
            this.panelWorkbook.ResumeLayout(false);
            this.panelSlice.ResumeLayout(false);
            this.panelOrders.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelAppBar;
        private Wisej.Web.Label labelAppTitle;
        private OrderDesk.Shared.TracePanel trace;
        private Wisej.Web.Panel panelWorkbook;
        private Wisej.Web.Label labelWorkbookTitle;
        private Wisej.Web.Label labelStatus;
        private Wisej.Web.DataGridView gridWorkbook;
        private Wisej.Web.DataGridViewTextBoxColumn colFeature;
        private Wisej.Web.DataGridViewTextBoxColumn colDependency;
        private Wisej.Web.DataGridViewTextBoxColumn colTag;
        private Wisej.Web.DataGridViewTextBoxColumn colVerdict;
        private Wisej.Web.DataGridViewTextBoxColumn colEffort;
        private Wisej.Web.Panel panelSlice;
        private Wisej.Web.Label labelSliceTitle;
        private Wisej.Web.Label labelSliceStartup;
        private Wisej.Web.Label labelSliceUser;
        private Wisej.Web.Label labelSliceScreen;
        private Wisej.Web.Label labelSliceDialog;
        private Wisej.Web.Label labelSliceGrid;
        private Wisej.Web.Label labelSliceFile;
        private Wisej.Web.Label labelSliceBoundary;
        private Wisej.Web.Panel panelOrders;
        private Wisej.Web.Label labelOrdersTitle;
        private Wisej.Web.Label labelOrdersHint;
        private Wisej.Web.DataGridView gridOrders;
        private Wisej.Web.DataGridViewTextBoxColumn colOrderId;
        private Wisej.Web.DataGridViewTextBoxColumn colOrderCustomer;
        private Wisej.Web.DataGridViewTextBoxColumn colOrderTotal;
        private Wisej.Web.DataGridViewTextBoxColumn colOrderStatus;
        private Wisej.Web.Label labelBanner;
        private Wisej.Web.Button buttonRunSlice;
        private Wisej.Web.Button buttonSecondSession;
        private Wisej.Web.Button buttonExportDesktop;
        private Wisej.Web.Button buttonExportDownload;
        private Wisej.Web.Button buttonReplay;
        private Wisej.Web.Timer timerReplay;
    }
}
