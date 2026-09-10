namespace OperationsConsole
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
            if (disposing)
            {
                Wisej.Web.Application.ResponsiveProfileChanged -= this.Application_ResponsiveProfileChanged;

                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolBar = new Wisej.Web.ToolBar();
            this.btnMobileMenu = new Wisej.Web.ToolBarButton();
            this.sepMobileMenu = new Wisej.Web.ToolBarButton();
            this.btnNew = new Wisej.Web.ToolBarButton();
            this.btnRefresh = new Wisej.Web.ToolBarButton();
            this.btnSave = new Wisej.Web.ToolBarButton();
            this.sepDemo = new Wisej.Web.ToolBarButton();
            this.btnForceNarrow = new Wisej.Web.ToolBarButton();
            this.btnSimulateFailure = new Wisej.Web.ToolBarButton();
            this.statusBar = new Wisej.Web.StatusBar();
            this.pnlStatus = new Wisej.Web.StatusBarPanel();
            this.pnlProfile = new Wisej.Web.StatusBarPanel();
            this.pnlRecords = new Wisej.Web.StatusBarPanel();
            this.pnlRecord = new Wisej.Web.StatusBarPanel();
            this.pnlControl = new Wisej.Web.StatusBarPanel();
            this.pnlRefresh = new Wisej.Web.StatusBarPanel();
            this.splitMain = new Wisej.Web.SplitContainer();
            this.navList = new Wisej.Web.ListBox();
            this.lblSections = new Wisej.Web.Label();
            this.tabDetail = new Wisej.Web.TabControl();
            this.tabEditors = new Wisej.Web.TabPage();
            this.tabLayouts = new Wisej.Web.TabPage();
            this.tabListsTrees = new Wisej.Web.TabPage();
            this.tabDataGridView = new Wisej.Web.TabPage();
            this.tabDashboard = new Wisej.Web.TabPage();
            this.tabWidgets = new Wisej.Web.TabPage();
            this.pnlEventLog = new Wisej.Web.Panel();
            this.lblEventLogTitle = new Wisej.Web.Label();
            this.lstEventLog = new Wisej.Web.ListBox();
            this.btnClearLog = new Wisej.Web.Button();
            this.navContextMenu = new Wisej.Web.ContextMenu(this.components);
            this.mnuRefresh = new Wisej.Web.MenuItem();
            this.mnuNew = new Wisej.Web.MenuItem();
            this.mnuSave = new Wisej.Web.MenuItem();
            this.mnuSeparator = new Wisej.Web.MenuItem();
            this.mnuSimulateFailure = new Wisej.Web.MenuItem();
            this.sectionsMenu = new Wisej.Web.ContextMenu(this.components);
            this.mnuGoEditors = new Wisej.Web.MenuItem();
            this.mnuGoLayouts = new Wisej.Web.MenuItem();
            this.mnuGoListsTrees = new Wisej.Web.MenuItem();
            this.mnuGoDataGridView = new Wisej.Web.MenuItem();
            this.mnuGoDashboard = new Wisej.Web.MenuItem();
            this.mnuGoWidgets = new Wisej.Web.MenuItem();
            this.mnuGoSeparator = new Wisej.Web.MenuItem();
            this.mnuShowNavigation = new Wisej.Web.MenuItem();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.tabDetail.SuspendLayout();
            this.pnlEventLog.SuspendLayout();
            this.SuspendLayout();
            //
            // toolBar  (command surface · Dock = Top — the frequently used commands)
            //
            this.toolBar.Buttons.AddRange(new Wisej.Web.ToolBarButton[] {
                this.btnMobileMenu,
                this.sepMobileMenu,
                this.btnNew,
                this.btnRefresh,
                this.btnSave,
                this.sepDemo,
                this.btnForceNarrow,
                this.btnSimulateFailure});
            this.toolBar.Dock = Wisej.Web.DockStyle.Top;
            this.toolBar.Name = "toolBar";
            this.toolBar.ShowToolTips = true;
            this.toolBar.Size = new System.Drawing.Size(1280, 42);
            this.toolBar.TabIndex = 0;
            this.toolBar.ButtonClick += new Wisej.Web.ToolBarButtonClickEventHandler(this.toolBar_ButtonClick);
            //
            // btnMobileMenu  (narrow profile only: the way back to the collapsed navigation)
            //
            this.btnMobileMenu.Name = "btnMobileMenu";
            this.btnMobileMenu.Text = "☰ Sections";
            this.btnMobileMenu.ToolTipText = "Opens the sections menu and can expand the navigation panel again (narrow profile).";
            this.btnMobileMenu.Visible = false;
            //
            // sepMobileMenu
            //
            this.sepMobileMenu.Name = "sepMobileMenu";
            this.sepMobileMenu.Style = Wisej.Web.ToolBarButtonStyle.Separator;
            this.sepMobileMenu.Visible = false;
            //
            // btnNew  (one-line handler → NewRecord())
            //
            this.btnNew.Name = "btnNew";
            this.btnNew.Text = "New";
            this.btnNew.ToolTipText = "Adds a record to the section that is open (NewRecord()).";
            //
            // btnRefresh  (one-line handler → RefreshCurrentSection())
            //
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.ToolTipText = "Calls ISection.RefreshSection() on the page of the selected tab (RefreshCurrentSection()).";
            //
            // btnSave  (one-line handler → SaveCurrentSection())
            //
            this.btnSave.Name = "btnSave";
            this.btnSave.Text = "Save";
            this.btnSave.ToolTipText = "Saves the section that is open (SaveCurrentSection()).";
            //
            // sepDemo
            //
            this.sepDemo.Name = "sepDemo";
            this.sepDemo.Style = Wisej.Web.ToolBarButtonStyle.Separator;
            //
            // btnForceNarrow  (review aid: the narrow path without resizing the browser)
            //
            this.btnForceNarrow.Name = "btnForceNarrow";
            this.btnForceNarrow.Style = Wisej.Web.ToolBarButtonStyle.ToggleButton;
            this.btnForceNarrow.Text = "Force narrow";
            this.btnForceNarrow.ToolTipText = "Runs ApplyNarrowProfile(true) at any browser width so the narrow layout can be reviewed without resizing.";
            //
            // btnSimulateFailure  (failure path: SectionCatalog.CreatePage throws for the next tab)
            //
            this.btnSimulateFailure.Name = "btnSimulateFailure";
            this.btnSimulateFailure.Style = Wisej.Web.ToolBarButtonStyle.ToggleButton;
            this.btnSimulateFailure.Text = "Simulate page failure";
            this.btnSimulateFailure.ToolTipText = "The next page the tab surface creates throws inside SectionCatalog: the tab shows a friendly placeholder, the StatusBar turns red and an AlertBox explains.";
            //
            // statusBar  (status surface · Dock = Bottom — real status, not the word "Ready")
            //
            this.statusBar.Dock = Wisej.Web.DockStyle.Bottom;
            this.statusBar.Name = "statusBar";
            this.statusBar.Panels.AddRange(new Wisej.Web.StatusBarPanel[] {
                this.pnlStatus,
                this.pnlProfile,
                this.pnlRecords,
                this.pnlRecord,
                this.pnlControl,
                this.pnlRefresh});
            this.statusBar.ShowPanels = true;
            this.statusBar.SizingGrip = false;
            this.statusBar.Size = new System.Drawing.Size(1280, 30);
            this.statusBar.TabIndex = 1;
            //
            // pnlStatus  (the message, coloured by StatusLevel — AllowHtml because StatusBarPanel has no ForeColor)
            //
            this.pnlStatus.AllowHtml = true;
            this.pnlStatus.AutoSize = Wisej.Web.StatusBarPanelAutoSize.Spring;
            this.pnlStatus.MinWidth = 240;
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Text = "Ready";
            this.pnlStatus.ToolTipText = "The outcome of the last command, green / amber / red.";
            //
            // pnlProfile  (active client profile)
            //
            this.pnlProfile.AutoSize = Wisej.Web.StatusBarPanelAutoSize.Contents;
            this.pnlProfile.MinWidth = 140;
            this.pnlProfile.Name = "pnlProfile";
            this.pnlProfile.Text = "Desktop profile";
            this.pnlProfile.ToolTipText = "Application.ActiveProfile.Name — Phone ≤ 600 px, Tablet ≤ 1024 px, Desktop (ClientProfiles.json).";
            //
            // pnlRecords  (selected record count of the section that is open)
            //
            this.pnlRecords.AutoSize = Wisej.Web.StatusBarPanelAutoSize.Contents;
            this.pnlRecords.MinWidth = 95;
            this.pnlRecords.Name = "pnlRecords";
            this.pnlRecords.Text = "— records";
            this.pnlRecords.ToolTipText = "Records held by the section that is open (a placeholder section has none).";
            //
            // pnlRecord  (IConsoleShell.SetSelectedRecord)
            //
            this.pnlRecord.AutoSize = Wisej.Web.StatusBarPanelAutoSize.Contents;
            this.pnlRecord.MinWidth = 115;
            this.pnlRecord.Name = "pnlRecord";
            this.pnlRecord.Text = "Record: —";
            this.pnlRecord.ToolTipText = "ID of the selected record (ConsoleLog.Record).";
            //
            // pnlControl  (IConsoleShell.SetSelectedControl)
            //
            this.pnlControl.AutoSize = Wisej.Web.StatusBarPanelAutoSize.Contents;
            this.pnlControl.MinWidth = 150;
            this.pnlControl.Name = "pnlControl";
            this.pnlControl.Text = "Control: —";
            this.pnlControl.ToolTipText = "Name of the control used last (ConsoleLog.Control).";
            //
            // pnlRefresh  (last refresh time)
            //
            this.pnlRefresh.AutoSize = Wisej.Web.StatusBarPanelAutoSize.Contents;
            this.pnlRefresh.MinWidth = 135;
            this.pnlRefresh.Name = "pnlRefresh";
            this.pnlRefresh.Text = "Refreshed: —";
            this.pnlRefresh.ToolTipText = "Time of the last Refresh command.";
            //
            // splitMain  (work surface · Dock = Fill — the user decides how much room the navigation gets)
            //
            this.splitMain.Dock = Wisej.Web.DockStyle.Fill;
            this.splitMain.FixedPanel = Wisej.Web.FixedPanel.Panel1;
            this.splitMain.Name = "splitMain";
            this.splitMain.Orientation = Wisej.Web.Orientation.Vertical;
            //
            // splitMain.Panel1  (navigation — Fill added first, the caption Top added last)
            //
            this.splitMain.Panel1.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.splitMain.Panel1.Controls.Add(this.navList);
            this.splitMain.Panel1.Controls.Add(this.lblSections);
            this.splitMain.Panel1.Name = "Panel1";
            //
            // splitMain.Panel2  (detail surface)
            //
            this.splitMain.Panel2.Controls.Add(this.tabDetail);
            this.splitMain.Panel2.Name = "Panel2";
            this.splitMain.Panel1MinSize = 180;
            this.splitMain.Panel2MinSize = 360;
            this.splitMain.Size = new System.Drawing.Size(960, 728);
            this.splitMain.SplitterDistance = 240;
            this.splitMain.TabIndex = 2;
            //
            // navList  (the navigation list · Dock = Fill; ContextMenu = the same commands as the ToolBar)
            //
            this.navList.AccessibleName = "Sections";
            this.navList.ContextMenu = this.navContextMenu;
            this.navList.Dock = Wisej.Web.DockStyle.Fill;
            this.navList.Name = "navList";
            this.navList.RightClickSelection = true;
            this.navList.TabIndex = 1;
            this.navList.SelectedIndexChanged += new System.EventHandler(this.navList_SelectedIndexChanged);
            //
            // lblSections  (Dock = Top — added last so it wins the top edge of Panel1)
            //
            this.lblSections.AutoSize = false;
            this.lblSections.Dock = Wisej.Web.DockStyle.Top;
            this.lblSections.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblSections.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblSections.Name = "lblSections";
            this.lblSections.Padding = new Wisej.Web.Padding(14, 0, 8, 0);
            this.lblSections.Size = new System.Drawing.Size(240, 34);
            this.lblSections.Text = "SECTIONS";
            this.lblSections.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // tabDetail  (peer sections · Dock = Fill inside splitMain.Panel2)
            //
            this.tabDetail.Dock = Wisej.Web.DockStyle.Fill;
            this.tabDetail.Name = "tabDetail";
            this.tabDetail.SelectedIndex = 0;
            this.tabDetail.ShowToolTips = true;
            this.tabDetail.TabIndex = 2;
            this.tabDetail.TabPages.AddRange(new Wisej.Web.TabPage[] {
                this.tabEditors,
                this.tabLayouts,
                this.tabListsTrees,
                this.tabDataGridView,
                this.tabDashboard,
                this.tabWidgets});
            this.tabDetail.SelectedIndexChanged += new System.EventHandler(this.tabDetail_SelectedIndexChanged);
            //
            // tabEditors
            //
            this.tabEditors.Name = "tabEditors";
            this.tabEditors.Tag = OperationsConsole.Models.SectionKey.Editors;
            this.tabEditors.Text = "Editors";
            this.tabEditors.ToolTipText = "Module 2 · editors, buttons, validation and feedback";
            //
            // tabLayouts
            //
            this.tabLayouts.Name = "tabLayouts";
            this.tabLayouts.Tag = OperationsConsole.Models.SectionKey.Layouts;
            this.tabLayouts.Text = "Layouts";
            this.tabLayouts.ToolTipText = "Module 3 · containers, layouts, navigation and reuse";
            //
            // tabListsTrees
            //
            this.tabListsTrees.Name = "tabListsTrees";
            this.tabListsTrees.Tag = OperationsConsole.Models.SectionKey.ListsTrees;
            this.tabListsTrees.Text = "Lists and Trees";
            this.tabListsTrees.ToolTipText = "Module 4 · lists, trees, repeaters and hierarchical data";
            //
            // tabDataGridView
            //
            this.tabDataGridView.Name = "tabDataGridView";
            this.tabDataGridView.Tag = OperationsConsole.Models.SectionKey.DataGridView;
            this.tabDataGridView.Text = "DataGridView";
            this.tabDataGridView.ToolTipText = "Module 5 · DataGridView mastery";
            //
            // tabDashboard
            //
            this.tabDashboard.Name = "tabDashboard";
            this.tabDashboard.Tag = OperationsConsole.Models.SectionKey.Dashboard;
            this.tabDashboard.Text = "Dashboard";
            this.tabDashboard.ToolTipText = "Module 6 · charts, dashboards, content, media and documents";
            //
            // tabWidgets
            //
            this.tabWidgets.Name = "tabWidgets";
            this.tabWidgets.Tag = OperationsConsole.Models.SectionKey.Widgets;
            this.tabWidgets.Text = "Widgets";
            this.tabWidgets.ToolTipText = "Module 7 · custom widgets, extensions, theming and capstone";
            //
            // pnlEventLog  (Event log card · Dock = Right — the lab instrument from Module 1, unchanged)
            //
            this.pnlEventLog.BackColor = System.Drawing.Color.White;
            this.pnlEventLog.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlEventLog.Controls.Add(this.lblEventLogTitle);
            this.pnlEventLog.Controls.Add(this.lstEventLog);
            this.pnlEventLog.Controls.Add(this.btnClearLog);
            this.pnlEventLog.Dock = Wisej.Web.DockStyle.Right;
            this.pnlEventLog.Name = "pnlEventLog";
            this.pnlEventLog.Size = new System.Drawing.Size(320, 728);
            this.pnlEventLog.TabIndex = 3;
            //
            // lblEventLogTitle
            //
            this.lblEventLogTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblEventLogTitle.AutoSize = false;
            this.lblEventLogTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblEventLogTitle.Location = new System.Drawing.Point(16, 12);
            this.lblEventLogTitle.Name = "lblEventLogTitle";
            this.lblEventLogTitle.Size = new System.Drawing.Size(288, 30);
            this.lblEventLogTitle.Text = "Event log";
            this.lblEventLogTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lstEventLog  (every user action, service call and decision: "HH:mm:ss  message")
            //
            this.lstEventLog.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstEventLog.Font = new System.Drawing.Font("monospace", 9F);
            this.lstEventLog.Location = new System.Drawing.Point(16, 50);
            this.lstEventLog.Name = "lstEventLog";
            this.lstEventLog.Size = new System.Drawing.Size(288, 618);
            this.lstEventLog.TabIndex = 20;
            //
            // btnClearLog
            //
            this.btnClearLog.AccessibleName = "Clear the event log";
            this.btnClearLog.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.btnClearLog.Location = new System.Drawing.Point(16, 680);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(100, 32);
            this.btnClearLog.TabIndex = 21;
            this.btnClearLog.Text = "Clear";
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            //
            // navContextMenu  (commands tied to the navigation selection — the same command methods as the ToolBar)
            //
            this.navContextMenu.MenuItems.AddRange(new Wisej.Web.MenuItem[] {
                this.mnuRefresh,
                this.mnuNew,
                this.mnuSave,
                this.mnuSeparator,
                this.mnuSimulateFailure});
            this.navContextMenu.Name = "navContextMenu";
            //
            // mnuRefresh
            //
            this.mnuRefresh.Name = "mnuRefresh";
            this.mnuRefresh.Text = "Refresh";
            this.mnuRefresh.Click += new System.EventHandler(this.mnuRefresh_Click);
            //
            // mnuNew
            //
            this.mnuNew.Name = "mnuNew";
            this.mnuNew.Text = "New";
            this.mnuNew.Click += new System.EventHandler(this.mnuNew_Click);
            //
            // mnuSave
            //
            this.mnuSave.Name = "mnuSave";
            this.mnuSave.Text = "Save";
            this.mnuSave.Click += new System.EventHandler(this.mnuSave_Click);
            //
            // mnuSeparator
            //
            this.mnuSeparator.Name = "mnuSeparator";
            this.mnuSeparator.Text = "-";
            //
            // mnuSimulateFailure
            //
            this.mnuSimulateFailure.Name = "mnuSimulateFailure";
            this.mnuSimulateFailure.Text = "Simulate page failure";
            this.mnuSimulateFailure.Click += new System.EventHandler(this.mnuSimulateFailure_Click);
            //
            // sectionsMenu  (narrow profile: the navigation as a menu, opened by btnMobileMenu)
            //
            this.sectionsMenu.MenuItems.AddRange(new Wisej.Web.MenuItem[] {
                this.mnuGoEditors,
                this.mnuGoLayouts,
                this.mnuGoListsTrees,
                this.mnuGoDataGridView,
                this.mnuGoDashboard,
                this.mnuGoWidgets,
                this.mnuGoSeparator,
                this.mnuShowNavigation});
            this.sectionsMenu.Name = "sectionsMenu";
            //
            // mnuGoEditors
            //
            this.mnuGoEditors.Name = "mnuGoEditors";
            this.mnuGoEditors.Text = "Editors";
            this.mnuGoEditors.Click += new System.EventHandler(this.mnuGoEditors_Click);
            //
            // mnuGoLayouts
            //
            this.mnuGoLayouts.Name = "mnuGoLayouts";
            this.mnuGoLayouts.Text = "Layouts";
            this.mnuGoLayouts.Click += new System.EventHandler(this.mnuGoLayouts_Click);
            //
            // mnuGoListsTrees
            //
            this.mnuGoListsTrees.Name = "mnuGoListsTrees";
            this.mnuGoListsTrees.Text = "Lists and Trees";
            this.mnuGoListsTrees.Click += new System.EventHandler(this.mnuGoListsTrees_Click);
            //
            // mnuGoDataGridView
            //
            this.mnuGoDataGridView.Name = "mnuGoDataGridView";
            this.mnuGoDataGridView.Text = "DataGridView";
            this.mnuGoDataGridView.Click += new System.EventHandler(this.mnuGoDataGridView_Click);
            //
            // mnuGoDashboard
            //
            this.mnuGoDashboard.Name = "mnuGoDashboard";
            this.mnuGoDashboard.Text = "Dashboard";
            this.mnuGoDashboard.Click += new System.EventHandler(this.mnuGoDashboard_Click);
            //
            // mnuGoWidgets
            //
            this.mnuGoWidgets.Name = "mnuGoWidgets";
            this.mnuGoWidgets.Text = "Widgets";
            this.mnuGoWidgets.Click += new System.EventHandler(this.mnuGoWidgets_Click);
            //
            // mnuGoSeparator
            //
            this.mnuGoSeparator.Name = "mnuGoSeparator";
            this.mnuGoSeparator.Text = "-";
            //
            // mnuShowNavigation  (the way back: expand splitMain.Panel1 again)
            //
            this.mnuShowNavigation.Name = "mnuShowNavigation";
            this.mnuShowNavigation.Text = "Show the navigation panel";
            this.mnuShowNavigation.Click += new System.EventHandler(this.mnuShowNavigation_Click);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            // Docking is applied from the LAST added control to the FIRST (the designer serialises the reverse z-order).
            // The plan in docs/LayoutNotes.md — ToolBar Top, StatusBar Bottom, SplitContainer Fill — therefore reads
            // bottom-up here: splitMain (Fill) first, then the Right card, then the Bottom and Top bars.
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.pnlEventLog);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.toolBar);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1280, 800);
            this.Text = "Operations Console";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            this.splitMain.ResumeLayout(false);
            this.tabDetail.ResumeLayout(false);
            this.pnlEventLog.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        // command surface (Dock = Top)
        private Wisej.Web.ToolBar toolBar;
        private Wisej.Web.ToolBarButton btnMobileMenu;
        private Wisej.Web.ToolBarButton sepMobileMenu;
        private Wisej.Web.ToolBarButton btnNew;
        private Wisej.Web.ToolBarButton btnRefresh;
        private Wisej.Web.ToolBarButton btnSave;
        private Wisej.Web.ToolBarButton sepDemo;
        private Wisej.Web.ToolBarButton btnForceNarrow;
        private Wisej.Web.ToolBarButton btnSimulateFailure;

        // status surface (Dock = Bottom)
        private Wisej.Web.StatusBar statusBar;
        private Wisej.Web.StatusBarPanel pnlStatus;
        private Wisej.Web.StatusBarPanel pnlProfile;
        private Wisej.Web.StatusBarPanel pnlRecords;
        private Wisej.Web.StatusBarPanel pnlRecord;
        private Wisej.Web.StatusBarPanel pnlControl;
        private Wisej.Web.StatusBarPanel pnlRefresh;

        // work surface (Dock = Fill): navigation | TabControl detail
        private Wisej.Web.SplitContainer splitMain;
        private Wisej.Web.ListBox navList;
        private Wisej.Web.Label lblSections;
        private Wisej.Web.TabControl tabDetail;
        private Wisej.Web.TabPage tabEditors;
        private Wisej.Web.TabPage tabLayouts;
        private Wisej.Web.TabPage tabListsTrees;
        private Wisej.Web.TabPage tabDataGridView;
        private Wisej.Web.TabPage tabDashboard;
        private Wisej.Web.TabPage tabWidgets;

        // Event log card (shell contract, Module 1)
        private Wisej.Web.Panel pnlEventLog;
        private Wisej.Web.Label lblEventLogTitle;
        private Wisej.Web.ListBox lstEventLog;
        private Wisej.Web.Button btnClearLog;

        // menus
        private Wisej.Web.ContextMenu navContextMenu;
        private Wisej.Web.MenuItem mnuRefresh;
        private Wisej.Web.MenuItem mnuNew;
        private Wisej.Web.MenuItem mnuSave;
        private Wisej.Web.MenuItem mnuSeparator;
        private Wisej.Web.MenuItem mnuSimulateFailure;
        private Wisej.Web.ContextMenu sectionsMenu;
        private Wisej.Web.MenuItem mnuGoEditors;
        private Wisej.Web.MenuItem mnuGoLayouts;
        private Wisej.Web.MenuItem mnuGoListsTrees;
        private Wisej.Web.MenuItem mnuGoDataGridView;
        private Wisej.Web.MenuItem mnuGoDashboard;
        private Wisej.Web.MenuItem mnuGoWidgets;
        private Wisej.Web.MenuItem mnuGoSeparator;
        private Wisej.Web.MenuItem mnuShowNavigation;
    }
}
