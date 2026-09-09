using System;
using System.Globalization;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The one trace every layer writes to ("UI →", "Service:", "Data:", "Security:", "Integrations:").
    /// The page subscribes to Written and appends to lstTrace — services never touch a control.
    /// Per session (created in the page constructor), never static.
    /// </summary>
    public class ActivityTrace
    {
        public event Action<string> Written;

        public void Write(string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            Written?.Invoke($"{time}  {message}");
        }
    }
}
