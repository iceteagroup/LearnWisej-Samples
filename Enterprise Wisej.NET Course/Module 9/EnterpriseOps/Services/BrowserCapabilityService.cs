using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EnterpriseOps.Services
{
    /// <summary>One detected browser capability, as the server chose to record it.</summary>
    public sealed class CapabilityReading
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public bool Available { get; set; }
        public string Detail { get; set; }

        /// <summary>What the SERVER decides to do about a missing capability. Never a permission.</summary>
        public string Fallback { get; set; }

        public override string ToString()
        {
            string mark = Available ? "✓" : "✕";
            string tail = Available
                ? (string.IsNullOrEmpty(Detail) ? "" : "  " + Detail)
                : "  " + (string.IsNullOrEmpty(Fallback) ? "unavailable" : Fallback);
            return $"{mark} {Title,-22}{tail}";
        }
    }

    /// <summary>
    /// Turns a capability report from the browser into server-side facts and UX fallbacks.
    ///
    /// The rule of the module in one class: feature detection buys the user a better SCREEN, never a
    /// bigger permission. Nothing here is ever consulted by ClientCommandService — grep for it and
    /// you will find no call, which is the point a reviewer should check.
    ///
    /// Validation: the server owns the key list. Keys it does not know are counted and dropped,
    /// which is what happens to a forged report that tries to smuggle "canApprove": true.
    /// </summary>
    public sealed class BrowserCapabilityService
    {
        private readonly ActivityTrace _trace;
        private readonly List<CapabilityReading> _readings = new List<CapabilityReading>();

        /// <summary>Key → (display title, what the server does when it is missing).</summary>
        private static readonly Dictionary<string, string[]> Known = new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            ["clipboard"] = new[] { "Clipboard", "copy buttons hidden, text stays selectable" },
            ["keyboardShortcuts"] = new[] { "Keyboard shortcuts", "palette reachable from the toolbar button" },
            ["camera"] = new[] { "Camera", "manual entry offered" },
            ["barcodeScanner"] = new[] { "Barcode scanner", "asset code typed by hand" },
            ["notifications"] = new[] { "Notifications", "in-page banner instead of a toast" },
            ["touch"] = new[] { "Touch input", "pointer layout kept" },
            ["online"] = new[] { "Network online", "queue actions until the socket is back" },
            ["localStorage"] = new[] { "Local storage", "draft kept in the session on the server" },
        };

        /// <summary>Descriptive readings (not booleans): shown as-is, still bounded and sanitised.</summary>
        private static readonly Dictionary<string, string> KnownFacts = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["viewport"] = "Viewport",
            ["language"] = "Language",
            ["timeZone"] = "Time zone",
        };

        public BrowserCapabilityService(ActivityTrace trace)
        {
            _trace = trace;
        }

        public IReadOnlyList<CapabilityReading> Readings => _readings;
        public DateTime? CollectedUtc { get; private set; }
        public int IgnoredKeys { get; private set; }
        public int Reports { get; private set; }

        /// <summary>
        /// The report arrives as one compact string of <c>key=value</c> pairs separated by ';'
        /// — small enough to validate in a loop, and impossible to turn into an arbitrary object
        /// graph. Anything the server does not recognise is dropped and counted.
        /// </summary>
        public IReadOnlyList<CapabilityReading> Accept(string report)
        {
            Reports++;
            report = report ?? "";
            if (report.Length > 800)
            {
                _trace.Interop($"capability report is {report.Length} chars (limit 800) — truncated before parsing");
                report = report.Substring(0, 800);
            }
            _trace.Client($"capability report #{Reports} · {report.Length} chars");

            var accepted = new List<CapabilityReading>();
            int ignored = 0;

            foreach (string pair in report.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int split = pair.IndexOf('=');
                if (split <= 0) { ignored++; continue; }

                string key = pair.Substring(0, split).Trim();
                string value = Sanitize(pair.Substring(split + 1).Trim(), 40);

                if (Known.TryGetValue(key, out string[] known))
                {
                    bool available = value == "1" || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
                    accepted.Add(new CapabilityReading
                    {
                        Key = key,
                        Title = known[0],
                        Available = available,
                        Detail = "",
                        Fallback = known[1],
                    });
                }
                else if (KnownFacts.TryGetValue(key, out string title))
                {
                    accepted.Add(new CapabilityReading
                    {
                        Key = key,
                        Title = title,
                        Available = value.Length > 0,
                        Detail = value.Length > 0 ? value : "not reported",
                        Fallback = "server default used",
                    });
                }
                else
                {
                    ignored++;
                    _trace.Interop($"capability key '{Sanitize(key, 24)}' is not in the server's list → ignored (a report cannot invent a capability)");
                }
            }

            IgnoredKeys = ignored;
            CollectedUtc = DateTime.UtcNow;
            _readings.Clear();
            _readings.AddRange(accepted.OrderBy(r => Order(r.Key)));

            int missing = _readings.Count(r => !r.Available);
            _trace.Service($"BrowserCapabilityService: {_readings.Count} readings accepted, {ignored} ignored, {missing} fallback(s) chosen server-side");
            foreach (CapabilityReading reading in _readings.Where(r => !r.Available && Known.ContainsKey(r.Key)))
                _trace.Service($"  fallback · {reading.Title} unavailable → {reading.Fallback}");

            return _readings;
        }

        /// <summary>The one-line summary the status bar shows.</summary>
        public string Summary()
        {
            if (CollectedUtc == null) return "No capability report yet — the widget has not reported.";
            int available = _readings.Count(r => r.Available);
            return $"{available}/{_readings.Count} available · {IgnoredKeys} unknown key(s) ignored · collected {CollectedUtc.Value.ToString("HH:mm:ss", CultureInfo.InvariantCulture)}Z";
        }

        public void Reset()
        {
            _readings.Clear();
            CollectedUtc = null;
            IgnoredKeys = 0;
        }

        private static int Order(string key)
        {
            string[] order = { "viewport", "language", "timeZone", "online", "touch", "keyboardShortcuts", "clipboard", "localStorage", "notifications", "camera", "barcodeScanner" };
            int index = Array.IndexOf(order, key);
            return index < 0 ? order.Length : index;
        }

        /// <summary>Bound the length and drop control characters: the value is printed into a list box.</summary>
        private static string Sanitize(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return "";
            var clean = new string(value.Where(c => !char.IsControl(c) && c != ';' && c != '=').ToArray());
            return clean.Length <= maxLength ? clean : clean.Substring(0, maxLength);
        }
    }
}
