using System;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The server-side log: every line (time + layer + message) goes to
    /// <see cref="System.Diagnostics.Trace"/>, so it appears in the debugger's Output window or in any
    /// configured trace listener. One instance per session, created by the page.
    /// </summary>
    public sealed class ActivityTrace : IActivityTrace
    {
        public void Trace(string layer, string message)
        {
            System.Diagnostics.Trace.WriteLine($"{DateTime.Now:HH:mm:ss.fff}  {layer} {message}");
        }
    }
}
