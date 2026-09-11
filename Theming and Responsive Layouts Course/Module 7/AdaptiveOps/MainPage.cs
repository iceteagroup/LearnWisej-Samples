using System;
using System.Globalization;
using AdaptiveOps.Dialogs;
using AdaptiveOps.Models;
using Wisej.Web;

namespace AdaptiveOps
{
    /// <summary>
    /// Adaptive Operations Console, the capstone: the AdaptiveOps theme with semantic appearances
    /// (action-button, nav-item, metric-card, compact-editor …), the scoped stylesheet, a shell of docked
    /// regions with Flow (toolbar, metric cards), Table (details editor) and Flex (grid) containers, and
    /// ClientProfiles.json applied by one idempotent ApplyProfile. On phone profiles the details editor opens
    /// as a modal DetailsDialog. The profile indicator in the status bar is shown in test mode only.
    /// </summary>
    public partial class MainPage : Page
    {
        /// <summary>Test mode shows the profile indicator. ADAPTIVEOPS_TESTMODE=0 hides it; unset means on.</summary>
        private static readonly bool TestMode = ReadTestMode();

        private readonly TicketRepository _repository = new TicketRepository();

        private string _selectedId;
        private bool _suppressSelection;

        /// <summary>The profile ApplyProfile last applied successfully.</summary>
        private string _lastProfile;

        /// <summary>Guards the re-apply that runs after a failed ApplyProfile against recursion.</summary>
        private bool _recoveringProfile;

        public MainPage()
        {
            InitializeComponent();

            this.lblProfile.Visible = TestMode;

            // Session-level events: both are unsubscribed in Dispose(bool) (MainPage.Designer.cs).
            Application.BrowserSizeChanged += this.Application_BrowserSizeChanged;
            Application.ResponsiveProfileChanged += this.Application_ResponsiveProfileChanged;

            ApplyProfile(ProfileNameOf(Application.ActiveProfile));
            ReportWidth();
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            LoadTickets();
            ReportWidth();
            SetStatus("Ready");
        }

        private void Application_BrowserSizeChanged(object sender, EventArgs e)
        {
            ReportWidth();
        }

        private void Application_ResponsiveProfileChanged(object sender, ResponsiveProfileChangedEventArgs e)
        {
            ApplyProfile(ProfileNameOf(e.CurrentProfile));
            ReportWidth();
        }

        private void ReportWidth()
        {
            try
            {
                int width = Application.Browser.Size.Width;
                if (width <= 0)
                    throw new InvalidOperationException("The browser has not reported its size yet.");

                this.widthLabel.Text = $"Width: {width} px";
            }
            catch (Exception)
            {
                this.widthLabel.Text = "Width: unavailable";
            }
        }

        private static bool ReadTestMode()
        {
            string value = Environment.GetEnvironmentVariable("ADAPTIVEOPS_TESTMODE");
            return string.IsNullOrEmpty(value) || value != "0";
        }

        #region Profiles: one idempotent ApplyProfile

        /// <summary>"Desktop" for the framework's default profile, otherwise the profile's own name.</summary>
        private static string ProfileNameOf(Wisej.Core.ClientProfile profile)
        {
            if (profile == null || string.IsNullOrEmpty(profile.Name) || profile.Name == "Default")
                return "Desktop";
            return profile.Name;
        }

        /// <summary>
        /// Assigns the final layout values for a profile. Returns early when the profile has not changed and
        /// sets final values instead of toggling, so a repeated event changes nothing. A failure half-way is
        /// reported in the status region and undone by re-applying the last good profile.
        /// </summary>
        private void ApplyProfile(string name, bool force = false)
        {
            name = string.IsNullOrEmpty(name) ? "Desktop" : name;
            if (!force && string.Equals(name, _lastProfile, StringComparison.Ordinal))
                return;

            bool phone = name.StartsWith("Phone", StringComparison.Ordinal);
            bool phoneLandscape = name == "Phone (Landscape)";
            bool tabletPortrait = name == "Tablet";
            bool narrow = phone || name.StartsWith("Tablet", StringComparison.Ordinal) || name == "Small Desktop";
            bool desktop = !narrow;
            bool iconToolbar = phone || tabletPortrait;

            try
            {
                this.SuspendLayout();
                try
                {
                    // Navigation rail: text on desktop, icon-only on every narrow profile.
                    this.navRail.IconOnly = narrow;
                    this.navigationPanel.Width = desktop ? 220 : (phone ? 60 : 68);

                    // Details region: docked right, docked under the grid on a portrait tablet, hidden on a
                    // phone where "Open details" opens the editor as a dialog instead.
                    this.detailsPanel.Visible = !phone;
                    if (tabletPortrait)
                    {
                        this.detailsPanel.Dock = DockStyle.Bottom;
                        this.detailsPanel.MinimumSize = new System.Drawing.Size(0, 240);
                        this.detailsPanel.MaximumSize = new System.Drawing.Size(0, 360);
                        this.detailsPanel.Height = 300;
                        this.detailsPanel.Padding = new Padding(8, 4, 8, 4);
                    }
                    else
                    {
                        this.detailsPanel.Dock = DockStyle.Right;
                        this.detailsPanel.MinimumSize = new System.Drawing.Size(280, 0);
                        this.detailsPanel.MaximumSize = new System.Drawing.Size(420, 0);
                        this.detailsPanel.Width = desktop ? 340 : 300;
                        this.detailsPanel.Padding = new Padding(0, 4, 8, 4);
                    }
                    this.detailsEditor.Compact = narrow;
                    this.btnOpenDetails.Visible = phone;

                    // Metric cards: the FlowLayoutPanel wraps; the profile sets the row budget (one or two rows).
                    this.metricsFlow.Height = (desktop || phoneLandscape) ? 92 : 184;

                    // Grid: stacked vertically on a portrait phone.
                    this.contentFlex.LayoutStyle = (phone && !phoneLandscape) ? FlexLayoutStyle.Vertical : FlexLayoutStyle.Horizontal;

                    // Grid columns: fewer on narrow profiles.
                    this.colPriority.Visible = !phone;
                    this.colOwner.Visible = desktop;
                    this.colDue.Visible = !phone;

                    // Toolbar: icon-only on phone and portrait tablet; ToolTipText and AccessibleName stay.
                    this.btnRefresh.Display = iconToolbar ? Display.Icon : Display.Both;
                    this.btnRefresh.Width = iconToolbar ? 40 : 110;
                    this.lblAppTitle.Text = phone ? "Adaptive Ops" : "Adaptive Operations Console";
                    this.lblAppTitle.Width = phone ? 120 : 236;

                    this.workspacePanel.MinimumSize = phone ? new System.Drawing.Size(240, 200) : new System.Drawing.Size(320, 240);
                }
                finally
                {
                    this.ResumeLayout(true);
                }

                _lastProfile = name;
                this.lblProfile.Text = "Profile: " + name;
            }
            catch (Exception)
            {
                // Friendly failure: say so in the status region, then put the shell back on the last good
                // profile so nothing stays half-applied. _lastProfile is not advanced, so the next event retries.
                SetStatus($"Could not apply the {name} layout; kept {_lastProfile ?? "the startup layout"}.");

                if (_lastProfile != null && !_recoveringProfile)
                {
                    _recoveringProfile = true;
                    try { ApplyProfile(_lastProfile, force: true); }
                    finally { _recoveringProfile = false; }
                }
            }
        }

        #endregion

        #region Tickets: grid, editor, dialog, save

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadTickets();
            SetStatus("Ready");
        }

        private void LoadTickets()
        {
            var today = DateTime.Today;
            var tickets = _repository.GetAll();

            this.cardOpen.SetValue(_repository.CountOpen());
            this.cardOverdue.SetValue(_repository.CountOverdue(today));
            this.cardMine.SetValue(_repository.CountAssignedToMe());
            this.cardClosed.SetValue(_repository.CountClosedThisWeek(today));

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
                ShowTicket((string)this.gridTickets.Rows[reselect].Tag);
            else
            {
                _selectedId = null;
                this.detailsEditor.Clear();
            }
        }

        private void gridTickets_SelectionChanged(object sender, EventArgs e)
        {
            if (_suppressSelection)
                return;

            var row = this.gridTickets.SelectedRows.Count > 0 ? this.gridTickets.SelectedRows[0] : this.gridTickets.CurrentRow;
            if (row == null || row.Tag == null)
            {
                _selectedId = null;
                this.detailsEditor.Clear();
                return;
            }

            ShowTicket((string)row.Tag);
        }

        private void ShowTicket(string id)
        {
            var t = _repository.Get(id);
            if (t == null)
            {
                _selectedId = null;
                this.detailsEditor.Clear();
                return;
            }

            _selectedId = t.Id;
            this.detailsEditor.ShowTicket(t);
        }

        /// <summary>Phone profiles: the editor opens as a modal dialog, behaviour a property value cannot express.</summary>
        private void btnOpenDetails_Click(object sender, EventArgs e)
        {
            if (_selectedId == null)
            {
                SetStatus("Select a ticket first.");
                return;
            }

            bool maximized = _lastProfile != null && _lastProfile.StartsWith("Phone", StringComparison.Ordinal);
            var dialog = new DetailsDialog(ticket => TrySave(ticket), maximized);
            dialog.Editor.ShowTicket(_repository.Get(_selectedId));
            dialog.ShowDialog((form, result) => form.Dispose());
        }

        private void detailsEditor_SaveRequested(object sender, Ticket ticket)
        {
            var error = TrySave(ticket);
            if (error != null)
                this.detailsEditor.ShowValidationError(error);
        }

        /// <summary>
        /// The only write path. TicketRepository.Save validates on the server and throws
        /// TicketValidationException; the caller shows it on the right editor and in the validation label.
        /// </summary>
        private TicketValidationException TrySave(Ticket ticket)
        {
            if (ticket == null || ticket.Id == null)
            {
                SetStatus("Select a ticket first.");
                return new TicketValidationException("Select a ticket in the grid before saving.");
            }

            try
            {
                var saved = _repository.Save(ticket);
                this.detailsEditor.ClearValidation();
                LoadTickets();
                SetStatus($"Saved {saved.Id}");
                AlertBox.Show($"{saved.Id} saved.", MessageBoxIcon.Information,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return null;
            }
            catch (TicketValidationException ex)
            {
                SetStatus("Not saved: " + ex.Message);
                return ex;
            }
        }

        #endregion

        private void navRail_Navigated(object sender, string view)
        {
            this.lblWorkspaceTitle.Text = view == "Dashboard" ? "Tickets" : view;
        }

        private void SetStatus(string text)
        {
            this.lblStatus.Text = text;
        }
    }
}
