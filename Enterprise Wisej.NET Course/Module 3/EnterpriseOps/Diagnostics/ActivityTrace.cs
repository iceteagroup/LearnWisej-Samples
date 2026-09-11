using System;
using System.Globalization;

namespace EnterpriseOps.Diagnostics
{
    /// <summary>
    /// Server-side diagnostic log. Each layer writes the decision it took, tagged with the layer name and a
    /// timestamp, to <see cref="System.Diagnostics.Trace"/>. One instance per session, handed to every
    /// service the session builds — never static.
    /// </summary>
    public sealed class ActivityTrace : IActivityTrace
    {
        public void Session(string message) => Emit("Session:", message);
        public void Service(string message) => Emit("Service:", message);
        public void Data(string message) => Emit("Data:", message);
        public void Audit(string message) => Emit("Audit:", message);

        /// <summary>
        /// <see cref="IActivityTrace"/> — used by services that put the layer at the front of the message
        /// themselves (TenantGuard, ErrorLog); the layer is split back off so every line lines up.
        /// </summary>
        public void Write(string message)
        {
            if (message == null)
                return;

            int colon = message.IndexOf(": ", StringComparison.Ordinal);
            if (colon > 0 && colon <= 12)
                Emit(message.Substring(0, colon + 1), message.Substring(colon + 2));
            else
                Emit(string.Empty, message);
        }

        private static void Emit(string layer, string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            System.Diagnostics.Trace.WriteLine($"{time}  {layer.PadRight(10)} {message}");
        }
    }
}
