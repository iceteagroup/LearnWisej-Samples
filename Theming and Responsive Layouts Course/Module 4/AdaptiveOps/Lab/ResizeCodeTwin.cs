using System;
using System.Drawing;
using Wisej.Web;

namespace AdaptiveOps.Lab
{
    /// <summary>Argument of <see cref="ResizeCodeTwin.Traced"/>: one line for the live trace.</summary>
    public sealed class TraceEventArgs : EventArgs
    {
        public TraceEventArgs(string line)
        {
            Line = line;
        }

        public string Line { get; }
    }

    /// <summary>
    /// The "before" of the module, kept alive on purpose in the second workspace tab: a miniature
    /// shell whose five panels have no Dock and no Anchor. A <c>Resize</c> handler recomputes their
    /// <c>Bounds</c> on every browser resize — six assignments, one server round trip each time,
    /// and every one of them applied AFTER the browser has already painted the old layout, which is
    /// the lag and flicker the learner can see next to the docked shell that needs no code at all.
    ///
    /// The numbers were "tested" at a width of about 720 px: the workspace keeps a fixed width of
    /// 380 px while the details panel is pinned to the right edge by hand, so below that width the
    /// details panel slides over the ticket grid, and the Save button at a fixed y = 180 drops below
    /// the panel when the browser is short. Compare docs/LayoutComparison.md.
    /// </summary>
    public partial class ResizeCodeTwin : UserControl
    {
        // The developer's tested geometry. Every one of these is a rule the layout engine could apply for us.
        private const int RailWidth = 120;
        private const int WorkspaceWidth = 380;   // fixed! this is what makes the details panel overlap the grid
        private const int DetailsWidth = 220;
        private const int ToolbarHeight = 36;
        private const int StatusHeight = 24;
        private const int Gap = 4;

        private int _resizeCount;

        public ResizeCodeTwin()
        {
            InitializeComponent();
        }

        /// <summary>Raised on every Resize with a line describing what the handler did.</summary>
        public event EventHandler<TraceEventArgs> Traced;

        /// <summary>How many times the handler has run since the page loaded.</summary>
        public int ResizeCount => _resizeCount;

        /// <summary>
        /// ✕ The smell. Six Bounds lines that run on the server after each client resize.
        /// The docked shell in MainPage has zero lines for the same job.
        /// </summary>
        private void ResizeCodeTwin_Resize(object sender, EventArgs e)
        {
            int w = this.ClientSize.Width;
            int h = this.ClientSize.Height;
            int top = ToolbarHeight + Gap;
            int middle = Math.Max(0, h - ToolbarHeight - StatusHeight - 2 * Gap);

            this.twinToolbar.Bounds = new Rectangle(0, 0, w, ToolbarHeight);
            this.twinStatus.Bounds = new Rectangle(0, h - StatusHeight, w, StatusHeight);
            this.twinRail.Bounds = new Rectangle(0, top, RailWidth, middle);
            this.twinWorkspace.Bounds = new Rectangle(RailWidth + Gap, top, WorkspaceWidth, middle);
            this.twinDetails.Bounds = new Rectangle(w - DetailsWidth, top, DetailsWidth, middle);
            // btnTwinSave keeps its designer Location (12, 180): nothing moves it when the panel gets shorter.

            _resizeCount++;

            int overlap = (RailWidth + Gap + WorkspaceWidth) - (w - DetailsWidth);
            bool saveClipped = this.btnTwinSave.Bottom > middle;

            string verdict = overlap > 0
                ? $"details overlaps the grid by {overlap} px"
                : "no overlap at this width";
            if (saveClipped)
                verdict += " · Save button clipped";

            this.lblTwinCounter.Text = $"✕ Resize #{_resizeCount} · 6 Bounds lines · {w}×{h} · {verdict}";

            Traced?.Invoke(this, new TraceEventArgs(
                $"✕ twin Resize #{_resizeCount}: {w}×{h} → 6 Bounds assignments on the server after the client painted · {verdict}"));
        }
    }
}
