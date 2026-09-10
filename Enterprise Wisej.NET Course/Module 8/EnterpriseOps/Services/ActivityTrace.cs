using System;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The one place every layer writes to. The page subscribes to <see cref="EntryAdded"/> and renders the
    /// lines in <c>lstTrace</c> — that is how the reviewer sees which layer took each decision without a
    /// debugger. One instance per session (created by the page); never a static.
    ///
    /// The prefixes the samples of this course use: <c>UI →</c>, <c>Component:</c>, <c>Client →</c>,
    /// <c>Service:</c>, <c>Data:</c>, <c>Security:</c>, <c>Package:</c>.
    /// </summary>
    public class ActivityTrace : IActivityTrace
    {
        /// <summary>Raised for every line, on the request thread that produced it.</summary>
        public event EventHandler<string> EntryAdded;

        /// <summary>Writes a raw line (the <see cref="IActivityTrace"/> contract used by the services).</summary>
        public void Write(string line)
        {
            EntryAdded?.Invoke(this, $"{DateTime.Now:HH:mm:ss.fff}  {line}");
        }

        /// <summary>A user action on the screen.</summary>
        public void Ui(string line) => Write("UI → " + line);

        /// <summary>A decision taken inside a reusable component (StatusTimeline, WorkOrderChartWidget).</summary>
        public void Component(string line) => Write("Component: " + line);

        /// <summary>Something the browser told the server through the widget's event contract.</summary>
        public void Client(string line) => Write("Client → " + line);

        /// <summary>Resource packaging: what was registered, in which order, from where.</summary>
        public void Package(string line) => Write("Package: " + line);
    }
}
