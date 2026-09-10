using System;
using System.Drawing;
using OperationsConsole.Models;
using OperationsConsole.Services;
using OperationsConsole.Shell;
using Wisej.Web;

namespace OperationsConsole
{
    /// <summary>
    /// The Operations Console shell (Module 1 · Control Library Mental Model).
    /// Four docked areas — navigationPanel (Left), commandPanel (Top), statusPanel (Bottom), contentPanel (Fill, added last) —
    /// plus the Event log card. Every navigation button routes through the single <see cref="Navigate"/> method,
    /// and the shell is the only implementation of <see cref="IConsoleShell"/>: sections talk to it through <see cref="ConsoleLog"/>.
    /// </summary>
    public partial class MainPage : Page, IConsoleShell
    {
        private readonly SectionCatalog _catalog = new SectionCatalog();
        private Control _currentPage;

        public MainPage()
        {
            InitializeComponent();

            Application.ResponsiveProfileChanged += Application_ResponsiveProfileChanged;

            ShowReadyState();
        }

        /// <summary>The initial "Ready" state the lab asks for: honest status, empty content area, diagnostics filled in.</summary>
        private void ShowReadyState()
        {
            SetStatus("Ready — choose a section on the left.", StatusLevel.Ok);
            SetSelectedControl(null);
            SetSelectedRecord(null);
            UpdateProfileLabel();
            AddLog("Operations Console shell ready · " + _catalog.Sections.Count + " sections in the catalog");
            AddLog("dock order: commandPanel Top, statusPanel Bottom, then navigationPanel Left + pnlEventLog Right, contentPanel Fill takes what is left");
        }

        // ------------------------------------------------------------------------------------------------------------
        // Navigation: one handler per button, each one a single call into OpenSection → Navigate.
        // ------------------------------------------------------------------------------------------------------------

        private void editorsButton_Click(object sender, EventArgs e) => OpenSection(SectionKey.Editors, (Control)sender);
        private void layoutsButton_Click(object sender, EventArgs e) => OpenSection(SectionKey.Layouts, (Control)sender);
        private void listsTreesButton_Click(object sender, EventArgs e) => OpenSection(SectionKey.ListsTrees, (Control)sender);
        private void gridButton_Click(object sender, EventArgs e) => OpenSection(SectionKey.DataGridView, (Control)sender);
        private void dashboardButton_Click(object sender, EventArgs e) => OpenSection(SectionKey.Dashboard, (Control)sender);
        private void widgetsButton_Click(object sender, EventArgs e) => OpenSection(SectionKey.Widgets, (Control)sender);

        /// <summary>
        /// Asks the catalog for the section page and navigates to it. The failure path (a page that cannot be created)
        /// is caught here: the status area turns red and stays honest, an AlertBox tells the user what happened in plain
        /// words, the previous page stays on screen, and the exception details go to the Event log only.
        /// </summary>
        private void OpenSection(SectionKey key, Control source)
        {
            var info = _catalog.Get(key);
            SetSelectedControl(source.Name);
            AddLog(source.Name + " → Navigate(" + info.PageTypeName + ", \"" + info.Title + "\")");

            try
            {
                var page = _catalog.CreatePage(key);
                Navigate(page, info.Title);
            }
            catch (Exception ex)
            {
                AddLog("✗ " + info.PageTypeName + " could not be created — " + ex.GetType().Name);
                AddLog("   " + ex.Message);
                SetStatus("The " + info.Title + " section could not be opened — the previous page is still shown.", StatusLevel.Error);
                AlertBox.Show(
                    "The " + info.Title + " section could not be opened. Please try again; if it keeps failing, contact support.",
                    MessageBoxIcon.Error, alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);

                // one-shot simulation: the recovery is simply clicking the section again
                if (chkSimulateFailure.Checked)
                    chkSimulateFailure.Checked = false;
            }
        }

        /// <summary>
        /// The one navigation method of the shell — the Navigate(Control content, string title) the lab asks for:
        /// clears the content area, docks the page to Fill, adds it and updates the status area.
        /// </summary>
        private void Navigate(Control content, string title)
        {
            var previous = _currentPage;

            contentPanel.Controls.Clear();
            content.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(content);
            _currentPage = content;

            previous?.Dispose();   // the old page is a server object too — release it, do not just hide it

            SetStatus("Showing " + title, StatusLevel.Ok);
            SetSelectedRecord(null);
            AddLog("✓ showing " + title + " (" + content.GetType().Name + ", Dock = Fill)");
        }

        // ------------------------------------------------------------------------------------------------------------
        // Command area
        // ------------------------------------------------------------------------------------------------------------

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            SetSelectedControl(btnRefresh.Name);

            if (_currentPage is ISection section)
            {
                AddLog("btnRefresh → " + _currentPage.GetType().Name + ".RefreshSection()");
                section.RefreshSection();
                lblLastRefresh.Text = "Refreshed: " + DateTime.Now.ToString("HH:mm:ss");
                SetStatus(section.Title + " refreshed at " + DateTime.Now.ToString("HH:mm:ss"), StatusLevel.Ok);
            }
            else
            {
                AddLog("btnRefresh → nothing to refresh (no section open)");
                SetStatus("Nothing to refresh — open a section first.", StatusLevel.Warning);
            }
        }

        private void chkSimulateFailure_CheckedChanged(object sender, EventArgs e)
        {
            _catalog.SimulateFailure = chkSimulateFailure.Checked;
            AddLog(chkSimulateFailure.Checked
                ? "SectionCatalog.SimulateFailure = true — the next navigation will throw inside the page factory"
                : "SectionCatalog.SimulateFailure = false — navigation works again (recovery)");
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            lstEventLog.Items.Clear();
        }

        // ------------------------------------------------------------------------------------------------------------
        // Responsive profile (diagnostic panel)
        // ------------------------------------------------------------------------------------------------------------

        private void Application_ResponsiveProfileChanged(object sender, ResponsiveProfileChangedEventArgs e)
        {
            UpdateProfileLabel();
            AddLog("profile changed → " + Application.ActiveProfile.Name + " (" + Application.Browser.Size.Width + " px wide)");
        }

        private void UpdateProfileLabel()
        {
            lblActiveProfile.Text = "Profile: " + Application.ActiveProfile.Name;
        }

        // ------------------------------------------------------------------------------------------------------------
        // IConsoleShell — the contract sections and services use (see Shell/IConsoleShell.cs)
        // ------------------------------------------------------------------------------------------------------------

        public void AddLog(string message)
        {
            lstEventLog.Items.Add(DateTime.Now.ToString("HH:mm:ss") + "  " + message);
            lstEventLog.SelectedIndex = lstEventLog.Items.Count - 1;
        }

        public void SetStatus(string text, StatusLevel level)
        {
            statusLabel.Text = text;
            switch (level)
            {
                case StatusLevel.Warning: statusLabel.ForeColor = Color.FromArgb(232, 161, 60); break;
                case StatusLevel.Error:   statusLabel.ForeColor = Color.FromArgb(224, 86, 59);  break;
                default:                  statusLabel.ForeColor = Color.FromArgb(31, 157, 87);  break;
            }
        }

        public void SetSelectedControl(string controlName)
        {
            lblSelectedControl.Text = "Control: " + (string.IsNullOrEmpty(controlName) ? "—" : controlName);
        }

        public void SetSelectedRecord(string recordId)
        {
            lblSelectedRecord.Text = "Record: " + (string.IsNullOrEmpty(recordId) ? "—" : recordId);
        }
    }
}
