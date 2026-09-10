using System;
using System.Globalization;
using AdaptiveOps.Models;
using AdaptiveOps.Shell;
using Wisej.Web;

namespace AdaptiveOps
{
    /// <summary>
    /// Adaptive Operations Console — Module 4 shell (Layout Fundamentals: Docking, Anchoring,
    /// AutoSize, AutoScroll and Shell Composition).
    ///
    /// The shell is five docked regions declared in MainPage.Designer.cs — toolbar (Top, 56),
    /// status (Bottom, 28), navigation (Left, 220), details (Right, 340, Min 260 / Max 480) and the
    /// workspace (Fill, MinimumSize 320×240) — composed by Dock and child order only. Four of the
    /// regions host a UserControl with its own local layout: Shell/NavigationRail (anchored buttons
    /// in an AutoScroll rail), Shell/DetailsEditor (header · scrolling fields · command bar with
    /// Bottom|Right buttons), Shell/Workspace (metric cards, tabbed grid with MinimumSize, trace)
    /// and Shell/StatusBar (three docked labels). The page knows their public surface — a Ticket
    /// property, a Saved event, a SelectedSection — and none of their child controls.
    ///
    /// There is no Resize handler that sets Bounds. MainPage_Resize and
    /// Application.BrowserSizeChanged only REPORT: after every browser resize the trace lists the
    /// size of each region, whether the rail and the details fields scroll, and whether a
    /// MinimumSize is holding. The "before" — five panels positioned by a Resize handler — lives on
    /// in the workspace's second tab (Lab/ResizeCodeTwin) so both can be resized side by side.
    ///
    /// Toolbar buttons exercise the lab paths:
    ///   Compose shell       success   re-apply the composition (child order, Dock, sizes) and log every region
    ///   Animate details     progress  a Wisej.Web.Timer steps the details width 340 → 220 → 560 → 340; the
    ///                                 docked workspace follows and MinimumSize / MaximumSize clamp the requests
    ///   Swap dock order     failure   the workspace is moved to the end of the child order and docks FIRST:
    ///                                 it takes the whole page and the other regions cover it
    ///   Dock+Anchor fight   failure   an Anchor is set on the docked details region; the trace logs what the
    ///                                 framework did with Dock and Anchor
    ///   Squeeze workspace   failure   the workspace MinimumSize is raised above the remaining width: the guard
    ///                                 rail holds and the page (AutoScroll) grows a horizontal scrollbar
    ///   Restore             recovery  composition and seed tickets restored
    /// Saving from the details editor exercises validation (empty title → rejected on the server)
    /// and a thrown exception (owner "fault") caught around the Saved event.
    /// See docs/ShellComposition.md and docs/LayoutComparison.md.
    /// </summary>
    public partial class MainPage : Page
    {
        private readonly TicketRepository _repository = new TicketRepository();

        /// <summary>Id of the ticket shown in the details editor, or null.</summary>
        private string _selectedId;

        /// <summary>Progress path: index into <see cref="AnimationWidths"/>.</summary>
        private int _animationStep;

        /// <summary>What the lab buttons did to the composition since the last Compose/Restore.</summary>
        private string _compositionState = "composed";

        private const int NavigationWidth = 220;
        private const int DetailsWidth = 340;
        private static readonly System.Drawing.Size WorkspaceMinimum = new System.Drawing.Size(320, 240);

        /// <summary>
        /// The widths the progress path asks for. 240 and 220 are below the details region's
        /// MinimumSize (260); 520 and 560 are above its MaximumSize (480): the framework clamps them.
        /// </summary>
        private static readonly int[] AnimationWidths =
        {
            320, 300, 280, 260, 240, 220, 260, 320, 380, 440, 480, 520, 560, 480, 400, 340,
        };

        public MainPage()
        {
            InitializeComponent();

            // Session-level event: it is unsubscribed in Dispose(bool) (MainPage.Designer.cs).
            Application.BrowserSizeChanged += this.Application_BrowserSizeChanged;

            ReportBrowser("constructor");
        }

        #region Shell: load, resize, composition

        private void MainPage_Load(object sender, EventArgs e)
        {
            AddTrace("• server shell built from nested containers: " + DescribeShell());
            AddTrace("• server 0 Bounds/Location/Size assignments in MainPage on resize — Dock and child order do the job (the Resize-code twin tab keeps the before)");
            VerifyComposition();

            // The Margin demo of the lab: 12 px of Margin on the docked rail, and the rail still sits
            // exactly at the page's left edge. The gap next to it is navigationPanel.Padding.Left.
            AddTrace($"• server Margin ignored by Dock: navigationPanel.Margin={this.navigationPanel.Margin.Left} but Left={this.navigationPanel.Left}, Top={this.navigationPanel.Top} (toolbar height) — the {this.navigationPanel.Padding.Left} px gap is Padding");

            AddTrace("• server AutoSize: captions and card titles are AutoSize labels (content decides); subtitles, values and status labels are fixed + AutoEllipsis (container decides); no container auto-sizes from children docked back to it");
            AddTrace("• server base theme: " + CurrentThemeName() + " (selected in Default.json) owns the look of every control; this module only moves layout");
            this.statusBar.ThemeText = "theme: " + CurrentThemeName();

            LoadTickets("page load");
            ReportLayout("page load");
            this.statusBar.ShowStatus("ready", StatusKind.Normal);
        }

        /// <summary>Fired by the framework when the browser window is resized (session-level event).</summary>
        private void Application_BrowserSizeChanged(object sender, EventArgs e)
        {
            ReportLayout("Application.BrowserSizeChanged");
        }

        /// <summary>
        /// The main Page fills the browser viewport, so Resize fires on every window resize. This
        /// handler only reports what the layout engine did; it never sets Bounds, Location or Size.
        /// </summary>
        private void MainPage_Resize(object sender, EventArgs e)
        {
            ReportLayout("Page.Resize");
        }

        /// <summary>Writes "Browser 1348 × 680 px · Desktop" into the status bar; returns false when the size is not known yet.</summary>
        private bool ReportBrowser(string reason)
        {
            try
            {
                var browser = Application.Browser;
                var size = browser.Size;
                if (size.Width <= 0 || size.Height <= 0)
                    throw new InvalidOperationException("the browser has not reported its size yet");

                string device = string.IsNullOrEmpty(browser.Device) ? "Desktop" : browser.Device;
                this.statusBar.BrowserText = $"Browser {size.Width} × {size.Height} px · {device} · {_compositionState}";
                return true;
            }
            catch (Exception ex)
            {
                this.statusBar.BrowserText = "Browser size unavailable — " + ex.Message;
                AddTrace($"• server width read failed ({reason}): {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// After every resize: the browser size, the five regions as the engine laid them out, the
        /// scroll state of the rail and of the details fields, and whether a MinimumSize is holding.
        /// </summary>
        private void ReportLayout(string reason)
        {
            if (!ReportBrowser(reason))
                return;

            var size = Application.Browser.Size;
            AddTrace($"← client resize ({reason}): browser {size.Width}×{size.Height} · page {this.Width}×{this.Height} · {DescribeShell()}");
            AddTrace("    " + this.navigationRail.DescribeScroll());
            AddTrace("    " + this.detailsEditor.DescribeScroll() + " · " + this.workspace.DescribeGrid());

            int remaining = RemainingWidthForWorkspace();
            if (remaining < this.workspacePanel.MinimumSize.Width)
                AddTrace($"    ✕ MinimumSize holds: {remaining} px left for the workspace < minimum {this.workspacePanel.MinimumSize.Width} → the fill region keeps {this.workspacePanel.Width} px and the page scrolls horizontally (Page.AutoScroll)");
        }

        /// <summary>Width the Fill region gets after the Left and Right regions and the page Padding took theirs.</summary>
        private int RemainingWidthForWorkspace()
        {
            return this.ClientSize.Width - this.Padding.Horizontal - this.navigationPanel.Width - this.detailsPanel.Width;
        }

        /// <summary>One line with the Dock and size of the five regions, as the running app laid them out.</summary>
        private string DescribeShell()
        {
            return string.Join(" · ",
                DescribeRegion("toolbar", this.toolbarPanel),
                DescribeRegion("rail", this.navigationPanel),
                DescribeRegion("workspace", this.workspacePanel) + $" min {this.workspacePanel.MinimumSize.Width}×{this.workspacePanel.MinimumSize.Height}",
                DescribeRegion("details", this.detailsPanel) + $" min {this.detailsPanel.MinimumSize.Width} max {this.detailsPanel.MaximumSize.Width}",
                DescribeRegion("status", this.statusPanel));
        }

        private static string DescribeRegion(string name, Control region)
        {
            return $"{name} Dock={region.Dock} {region.Width}×{region.Height}";
        }

        /// <summary>
        /// The success path and the recovery: the composition the module prescribes, applied in code
        /// exactly as the Designer applied it. Child order first (it IS the docking priority), then
        /// Dock on each region, then the sizes and guard rails. No Bounds anywhere.
        /// </summary>
        private void ComposeShell(string reason)
        {
            if (this.timerAnimate.Enabled)
            {
                this.timerAnimate.Stop();
                this.btnAnimate.Enabled = true;
            }

            this.SuspendLayout();
            try
            {
                // Child order = dock priority: index 0 is docked LAST (Fill takes what is left).
                this.Controls.SetChildIndex(this.workspacePanel, 0);
                this.Controls.SetChildIndex(this.detailsPanel, 1);
                this.Controls.SetChildIndex(this.navigationPanel, 2);
                this.Controls.SetChildIndex(this.statusPanel, 3);
                this.Controls.SetChildIndex(this.toolbarPanel, 4);

                this.toolbarPanel.Dock = DockStyle.Top;
                this.statusPanel.Dock = DockStyle.Bottom;
                this.navigationPanel.Dock = DockStyle.Left;
                this.detailsPanel.Dock = DockStyle.Right;
                this.workspacePanel.Dock = DockStyle.Fill;

                this.navigationPanel.Width = NavigationWidth;
                this.detailsPanel.Width = DetailsWidth;
                this.workspacePanel.MinimumSize = WorkspaceMinimum;
            }
            finally
            {
                this.ResumeLayout(true);
            }

            _compositionState = "composed";
            this.lblProgress.Text = string.Empty;
            AddTrace($"• server composition applied ({reason}): child order [workspace, details, rail, status, toolbar] → docked Top, Bottom, Left, Right, Fill · {DescribeShell()}");
            VerifyComposition();
            ReportLayout(reason);
        }

        /// <summary>
        /// Acceptance check of the lab: the five regions must be docked as the module prescribes and
        /// in the right child order. A failure is logged, not thrown, so the shell still shows.
        /// </summary>
        private void VerifyComposition()
        {
            bool docks =
                this.toolbarPanel.Dock == DockStyle.Top &&
                this.statusPanel.Dock == DockStyle.Bottom &&
                this.navigationPanel.Dock == DockStyle.Left &&
                this.detailsPanel.Dock == DockStyle.Right &&
                this.workspacePanel.Dock == DockStyle.Fill;

            bool order = this.Controls.GetChildIndex(this.workspacePanel) == 0;

            bool guards =
                this.workspacePanel.MinimumSize == WorkspaceMinimum &&
                this.detailsPanel.MinimumSize.Width == 260 &&
                this.detailsPanel.MaximumSize.Width == 480;

            AddTrace(docks && order && guards
                ? "• server composition verified: Dock Top/Bottom/Left/Right/Fill, workspace docked last (child index 0), MinimumSize 320×240 on the workspace, 260–480 on the details region"
                : $"✕ server composition BROKEN: docks {(docks ? "ok" : "wrong")}, child order {(order ? "ok" : "wrong — workspace is not docked last")}, guard rails {(guards ? "ok" : "changed")} — click Restore");
        }

        private static string CurrentThemeName()
        {
            try { return Application.Theme?.Name ?? "(default)"; }
            catch (Exception) { return "(default)"; }
        }

        #endregion

        #region Success path: Compose shell

        private void btnCompose_Click(object sender, EventArgs e)
        {
            ComposeShell("Compose shell");
            this.statusBar.ShowStatus("ready", StatusKind.Normal);
        }

        #endregion

        #region Progress path: Animate details (Wisej.Web.Timer)

        private void btnAnimate_Click(object sender, EventArgs e)
        {
            if (this.timerAnimate.Enabled)
                return;

            _animationStep = 0;
            this.btnAnimate.Enabled = false;
            this.lblProgress.Text = $"Animate details · step 0 of {AnimationWidths.Length}";
            this.statusBar.ShowStatus("animating", StatusKind.Warn);
            AddTrace($"• server animation started: {AnimationWidths.Length} widths for the details region every {this.timerAnimate.Interval} ms (Wisej.Web.Timer); the workspace is docked Fill, so it follows without code");
            this.timerAnimate.Start();
        }

        private void timerAnimate_Tick(object sender, EventArgs e)
        {
            int requested = AnimationWidths[_animationStep];
            this.detailsPanel.Width = requested;
            int actual = this.detailsPanel.Width;

            string clamp = actual == requested
                ? string.Empty
                : actual < requested ? " (clamped by MaximumSize)" : " (clamped by MinimumSize)";

            _animationStep++;
            this.lblProgress.Text = $"Animate details · step {_animationStep} of {AnimationWidths.Length} · details {actual} px";
            AddTrace($"→ render step {_animationStep}/{AnimationWidths.Length}: details.Width requested {requested} → actual {actual}{clamp} · workspace {this.workspacePanel.Width}×{this.workspacePanel.Height} · {this.detailsEditor.DescribeScroll()}");

            if (_animationStep < AnimationWidths.Length)
                return;

            this.timerAnimate.Stop();
            this.btnAnimate.Enabled = true;
            this.lblProgress.Text = $"Animate details complete · details back to {this.detailsPanel.Width} px";
            AddTrace("• server animation complete: every step was one width assignment on the details region — the rail, the toolbar and the status bar never moved, the workspace re-filled by Dock");
            this.statusBar.ShowStatus("ready", StatusKind.Normal);
        }

        #endregion

        #region Failure paths: wrong dock order, Dock+Anchor fight, MinimumSize violated

        /// <summary>
        /// Moves the workspace to the end of the child order so it is docked FIRST. Fill takes the
        /// whole page; the toolbar, rail, details and status bar are then laid out over it. The
        /// status bar hides the last grid row: the lesson's picture, made with the same five panels.
        /// </summary>
        private void btnSwapOrder_Click(object sender, EventArgs e)
        {
            this.Controls.SetChildIndex(this.workspacePanel, this.Controls.Count - 1);
            _compositionState = "dock order swapped";
            this.lblProgress.Text = "Workspace docked first — the other regions cover it";

            AddTrace($"✕ wrong dock order: workspace moved to child index {this.Controls.GetChildIndex(this.workspacePanel)} (docked FIRST) → workspace {this.workspacePanel.Width}×{this.workspacePanel.Height} = the whole page; toolbar covers the metric cards, status bar covers the last grid row, rail and details cover the sides");
            AddTrace("    fix: SendToBack on the workspace (or BringToFront on the edges) — a child-index change, no size touched. Click Restore.");
            VerifyComposition();
            ReportLayout("Swap dock order");
            this.statusBar.ShowStatus("dock order wrong", StatusKind.Error);
        }

        /// <summary>
        /// Puts an Anchor on a docked control and logs what the framework does with the two
        /// properties, so the smell "Dock and Anchor fighting on one control" is observed, not assumed.
        /// </summary>
        private void btnAnchorFight_Click(object sender, EventArgs e)
        {
            var dockBefore = this.detailsPanel.Dock;
            var anchorBefore = this.detailsPanel.Anchor;

            this.detailsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;

            var dockAfter = this.detailsPanel.Dock;
            var anchorAfter = this.detailsPanel.Anchor;
            _compositionState = "Dock+Anchor fight";
            this.lblProgress.Text = $"details: Dock={dockAfter} Anchor={anchorAfter}";

            AddTrace($"✕ Dock+Anchor fight: details region had Dock={dockBefore} Anchor={anchorBefore}; after Anchor = Top|Bottom|Right it reports Dock={dockAfter} Anchor={anchorAfter}");
            AddTrace(dockAfter == DockStyle.None
                ? "    the framework dropped Dock when Anchor was set: the details region now floats at its last bounds, the workspace (Fill) extends underneath it — resize the browser and watch them overlap"
                : "    Dock survived: while Dock is set the Anchor is stored but the dock engine decides the bounds — the two settings never both apply, which is why the smell list bans the combination");
            VerifyComposition();
            ReportLayout("Dock+Anchor fight");
            this.statusBar.ShowStatus("Dock and Anchor on one control", StatusKind.Error);
        }

        /// <summary>
        /// Raises the workspace MinimumSize above the width that is left for it — the same situation
        /// as a browser narrower than rail + details + minimum. The guard rail holds: the fill
        /// region keeps its minimum, overflows under the details region, and the page scrolls.
        /// </summary>
        private void btnSqueeze_Click(object sender, EventArgs e)
        {
            int remaining = RemainingWidthForWorkspace();
            int minimum = remaining + 240;
            this.workspacePanel.MinimumSize = new System.Drawing.Size(minimum, WorkspaceMinimum.Height);
            _compositionState = "MinimumSize violated";
            this.lblProgress.Text = $"workspace MinimumSize {minimum} > {remaining} px available";

            int equivalent = this.Padding.Horizontal + this.navigationPanel.Width + this.detailsPanel.Width + WorkspaceMinimum.Width;
            AddTrace($"✕ MinimumSize violated: workspace MinimumSize set to {minimum}×{WorkspaceMinimum.Height} but only {remaining} px are left → workspace reports {this.workspacePanel.Width}×{this.workspacePanel.Height}; it cannot shrink, so it runs under the details region and the page (AutoScroll) shows a horizontal scrollbar");
            AddTrace($"    this is what the real minimum (320) does when the browser is narrower than {equivalent} px — decide on purpose whether the page may scroll or a region must move (Module 6)");
            VerifyComposition();
            ReportLayout("Squeeze workspace");
            this.statusBar.ShowStatus("MinimumSize violated", StatusKind.Error);
        }

        #endregion

        #region Recovery: Restore

        private void btnRestore_Click(object sender, EventArgs e)
        {
            _repository.Reset();
            this.workspace.HideBanner();
            AddTrace("• server repository reset to the seed tickets; banner cleared");
            ComposeShell("Restore");
            LoadTickets("Restore");
            this.statusBar.ShowStatus("ready", StatusKind.Normal);
        }

        #endregion

        #region Tickets: load, select, save (the DetailsEditor's Saved event)

        /// <summary>
        /// Re-reads everything from the repository: the metric cards, the grid and, when a ticket is
        /// selected, the details editor. The server is the source of truth.
        /// </summary>
        private void LoadTickets(string reason)
        {
            var today = DateTime.Today;
            var tickets = _repository.GetAll();

            this.workspace.SetMetrics(
                _repository.CountOpen(),
                _repository.CountOverdue(today),
                _repository.CountAssignedToMe(),
                _repository.CountClosedThisWeek(today));

            string selected = this.workspace.ShowTickets(tickets, _selectedId);
            AddTrace($"→ render {reason}: {tickets.Count} tickets · open {_repository.CountOpen()} · overdue {_repository.CountOverdue(today)} · mine {_repository.CountAssignedToMe()} · closed this week {_repository.CountClosedThisWeek(today)}");

            ShowTicket(selected, reason);
        }

        private void workspace_SelectionChanged(object sender, EventArgs e)
        {
            ShowTicket(this.workspace.SelectedTicketId, "grid selection");
        }

        /// <summary>Hands the repository copy of the ticket to the editor through its one property.</summary>
        private void ShowTicket(string id, string reason)
        {
            var t = id == null ? null : _repository.Get(id);
            _selectedId = t?.Id;
            this.detailsEditor.Ticket = t;

            if (t != null)
                AddTrace($"← client {reason}: ticket {t.Id} \"{t.Title}\" → DetailsEditor.Ticket (the shell never touches a field)");
        }

        /// <summary>
        /// The only write path. TicketRepository.Save validates on the server and throws
        /// TicketValidationException; any other exception is caught too. Both become a message in
        /// the editor (through e.Error), the banner and the status bar — never a broken layout.
        /// </summary>
        private void detailsEditor_Saved(object sender, TicketSaveEventArgs e)
        {
            try
            {
                var saved = _repository.Save(e.Ticket);

                this.workspace.HideBanner();
                AddTrace($"• server saved {saved.Id} \"{saved.Title}\" ({saved.Priority}, {saved.Status}, {saved.Owner}, due {saved.DueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)})");
                LoadTickets("Save");
                this.statusBar.ShowStatus($"saved {saved.Id}", StatusKind.Normal);
                AlertBox.Show($"{saved.Id} saved.", MessageBoxIcon.Information,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            }
            catch (TicketValidationException ex)
            {
                // Failure path: the server refused the ticket. Nothing was written.
                e.Error = ex.Message;
                this.workspace.ShowBanner($"✖ Rejected on the server: {ex.Message} Nothing was written.");
                AddTrace($"• server rejected {e.Ticket.Id}: {ex.Message}");
                this.statusBar.ShowStatus("validation error", StatusKind.Error);
                AlertBox.Show(ex.Message, MessageBoxIcon.Warning,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            }
            catch (Exception ex)
            {
                // Lab step 8: an exception becomes a visible message rather than a broken layout.
                e.Error = "Server error: " + ex.Message;
                this.workspace.ShowBanner($"✖ Server error while saving {e.Ticket.Id}: {ex.Message}");
                AddTrace($"• server threw {ex.GetType().Name} while saving {e.Ticket.Id}: {ex.Message} — caught around DetailsEditor.Saved");
                this.statusBar.ShowStatus("server error", StatusKind.Error);
                AlertBox.Show(ex.Message, MessageBoxIcon.Error,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            }
        }

        private void detailsEditor_Cancelled(object sender, EventArgs e)
        {
            AddTrace($"← client cancel: DetailsEditor restored ticket {_selectedId ?? "(none)"} from its own copy");
        }

        #endregion

        #region Navigation rail

        private void navigationRail_SectionChanged(object sender, EventArgs e)
        {
            string section = this.navigationRail.SelectedSection;
            this.workspace.Title = section == "Dashboard" ? "Tickets" : section;
            AddTrace($"← client navigation: NavigationRail.SelectedSection = {section} (the rail's buttons are private to the UserControl)");
        }

        #endregion

        #region UI helpers

        /// <summary>Appends one line to the "Layout &amp; theme · live trace" card (owned by the Workspace region).</summary>
        private void AddTrace(string line)
        {
            this.workspace.AddTrace(line);
        }

        private void btnClearTrace_Click(object sender, EventArgs e)
        {
            this.workspace.ClearTrace();
        }

        #endregion
    }
}
