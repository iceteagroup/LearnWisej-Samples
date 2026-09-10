using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using AdaptiveOps.Models;
using Wisej.Web;

namespace AdaptiveOps
{
    /// <summary>
    /// Adaptive Operations Console — Module 2 (Theme Builder and Theme JSON Internals).
    ///
    /// The shell is the Module 1 console (five docked regions declared in MainPage.Designer.cs) but the
    /// look now comes from ONE file: Themes/AdaptiveOps.theme, created from the Bootstrap-4 base theme and
    /// selected at startup by "theme": "AdaptiveOps" in Default.json (and Wisej.DefaultTheme in Web.config).
    /// Nothing in this project sets BackColor, ForeColor or Font. A control only names the appearance it
    /// wants (AppearanceKey: action-button, metric-card, metric-strip, heading-label, status-label, trace-list …)
    /// and, where the theme defines custom states (metric-strip danger/warning/success, status-label
    /// ok/warn/error), which state is active (Control.States).
    ///
    /// Toolbar buttons exercise the lab paths against the theme engine:
    ///   Apply theme     success   read Application.Theme.Name, give the primary commands AppearanceKey =
    ///                             "action-button", fill the token inspector with every resolved token
    ///   Walk states     progress  a Wisej.Web.Timer resolves one appearance/state per tick and logs the value
    ///                             (button vs action-button shows what is overridden and what is inherited)
    ///   Missing theme   failure   Application.LoadTheme("Missing-Theme") in try/catch + a misspelt token name;
    ///                             the app stays alive and the trace says what each call returned
    ///   Invalid ticket  failure   server-side validation rejects the ticket; the Title editor enters the
    ///                             theme's "invalid" state, painted with the danger token (no code colour)
    ///   Reset           recovery  Application.LoadTheme("AdaptiveOps"), action-button re-applied, editor
    ///                             valid again, seed tickets restored
    ///   Base theme ⇄    compare   Application.LoadTheme("Bootstrap-4") and back, so the base look and the
    ///                             custom look can be told apart (the swap is global to every session)
    ///
    /// The status bar reports the browser size (Module 1) and the active theme name (this module's
    /// acceptance criterion). Every decision is logged in the "Layout & theme · live trace" card.
    /// See docs/ThemeNotes.md (file structure, tokens, inheritance, state order) and docs/ThemeBuilderSteps.md.
    /// </summary>
    public partial class MainPage : Page
    {
        /// <summary>The custom theme: Themes/AdaptiveOps.theme, named in Default.json.</summary>
        private const string ThemeName = "AdaptiveOps";

        /// <summary>The embedded base theme the custom file was created from (Module 1 recorded it).</summary>
        private const string BaseThemeName = "Bootstrap-4";

        private readonly TicketRepository _repository = new TicketRepository();

        /// <summary>Id of the ticket shown in the details editor, or null.</summary>
        private string _selectedId;

        /// <summary>True while the grid is being refilled, so SelectionChanged does not re-enter.</summary>
        private bool _suppressSelection;

        /// <summary>Progress path: index of the next theme query the state walk resolves.</summary>
        private int _walkStep;

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

        #region Shell: load, resize, theme report

        private void MainPage_Load(object sender, EventArgs e)
        {
            AddTrace("• server shell built: " + DescribeShell());
            VerifyShell();

            // The lab's theme check, wrapped so a missing or malformed theme file produces a readable
            // status message instead of a blank page.
            try
            {
                string name = CurrentThemeName();
                ReportTheme("page load");

                if (name == ThemeName)
                {
                    AddTrace($"• server theme \"{name}\" is active — loaded from Themes/{ThemeName}.theme through Default.json \"theme\" (Web.config Wisej.DefaultTheme names it too)");
                    SetStatus("theme " + name, StatusKind.Ok);
                }
                else
                {
                    ShowBanner($"⚠ Default.json asks for \"{ThemeName}\" but the running theme is \"{name}\". The framework could not find Themes/{ThemeName}.theme (or the JSON is malformed) and fell back. Check the /Themes folder next to the binaries.");
                    AddTrace($"• server THEME FALLBACK: expected \"{ThemeName}\", running \"{name}\"");
                    SetStatus("theme fallback", StatusKind.Warn);
                }

                TraceThemeFile();
            }
            catch (Exception ex)
            {
                ShowBanner("✖ Theme check failed: " + ex.Message);
                AddTrace($"• server theme check threw {ex.GetType().Name}: {ex.Message}");
                SetStatus("theme error", StatusKind.Error);
            }

            LoadTickets("page load");
            ReportWidth("page load");
        }

        /// <summary>Fired by the framework when the browser window is resized (session-level event).</summary>
        private void Application_BrowserSizeChanged(object sender, EventArgs e)
        {
            ReportWidth("Application.BrowserSizeChanged");
        }

        /// <summary>The main Page fills the viewport, so Resize fires on every window resize. Docking answers it; this only reports.</summary>
        private void MainPage_Resize(object sender, EventArgs e)
        {
            ReportWidth("Page.Resize");
        }

        /// <summary>Writes "Browser 1348 × 680 px · Desktop" into the status bar, with an honest fallback text.</summary>
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

        /// <summary>Writes the active theme name into the status bar (the lab's acceptance check) and the trace.</summary>
        private void ReportTheme(string reason)
        {
            string name = CurrentThemeName();
            this.lblTheme.Text = name == ThemeName
                ? $"theme: {name} (Themes/{ThemeName}.theme · Default.json)"
                : $"theme: {name} (expected {ThemeName})";
            AddTrace($"• server Application.Theme.Name = \"{name}\" ({reason})");
        }

        /// <summary>
        /// Wisej.NET loads a named theme "from a file in the application /Themes directory or from the embedded
        /// resources"; the csproj ships Themes/*.theme as Content copied to the output, so the file is expected
        /// in both places. Logged, not asserted: the running theme name is the real test.
        /// </summary>
        private void TraceThemeFile()
        {
            string file = ThemeName + ".theme";
            string inProject = Path.Combine(Directory.GetCurrentDirectory(), "Themes", file);
            string inOutput = Path.Combine(AppContext.BaseDirectory, "Themes", file);
            AddTrace($"• server theme file: project folder {(File.Exists(inProject) ? "✓" : "✗")} {inProject}");
            AddTrace($"• server theme file: output folder  {(File.Exists(inOutput) ? "✓" : "✗")} {inOutput}");
        }

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

        #endregion

        #region Success path: Apply theme (AppearanceKey = "action-button" + token inspector)

        /// <summary>
        /// The lab's btnApplyTheme_Click: read Application.Theme.Name, write it to the status label, give the
        /// primary command its semantic variant through AppearanceKey — never through BackColor — and show
        /// every token's resolved value.
        /// </summary>
        private void btnApplyTheme_Click(object sender, EventArgs e)
        {
            try
            {
                ReportTheme("Apply theme");
                ApplyActionButton();
                FillTokenInspector();
                this.tabWorkspace.SelectedIndex = 1;

                var theme = Application.Theme;
                AddTrace($"→ theme brandPrimary = {Hex(theme.GetColor("brandPrimary"))} · brandAccent = {Hex(theme.GetColor("brandAccent"))} · danger = {Hex(theme.GetColor("danger"))} · focusFrame = {Hex(theme.GetColor("focusFrame"))}");
                AddTrace($"→ theme action-button/backgroundColor: default {Hex(theme.GetColor("action-button", "backgroundColor"))} · hovered {Hex(theme.GetColor("action-button", "backgroundColor", "hovered"))} · pressed {Hex(theme.GetColor("action-button", "backgroundColor", "pressed"))}");

                HideBanner();
                SetStatus($"theme {CurrentThemeName()} applied", CurrentThemeName() == ThemeName ? StatusKind.Ok : StatusKind.Warn);
            }
            catch (Exception ex)
            {
                ShowBanner("✖ Apply theme failed: " + ex.Message);
                AddTrace($"• server Apply theme threw {ex.GetType().Name}: {ex.Message}");
                SetStatus("theme error", StatusKind.Error);
            }
        }

        /// <summary>AppearanceKey selects the theme block; the widget stays a Button (focused and disabled come from "button").</summary>
        private void ApplyActionButton()
        {
            this.btnSave.AppearanceKey = "action-button";
            this.btnApplyTheme.AppearanceKey = "action-button";
            AddTrace("→ render btnSave.AppearanceKey = \"action-button\" and btnApplyTheme.AppearanceKey = \"action-button\" (inherit: button; own default/hovered/pressed; focused + disabled from button)");
        }

        /// <summary>One token per row: how the theme file defines it, how the server resolves it, which appearances use it.</summary>
        private void FillTokenInspector()
        {
            var theme = Application.Theme;
            this.gridTokens.Rows.Clear();

            foreach (var t in TokenCatalog)
            {
                string defined;
                string resolved;

                if (t.Kind == "color")
                {
                    defined = RawThemeValue(theme.Colors, t.Name);
                    var c = theme.GetColor(t.Name);
                    resolved = c.IsEmpty ? "Color.Empty (unknown token)" : Hex(c);
                }
                else
                {
                    defined = RawThemeValue(theme.Fonts, t.Name);
                    var f = theme.GetFont(t.Name);
                    resolved = f == null ? "null (unknown font)" : $"{f.Name} {f.Size:0.#}px{(f.Bold ? " bold" : "")}";
                }

                this.gridTokens.Rows.Add(new object[] { t.Name, t.Kind, defined, resolved, t.UsedBy });
            }

            AddTrace($"→ render token inspector: {TokenCatalog.Length} tokens resolved through Application.Theme.GetColor / GetFont on theme \"{theme.Name}\"");
        }

        /// <summary>
        /// The theme sections are dynamic objects with a string indexer; the raw value is what the JSON says
        /// ("#2454A6", or the font object). Null when the token does not exist in the running theme.
        /// </summary>
        private static string RawThemeValue(object section, string name)
        {
            try
            {
                dynamic d = section;
                object raw = d[name];
                return raw == null ? "(not defined in this theme)" : raw.ToString();
            }
            catch (Exception)
            {
                return "(not readable)";
            }
        }

        private sealed class TokenInfo
        {
            public TokenInfo(string name, string kind, string usedBy) { Name = name; Kind = kind; UsedBy = usedBy; }
            public string Name { get; }
            public string Kind { get; }
            public string UsedBy { get; }
        }

        private static readonly TokenInfo[] TokenCatalog =
        {
            new TokenInfo("brandPrimary", "color", "action-button default · panel/captionbar · metric-strip default · tabview/page/button checked text · button checked"),
            new TokenInfo("brandAccent", "color", "table-header-cell sorted text (sort a column to see it); Module 3 reuses it in the CssClass layer"),
            new TokenInfo("surface", "color", "metric-card · tabview/page · textbox · table rows · tooltip/atom text · trace-list"),
            new TokenInfo("surfaceAlt", "color", "page background · button face · tab bar buttons · table header · odd table rows"),
            new TokenInfo("textMain", "color", "root/page text · button text · heading-label · card-title · tooltip/atom background"),
            new TokenInfo("textMuted", "color", "muted-label · overline-label · mono-label · tab button text · table-header-cell text · status-label default"),
            new TokenInfo("danger", "color", "textbox invalid border · banner-label · metric-strip [danger] · status-label [error] · tooltip-error"),
            new TokenInfo("warning", "color", "metric-strip [warning] · status-label [warn]"),
            new TokenInfo("success", "color", "metric-strip [success] · status-label [ok]"),
            new TokenInfo("focusFrame", "color", "textbox / combobox / list focused border · tab button focused inner border (same value as brandPrimary — see ThemeNotes.md)"),
            new TokenInfo("brandPrimaryHover", "color", "action-button hovered (derived shade, defined once)"),
            new TokenInfo("brandPrimaryPressed", "color", "action-button pressed (derived shade, defined once)"),
            new TokenInfo("surfaceHover", "color", "button hovered · table-header-cell hovered · list item hovered · selected table rows"),
            new TokenInfo("focusShadow", "color", "button focused ring (rgba of brandPrimary)"),
            new TokenInfo("default", "font", "root (every widget without its own font)"),
            new TokenInfo("heading", "font", "heading-label: application title, metric values"),
            new TokenInfo("mono", "font", "trace-list, mono-label (browser size)"),
            new TokenInfo("defaultBold", "font", "panel/captionbar · card-title · overline-label · status-label · banner-label (kept from the base theme)"),
        };

        #endregion

        #region Progress path: Walk states (Wisej.Web.Timer)

        private sealed class ThemeQuery
        {
            public ThemeQuery(string label, Func<string> resolve, string note = null) { Label = label; Resolve = resolve; Note = note; }
            public string Label { get; }
            public Func<string> Resolve { get; }
            public string Note { get; }
        }

        /// <summary>
        /// The states are not something code switches on (hovered / pressed / focused are set by the browser);
        /// what code CAN do is ask the theme what each state resolves to. Walking button and action-button side
        /// by side shows exactly which states action-button overrides and which it inherits.
        /// </summary>
        private ThemeQuery[] BuildWalk()
        {
            var t = Application.Theme;
            return new[]
            {
                new ThemeQuery("button / backgroundColor [default]", () => Hex(t.GetColor("button", "backgroundColor")), "surfaceAlt"),
                new ThemeQuery("button / backgroundColor [hovered]", () => Hex(t.GetColor("button", "backgroundColor", "hovered")), "surfaceHover"),
                new ThemeQuery("button / shadowColor [focused]", () => Hex(t.GetColor("button", "shadowColor", "focused")), "focusShadow ring"),
                new ThemeQuery("button / opacity [disabled]", () => t.GetProperty<double>("button", "opacity", "disabled").ToString(CultureInfo.InvariantCulture)),
                new ThemeQuery("action-button / backgroundColor [default]", () => Hex(t.GetColor("action-button", "backgroundColor")), "OWN — brandPrimary"),
                new ThemeQuery("action-button / textColor [default]", () => Hex(t.GetColor("action-button", "textColor")), "OWN — white"),
                new ThemeQuery("action-button / backgroundColor [hovered]", () => Hex(t.GetColor("action-button", "backgroundColor", "hovered")), "OWN — brandPrimaryHover"),
                new ThemeQuery("action-button / backgroundColor [pressed]", () => Hex(t.GetColor("action-button", "backgroundColor", "pressed")) + " transform " + (t.GetStyle<string>("action-button", "transform", "pressed") ?? "(none)"), "OWN — brandPrimaryPressed; below hovered so it wins while the mouse is held"),
                new ThemeQuery("action-button / shadowColor [focused]", () => Hex(t.GetColor("action-button", "shadowColor", "focused")), "INHERITED from button — not overridden"),
                new ThemeQuery("action-button / opacity [disabled]", () => t.GetProperty<double>("action-button", "opacity", "disabled").ToString(CultureInfo.InvariantCulture), "INHERITED from button"),
                new ThemeQuery("action-button / height [default]", () => t.GetProperty<int>("action-button", "height").ToString(CultureInfo.InvariantCulture), "INHERITED from button"),
                new ThemeQuery("textbox / color [invalid]", () => Hex(t.GetColor("textbox", "color", "invalid")), "danger — why the invalid editor is styled in the theme, not in code"),
                new ThemeQuery("textbox / color [focused]", () => Hex(t.GetColor("textbox", "color", "focused")), "focusFrame"),
                new ThemeQuery("table-header-cell / backgroundColor [default]", () => Hex(t.GetColor("table-header-cell", "backgroundColor")), "surfaceAlt (grid header component)"),
                new ThemeQuery("table-header-cell / textColor [default]", () => Hex(t.GetColor("table-header-cell", "textColor")), "textMuted"),
                new ThemeQuery("tooltip/atom / backgroundColor [default]", () => Hex(t.GetColor("tooltip/atom", "backgroundColor")), "textMain (child component path with a slash)"),
                new ThemeQuery("panel/captionbar / backgroundColor [default]", () => Hex(t.GetColor("panel/captionbar", "backgroundColor")), "brandPrimary (the two headered cards)"),
                new ThemeQuery("tabview/page/button / textColor [checked]", () => Hex(t.GetColor("tabview/page/button", "textColor", "checked")), "brandPrimary"),
                new ThemeQuery("metric-strip / backgroundColor [danger]", () => Hex(t.GetColor("metric-strip", "backgroundColor", "danger")), "custom state set with Control.States"),
                new ThemeQuery("status-label / textColor [error]", () => Hex(t.GetColor("status-label", "textColor", "error")), "custom state set with Control.States"),
                new ThemeQuery("font heading", () => FontText(t.GetFont("heading"))),
                new ThemeQuery("font mono", () => FontText(t.GetFont("mono"))),
            };
        }

        private ThemeQuery[] _walk;

        private void btnWalkStates_Click(object sender, EventArgs e)
        {
            if (this.timerWalk.Enabled)
                return;

            _walk = BuildWalk();
            _walkStep = 0;
            this.lblProgress.Text = $"State walk · 0 of {_walk.Length}";
            this.btnWalkStates.Enabled = false;
            SetStatus("walking theme states", StatusKind.Warn);
            AddTrace($"• server state walk started on theme \"{CurrentThemeName()}\": {_walk.Length} queries every {this.timerWalk.Interval} ms (Wisej.Web.Timer, server side)");
            this.timerWalk.Start();
        }

        private void timerWalk_Tick(object sender, EventArgs e)
        {
            if (_walk == null || _walkStep >= _walk.Length)
            {
                this.timerWalk.Stop();
                return;
            }

            var q = _walk[_walkStep];
            _walkStep++;

            string value;
            try { value = q.Resolve(); }
            catch (Exception ex) { value = $"threw {ex.GetType().Name}: {ex.Message}"; }

            this.lblProgress.Text = $"State walk · {_walkStep} of {_walk.Length} · {q.Label}";
            AddTrace($"→ theme {q.Label} = {value}{(q.Note == null ? "" : "   ← " + q.Note)}");

            if (_walkStep < _walk.Length)
                return;

            this.timerWalk.Stop();
            this.lblProgress.Text = $"State walk complete · {_walk.Length}/{_walk.Length}";
            this.btnWalkStates.Enabled = true;
            SetStatus("ready", StatusKind.Ok);
        }

        #endregion

        #region Failure paths: Missing theme, Invalid ticket

        /// <summary>
        /// What happens when the theme name does not match any file in /Themes, and what a misspelt token
        /// returns. Both are wrapped so the console keeps running; Reset is the recovery.
        /// </summary>
        private void btnMissingTheme_Click(object sender, EventArgs e)
        {
            string before = CurrentThemeName();
            string outcome;

            try
            {
                Application.LoadTheme("Missing-Theme");
                string after = CurrentThemeName();
                outcome = after == before
                    ? $"Application.LoadTheme(\"Missing-Theme\") returned without throwing and the theme is still \"{after}\""
                    : $"Application.LoadTheme(\"Missing-Theme\") returned without throwing; Application.Theme.Name is now \"{after}\"";
                AddTrace("• server " + outcome);
            }
            catch (Exception ex)
            {
                outcome = $"Application.LoadTheme(\"Missing-Theme\") threw {ex.GetType().Name}: {ex.Message}";
                AddTrace("• server " + outcome + $" — theme still \"{CurrentThemeName()}\"");
            }

            // A wrong token name is silent: GetColor returns Color.Empty, the client paints nothing.
            var theme = Application.Theme;
            var typoToken = theme.GetColor("brandPrimry");
            var typoAppearance = theme.GetColor("action-buton", "backgroundColor");
            AddTrace($"→ theme GetColor(\"brandPrimry\") = {(typoToken.IsEmpty ? "Color.Empty (IsEmpty = true)" : Hex(typoToken))}   ← misspelt token: no exception, no colour");
            AddTrace($"→ theme GetColor(\"action-buton\", \"backgroundColor\") = {(typoAppearance.IsEmpty ? "Color.Empty" : Hex(typoAppearance))}   ← misspelt AppearanceKey: the widget would render bare");

            ReportTheme("Missing theme");
            ShowBanner($"✖ {outcome}. A misspelt token (\"brandPrimry\") resolves to Color.Empty. The console is still alive; Reset reloads {ThemeName}.");
            SetStatus("theme error", StatusKind.Error);
            AlertBox.Show("Missing-Theme could not be applied — see the trace.", MessageBoxIcon.Warning,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        /// <summary>
        /// Blanks the title and saves. The repository refuses it on the server (TicketValidationException);
        /// SaveTicket then puts the editor into the theme's "invalid" state — the danger colour is the theme's.
        /// </summary>
        private void btnInvalidTicket_Click(object sender, EventArgs e)
        {
            if (_selectedId == null)
                ShowTicket(_repository.GetAll()[0].Id, "Invalid ticket");

            this.txtTitle.Text = string.Empty;
            SaveTicket(ReadEditor(), "Invalid ticket");
        }

        #endregion

        #region Recovery and comparison

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (this.timerWalk.Enabled)
            {
                this.timerWalk.Stop();
                this.btnWalkStates.Enabled = true;
            }

            if (CurrentThemeName() != ThemeName)
            {
                try
                {
                    Application.LoadTheme(ThemeName);
                    AddTrace($"• server Application.LoadTheme(\"{ThemeName}\") — custom theme reloaded (for every session of this app)");
                }
                catch (Exception ex)
                {
                    AddTrace($"• server Application.LoadTheme(\"{ThemeName}\") threw {ex.GetType().Name}: {ex.Message}");
                }
            }

            ApplyActionButton();
            ClearInvalid();
            _repository.Reset();
            HideBanner();
            this.lblProgress.Text = string.Empty;
            this.btnCompareTheme.Text = "Base theme ⇄";
            AddTrace("• server repository reset to the seed tickets; banner cleared; editor valid");
            LoadTickets("Reset");
            FillTokenInspector();
            ReportTheme("Reset");
            SetStatus("ready", StatusKind.Ok);
        }

        /// <summary>
        /// Bootstrap-4 ⇄ AdaptiveOps. Application.LoadTheme swaps the theme for every session, live. On the
        /// base theme the custom appearances (action-button, metric-card, status-label …) do not exist, so the
        /// controls that select them render as bare widgets: that is what "the wrong AppearanceKey" looks like.
        /// </summary>
        private void btnCompareTheme_Click(object sender, EventArgs e)
        {
            bool onCustom = CurrentThemeName() == ThemeName;
            string target = onCustom ? BaseThemeName : ThemeName;

            try
            {
                Application.LoadTheme(target);
                var theme = Application.Theme;
                AddTrace($"• server Application.LoadTheme(\"{target}\") — global swap, every session now renders \"{theme.Name}\"");

                var action = theme.GetColor("action-button", "backgroundColor");
                var card = theme.GetColor("metric-card", "backgroundColor");
                AddTrace(action.IsEmpty
                    ? "→ theme action-button and metric-card are NOT defined in this theme (GetColor = Color.Empty): Save and the cards fall back to bare widgets — the 'wrong AppearanceKey' pitfall, seen live"
                    : $"→ theme action-button/backgroundColor = {Hex(action)} · metric-card/backgroundColor = {Hex(card)} — the custom appearances are back");
                AddTrace($"→ theme button/backgroundColor [hovered] = {Hex(theme.GetColor("button", "backgroundColor", "hovered"))} · textbox/color [invalid] = {Hex(theme.GetColor("textbox", "color", "invalid"))} · panel/captionbar = {Hex(theme.GetColor("panel/captionbar", "backgroundColor"))}");

                this.btnCompareTheme.Text = onCustom ? $"{ThemeName} ⇄" : "Base theme ⇄";
                ReportTheme("Base theme ⇄");
                FillTokenInspector();
                SetStatus($"theme {theme.Name}", onCustom ? StatusKind.Warn : StatusKind.Ok);
            }
            catch (Exception ex)
            {
                ShowBanner($"✖ Application.LoadTheme(\"{target}\") failed: {ex.Message}");
                AddTrace($"• server LoadTheme(\"{target}\") threw {ex.GetType().Name}: {ex.Message}");
                SetStatus("theme error", StatusKind.Error);
            }
        }

        #endregion

        #region Tickets: load, select, edit, save

        /// <summary>Re-reads everything from the repository: the four metric cards, the grid and the details editor.</summary>
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
            ClearInvalid();

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
            ClearInvalid();
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
        /// TicketValidationException; the catch block flags the editor through the theme's invalid state.
        /// </summary>
        private bool SaveTicket(Ticket ticket, string origin)
        {
            try
            {
                var saved = _repository.Save(ticket);

                ClearInvalid();
                HideBanner();
                AddTrace($"• server saved {saved.Id} \"{saved.Title}\" ({saved.Priority}, {saved.Status}, {saved.Owner}, due {saved.DueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)})");
                LoadTickets(origin);
                SetStatus($"saved {saved.Id}", StatusKind.Ok);
                AlertBox.Show($"{saved.Id} saved.", MessageBoxIcon.Information,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return true;
            }
            catch (TicketValidationException ex)
            {
                // Failure path: the server refused the ticket. The offending editor gets Invalid = true, which
                // adds the "invalid" state on the client; the theme's textbox appearance paints that state with
                // the danger token and shows InvalidMessage in the tooltip-error appearance. No colour in code.
                MarkInvalid(ex.Message);
                ShowBanner($"✖ Rejected on the server ({origin}): {ex.Message} Nothing was written. The red border is the theme's textbox [invalid] state (danger).");
                AddTrace($"• server rejected {ticket.Id ?? "(none)"} ({origin}): {ex.Message}");
                AddTrace("→ render txtTitle.Invalid = true → client state \"invalid\" → textbox/color [invalid] = danger (theme), InvalidMessage in tooltip-error");
                SetStatus("validation error", StatusKind.Error);
                AlertBox.Show(ex.Message, MessageBoxIcon.Warning,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return false;
            }
        }

        private void MarkInvalid(string message)
        {
            bool owner = message.StartsWith("Owner", StringComparison.Ordinal);
            var editor = owner ? this.txtOwner : this.txtTitle;
            editor.InvalidMessage = message;
            editor.Invalid = true;
        }

        private void ClearInvalid()
        {
            this.txtTitle.Invalid = false;
            this.txtTitle.InvalidMessage = string.Empty;
            this.txtOwner.Invalid = false;
            this.txtOwner.InvalidMessage = string.Empty;
        }

        #endregion

        #region Navigation rail

        /// <summary>The rail is five plain Buttons ("button" appearance); Settings opens the token inspector, the rest the tickets.</summary>
        private void btnNav_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null)
                return;

            this.tabWorkspace.SelectedIndex = button.Text == "Settings" ? 1 : 0;
            AddTrace($"← client navigation: {button.Text} (rail buttons keep the base \"button\" appearance: surfaceAlt / surfaceHover / focusShadow)");
        }

        #endregion

        #region UI helpers: trace, status, banner

        private enum StatusKind { Ok, Warn, Error }

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
        /// Module 1 set three ForeColor values here. Module 2 sets a custom theme STATE instead: the
        /// "status-label" appearance maps ok / warn / error to the success / warning / danger tokens.
        /// </summary>
        private void SetStatus(string text, StatusKind kind)
        {
            this.lblStatus.Text = "● " + text;
            this.lblStatus.States = new[] { kind switch { StatusKind.Error => "error", StatusKind.Warn => "warn", _ => "ok" } };
        }

        /// <summary>The banner is a docked Panel hidden by default; its colours are the "banner-label" appearance.</summary>
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

        private static string Hex(Color c)
        {
            if (c.IsEmpty)
                return "Color.Empty";

            string hex = $"#{c.R:X2}{c.G:X2}{c.B:X2}";
            if (c.A < 255)
                hex += $" α{c.A}";
            if (!string.IsNullOrEmpty(c.Name) && c.Name.StartsWith("@", StringComparison.Ordinal))
                hex += $" ({c.Name})";
            return hex;
        }

        private static string FontText(Font f)
        {
            return f == null ? "null" : $"{f.Name} {f.Size:0.#}px{(f.Bold ? " bold" : "")}";
        }

        private static string N(int value) => value.ToString(CultureInfo.InvariantCulture);

        #endregion
    }
}
