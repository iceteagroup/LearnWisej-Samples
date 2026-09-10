using System;
using TicketOps.Infrastructure;
using Wisej.Core;
using Wisej.Web;

namespace TicketOps.Controls
{
    /// <summary>Raised on the server when the browser reports a keyboard shortcut it handled.</summary>
    public sealed class ShortcutEventArgs : EventArgs
    {
        public string Shortcut { get; }

        public ShortcutEventArgs(string shortcut)
        {
            Shortcut = shortcut;
        }
    }

    /// <summary>
    /// The global search box: a TextBox that also owns the server end of the Ctrl+K interop point.
    ///
    /// Client side: the Wisej.NET JavaScript extender (see WorkOrdersView.Designer.cs) runs
    /// <c>ticketOps.attachSearchShortcuts(this)</c> when this control's widget is created — after the
    /// widget exists, never at page load — and the script focuses the box on Ctrl+K in the browser.
    ///
    /// Server side: the script then calls <see cref="ReportShortcut"/>, a [WebMethod] registered on this
    /// control (OnWebRender → RegisterWebMethods), so the server learns what happened through a normal
    /// callback. The argument is client input: it is validated against a fixed list before anything is
    /// logged or raised. Nothing is authorized here — a focus change has no business meaning — which is
    /// exactly why this round trip is allowed to be "tell", not "ask".
    /// </summary>
    public class GlobalSearchBox : TextBox
    {
        // A constant whitelist, not per-user state: fine as static.
        private static readonly string[] KnownShortcuts = { "ctrl+k" };
        private const int MaxShortcutLength = 16;

        /// <summary>Set by the hosting screen so the callback can leave a [CLIENT] line in the trace.</summary>
        public ILog Log { get; set; }

        public event EventHandler<ShortcutEventArgs> ShortcutPressed;

        public GlobalSearchBox()
        {
            this.Watermark = "Search work orders…";
        }

        // ==== UNVERIFIED at runtime (documented in the Wisej.Framework XML: WebMethodAttribute remarks) ====
        // Wisej.NET registers [WebMethod]s of top-level containers automatically; a child control opts in by
        // adding them to its own render config. The client then gets ReportShortcut(args, callback) and
        // ReportShortcutAsync(args) → Promise on this widget (verified for a Widget subclass in the
        // Application Integration Course, Module 8).
        protected override void OnWebRender(dynamic config)
        {
            base.OnWebRender((object)config);
            RegisterWebMethods(config);
        }

        /// <summary>
        /// Server callback handler for the Ctrl+K shortcut. Called from the browser; returns true when the
        /// report was accepted. Anything unexpected is refused without echoing it back.
        /// </summary>
        [WebMethod]
        public bool ReportShortcut(string shortcut)
        {
            string name = (shortcut ?? "").Trim().ToLowerInvariant();
            if (name.Length == 0 || name.Length > MaxShortcutLength || Array.IndexOf(KnownShortcuts, name) < 0)
            {
                // Client input we did not expect: log the fact, never the payload.
                Log?.Warn(LogLayer.Client, "GlobalSearchBox.ReportShortcut", $"refused an unknown shortcut report ({name.Length} chars) — client input is not trusted");
                return false;
            }

            Log?.Info(LogLayer.Client, "GlobalSearchBox.ReportShortcut", "Ctrl+K → focus search (focused in the browser; the server is told, not asked)");
            ShortcutPressed?.Invoke(this, new ShortcutEventArgs(name));
            return true;
        }
    }
}
