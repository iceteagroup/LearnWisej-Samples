using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using AdaptiveOps.Layout;
using AdaptiveOps.Models;
using AdaptiveOps.Shell;
using Wisej.Web;

namespace AdaptiveOps
{
    /// <summary>
    /// Adaptive Operations Console — Module 5 (FlowLayoutPanel, TableLayoutPanel, FlexLayoutPanel and
    /// the extended layout properties).
    ///
    /// The shell is still five docked regions declared in MainPage.Designer.cs — toolbar (Top, 100),
    /// status (Bottom, 28), trace (Bottom, 150), navigation (Left, 220), workspace (Fill, MinimumSize
    /// 320×240) — and docking still gets its gaps from Padding, because the default layout engine ignores
    /// Margin. What changed is the content INSIDE the regions, rebuilt with the three layout engines:
    ///
    ///   toolbar    Layout/FilterBar : FlowLayoutPanel      FlowDirection, WrapContents, SetFillWeight(search, 1),
    ///                                                      SetFlowBreak(apply, true) — the lab buttons wrap onto row 2
    ///   details    Shell/TicketEditor (TableLayoutPanel)   ColumnStyles Absolute 140 | Percent 100, RowStyles AutoSize
    ///                                                      + one Percent 100 row, SetColumnSpan(notes, 2)
    ///   workspace  Layout/DashboardWorkspace : FlexLayoutPanel   LayoutStyle Horizontal, Spacing 8, FillWeight 2 : 1,
    ///                                                      MinimumSize on both regions, MaximumSize + AlignY Top on details —
    ///                                                      built entirely in code with the Wisej.Web.Markup fluent chain
    ///   rail       FlowLayoutPanel TopDown                 the five buttons flow down; their gap is their Margin
    ///
    /// "Same region, three ways": the four metric cards sit in a TabControl whose pages host the same
    /// cards in a FlowLayoutPanel, a TableLayoutPanel (4 × Percent 25, GrowStyle AddRows) and a
    /// FlexLayoutPanel (FillWeight 1 each). SwitchEngine() re-parents the cards and the trace reports
    /// the bounds each engine produced once the client has laid them out.
    ///
    /// Lab buttons (row 2 of the filter bar):
    ///   Switch engine      success   Flow → Table → Flex → Flow; the card sizes are logged for each engine
    ///   Add cards          progress  a Wisej.Web.Timer adds one card per tick: Flow wraps, Table grows rows, Flex redistributes
    ///   Cell collision     failure   Controls.Add(card, 0, 0) into the occupied table cell — what happens is logged, caught if it throws
    ///   No-wrap overflow   failure   WrapContents = false on the Flow host (clipped) and on the filter bar (scrolled)
    ///   Restore            recovery  the default arrangement: Flow, four cards, wrapping on, filters cleared
    ///   Apply              command   ApplyFilters(); an invalid /regex/ is rejected and reported in the status label
    ///
    /// No handler here sets Bounds or Location: the containers own position and size, the base theme
    /// (Bootstrap-4, Default.json) owns the look of every standard control, and this file only moves
    /// data and re-parents cards. See docs/LayoutComparison.md, docs/LayoutNotes.md and docs/FluentMarkupRegion.md.
    /// </summary>
    public partial class MainPage : Page
    {
        private enum LayoutEngineKind { Flow = 0, Table = 1, Flex = 2 }

        private readonly TicketRepository _repository = new TicketRepository();

        /// <summary>Id of the ticket shown in the details editor, or null.</summary>
        private string _selectedId;

        /// <summary>True while the grid is being refilled, so SelectionChanged does not re-enter.</summary>
        private bool _suppressSelection;

        /// <summary>True while SwitchEngine() selects a tab, so SelectedIndexChanged does not re-enter.</summary>
        private bool _switching;

        /// <summary>The grid filter set by ApplyFilters(); null shows every ticket.</summary>
        private Func<Ticket, bool> _filter;
        private string _filterDescription = "no filter";

        /// <summary>Which host the metric cards currently live in.</summary>
        private LayoutEngineKind _engine = LayoutEngineKind.Flow;

        /// <summary>Cards added by the progress path (removed by Restore).</summary>
        private readonly List<MetricCard> _extraCards = new List<MetricCard>();

        /// <summary>The intruder of the cell-collision failure path, or null.</summary>
        private MetricCard _collisionCard;

        // The debounced "← client bounds" trace: which parts are stale and why.
        private bool _cardsDirty;
        private bool _dashboardDirty;
        private string _boundsReason = string.Empty;

        /// <summary>The extra metrics the progress path adds, one card per tick.</summary>
        private static readonly (string Title, TicketPriority? Priority, TicketStatus? Status, Color Accent)[] ExtraMetrics =
        {
            ("Critical", TicketPriority.Critical, null, Color.FromArgb(180, 35, 24)),
            ("High", TicketPriority.High, null, Color.FromArgb(181, 71, 8)),
            ("Medium", TicketPriority.Medium, null, Color.FromArgb(36, 84, 166)),
            ("Low", TicketPriority.Low, null, Color.FromArgb(103, 112, 133)),
            ("Waiting", null, TicketStatus.Waiting, Color.FromArgb(181, 71, 8)),
            ("Resolved", null, TicketStatus.Resolved, Color.FromArgb(2, 122, 72)),
        };

        public MainPage()
        {
            InitializeComponent();

            // The dashboard is built in code (Layout/DashboardWorkspace.cs, fluent markup), so its grid,
            // editor and region events are wired here rather than in the Designer file.
            this.dashboard.Grid.SelectionChanged += this.gridTickets_SelectionChanged;
            this.dashboard.Editor.SaveClick += this.editor_SaveClick;
            this.dashboard.ListRegion.Resize += this.region_Resize;
            this.dashboard.DetailsRegion.Resize += this.region_Resize;

            // Every card reports the bounds its current engine gave it (debounced into one trace line).
            foreach (var card in BaseCards)
                card.Resize += this.card_Resize;

            // Session-level event: it is unsubscribed in Dispose(bool) (MainPage.Designer.cs).
            Application.BrowserSizeChanged += this.Application_BrowserSizeChanged;

            ReportWidth("constructor");
        }

        private MetricCard[] BaseCards => new[] { this.cardOpen, this.cardOverdue, this.cardMine, this.cardClosed };

        private IEnumerable<MetricCard> AllCards => BaseCards.Concat(_extraCards);

        /// <summary>The host the cards currently live in (all three derive from Panel, so AutoScroll and ClientSize are shared).</summary>
        private Panel CurrentHost => _engine switch
        {
            LayoutEngineKind.Table => this.tableHost,
            LayoutEngineKind.Flex => this.flexHost,
            _ => this.flowHost,
        };

        #region Shell: load, resize

        private void MainPage_Load(object sender, EventArgs e)
        {
            AddTrace("• server shell built: " + DescribeShell());
            AddTrace("• server base theme: " + CurrentThemeName() + " (Default.json) owns every standard control; the containers own position and size");
            AddTrace("• server filter bar: " + this.filterBar.Describe());
            AddTrace("• server ticket editor: " + this.dashboard.Editor.Describe());
            AddTrace("• server dashboard: " + this.dashboard.Describe());
            AddTrace("• server same region, three ways: " + DescribeEngine(LayoutEngineKind.Flow) + " | " + DescribeEngine(LayoutEngineKind.Table) + " | " + DescribeEngine(LayoutEngineKind.Flex));

            VerifyShell();
            VerifyContainers();
            LoadTickets("page load");
            ReportWidth("page load");
            UpdateEngineLabel();
            SetStatus("ready", StatusKind.Normal);
        }

        /// <summary>Fired by the framework when the browser window is resized (session-level event).</summary>
        private void Application_BrowserSizeChanged(object sender, EventArgs e)
        {
            ReportWidth("Application.BrowserSizeChanged");
        }

        /// <summary>
        /// The main Page fills the browser viewport, so its Resize event fires on every window resize.
        /// The containers already answer every width: this handler only reports. It never sets Bounds.
        /// </summary>
        private void MainPage_Resize(object sender, EventArgs e)
        {
            ReportWidth("Page.Resize");
        }

        /// <summary>
        /// Writes "Browser 1348 × 738 px · Desktop" into the status bar and traces what the three engines
        /// did with the new width. Wrapped in try/catch so the label carries an honest fallback text.
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

                AddTrace($"← client resize ({reason}): browser {size.Width}×{size.Height} · page {this.Width}×{this.Height} · workspace {this.workspacePanel.Width}×{this.workspacePanel.Height} · filter bar {this.filterBar.CountRows()} row(s) · card host {this.CurrentHost.ClientSize.Width} px wide · list {this.dashboard.ListRegion.Width} / details {this.dashboard.DetailsRegion.Width}{(this.dashboard.DetailsAtMinimum ? " (details at MinimumSize)" : string.Empty)}");
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
                DescribeRegion("trace", this.tracePanel),
                DescribeRegion("status", this.statusPanel),
                DescribeRegion("workspace", this.workspacePanel) + $" min {this.workspacePanel.MinimumSize.Width}×{this.workspacePanel.MinimumSize.Height}");
        }

        private static string DescribeRegion(string name, Panel region)
        {
            return $"{name} Dock={region.Dock} {region.Width}×{region.Height}";
        }

        /// <summary>The five regions must still be docked as the shell prescribes. A failure is logged, not thrown.</summary>
        private void VerifyShell()
        {
            bool ok =
                this.toolbarPanel.Dock == DockStyle.Top &&
                this.statusPanel.Dock == DockStyle.Bottom &&
                this.tracePanel.Dock == DockStyle.Bottom &&
                this.navigationPanel.Dock == DockStyle.Left &&
                this.workspacePanel.Dock == DockStyle.Fill &&
                this.workspacePanel.MinimumSize.Width == 320 &&
                this.workspacePanel.MinimumSize.Height == 240;

            AddTrace(ok
                ? "• server dock order verified: Top, Bottom, Bottom, Left, Fill (child order in MainPage.Designer.cs) — gaps come from Padding, not Margin"
                : "• server DOCK ORDER WRONG — check the Controls.Add order at the end of InitializeComponent()");
        }

        /// <summary>
        /// The lab's acceptance criteria, read back from the containers with the Get* counterparts of the
        /// extended properties. Logged, never thrown.
        /// </summary>
        private void VerifyContainers()
        {
            var bar = this.filterBar;
            bool flowOk =
                bar.FlowDirection == FlowDirection.LeftToRight &&
                bar.WrapContents &&
                bar.GetFillWeight(bar.SearchBox) == 1 &&
                bar.SearchBox.MinimumSize.Width >= 180 &&
                bar.GetFlowBreak(bar.ApplyButton) &&
                bar.Controls.Cast<Control>().All(c => c.Dock == DockStyle.None);

            var table = this.dashboard.Editor.Table;
            var col0 = table.ColumnStyles.Count > 0 ? table.ColumnStyles[0] as ColumnStyle : null;
            var col1 = table.ColumnStyles.Count > 1 ? table.ColumnStyles[1] as ColumnStyle : null;
            bool tableOk =
                table.ColumnCount == 2 &&
                col0 != null && col0.SizeType == SizeType.Absolute && (int)col0.Width == 140 &&
                col1 != null && col1.SizeType == SizeType.Percent && (int)col1.Width == 100 &&
                table.GetColumnSpan(this.dashboard.Editor.NotesEditor) == 2 &&
                this.dashboard.Editor.NotesEditor.MinimumSize.Height >= 120;

            var flex = this.dashboard;
            bool flexOk =
                flex.LayoutStyle == FlexLayoutStyle.Horizontal &&
                flex.GetFillWeight(flex.ListRegion) == 2 &&
                flex.GetFillWeight(flex.DetailsRegion) == 1 &&
                !flex.ListRegion.MinimumSize.IsEmpty &&
                !flex.DetailsRegion.MinimumSize.IsEmpty &&
                flex.DetailsRegion.MaximumSize.Width > 0 &&
                flex.GetAlignY(flex.DetailsRegion) == VerticalAlignment.Top;

            AddTrace($"• server acceptance check: flow {(flowOk ? "ok" : "FAILED")} (WrapContents, FillWeight(search)=1 + MinimumSize, FlowBreak(apply), no Dock on children) · "
                   + $"table {(tableOk ? "ok" : "FAILED")} (Absolute 140 | Percent 100, notes span 2, notes min height 120) · "
                   + $"flex {(flexOk ? "ok" : "FAILED")} (Horizontal, 2 : 1, MinimumSize on both, MaximumSize on details, AlignY Top)");
        }

        private static string CurrentThemeName()
        {
            try { return Application.Theme?.Name ?? "(default)"; }
            catch (Exception) { return "(default)"; }
        }

        #endregion

        #region Filter bar commands

        private void filterBar_Command(object sender, FilterBarCommandEventArgs e)
        {
            switch (e.Command)
            {
                case FilterBarCommand.Apply:
                    ApplyFilters();
                    break;
                case FilterBarCommand.SwitchEngine:
                    SwitchEngine(NextEngine(_engine), "Switch engine");
                    break;
                case FilterBarCommand.AddCards:
                    StartAddCards();
                    break;
                case FilterBarCommand.CellCollision:
                    CellCollision();
                    break;
                case FilterBarCommand.NoWrapOverflow:
                    NoWrapOverflow();
                    break;
                case FilterBarCommand.Restore:
                    Restore();
                    break;
                case FilterBarCommand.ClearTrace:
                    this.listTrace.Items.Clear();
                    break;
            }
        }

        /// <summary>
        /// The thin command behind Apply: builds the grid filter from the bar and re-reads the grid.
        /// "/pattern/" is a regular expression; one that does not parse is the failure path — the
        /// exception is caught, the status label says why, and the grid keeps its last good filter
        /// instead of being left half-updated.
        /// </summary>
        private void ApplyFilters()
        {
            string text = (this.filterBar.SearchText ?? string.Empty).Trim();
            string status = this.filterBar.StatusFilter;

            try
            {
                Func<Ticket, bool> textMatch = _ => true;
                string textDescription = "any text";
                if (text.Length >= 2 && text.StartsWith("/", StringComparison.Ordinal) && text.EndsWith("/", StringComparison.Ordinal))
                {
                    var regex = new Regex(text.Substring(1, text.Length - 2), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(250));
                    textMatch = t => regex.IsMatch(Haystack(t));
                    textDescription = $"regex {text}";
                }
                else if (text.Length > 0)
                {
                    textMatch = t => Haystack(t).IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0;
                    textDescription = $"contains \"{text}\"";
                }

                TicketStatus? wanted = status == null ? (TicketStatus?)null : Enum.Parse<TicketStatus>(status);
                _filter = t => textMatch(t) && (wanted == null || t.Status == wanted.Value);
                _filterDescription = $"{textDescription}, status {status ?? "any"}";

                HideBanner();
                int shown = LoadTickets("Apply");
                SetStatus($"filters applied · {shown} of {_repository.GetAll().Count} tickets", StatusKind.Normal);
            }
            catch (ArgumentException ex)
            {
                // Failure path: RegexParseException derives from ArgumentException. Nothing was changed —
                // the grid still shows the last good filter and no container was touched.
                string message = ex.Message.Split('\n')[0].Trim();
                AddTrace($"• server ApplyFilters rejected: {message} — grid left as it was ({_filterDescription})");
                ShowBanner($"✖ ApplyFilters failed: {message} The grid keeps its last good filter.");
                SetStatus("filter error: invalid /regex/", StatusKind.Error);
            }
        }

        private static string Haystack(Ticket t)
        {
            return string.Join(" ", t.Id, t.Title, t.Owner, t.Notes);
        }

        #endregion

        #region Success path: Switch engine (Flow → Table → Flex)

        private static LayoutEngineKind NextEngine(LayoutEngineKind engine)
        {
            return (LayoutEngineKind)(((int)engine + 1) % 3);
        }

        /// <summary>A click on a tab is the same switch as the button; the guard stops the re-entry from SwitchEngine().</summary>
        private void metricsTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_switching)
                return;

            var target = (LayoutEngineKind)Math.Max(0, Math.Min(2, this.metricsTabs.SelectedIndex));
            if (target != _engine)
                SwitchEngine(target, "tab click");
        }

        /// <summary>
        /// Moves every metric card into the host of the requested engine and gives it the extended
        /// properties that engine reads: nothing for Flow (each card keeps its own 192×76 and wraps),
        /// a cell + Dock=Fill for Table, FillWeight 1 + AlignY Top for Flex. Controls.Add re-parents.
        /// </summary>
        private void SwitchEngine(LayoutEngineKind engine, string reason)
        {
            var cards = AllCards.ToList();

            switch (engine)
            {
                case LayoutEngineKind.Flow:
                    foreach (var card in cards)
                    {
                        card.Dock = DockStyle.None;
                        card.Size = new Size(192, 76);
                        this.flowHost.Controls.Add(card);
                    }
                    break;

                case LayoutEngineKind.Table:
                    EnsureTableRows(cards.Count);
                    for (int i = 0; i < cards.Count; i++)
                    {
                        cards[i].Dock = DockStyle.Fill;
                        this.tableHost.Controls.Add(cards[i], i % this.tableHost.ColumnCount, i / this.tableHost.ColumnCount);
                    }
                    break;

                case LayoutEngineKind.Flex:
                    foreach (var card in cards)
                    {
                        card.Dock = DockStyle.None;
                        card.Size = new Size(192, 76);
                        this.flexHost.Controls.Add(card);
                        this.flexHost.SetFillWeight(card, 1);
                        this.flexHost.SetAlignY(card, VerticalAlignment.Top);
                    }
                    break;
            }

            _engine = engine;

            _switching = true;
            try
            {
                if (this.metricsTabs.SelectedIndex != (int)engine)
                    this.metricsTabs.SelectedIndex = (int)engine;
            }
            finally
            {
                _switching = false;
            }

            AddTrace($"• server engine → {engine} ({reason}): {DescribeEngine(engine)}");
            UpdateEngineLabel();
            RequestBoundsTrace($"after switch to {engine}", cards: true, dashboard: false);
        }

        /// <summary>Adds an Absolute 84 RowStyle for every table row the cards need (GrowStyle adds the rows themselves).</summary>
        private void EnsureTableRows(int cardCount)
        {
            int rows = Math.Max(1, (cardCount + this.tableHost.ColumnCount - 1) / this.tableHost.ColumnCount);
            if (this.tableHost.RowCount < rows)
                this.tableHost.RowCount = rows;
            while (this.tableHost.RowStyles.Count < this.tableHost.RowCount)
                this.tableHost.RowStyles.Add(new RowStyle(SizeType.Absolute, 84F));
        }

        /// <summary>What the engine is configured to do with the cards, from the properties the Designer set.</summary>
        private string DescribeEngine(LayoutEngineKind engine)
        {
            int n = AllCards.Count();
            switch (engine)
            {
                case LayoutEngineKind.Table:
                    string cells = string.Join(", ", this.tableHost.Controls.Cast<Control>().Select(c => $"{c.Name}@{this.tableHost.GetPositionFromControl(c)}"));
                    return $"Table host: TableLayoutPanel {this.tableHost.ColumnCount} cols × {this.tableHost.RowCount} row(s), ColumnStyles Percent 25 ×4, RowStyles Absolute 84, GrowStyle {this.tableHost.GrowStyle}; cards Dock=Fill in their cells → each takes a quarter of the width{(cells.Length > 0 ? " · " + cells : string.Empty)}";
                case LayoutEngineKind.Flex:
                    return $"Flex host: FlexLayoutPanel {this.flexHost.LayoutStyle}, Spacing {this.flexHost.Spacing}; {n} cards with FillWeight 1 each (MinimumSize 140) and AlignY Top → equal shares of one row, never wrapping";
                default:
                    return $"Flow host: FlowLayoutPanel {this.flowHost.FlowDirection}, WrapContents {this.flowHost.WrapContents}, AutoScroll {this.flowHost.AutoScroll}; {n} cards, each its own 192×76 with Margin 4 and FillWeight 0 → as many per row as fit, then wrap";
            }
        }

        private void UpdateEngineLabel()
        {
            var cards = AllCards.Where(c => c.Parent != null).ToList();
            int rows = cards.Select(c => c.Top).Distinct().Count();
            this.lblEngine.Text = $"engine: {_engine} · {cards.Count} cards · {rows} row(s)";
        }

        #endregion

        #region Client bounds trace (debounced)

        private void card_Resize(object sender, EventArgs e)
        {
            RequestBoundsTrace("card Resize", cards: true, dashboard: false);
        }

        private void region_Resize(object sender, EventArgs e)
        {
            RequestBoundsTrace("region Resize", cards: false, dashboard: true);
        }

        /// <summary>
        /// The engines lay the cards out on the client and the framework writes the bounds back; one
        /// switch produces several Resize events, so they are collected and traced once by timerSizes.
        /// </summary>
        private void RequestBoundsTrace(string reason, bool cards, bool dashboard)
        {
            _boundsReason = reason;
            _cardsDirty |= cards;
            _dashboardDirty |= dashboard;
            if (!this.timerSizes.Enabled)
                this.timerSizes.Start();
        }

        private void timerSizes_Tick(object sender, EventArgs e)
        {
            this.timerSizes.Stop();

            if (_cardsDirty)
                AddTrace($"← client card bounds ({_boundsReason}) · {DescribeCardBounds()}");
            if (_dashboardDirty)
                AddTrace($"← client dashboard ({_boundsReason}) · {this.dashboard.Describe()}");

            _cardsDirty = false;
            _dashboardDirty = false;
            UpdateEngineLabel();
        }

        /// <summary>"Flow: 4 cards in 1 row(s) · host 1096×160 · open 192×76 @(4,4) · …" plus clipping and table geometry.</summary>
        private string DescribeCardBounds()
        {
            var host = this.CurrentHost;
            var cards = AllCards.Where(c => c.Parent == host).ToList();
            if (_collisionCard != null && _collisionCard.Parent == host)
                cards.Add(_collisionCard);

            int rows = cards.Select(c => c.Top).Distinct().Count();
            int beyond = cards.Count(c => c.Right > host.ClientSize.Width);
            string clipping = beyond > 0
                ? $" · {beyond} card(s) past the right edge ({(host.AutoScroll ? "scrolled" : "CLIPPED")})"
                : string.Empty;
            string toolbar = this.filterBar.Overflow() > 0
                ? $" · toolbar overflows by {this.filterBar.Overflow()} px ({(this.filterBar.AutoScroll ? "scrolled" : "CLIPPED")})"
                : string.Empty;
            string table = _engine == LayoutEngineKind.Table
                ? $" · col widths {string.Join(",", this.tableHost.GetColumnWidths())} · row heights {string.Join(",", this.tableHost.GetRowHeights())}"
                : string.Empty;

            return $"{_engine}: {cards.Count} cards in {rows} row(s) · host {host.ClientSize.Width}×{host.ClientSize.Height}{clipping}{toolbar}{table} · {string.Join(" · ", cards.Select(c => c.DescribeBounds()))}";
        }

        #endregion

        #region Progress path: Add cards (Wisej.Web.Timer)

        private void StartAddCards()
        {
            if (this.timerAddCards.Enabled)
                return;

            if (_extraCards.Count >= ExtraMetrics.Length)
            {
                AddTrace("• server add cards: all six extra cards are already placed — Restore first");
                return;
            }

            this.filterBar.AddCardsEnabled = false;
            this.filterBar.ProgressText = $"Adding cards · {_extraCards.Count} of {ExtraMetrics.Length}";
            SetStatus("adding cards", StatusKind.Warn);
            AddTrace($"• server add cards started: one card every {this.timerAddCards.Interval} ms into the {_engine} host (Wisej.Web.Timer, no client code)");
            this.timerAddCards.Start();
        }

        private void timerAddCards_Tick(object sender, EventArgs e)
        {
            var metric = ExtraMetrics[_extraCards.Count];
            var card = new MetricCard
            {
                Name = "cardExtra" + (_extraCards.Count + 1).ToString(CultureInfo.InvariantCulture),
                Title = metric.Title,
                Accent = metric.Accent,
                Value = N(CountMetric(metric.Priority, metric.Status)),
            };
            card.Resize += this.card_Resize;
            _extraCards.Add(card);

            int total = AllCards.Count();
            switch (_engine)
            {
                case LayoutEngineKind.Table:
                    // No cell given: the engine takes the next free cell and GrowStyle AddRows adds the row.
                    card.Dock = DockStyle.Fill;
                    this.tableHost.Controls.Add(card);
                    EnsureTableRows(total);
                    AddTrace($"→ render card {total}: {card.Name} \"{metric.Title}\" added with Controls.Add(card) — the table placed it at {this.tableHost.GetPositionFromControl(card)} (GrowStyle {this.tableHost.GrowStyle}, RowCount now {this.tableHost.RowCount})");
                    break;

                case LayoutEngineKind.Flex:
                    this.flexHost.Controls.Add(card);
                    this.flexHost.SetFillWeight(card, 1);
                    this.flexHost.SetAlignY(card, VerticalAlignment.Top);
                    AddTrace($"→ render card {total}: {card.Name} \"{metric.Title}\" added with FillWeight 1 — {total} equal shares of {this.flexHost.ClientSize.Width} px, none below 140");
                    break;

                default:
                    this.flowHost.Controls.Add(card);
                    AddTrace($"→ render card {total}: {card.Name} \"{metric.Title}\" added to the Flow host — 200 px each in {this.flowHost.ClientSize.Width} px: wraps when the row is full");
                    break;
            }

            this.filterBar.ProgressText = $"Adding cards · {_extraCards.Count} of {ExtraMetrics.Length} · {metric.Title}";
            UpdateEngineLabel();
            RequestBoundsTrace($"after card {total}", cards: true, dashboard: false);

            if (_extraCards.Count < ExtraMetrics.Length)
                return;

            this.timerAddCards.Stop();
            this.filterBar.AddCardsEnabled = true;
            this.filterBar.ProgressText = $"Added {ExtraMetrics.Length} cards · {total} in the {_engine} host";
            SetStatus("ready", StatusKind.Normal);
        }

        private int CountMetric(TicketPriority? priority, TicketStatus? status)
        {
            return _repository.GetAll().Count(t =>
                (priority == null || t.Priority == priority.Value) &&
                (status == null || t.Status == status.Value));
        }

        private void RefreshExtraCards()
        {
            for (int i = 0; i < _extraCards.Count && i < ExtraMetrics.Length; i++)
                _extraCards[i].Value = N(CountMetric(ExtraMetrics[i].Priority, ExtraMetrics[i].Status));
        }

        #endregion

        #region Failure paths: cell collision, no-wrap overflow

        /// <summary>
        /// Adds a card to cell (0,0) of the table host while the Open card already occupies it. What the
        /// framework does with two controls in one cell is logged (position of both, who GetControlFromPosition
        /// returns), and caught if Add throws. Restore removes the intruder.
        /// </summary>
        private void CellCollision()
        {
            if (_engine != LayoutEngineKind.Table)
                SwitchEngine(LayoutEngineKind.Table, "Cell collision needs the Table host");

            if (_collisionCard != null)
            {
                AddTrace("• server cell collision: the intruder is already in the table — Restore first");
                return;
            }

            var occupant = this.tableHost.GetControlFromPosition(0, 0);
            AddTrace($"• server cell collision: cell (0,0) holds {occupant?.Name ?? "nothing"}; calling tableHost.Controls.Add(cardCollision, 0, 0) …");

            var intruder = new MetricCard
            {
                Name = "cardCollision",
                Title = "Collision",
                Value = "!",
                Accent = Color.FromArgb(180, 35, 24),
                Dock = DockStyle.Fill,
            };

            try
            {
                this.tableHost.Controls.Add(intruder, 0, 0);
                _collisionCard = intruder;
                _collisionCard.Resize += this.card_Resize;

                var intruderPos = this.tableHost.GetPositionFromControl(intruder);
                string occupantPos = occupant != null ? this.tableHost.GetPositionFromControl(occupant).ToString() : "-";
                var now = this.tableHost.GetControlFromPosition(0, 0);
                AddTrace($"• server cell collision: no exception — Add accepted the cell. cardCollision reports {intruderPos}, {occupant?.Name ?? "occupant"} reports {occupantPos}, GetControlFromPosition(0,0) = {now?.Name ?? "null"}, RowCount {this.tableHost.RowCount}. Look at the Table tab: the engine either draws the two on top of each other or pushes one to the next free cell");
                ShowBanner("✖ Cell collision: two controls claim cell (0,0) of the TableLayoutPanel — the trace says what the engine did; Restore removes the intruder.");
            }
            catch (Exception ex)
            {
                intruder.Dispose();
                AddTrace($"• server cell collision: Controls.Add threw {ex.GetType().Name}: {ex.Message}");
                ShowBanner($"✖ Cell collision rejected by the framework: {ex.Message}");
            }

            SetStatus("layout fault: cell collision", StatusKind.Error);
            RequestBoundsTrace("after cell collision", cards: true, dashboard: false);
        }

        /// <summary>
        /// WrapContents = false on the Flow host (AutoScroll off → the cards past the edge are clipped)
        /// and on the filter bar (AutoScroll stays on → its two rows become one long scrolled row).
        /// The bounds trace counts the cards beyond the right edge.
        /// </summary>
        private void NoWrapOverflow()
        {
            if (_engine != LayoutEngineKind.Flow)
                SwitchEngine(LayoutEngineKind.Flow, "No-wrap overflow needs the Flow host");

            this.flowHost.WrapContents = false;
            this.flowHost.AutoScroll = false;
            this.filterBar.WrapContents = false;

            int cards = AllCards.Count();
            AddTrace($"• server no-wrap: flowHost.WrapContents=false, AutoScroll=false → one row of {cards} × 200 px = {cards * 200} px in a {this.flowHost.ClientSize.Width}-px host: anything past the edge is clipped · filterBar.WrapContents=false (AutoScroll stays true) → the FlowBreak is ignored, both toolbar rows become one and scroll horizontally");
            ShowBanner("✖ WrapContents = false: the Flow host clips its cards and the toolbar overflows into a horizontal scrollbar — Restore turns wrapping back on.");
            SetStatus("layout fault: no wrap", StatusKind.Error);
            RequestBoundsTrace("after WrapContents=false", cards: true, dashboard: false);
        }

        #endregion

        #region Recovery: Restore

        /// <summary>The default arrangement: Flow engine, the four base cards, wrapping on, filters cleared, banner hidden.</summary>
        private void Restore()
        {
            if (this.timerAddCards.Enabled)
                this.timerAddCards.Stop();
            this.filterBar.AddCardsEnabled = true;
            this.filterBar.ProgressText = string.Empty;

            foreach (var card in _extraCards)
                RemoveCard(card);
            _extraCards.Clear();

            if (_collisionCard != null)
            {
                RemoveCard(_collisionCard);
                _collisionCard = null;
            }

            this.flowHost.WrapContents = true;
            this.flowHost.AutoScroll = true;
            this.filterBar.WrapContents = true;

            this.filterBar.SearchText = string.Empty;
            this.filterBar.StatusFilter = null;
            _filter = null;
            _filterDescription = "no filter";

            HideBanner();
            SwitchEngine(LayoutEngineKind.Flow, "Restore");
            LoadTickets("Restore");
            AddTrace("• server restored the default arrangement: Flow engine, 4 cards, WrapContents on, filters cleared, banner hidden");
            SetStatus("ready", StatusKind.Normal);
        }

        private void RemoveCard(MetricCard card)
        {
            card.Resize -= this.card_Resize;
            card.Parent?.Controls.Remove(card);
            card.Dispose();
        }

        #endregion

        #region Tickets: load, select, save

        /// <summary>
        /// Re-reads everything from the repository: the metric cards (always all tickets), the grid
        /// (through the current filter) and, when a ticket is selected, the details editor. Returns the
        /// number of rows shown.
        /// </summary>
        private int LoadTickets(string reason)
        {
            var today = DateTime.Today;
            var all = _repository.GetAll();
            var shown = _filter == null ? all : all.Where(_filter).ToList();

            this.cardOpen.Value = N(_repository.CountOpen());
            this.cardOverdue.Value = N(_repository.CountOverdue(today));
            this.cardMine.Value = N(_repository.CountAssignedToMe());
            this.cardClosed.Value = N(_repository.CountClosedThisWeek(today));
            RefreshExtraCards();

            var grid = this.dashboard.Grid;
            string keep = _selectedId;
            int reselect = -1;

            _suppressSelection = true;
            try
            {
                grid.Rows.Clear();
                foreach (var t in shown)
                {
                    int index = grid.Rows.Add(new object[]
                    {
                        t.Id,
                        t.Title,
                        t.Priority.ToString(),
                        t.Status.ToString(),
                        t.Owner,
                        t.DueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                    });
                    grid.Rows[index].Tag = t.Id;
                    if (t.Id == keep)
                        reselect = index;
                }

                if (reselect < 0 && grid.Rows.Count > 0)
                    reselect = 0;

                grid.ClearSelection();
                if (reselect >= 0)
                    grid.Rows[reselect].Selected = true;
            }
            finally
            {
                _suppressSelection = false;
            }

            AddTrace($"→ render {reason}: {shown.Count} of {all.Count} tickets ({_filterDescription}) · open {this.cardOpen.Value} · overdue {this.cardOverdue.Value} · mine {this.cardMine.Value} · closed this week {this.cardClosed.Value}");

            if (reselect >= 0)
                ShowTicket((string)grid.Rows[reselect].Tag, reason);
            else
                ClearEditor();

            return shown.Count;
        }

        private void gridTickets_SelectionChanged(object sender, EventArgs e)
        {
            if (_suppressSelection)
                return;

            var grid = this.dashboard.Grid;
            var row = grid.SelectedRows.Count > 0 ? grid.SelectedRows[0] : grid.CurrentRow;

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
            this.dashboard.Editor.ShowTicket(t);
            AddTrace($"← client {reason}: ticket {t.Id} \"{t.Title}\" loaded into the TableLayoutPanel editor");
        }

        private void ClearEditor()
        {
            _selectedId = null;
            this.dashboard.Editor.Clear();
        }

        private void editor_SaveClick(object sender, EventArgs e)
        {
            if (_selectedId == null)
            {
                ShowBanner("Select a ticket in the grid before saving.");
                SetStatus("nothing selected", StatusKind.Warn);
                AddTrace("• server save skipped: no ticket selected");
                return;
            }

            SaveTicket(this.dashboard.Editor.Read(_selectedId), "Save");
        }

        /// <summary>
        /// The only write path. TicketRepository.Save validates on the server and throws
        /// TicketValidationException; the catch block is the server-side failure path.
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
                    alignment: ContentAlignment.TopRight, autoCloseDelay: 4000);
                return true;
            }
            catch (TicketValidationException ex)
            {
                ShowBanner($"✖ Rejected on the server ({origin}): {ex.Message} Nothing was written.");
                AddTrace($"• server rejected {ticket.Id ?? "(none)"} ({origin}): {ex.Message}");
                SetStatus("validation error", StatusKind.Error);
                AlertBox.Show(ex.Message, MessageBoxIcon.Warning,
                    alignment: ContentAlignment.TopRight, autoCloseDelay: 4000);
                return false;
            }
        }

        #endregion

        #region Navigation rail

        /// <summary>The rail is a TopDown FlowLayoutPanel of five Buttons; a click only retitles the list region and is traced.</summary>
        private void btnNav_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null)
                return;

            this.dashboard.ListTitle.Text = button.Text == "Dashboard" ? "Tickets" : button.Text;
            AddTrace($"← client navigation: {button.Text} (rail = FlowLayoutPanel {this.navigationRail.FlowDirection}, WrapContents {this.navigationRail.WrapContents}; the 8-px gaps are each button's Margin)");
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

        private void SetStatus(string text, StatusKind kind)
        {
            this.lblStatus.Text = "● " + text;

            // The three status colours are control properties in this build; the theme modules of the
            // course move them into colour tokens (success / warning / danger).
            this.lblStatus.ForeColor = kind switch
            {
                StatusKind.Error => Color.FromArgb(180, 35, 24),
                StatusKind.Warn => Color.FromArgb(181, 71, 8),
                _ => Color.FromArgb(2, 122, 72),
            };
        }

        /// <summary>The banner is a docked Panel hidden by default: showing it pushes the dashboard down, no Bounds involved.</summary>
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
