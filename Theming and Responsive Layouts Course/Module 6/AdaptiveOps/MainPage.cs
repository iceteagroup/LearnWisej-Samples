using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AdaptiveOps.Dialogs;
using AdaptiveOps.Models;
using AdaptiveOps.Shell;
using Wisej.Core;
using Wisej.Web;

namespace AdaptiveOps
{
    /// <summary>
    /// Adaptive Operations Console — Module 6 (ClientProfile and Responsive Properties).
    ///
    /// The same five docked regions as before (toolbar Top, status Bottom, navigation Left, details Right,
    /// workspace Fill), but the console now knows WHICH client it is talking to. ClientProfiles.json in the
    /// project root defines six profiles, narrow to broad — Phone, Phone (Landscape), Tablet, Tablet (Landscape),
    /// Small Desktop, Desktop — and the framework matches them top to bottom into Application.ActiveProfile
    /// (first match wins, nothing merges). Application.ResponsiveProfileChanged routes every change through ONE
    /// method, <see cref="ApplyProfile(ClientProfile, string, ClientProfile)"/>, which changes behaviour, not widths:
    ///
    ///   Phone            rail hidden + toolbar "Menu" button · toolbar icon-only · details editor moves into a
    ///                    modal TicketEditorForm opened on row selection · metric cards stack 1×4 · grid hides Owner, Due
    ///   Phone (Landscape) as Phone, but the title stays and the cards keep one row
    ///   Tablet           rail icon-only (64 px) · details docked UNDER the grid (Dock Bottom, 320 px) · Owner hidden
    ///   Tablet (Landscape) rail icon-only · details docked Right but narrower (300 px) · toolbar icon-only
    ///   Small Desktop    toolbar icon-only · details 300 px · metric cards 2×2
    ///   Desktop          everything visible, full labels, 4 cards in a row
    ///
    /// In Visual Studio the deterministic values in that table (Visible, Display, Dock, Size per profile) would be
    /// assigned in the Designer's responsive-profile dropdown and stored in Control.ResponsiveProfiles; this hand-
    /// written sample expresses them in ApplyProfile so the mapping is readable in one place. Each region is
    /// applied inside its own try/catch, so one faulty region never blocks the others.
    ///
    /// Toolbar buttons exercise the four lab paths:
    ///   Phone / Tablet / Desktop  success   ApplyProfile with the matching ClientProfile object from Application.Browser.Profiles
    ///   Step profiles             progress  a Wisej.Web.Timer walks Desktop → Small Desktop → Tablet → Phone and back
    ///   Unknown                   failure   the profile "Kiosk" does not exist: logged, current profile kept
    ///   Faulty region             failure   a fault injected into the metrics region; the other five regions still apply
    ///   Re-apply                  recovery  Application.ActiveProfile is read again and applied; simulation ends
    ///
    /// Every profile change is logged in the "Layout & theme · live trace" card with the previous and current
    /// profile names and the browser size (see docs/ProfileNotes.md and docs/ResponsiveQAMatrix.md).
    /// </summary>
    public partial class MainPage : Page
    {
        private readonly TicketRepository _repository = new TicketRepository();

        /// <summary>Id of the ticket shown in the details editor, or null.</summary>
        private string _selectedId;

        /// <summary>True while the grid is being refilled, so SelectionChanged does not re-enter.</summary>
        private bool _suppressSelection;

        /// <summary>The profile the console is currently laid out for (real or simulated).</summary>
        private ProfileKind _appliedKind = ProfileKind.Desktop;

        /// <summary>Name of the applied profile as shown in the status bar ("Desktop", "Phone (Landscape)" …).</summary>
        private string _appliedName = "Desktop";

        /// <summary>True when the applied profile came from a lab button, not from the browser.</summary>
        private bool _simulated;

        /// <summary>Region name that must throw on the next ApplyProfile (failure path 2), or null.</summary>
        private string _faultInRegion;

        /// <summary>The reusable phone dialog; created lazily, disposed with the page (MainPage.Designer.cs).</summary>
        private TicketEditorForm _editorForm;

        /// <summary>Desktop widths of the toolbar buttons, captured once so icon-only mode can be undone.</summary>
        private readonly Dictionary<Button, int> _toolbarWidths = new Dictionary<Button, int>();

        /// <summary>Progress path: index into <see cref="StepSequence"/>.</summary>
        private int _stepIndex;

        private static readonly string[] StepSequence =
        {
            "Desktop", "Small Desktop", "Tablet", "Phone", "Tablet", "Small Desktop", "Desktop",
        };

        /// <summary>
        /// The six behaviours the console knows. A ClientProfile is resolved to one of these by name, so a
        /// profile that is not in ClientProfiles.json (or whose name the console does not know) is a miss.
        /// </summary>
        private enum ProfileKind
        {
            Phone,
            PhoneLandscape,
            Tablet,
            TabletLandscape,
            SmallDesktop,
            Desktop,
        }

        public MainPage()
        {
            InitializeComponent();

            foreach (var b in ToolbarButtons())
                _toolbarWidths[b] = b.Width;

            // Session-level events: both are unsubscribed in Dispose(bool) (MainPage.Designer.cs) because
            // Application outlives the page.
            Application.BrowserSizeChanged += this.Application_BrowserSizeChanged;
            Application.ResponsiveProfileChanged += this.Application_ResponsiveProfileChanged;

            // Apply once at construction so the very first render is already right for this client.
            ApplyProfile(Application.ActiveProfile, "constructor");
        }

        #region Shell: load, resize, theme

        private void MainPage_Load(object sender, EventArgs e)
        {
            AddTrace("• server shell built: " + DescribeShell());
            AddTrace("• server base theme: " + CurrentThemeName() + " (Default.json); ClientProfiles.json in the project root, copied next to the assembly");
            this.lblTheme.Text = "theme: " + CurrentThemeName();

            LogKnownProfiles();
            VerifyShell();
            LoadTickets("page load");
            UpdateStatusBar("page load");
            SetStatus("ready", StatusKind.Normal);
        }

        /// <summary>
        /// Fired by the framework when the browser window is resized (session-level event). The profile
        /// itself is re-evaluated by the framework; this handler only refreshes the size in the status bar.
        /// </summary>
        private void Application_BrowserSizeChanged(object sender, EventArgs e)
        {
            UpdateStatusBar("Application.BrowserSizeChanged");
        }

        /// <summary>The main Page fills the viewport, so its Resize fires on every window resize too. It only reports.</summary>
        private void MainPage_Resize(object sender, EventArgs e)
        {
            UpdateStatusBar("Page.Resize");
        }

        /// <summary>Writes "profile: Phone · browser 390 × 844 px · Mobile" into the status bar.</summary>
        private void UpdateStatusBar(string reason)
        {
            string tag = _simulated ? " (simulated)" : string.Empty;
            try
            {
                var browser = Application.Browser;
                var size = browser.Size;
                if (size.Width <= 0 || size.Height <= 0)
                    throw new InvalidOperationException("the browser has not reported its size yet");

                string device = string.IsNullOrEmpty(browser.Device) ? "Desktop" : browser.Device;
                this.lblProfile.Text = $"profile: {_appliedName}{tag} · browser {size.Width} × {size.Height} px · {device}";

                if (reason != null)
                    AddTrace($"← client resize ({reason}): browser {size.Width}×{size.Height} · page {this.Width}×{this.Height} · workspace {this.workspacePanel.Width}×{this.workspacePanel.Height} · ActiveProfile {Application.ActiveProfile?.Name ?? "(null)"}");
            }
            catch (Exception ex)
            {
                this.lblProfile.Text = $"profile: {_appliedName}{tag} · browser size unavailable — {ex.Message}";
                if (reason != null)
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
            return $"{name} Dock={region.Dock} {region.Width}×{region.Height}" + (region.Visible ? string.Empty : " hidden");
        }

        /// <summary>
        /// Acceptance check of the lab, run at startup: the five regions must be docked as the module prescribes
        /// and the grid must still have a MinimumSize. A failure is logged, not thrown, so the shell still shows.
        /// </summary>
        private void VerifyShell()
        {
            bool ok =
                this.toolbarPanel.Dock == DockStyle.Top &&
                this.statusPanel.Dock == DockStyle.Bottom &&
                this.navigationPanel.Dock == DockStyle.Left &&
                this.workspacePanel.Dock == DockStyle.Fill &&
                this.workspacePanel.MinimumSize.Width == 320 &&
                this.workspacePanel.MinimumSize.Height == 240 &&
                this.gridTickets.MinimumSize.Width > 0;

            AddTrace(ok
                ? $"• server dock order verified: Top, Bottom, Left, {this.detailsPanel.Dock}, Fill · gridTickets MinimumSize {this.gridTickets.MinimumSize.Width}×{this.gridTickets.MinimumSize.Height}"
                : "• server DOCK ORDER WRONG — check the Controls.Add order at the end of InitializeComponent()");
        }

        private static string CurrentThemeName()
        {
            try { return Application.Theme?.Name ?? "(default)"; }
            catch (Exception) { return "(default)"; }
        }

        /// <summary>
        /// Logs the profiles the framework loaded, in match order. When "Desktop" is in the list the custom
        /// ClientProfiles.json was read; when the list stops at "Small Desktop" only the embedded defaults are active.
        /// </summary>
        private void LogKnownProfiles()
        {
            try
            {
                var profiles = Application.Browser.Profiles;
                if (profiles == null || profiles.Length == 0)
                {
                    AddTrace("• server Application.Browser.Profiles is empty — ClientProfiles.json was not loaded");
                    return;
                }

                AddTrace("• server client profiles in match order (first match wins): " + string.Join(" → ", profiles.Select(p => p.Name)));
                foreach (var p in profiles)
                    AddTrace($"    {p.Name}: {DescribeRule(p)}");

                if (!profiles.Any(p => p.Name == "Desktop"))
                    AddTrace("• server WARNING: no \"Desktop\" profile — the custom ClientProfiles.json was not picked up; a wide desktop browser will report \"Default\"");
            }
            catch (Exception ex)
            {
                AddTrace("• server could not read Application.Browser.Profiles: " + ex.Message);
            }
        }

        /// <summary>The match rule of a profile as a readable string (only the members the profile sets).</summary>
        private static string DescribeRule(ClientProfile p)
        {
            var parts = new List<string>();
            if (!string.IsNullOrEmpty(p.Device)) parts.Add("device " + p.Device);
            if (p.Landscape.HasValue) parts.Add("landscape " + (p.Landscape.Value ? "true" : "false"));
            if (p.MinWidth > 0) parts.Add("minWidth " + p.MinWidth);
            if (p.MaxWidth > 0) parts.Add("maxWidth " + p.MaxWidth);
            if (p.MinScreenWidth > 0) parts.Add("minScreenWidth " + p.MinScreenWidth);
            if (p.MaxScreenWidth > 0) parts.Add("maxScreenWidth " + p.MaxScreenWidth);
            if (!string.IsNullOrEmpty(p.UserAgent)) parts.Add("userAgent " + p.UserAgent);
            return parts.Count == 0 ? "(no rule — matches everything)" : string.Join(", ", parts);
        }

        #endregion

        #region Profiles: the event, ApplyProfile and the six behaviours

        /// <summary>
        /// The application-wide event: the framework re-evaluated ClientProfiles.json (page load, resize,
        /// rotation, device emulation + refresh) and the first match changed. Everything goes through ApplyProfile.
        /// </summary>
        private void Application_ResponsiveProfileChanged(object sender, ResponsiveProfileChangedEventArgs e)
        {
            _simulated = false;
            ApplyProfile(e.CurrentProfile, "Application.ResponsiveProfileChanged", e.PreviousProfile);
        }

        /// <summary>
        /// The same event on the Page (containers — Form, Page, Desktop, UserControl — and DataGridView / ListView
        /// get it too). Nothing is applied here: the Application handler already did, and applying twice would
        /// only prove the method is idempotent. It is traced so the order of the two events can be seen.
        /// </summary>
        private void MainPage_ResponsiveProfileChanged(object sender, ResponsiveProfileChangedEventArgs e)
        {
            AddTrace($"← client Page.ResponsiveProfileChanged too: {e.PreviousProfile?.Name ?? "(null)"} → {e.CurrentProfile?.Name ?? "(null)"} (container-level; nothing to do, the Application handler applied it)");
        }

        /// <summary>
        /// Applies a client profile. <paramref name="profile"/> null means "no profile matched" and is treated as
        /// ClientProfile.Default, which the console maps to Desktop. A profile whose name the console does not know
        /// is logged and ignored: the previous layout stays. Returns true when a layout was applied.
        /// </summary>
        private bool ApplyProfile(ClientProfile profile, string reason, ClientProfile previous = null)
        {
            string name = profile?.Name ?? ClientProfile.Default?.Name ?? "Default";
            var kind = KindFromName(name);
            if (kind == null)
            {
                AddTrace($"• server profile \"{name}\" is not one the console knows ({reason}); keeping \"{_appliedName}\"");
                return false;
            }

            string previousName = previous?.Name ?? _appliedName;
            var browser = Application.Browser;
            string size;
            try { size = $"{browser.Size.Width}×{browser.Size.Height} px · screen {browser.ScreenSize.Width}×{browser.ScreenSize.Height} · {browser.Device}"; }
            catch (Exception) { size = "(browser size unavailable)"; }

            AddTrace($"• server profile change ({reason}): \"{previousName}\" → \"{name}\" · browser {size}" + (profile == null ? " · ActiveProfile was null, using ClientProfile.Default" : string.Empty));

            ApplyKind(kind.Value, name == "Default" ? "Desktop (Default)" : name);
            return true;
        }

        /// <summary>
        /// The six behaviours, one region at a time. Idempotent: every region computes its target state from the
        /// kind and assigns it; assigning the same value twice changes nothing on the client, an editor that is
        /// already in the dialog stays there, a dialog that is already open is not opened again.
        /// </summary>
        private void ApplyKind(ProfileKind kind, string name)
        {
            bool phone = kind == ProfileKind.Phone || kind == ProfileKind.PhoneLandscape;
            bool tablet = kind == ProfileKind.Tablet || kind == ProfileKind.TabletLandscape;
            bool iconOnlyToolbar = kind != ProfileKind.Desktop;

            _appliedKind = kind;
            _appliedName = name;

            var results = new List<string>();
            int failed = 0;

            // toolbar: icon-only commands below Desktop; the Menu button exists only where the rail is gone.
            if (!ApplyRegion("toolbar", results, () =>
            {
                foreach (var b in ToolbarButtons())
                {
                    b.Display = iconOnlyToolbar ? Display.Icon : Display.Both;
                    b.Width = iconOnlyToolbar ? 36 : _toolbarWidths[b];
                    b.AccessibleName = b.Text;
                }
                this.btnMenu.Visible = phone;
                this.lblAppTitle.Visible = kind != ProfileKind.Phone;
                this.lblAppTitle.Text = phone ? "AdaptiveOps" : "Adaptive Operations Console";
                this.lblAppTitle.Width = phone ? 120 : 250;
                return iconOnlyToolbar ? "icon-only" + (phone ? ", Menu button shown" : string.Empty) : "labels";
            })) failed++;

            // navigation: hidden on Phone (Menu replaces it), icon-only on Tablet, full otherwise.
            if (!ApplyRegion("navigation", results, () =>
            {
                if (phone)
                {
                    this.navigationPanel.Visible = false;
                    return "rail hidden → Menu button";
                }
                var mode = tablet ? RailMode.IconOnly : RailMode.Full;
                this.navigationRail.SetMode(mode);
                this.navigationPanel.Width = NavigationRail.WidthFor(mode);
                this.navigationPanel.Visible = true;
                return mode == RailMode.IconOnly ? "rail icon-only 64 px (labels as tooltips)" : "rail full 220 px";
            })) failed++;

            // details: a dialog on Phone, docked under the grid on Tablet, docked right (narrower) elsewhere.
            if (!ApplyRegion("details", results, () =>
            {
                if (phone)
                {
                    this.detailsPanel.Visible = false;
                    HostEditorInDialog();
                    if (_selectedId != null)
                        OpenEditorDialog("profile change with " + _selectedId + " selected");
                    return "editor → modal TicketEditorForm (opens on row selection)";
                }

                HostEditorInPanel();
                if (kind == ProfileKind.Tablet)
                {
                    this.detailsPanel.Dock = DockStyle.Bottom;
                    this.detailsPanel.Padding = new Padding(8, 4, 8, 4);
                    this.detailsPanel.Height = 320;
                    this.detailsPanel.Visible = true;
                    return "editor docked UNDER the grid (Dock Bottom 320 px, scrolls)";
                }

                int width = kind == ProfileKind.Desktop ? 340 : 300;
                this.detailsPanel.Dock = DockStyle.Right;
                this.detailsPanel.Padding = new Padding(0, 4, 8, 4);
                this.detailsPanel.Width = width;
                this.detailsPanel.Visible = true;
                return $"editor docked Right {width} px";
            })) failed++;

            // metrics: 1×4 on Phone, 2×2 on Small Desktop, one row of four everywhere else.
            if (!ApplyRegion("metrics", results, () =>
            {
                if (_faultInRegion == "metrics")
                {
                    _faultInRegion = null;
                    throw new InvalidOperationException("injected fault: the metrics layout threw before it was applied");
                }
                int columns = kind == ProfileKind.Phone ? 1 : kind == ProfileKind.SmallDesktop ? 2 : 4;
                LayoutMetrics(columns);
                return $"cards {columns}×{4 / columns}";
            })) failed++;

            // grid: the secondary columns give way on the narrow profiles.
            if (!ApplyRegion("grid", results, () =>
            {
                this.colOwner.Visible = !phone && kind != ProfileKind.Tablet;
                this.colDue.Visible = !phone;
                var hidden = new List<string>();
                if (!this.colOwner.Visible) hidden.Add("Owner");
                if (!this.colDue.Visible) hidden.Add("Due");
                return hidden.Count == 0 ? "all columns" : "hidden " + string.Join(", ", hidden) + " (still in the editor)";
            })) failed++;

            // status bar: the profile label is always there; the theme label yields on phones.
            if (!ApplyRegion("status", results, () =>
            {
                this.lblTheme.Visible = !phone;
                this.lblStatus.Width = phone ? 110 : 200;
                this.tracePanel.Height = phone ? 132 : 176;
                UpdateStatusBar(null);
                return phone ? "profile + size (theme label hidden)" : "profile + size + theme";
            })) failed++;

            AddTrace($"→ render profile \"{name}\": " + string.Join(" · ", results));

            if (failed > 0)
            {
                ShowBanner($"✖ {failed} region(s) failed while applying \"{name}\"; the other {6 - failed} applied. Re-apply recovers.");
                SetStatus("partial profile", StatusKind.Error);
            }
            else if (_simulated)
            {
                SetStatus($"simulating {name}", StatusKind.Warn);
            }
            else if (!this.timerProfiles.Enabled)
            {
                SetStatus("ready", StatusKind.Normal);
            }
        }

        /// <summary>
        /// Runs one region of ApplyKind in its own try/catch so a failure there never stops the next region.
        /// The action returns a short description for the trace.
        /// </summary>
        private bool ApplyRegion(string region, List<string> results, Func<string> apply)
        {
            try
            {
                results.Add($"{region}: {apply()}");
                return true;
            }
            catch (Exception ex)
            {
                results.Add($"{region}: FAILED");
                AddTrace($"• server region \"{region}\" failed: {ex.Message} — the remaining regions still apply");
                return false;
            }
        }

        /// <summary>Maps a profile name to a behaviour. Both spellings of the landscape names are accepted.</summary>
        private static ProfileKind? KindFromName(string name)
        {
            switch ((name ?? string.Empty).Trim())
            {
                case "Phone": return ProfileKind.Phone;
                case "Phone (Landscape)":
                case "Phone Landscape": return ProfileKind.PhoneLandscape;
                case "Tablet": return ProfileKind.Tablet;
                case "Tablet (Landscape)":
                case "Tablet Landscape": return ProfileKind.TabletLandscape;
                case "Small Desktop": return ProfileKind.SmallDesktop;
                case "Desktop":
                case "Default": return ProfileKind.Desktop;
                default: return null;
            }
        }

        private IEnumerable<Button> ToolbarButtons()
        {
            yield return this.btnRefresh;
            yield return this.btnProfilePhone;
            yield return this.btnProfileTablet;
            yield return this.btnProfileDesktop;
            yield return this.btnStepProfiles;
            yield return this.btnUnknownProfile;
            yield return this.btnFaultyRegion;
            yield return this.btnReapply;
        }

        /// <summary>
        /// Re-arranges the four metric cards in the TableLayoutPanel: the cards keep Dock=Fill in their cells,
        /// only the grid shape (columns × rows) and the cell positions change. Height follows the row count.
        /// </summary>
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

        #endregion

        #region Details editor hosting: docked card ↔ phone dialog

        /// <summary>Moves the single editor instance into the docked details card (no-op when it is already there).</summary>
        private void HostEditorInPanel()
        {
            if (this.ticketEditor.Parent == this.detailsCard)
                return;

            if (_editorForm != null && _editorForm.Visible)
            {
                AddTrace("• server closing the editor dialog: the profile is no longer a phone (nothing typed is lost — the editor is the same control)");
                _editorForm.Close();
            }

            this.ticketEditor.Parent?.Controls.Remove(this.ticketEditor);
            this.detailsCard.Controls.Add(this.ticketEditor);
            this.ticketEditor.Dock = DockStyle.Fill;
            this.ticketEditor.CloseButtonVisible = false;
        }

        /// <summary>Moves the single editor instance into the reusable dialog (no-op when it is already there). Does not open it.</summary>
        private void HostEditorInDialog()
        {
            if (_editorForm == null || _editorForm.IsDisposed)
                _editorForm = new TicketEditorForm();

            if (this.ticketEditor.Parent == _editorForm)
                return;

            _editorForm.HostEditor(this.ticketEditor);
            this.ticketEditor.CloseButtonVisible = true;
        }

        /// <summary>
        /// Opens the dialog once. ShowDialog with the close callback is non-blocking in Wisej.NET; a dialog that is
        /// already visible is left alone (opening it again would throw and, worse, would be a second editor).
        /// </summary>
        private void OpenEditorDialog(string reason)
        {
            HostEditorInDialog();
            if (_editorForm.Visible)
            {
                AddTrace($"• server editor dialog already open ({reason}) — reused, not opened again");
                return;
            }

            _editorForm.WindowState = _appliedKind == ProfileKind.Phone ? FormWindowState.Maximized : FormWindowState.Normal;
            AddTrace($"→ render editor dialog opened ({reason}): TicketEditorForm.ShowDialog, {(_editorForm.WindowState == FormWindowState.Maximized ? "maximized" : "centred 420×620")}");
            _editorForm.ShowDialog((form, result) => AddTrace($"← client editor dialog closed ({result}); the editor keeps its values for the next open"));
        }

        private void ticketEditor_CloseRequested(object sender, EventArgs e)
        {
            if (_editorForm != null && _editorForm.Visible)
                _editorForm.Close();
        }

        #endregion

        #region Success path: simulate a profile / Refresh

        /// <summary>
        /// Phone / Tablet / Desktop: find the ClientProfile object by name in Application.Browser.Profiles and
        /// apply it, exactly as the event would. When the object cannot be found (profiles not loaded), the
        /// console falls back to its internal ProfileKind for the same name and says so in the trace.
        /// </summary>
        private void btnSimulateProfile_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            string name = button?.Tag as string;
            if (string.IsNullOrEmpty(name))
                return;

            SimulateProfile(name, "simulate button");
        }

        private bool SimulateProfile(string name, string reason)
        {
            _simulated = true;
            var profile = FindProfile(name);
            if (profile != null)
                return ApplyProfile(profile, $"{reason}: ClientProfile \"{profile.Name}\" from Application.Browser.Profiles");

            var kind = KindFromName(name);
            if (kind == null)
            {
                _simulated = false;
                string known = string.Join(", ", KnownProfileNames());
                AddTrace($"• server profile \"{name}\" not found ({reason}); known profiles: {known}; keeping \"{_appliedName}\"");
                return false;
            }

            AddTrace($"• server profile object \"{name}\" not in Application.Browser.Profiles ({reason}) — applying the console's internal {kind} behaviour instead");
            ApplyKind(kind.Value, name);
            return true;
        }

        /// <summary>The loaded ClientProfile with this name, or null. Application.Browser.Profiles is the merged list the framework matches.</summary>
        private static ClientProfile FindProfile(string name)
        {
            try
            {
                var profiles = Application.Browser.Profiles;
                return profiles?.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.Ordinal));
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static IEnumerable<string> KnownProfileNames()
        {
            try
            {
                var profiles = Application.Browser.Profiles;
                if (profiles != null && profiles.Length > 0)
                    return profiles.Select(p => p.Name).ToList();
            }
            catch (Exception)
            {
            }
            return Enum.GetNames(typeof(ProfileKind));
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadTickets("Refresh");
            if (!_simulated && !this.timerProfiles.Enabled)
                SetStatus("ready", StatusKind.Normal);
        }

        /// <summary>
        /// Re-reads everything from the repository: the four metric cards, the grid and, when a ticket is
        /// selected, the editor. The server is the source of truth. Re-selection never opens the phone dialog:
        /// only a user's row selection does.
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
                ShowTicket((string)this.gridTickets.Rows[reselect].Tag, reason, userInitiated: false);
            else
                ClearEditor();
        }

        #endregion

        #region Progress path: Step profiles (Wisej.Web.Timer)

        private void btnStepProfiles_Click(object sender, EventArgs e)
        {
            if (this.timerProfiles.Enabled)
                return;

            _stepIndex = 0;
            this.btnStepProfiles.Enabled = false;
            HideBanner();
            SetStatus($"stepping 0/{StepSequence.Length}", StatusKind.Warn);
            AddTrace($"• server step-through started: {string.Join(" → ", StepSequence)} then back to Application.ActiveProfile, one every {this.timerProfiles.Interval} ms (Wisej.Web.Timer, no client code)");
            this.timerProfiles.Start();
        }

        private void timerProfiles_Tick(object sender, EventArgs e)
        {
            if (_stepIndex < StepSequence.Length)
            {
                string name = StepSequence[_stepIndex];
                _stepIndex++;
                SimulateProfile(name, $"step {_stepIndex}/{StepSequence.Length}");
                SetStatus($"stepping {_stepIndex}/{StepSequence.Length} · {name}", StatusKind.Warn);
                return;
            }

            // Last tick: leave the console in the state the browser really has.
            this.timerProfiles.Stop();
            this.btnStepProfiles.Enabled = true;
            Reapply("step-through finished");
        }

        #endregion

        #region Failure paths and recovery

        /// <summary>Failure 1: a profile name that is in nobody's ClientProfiles.json. Nothing changes, the miss is logged.</summary>
        private void btnUnknownProfile_Click(object sender, EventArgs e)
        {
            string before = _appliedName;
            bool applied = SimulateProfile("Kiosk", "Unknown button");
            if (!applied)
            {
                ShowBanner($"✖ Profile \"Kiosk\" does not exist in ClientProfiles.json. Nothing changed: the console still shows \"{before}\". Add the profile (narrow rules first) or fix the name.");
                SetStatus("unknown profile", StatusKind.Error);
            }
        }

        /// <summary>
        /// Failure 2: the metrics region throws inside ApplyProfile. The per-region try/catch means the other
        /// five regions still apply and the trace names the one that did not. The fault is one-shot.
        /// </summary>
        private void btnFaultyRegion_Click(object sender, EventArgs e)
        {
            _faultInRegion = "metrics";
            AddTrace("• server fault injected into the \"metrics\" region for the next ApplyProfile");
            ApplyKind(_appliedKind, _appliedName);
        }

        /// <summary>Recovery: read Application.ActiveProfile again and apply it; any simulation ends.</summary>
        private void btnReapply_Click(object sender, EventArgs e)
        {
            if (this.timerProfiles.Enabled)
            {
                this.timerProfiles.Stop();
                this.btnStepProfiles.Enabled = true;
                AddTrace("• server step-through stopped by Re-apply");
            }

            Reapply("Re-apply button");
        }

        private void Reapply(string reason)
        {
            _simulated = false;
            _faultInRegion = null;
            HideBanner();
            var active = Application.ActiveProfile;
            AddTrace($"• server recovery ({reason}): Application.ActiveProfile re-read → \"{active?.Name ?? "(null → Default)"}\"");
            ApplyProfile(active, reason);
            SetStatus("ready", StatusKind.Normal);
        }

        #endregion

        #region Details editor: selection, save, validation

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

            ShowTicket((string)row.Tag, "grid selection", userInitiated: true);
        }

        /// <summary>
        /// Fills the editor from the repository copy of the ticket. On the Phone profiles a user-initiated
        /// selection also opens the editor dialog — that is the phone's replacement for the hidden side panel.
        /// </summary>
        private void ShowTicket(string id, string reason, bool userInitiated)
        {
            var t = _repository.Get(id);
            if (t == null)
            {
                ClearEditor();
                return;
            }

            _selectedId = t.Id;
            this.ticketEditor.LoadTicket(t);
            AddTrace($"← client {reason}: ticket {t.Id} \"{t.Title}\" loaded into the editor ({(this.ticketEditor.Parent == _editorForm ? "in the dialog" : "docked")})");

            bool phone = _appliedKind == ProfileKind.Phone || _appliedKind == ProfileKind.PhoneLandscape;
            if (phone && userInitiated)
                OpenEditorDialog("row selected on a phone");
        }

        private void ClearEditor()
        {
            _selectedId = null;
            this.ticketEditor.Clear();
        }

        private void ticketEditor_SaveRequested(object sender, EventArgs e)
        {
            if (_selectedId == null)
            {
                this.ticketEditor.ShowMessage("Select a ticket in the grid before saving.", true);
                SetStatus("nothing selected", StatusKind.Warn);
                AddTrace("• server save skipped: no ticket selected");
                return;
            }

            SaveTicket(this.ticketEditor.ReadTicket(), "Save");
        }

        /// <summary>
        /// The only write path. TicketRepository.Save validates on the server and throws TicketValidationException.
        /// Feedback goes into the editor itself (visible inside the phone dialog) as well as the banner and a toast.
        /// </summary>
        private bool SaveTicket(Ticket ticket, string origin)
        {
            try
            {
                var saved = _repository.Save(ticket);

                HideBanner();
                AddTrace($"• server saved {saved.Id} \"{saved.Title}\" ({saved.Priority}, {saved.Status}, {saved.Owner}, due {saved.DueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)})");
                LoadTickets(origin);
                this.ticketEditor.ShowMessage($"✓ {saved.Id} saved on the server", false);
                SetStatus($"saved {saved.Id}", StatusKind.Normal);
                AlertBox.Show($"{saved.Id} saved.", MessageBoxIcon.Information,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return true;
            }
            catch (TicketValidationException ex)
            {
                // The server refused the ticket: nothing was written, the grid and the metrics still show the
                // last good state, and the reason is shown where the editor is.
                this.ticketEditor.ShowMessage("✖ " + ex.Message + " Nothing was written.", true);
                ShowBanner($"✖ Rejected on the server ({origin}): {ex.Message} Nothing was written.");
                AddTrace($"• server rejected {ticket.Id ?? "(none)"} ({origin}): {ex.Message}");
                SetStatus("validation error", StatusKind.Error);
                AlertBox.Show(ex.Message, MessageBoxIcon.Warning,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return false;
            }
        }

        #endregion

        #region Navigation rail and the phone menu

        private void navigationRail_SectionSelected(object sender, SectionEventArgs e)
        {
            this.lblWorkspaceTitle.Text = e.Section == "Dashboard" ? "Tickets" : e.Section;
            AddTrace($"← client navigation: {e.Section} (rail {(this.navigationPanel.Visible ? this.navigationRail.Mode.ToString() : "hidden → phone menu")})");
        }

        /// <summary>
        /// The phone's replacement for the hidden rail: the same five sections as a ContextMenu anchored to the
        /// button. Each item raises the rail's SectionSelected, so the navigation handler is the same code.
        /// </summary>
        private void btnMenu_Click(object sender, EventArgs e)
        {
            var menu = new ContextMenu();
            foreach (var section in this.navigationRail.Sections)
            {
                string name = section;
                menu.MenuItems.Add(new MenuItem(name, (s, a) => this.navigationRail.Select(name)));
            }

            AddTrace("← client Menu: " + this.navigationRail.Sections.Count + " sections as a ContextMenu (the rail is hidden on this profile)");
            menu.Show(this.btnMenu, Placement.BottomLeft, m => m.Dispose());
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

            // The three status colours are control properties here, as in Module 1; Module 2 moved them into
            // theme colour tokens (success / warning / danger).
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
