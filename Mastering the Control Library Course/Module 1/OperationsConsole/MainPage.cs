using System;
using System.Drawing;
using OperationsConsole.Models;
using OperationsConsole.Services;
using OperationsConsole.Shell;
using Wisej.Web;

namespace OperationsConsole
{
    /// <summary>
    /// The Operations Console shell: navigationPanel (Left), commandPanel (Top), statusPanel (Bottom) and
    /// contentPanel (Fill, added last). Every navigation button routes through <see cref="Navigate"/>.
    /// </summary>
    public partial class MainPage : Page, IConsoleShell
    {
        private readonly SectionCatalog _catalog = new SectionCatalog();
        private Control _currentPage;

        public MainPage()
        {
            InitializeComponent();

            Application.ResponsiveProfileChanged += Application_ResponsiveProfileChanged;

            SetStatus("Ready", StatusLevel.Ok);
            UpdateProfileLabel();
        }

        // ------------------------------------------------------------------------------------------------------------
        // Navigation
        // ------------------------------------------------------------------------------------------------------------

        private void editorsButton_Click(object sender, EventArgs e) => OpenSection(SectionKey.Editors, (Control)sender);
        private void layoutsButton_Click(object sender, EventArgs e) => OpenSection(SectionKey.Layouts, (Control)sender);
        private void listsTreesButton_Click(object sender, EventArgs e) => OpenSection(SectionKey.ListsTrees, (Control)sender);
        private void gridButton_Click(object sender, EventArgs e) => OpenSection(SectionKey.DataGridView, (Control)sender);
        private void dashboardButton_Click(object sender, EventArgs e) => OpenSection(SectionKey.Dashboard, (Control)sender);
        private void widgetsButton_Click(object sender, EventArgs e) => OpenSection(SectionKey.Widgets, (Control)sender);

        /// <summary>
        /// Creates the section page and navigates to it. A page that cannot be created keeps the previous page on
        /// screen, turns the status area red and tells the user in plain words.
        /// </summary>
        private void OpenSection(SectionKey key, Control source)
        {
            var info = _catalog.Get(key);
            SetSelectedControl(source.Name);

            try
            {
                Navigate(_catalog.CreatePage(key), info.Title);
            }
            catch (Exception)
            {
                SetStatus("Could not open " + info.Title + " — the previous page is still shown.", StatusLevel.Error);
                AlertBox.Show(
                    "The " + info.Title + " section could not be opened. Please try again; if it keeps failing, contact support.",
                    MessageBoxIcon.Error, alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);
            }
        }

        /// <summary>Clears the content area, docks the page to Fill, adds it and updates the status area.</summary>
        private void Navigate(Control content, string title)
        {
            var previous = _currentPage;

            contentPanel.Controls.Clear();
            content.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(content);
            _currentPage = content;

            previous?.Dispose();

            SetStatus("Showing " + title, StatusLevel.Ok);
            SetSelectedRecord(null);
        }

        // ------------------------------------------------------------------------------------------------------------
        // Command area
        // ------------------------------------------------------------------------------------------------------------

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            SetSelectedControl(btnRefresh.Name);

            if (_currentPage is ISection section)
            {
                section.RefreshSection();
                lblLastRefresh.Text = "Refreshed: " + DateTime.Now.ToString("HH:mm:ss");
                SetStatus(section.Title + " refreshed at " + DateTime.Now.ToString("HH:mm:ss"), StatusLevel.Ok);
            }
            else
            {
                SetStatus("Nothing to refresh — open a section first.", StatusLevel.Warning);
            }
        }

        private void chkSimulateFailure_CheckedChanged(object sender, EventArgs e)
        {
            _catalog.SimulateFailure = chkSimulateFailure.Checked;
        }

        // ------------------------------------------------------------------------------------------------------------
        // Responsive profile
        // ------------------------------------------------------------------------------------------------------------

        private void Application_ResponsiveProfileChanged(object sender, ResponsiveProfileChangedEventArgs e)
        {
            UpdateProfileLabel();
        }

        private void UpdateProfileLabel()
        {
            lblActiveProfile.Text = "Profile: " + Application.ActiveProfile.Name;
        }

        // ------------------------------------------------------------------------------------------------------------
        // IConsoleShell
        // ------------------------------------------------------------------------------------------------------------

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
