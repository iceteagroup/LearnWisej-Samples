using System;
using System.Globalization;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// Server-side diagnostic log shared by every layer ("Service:", "Data:", "Security:", "Integrations:").
    /// Each line is timestamped and written to System.Diagnostics.Trace. Per session, never static.
    /// </summary>
    public class ActivityTrace
    {
        public void Write(string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            System.Diagnostics.Trace.WriteLine($"{time}  {message}");
        }
    }
}
