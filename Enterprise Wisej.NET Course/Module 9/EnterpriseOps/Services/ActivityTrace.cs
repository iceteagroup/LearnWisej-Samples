using System;
using System.Globalization;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// Server-side diagnostic log shared by every layer (per session — one instance per page, never static).
    /// Each line goes to <see cref="System.Diagnostics.Trace"/> as <c>HH:mm:ss.fff  Layer: message</c>.
    ///
    /// Besides the usual layers there are two for the browser boundary:
    ///   Client:  something the browser reported (a WebMethod call, a capability report, a widget error)
    ///   Interop: the contract layer — payload parsing, validation, the WebMethod itself
    /// </summary>
    public sealed class ActivityTrace
    {
        public void Client(string message) => Add("Client:   ", message);      // the browser said this — untrusted
        public void Interop(string message) => Add("Interop:  ", message);     // contract validation / WebMethod
        public void Security(string message) => Add("Security: ", message);    // PermissionService decision
        public void Service(string message) => Add("Service:  ", message);     // the business rule that ran
        public void Data(string message) => Add("Data:     ", message);        // the repository
        public void Audit(string message) => Add("Audit:    ", message);       // the append-only record

        private static void Add(string layer, string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            System.Diagnostics.Trace.WriteLine($"{time}  {layer} {message}");
        }
    }
}
