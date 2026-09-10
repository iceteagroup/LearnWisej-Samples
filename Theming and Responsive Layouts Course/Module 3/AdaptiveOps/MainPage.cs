using System;
using System.Globalization;
using System.IO;
using AdaptiveOps.Models;
using Wisej.Core;
using Wisej.Web;

namespace AdaptiveOps
{
    /// <summary>
    /// Adaptive Operations Console — Module 3 (CSS, CssClass, CssStyle, States, and Runtime Theme Changes).
    ///
    /// The shell is the same five docked regions as before (MainPage.Designer.cs); what changes is who
    /// owns each visual decision. The console now ships its own theme, Themes/AdaptiveOps.theme, selected
    /// in Default.json, and a scoped stylesheet, Styles/AdaptiveOps.css, linked from Default.html.
    /// The same metric card is styled by three mechanisms, each for one kind of decision:
    ///
    ///   1. theme appearance   AppearanceKey = "metric-card" / "card" / "action-button" / "status-label":
    ///                         surface, border, radius, text colour, font, every hover/pressed/disabled state,
    ///                         and the custom state "stale" (AddState / RemoveState) — the identity layer.
    ///   2. CssClass           CssClass = "metric-card" (+ "elevated"), "metric-title", "compact-badge",
    ///                         "banner-danger", "progress-track/-fill": reusable, searchable hooks for the
    ///                         parts of the console that are ours, styled in one file.
    ///   3. CssStyle           exactly one assignment, SetSlaProgress(): the SLA fill computed from live
    ///                         data. An inline style cannot be a class; it is also the last resort.
    ///
    /// Decision tree (also written to the trace at start-up): theme first — is it the identity of a
    /// standard control or a reusable semantic variant? then CssClass — is it app-owned UI a designer
    /// should restyle in one place? then CssStyle — is it one computed value? If none fits, it is data
    /// or layout, not styling.
    ///
    /// Toolbar buttons exercise the four lab paths plus the runtime theme changes:
    ///   Apply styles           success   stale state from data, toggle .elevated, set the SLA fill (the one CssStyle)
    ///   Animate SLA            progress  a Wisej.Web.Timer drives the CssStyle 0 → 100 % and settles on the real value
    ///   Style miss             failure   undefined class, invalid CssStyle string, LoadTheme("Nope") — nothing breaks, nothing renders
    ///   Reset                  recovery  states and classes removed, AdaptiveOps theme reloaded, repository reset
    ///   Global: MaterialDark-4           Application.LoadTheme(...) — changes the theme for EVERY session
    ///   Session-only dark                Application.Theme = new ClientTheme("AdaptiveOps-Dark", Application.Theme) — THIS session only
    ///   Back to AdaptiveOps              Application.LoadTheme("AdaptiveOps") and forget the session choice
    ///
    /// The status bar reports the browser size and the active theme (shared vs session). Every decision is
    /// logged in the "Layout & theme · live trace" card; see docs/CssDecisions.md, docs/RuntimeThemeNotes.md
    /// and docs/LabNotes.md.
    /// </summary>
    public partial class MainPage : Page
    {
        /// <summary>The shared theme every session starts with (Default.json / Web.config → Themes/AdaptiveOps.theme).</summary>
        private const string SharedThemeName = "AdaptiveOps";

        /// <summary>Name of the per-session copy created by "Session-only dark". It is never written to disk.</summary>
        private const string SessionDarkThemeName = "AdaptiveOps-Dark";

        /// <summary>An embedded stock theme used to show what a GLOBAL swap does to every session.</summary>
        private const string GlobalDemoThemeName = "MaterialDark-4";

        /// <summary>Key in Application.Session that remembers the per-session theme choice across reloads.</summary>
        private const string ThemeChoiceKey = "ThemeChoice";

        /// <summary>The custom theme state the Overdue card gets when the data says so.</summary>
        private const string StaleState = "stale";

        /// <summary>The stylesheet class that lifts a metric card.</summary>
        private const string ElevatedClass = "elevated";

        /// <summary>
        /// The colour tokens the session-only dark copy overrides. Only tokens are touched — every
        /// appearance keeps referring to them by name, so the dark copy inherits all states for free.
        /// </summary>
        private static readonly (string Token, string Value)[] DarkTokens =
        {
            ("surface", "#1E2430"), ("surfaceAlt", "#141A24"), ("window", "#141A24"),
            ("windowText", "#E6EAF0"), ("textMain", "#E6EAF0"), ("textMuted", "#9AA5B8"), ("controlText", "#C9D1DC"),
            ("buttonFace", "#2A3342"), ("buttonText", "#E6EAF0"), ("buttonHighlight", "#3A4556"), ("windowFrame", "#3A4556"),
            ("warningBg", "#4A3A1A"), ("dangerBg", "#4A1F1B"), ("gray-200", "#2A3342"), ("light", "#1E2430"), ("toolbar", "#1E2430"),
            ("table-row-background", "#1E2430"), ("table-row-background-even", "#1E2430"), ("table-row-background-odd", "#232B38"),
            ("table-row-background-selected", "#2F3B4E"), ("table-row-background-focused", "#2F3B4E"),
            ("table-row-background-focused-selected", "#3B5C9E"), ("table-row", "#C9D1DC"), ("table-row-selected", "#FFFFFF"),
            ("table-row-line", "#3A4556"), ("table-column-line", "#3A4556"), ("text-placeholder", "#6B7688"),
        };

        private readonly TicketRepository _repository = new TicketRepository();

        /// <summary>Id of the ticket shown in the details editor, or null.</summary>
        private string _selectedId;

        /// <summary>True while the grid is being refilled, so SelectionChanged does not re-enter.</summary>
        private bool _suppressSelection;

        /// <summary>The SLA share the data says (0..100); the fill settles here after an animation.</summary>
        private int _slaTarget;

        /// <summary>Progress path: where the animated fill currently is (0..100).</summary>
        private int _slaAnimated;

        public MainPage()
        {
            InitializeComponent();

            // Editor drop-downs come from the enums; the theme owns how they look.
            this.cboPriority.Items.AddRange(Enum.GetNames(typeof(TicketPriority)));
            this.cboStatus.Items.AddRange(Enum.GetNames(typeof(TicketStatus)));

            // Session-level event: it is unsubscribed in Dispose(bool) (MainPage.Designer.cs).
            Application.BrowserSizeChanged += this.Application_BrowserSizeChanged;

            ReportWidth("constructor");
        }

        #region Shell: load, resize, theme label

        private void MainPage_Load(object sender, EventArgs e)
        {
            AddTrace("• server shell built: " + DescribeShell());
            AddTrace("• decision tree: 1 theme appearance (identity of standard controls · semantic variants via AppearanceKey · custom states) → 2 CssClass + Styles/AdaptiveOps.css (app-owned UI, one file) → 3 CssStyle (one computed value, last resort)");
            ReportStartupAssets();
            RestoreSessionTheme();
            RefreshThemeLabel();

            VerifyShell();
            LoadTickets("page load");
            ReportWidth("page load");
            SetStatus("ready", StatusKind.Normal);
        }

        /// <summary>
        /// Says where the theme and the stylesheet come from, and checks that both files exist in the
        /// project folder the static file server serves. A missing stylesheet is the first thing to
        /// check when "the class does not work".
        /// </summary>
        private void ReportStartupAssets()
        {
            AddTrace($"• server theme: {CurrentThemeName()} selected in Default.json (\"theme\") and Web.config (Wisej.DefaultTheme) → Themes/{SharedThemeName}.theme");
            ReportProjectFile("Themes/AdaptiveOps.theme", "theme file");
            ReportProjectFile("Styles/AdaptiveOps.css", "stylesheet (linked from Default.html, served as /Styles/AdaptiveOps.css)");
        }

        private void ReportProjectFile(string relativePath, string what)
        {
            try
            {
                string path = Application.MapPath(relativePath);
                bool exists = File.Exists(path);
                AddTrace(exists
                    ? $"• server {what} present: {relativePath} ({new FileInfo(path).Length:N0} bytes)"
                    : $"• server {what} MISSING at {path} — classes will be on the elements but nothing will render");
            }
            catch (Exception ex)
            {
                AddTrace($"• server could not check {relativePath}: {ex.Message}");
            }
        }

        /// <summary>Fired by the framework when the browser window is resized (session-level event).</summary>
        private void Application_BrowserSizeChanged(object sender, EventArgs e)
        {
            ReportWidth("Application.BrowserSizeChanged");
        }

        /// <summary>The main Page fills the browser viewport, so Resize fires on every window resize too. It only reports.</summary>
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
                AddTrace($"← client resize ({reason}): browser {size.Width}×{size.Height} · workspace {this.workspacePanel.Width}×{this.workspacePanel.Height}");
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
                DescribeRegion("toolbar", this.toolbarPanel),
                DescribeRegion("navigation", this.navigationPanel),
                DescribeRegion("details", this.detailsPanel),
                DescribeRegion("status", this.statusPanel),
                DescribeRegion("workspace", this.workspacePanel));
        }

        private static string DescribeRegion(string name, Panel region)
        {
            return $"{name} Dock={region.Dock} {region.Width}×{region.Height}";
        }

        private void VerifyShell()
        {
            bool ok =
                this.toolbarPanel.Dock == DockStyle.Top &&
                this.statusPanel.Dock == DockStyle.Bottom &&
                this.navigationPanel.Dock == DockStyle.Left &&
                this.detailsPanel.Dock == DockStyle.Right &&
                this.workspacePanel.Dock == DockStyle.Fill;

            AddTrace(ok
                ? "• server dock order verified: Top, Bottom, Left, Right, Fill (child order in MainPage.Designer.cs)"
                : "• server DOCK ORDER WRONG — check the Controls.Add order at the end of InitializeComponent()");
        }

        private static string CurrentThemeName()
        {
            try { return Application.Theme?.Name ?? "(none)"; }
            catch (Exception) { return "(none)"; }
        }

        /// <summary>"theme: AdaptiveOps · shared" or "theme: AdaptiveOps-Dark · this session only".</summary>
        private void RefreshThemeLabel()
        {
            string name = CurrentThemeName();
            bool sessionOnly = string.Equals(name, SessionDarkThemeName, StringComparison.OrdinalIgnoreCase);
            this.lblTheme.Text = $"theme: {name} · {(sessionOnly ? "this session only" : "shared (every session)")}";
        }

        #endregion

        #region Styling: the three mechanisms

        /// <summary>
        /// Re-reads everything from the repository: the metric cards, the SLA row, the grid and the
        /// details editor, and then re-derives the data-driven styling. The server is the source of
        /// truth, so a refresh is always "read again and render", never "patch the browser".
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

            ApplyStaleState(today, reason);
            SetSlaProgress(_repository.SlaOnTimePercent(today), reason);

            if (reselect >= 0)
                ShowTicket((string)this.gridTickets.Rows[reselect].Tag, reason);
            else
                ClearEditor();
        }

        /// <summary>
        /// Mechanism 1 — a custom theme state. The threshold is data (TicketRepository.StaleAfterDays);
        /// the look is the "stale" state of the metric-card appearance in Themes/AdaptiveOps.theme.
        /// The code adds or removes the state and never reads it back to decide anything.
        /// </summary>
        private void ApplyStaleState(DateTime today, string reason)
        {
            int stale = _repository.CountStaleOverdue(today);
            bool shouldBeStale = stale > 0;
            bool isStale = this.cardOverdue.HasState(StaleState);

            if (shouldBeStale && !isStale)
            {
                this.cardOverdue.AddState(StaleState);
                this.lblOverdueValue.AddState(StaleState);
                AddTrace($"→ render theme state ({reason}): cardOverdue.AddState(\"{StaleState}\") — {stale} ticket(s) ≥ {TicketRepository.StaleAfterDays} days past due; look = metric-card/stale in the theme (warningBg surface, warning border), no colour in code");
            }
            else if (!shouldBeStale && isStale)
            {
                this.cardOverdue.RemoveState(StaleState);
                this.lblOverdueValue.RemoveState(StaleState);
                AddTrace($"→ render theme state ({reason}): cardOverdue.RemoveState(\"{StaleState}\") — no ticket ≥ {TicketRepository.StaleAfterDays} days past due any more");
            }
            else
            {
                AddTrace($"• server theme state unchanged ({reason}): cardOverdue stale={isStale} ({stale} ticket(s) past the threshold)");
            }
        }

        /// <summary>
        /// Mechanism 3 — the single CssStyle of the module. The fill Panel always spans the whole
        /// track (Dock = Fill, styled by .progress-fill); how much of it is revealed follows the live
        /// SLA share, and a percentage that changes with the data cannot be a class in a stylesheet.
        ///
        /// Why clip-path and not width: the layout engine writes "width" inline on every widget on
        /// each layout pass, so a CssStyle width would be overwritten the next time the row resizes.
        /// The one CssStyle must set something no other layer owns — and clip-path is exactly that.
        /// The value is written through WriteFillStyle, the only CssStyle assignment in the project.
        /// </summary>
        private void SetSlaProgress(int percent, string reason)
        {
            percent = Math.Max(0, Math.Min(100, percent));
            _slaTarget = this.timerSla.Enabled ? _slaTarget : percent;

            string css = WriteFillStyle(percent);
            this.lblSlaValue.Text = $"{percent} %";

            AddTrace($"→ render CssStyle ({reason}): barFill.CssStyle = \"{css}\" — the one computed inline value ({percent} % of open tickets on time)");
        }

        /// <summary>Formats the inline value for a percentage and writes it. The only CssStyle assignment site.</summary>
        private string WriteFillStyle(int percent)
        {
            string css = $"clip-path:inset(0 {100 - percent}% 0 0)";
            WriteFillStyle(css);
            return css;
        }

        /// <summary>
        /// The single place in the project that assigns Control.CssStyle. Search the project for
        /// "CssStyle =": this is the only hit. The failure path pushes an invalid string through here
        /// on purpose to show what a bad inline style does.
        /// </summary>
        private void WriteFillStyle(string css)
        {
            this.barFill.CssStyle = css;
        }

        /// <summary>The badge in the editor: CssClass "compact-badge" plus a modifier per priority. The stylesheet owns the pill.</summary>
        private void UpdatePriorityBadge(TicketPriority? priority)
        {
            if (priority == null)
            {
                this.lblPriorityBadge.Text = string.Empty;
                this.lblPriorityBadge.CssClass = "compact-badge";
                return;
            }

            this.lblPriorityBadge.Text = priority.ToString();
            this.lblPriorityBadge.CssClass = "compact-badge priority-" + priority.ToString().ToLowerInvariant();
        }

        #endregion

        #region Success path: Apply styles

        /// <summary>
        /// The success path shows the three mechanisms on the same card, one after the other:
        /// the stale state is re-derived from the data (theme), the elevated class is toggled
        /// (stylesheet) and the SLA fill is recomputed (the one CssStyle).
        /// </summary>
        private void btnApplyStyles_Click(object sender, EventArgs e)
        {
            AddTrace("• server Apply styles: one card, three mechanisms — 1 theme state, 2 CssClass, 3 CssStyle");

            var today = DateTime.Today;
            ApplyStaleState(today, "Apply styles");

            if (this.cardOverdue.HasCssClass(ElevatedClass))
            {
                this.cardOverdue.RemoveCssClass(ElevatedClass);
                AddTrace($"→ render CssClass (Apply styles): cardOverdue.RemoveCssClass(\"{ElevatedClass}\") → CssClass = \"{this.cardOverdue.CssClass}\" — .metric-card.elevated in Styles/AdaptiveOps.css no longer matches");
            }
            else
            {
                this.cardOverdue.AddCssClass(ElevatedClass);
                AddTrace($"→ render CssClass (Apply styles): cardOverdue.AddCssClass(\"{ElevatedClass}\") → CssClass = \"{this.cardOverdue.CssClass}\" — shadow and lift come from .metric-card.elevated, one rule for every card that gets the class");
            }

            SetSlaProgress(_repository.SlaOnTimePercent(today), "Apply styles");
            AddTrace("• decision: stale = theme state (a designer restyles it in the theme) · elevated = CssClass (app-owned surface, one rule) · SLA fill = CssStyle (computed from data, nothing else could hold 63 %)");
            SetStatus("styles applied", StatusKind.Normal);
        }

        #endregion

        #region Progress path: Animate SLA (Wisej.Web.Timer)

        private void btnAnimateSla_Click(object sender, EventArgs e)
        {
            if (this.timerSla.Enabled)
                return;

            _slaTarget = _repository.SlaOnTimePercent(DateTime.Today);
            _slaAnimated = 0;
            this.btnAnimateSla.Enabled = false;
            SetStatus("animating SLA", StatusKind.Warn);
            AddTrace($"• server Animate SLA: a Wisej.Web.Timer ({this.timerSla.Interval} ms) moves the fill 0 → 100 % through the same single CssStyle, then settles on the data value {_slaTarget} %");
            SetSlaProgress(0, "animate 0 %");
            this.timerSla.Start();
        }

        private void timerSla_Tick(object sender, EventArgs e)
        {
            _slaAnimated = Math.Min(100, _slaAnimated + 5);

            string css = WriteFillStyle(_slaAnimated);
            this.lblSlaValue.Text = $"{_slaAnimated} %";
            if (_slaAnimated % 25 == 0)
                AddTrace($"→ render progress {_slaAnimated}/100: barFill.CssStyle = \"{css}\"");

            if (_slaAnimated < 100)
                return;

            this.timerSla.Stop();
            SetSlaProgress(_slaTarget, "animation complete");
            this.btnAnimateSla.Enabled = true;
            SetStatus("ready", StatusKind.Normal);
        }

        #endregion

        #region Failure path and recovery

        /// <summary>
        /// Three misses that break nothing and render nothing: a class the stylesheet does not define,
        /// an inline style the browser cannot parse, and a theme name that does not exist. Each is
        /// logged with the check that finds it. Reset recovers all three.
        /// </summary>
        private void btnStyleMiss_Click(object sender, EventArgs e)
        {
            AddTrace("• server Style miss: three misses, nothing throws, nothing renders");

            // 1. a class the stylesheet does not define: it IS on the element (inspect it), no rule matches.
            this.cardMine.AddCssClass("metric-card-glow");
            AddTrace($"→ render CssClass miss: cardMine.CssClass = \"{this.cardMine.CssClass}\" — \"metric-card-glow\" is on the element but Styles/AdaptiveOps.css has no such rule → no change. Checklist: class on the element? yes · rule in the stylesheet? no");

            // 2. an inline style the browser cannot parse: the client parses CssStyle through a scratch
            //    element's style attribute, so invalid declarations are dropped silently.
            WriteFillStyle("clip-path:nonsense(42); colr:red");
            this.lblSlaValue.Text = "?";
            AddTrace("→ render CssStyle miss: barFill.CssStyle = \"clip-path:nonsense(42); colr:red\" — the browser's CSS parser drops both declarations; the previous clip-path is gone, so the fill shows 100 % instead of the SLA. Nothing breaks, nothing right renders");

            // 3. a theme that does not exist: caught, reported, and the session is never left half-themed.
            SwitchGlobalTheme("Nope", "Style miss");

            ShowBanner("✖ Style miss: undefined class, invalid CssStyle and LoadTheme(\"Nope\") were applied — nothing broke, nothing rendered. Reset recovers.");
            SetStatus("style miss", StatusKind.Error);
        }

        /// <summary>
        /// Recovery: lab states and classes off, the failure leftovers removed, the shared theme
        /// reloaded, the repository reset, and everything re-derived from the server state.
        /// </summary>
        private void btnReset_Click(object sender, EventArgs e)
        {
            if (this.timerSla.Enabled)
            {
                this.timerSla.Stop();
                this.btnAnimateSla.Enabled = true;
            }

            this.cardOverdue.RemoveCssClass(ElevatedClass);
            this.cardMine.RemoveCssClass("metric-card-glow");
            this.cardOverdue.RemoveState(StaleState);
            this.lblOverdueValue.RemoveState(StaleState);
            AddTrace($"• server Reset: classes back to \"{this.cardOverdue.CssClass}\" / \"{this.cardMine.CssClass}\", stale state removed (it is re-derived from the data below), invalid CssStyle replaced by the computed value");

            ForgetThemeChoice();
            SwitchGlobalTheme(SharedThemeName, "Reset");

            _repository.Reset();
            HideBanner();
            LoadTickets("Reset");
            SetStatus("ready", StatusKind.Normal);
        }

        #endregion

        #region Runtime theme changes: global vs session

        /// <summary>GLOBAL: Application.LoadTheme swaps the theme for every session on this server.</summary>
        private void btnThemeGlobal_Click(object sender, EventArgs e)
        {
            ForgetThemeChoice();
            if (SwitchGlobalTheme(GlobalDemoThemeName, "Global: MaterialDark-4"))
                SetStatus("global theme changed", StatusKind.Warn);
            else
                SetStatus("theme change failed", StatusKind.Error);
        }

        /// <summary>SESSION: a copy of the current theme with dark colour tokens, assigned to this session only.</summary>
        private void btnThemeSession_Click(object sender, EventArgs e)
        {
            if (ApplySessionDarkTheme("Session-only dark"))
                SetStatus("dark theme · this session only", StatusKind.Normal);
            else
                SetStatus("session theme failed", StatusKind.Error);
        }

        private void btnThemeBack_Click(object sender, EventArgs e)
        {
            ForgetThemeChoice();
            if (SwitchGlobalTheme(SharedThemeName, "Back to AdaptiveOps"))
                SetStatus("ready", StatusKind.Normal);
            else
                SetStatus("theme change failed", StatusKind.Error);
        }

        /// <summary>
        /// Application.LoadTheme(name): the shared theme object changes, so EVERY session renders the
        /// new theme on its next round-trip. Wrapped so that an unknown name is caught and the session
        /// is put back on the theme it had — never left half-themed.
        /// </summary>
        private bool SwitchGlobalTheme(string name, string origin)
        {
            ClientTheme before = Application.Theme;
            string beforeName = CurrentThemeName();

            try
            {
                Application.LoadTheme(name);

                string after = CurrentThemeName();
                if (!string.Equals(after, name, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException($"the active theme is '{after}', not '{name}'");

                AddTrace($"→ render Application.LoadTheme(\"{name}\") ({origin}): GLOBAL — the shared theme changed from {beforeName} to {after}; every session on this server follows on its next request. Open a second tab to see it");
                if (!string.Equals(name, SharedThemeName, StringComparison.OrdinalIgnoreCase))
                    AddTrace($"• server note: {name} has no action-button / card / metric-card / muted-label appearances — the controls that use those AppearanceKeys fall back to plain widgets until \"Back to AdaptiveOps\". That is the theme owning them, not a bug");
                return true;
            }
            catch (Exception ex)
            {
                AddTrace($"• server LoadTheme(\"{name}\") FAILED ({origin}): {ex.GetType().Name}: {ex.Message}");
                try
                {
                    if (before != null && !ReferenceEquals(Application.Theme, before))
                        Application.Theme = before;
                    AddTrace($"• server theme restored for this session: {CurrentThemeName()} (never left half-themed)");
                }
                catch (Exception restoreEx)
                {
                    AddTrace($"• server theme restore failed: {restoreEx.Message}");
                }
                return false;
            }
            finally
            {
                RefreshThemeLabel();
            }
        }

        /// <summary>
        /// Application.Theme = copy: a new ClientTheme built FROM the current theme, with the dark
        /// colour tokens changed on the copy. Only this session renders it; the shared theme object is
        /// never touched (the trace proves it by reading the shared "surface" token before and after).
        /// The choice is remembered in Application.Session so a reload restores it.
        /// </summary>
        private bool ApplySessionDarkTheme(string origin)
        {
            ClientTheme shared = Application.Theme;
            if (shared == null)
            {
                AddTrace($"• server {origin}: Application.Theme is null, nothing to copy");
                return false;
            }

            if (string.Equals(shared.Name, SessionDarkThemeName, StringComparison.OrdinalIgnoreCase))
            {
                AddTrace($"• server {origin}: this session already renders {SessionDarkThemeName}");
                return true;
            }

            try
            {
                string sharedSurfaceBefore = ReadToken(shared, "surface");

                // The copy, never the shared object: mutating Application.Theme.Colors.x on the theme that
                // LoadTheme installed would change it for every session.
                var dark = new ClientTheme(SessionDarkThemeName, shared);
                var colors = dark.Colors as DynamicObject;
                if (colors == null)
                    throw new InvalidOperationException("the copied theme exposes no Colors collection");

                int changed = 0;
                foreach (var (token, value) in DarkTokens)
                {
                    colors[token] = value;
                    changed++;
                }

                Application.Theme = dark;
                RememberThemeChoice("dark");

                string sharedSurfaceAfter = ReadToken(shared, "surface");
                AddTrace($"→ render Application.Theme = new ClientTheme(\"{SessionDarkThemeName}\", Application.Theme) ({origin}): {changed} colour tokens changed ON THE COPY (surface {ReadToken(dark, "surface")}, surfaceAlt {ReadToken(dark, "surfaceAlt")}, …) — THIS SESSION ONLY");
                AddTrace($"• server proof: shared theme '{shared.Name}' Colors.surface was {sharedSurfaceBefore} and is still {sharedSurfaceAfter}; a second tab keeps the light theme. Choice stored in Application.Session[\"{ThemeChoiceKey}\"] = dark");

                if (!string.Equals(shared.Name, SharedThemeName, StringComparison.OrdinalIgnoreCase))
                    AddTrace($"• server note: the copy is based on '{shared.Name}', the theme this session had — click \"Back to AdaptiveOps\" first if you want the dark copy of AdaptiveOps");
                return true;
            }
            catch (Exception ex)
            {
                AddTrace($"• server session theme FAILED ({origin}): {ex.GetType().Name}: {ex.Message}");
                try
                {
                    if (!ReferenceEquals(Application.Theme, shared))
                        Application.Theme = shared;
                    AddTrace($"• server theme restored for this session: {CurrentThemeName()}");
                }
                catch (Exception restoreEx)
                {
                    AddTrace($"• server theme restore failed: {restoreEx.Message}");
                }
                return false;
            }
            finally
            {
                RefreshThemeLabel();
            }
        }

        /// <summary>On page load: if this session chose dark before a reload, apply the copy again.</summary>
        private void RestoreSessionTheme()
        {
            string choice = ReadThemeChoice();
            if (choice == null)
            {
                AddTrace("• server session theme choice: none stored (Application.Session) — this session follows the shared theme");
                return;
            }

            AddTrace($"• server session theme choice restored from Application.Session: {choice}");
            if (choice == "dark")
                ApplySessionDarkTheme("session restore");
        }

        private static string ReadToken(ClientTheme theme, string token)
        {
            try
            {
                var colors = theme?.Colors as DynamicObject;
                if (colors == null || !colors.Contains(token))
                    return "(unset)";
                return Convert.ToString(colors[token], CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return "(unreadable)";
            }
        }

        private static void RememberThemeChoice(string choice)
        {
            var session = Application.Session as DynamicObject;
            if (session != null)
                session[ThemeChoiceKey] = choice;
        }

        private void ForgetThemeChoice()
        {
            var session = Application.Session as DynamicObject;
            if (session != null && session.Contains(ThemeChoiceKey))
            {
                session.Delete(ThemeChoiceKey);
                AddTrace($"• server Application.Session[\"{ThemeChoiceKey}\"] cleared — this session follows the shared theme again");
            }
        }

        private static string ReadThemeChoice()
        {
            try
            {
                var session = Application.Session as DynamicObject;
                if (session == null || !session.Contains(ThemeChoiceKey))
                    return null;
                return session[ThemeChoiceKey] as string;
            }
            catch (Exception)
            {
                return null;
            }
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
            UpdatePriorityBadge(t.Priority);

            AddTrace($"← client {reason}: ticket {t.Id} \"{t.Title}\" in the editor · badge CssClass = \"{this.lblPriorityBadge.CssClass}\"");
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
            UpdatePriorityBadge(null);
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
        /// TicketValidationException. A successful save re-derives the stale state and the SLA fill,
        /// so moving an old due date forward is how the learner watches the state come and go.
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

        private void btnNav_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null)
                return;

            this.lblWorkspaceTitle.Text = button.Text == "Dashboard" ? "Tickets" : button.Text;
            AddTrace($"← client navigation: {button.Text}");
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

        /// <summary>
        /// The status colour is a semantic condition, so it is a custom theme state on the
        /// "status-label" appearance ("warn", "error"; the default state is the success colour).
        /// Module 1 set three ForeColor values here; Module 3 sets none.
        /// </summary>
        private void SetStatus(string text, StatusKind kind)
        {
            this.lblStatus.Text = "● " + text;
            this.lblStatus.RemoveState("warn");
            this.lblStatus.RemoveState("error");

            if (kind == StatusKind.Warn)
                this.lblStatus.AddState("warn");
            else if (kind == StatusKind.Error)
                this.lblStatus.AddState("error");
        }

        /// <summary>
        /// The banner is a docked Panel hidden by default: showing it pushes the grid down. Its red
        /// bold text is the "banner-label" appearance; its tinted surface is the .banner-danger class.
        /// </summary>
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
