using System;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The server-side log every layer writes to. Each line goes to <see cref="System.Diagnostics.Trace"/>
    /// with a timestamp and the layer prefix (<c>Service:</c>, <c>Data:</c>, <c>Security:</c>,
    /// <c>Component:</c>). One instance per session (created by the page); never a static.
    /// </summary>
    public class ActivityTrace : IActivityTrace
    {
        /// <summary>Writes a raw line (the <see cref="IActivityTrace"/> contract used by the services).</summary>
        public void Write(string line)
        {
            System.Diagnostics.Trace.WriteLine($"{DateTime.Now:HH:mm:ss.fff}  {line}");
        }

        /// <summary>Something a reusable component reported (StatusTimeline, WorkOrderChartWidget).</summary>
        public void Component(string line) => Write("Component: " + line);
    }
}
