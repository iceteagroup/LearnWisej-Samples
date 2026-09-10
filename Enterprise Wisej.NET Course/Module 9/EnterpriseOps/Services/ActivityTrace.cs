using System;
using System.Globalization;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The live activity trace every layer writes to (per session — one instance per page, never static).
    /// The page subscribes to EntryAdded and appends the line to lstTrace; services never touch a control.
    ///
    /// Module 9 adds two layers to the usual set, because the interesting thing here is the *boundary*:
    ///   Client:  something the browser reported (a widget event, a capability report, a palette keystroke)
    ///   Interop: the contract layer — payload parsing, validation, the WebMethod itself
    /// Reading the trace top to bottom shows the crossing: Client → Interop → Security → Service → Data.
    /// </summary>
    public sealed class ActivityTrace
    {
        public event Action<string> EntryAdded;

        public void Ui(string message) => Add("UI →      ", message);
        public void UiResult(string message) => Add("UI ←      ", message);
        public void Client(string message) => Add("Client:   ", message);      // the browser said this — untrusted
        public void Interop(string message) => Add("Interop:  ", message);     // contract validation / WebMethod
        public void Security(string message) => Add("Security: ", message);    // PermissionService decision
        public void Service(string message) => Add("Service:  ", message);     // the business rule that ran
        public void Data(string message) => Add("Data:     ", message);        // the repository
        public void Audit(string message) => Add("Audit:    ", message);       // the append-only record

        /// <summary>A null entry tells the page to clear the list.</summary>
        public void Clear() => EntryAdded?.Invoke(null);

        private void Add(string layer, string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            EntryAdded?.Invoke($"{time}  {layer} {message}");
        }
    }
}
