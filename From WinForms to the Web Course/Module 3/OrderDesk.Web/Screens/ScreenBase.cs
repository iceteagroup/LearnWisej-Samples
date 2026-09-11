using System;
using Wisej.Web;

namespace OrderDesk.Screens
{
    /// <summary>
    /// What every screen hosted by the shell shares: a name, a status line for the StatusBar and
    /// <see cref="OnShown"/>, called by the shell each time the screen becomes the current one.
    /// </summary>
    public class ScreenBase : UserControl
    {
        public event EventHandler StatusChanged;

        public virtual string ScreenName => Name;

        /// <summary>One line for the shell's StatusBar (was ToolStripStatusLabel.Text).</summary>
        public virtual string StatusText => "";

        /// <summary>Called by the shell after the screen became visible; <paramref name="first"/> on creation.</summary>
        public virtual void OnShown(bool first) { }

        protected void RaiseStatusChanged() => StatusChanged?.Invoke(this, EventArgs.Empty);
    }
}
