using System;
using System.Globalization;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// Server-side diagnostic log. Services write their decisions here with a layer prefix
    /// (<c>Service:</c>, <c>Security:</c>, <c>Queue:</c>); every line goes to
    /// <see cref="System.Diagnostics.Trace"/> with a timestamp.
    /// </summary>
    public sealed class ActivityTrace
    {
        public void Add(string message)
        {
            string line = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture) + "  " + message;
            System.Diagnostics.Trace.WriteLine(line);
        }

        public void Error(Exception ex, string correlationId) =>
            Add($"ERROR  {ex.GetType().Name}: {ex.Message} [{correlationId}]");
    }
}
