using OrderDesk.Shared;
using Wisej.Web;

namespace OrderDesk.Pages
{
    /// <summary>
    /// Base of the three navigation pages (Orders / Customers / Reports). Each WinForms Form became
    /// a Panel hosted by the MainPage shell: created once, shown by toggling Visible, never
    /// recreated on navigation. The TracePanel is a lab prop injected by the shell.
    /// </summary>
    public class ModulePage : Panel
    {
        /// <summary>The migration trace of the hosting MainPage (lab prop; may be null in the designer).</summary>
        public TracePanel Trace { get; set; }

        /// <summary>The caption shown in the nav, the View menu and the status bar.</summary>
        public virtual string Title => this.Name;

        protected void Log(TraceKind kind, string name, string payload = "")
        {
            if (Trace == null || Trace.IsDisposed) return;
            Trace.Add(kind, name, payload);
        }
    }
}
