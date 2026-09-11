using System;
using System.ComponentModel;

namespace EnterpriseOps.Widgets
{
    /// <summary>
    /// One slice of the work-order chart, in the wrapper's own vocabulary: a stable <see cref="Key"/> the
    /// server understands, a <see cref="Label"/> the user reads, a <see cref="Value"/>.
    ///
    /// <para>
    /// This class is the answer to the lab's second review question — <b>what data crosses from server to
    /// client</b>. Three fields per slice and nothing else: no <c>WorkOrder</c>, no tenant id, no version,
    /// no user name, no correlation id. A wrapper that sent the entity to the chart would have leaked the
    /// domain into the browser, where anybody can read it.
    /// </para>
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class ChartSegment
    {
        public ChartSegment()
        {
        }

        public ChartSegment(string key, string label, int value)
        {
            this.Key = key;
            this.Label = label;
            this.Value = value;
        }

        /// <summary>
        /// The stable identifier of the slice. It is the only thing that comes back when the user clicks,
        /// and the server re-validates it before using it — the browser chose it.
        /// </summary>
        [Category("Chart"), Description("The stable identifier of the slice; the only value that comes back on a click.")]
        public string Key { get; set; }

        /// <summary>The text the user sees on and under the slice.</summary>
        [Category("Chart"), Description("The text the user sees on and under the slice.")]
        public string Label { get; set; }

        /// <summary>How big the slice is. Must be zero or greater.</summary>
        [Category("Chart"), DefaultValue(0), Description("How big the slice is. Must be zero or greater.")]
        public int Value { get; set; }

        public override string ToString() => $"{this.Key}: {this.Label} = {this.Value}";
    }

    /// <summary>The named server event a click on a slice becomes.</summary>
    public class ChartSegmentEventArgs : EventArgs
    {
        public ChartSegmentEventArgs(string key, string label, int value, int percent)
        {
            this.Key = key;
            this.Label = label;
            this.Value = value;
            this.Percent = percent;
        }

        /// <summary>The key the browser sent. Treat it as untrusted input: validate before you query with it.</summary>
        public string Key { get; }

        /// <summary>The label the browser rendered (diagnostics only — the server already knows the label).</summary>
        public string Label { get; }

        /// <summary>The value the browser rendered.</summary>
        public int Value { get; }

        /// <summary>The percentage the browser rendered, 0..100.</summary>
        public int Percent { get; }
    }

    /// <summary>
    /// A failure the client adapter caught and reported through the contract's <c>error</c> event, instead
    /// of throwing it at the screen. <see cref="Phase"/> says where: <c>init</c>, <c>update</c>,
    /// <c>render</c>, <c>resize</c> or <c>select</c>.
    /// </summary>
    public class WidgetErrorEventArgs : EventArgs
    {
        public WidgetErrorEventArgs(string phase, string message)
        {
            this.Phase = phase ?? "unknown";
            this.Message = message ?? "";
        }

        /// <summary>Where it failed: init · update · render · resize · select.</summary>
        public string Phase { get; }

        /// <summary>The vendor's message, verbatim. Log it; do not put it in front of the user unchanged.</summary>
        public string Message { get; }

        /// <summary>True when the client is now showing the component's embedded fallback instead of the chart.</summary>
        public bool FallbackRendered { get; internal set; }

        public override string ToString() => $"{this.Phase}: {this.Message}";
    }
}
