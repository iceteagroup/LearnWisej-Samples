using System;
using System.Collections.Generic;
using System.Globalization;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The one door every layer logs through. Services, stores and security rules only say which layer
    /// decided and what; the trace stamps the time, keeps the line in a per-session buffer and hands it to
    /// whichever screen is attached (its <c>lstTrace</c>). The buffer is what lets the trace survive the
    /// navigation MigrationDossierPage ⇄ WorkOrdersPage — flow 9 walks exactly that path.
    /// The reviewer reads the right-hand card to prove the handler was thin and the service decided.
    /// </summary>
    public sealed class ActivityTrace
    {
        private readonly List<string> _lines = new List<string>();
        private Action<string> _sink;   // null while no screen is attached; lines still buffer

        /// <summary>Every line written in this session, oldest first.</summary>
        public IReadOnlyList<string> Lines => _lines;

        /// <summary>A screen attaches its list box: it receives the buffer first, then every new line.</summary>
        public void Attach(Action<string> sink)
        {
            _sink = sink;
            if (sink == null)
                return;
            foreach (var line in _lines)
                sink(line);
        }

        public void Detach() => _sink = null;

        public void Clear() => _lines.Clear();

        public void Ui(string message) => Write("UI →", message);
        public void Service(string message) => Write("Service:", message);
        public void Data(string message) => Write("Data:", message);
        public void Security(string message) => Write("Security:", message);
        public void Job(string message) => Write("Job:", message);

        private void Write(string layer, string message)
        {
            string line = $"{DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)}  {layer,-9} {message}";
            _lines.Add(line);
            _sink?.Invoke(line);
        }
    }
}
