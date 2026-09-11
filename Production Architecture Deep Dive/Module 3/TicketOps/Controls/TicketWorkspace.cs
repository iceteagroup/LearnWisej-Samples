using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using TicketOps.Services;
using Wisej.Web;

namespace TicketOps.Controls
{
    /// <summary>What the workspace wants the host screen to show in its StatusBanner.</summary>
    public sealed class WorkspaceStatusEventArgs : EventArgs
    {
        public WorkspaceStatusEventArgs(string status, StatusKind kind, string banner)
        {
            Status = status;
            Kind = kind;
            Banner = banner;
        }

        /// <summary>Short state text ("ready", "loading", "not saved").</summary>
        public string Status { get; }
        public StatusKind Kind { get; }
        /// <summary>A sentence for the banner line, or null to hide the banner.</summary>
        public string Banner { get; }
    }

    /// <summary>
    /// TicketOps Console · the responsive Ticket Workspace.
    ///
    /// One UserControl, four regions, one engine per container:
    ///   outer shell   Dock          header Top · toolbar Top · navigation Left · status Bottom · body Fill
    ///   body          FlexLayout    ticket list : side pane = 3 : 2 (1 : 1 on tablet)
    ///   side pane     FlexLayout    detail : activity = 3 : 2 (vertical) — or a TabControl on tablet / phone
    ///   detail form   TableLayout   35 % captions / 65 % fields, Notes row takes the rest
    ///   filter chips  FlowLayout    wrap to a second line when the list gets narrow
    ///   search        SearchBar     the same UserControl twice (tickets, activity)
    ///
    /// The active client profile decides the arrangement: <see cref="ApplyResponsiveProfile"/> toggles
    /// Visible / Dock / Parent on panels that already exist — nothing is rebuilt, so the selected ticket,
    /// the draft in the editor and the search text survive every switch. The service never hears about
    /// profiles: desktop, tablet and phone call the same ITicketService.
    /// </summary>
    public partial class TicketWorkspace : UserControl
    {
        public const string ProfileDesktop = "Desktop";
        public const string ProfileTablet = "Tablet";
        public const string ProfilePhone = "Phone";

        private ITicketService _tickets;
        private ILog _log;
        private bool _subscribed;

        private IReadOnlyList<Ticket> _rows = new List<Ticket>();
        private int? _selectedId;
        private string _activeChip;
        private string _appliedProfile = ProfileDesktop;
        private bool _phoneShowsTask;
        private bool _suppressSelection;

        // The Designer keeps the parameterless constructor; real wiring goes through the other one (or Attach).
        public TicketWorkspace() : this(null, new ActivityLog())
        {
        }

        public TicketWorkspace(ITicketService tickets, ILog log)
        {
            InitializeComponent();

            _log = log ?? new ActivityLog();

            this.cmbPriority.Items.AddRange(new object[] { "Low", "Medium", "High" });
            this.cmbPriority.SelectedIndex = 1;
            this.cmbAssignee.Items.AddRange(new object[] { "(unassigned)", "A. Rivera", "M. Chen", "S. Patel", "J. Kim" });
            this.cmbAssignee.SelectedIndex = 0;

            // Two SearchBars, one control: only the intent differs.
            this.searchTickets.Placeholder = Strings.SearchPlaceholderTickets;
            this.searchActivity.Placeholder = Strings.SearchPlaceholderActivity;
            this.searchActivity.ButtonText = "Filter";

            BuildChips();

            if (tickets != null)
                Attach(tickets, _log);
        }

        #region Public surface (what the host screen talks to)

        /// <summary>Raised whenever the workspace wants the host's StatusBanner updated.</summary>
        public event EventHandler<WorkspaceStatusEventArgs> StatusChanged;

        /// <summary>
        /// Gives a Designer-placed instance its dependencies and starts following the session's responsive
        /// profile. Unsubscribed in Dispose (Designer file).
        /// </summary>
        public void Attach(ITicketService tickets, ILog log)
        {
            _tickets = tickets ?? throw new ArgumentNullException(nameof(tickets));
            _log = log ?? _log;

            if (!_subscribed)
            {
                // Session-level event: fires when the browser crosses a width boundary from ClientProfiles.json.
                Application.ResponsiveProfileChanged += this.Application_ResponsiveProfileChanged;
                _subscribed = true;
            }
        }

        /// <summary>Loads the grid and the activity feed through the service (called once from the host's Load).</summary>
        public Task LoadAsync() => RefreshAsync();

        /// <summary>Applies the layout of the browser's current profile.</summary>
        public void ApplyActiveProfile()
        {
            ApplyResponsiveProfile(Application.ActiveProfile.Name);
        }

        #endregion

        #region Client profile → layout

        private void Application_ResponsiveProfileChanged(object sender, ResponsiveProfileChangedEventArgs e)
        {
            try
            {
                OnResponsiveProfileChanged(sender, e);
            }
            catch (Exception ex)
            {
                ReportFailure("TicketWorkspace.ResponsiveProfileChanged", ex);
            }
        }

        /// <summary>The lab's handler: read the active profile, delegate to one named arrangement.</summary>
        private void OnResponsiveProfileChanged(object sender, EventArgs e)
        {
            ApplyResponsiveProfile(Application.ActiveProfile.Name);
        }

        /// <summary>
        /// Maps a profile name to an arrangement of the SAME panels. Anything longer than a property toggle
        /// lives in a named method.
        /// </summary>
        public void ApplyResponsiveProfile(string profileName)
        {
            switch (profileName)
            {
                case ProfileTablet:
                    CollapseActivityToTab();    // same feed, reachable through a tab to reclaim width
                    break;

                case ProfilePhone:
                    ShowSingleTaskView();       // one pane at a time, big touch targets, a way back
                    break;

                default:
                    // Desktop, or a name the JSON does not define (e.g. the built-in "Small Desktop"):
                    // never leave the screen half-arranged.
                    ShowAllPanels();            // full layout: nav, list, detail and activity at once
                    break;
            }

            _appliedProfile = profileName;
            this.lblStatus.Text = $"Active profile: {profileName}";
        }

        /// <summary>Desktop: everything visible; list and side pane 3 : 2; detail over activity.</summary>
        private void ShowAllPanels()
        {
            this.pnlNavigation.Visible = true;
            SetNavigationCompact(false);
            this.pnlToolbar.Visible = true;
            this.btnBack.Visible = false;
            this.lblTitle.Text = Strings.WorkspaceTitle;

            MovePanelsToFlex();
            this.flexSide.Visible = true;
            this.tabActivity.Visible = false;

            this.pnlList.Visible = true;
            this.pnlSide.Visible = true;
            this.flexBody.SetFillWeight(this.pnlList, 3);
            this.flexBody.SetFillWeight(this.pnlSide, 2);
            this.flexBody.Padding = new Padding(12);

            this.colAssignee.Visible = true;
            this.colStatus.Visible = true;
        }

        /// <summary>Tablet: compact navigation, Assignee column hidden, detail and activity behind tabs.</summary>
        private void CollapseActivityToTab()
        {
            this.pnlNavigation.Visible = true;
            SetNavigationCompact(true);
            this.pnlToolbar.Visible = true;
            this.btnBack.Visible = false;
            this.lblTitle.Text = Strings.WorkspaceTitle;

            MovePanelsToTabs();
            this.flexSide.Visible = false;
            this.tabActivity.Visible = true;

            this.pnlList.Visible = true;
            this.pnlSide.Visible = true;
            this.flexBody.SetFillWeight(this.pnlList, 1);
            this.flexBody.SetFillWeight(this.pnlSide, 1);
            this.flexBody.Padding = new Padding(10);

            this.colAssignee.Visible = false;   // a server-side property changed by the profile
            this.colStatus.Visible = true;
        }

        /// <summary>Phone: navigation and toolbar gone, one pane at a time, a Back button in the header.</summary>
        private void ShowSingleTaskView()
        {
            this.pnlNavigation.Visible = false;
            this.pnlToolbar.Visible = false;

            MovePanelsToTabs();
            this.flexSide.Visible = false;
            this.tabActivity.Visible = true;

            this.flexBody.SetFillWeight(this.pnlList, 1);
            this.flexBody.SetFillWeight(this.pnlSide, 1);
            this.flexBody.Padding = new Padding(6);

            this.colAssignee.Visible = false;
            this.colStatus.Visible = false;

            ShowPhonePage(_phoneShowsTask);
        }

        /// <summary>Phone has two pages made of the same panels: the list, or the task (detail + activity tabs).</summary>
        private void ShowPhonePage(bool task)
        {
            _phoneShowsTask = task;
            this.pnlList.Visible = !task;
            this.pnlSide.Visible = task;
            this.btnBack.Visible = task;
            this.lblTitle.Text = task
                ? (_selectedId.HasValue ? $"Ticket #{_selectedId.Value}" : "New ticket")
                : "Tickets";
        }

        /// <summary>Re-parents the detail and activity panels into the vertical flex (desktop). Same instances.</summary>
        private void MovePanelsToFlex()
        {
            if (this.pnlDetail.Parent == this.flexSide && this.pnlActivity.Parent == this.flexSide)
                return;

            this.pnlDetail.Parent = this.flexSide;
            this.pnlActivity.Parent = this.flexSide;
            this.flexSide.SetFillWeight(this.pnlDetail, 3);
            this.flexSide.SetFillWeight(this.pnlActivity, 2);
        }

        /// <summary>Re-parents the detail and activity panels into the two tab pages (tablet, phone). Same instances.</summary>
        private void MovePanelsToTabs()
        {
            if (this.pnlDetail.Parent == this.tabPageDetails && this.pnlActivity.Parent == this.tabPageActivity)
                return;

            this.pnlDetail.Parent = this.tabPageDetails;
            this.pnlDetail.Dock = DockStyle.Fill;
            this.pnlActivity.Parent = this.tabPageActivity;
            this.pnlActivity.Dock = DockStyle.Fill;
        }

        private void SetNavigationCompact(bool compact)
        {
            this.pnlNavigation.Width = compact ? 56 : 150;
            this.lblNavDashboard.Text = compact ? "▦" : "▦   Dashboard";
            this.lblNavTickets.Text = compact ? "◳" : "◳   Tickets";
            this.lblNavReports.Text = compact ? "▤" : "▤   Reports";
            this.lblNavSettings.Text = compact ? "⚙" : "⚙   Settings";
        }

        #endregion

        #region Data → UI and UI → data

        /// <summary>Reloads the grid (with the current filter) and the activity feed through the service.</summary>
        private async Task RefreshAsync()
        {
            try
            {
                Notify("loading", StatusKind.Busy, null);

                var tickets = await _tickets.SearchAsync(CurrentFilter());
                if (!tickets.Succeeded)
                {
                    ShowResult(tickets);
                    return;
                }
                FillGrid(tickets.Value);

                var events = await _tickets.GetActivityAsync(this.searchActivity.Query);
                if (events.Succeeded)
                    FillActivity(events.Value);

                Notify("ready", StatusKind.Success, null);
            }
            catch (Exception ex)
            {
                ReportFailure("TicketWorkspace.Refresh", ex);
            }
        }

        /// <summary>Runs a ticket search through the service (a query that is too short comes back as a failed result).</summary>
        private async Task SearchTicketsAsync(string query)
        {
            try
            {
                var result = await _tickets.SearchAsync(CurrentFilter(query));
                ShowResult(result);
                if (result.Succeeded)
                    FillGrid(result.Value);
            }
            catch (Exception ex)
            {
                ReportFailure("TicketWorkspace.SearchTickets", ex);
            }
        }

        private TicketFilter CurrentFilter() => CurrentFilter(this.searchTickets.Query);

        private TicketFilter CurrentFilter(string text) => new TicketFilter { Text = text ?? "", Chip = _activeChip };

        /// <summary>data → UI: the only place that fills the grid. Keeps the selection when the row still exists.</summary>
        private void FillGrid(IReadOnlyList<Ticket> rows)
        {
            _rows = rows;
            _suppressSelection = true;
            try
            {
                this.gridTickets.Rows.Clear();
                int selectedIndex = -1;
                for (int i = 0; i < rows.Count; i++)
                {
                    var t = rows[i];
                    this.gridTickets.Rows.Add(t.Id, t.Title, t.Priority.ToString(), t.IsAssigned ? t.Assignee : "—", StatusText(t.Status));
                    if (_selectedId.HasValue && t.Id == _selectedId.Value)
                        selectedIndex = i;
                }

                if (selectedIndex >= 0)
                    this.gridTickets.CurrentCell = this.gridTickets.Rows[selectedIndex].Cells[0];
                else
                {
                    this.gridTickets.ClearSelection();
                    this.gridTickets.CurrentCell = null;
                }
            }
            finally
            {
                _suppressSelection = false;
            }
        }

        private void FillActivity(IReadOnlyList<TicketEvent> events)
        {
            var now = DateTime.Now;
            this.listActivity.Items.Clear();
            foreach (var e in events)
                this.listActivity.Items.Add($"● {e.Text} — {e.Age(now)}");
            this.lblActivityHead.Text = $"ACTIVITY · {events.Count}";
        }

        private void FillDetail(Ticket t)
        {
            _selectedId = t.Id;
            this.lblDetailTitle.Text = $"#{t.Id} — {t.Title}";
            this.txtTitle.Text = t.Title;
            this.cmbPriority.SelectedIndex = (int)t.Priority;
            int assignee = this.cmbAssignee.Items.IndexOf(t.Assignee ?? "");
            this.cmbAssignee.SelectedIndex = assignee > 0 ? assignee : 0;
            this.lblStatusValue.Text = StatusText(t.Status) + (t.IsEscalated ? " · escalated" : "");
            this.numHours.Value = (decimal)t.HoursLogged;
            this.txtNotes.Text = t.Notes ?? "";
        }

        private void ClearDetail()
        {
            _selectedId = null;
            this.lblDetailTitle.Text = "New ticket";
            this.txtTitle.Text = "";
            this.cmbPriority.SelectedIndex = 1;
            this.cmbAssignee.SelectedIndex = 0;
            this.lblStatusValue.Text = "Open";
            this.numHours.Value = 0;
            this.txtNotes.Text = "";
        }

        private TicketDraft ReadDraftFromForm()
        {
            string assignee = this.cmbAssignee.SelectedIndex > 0 ? this.cmbAssignee.SelectedItem.ToString() : "";
            return new TicketDraft
            {
                Id = _selectedId,
                Title = this.txtTitle.Text,
                Priority = (TicketPriority)Math.Max(0, this.cmbPriority.SelectedIndex),
                Assignee = assignee,
                HoursLogged = (double)this.numHours.Value,
                Notes = this.txtNotes.Text
            };
        }

        private static string StatusText(TicketStatus status)
            => status == TicketStatus.InProgress ? "In Progress" : status.ToString();

        private void ShowResult<T>(OperationResult<T> result)
        {
            if (result.Succeeded)
                Notify(result.Message, StatusKind.Success, null);
            else
                Notify("not applied", StatusKind.Warning, result.Message);
        }

        /// <summary>Unexpected failure: the details go to the log, the user sees one safe sentence.</summary>
        private void ReportFailure(string source, Exception ex)
        {
            _log.Error(LogLayer.UI, source, ex);
            Notify("failed", StatusKind.Error, "✖ " + Strings.ActionFailed);
            AlertBox.Show(Strings.ActionFailed, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        private void Notify(string status, StatusKind kind, string banner)
        {
            StatusChanged?.Invoke(this, new WorkspaceStatusEventArgs(status, kind, banner));
        }

        #endregion

        #region Handlers

        private void gridTickets_SelectionChanged(object sender, EventArgs e)
        {
            if (_suppressSelection)
                return;

            var row = this.gridTickets.CurrentRow;
            if (row == null || row.Index < 0 || row.Index >= _rows.Count)
                return;

            FillDetail(_rows[row.Index]);

            if (_appliedProfile == ProfilePhone)
                ShowPhonePage(true);            // phone: selecting a row IS the navigation
            else if (this.tabActivity.Visible)
                this.tabActivity.SelectedIndex = 0;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var draft = ReadDraftFromForm();
                var result = await _tickets.SaveAsync(draft);
                ShowResult(result);
                if (result.Succeeded)
                {
                    _selectedId = result.Value.Id;
                    await RefreshAsync();
                }
            }
            catch (Exception ex)
            {
                ReportFailure("TicketWorkspace.btnSave_Click", ex);
            }
        }

        private async void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_selectedId.HasValue)
                {
                    ShowResult(OperationResult<Ticket>.Fail(Strings.SelectTicketFirst));
                    return;
                }

                var result = await _tickets.CloseAsync(_selectedId.Value);
                ShowResult(result);
                if (result.Succeeded)
                {
                    ClearDetail();
                    await RefreshAsync();
                    if (_appliedProfile == ProfilePhone)
                        ShowPhonePage(false);
                }
            }
            catch (Exception ex)
            {
                ReportFailure("TicketWorkspace.btnClose_Click", ex);
            }
        }

        private void btnNewTicket_Click(object sender, EventArgs e)
        {
            _suppressSelection = true;
            this.gridTickets.ClearSelection();
            this.gridTickets.CurrentCell = null;
            _suppressSelection = false;

            ClearDetail();

            if (_appliedProfile == ProfilePhone)
                ShowPhonePage(true);
            else if (this.tabActivity.Visible)
                this.tabActivity.SelectedIndex = 0;
            this.txtTitle.Focus();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            ShowPhonePage(false);
        }

        private async void searchTickets_SearchRequested(object sender, SearchEventArgs e)
        {
            await SearchTicketsAsync(e.Query);
        }

        private async void searchActivity_SearchRequested(object sender, SearchEventArgs e)
        {
            try
            {
                var result = await _tickets.GetActivityAsync(e.Query);
                ShowResult(result);
                if (result.Succeeded)
                    FillActivity(result.Value);
            }
            catch (Exception ex)
            {
                ReportFailure("TicketWorkspace.searchActivity_SearchRequested", ex);
            }
        }

        private void BuildChips()
        {
            foreach (string chip in TicketFilter.Chips)
            {
                var button = new Button
                {
                    Name = "chip" + chip,
                    Text = chip,
                    Size = new System.Drawing.Size(chip.Length > 6 ? 104 : 78, 28),
                    Margin = new Padding(0, 0, 6, 6),
                    Tag = chip
                };
                button.Click += this.chip_Click;
                this.flowChips.Controls.Add(button);
            }
        }

        private async void chip_Click(object sender, EventArgs e)
        {
            try
            {
                var clicked = (Button)sender;
                string chip = (string)clicked.Tag;
                _activeChip = _activeChip == chip ? null : chip;

                foreach (Control c in this.flowChips.Controls)
                    if (c is Button b)
                        b.Text = (string)b.Tag == _activeChip ? "✓ " + (string)b.Tag : (string)b.Tag;

                await SearchTicketsAsync(this.searchTickets.Query);
            }
            catch (Exception ex)
            {
                ReportFailure("TicketWorkspace.chip_Click", ex);
            }
        }

        private void lblNav_Click(object sender, EventArgs e)
        {
            var clicked = (Label)sender;
            foreach (Control c in this.pnlNavigation.Controls)
            {
                if (c is Label item)
                {
                    bool active = item == clicked;
                    item.ForeColor = active ? System.Drawing.Color.White : System.Drawing.Color.FromArgb(143, 169, 201);
                    item.BackColor = active ? System.Drawing.Color.FromArgb(31, 66, 110) : System.Drawing.Color.FromArgb(15, 36, 64);
                }
            }
        }

        #endregion
    }
}
