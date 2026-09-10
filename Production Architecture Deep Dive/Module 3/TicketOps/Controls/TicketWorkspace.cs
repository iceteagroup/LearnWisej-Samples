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
    /// TicketOps Console · the responsive Ticket Workspace (Module 3).
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
        private string _pinnedProfile;
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

        /// <summary>Raised after a profile layout was applied (real or preview); the argument is the profile name.</summary>
        public event EventHandler<string> ProfileApplied;

        /// <summary>The profile whose layout is currently on screen.</summary>
        public string AppliedProfile => _appliedProfile;

        /// <summary>True while a preview pins the layout and real profile changes are ignored.</summary>
        public bool IsPinned => _pinnedProfile != null;

        /// <summary>
        /// Gives a Designer-placed instance its dependencies (the same way the trace panel gets its log) and
        /// starts following the session's responsive profile. Unsubscribed in Dispose (Designer file).
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
        public Task LoadAsync() => RefreshAsync("initial load");

        /// <summary>Apply the browser's real profile and follow it from now on.</summary>
        public void FollowBrowser()
        {
            _pinnedProfile = null;
            ApplyResponsiveProfile(Application.ActiveProfile.Name);
        }

        /// <summary>Apply a profile's layout on demand and ignore real changes until <see cref="FollowBrowser"/>.</summary>
        public void PreviewProfile(string profileName)
        {
            _pinnedProfile = profileName;
            ApplyResponsiveProfile(profileName);
        }

        /// <summary>"selected #1003, draft title "…", chip High" — the state that must survive a profile switch.</summary>
        public string DescribeState()
        {
            string selected = _selectedId.HasValue ? "#" + _selectedId.Value : "none";
            string chip = _activeChip ?? "none";
            return $"selected {selected}, draft title \"{this.txtTitle.Text}\", search \"{this.searchTickets.Query}\", chip {chip}, side tab {this.tabActivity.SelectedIndex}";
        }

        #endregion

        #region Client profile → layout (the module's core)

        private void Application_ResponsiveProfileChanged(object sender, ResponsiveProfileChangedEventArgs e)
        {
            try
            {
                string previous = e.PreviousProfile != null ? e.PreviousProfile.Name : "?";
                string current = e.CurrentProfile != null ? e.CurrentProfile.Name : Application.ActiveProfile.Name;
                var size = Application.Browser.Size;
                _log.Info(LogLayer.Client, "TicketWorkspace.ResponsiveProfileChanged",
                    $"{previous} → {current} · browser {size.Width}×{size.Height} px · device {Application.Browser.Device}");

                if (_pinnedProfile != null)
                {
                    _log.Info(LogLayer.UI, "TicketWorkspace.ResponsiveProfileChanged",
                        $"preview pinned to {_pinnedProfile} — the browser's {current} layout waits until \"Live\" is chosen");
                    return;
                }

                OnResponsiveProfileChanged(sender, e);
            }
            catch (Exception ex)
            {
                ReportFailure("TicketWorkspace.ResponsiveProfileChanged", ex);
            }
        }

        /// <summary>
        /// The control-level twin of the Application event (wired in the Designer). It only reports, so the
        /// reviewer can see whether Control.ResponsiveProfileChanged fires for a UserControl as well.
        /// </summary>
        private void TicketWorkspace_ResponsiveProfileChanged(object sender, ResponsiveProfileChangedEventArgs e)
        {
            string current = e.CurrentProfile != null ? e.CurrentProfile.Name : "?";
            _log.Info(LogLayer.Client, "TicketWorkspace.Control.ResponsiveProfileChanged", $"control-level event fired too → {current}");
        }

        /// <summary>The lab's handler: read the active profile, delegate to one named arrangement.</summary>
        private void OnResponsiveProfileChanged(object sender, EventArgs e)
        {
            ApplyResponsiveProfile(Application.ActiveProfile.Name);
        }

        /// <summary>
        /// Maps a profile name to an arrangement of the SAME panels. The switch reads like the table in the
        /// applied guide; anything longer than a property toggle lives in a named method.
        /// </summary>
        public void ApplyResponsiveProfile(string profileName)
        {
            string stateBefore = DescribeState();

            switch (profileName)
            {
                case ProfileDesktop:
                    ShowAllPanels();            // full layout: nav, list, detail and activity at once
                    break;

                case ProfileTablet:
                    CollapseActivityToTab();    // same feed, reachable through a tab to reclaim width
                    break;

                case ProfilePhone:
                    ShowSingleTaskView();       // one pane at a time, big touch targets, a way back
                    break;

                default:
                    // A name the JSON does not define (or a built-in one such as "Small Desktop"): never leave
                    // the screen half-arranged — fall back to the desktop layout and say so.
                    _log.Warn(LogLayer.UI, "TicketWorkspace.ApplyResponsiveProfile",
                        $"\"{profileName}\" is not Desktop / Tablet / Phone — not defined in ClientProfiles.json → desktop layout as the safe fallback");
                    ShowAllPanels();
                    _appliedProfile = profileName;
                    this.lblStatus.Text = $"Active profile: {profileName} (fallback: desktop layout)";
                    Notify("unknown profile", StatusKind.Warning, string.Format(Strings.UnknownProfile, profileName));
                    ProfileApplied?.Invoke(this, profileName);
                    return;
            }

            _appliedProfile = profileName;
            this.lblStatus.Text = $"Active profile: {profileName}";
            _log.Info(LogLayer.UI, "TicketWorkspace.ApplyResponsiveProfile",
                $"{profileName} → {DescribeLayout(profileName)} · state kept (moved, not rebuilt): {stateBefore}");
            ProfileApplied?.Invoke(this, profileName);
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
            _log.Info(LogLayer.UI, "TicketWorkspace.MovePanelsToFlex", "pnlDetail, pnlActivity → flexSide (re-parented, widget state intact)");
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
            _log.Info(LogLayer.UI, "TicketWorkspace.MovePanelsToTabs", "pnlDetail → tabPageDetails, pnlActivity → tabPageActivity (re-parented, widget state intact)");
        }

        private void SetNavigationCompact(bool compact)
        {
            this.pnlNavigation.Width = compact ? 56 : 150;
            this.lblNavDashboard.Text = compact ? "▦" : "▦   Dashboard";
            this.lblNavTickets.Text = compact ? "◳" : "◳   Tickets";
            this.lblNavReports.Text = compact ? "▤" : "▤   Reports";
            this.lblNavSettings.Text = compact ? "⚙" : "⚙   Settings";
        }

        private static string DescribeLayout(string profileName)
        {
            switch (profileName)
            {
                case ProfileTablet: return "nav compact (56 px) · Assignee column hidden · list : side = 1 : 1 · detail + activity in tabs";
                case ProfilePhone: return "nav + toolbar hidden · one pane at a time · Back button · detail + activity in tabs";
                default: return "nav 150 px · list : side = 3 : 2 · detail : activity = 3 : 2 · every panel visible";
            }
        }

        #endregion

        #region Data → UI and UI → data

        /// <summary>Reloads the grid (with the current filter) and the activity feed through the service.</summary>
        public async Task RefreshAsync(string reason)
        {
            try
            {
                Notify("loading", StatusKind.Busy, null);
                _log.Info(LogLayer.UI, "TicketWorkspace.Refresh", $"{reason} → ITicketService.SearchAsync {CurrentFilter()} + GetActivityAsync");

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
                _log.Info(LogLayer.UI, "TicketWorkspace.Refresh", $"{_rows.Count} tickets, {(events.Succeeded ? events.Value.Count : 0)} events shown");
            }
            catch (Exception ex)
            {
                ReportFailure("TicketWorkspace.Refresh", ex);
            }
        }

        /// <summary>Runs a ticket search through the service (also the failure path when the query is too short).</summary>
        public async Task SearchTicketsAsync(string query)
        {
            try
            {
                var filter = CurrentFilter(query);
                _log.Info(LogLayer.UI, "TicketWorkspace.SearchTickets", $"→ ITicketService.SearchAsync {filter}");
                var result = await _tickets.SearchAsync(filter);
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
            {
                Notify(result.Message, StatusKind.Success, null);
                _log.Info(LogLayer.UI, "TicketWorkspace.ShowResult", $"OK · {result.Message}");
            }
            else
            {
                // Expected outcome: the service explained it in words the user may read.
                Notify("not applied", StatusKind.Warning, result.Message);
                _log.Warn(LogLayer.UI, "TicketWorkspace.ShowResult", $"FAIL · {result.Message}");
            }
        }

        /// <summary>
        /// Unexpected failure: details go to the log (with the exception type and message), the user sees one
        /// generic sentence. Nothing internal leaks through the banner.
        /// </summary>
        private void ReportFailure(string source, Exception ex)
        {
            _log.Error(LogLayer.UI, source, ex, $"caught {ex.GetType().Name} — user sees the safe message");
            Notify("failed", StatusKind.Error, "✖ " + Strings.ActionFailed);
            AlertBox.Show(Strings.ActionFailed, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        private void Notify(string status, StatusKind kind, string banner)
        {
            StatusChanged?.Invoke(this, new WorkspaceStatusEventArgs(status, kind, banner));
        }

        #endregion

        #region Thin handlers

        private void gridTickets_SelectionChanged(object sender, EventArgs e)
        {
            if (_suppressSelection)
                return;

            var row = this.gridTickets.CurrentRow;
            if (row == null || row.Index < 0 || row.Index >= _rows.Count)
                return;

            var t = _rows[row.Index];
            FillDetail(t);
            _log.Info(LogLayer.UI, "TicketWorkspace.gridTickets_SelectionChanged", $"#{t.Id} → detail form (display only, no service call)");

            if (_appliedProfile == ProfilePhone)
                ShowPhonePage(true);            // phone: selecting a row IS the navigation
            else if (this.tabActivity.Visible)
                this.tabActivity.SelectedIndex = 0;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var draft = ReadDraftFromForm();                                       // UI → data
                _log.Info(LogLayer.UI, "TicketWorkspace.btnSave_Click", $"→ ITicketService.SaveAsync {draft}");
                var result = await _tickets.SaveAsync(draft);                         // the decision lives in the service
                ShowResult(result);                                                    // data → UI
                if (result.Succeeded)
                {
                    _selectedId = result.Value.Id;
                    await RefreshAsync($"saved #{result.Value.Id}");
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

                _log.Info(LogLayer.UI, "TicketWorkspace.btnClose_Click", $"→ ITicketService.CloseAsync(#{_selectedId})");
                var result = await _tickets.CloseAsync(_selectedId.Value);
                ShowResult(result);
                if (result.Succeeded)
                {
                    ClearDetail();
                    await RefreshAsync("closed a ticket");
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
            _log.Info(LogLayer.UI, "TicketWorkspace.btnNewTicket_Click", "editor cleared (display only, no service call)");

            if (_appliedProfile == ProfilePhone)
                ShowPhonePage(true);
            else if (this.tabActivity.Visible)
                this.tabActivity.SelectedIndex = 0;
            this.txtTitle.Focus();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            _log.Info(LogLayer.UI, "TicketWorkspace.btnBack_Click", $"phone: back to the list — edits in progress stay in the hidden editor ({DescribeState()})");
            ShowPhonePage(false);
        }

        private async void searchTickets_SearchRequested(object sender, SearchEventArgs e)
        {
            _log.Info(LogLayer.UI, "TicketWorkspace.searchTickets_SearchRequested", $"SearchBar #1 raised SearchRequested(\"{e.Query}\")");
            await SearchTicketsAsync(e.Query);
        }

        private async void searchActivity_SearchRequested(object sender, SearchEventArgs e)
        {
            try
            {
                _log.Info(LogLayer.UI, "TicketWorkspace.searchActivity_SearchRequested", $"SearchBar #2 raised SearchRequested(\"{e.Query}\") → ITicketService.GetActivityAsync");
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
                    Tag = chip,
                    ToolTipText = $"FlowLayoutPanel chip: filters through ITicketService.SearchAsync (chip \"{chip}\")"
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

                _log.Info(LogLayer.UI, "TicketWorkspace.chip_Click", _activeChip == null ? "chip cleared" : $"chip \"{_activeChip}\" on");
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
            _log.Info(LogLayer.UI, "TicketWorkspace.lblNav_Click", $"navigation → {(string)clicked.Tag} (other screens are outside this module; the workspace stays)");
        }

        #endregion
    }
}
