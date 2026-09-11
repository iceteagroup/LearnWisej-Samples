using System;
using System.Globalization;

namespace EnterpriseOps.Diagnostics
{
    /// <summary>
    /// Server-side activity log: every layer records the decision it took, tagged with the layer name, and each
    /// line is written to <see cref="System.Diagnostics.Trace"/>. One instance per session, shared with every service.
    /// </summary>
    public sealed class ActivityTrace
    {
        public void Service(string message) => Add("Service:     ", message);
        public void Data(string message) => Add("Data:        ", message);
        public void Security(string message) => Add("Security:    ", message);
        public void Integration(string message) => Add("Integration: ", message);
        public void Diagnostics(string message) => Add("Diagnostics: ", message);

        private static void Add(string layer, string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            System.Diagnostics.Trace.WriteLine($"{time}  {layer} {message}");
        }
    }
}
