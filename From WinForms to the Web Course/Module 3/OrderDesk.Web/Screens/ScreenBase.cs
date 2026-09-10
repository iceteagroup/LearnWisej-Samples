using System;
using OrderDesk.Views;
using Wisej.Web;

namespace OrderDesk.Screens
{
    /// <summary>
    /// What every screen hosted by the shell shares: a name, a status line for the StatusBar, the
    /// <see cref="Trace"/> event (so the lab console can log what the migrated code does without the
    /// screen knowing about the TracePanel) and <see cref="OnShown"/>, called by the shell each time
    /// the screen becomes the current one.
    /// </summary>
    public class ScreenBase : UserControl
    {
        public event EventHandler<TraceEventArgs> Trace;
        public event EventHandler StatusChanged;

        public virtual string ScreenName => Name;

        /// <summary>One line for the shell's StatusBar (was ToolStripStatusLabel.Text).</summary>
        public virtual string StatusText => "";

        /// <summary>Called by the shell after the screen became visible; <paramref name="first"/> on creation.</summary>
        public virtual void OnShown(bool first) { }

        protected void RaiseTrace(TraceKind kind, string name, string payload = "") =>
            Trace?.Invoke(this, new TraceEventArgs(kind, name, payload));

        protected void RaiseStatusChanged() => StatusChanged?.Invoke(this, EventArgs.Empty);
    }
}
