using System;
using System.Globalization;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The server-side diagnostic log the services write to. Every line carries a timestamp and the layer that
    /// wrote it (<c>Service:</c>, <c>Data:</c>, <c>Security:</c>, <c>Job:</c>, <c>Error:</c>) and goes to
    /// <see cref="System.Diagnostics.Trace"/>, so any configured trace listener (debugger output, console, file) sees it.
    /// </summary>
    public sealed class ActivityTrace
    {
        public void Write(string message)
        {
            string line = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture) + "  " + message;
            System.Diagnostics.Trace.WriteLine(line);
        }
    }
}
