using System;
using System.Globalization;

namespace EnterpriseOps.Diagnostics
{
    /// <summary>
    /// The live activity trace: every layer reports the decision it took, tagged with the layer name, so a
    /// reviewer can prove from the screen alone that the handler was thin and the service decided.
    ///
    /// One instance per session (created by the page, shared with every service). The page subscribes to
    /// <see cref="EntryAdded"/> and appends each line to lstTrace; nothing in this class knows about controls.
    /// </summary>
    public sealed class ActivityTrace
    {
        public event Action<string> EntryAdded;

        public void Ui(string message) => Add("UI →         ", message);
        public void UiResult(string message) => Add("UI ←         ", message);
        public void Service(string message) => Add("Service:     ", message);
        public void Data(string message) => Add("Data:        ", message);
        public void Security(string message) => Add("Security:    ", message);
        public void Integration(string message) => Add("Integration: ", message);
        public void Diagnostics(string message) => Add("Diagnostics: ", message);
        public void Architecture(string message) => Add("Architecture:", message);

        public void Clear() => EntryAdded?.Invoke(null);

        private void Add(string layer, string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            EntryAdded?.Invoke($"{time}  {layer} {message}");
        }
    }
}
