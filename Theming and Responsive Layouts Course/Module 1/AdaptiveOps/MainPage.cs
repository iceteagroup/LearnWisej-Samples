using System;
using System.Globalization;
using AdaptiveOps.Models;
using Wisej.Web;

namespace AdaptiveOps
{
    /// <summary>
    /// Adaptive Operations Console — Module 1 shell (The Wisej.NET Visual System).
    ///
    /// The whole layout is five docked Panels declared in MainPage.Designer.cs:
    /// toolbar (Top, 56), status (Bottom, 28), navigation (Left, 220), details (Right, 340)
    /// and the workspace (Fill, MinimumSize 320×240). Nothing in this file positions a control:
    /// the layout containers own position and size, the base theme (Bootstrap-4, selected in
    /// Default.json) owns the look of every standard control, and this code-behind only moves
    /// data between the TicketRepository (the server-side source of truth) and the controls.
    ///
    /// Toolbar buttons exercise the four lab paths:
    ///   Refresh        success   reload from the repository, refresh the metric cards and the grid
    ///   Simulate load  progress  a Wisej.Web.Timer steps through 5 steps and reports each one
    ///   Invalid ticket failure   save a ticket with an empty title; the repository throws, the UI catches
    ///   Reset          recovery  seed data restored, banner cleared, everything re-read from the server
    ///
    /// The status bar reports the browser size (Application.Browser.Size / Device), refreshed from
    /// Application.BrowserSizeChanged and the Page.Resize event, and the active theme name.
    /// Every decision is logged in the "Layout & theme · live trace" card (see docs/VisualInventory.md
    /// and docs/VisualLayers.md for which layer owns what).
    /// </summary>
    public partial class MainPage : Page
    {
        private readonly TicketRepository _repository = new TicketRepository();

        /// <summary>Id of the ticket shown in the details editor, or null.</summary>
        private string _selectedId;

        /// <summary>True while the grid is being refilled, so SelectionChanged does not re-enter.</summary>
        private bool _suppressSelection;

        /// <summary>Progress path: the step the simulated load has reached (0..5).</summary>
        private int _loadStep;

        private static readonly string[] LoadSteps =
        {
            "reading the ticket repository",
            "computing the four metrics",
            "filling the ticket grid",
            "refreshing the details editor",
            "done",
        };

        public MainPage()
        {
            InitializeComponent();

            // Editor drop-downs come from the enums; the base theme owns how they look.
            this.cboPriority.Items.AddRange(Enum.GetNames(typeof(TicketPriority)));
            this.cboStatus.Items.AddRange(Enum.GetNames(typeof(TicketStatus)));

            // Session-level event: it is unsubscribed in Dispose(bool) (MainPage.Designer.cs).
            Application.BrowserSizeChanged += this.Application_BrowserSizeChanged;

            // The lab asks for the width label to be filled once from the constructor so it is never empty.
            ReportWidth("constructor");
        }

        #region Shell: load, resize, theme

        private void MainPage_Load(object sender, EventArgs e)
        {
            AddTrace("• server shell built: " + DescribeShell());
            AddTrace("• server base theme: " + CurrentThemeName() + " (selected in Default.json; no custom theme, no CSS in Module 1)");
            this.lblTheme.Text = "theme: " + CurrentThemeName();

            VerifyShell();
            LoadTickets("page load");
            ReportWidth("page load");
            SetStatus("ready", StatusKind.Normal);
        }

        /// <summary>
        /// Fired by the framework when the browser window is resized (session-level event).
        /// </summary>
        private void Application_BrowserSizeChanged(object sender, EventArgs e)
        {
            ReportWidth("Application.BrowserSizeChanged");
        }

        /// <summary>
        /// The main Page always fills the browser viewport, so its Resize event fires on every
        /// window resize too. Docking already answers every width: this handler only reports.
        /// It never sets Bounds.
        /// </summary>
        private void MainPage_Resize(object sender, EventArgs e)
        {
            ReportWidth("Page.Resize");
        }

        /// <summary>
        /// Writes "Browser 1348 × 680 px · Desktop" into the status bar. Wrapped in try/catch so the
        /// label carries an honest fallback text rather than a stale number when the size cannot be read.
        /// </summary>
        private void ReportWidth(string reason)
        {
            try
            {
                var browser = Application.Browser;
                var size = browser.Size;
                if (size.Width <= 0 || size.Height <= 0)
                    throw new InvalidOperationException("the browser has not reported its size yet");

                string device = string.IsNullOrEmpty(browser.Device) ? "Desktop" : browser.Device;
                this.lblBrowserWidth.Text = $"Browser {size.Width} × {size.Height} px · {device}";

                AddTrace($"← client resize ({reason}): browser {size.Width}×{size.Height} · page {this.Width}×{this.Height} · workspace {this.workspacePanel.Width}×{this.workspacePanel.Height}");
            }
            catch (Exception ex)
            {
                this.lblBrowserWidth.Text = "Browser size unavailable — " + ex.Message;
                AddTrace($"• server width read failed ({reason}): {ex.Message}");
            }
        }

        /// <summary>One line with the Dock and size of the five regions, as the running app laid them out.</summary>
        private string DescribeShell()
        {
            return string.Join(" · ",
                DescribeRegion("toolbar", this.toolbarPanel),
                DescribeRegion("navigation", this.navigationPanel),
                DescribeRegion("details", this.detailsPanel),
                DescribeRegion("status", this.statusPanel),
                DescribeRegion("workspace", this.workspacePanel) + $" min {this.workspacePanel.MinimumSize.Width}×{this.workspacePanel.MinimumSize.Height}");
        }

        private static string DescribeRegion(string name, Panel region)
        {
            return $"{name} Dock={region.Dock} {region.Width}×{region.Height}";
        }

        /// <summary>
        /// Acceptance check of the lab, run at startup: the five regions must be docked exactly as
        /// the module prescribes. A failure is logged, not thrown, so the shell still shows.
        /// </summary>
        private void VerifyShell()
        {
            bool ok =
                this.toolbarPanel.Dock == DockStyle.Top &&
                this.statusPanel.Dock == DockStyle.Bottom &&
                this.navigationPanel.Dock == DockStyle.Left &&
                this.detailsPanel.Dock == DockStyle.Right &&
                this.workspacePanel.Dock == DockStyle.Fill &&
                this.workspacePanel.MinimumSize.Width == 320 &&
                this.workspacePanel.MinimumSize.Height == 240;

            AddTrace(ok
                ? "• server dock order verified: Top, Bottom, Left, Right, Fill (child order in MainPage.Designer.cs)"
                : "• server DOCK ORDER WRONG — check the Controls.Add order at the end of InitializeComponent()");
        }

        private static string CurrentThemeName()
        {
            try { return Application.Theme?.Name ?? "(default)"; }
            catch (Exception) { return "(default)"; }
        }

        #endregion

        #region Success path: Refresh (and the shared reload)

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadTickets("Refresh");
            SetStatus("ready", StatusKind.Normal);
        }

        /// <summary>
        /// Re-reads everything from the repository: the four metric cards, the grid and, when a
        /// ticket is selected, the details editor. The server is the source of truth, so a refresh
        /// is always "read again and render", never "patch the browser".
        /// </summary>
        private void LoadTickets(string reason)
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

            AddTrace($"→ render {reason}: {tickets.Count} tickets · open {this.lblOpenValue.Text} · overdue {this.lblOverdueValue.Text} · mine {this.lblMineValue.Text} · closed this week {this.lblClosedValue.Text}");

            if (reselect >= 0)
                ShowTicket((string)this.gridTickets.Rows[reselect].Tag, reason);
            else
                ClearEditor();
        }

        #endregion

        #region Progress path: Simulate load (Wisej.Web.Timer)

        private void btnSimulateLoad_Click(object sender, EventArgs e)
        {
            if (this.timerLoad.Enabled)
                return;

            _loadStep = 0;
            this.lblProgress.Text = $"Simulated load · step 0 of {LoadSteps.Length}";
            this.btnSimulateLoad.Enabled = false;
            SetStatus("loading", StatusKind.Warn);
            AddTrace($"• server simulated load started: {LoadSteps.Length} steps every {this.timerLoad.Interval} ms (Wisej.Web.Timer, no client code)");
            this.timerLoad.Start();
        }

        private void timerLoad_Tick(object sender, EventArgs e)
        {
            _loadStep++;
            string step = LoadSteps[Math.Min(_loadStep, LoadSteps.Length) - 1];
            this.lblProgress.Text = $"Simulated load · step {_loadStep} of {LoadSteps.Length} · {step}";
            AddTrace($"→ render progress {_loadStep}/{LoadSteps.Length}: {step}");

            if (_loadStep < LoadSteps.Length)
                return;

            this.timerLoad.Stop();
            LoadTickets("simulated load");
            this.lblProgress.Text = $"Simulated load complete · {LoadSteps.Length}/{LoadSteps.Length}";
            this.btnSimulateLoad.Enabled = true;
            SetStatus("ready", StatusKind.Normal);
        }

        #endregion

        #region Failure path and recovery

        /// <summary>
        /// Blanks the title in the editor and tries to save it. The repository refuses the ticket
        /// on the server (TicketValidationException), nothing is written, the banner and the
        /// trace say why. Reset (or selecting the ticket again) recovers.
        /// </summary>
        private void btnInvalidTicket_Click(object sender, EventArgs e)
        {
            if (_selectedId == null)
                ShowTicket(_repository.GetAll()[0].Id, "Invalid ticket");

            this.txtTitle.Text = string.Empty;
            var ticket = ReadEditor();
            SaveTicket(ticket, "Invalid ticket");
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (this.timerLoad.Enabled)
            {
                this.timerLoad.Stop();
                this.btnSimulateLoad.Enabled = true;
            }

            _repository.Reset();
            HideBanner();
            this.lblProgress.Text = string.Empty;
            AddTrace("• server repository reset to the seed tickets; banner cleared");
            LoadTickets("Reset");
            SetStatus("ready", StatusKind.Normal);
        }

        #endregion

        #region Details editor

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

            ShowTicket((string)row.Tag, "grid selection");
        }

        /// <summary>Fills the editor from the repository copy of the ticket (never from the grid cells).</summary>
        private void ShowTicket(string id, string reason)
        {
            var t = _repository.Get(id);
            if (t == null)
            {
                ClearEditor();
                return;
            }

            _selectedId = t.Id;
            this.lblDetailsSubtitle.Text = $"{t.Id} · {t.Status} · due {t.DueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";
            this.txtTitle.Text = t.Title;
            this.cboPriority.SelectedIndex = (int)t.Priority;
            this.cboStatus.SelectedIndex = (int)t.Status;
            this.txtOwner.Text = t.Owner;
            this.dtpDue.Value = t.DueDate;
            this.txtNotes.Text = t.Notes;

            AddTrace($"← client {reason}: ticket {t.Id} \"{t.Title}\" loaded into the details editor");
        }

        private void ClearEditor()
        {
            _selectedId = null;
            this.lblDetailsSubtitle.Text = "Select a ticket in the grid";
            this.txtTitle.Text = string.Empty;
            this.cboPriority.SelectedIndex = -1;
            this.cboStatus.SelectedIndex = -1;
            this.txtOwner.Text = string.Empty;
            this.dtpDue.Value = DateTime.Today;
            this.txtNotes.Text = string.Empty;
        }

        /// <summary>Builds the ticket the editor currently describes. Validation is NOT done here.</summary>
        private Ticket ReadEditor()
        {
            return new Ticket
            {
                Id = _selectedId,
                Title = this.txtTitle.Text,
                Priority = (TicketPriority)Math.Max(0, this.cboPriority.SelectedIndex),
                Status = (TicketStatus)Math.Max(0, this.cboStatus.SelectedIndex),
                Owner = this.txtOwner.Text,
                DueDate = this.dtpDue.Value,
                Notes = this.txtNotes.Text
            };
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_selectedId == null)
            {
                ShowBanner("Select a ticket in the grid before saving.");
                SetStatus("nothing selected", StatusKind.Warn);
                AddTrace("• server save skipped: no ticket selected");
                return;
            }

            SaveTicket(ReadEditor(), "Save");
        }

        /// <summary>
        /// The only write path. TicketRepository.Save validates on the server and throws
        /// TicketValidationException; the catch block is the failure path of the lab.
        /// </summary>
        private bool SaveTicket(Ticket ticket, string origin)
        {
            try
            {
                var saved = _repository.Save(ticket);

                HideBanner();
                AddTrace($"• server saved {saved.Id} \"{saved.Title}\" ({saved.Priority}, {saved.Status}, {saved.Owner}, due {saved.DueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)})");
                LoadTickets(origin);
                SetStatus($"saved {saved.Id}", StatusKind.Normal);
                AlertBox.Show($"{saved.Id} saved.", MessageBoxIcon.Information,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return true;
            }
            catch (TicketValidationException ex)
            {
                // Failure path: the server refused the ticket. Nothing was written, the grid and the
                // metrics still show the last good state, the UI says why.
                ShowBanner($"✖ Rejected on the server ({origin}): {ex.Message} Nothing was written.");
                AddTrace($"• server rejected {ticket.Id ?? "(none)"} ({origin}): {ex.Message}");
                SetStatus("validation error", StatusKind.Error);
                AlertBox.Show(ex.Message, MessageBoxIcon.Warning,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return false;
            }
        }

        #endregion

        #region Navigation rail

        /// <summary>
        /// The rail is five plain Buttons in Module 1; a click only retitles the workspace and is traced.
        /// Later modules turn it into a UserControl and collapse it on the phone profile.
        /// </summary>
        private void btnNav_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null)
                return;

            this.lblWorkspaceTitle.Text = button.Text == "Dashboard" ? "Tickets" : button.Text;
            AddTrace($"← client navigation: {button.Text} (rail = navigationPanel Dock=Left, five Buttons with Anchor Left|Right)");
        }

        #endregion

        #region UI helpers: trace, status, banner

        private enum StatusKind { Normal, Warn, Error }

        /// <summary>
        /// Appends one line to the "Layout & theme · live trace" card and selects it.
        /// Lines start with "→ " (server to client), "← " (client to server) or "• " (server decision).
        /// </summary>
        private void AddTrace(string line)
        {
            while (this.listTrace.Items.Count >= 400)
                this.listTrace.Items.RemoveAt(0);

            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            this.listTrace.Items.Add($"{time}  {line}");
            this.listTrace.SelectedIndex = this.listTrace.Items.Count - 1;
        }

        private void btnClearTrace_Click(object sender, EventArgs e)
        {
            this.listTrace.Items.Clear();
        }

        private void SetStatus(string text, StatusKind kind)
        {
            this.lblStatus.Text = "● " + text;

            // Module 1 sets the three status colours as control properties; Module 2 moves them into
            // theme color tokens (success / warning / danger) — see docs/VisualLayers.md.
            this.lblStatus.ForeColor = kind switch
            {
                StatusKind.Error => System.Drawing.Color.FromArgb(180, 35, 24),
                StatusKind.Warn => System.Drawing.Color.FromArgb(181, 71, 8),
                _ => System.Drawing.Color.FromArgb(2, 122, 72),
            };
        }

        /// <summary>The banner is a docked Panel hidden by default: showing it pushes the grid down, no Bounds involved.</summary>
        private void ShowBanner(string text)
        {
            this.lblBanner.Text = text;
            this.bannerPanel.Visible = true;
        }

        private void HideBanner()
        {
            this.bannerPanel.Visible = false;
            this.lblBanner.Text = string.Empty;
        }

        private static string N(int value) => value.ToString(CultureInfo.InvariantCulture);

        #endregion
    }
}
