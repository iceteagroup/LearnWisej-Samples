using System;
using System.Globalization;

namespace EnterpriseOps.Diagnostics
{
    /// <summary>
    /// Server-side diagnostic log: every layer writes the decision it took, tagged with the layer name, to
    /// <see cref="System.Diagnostics.Trace"/> as <c>HH:mm:ss.fff  Layer: message</c>.
    ///
    /// One instance per session, created in <c>Program.Main</c> and handed to every service.
    ///
    /// Security note: this is a log. It names users, tenants and permissions — never a password, a token, a claim
    /// value that identifies a person beyond the subject id, or a full record. The hardening checklist has an item
    /// for exactly this.
    /// </summary>
    public sealed class ActivityTrace
    {
        public void Service(string message) => Write("Service:  ", message);
        public void Data(string message) => Write("Data:     ", message);
        public void Security(string message) => Write("Security: ", message);
        public void Identity(string message) => Write("Identity: ", message);
        public void Audit(string message) => Write("Audit:    ", message);

        private static void Write(string layer, string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            System.Diagnostics.Trace.WriteLine($"{time}  {layer} {message}");
        }
    }
}
