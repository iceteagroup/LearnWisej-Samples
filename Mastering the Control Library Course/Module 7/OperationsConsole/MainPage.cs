using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using OperationsConsole.Models;
using OperationsConsole.Services;
using OperationsConsole.Shell;
using Wisej.Web;

namespace OperationsConsole
{
    /// <summary>
    /// The Operations Console shell: <c>toolBar</c> Top, <c>statusBar</c> Bottom and <c>splitMain</c> Fill, with the
    /// navigation list in <c>splitMain.Panel1</c> and the peer sections (<c>tabDetail</c>) in <c>splitMain.Panel2</c>.
    /// Every command surface — ToolBar button, ContextMenu item, navigation list, tab — calls one of the named
    /// command methods (<see cref="NewRecord"/>, <see cref="RefreshCurrentSection"/>, <see cref="SaveCurrentSection"/>,
    /// <see cref="SelectSection"/>).
    /// </summary>
    public partial class MainPage : Page, IConsoleShell
    {
        private static readonly string OkColor = "#1f9d57";
        private static readonly string WarningColor = "#e8a13c";
        private static readonly string ErrorColor = "#e0563b";

        private readonly SectionCatalog _catalog = new SectionCatalog();
        private readonly Dictionary<SectionKey, TabPage> _tabsByKey = new Dictionary<SectionKey, TabPage>();
        private readonly Dictionary<SectionKey, Control> _pages = new Dictionary<SectionKey, Control>();
        private readonly List<SectionKey> _navOrder = new List<SectionKey>();

        private bool _syncingSelection;
        private bool _narrow;

        public MainPage()
        {
            InitializeComponent();

            BuildNavigation();

            Application.ResponsiveProfileChanged += Application_ResponsiveProfileChanged;
        }

        /// <summary>The navigation list and the tabs are matched by <see cref="SectionKey"/>.</summary>
        private void BuildNavigation()
        {
            foreach (TabPage tab in tabDetail.TabPages)
                _tabsByKey[(SectionKey)tab.Tag] = tab;

            foreach (var info in _catalog.Sections)
            {
                _navOrder.Add(info.Key);
                navList.Items.Add(info.Title);
            }
        }

        /// <summary>
        /// The first section page is created here and not in the constructor, because
        /// <see cref="Application.MainPage"/> is only set once the constructor has returned.
        /// </summary>
        private void MainPage_Load(object sender, EventArgs e)
        {
            SetSelectedControl(null);
            SetSelectedRecord(null);
            UpdateProfile();

            SelectSection(SectionKey.Layouts, null);
        }

        // ------------------------------------------------------------------------------------------------------------
        // Command surfaces — every handler is one line that calls a named command method
        // ------------------------------------------------------------------------------------------------------------

        private void toolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            switch (e.Button.Name)
            {
                case "btnNew": NewRecord(); break;
                case "btnRefresh": RefreshCurrentSection(); break;
                case "btnSave": SaveCurrentSection(); break;
                case "btnMobileMenu": ShowSectionsMenu(); break;
            }
        }

        private void mnuNew_Click(object sender, EventArgs e) => NewRecord();
        private void mnuRefresh_Click(object sender, EventArgs e) => RefreshCurrentSection();
        private void mnuSave_Click(object sender, EventArgs e) => SaveCurrentSection();

        private void mnuGoEditors_Click(object sender, EventArgs e) => SelectSection(SectionKey.Editors, "mnuGoEditors");
        private void mnuGoLayouts_Click(object sender, EventArgs e) => SelectSection(SectionKey.Layouts, "mnuGoLayouts");
        private void mnuGoListsTrees_Click(object sender, EventArgs e) => SelectSection(SectionKey.ListsTrees, "mnuGoListsTrees");
        private void mnuGoDataGridView_Click(object sender, EventArgs e) => SelectSection(SectionKey.DataGridView, "mnuGoDataGridView");
        private void mnuGoDashboard_Click(object sender, EventArgs e) => SelectSection(SectionKey.Dashboard, "mnuGoDashboard");
        private void mnuGoWidgets_Click(object sender, EventArgs e) => SelectSection(SectionKey.Widgets, "mnuGoWidgets");
        private void mnuShowNavigation_Click(object sender, EventArgs e) => ShowNavigationPanel();

        private void navList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_syncingSelection || navList.SelectedIndex < 0)
                return;

            SelectSection(_navOrder[navList.SelectedIndex], navList.Name);
        }

        private void tabDetail_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_syncingSelection || tabDetail.SelectedTab == null)
                return;

            SelectSection((SectionKey)tabDetail.SelectedTab.Tag, tabDetail.Name);
        }

        // ------------------------------------------------------------------------------------------------------------
        // Navigation: the list and the tabs are kept in step, and a tab's page is created on first use
        // ------------------------------------------------------------------------------------------------------------

        /// <summary>Selects a section on both surfaces and makes sure its page exists.</summary>
        private void SelectSection(SectionKey key, string source)
        {
            if (_syncingSelection)
                return;

            _syncingSelection = true;
            try
            {
                var tab = _tabsByKey[key];
                var index = _navOrder.IndexOf(key);

                if (navList.SelectedIndex != index)
                    navList.SelectedIndex = index;

                if (tabDetail.SelectedTab != tab)
                    tabDetail.SelectedTab = tab;

                SetSelectedControl(source);
                EnsurePage(tab);
            }
            finally
            {
                _syncingSelection = false;
            }
        }

        /// <summary>
        /// Creates the section page for a tab and docks it Fill. A page that cannot be created turns the StatusBar
        /// red and shows an <see cref="AlertBox"/>; Refresh tries again.
        /// </summary>
        private void EnsurePage(TabPage tab)
        {
            var key = (SectionKey)tab.Tag;
            var info = _catalog.Get(key);

            if (!_pages.ContainsKey(key))
            {
                try
                {
                    var page = _catalog.CreatePage(key);
                    page.Dock = DockStyle.Fill;
                    tab.Controls.Add(page);
                    _pages[key] = page;
                }
                catch (Exception)
                {
                    SetStatus("The " + info.Title + " section could not be opened — Refresh tries again.", StatusLevel.Error);
                    AlertBox.Show(
                        "The " + info.Title + " section could not be opened. Choose Refresh to try again; if it keeps failing, contact support.",
                        MessageBoxIcon.Error, alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);
                    SetSelectedRecord(null);
                    UpdateRecordCount();
                    return;
                }
            }

            SetSelectedRecord(null);
            UpdateRecordCount();
            SetStatus("Showing " + info.Title, StatusLevel.Ok);
        }

        // ------------------------------------------------------------------------------------------------------------
        // The named commands the ToolBar, the ContextMenu and the UserControls call
        // ------------------------------------------------------------------------------------------------------------

        /// <summary>New: asks the open section to create a record.</summary>
        private void NewRecord()
        {
            SetSelectedControl("btnNew");

            var info = CurrentSection;
            if (CurrentPage is ISectionRecords records)
            {
                var id = records.NewRecord();

                SetSelectedRecord(id);
                UpdateRecordCount();
                SetStatus("Added " + id + " to " + info.Title + ".", StatusLevel.Ok);
                ShowToast("Added " + id + ".", "icon-check");
            }
            else
            {
                SetStatus("New is not available on " + info.Title + ".", StatusLevel.Warning);
                ShowToast("Nothing to create on " + info.Title + ".", "icon-warning");
            }
        }

        /// <summary>Refresh: calls <see cref="ISection.RefreshSection"/> on the page of the selected tab.</summary>
        private void RefreshCurrentSection()
        {
            SetSelectedControl("btnRefresh");

            var tab = tabDetail.SelectedTab;
            if (tab == null)
                return;

            var info = CurrentSection;

            if (!_pages.ContainsKey(info.Key))
            {
                EnsurePage(tab);

                if (!_pages.ContainsKey(info.Key))
                    return;
            }

            var section = (ISection)_pages[info.Key];
            section.RefreshSection();

            StampRefresh();
            UpdateRecordCount();
            SetStatus(section.Title + " refreshed at " + DateTime.Now.ToString("HH:mm:ss") + ".", StatusLevel.Ok);
        }

        /// <summary>Save: the section decides — saved, rejected or failed.</summary>
        private void SaveCurrentSection()
        {
            SetSelectedControl("btnSave");

            var info = CurrentSection;
            if (CurrentPage is ISectionRecords records)
            {
                var result = records.Save();

                SetStatus(result.Message, result.Level);
                ShowToast(result.Message, result.Accepted ? "icon-check" : result.Level == StatusLevel.Error ? "icon-error" : "icon-warning");

                if (result.Accepted)
                {
                    StampRefresh();
                    UpdateRecordCount();
                }
            }
            else
            {
                SetStatus("Nothing to save on " + info.Title + ".", StatusLevel.Warning);
                ShowToast("Nothing to save on " + info.Title + ".", "icon-warning");
            }
        }

        /// <summary>Narrow profile: opens the navigation as a menu, because the panel is collapsed.</summary>
        private void ShowSectionsMenu()
        {
            SetSelectedControl("btnMobileMenu");
            sectionsMenu.Show(toolBar, new Point(4, toolBar.Height), null);
        }

        private void ShowNavigationPanel()
        {
            splitMain.Panel1Collapsed = false;
            SetStatus("Navigation panel expanded.", StatusLevel.Ok);
        }

        // ------------------------------------------------------------------------------------------------------------
        // Responsive profile
        // ------------------------------------------------------------------------------------------------------------

        private void Application_ResponsiveProfileChanged(object sender, ResponsiveProfileChangedEventArgs e)
        {
            UpdateProfile();
        }

        private void UpdateProfile()
        {
            var profile = ActiveProfileName;
            ApplyNarrowProfile(profile == "Phone" || profile == "Tablet");
        }

        /// <summary>
        /// The narrow layout: the navigation panel collapses (it is not destroyed), the ToolBar shows the way back,
        /// and the StatusBar names the profile.
        /// </summary>
        private void ApplyNarrowProfile(bool narrow)
        {
            _narrow = narrow;

            splitMain.Panel1Collapsed = narrow;
            btnMobileMenu.Visible = narrow;
            sepMobileMenu.Visible = narrow;
            CompactStatusBar(narrow);
            UpdateProfilePanel();
        }

        /// <summary>On a narrow screen the diagnostic panels stop reserving a minimum width.</summary>
        private void CompactStatusBar(bool narrow)
        {
            pnlControl.MinWidth = narrow ? 0 : 150;
            pnlRecord.MinWidth = narrow ? 0 : 115;
            pnlStatus.MinWidth = narrow ? 120 : 240;
        }

        private void UpdateProfilePanel()
        {
            pnlProfile.Text = (_narrow ? "Narrow profile · " : "Desktop profile · ") + ActiveProfileName;
        }

        private static string ActiveProfileName
            => Application.ActiveProfile == null ? "Desktop" : Application.ActiveProfile.Name;

        // ------------------------------------------------------------------------------------------------------------
        // StatusBar helpers
        // ------------------------------------------------------------------------------------------------------------

        private SectionInfo CurrentSection
            => _catalog.Get(tabDetail.SelectedTab == null ? SectionKey.Editors : (SectionKey)tabDetail.SelectedTab.Tag);

        private Control CurrentPage
        {
            get
            {
                if (tabDetail.SelectedTab == null)
                    return null;

                Control page;
                return _pages.TryGetValue((SectionKey)tabDetail.SelectedTab.Tag, out page) ? page : null;
            }
        }

        private void UpdateRecordCount()
        {
            var records = CurrentPage as ISectionRecords;
            pnlRecords.Text = records == null ? "— records" : records.RecordCount + " records";
        }

        private void StampRefresh()
        {
            pnlRefresh.Text = "Refreshed: " + DateTime.Now.ToString("HH:mm:ss");
        }

        private static void ShowToast(string text, string icon)
        {
            new Toast(text, icon) { AutoCloseDelay = 3000, Alignment = ContentAlignment.TopRight }.Show();
        }

        // ------------------------------------------------------------------------------------------------------------
        // IConsoleShell
        // ------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// <see cref="StatusBarPanel"/> has no <c>ForeColor</c>, so the level is expressed with <c>AllowHtml</c> and one
        /// coloured span; the text itself is HTML-encoded.
        /// </summary>
        public void SetStatus(string text, StatusLevel level)
        {
            string color;
            switch (level)
            {
                case StatusLevel.Warning: color = WarningColor; break;
                case StatusLevel.Error: color = ErrorColor; break;
                default: color = OkColor; break;
            }

            pnlStatus.Text = "<span style='color:" + color + ";font-weight:bold'>" + WebUtility.HtmlEncode(text ?? "") + "</span>";
        }

        public void SetSelectedControl(string controlName)
        {
            pnlControl.Text = "Control: " + (string.IsNullOrEmpty(controlName) ? "—" : controlName);
        }

        public void SetSelectedRecord(string recordId)
        {
            pnlRecord.Text = "Record: " + (string.IsNullOrEmpty(recordId) ? "—" : recordId);
        }
    }
}
