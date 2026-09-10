using System;
using System.Globalization;

namespace EnterpriseOps.Diagnostics
{
    /// <summary>
    /// The live activity trace: every layer reports the decision it took, tagged with the layer name, so a
    /// reviewer can prove from the screen alone that the handler was thin and the service decided.
    ///
    /// One instance per session — created by the page, handed to every service it builds. It is deliberately
    /// NOT static: two sessions must never write into each other's trace. Nothing in this class knows about
    /// controls; the page subscribes to <see cref="EntryAdded"/> and appends each line to lstTrace.
    /// </summary>
    public sealed class ActivityTrace : IActivityTrace
    {
        /// <summary>Raised for every line. A null argument means "clear".</summary>
        public event Action<string> EntryAdded;

        public void Ui(string message) => Emit("UI →", message);
        public void UiResult(string message) => Emit("UI ←", message);
        public void Session(string message) => Emit("Session:", message);
        public void Service(string message) => Emit("Service:", message);
        public void Data(string message) => Emit("Data:", message);
        public void Security(string message) => Emit("Security:", message);
        public void Audit(string message) => Emit("Audit:", message);
        public void Job(string message) => Emit("Job:", message);

        /// <summary>
        /// <see cref="IActivityTrace"/> — the sink the shared services (TenantGuard …) write to. They put the
        /// layer at the front of the message themselves; this splits it back off so every line still lines up.
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

        public void Clear() => EntryAdded?.Invoke(null);

        private void Emit(string layer, string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            EntryAdded?.Invoke($"{time}  {layer.PadRight(10)} {message}");
        }
    }
}
