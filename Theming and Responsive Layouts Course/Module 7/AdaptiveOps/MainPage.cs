using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using AdaptiveOps.Dialogs;
using AdaptiveOps.Governance;
using AdaptiveOps.Models;
using AdaptiveOps.Views;
using Wisej.Web;

namespace AdaptiveOps
{
    /// <summary>
    /// Adaptive Operations Console — Module 7 capstone (Mobile-Ready Capstone, Accessibility, Performance
    /// and Production Governance). This is the finished console: every layer of the visual system in place.
    ///
    ///   theme        Themes/AdaptiveOps.theme (Bootstrap-4 derived; tokens, fonts, focus frame in settings,
    ///                semantic appearances action-button, destructive-button, nav-item, metric-card, compact-editor,
    ///                surface-card, status-label …, table-header-cell and the textbox invalid state) selected in Default.json
    ///   AppearanceKey every business variant on screen is a semantic appearance; no BackColor/ForeColor/Font in code
    ///   CssClass     Styles/AdaptiveOps.css, linked from Default.html, for app-specific typography only
    ///   CssStyle     not used (the governance review fails when one appears outside the documented allow-list)
    ///   containers   Dock for the five regions and the workspace stack, FlowLayoutPanel for the toolbar commands and the
    ///                metric cards, TableLayoutPanel for the aligned editor and the cost bar, FlexLayoutPanel for the
    ///                proportional grid/trace split; MinimumSize/MaximumSize on every fill or docked region, no resize code
    ///   profiles     ClientProfiles.json (Phone → Phone (Landscape) → Tablet → Tablet (Landscape) → Small Desktop → Desktop)
    ///                applied by one idempotent ApplyProfile(name) called from the constructor and from
    ///                Application.ResponsiveProfileChanged; the one behaviour a property cannot express is the phone
    ///                details editor opening as a modal DetailsDialog
    ///
    /// Capstone additions: accessibility (TabIndex order across the shell, focus frame from the theme, ToolTipText and
    /// AccessibleName on every icon-only button, validation announced as text + icon), performance (the "Layout cost"
    /// card counts controls per region, Bulk fill times SuspendLayout/ResumeLayout with a Stopwatch, the Reports view is
    /// created lazily on first navigation) and governance (the production checklist computed at runtime by
    /// Governance/GovernanceReview.cs and shown in the Governance review dialog).
    ///
    /// Toolbar buttons exercise the four lab paths:
    ///   Governance review    success   run the checklist against the running app — all rules green
    ///   Review all profiles  progress  a Wisej.Web.Timer applies Phone → Tablet → Small Desktop → Desktop and logs the checklist per profile
    ///   Inject violation     failure   hard-coded BackColor, a CssStyle, an unknown CssClass, the base theme reloaded, and one
    ///                                  simulated failure armed inside ApplyProfile — the review shows the red items
    ///   Recover              recovery  violations cleared, AdaptiveOps theme reloaded, review re-run
    /// Every decision is logged in the "Layout & theme · live trace" card.
    /// </summary>
    public partial class MainPage : Page
    {
        private const string ThemeName = GovernanceReview.ExpectedThemeName;
        private const string BaseThemeName = "Bootstrap-4";
        private const int BulkFillRepeats = 20;

        /// <summary>Test mode shows the profile indicator in the status bar. ADAPTIVEOPS_TESTMODE=0 hides it; unset means on.</summary>
        private static readonly bool TestMode = ReadTestMode();

        private static readonly string[] ReviewProfiles = { "Phone", "Tablet", "Small Desktop", "Desktop" };

        private readonly TicketRepository _repository = new TicketRepository();

        /// <summary>Id of the ticket shown in the details editor, or null.</summary>
        private string _selectedId;

        /// <summary>True while the grid is being refilled, so SelectionChanged does not re-enter.</summary>
        private bool _suppressSelection;

        /// <summary>The profile ApplyProfile last finished successfully; the early-return guard of the handler.</summary>
        private string _lastProfile;

        /// <summary>Armed by "Inject violation": the next ApplyProfile throws once, half-way through.</summary>
        private bool _throwOnceInApplyProfile;

        /// <summary>Guards the re-apply that runs after a failed ApplyProfile against recursion.</summary>
        private bool _recoveringProfile;

        /// <summary>Progress path: index into <see cref="ReviewProfiles"/>.</summary>
        private int _reviewStep = -1;
        private readonly List<string> _reviewSummary = new List<string>();
        private int _reviewFailures;

        /// <summary>Created on first navigation to Reports (lazy).</summary>
        private ReportsView _reportsView;
        private string _currentView = "Dashboard";
        private string _bannerText;
        private int _controlsAtLoad;

        public MainPage()
        {
            InitializeComponent();

            this.lblProfile.Visible = TestMode;

            // Session-level events: both are unsubscribed in Dispose(bool) (MainPage.Designer.cs).
            Application.BrowserSizeChanged += this.Application_BrowserSizeChanged;
            Application.ResponsiveProfileChanged += this.Application_ResponsiveProfileChanged;

            // First paint: the constructor applies the active profile once; the handler applies later changes.
            ApplyProfile(ProfileNameOf(Application.ActiveProfile));
            ReportWidth("constructor");
        }

        #region Shell: load, resize, theme

        private void MainPage_Load(object sender, EventArgs e)
        {
            AddTrace("• server shell built: " + DescribeShell());

            string theme = CurrentThemeName();
            this.lblTheme.Text = "theme: " + theme;
            AddTrace(theme == ThemeName
                ? $"• server theme: {theme} — custom theme from Themes/AdaptiveOps.theme (Default.json \"theme\"), derived from {BaseThemeName}"
                : $"• server theme: {theme} — WARNING: expected {ThemeName}; check Default.json and Themes/AdaptiveOps.theme");
            AddTrace(TestMode
                ? "• server test mode on (ADAPTIVEOPS_TESTMODE): the profile indicator is visible in the status bar"
                : "• server test mode off: the profile indicator is hidden (set ADAPTIVEOPS_TESTMODE=1 to show it)");

            LoadTickets("page load");
            ReportWidth("page load");

            _controlsAtLoad = ControlTree.Count(this);
            UpdateLayoutCost("page load");

            var results = RunReview();
            int failed = results.Count(r => !r.Pass);
            AddTrace($"• server governance review at startup: {results.Count - failed}/{results.Count} rules pass" + (failed > 0 ? " — " + string.Join(", ", results.Where(r => !r.Pass).Select(r => r.Id)) : string.Empty));
            SetStatus(failed == 0 ? "ready · governance green" : $"ready · {failed} governance rule(s) failing", failed == 0 ? StatusKind.Ok : StatusKind.Warn);
        }

        private void Application_BrowserSizeChanged(object sender, EventArgs e)
        {
            ReportWidth("Application.BrowserSizeChanged");
        }

        /// <summary>
        /// The one profile handler. It only forwards the event's current profile to ApplyProfile, which
        /// returns early when nothing changed — running it twice after a refresh changes nothing.
        /// </summary>
        private void Application_ResponsiveProfileChanged(object sender, ResponsiveProfileChangedEventArgs e)
        {
            AddTrace($"← client profile changed: {ProfileNameOf(e.PreviousProfile)} → {ProfileNameOf(e.CurrentProfile)} (Application.ResponsiveProfileChanged)");
            ApplyProfile(ProfileNameOf(e.CurrentProfile));
            ReportWidth("profile changed");
        }

        /// <summary>The Page fills the browser viewport, so its Resize fires on every window resize. It only reports.</summary>
        private void MainPage_Resize(object sender, EventArgs e)
        {
            ReportWidth("Page.Resize");
        }

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
                AddTrace($"← client resize ({reason}): browser {size.Width}×{size.Height} · page {this.Width}×{this.Height} · workspace {this.workspacePanel.Width}×{this.workspacePanel.Height} · profile {_lastProfile ?? "(none)"}");
            }
            catch (Exception ex)
            {
                this.lblBrowserWidth.Text = "Browser size unavailable — " + ex.Message;
                AddTrace($"• server width read failed ({reason}): {ex.Message}");
            }
        }

        private string DescribeShell()
        {
            return string.Join(" · ",
                $"toolbar Dock={this.toolbarPanel.Dock} {this.toolbarPanel.Height} px (FlowLayoutPanel)",
                $"navigation Dock={this.navigationPanel.Dock} {this.navigationPanel.Width} px",
                $"details Dock={this.detailsPanel.Dock} {this.detailsPanel.Width} px (TableLayoutPanel editor)",
                $"status Dock={this.statusPanel.Dock} {this.statusPanel.Height} px",
                $"workspace Dock=Fill min {this.workspacePanel.MinimumSize.Width}×{this.workspacePanel.MinimumSize.Height} (Flow cards + Flex grid/trace)");
        }

        private static string CurrentThemeName()
        {
            try { return Application.Theme?.Name ?? "(default)"; }
            catch (Exception) { return "(default)"; }
        }

        private static bool ReadTestMode()
        {
            string value = Environment.GetEnvironmentVariable("ADAPTIVEOPS_TESTMODE");
            return string.IsNullOrEmpty(value) || value != "0";
        }

        #endregion

        #region Profiles: one idempotent ApplyProfile

        /// <summary>"Desktop" for the framework's default profile, otherwise the profile's own name.</summary>
        private static string ProfileNameOf(Wisej.Core.ClientProfile profile)
        {
            if (profile == null || string.IsNullOrEmpty(profile.Name) || profile.Name == "Default")
                return "Desktop";
            return profile.Name;
        }

        /// <summary>
        /// Assigns the final layout values for a profile. Idempotent and cheap: it returns early when the
        /// profile has not changed, sets final values instead of toggling, never rebuilds the UI and never
        /// reloads data. A failure half-way through is caught, reported in the status region and undone by
        /// re-applying the last good profile.
        /// </summary>
        /// <param name="name">Profile name from ClientProfiles.json ("Phone", "Tablet (Landscape)", "Desktop" …).</param>
        /// <param name="force">Re-apply even when unchanged (used by the governance review's idempotence check).</param>
        /// <param name="quiet">Suppress the trace line (the idempotence check runs it as a probe).</param>
        private void ApplyProfile(string name, bool force = false, bool quiet = false)
        {
            name = string.IsNullOrEmpty(name) ? "Desktop" : name;
            if (!force && string.Equals(name, _lastProfile, StringComparison.Ordinal))
            {
                if (!quiet)
                    AddTrace($"• server ApplyProfile({name}) skipped: same profile, no work");
                return;
            }

            var sw = Stopwatch.StartNew();
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
                    // Navigation rail: text on desktop, icon-only (Button.Display = Icon) on every narrow profile.
                    this.navRail.IconOnly = narrow;
                    this.navigationPanel.Width = desktop ? 220 : (phone ? 60 : 68);

                    // Armed by "Inject violation": fail here, half-way, so the catch below has something real to undo.
                    if (_throwOnceInApplyProfile)
                    {
                        _throwOnceInApplyProfile = false;
                        throw new InvalidOperationException("simulated failure inside ApplyProfile (armed by Inject violation)");
                    }

                    // Details region: docked right (desktop, landscape tablet, small desktop), docked under the grid
                    // (portrait tablet), hidden on phone where "Open details" opens the editor as a dialog instead.
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

                    // Metric cards: the FlowLayoutPanel wraps; the profile only sets the row budget (1, 2 or 3 rows of 84 + 8).
                    this.metricsFlow.Height = (desktop || phoneLandscape) ? 92 : (phone ? 276 : 184);

                    // Grid/trace split: side by side, stacked on a portrait phone (FlexLayoutStyle, not Bounds).
                    this.contentFlex.LayoutStyle = (phone && !phoneLandscape) ? FlexLayoutStyle.Vertical : FlexLayoutStyle.Horizontal;

                    // Grid columns: fewer on narrow profiles (DataGridViewColumn.Visible).
                    this.colPriority.Visible = !phone;
                    this.colOwner.Visible = desktop;
                    this.colDue.Visible = !phone;

                    // Toolbar commands: icon-only on phone and portrait tablet; tooltips and accessible names stay.
                    var display = iconToolbar ? Display.Icon : Display.Both;
                    foreach (var button in ToolbarButtons())
                    {
                        button.Display = display;
                        button.Width = iconToolbar ? 40 : 150;
                    }
                    this.lblAppTitle.Visible = !phone;

                    this.workspacePanel.MinimumSize = phone ? new System.Drawing.Size(240, 200) : new System.Drawing.Size(320, 240);
                }
                finally
                {
                    this.ResumeLayout(true);
                }

                _lastProfile = name;
                this.lblProfile.Text = "profile: " + name;
                sw.Stop();
                if (!quiet)
                    AddTrace($"• server ApplyProfile({name}) in {sw.Elapsed.TotalMilliseconds.ToString("F1", CultureInfo.InvariantCulture)} ms: {DescribeProfileLayout()}");
            }
            catch (Exception ex)
            {
                // Friendly failure: say what happened in the status region, then put the shell back on the last
                // good profile so nothing stays half-applied. _lastProfile is not advanced, so the next event retries.
                SetStatus($"profile change to {name} failed — kept {_lastProfile ?? "startup layout"}", StatusKind.Error);
                AddTrace($"• server ApplyProfile({name}) FAILED half-way: {ex.Message} — re-applying {_lastProfile ?? "(none)"} so the shell is consistent");
                ShowBanner($"Profile change to {name} failed: {ex.Message} The previous layout was restored.");

                if (_lastProfile != null && !_recoveringProfile)
                {
                    _recoveringProfile = true;
                    try { ApplyProfile(_lastProfile, force: true, quiet: true); }
                    finally { _recoveringProfile = false; }
                }
            }
        }

        private Button[] ToolbarButtons() => new[] { this.btnReview, this.btnReviewProfiles, this.btnInjectViolation, this.btnRecover, this.btnBulkFill, this.btnClearTrace };

        /// <summary>One line with what the profile produced — the "checks" column of the QA matrix.</summary>
        private string DescribeProfileLayout()
        {
            int visibleColumns = this.gridTickets.Columns.Cast<DataGridViewColumn>().Count(c => c.Visible);
            string details = this.detailsPanel.Visible ? $"docked {this.detailsPanel.Dock}" : "hidden → Open details dialog";
            return $"rail {this.navRail.Describe()} · details {details} · cards {this.metricsFlow.Height / 92} row(s) · grid {visibleColumns}/6 columns · toolbar {this.btnReview.Display} · content {this.contentFlex.LayoutStyle}";
        }

        /// <summary>Everything ApplyProfile assigns, as one string: two identical snapshots prove idempotence.</summary>
        private string ProfileSnapshot()
        {
            return string.Join("|",
                this.navigationPanel.Width, this.navRail.IconOnly,
                this.detailsPanel.Visible, this.detailsPanel.Dock, this.detailsPanel.Width, this.detailsPanel.Height, this.detailsPanel.MinimumSize, this.detailsPanel.MaximumSize,
                this.detailsEditor.Compact, this.btnOpenDetails.Visible,
                this.metricsFlow.Height, this.contentFlex.LayoutStyle,
                this.colPriority.Visible, this.colOwner.Visible, this.colDue.Visible,
                this.btnReview.Display, this.btnReview.Width, this.lblAppTitle.Visible,
                this.workspacePanel.MinimumSize);
        }

        #endregion

        #region Governance review (success path) and the review context

        private void btnReview_Click(object sender, EventArgs e)
        {
            ShowReview("Governance review");
        }

        /// <summary>Runs the checklist, traces every rule, shows the dialog and reports in the status region.</summary>
        private void ShowReview(string origin)
        {
            var results = RunReview();
            int failed = results.Count(r => !r.Pass);

            AddTrace($"• server governance review ({origin}) on profile {_lastProfile}: {results.Count - failed}/{results.Count} rules pass");
            foreach (var r in results)
                AddTrace($"   {r}");

            var dialog = new GovernanceDialog();
            dialog.ShowResults(results, _lastProfile, origin + " · " + CurrentThemeName() + " theme");
            dialog.ShowDialog((form, result) =>
            {
                AddTrace("← client governance review closed");
                form.Dispose();
            });

            this.lblProgress.Text = failed == 0 ? $"Governance review: all {results.Count} rules pass" : $"Governance review: {failed} of {results.Count} rules FAIL";
            SetStatus(failed == 0 ? "governance green" : $"{failed} governance rule(s) failing", failed == 0 ? StatusKind.Ok : StatusKind.Error);
        }

        private IReadOnlyList<GovernanceResult> RunReview()
        {
            var context = new GovernanceContext
            {
                Root = this,
                ProfileName = _lastProfile,
                ResolveProjectFile = ResolveProjectFile,
                ValidationLabel = this.detailsEditor.ValidationLabel,
                BoundedRegion = this.detailsPanel,
                ReapplyProfile = () =>
                {
                    string before = ProfileSnapshot();
                    ApplyProfile(_lastProfile, force: true, quiet: true);
                    return (before, ProfileSnapshot());
                },
            };
            context.StableRegions.Add(new KeyValuePair<string, Control>("workspace", this.workspacePanel));
            context.StableRegions.Add(new KeyValuePair<string, Control>("gridCard", this.gridCard));
            context.StableRegions.Add(new KeyValuePair<string, Control>("gridTickets", this.gridTickets));
            context.StableRegions.Add(new KeyValuePair<string, Control>("traceCard", this.traceCard));
            context.StableRegions.Add(new KeyValuePair<string, Control>("detailsPanel", this.detailsPanel));
            return GovernanceReview.Run(context);
        }

        /// <summary>
        /// Finds a project file at runtime: the project folder is the working directory under dotnet run and
        /// Application.StartupPath under the Wisej host; a published build has the governed files next to the assembly.
        /// </summary>
        private static string ResolveProjectFile(string relativePath)
        {
            var candidates = new List<string>();
            try { candidates.Add(Application.StartupPath); } catch (Exception) { /* not available outside a request */ }
            candidates.Add(Directory.GetCurrentDirectory());
            string dir = AppContext.BaseDirectory;
            for (int i = 0; i < 5 && !string.IsNullOrEmpty(dir); i++)
            {
                candidates.Add(dir);
                dir = Path.GetDirectoryName(dir.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            }

            foreach (var candidate in candidates.Where(c => !string.IsNullOrEmpty(c)))
            {
                string full = Path.Combine(candidate, relativePath.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(full))
                    return full;
            }
            return null;
        }

        #endregion

        #region Progress path: review across every profile (Wisej.Web.Timer)

        private void btnReviewProfiles_Click(object sender, EventArgs e)
        {
            if (this.timerReview.Enabled)
                return;

            _reviewStep = -1;
            _reviewSummary.Clear();
            _reviewFailures = 0;
            this.btnReviewProfiles.Enabled = false;
            this.lblProgress.Text = $"Review across every profile · 0 of {ReviewProfiles.Length}";
            SetStatus("reviewing every profile", StatusKind.Warn);
            AddTrace($"• server review across every profile started: {string.Join(" → ", ReviewProfiles)} then back to {ProfileNameOf(Application.ActiveProfile)} (Wisej.Web.Timer, {this.timerReview.Interval} ms per step)");
            this.timerReview.Start();
        }

        private void timerReview_Tick(object sender, EventArgs e)
        {
            _reviewStep++;
            if (_reviewStep < ReviewProfiles.Length)
            {
                string name = ReviewProfiles[_reviewStep];
                ApplyProfile(name);
                var results = RunReview();
                int failed = results.Count(r => !r.Pass);
                string failing = failed == 0 ? "all green" : "FAIL " + string.Join(", ", results.Where(r => !r.Pass).Select(r => r.Id));

                AddTrace($"→ render review {_reviewStep + 1}/{ReviewProfiles.Length} · {name}: {results.Count - failed}/{results.Count} rules ({failing}) · {DescribeProfileLayout()}");
                this.lblProgress.Text = $"Review across every profile · {_reviewStep + 1} of {ReviewProfiles.Length} · {name}: {results.Count - failed}/{results.Count}";
                _reviewSummary.Add($"{name} {results.Count - failed}/{results.Count}");
                _reviewFailures += failed;
                return;
            }

            this.timerReview.Stop();
            string real = ProfileNameOf(Application.ActiveProfile);
            ApplyProfile(real);
            this.btnReviewProfiles.Enabled = true;
            this.lblProgress.Text = "Review complete · " + string.Join(" · ", _reviewSummary) + $" · back on {real}";
            bool allGreen = _reviewFailures == 0;
            SetStatus(allGreen ? "every profile green" : "review finished with failures — see trace", allGreen ? StatusKind.Ok : StatusKind.Error);
            AddTrace("• server review across every profile finished: " + this.lblProgress.Text);
        }

        #endregion

        #region Failure path and recovery

        /// <summary>
        /// Breaks the rules on purpose so the review has red items: a hard-coded BackColor on one card, a
        /// CssStyle on a second, an undefined CssClass on a third, the base theme loaded in place of the custom
        /// one, and one failure armed inside ApplyProfile for the next profile change.
        /// </summary>
        private void btnInjectViolation_Click(object sender, EventArgs e)
        {
            this.cardOpen.BackColor = System.Drawing.Color.FromArgb(255, 244, 229);
            AddTrace("✕ injected: cardOpen.BackColor = #FFF4E5 — a colour the theme should own (metric-card appearance)");

            this.cardOverdue.CssStyle = "border-color: #F59E0B";
            AddTrace("✕ injected: cardOverdue.CssStyle = \"border-color: #F59E0B\" — inline style for a value that is not dynamic");

            this.cardMine.CssClass = "metric-card legacy-card";
            AddTrace("✕ injected: cardMine.CssClass += legacy-card — a class Styles/AdaptiveOps.css does not define");

            try
            {
                Application.LoadTheme(BaseThemeName);
                this.lblTheme.Text = "theme: " + CurrentThemeName();
                AddTrace($"✕ injected: Application.LoadTheme(\"{BaseThemeName}\") — the base theme replaces the custom one, and LoadTheme is global: every session sees it");
            }
            catch (Exception ex)
            {
                AddTrace($"• server LoadTheme(\"{BaseThemeName}\") failed: {ex.Message}");
            }

            _throwOnceInApplyProfile = true;
            AddTrace("✕ armed: the next ApplyProfile throws once half-way (run \"Review all profiles\" or change the browser profile to see the friendly failure)");

            ShowBanner("Violations injected: hard-coded BackColor, CssStyle, unknown CssClass, base theme, and one armed ApplyProfile failure. Recover clears them.");
            ShowReview("Inject violation");
        }

        private void btnRecover_Click(object sender, EventArgs e)
        {
            if (this.timerReview.Enabled)
            {
                this.timerReview.Stop();
                this.btnReviewProfiles.Enabled = true;
            }

            this.cardOpen.ResetBackColor();
            this.cardOverdue.CssStyle = string.Empty;
            this.cardMine.CssClass = "metric-card";
            _throwOnceInApplyProfile = false;

            try
            {
                if (CurrentThemeName() != ThemeName)
                    Application.LoadTheme(ThemeName);
                this.lblTheme.Text = "theme: " + CurrentThemeName();
            }
            catch (Exception ex)
            {
                AddTrace($"• server LoadTheme(\"{ThemeName}\") failed: {ex.Message}");
            }

            HideBanner();
            this.detailsEditor.ClearValidation();
            AddTrace("• server recovered: ResetBackColor(), CssStyle cleared, CssClass restored, AdaptiveOps theme reloaded, ApplyProfile failure disarmed");

            _repository.Reset();
            LoadTickets("Recover");
            ApplyProfile(ProfileNameOf(Application.ActiveProfile), force: true);
            ShowReview("Recover");
        }

        #endregion

        #region Performance: layout cost, bulk fill timing, lazy Reports view

        /// <summary>Recomputes the "Layout cost" card: controls per region and in total, and how many were created since load.</summary>
        private void UpdateLayoutCost(string reason)
        {
            int toolbar = ControlTree.Count(this.toolbarPanel);
            int rail = ControlTree.Count(this.navigationPanel);
            int workspace = ControlTree.Count(this.workspacePanel);
            int details = ControlTree.Count(this.detailsPanel);
            int status = ControlTree.Count(this.statusPanel);
            int total = ControlTree.Count(this);
            int created = _controlsAtLoad == 0 ? 0 : total - _controlsAtLoad;

            string breakdown = $"toolbar {toolbar} · rail {rail} · workspace {workspace} · details {details} · status {status} · created since load +{created}";
            this.cardCost.Update(total, breakdown);
            AddTrace($"• server layout cost ({reason}): {total} controls · {breakdown}");
        }

        /// <summary>
        /// Fills the grid with 240 rows twice and times both on the server: plain Rows.Add versus the same
        /// inside SuspendLayout()/ResumeLayout(true). The Stopwatch measures server work; the client receives
        /// one batched update per request either way — the point is to keep the server-side handler cheap.
        /// </summary>
        private void btnBulkFill_Click(object sender, EventArgs e)
        {
            var tickets = _repository.GetAll();
            int rows = tickets.Count * BulkFillRepeats;

            _suppressSelection = true;
            try
            {
                var sw = Stopwatch.StartNew();
                FillGridRepeated(tickets, BulkFillRepeats);
                var plain = sw.Elapsed;

                sw.Restart();
                this.gridTickets.SuspendLayout();
                try
                {
                    FillGridRepeated(tickets, BulkFillRepeats);
                }
                finally
                {
                    this.gridTickets.ResumeLayout(true);
                }
                var suspended = sw.Elapsed;

                string plainMs = plain.TotalMilliseconds.ToString("F1", CultureInfo.InvariantCulture);
                string suspendedMs = suspended.TotalMilliseconds.ToString("F1", CultureInfo.InvariantCulture);
                AddTrace($"• server bulk fill {rows} rows: plain {plainMs} ms · SuspendLayout/ResumeLayout(true) {suspendedMs} ms (Stopwatch, server side; one client update per request either way)");
                this.lblProgress.Text = $"Bulk fill {rows} rows: {plainMs} ms plain · {suspendedMs} ms suspended";
            }
            finally
            {
                _suppressSelection = false;
            }

            LoadTickets("bulk fill reset");
        }

        private void FillGridRepeated(IReadOnlyList<Ticket> tickets, int repeats)
        {
            this.gridTickets.Rows.Clear();
            for (int i = 0; i < repeats; i++)
            {
                foreach (var t in tickets)
                {
                    this.gridTickets.Rows.Add(new object[]
                    {
                        t.Id + "-" + i.ToString(CultureInfo.InvariantCulture),
                        t.Title,
                        t.Priority.ToString(),
                        t.Status.ToString(),
                        t.Owner,
                        t.DueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                    });
                }
            }
        }

        private void navRail_Navigated(object sender, string view)
        {
            ShowView(view);
        }

        /// <summary>
        /// Switches the workspace between the dashboard (cards, banner, grid/trace) and the Reports view.
        /// The Reports view is created on the first request only — lazy creation is the performance point.
        /// </summary>
        private void ShowView(string view)
        {
            _currentView = view;
            bool reports = view == "Reports";

            if (reports)
            {
                if (_reportsView == null)
                {
                    var sw = Stopwatch.StartNew();
                    int before = ControlTree.Count(this);

                    _reportsView = new ReportsView
                    {
                        Dock = DockStyle.Fill,
                        Name = "reportsView",
                        TabIndex = 4,
                        Visible = false,
                    };
                    this.workspacePanel.Controls.Add(_reportsView);
                    int cells = _reportsView.Bind(_repository.GetAll(), DateTime.Today,
                        $"Created lazily at {DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)} on first navigation");

                    int after = ControlTree.Count(this);
                    AddTrace($"→ render Reports view created lazily: +{after - before} controls ({cells} data cells) in {sw.Elapsed.TotalMilliseconds.ToString("F1", CultureInfo.InvariantCulture)} ms · total now {after}");
                }
                else
                {
                    _reportsView.Bind(_repository.GetAll(), DateTime.Today, _reportsView.Controls.Count > 0 ? "Re-bound from the repository (view reused, not rebuilt)" : string.Empty);
                    AddTrace("→ render Reports view shown again: reused, re-bound from the repository, no controls created");
                }
            }

            this.metricsFlow.Visible = !reports;
            this.contentFlex.Visible = !reports;
            this.bannerPanel.Visible = !reports && _bannerText != null;
            if (_reportsView != null)
                _reportsView.Visible = reports;

            this.lblWorkspaceTitle.Text = view == "Dashboard" ? "Tickets" : view;
            AddTrace($"← client navigation: {view}" + (view == "Settings" || view == "Help" ? " (placeholder: the grid card is retitled; no view is built for it)" : string.Empty));
            UpdateLayoutCost("navigation " + view);
        }

        #endregion

        #region Tickets: grid, editor, dialog, save

        private void LoadTickets(string reason)
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

            AddTrace($"→ render {reason}: {tickets.Count} tickets · open {this.cardOpen.Value} · overdue {this.cardOverdue.Value} · mine {this.cardMine.Value} · closed this week {this.cardClosed.Value}");

            if (reselect >= 0)
                ShowTicket((string)this.gridTickets.Rows[reselect].Tag, reason);
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

            ShowTicket((string)row.Tag, "grid selection");
        }

        private void ShowTicket(string id, string reason)
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
            AddTrace($"← client {reason}: ticket {t.Id} \"{t.Title}\" loaded into the details editor{(this.detailsPanel.Visible ? string.Empty : " (hidden on this profile; Open details shows it as a dialog)")}");
        }

        /// <summary>Phone profiles: the editor opens as a modal dialog — behaviour, not a property value.</summary>
        private void btnOpenDetails_Click(object sender, EventArgs e)
        {
            if (_selectedId == null)
            {
                SetStatus("select a ticket first", StatusKind.Warn);
                return;
            }

            bool maximized = _lastProfile != null && _lastProfile.StartsWith("Phone", StringComparison.Ordinal);
            var dialog = new DetailsDialog(TrySaveFromDialog, maximized);
            dialog.Editor.ShowTicket(_repository.Get(_selectedId));
            AddTrace($"→ render details for {_selectedId} opened as a modal DetailsDialog ({(maximized ? "maximized" : "centered")}) — the ResponsiveProfileChanged behaviour a property cannot express");
            dialog.ShowDialog((form, result) =>
            {
                AddTrace($"← client details dialog closed: {result}");
                form.Dispose();
            });
        }

        private TicketValidationException TrySaveFromDialog(Ticket ticket)
        {
            return TrySave(ticket, "Save (dialog)");
        }

        private void detailsEditor_SaveRequested(object sender, Ticket ticket)
        {
            var error = TrySave(ticket, "Save");
            if (error != null)
                this.detailsEditor.ShowValidationError(error);
        }

        /// <summary>
        /// The only write path. TicketRepository.Save validates on the server and throws
        /// TicketValidationException; the caller shows it on the right editor and in the validation label.
        /// </summary>
        private TicketValidationException TrySave(Ticket ticket, string origin)
        {
            if (ticket == null || ticket.Id == null)
            {
                SetStatus("nothing selected", StatusKind.Warn);
                AddTrace("• server save skipped: no ticket selected");
                return new TicketValidationException("Select a ticket in the grid before saving.");
            }

            try
            {
                var saved = _repository.Save(ticket);
                HideBanner();
                this.detailsEditor.ClearValidation();
                AddTrace($"• server saved {saved.Id} \"{saved.Title}\" ({saved.Priority}, {saved.Status}, {saved.Owner}, due {saved.DueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)})");
                LoadTickets(origin);
                SetStatus($"saved {saved.Id}", StatusKind.Ok);
                AlertBox.Show($"{saved.Id} saved.", MessageBoxIcon.Information,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return null;
            }
            catch (TicketValidationException ex)
            {
                ShowBanner($"Rejected on the server ({origin}): {ex.Message} Nothing was written.");
                AddTrace($"• server rejected {ticket.Id} ({origin}): {ex.Message} — field {ex.Field ?? "(general)"} marked Invalid + InvalidMessage; validation label announces it");
                SetStatus("validation error", StatusKind.Error);
                return ex;
            }
        }

        #endregion

        #region UI helpers: trace, status, banner

        private enum StatusKind { Ok, Warn, Error }

        /// <summary>
        /// Appends one line to the "Layout & theme · live trace" card and selects it.
        /// Lines start with "→ " (server to client), "← " (client to server), "• " (server decision) or "✕ " (injected violation).
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

        /// <summary>Status text plus a theme state (ok / warn / error) on the status-label appearance — no ForeColor in code.</summary>
        private void SetStatus(string text, StatusKind kind)
        {
            this.lblStatus.Text = "● " + text;
            this.lblStatus.RemoveState("ok");
            this.lblStatus.RemoveState("warn");
            this.lblStatus.RemoveState("error");
            this.lblStatus.AddState(kind == StatusKind.Ok ? "ok" : kind == StatusKind.Warn ? "warn" : "error");
        }

        /// <summary>The banner is a docked Panel (banner-danger appearance) hidden by default: showing it pushes the grid down.</summary>
        private void ShowBanner(string text)
        {
            _bannerText = text;
            this.lblBanner.Text = text;
            this.bannerPanel.Visible = _currentView != "Reports";
        }

        private void HideBanner()
        {
            _bannerText = null;
            this.bannerPanel.Visible = false;
            this.lblBanner.Text = string.Empty;
        }

        #endregion
    }
}
