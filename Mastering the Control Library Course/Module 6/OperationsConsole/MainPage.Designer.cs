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
            this.statusBar = new Wisej.Web.StatusBar();
            this.pnlStatus = new Wisej.Web.StatusBarPanel();
            this.pnlProfile = new Wisej.Web.StatusBarPanel();
            this.pnlRecords = new Wisej.Web.StatusBarPanel();
            this.pnlRecord = new Wisej.Web.StatusBarPanel();
            this.pnlControl = new Wisej.Web.StatusBarPanel();
            this.pnlRefresh = new Wisej.Web.StatusBarPanel();
            this.splitMain = new Wisej.Web.SplitContainer();
            this.navList = new Wisej.Web.ListBox();
            this.tabDetail = new Wisej.Web.TabControl();
            this.tabEditors = new Wisej.Web.TabPage();
            this.tabLayouts = new Wisej.Web.TabPage();
            this.tabListsTrees = new Wisej.Web.TabPage();
            this.tabDataGridView = new Wisej.Web.TabPage();
            this.tabDashboard = new Wisej.Web.TabPage();
            this.tabWidgets = new Wisej.Web.TabPage();
            this.navContextMenu = new Wisej.Web.ContextMenu(this.components);
            this.mnuRefresh = new Wisej.Web.MenuItem();
            this.mnuNew = new Wisej.Web.MenuItem();
            this.mnuSave = new Wisej.Web.MenuItem();
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
            this.SuspendLayout();
            //
            // toolBar
            //
            this.toolBar.Buttons.AddRange(new Wisej.Web.ToolBarButton[] {
                this.btnMobileMenu,
                this.sepMobileMenu,
                this.btnNew,
                this.btnRefresh,
                this.btnSave});
            this.toolBar.Dock = Wisej.Web.DockStyle.Top;
            this.toolBar.Name = "toolBar";
            this.toolBar.Size = new System.Drawing.Size(1280, 42);
            this.toolBar.TabIndex = 0;
            this.toolBar.ButtonClick += new Wisej.Web.ToolBarButtonClickEventHandler(this.toolBar_ButtonClick);
            //
            // btnMobileMenu
            //
            this.btnMobileMenu.Name = "btnMobileMenu";
            this.btnMobileMenu.Text = "☰ Sections";
            this.btnMobileMenu.Visible = false;
            //
            // sepMobileMenu
            //
            this.sepMobileMenu.Name = "sepMobileMenu";
            this.sepMobileMenu.Style = Wisej.Web.ToolBarButtonStyle.Separator;
            this.sepMobileMenu.Visible = false;
            //
            // btnNew
            //
            this.btnNew.Name = "btnNew";
            this.btnNew.Text = "New";
            //
            // btnRefresh
            //
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Text = "Refresh";
            //
            // btnSave
            //
            this.btnSave.Name = "btnSave";
            this.btnSave.Text = "Save";
            //
            // statusBar
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
            // pnlStatus
            //
            this.pnlStatus.AllowHtml = true;
            this.pnlStatus.AutoSize = Wisej.Web.StatusBarPanelAutoSize.Spring;
            this.pnlStatus.MinWidth = 240;
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Text = "Ready";
            //
            // pnlProfile
            //
            this.pnlProfile.AutoSize = Wisej.Web.StatusBarPanelAutoSize.Contents;
            this.pnlProfile.MinWidth = 140;
            this.pnlProfile.Name = "pnlProfile";
            this.pnlProfile.Text = "Desktop profile";
            //
            // pnlRecords
            //
            this.pnlRecords.AutoSize = Wisej.Web.StatusBarPanelAutoSize.Contents;
            this.pnlRecords.MinWidth = 95;
            this.pnlRecords.Name = "pnlRecords";
            this.pnlRecords.Text = "— records";
            //
            // pnlRecord
            //
            this.pnlRecord.AutoSize = Wisej.Web.StatusBarPanelAutoSize.Contents;
            this.pnlRecord.MinWidth = 115;
            this.pnlRecord.Name = "pnlRecord";
            this.pnlRecord.Text = "Record: —";
            //
            // pnlControl
            //
            this.pnlControl.AutoSize = Wisej.Web.StatusBarPanelAutoSize.Contents;
            this.pnlControl.MinWidth = 150;
            this.pnlControl.Name = "pnlControl";
            this.pnlControl.Text = "Control: —";
            //
            // pnlRefresh
            //
            this.pnlRefresh.AutoSize = Wisej.Web.StatusBarPanelAutoSize.Contents;
            this.pnlRefresh.MinWidth = 135;
            this.pnlRefresh.Name = "pnlRefresh";
            this.pnlRefresh.Text = "Refreshed: —";
            //
            // splitMain
            //
            this.splitMain.Dock = Wisej.Web.DockStyle.Fill;
            this.splitMain.FixedPanel = Wisej.Web.FixedPanel.Panel1;
            this.splitMain.Name = "splitMain";
            this.splitMain.Orientation = Wisej.Web.Orientation.Vertical;
            //
            // splitMain.Panel1
            //
            this.splitMain.Panel1.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.splitMain.Panel1.Controls.Add(this.navList);
            this.splitMain.Panel1.Name = "Panel1";
            //
            // splitMain.Panel2
            //
            this.splitMain.Panel2.Controls.Add(this.tabDetail);
            this.splitMain.Panel2.Name = "Panel2";
            this.splitMain.Panel1MinSize = 180;
            this.splitMain.Panel2MinSize = 360;
            this.splitMain.Size = new System.Drawing.Size(1280, 728);
            this.splitMain.SplitterDistance = 240;
            this.splitMain.TabIndex = 2;
            //
            // navList
            //
            this.navList.AccessibleName = "Sections";
            this.navList.ContextMenu = this.navContextMenu;
            this.navList.Dock = Wisej.Web.DockStyle.Fill;
            this.navList.Name = "navList";
            this.navList.RightClickSelection = true;
            this.navList.TabIndex = 1;
            this.navList.SelectedIndexChanged += new System.EventHandler(this.navList_SelectedIndexChanged);
            //
            // tabDetail
            //
            this.tabDetail.Dock = Wisej.Web.DockStyle.Fill;
            this.tabDetail.Name = "tabDetail";
            this.tabDetail.SelectedIndex = 0;
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
            //
            // tabLayouts
            //
            this.tabLayouts.Name = "tabLayouts";
            this.tabLayouts.Tag = OperationsConsole.Models.SectionKey.Layouts;
            this.tabLayouts.Text = "Layouts";
            //
            // tabListsTrees
            //
            this.tabListsTrees.Name = "tabListsTrees";
            this.tabListsTrees.Tag = OperationsConsole.Models.SectionKey.ListsTrees;
            this.tabListsTrees.Text = "Lists and Trees";
            //
            // tabDataGridView
            //
            this.tabDataGridView.Name = "tabDataGridView";
            this.tabDataGridView.Tag = OperationsConsole.Models.SectionKey.DataGridView;
            this.tabDataGridView.Text = "DataGridView";
            //
            // tabDashboard
            //
            this.tabDashboard.Name = "tabDashboard";
            this.tabDashboard.Tag = OperationsConsole.Models.SectionKey.Dashboard;
            this.tabDashboard.Text = "Dashboard";
            //
            // tabWidgets
            //
            this.tabWidgets.Name = "tabWidgets";
            this.tabWidgets.Tag = OperationsConsole.Models.SectionKey.Widgets;
            this.tabWidgets.Text = "Widgets";
            //
            // navContextMenu
            //
            this.navContextMenu.MenuItems.AddRange(new Wisej.Web.MenuItem[] {
                this.mnuRefresh,
                this.mnuNew,
                this.mnuSave});
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
            // sectionsMenu
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
            // mnuShowNavigation
            //
            this.mnuShowNavigation.Name = "mnuShowNavigation";
            this.mnuShowNavigation.Text = "Show the navigation panel";
            this.mnuShowNavigation.Click += new System.EventHandler(this.mnuShowNavigation_Click);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.splitMain);
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
        private Wisej.Web.TabControl tabDetail;
        private Wisej.Web.TabPage tabEditors;
        private Wisej.Web.TabPage tabLayouts;
        private Wisej.Web.TabPage tabListsTrees;
        private Wisej.Web.TabPage tabDataGridView;
        private Wisej.Web.TabPage tabDashboard;
        private Wisej.Web.TabPage tabWidgets;

        // menus
        private Wisej.Web.ContextMenu navContextMenu;
        private Wisej.Web.MenuItem mnuRefresh;
        private Wisej.Web.MenuItem mnuNew;
        private Wisej.Web.MenuItem mnuSave;
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
