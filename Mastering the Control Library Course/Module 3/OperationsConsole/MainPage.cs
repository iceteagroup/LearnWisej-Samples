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
    /// The Operations Console shell (Module 3 · Containers, Layouts, Navigation, and Reuse).
    /// <para>
    /// Three regions, docked in the order written down in <c>docs/LayoutNotes.md</c>: <c>toolBar</c> Top,
    /// <c>statusBar</c> Bottom and <c>splitMain</c> Fill — navigation (<c>navList</c>) in
    /// <c>splitMain.Panel1</c>, the peer sections (<c>tabDetail</c>) in <c>splitMain.Panel2</c>. The Event log
    /// card of Module 1 stays on the right, and this page is still the only implementation of
    /// <see cref="IConsoleShell"/>: sections talk to it through <see cref="ConsoleLog"/> and never to each other.
    /// </para>
    /// <para>
    /// Every command surface — ToolBar button, ContextMenu item, navigation list, tab — is a thin handler that
    /// calls one of the named command methods (<see cref="NewRecord"/>, <see cref="RefreshCurrentSection"/>,
    /// <see cref="SaveCurrentSection"/>, <see cref="SelectSection"/>). None of them contains business logic.
    /// </para>
    /// </summary>
    public partial class MainPage : Page, IConsoleShell
    {
        // status colours (the course palette)
        private static readonly string OkColor = "#1f9d57";
        private static readonly string WarningColor = "#e8a13c";
        private static readonly string ErrorColor = "#e0563b";

        /// <summary>Sections whose module number is at or below this are real pages in this folder; the rest are still placeholders.</summary>
        private const int BuiltThroughModule = 3;

        private readonly SectionCatalog _catalog = new SectionCatalog();
        private readonly Dictionary<SectionKey, TabPage> _tabsByKey = new Dictionary<SectionKey, TabPage>();
        private readonly Dictionary<SectionKey, Control> _pages = new Dictionary<SectionKey, Control>();
        private readonly List<SectionKey> _navOrder = new List<SectionKey>();

        private bool _syncingSelection;
        private bool _forceNarrow;
        private bool _narrow;

        public MainPage()
        {
            InitializeComponent();

            BuildNavigation();

            Application.ResponsiveProfileChanged += Application_ResponsiveProfileChanged;
        }

        /// <summary>
        /// The navigation list and the tab surface are two views of the same catalog: the list items and the
        /// tab pages are matched by <see cref="SectionKey"/>, so neither of them owns the section order.
        /// </summary>
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
        /// Fired before the page becomes visible. The first section page is created here and not in the
        /// constructor, because <see cref="Application.MainPage"/> — and therefore <see cref="ConsoleLog"/> —
        /// is only wired up once the constructor has returned.
        /// </summary>
        private void MainPage_Load(object sender, EventArgs e)
        {
            AddLog("Operations Console shell ready · " + _catalog.Sections.Count + " sections in the catalog");
            AddLog("child order: splitMain (Fill) added first, then pnlEventLog (Right), statusBar (Bottom), toolBar (Top) — docking is applied last-to-first");
            AddLog("splitMain.Panel1 = lblSections (Top) + navList (Fill) · splitMain.Panel2 = tabDetail (Fill)");

            SetSelectedControl(null);
            SetSelectedRecord(null);
            UpdateProfile("startup");

            SelectSection(SectionKey.Layouts, "startup");
            SetSelectedControl(null);   // nothing has been used yet: the diagnostic panel stays honest
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
                case "btnForceNarrow": ToggleForceNarrow(); break;
                case "btnSimulateFailure": ToggleSimulateFailure(); break;
            }
        }

        private void mnuNew_Click(object sender, EventArgs e) => NewRecord();
        private void mnuRefresh_Click(object sender, EventArgs e) => RefreshCurrentSection();
        private void mnuSave_Click(object sender, EventArgs e) => SaveCurrentSection();
        private void mnuSimulateFailure_Click(object sender, EventArgs e) => ToggleSimulateFailure();

        private void mnuGoEditors_Click(object sender, EventArgs e) => SelectSection(SectionKey.Editors, "mnuGoEditors");
        private void mnuGoLayouts_Click(object sender, EventArgs e) => SelectSection(SectionKey.Layouts, "mnuGoLayouts");
        private void mnuGoListsTrees_Click(object sender, EventArgs e) => SelectSection(SectionKey.ListsTrees, "mnuGoListsTrees");
        private void mnuGoDataGridView_Click(object sender, EventArgs e) => SelectSection(SectionKey.DataGridView, "mnuGoDataGridView");
        private void mnuGoDashboard_Click(object sender, EventArgs e) => SelectSection(SectionKey.Dashboard, "mnuGoDashboard");
        private void mnuGoWidgets_Click(object sender, EventArgs e) => SelectSection(SectionKey.Widgets, "mnuGoWidgets");
        private void mnuShowNavigation_Click(object sender, EventArgs e) => ShowNavigationPanel();

        private void btnClearLog_Click(object sender, EventArgs e) => lstEventLog.Items.Clear();

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
        // Navigation: the list and the tabs are kept in step, and the page of the selected tab is created on demand
        // ------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Selects a section on both surfaces (list and tab) and makes sure its page exists.
        /// <paramref name="source"/> is only the name of the control that asked, for the Event log and the StatusBar.
        /// </summary>
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
                EnsurePage(tab, source);
            }
            finally
            {
                _syncingSelection = false;
            }
        }

        /// <summary>
        /// Creates the section page for a tab through <see cref="SectionCatalog"/> and docks it Fill.
        /// The failure path lives here: a page that cannot be created leaves a friendly placeholder in the tab,
        /// turns the StatusBar red, shows an <see cref="AlertBox"/> in plain words and writes the exception to the
        /// Event log only. The recovery is Refresh (or selecting the tab again) once the simulation is switched off.
        /// </summary>
        private void EnsurePage(TabPage tab, string source)
        {
            var key = (SectionKey)tab.Tag;
            var info = _catalog.Get(key);

            if (_pages.ContainsKey(key))
            {
                ShowSectionStatus(info);
                return;
            }

            AddLog(source + " → SectionCatalog.CreatePage(" + info.PageTypeName + ")");

            try
            {
                var page = _catalog.CreatePage(key);
                page.Dock = DockStyle.Fill;

                ClearTab(tab);
                tab.Controls.Add(page);
                _pages[key] = page;

                AddLog("✓ " + info.PageTypeName + " hosted in " + tab.Name + " (Dock = Fill)");
                ShowSectionStatus(info);
            }
            catch (Exception ex)
            {
                AddLog("✗ " + info.PageTypeName + " could not be created — " + ex.GetType().Name);
                AddLog("   " + ex.Message);

                ClearTab(tab);
                tab.Controls.Add(CreateUnavailablePlaceholder(info));

                SetStatus("The " + info.Title + " section could not be opened — Refresh tries again.", StatusLevel.Error);
                AlertBox.Show(
                    "The " + info.Title + " section could not be opened. Choose Refresh to try again; if it keeps failing, contact support.",
                    MessageBoxIcon.Error, alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);

                SetSelectedRecord(null);
                UpdateRecordCount();
            }
        }

        /// <summary>Removes and disposes whatever the tab holds — a removed page is a server object, not just markup.</summary>
        private static void ClearTab(TabPage tab)
        {
            var previous = new List<Control>();
            foreach (Control child in tab.Controls)
                previous.Add(child);

            tab.Controls.Clear();

            foreach (var child in previous)
                child.Dispose();
        }

        /// <summary>The honest "this section is not available" surface: no exception text, no internals, one way out.</summary>
        private Control CreateUnavailablePlaceholder(SectionInfo info)
        {
            var host = new Panel
            {
                Name = "pnlSectionUnavailable",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(238, 242, 247)
            };

            var card = new Panel
            {
                Name = "pnlUnavailableCard",
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.White,
                BorderStyle = BorderStyle.Solid,
                Location = new Point(24, 24),
                Size = new Size(620, 150)
            };

            var title = new Label
            {
                Name = "lblUnavailableTitle",
                AutoSize = false,
                Font = new Font("default", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(224, 86, 59),
                Location = new Point(20, 16),
                Size = new Size(560, 28),
                Text = "The " + info.Title + " section is not available right now",
                TextAlign = ContentAlignment.MiddleLeft
            };

            var body = new Label
            {
                Name = "lblUnavailableBody",
                AutoSize = false,
                ForeColor = Color.FromArgb(58, 77, 99),
                Location = new Point(20, 50),
                Size = new Size(560, 44),
                Text = "Nothing was lost and the other sections still work. Choose Refresh (or Try again) to open it once more.",
                TextAlign = ContentAlignment.TopLeft
            };

            var retry = new Button
            {
                Name = "btnRetrySection",
                AccessibleName = "Try to open the section again",
                Location = new Point(20, 100),
                Size = new Size(120, 32),
                Text = "Try again"
            };
            retry.Click += (s, e) => RefreshCurrentSection();

            card.Controls.Add(title);
            card.Controls.Add(body);
            card.Controls.Add(retry);
            host.Controls.Add(card);
            return host;
        }

        private void ShowSectionStatus(SectionInfo info)
        {
            SetSelectedRecord(null);
            UpdateRecordCount();

            SetStatus(info.Module <= BuiltThroughModule
                ? "Showing " + info.Title
                : "Showing " + info.Title + " — still a placeholder, Module " + info.Module + " builds this section.",
                StatusLevel.Ok);
        }

        // ------------------------------------------------------------------------------------------------------------
        // The named commands the ToolBar, the ContextMenu and the UserControls call
        // ------------------------------------------------------------------------------------------------------------

        /// <summary>New: asks the section that is open to create a record. A placeholder section politely refuses.</summary>
        private void NewRecord()
        {
            SetSelectedControl("btnNew");

            var info = CurrentSection;
            if (CurrentPage is ISectionRecords records)
            {
                var id = records.NewRecord();
                AddLog("btnNew → " + info.PageTypeName + ".NewRecord() = " + id);

                SetSelectedRecord(id);
                UpdateRecordCount();
                SetStatus("Added " + id + " to " + info.Title + ".", StatusLevel.Ok);
                ShowToast("Added " + id + ".", "icon-check");
            }
            else
            {
                AddLog("btnNew → nothing to create on " + info.Title);
                SetStatus("New is not available on " + info.Title + " — that section holds no records yet.", StatusLevel.Warning);
                ShowToast("Nothing to create on " + info.Title + ".", "icon-warning");
            }
        }

        /// <summary>
        /// Refresh: calls <see cref="ISection.RefreshSection"/> on the page of the selected tab, and doubles as the
        /// recovery for a tab whose page could not be created.
        /// </summary>
        private void RefreshCurrentSection()
        {
            SetSelectedControl("btnRefresh");

            var tab = tabDetail.SelectedTab;
            if (tab == null)
                return;

            var info = CurrentSection;

            if (!_pages.ContainsKey(info.Key))
            {
                AddLog("btnRefresh → retrying " + info.PageTypeName + " (recovery)");
                EnsurePage(tab, "btnRefresh");

                if (!_pages.ContainsKey(info.Key))
                    return;      // still failing — EnsurePage already reported it
            }

            var section = (ISection)_pages[info.Key];
            AddLog("btnRefresh → " + info.PageTypeName + ".RefreshSection()");
            section.RefreshSection();

            StampRefresh();
            UpdateRecordCount();
            SetStatus(section.Title + " refreshed at " + DateTime.Now.ToString("HH:mm:ss") + ".", StatusLevel.Ok);
        }

        /// <summary>Save: the section decides. Success, a rejected save and "nothing to save" are three different answers.</summary>
        private void SaveCurrentSection()
        {
            SetSelectedControl("btnSave");

            var info = CurrentSection;
            if (CurrentPage is ISectionRecords records)
            {
                var result = records.Save();
                AddLog("btnSave → " + info.PageTypeName + ".Save() = " + (result.Accepted ? "accepted" : "rejected"));

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
                AddLog("btnSave → nothing to save on " + info.Title);
                SetStatus("Nothing to save on " + info.Title + " — that section holds no records yet.", StatusLevel.Warning);
                ShowToast("Nothing to save on " + info.Title + ".", "icon-warning");
            }
        }

        /// <summary>Narrow profile: opens the navigation as a menu, because the panel is collapsed.</summary>
        private void ShowSectionsMenu()
        {
            SetSelectedControl("btnMobileMenu");
            AddLog("btnMobileMenu → sectionsMenu.Show() (the navigation panel is collapsed)");
            sectionsMenu.Show(toolBar, new Point(4, toolBar.Height), null);
        }

        private void ShowNavigationPanel()
        {
            splitMain.Panel1Collapsed = false;
            AddLog("mnuShowNavigation → splitMain.Panel1Collapsed = false (the panel was collapsed, never rebuilt)");
            SetStatus("Navigation panel expanded — the selection was kept.", StatusLevel.Ok);
        }

        private void ToggleForceNarrow()
        {
            _forceNarrow = !_forceNarrow;
            btnForceNarrow.Pushed = _forceNarrow;
            AddLog("btnForceNarrow → " + (_forceNarrow ? "forcing the narrow profile" : "back to the profile of the browser"));
            UpdateProfile("btnForceNarrow");
        }

        private void ToggleSimulateFailure()
        {
            _catalog.SimulateFailure = !_catalog.SimulateFailure;
            btnSimulateFailure.Pushed = _catalog.SimulateFailure;
            mnuSimulateFailure.Checked = _catalog.SimulateFailure;

            AddLog(_catalog.SimulateFailure
                ? "SectionCatalog.SimulateFailure = true — the next page the tab surface creates will throw"
                : "SectionCatalog.SimulateFailure = false — pages can be created again (recovery)");

            SetStatus(_catalog.SimulateFailure
                ? "Page failures are being simulated — open a tab that is not loaded yet."
                : "Page failures are no longer simulated.", _catalog.SimulateFailure ? StatusLevel.Warning : StatusLevel.Ok);
        }

        // ------------------------------------------------------------------------------------------------------------
        // Responsive profile — the whole narrow layout, three properties (see docs/LayoutNotes.md §4)
        // ------------------------------------------------------------------------------------------------------------

        private void Application_ResponsiveProfileChanged(object sender, ResponsiveProfileChangedEventArgs e)
        {
            AddLog("profile changed → " + ActiveProfileName + " (" + Application.Browser.Size.Width + " px wide)");
            UpdateProfile("Application.ResponsiveProfileChanged");
        }

        /// <summary>Decides whether the shell is narrow — the browser profile, or the reviewer's Force narrow toggle.</summary>
        private void UpdateProfile(string source)
        {
            var narrow = _forceNarrow || IsNarrowProfile(ActiveProfileName);
            ApplyNarrowProfile(narrow);
            AddLog(source + " → ApplyNarrowProfile(" + (narrow ? "true" : "false") + ")");
        }

        private static bool IsNarrowProfile(string profileName)
            => profileName == "Phone" || profileName == "Tablet";

        /// <summary>
        /// The narrow layout: the navigation panel collapses (it is not destroyed), the ToolBar shows the way back,
        /// and the StatusBar names the profile. The desktop layout itself is never rebuilt.
        /// </summary>
        private void ApplyNarrowProfile(bool narrow)
        {
            _narrow = narrow;

            splitMain.Panel1Collapsed = narrow;      // 1 · collapse, do not rebuild
            btnMobileMenu.Visible = narrow;          // 2 · the way back
            sepMobileMenu.Visible = narrow;
            CompactStatusBar(narrow);                // 3 · column display: the diagnostics stop reserving room
            UpdateProfilePanel();                    // 4 · say which layout the user is looking at
        }

        /// <summary>
        /// The StatusBar keeps every panel on a narrow screen, but the two diagnostic panels stop reserving a
        /// minimum width: with <see cref="StatusBarPanelAutoSize.Contents"/> they then take only what their text
        /// needs. That is the "column display" property the module's reading lists as a narrow-profile change —
        /// no panel is removed, so nothing has to be rebuilt when the browser widens again.
        /// </summary>
        private void CompactStatusBar(bool narrow)
        {
            pnlControl.MinWidth = narrow ? 0 : 150;
            pnlRecord.MinWidth = narrow ? 0 : 115;
            pnlStatus.MinWidth = narrow ? 120 : 240;
        }

        private void UpdateProfilePanel()
        {
            // statusBar.Panels["pnlProfile"] would find the same panel by name; the designer field is type-safe.
            pnlProfile.Text = (_narrow ? "Narrow profile · " : "Desktop profile · ") + ActiveProfileName
                + (_forceNarrow ? " (forced)" : "");
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
        // IConsoleShell — the contract sections and services use (Shell/IConsoleShell.cs, unchanged since Module 1)
        // ------------------------------------------------------------------------------------------------------------

        public void AddLog(string message)
        {
            lstEventLog.Items.Add(DateTime.Now.ToString("HH:mm:ss") + "  " + message);
            lstEventLog.SelectedIndex = lstEventLog.Items.Count - 1;
        }

        /// <summary>
        /// From Module 3 the status area is a <c>StatusBar</c> panel. <see cref="StatusBarPanel"/> has no
        /// <c>ForeColor</c>, so the level is expressed with <c>AllowHtml</c> and one coloured span; the text
        /// itself is HTML-encoded, never injected raw.
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
