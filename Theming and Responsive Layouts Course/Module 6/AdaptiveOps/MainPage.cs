using System;
using System.Collections.Generic;
using System.Globalization;
using AdaptiveOps.Dialogs;
using AdaptiveOps.Models;
using AdaptiveOps.Shell;
using Wisej.Web;

namespace AdaptiveOps
{
    /// <summary>
    /// Adaptive Operations Console with client profiles. ClientProfiles.json (project root) defines six
    /// profiles, narrow to broad; the framework matches them into Application.ActiveProfile. ApplyProfile runs
    /// at startup and on every Application.ResponsiveProfileChanged: it shows the profile in the status bar,
    /// logs the change, and adapts the console (rail and details hidden on Phone with the editor opened as a
    /// modal form, details docked under the grid on Tablet, icon-only toolbar below Desktop).
    ///
    /// In Visual Studio the per-profile Visible / Display / Dock values would be set in the Designer's
    /// responsive-profile dropdown; this hand-written sample assigns them in ApplyProfile.
    /// </summary>
    public partial class MainPage : Page
    {
        private readonly TicketRepository _repository = new TicketRepository();

        private string _selectedId;
        private bool _suppressSelection;

        /// <summary>The profile the console is laid out for ("Desktop", "Phone", …).</summary>
        private string _profileName;

        /// <summary>The reusable phone dialog; created on first use, disposed with the page.</summary>
        private TicketEditorForm _editorForm;

        /// <summary>Desktop widths of the toolbar buttons, so icon-only mode can be undone.</summary>
        private readonly Dictionary<Button, int> _toolbarWidths = new Dictionary<Button, int>();

        public MainPage()
        {
            InitializeComponent();

            foreach (var b in ToolbarButtons())
                _toolbarWidths[b] = b.Width;

            // Session-level events: unsubscribed in Dispose(bool) because Application outlives the page.
            Application.BrowserSizeChanged += this.Application_BrowserSizeChanged;
            Application.ResponsiveProfileChanged += this.Application_ResponsiveProfileChanged;

            ApplyProfile(Application.ActiveProfile?.Name);
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            LoadTickets();
            ShowProfile();
            SetStatus("Ready");
        }

        private void Application_BrowserSizeChanged(object sender, EventArgs e)
        {
            ShowProfile();
        }

        private void Application_ResponsiveProfileChanged(object sender, ResponsiveProfileChangedEventArgs e)
        {
            ApplyProfile(e.CurrentProfile?.Name);
        }

        #region Profiles

        /// <summary>
        /// Lays the console out for a client profile. Every value is assigned (never toggled), so applying the
        /// same profile again changes nothing; the phone dialog is opened only if it is not open already.
        /// </summary>
        private void ApplyProfile(string profileName)
        {
            string name = string.IsNullOrEmpty(profileName) || profileName == "Default" ? "Desktop" : profileName;
            bool phone = name.StartsWith("Phone", StringComparison.Ordinal);
            bool tablet = name.StartsWith("Tablet", StringComparison.Ordinal);
            bool iconOnly = name != "Desktop";

            // Toolbar: icon-only below Desktop; the Menu button replaces the rail on a phone.
            foreach (var b in ToolbarButtons())
            {
                b.Display = iconOnly ? Display.Icon : Display.Both;
                b.Width = iconOnly ? 36 : _toolbarWidths[b];
            }
            this.btnMenu.Visible = phone;
            this.lblAppTitle.Text = phone ? "AdaptiveOps" : "Adaptive Operations Console";
            this.lblAppTitle.Width = phone ? 120 : 250;

            // Navigation: hidden on Phone, icon-only on Tablet, full otherwise.
            this.navigationPanel.Visible = !phone;
            if (!phone)
            {
                var mode = tablet ? RailMode.IconOnly : RailMode.Full;
                this.navigationRail.SetMode(mode);
                this.navigationPanel.Width = NavigationRail.WidthFor(mode);
            }

            // Details: a modal form on Phone, docked under the grid on Tablet, docked right elsewhere.
            if (phone)
            {
                this.detailsPanel.Visible = false;
                HostEditorInDialog();
                if (_selectedId != null)
                    OpenEditorDialog();
            }
            else
            {
                HostEditorInPanel();
                if (tablet)
                {
                    this.detailsPanel.Dock = DockStyle.Bottom;
                    this.detailsPanel.Padding = new Padding(8, 4, 8, 4);
                    this.detailsPanel.Height = 320;
                }
                else
                {
                    this.detailsPanel.Dock = DockStyle.Right;
                    this.detailsPanel.Padding = new Padding(0, 4, 8, 4);
                    this.detailsPanel.Width = name == "Desktop" ? 340 : 300;
                }
                this.detailsPanel.Visible = true;
            }

            // Metrics and grid: two cards per row and fewer columns on a phone.
            LayoutMetrics(phone ? 2 : 4);
            this.colOwner.Visible = !phone;
            this.colDue.Visible = !phone;

            if (name != _profileName)
                AddLog(_profileName == null ? $"Startup: {name}" : $"{_profileName} → {name}");

            _profileName = name;
            ShowProfile();
        }

        /// <summary>Writes "Profile: Tablet · 980 px" into the status bar.</summary>
        private void ShowProfile()
        {
            string width;
            try
            {
                int w = Application.Browser.Size.Width;
                width = w > 0 ? $"{w} px" : "width unavailable";
            }
            catch (Exception)
            {
                width = "width unavailable";
            }

            this.lblProfile.Text = $"Profile: {_profileName} · {width}";
        }

        private IEnumerable<Button> ToolbarButtons()
        {
            yield return this.btnRefresh;
            yield return this.btnReapply;
            yield return this.btnClearTrace;
        }

        /// <summary>Re-arranges the four metric cards in the TableLayoutPanel: columns × rows and the cell positions.</summary>
        private void LayoutMetrics(int columns)
        {
            var cards = new[] { this.cardOpen, this.cardOverdue, this.cardMine, this.cardClosed };
            int rows = cards.Length / columns;
            var table = this.metricsTable;

            table.SuspendLayout();
            try
            {
                table.ColumnCount = columns;
                table.RowCount = rows;

                table.ColumnStyles.Clear();
                for (int c = 0; c < columns; c++)
                    table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / columns));

                table.RowStyles.Clear();
                for (int r = 0; r < rows; r++)
                    table.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / rows));

                for (int i = 0; i < cards.Length; i++)
                    table.SetCellPosition(cards[i], new TableLayoutPanelCellPosition(i % columns, i / columns));

                table.Height = rows * 84;
            }
            finally
            {
                table.ResumeLayout(true);
            }
        }

        private void btnReapply_Click(object sender, EventArgs e)
        {
            ApplyProfile(Application.ActiveProfile?.Name);
            SetStatus("Profile re-applied");
        }

        #endregion

        #region Details editor hosting: docked panel or phone dialog

        /// <summary>Moves the editor into the docked details panel (no-op when it is already there).</summary>
        private void HostEditorInPanel()
        {
            if (this.ticketEditor.Parent == this.detailsCard)
                return;

            if (_editorForm != null && _editorForm.Visible)
                _editorForm.Close();

            this.ticketEditor.Parent?.Controls.Remove(this.ticketEditor);
            this.detailsCard.Controls.Add(this.ticketEditor);
            this.ticketEditor.Dock = DockStyle.Fill;
            this.ticketEditor.CloseButtonVisible = false;
        }

        /// <summary>Moves the editor into the reusable dialog (no-op when it is already there). Does not open it.</summary>
        private void HostEditorInDialog()
        {
            if (_editorForm == null || _editorForm.IsDisposed)
                _editorForm = new TicketEditorForm();

            if (this.ticketEditor.Parent == _editorForm)
                return;

            _editorForm.HostEditor(this.ticketEditor);
            this.ticketEditor.CloseButtonVisible = true;
        }

        /// <summary>Opens the dialog once; an editor that is already open is reused.</summary>
        private void OpenEditorDialog()
        {
            HostEditorInDialog();
            if (_editorForm.Visible)
                return;

            _editorForm.WindowState = _profileName == "Phone" ? FormWindowState.Maximized : FormWindowState.Normal;
            _editorForm.ShowDialog((form, result) => { });
        }

        private void ticketEditor_CloseRequested(object sender, EventArgs e)
        {
            if (_editorForm != null && _editorForm.Visible)
                _editorForm.Close();
        }

        #endregion

        #region Tickets

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadTickets();
            SetStatus("Ready");
        }

        private void LoadTickets()
        {
            var today = DateTime.Today;
            var tickets = _repository.GetAll();

            this.lblOpenValue.Text = N(_repository.CountOpen());
            this.lblOverdueValue.Text = N(_repository.CountOverdue(today));
            this.lblMineValue.Text = N(_repository.CountAssignedToMe());
            this.lblClosedValue.Text = N(_repository.CountClosedThisWeek(today));

            string keep = _selectedId;
            int reselect = -1;

            _suppressSelection = true;
            try
            {
                this.gridTickets.Rows.Clear();
                foreach (var t in tickets)
                {
                    int index = this.gridTickets.Rows.Add(new object[]
                    {
                        t.Id,
                        t.Title,
                        t.Priority.ToString(),
                        t.Status.ToString(),
                        t.Owner,
                        t.DueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                    });
                    this.gridTickets.Rows[index].Tag = t.Id;
                    if (t.Id == keep)
                        reselect = index;
                }

                if (reselect < 0 && this.gridTickets.Rows.Count > 0)
                    reselect = 0;

                this.gridTickets.ClearSelection();
                if (reselect >= 0)
                    this.gridTickets.Rows[reselect].Selected = true;
            }
            finally
            {
                _suppressSelection = false;
            }

            if (reselect >= 0)
                ShowTicket((string)this.gridTickets.Rows[reselect].Tag, userInitiated: false);
            else
                ClearEditor();
        }

        private void gridTickets_SelectionChanged(object sender, EventArgs e)
        {
            if (_suppressSelection)
                return;

            var row = this.gridTickets.SelectedRows.Count > 0
                ? this.gridTickets.SelectedRows[0]
                : this.gridTickets.CurrentRow;

            if (row == null || row.Tag == null)
            {
                ClearEditor();
                return;
            }

            ShowTicket((string)row.Tag, userInitiated: true);
        }

        /// <summary>Fills the editor; on a phone, a row the user selects opens the editor dialog.</summary>
        private void ShowTicket(string id, bool userInitiated)
        {
            var t = _repository.Get(id);
            if (t == null)
            {
                ClearEditor();
                return;
            }

            _selectedId = t.Id;
            this.ticketEditor.LoadTicket(t);

            bool phone = _profileName != null && _profileName.StartsWith("Phone", StringComparison.Ordinal);
            if (phone && userInitiated)
                OpenEditorDialog();
        }

        private void ClearEditor()
        {
            _selectedId = null;
            this.ticketEditor.Clear();
        }

        /// <summary>Saves through the repository; the outcome is shown inside the editor, visible in the phone dialog too.</summary>
        private void ticketEditor_SaveRequested(object sender, EventArgs e)
        {
            if (_selectedId == null)
            {
                this.ticketEditor.ShowMessage("Select a ticket in the grid before saving.", true);
                return;
            }

            var ticket = this.ticketEditor.ReadTicket();
            try
            {
                var saved = _repository.Save(ticket);
                LoadTickets();
                this.ticketEditor.ShowMessage($"{saved.Id} saved.", false);
                SetStatus($"Saved {saved.Id}");
                AlertBox.Show($"{saved.Id} saved.", MessageBoxIcon.Information,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            }
            catch (TicketValidationException ex)
            {
                this.ticketEditor.ShowMessage(ex.Message + " Nothing was written.", true);
                SetStatus("Not saved: " + ex.Message);
            }
        }

        #endregion

        #region Navigation rail and the phone menu

        private void navigationRail_SectionSelected(object sender, SectionEventArgs e)
        {
            this.lblWorkspaceTitle.Text = e.Section == "Dashboard" ? "Tickets" : e.Section;
        }

        /// <summary>On a phone the rail is hidden; the Menu button offers the same sections.</summary>
        private void btnMenu_Click(object sender, EventArgs e)
        {
            var menu = new ContextMenu();
            foreach (var section in this.navigationRail.Sections)
            {
                string name = section;
                menu.MenuItems.Add(new MenuItem(name, (s, a) => this.navigationRail.Select(name)));
            }

            menu.Show(this.btnMenu, Placement.BottomLeft, m => m.Dispose());
        }

        #endregion

        #region Profile log and status

        private void AddLog(string line)
        {
            string time = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            this.lstProfileLog.Items.Add($"{time}  {line}");
            this.lstProfileLog.SelectedIndex = this.lstProfileLog.Items.Count - 1;
        }

        private void btnClearTrace_Click(object sender, EventArgs e)
        {
            this.lstProfileLog.Items.Clear();
        }

        private void SetStatus(string text)
        {
            this.lblStatus.Text = text;
        }

        private static string N(int value) => value.ToString(CultureInfo.InvariantCulture);

        #endregion
    }
}
