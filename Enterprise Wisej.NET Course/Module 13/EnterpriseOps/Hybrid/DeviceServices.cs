using System;
using System.Threading.Tasks;
using EnterpriseOps.Services;
using Wisej.Web;

namespace EnterpriseOps.Hybrid
{
    /// <summary>The shape the screen lays itself out for. Detected, or forced by the Simulate switch.</summary>
    public enum DeviceProfile { Phone, Tablet, Desktop }

    /// <summary>
    /// What the application knows about the device it is running on, gathered once and passed around as a
    /// value. Screens read this; they never read <c>Application.Browser</c> themselves, so a test can hand
    /// them a phone without a phone.
    /// </summary>
    public class DeviceInfo
    {
        public DeviceProfile Profile { get; set; }

        /// <summary>What the client reported: "Mobile", "Tablet", "Desktop" (Application.Browser.Device).</summary>
        public string ReportedDevice { get; set; }

        public string BrowserType { get; set; }
        public string OperatingSystem { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }

        /// <summary>True when the Simulate switch overrode the detected profile.</summary>
        public bool Simulated { get; set; }

        /// <summary>The width the field card is laid out at for this shape — the device-aware part of the layout.</summary>
        public int FieldWidth
        {
            get
            {
                switch (Profile)
                {
                    case DeviceProfile.Phone: return 380;
                    case DeviceProfile.Tablet: return 620;
                    default: return 860;
                }
            }
        }

        /// <summary>Field mode is used with gloves: touch targets grow as the screen shrinks.</summary>
        public int TouchTargetHeight
        {
            get { return Profile == DeviceProfile.Phone ? 44 : Profile == DeviceProfile.Tablet ? 40 : 34; }
        }

        /// <summary>A phone shows the work order and nothing else; larger shapes can afford context columns.</summary>
        public bool ShowsContextColumns => Profile != DeviceProfile.Phone;

        public override string ToString()
            => $"{Profile}{(Simulated ? " (simulated)" : "")} · reported \"{ReportedDevice}\" · {BrowserType} on {OperatingSystem} · screen {ScreenWidth}×{ScreenHeight}";
    }

    /// <summary>
    /// The one place in the application that touches device/browser APIs. Everything above it — the field
    /// screen, the sync workflow, the tests — sees only <see cref="IDeviceServices"/> plus
    /// <see cref="Describe"/>.
    ///
    /// In a Wisej.NET Hybrid build the three interface methods would reach the native shell (connectivity
    /// callback, camera/barcode scanner, haptics). In this browser sample they are simulated, which is
    /// exactly the point of the abstraction: the screen cannot tell, and a unit test would substitute a
    /// third implementation.
    ///
    /// Connectivity is a field on this service — never a global. The "Go offline / Go online" button flips
    /// it, and everything that needs to know asks <see cref="IsOnlineAsync"/>.
    /// </summary>
    public class BrowserDeviceServices : IDeviceServices
    {
        // Asset tags the simulated scanner returns, in order. The third one belongs to another site: device
        // output is untrusted input, and the server rejects it (see WorkOrderService.ValidateScannedAssetAsync).
        private static readonly string[] SimulatedScans =
        {
            "ASSET-Pump-88121", "ASSET-Subs-44107", "ASSET-Harb-90233", "not-a-barcode",
        };

        private readonly IActivityTrace _trace;
        private int _scanIndex = -1;
        private DeviceProfile? _simulated;
        private bool _online = true;

        public BrowserDeviceServices(IActivityTrace trace)
        {
            _trace = trace;
        }

        /// <summary>The connectivity the shell reports. Flipped by the field screen's connection button.</summary>
        public bool IsOnline => _online;

        public void SetOnline(bool online)
        {
            _online = online;
            _trace.Log(TraceLayer.Device, online
                ? "shell reports connectivity RESTORED (network callback)"
                : "shell reports connectivity LOST — the app keeps running, the server is unreachable");
        }

        /// <summary>Forces a shape, as the Simulate phone / tablet / desktop switch does.</summary>
        public void Simulate(DeviceProfile profile)
        {
            _simulated = profile;
            _trace.Log(TraceLayer.Device, $"simulate switch → {profile}: the screen re-lays out, no screen code changes");
        }

        public void StopSimulating()
        {
            _simulated = null;
            _trace.Log(TraceLayer.Device, "simulate switch cleared → back to the detected device");
        }

        /// <summary>
        /// Reads the client's own report of itself. Application.Browser is populated by Wisej.NET from the
        /// client and refreshed when the browser is resized; it is a hint for layout, never an authorization
        /// input.
        /// </summary>
        public DeviceInfo Describe()
        {
            var info = new DeviceInfo
            {
                ReportedDevice = "Desktop",
                BrowserType = "unknown",
                OperatingSystem = "unknown",
                ScreenWidth = 0,
                ScreenHeight = 0,
            };

            try
            {
                var browser = Application.Browser;
                if (browser != null)
                {
                    info.ReportedDevice = browser.Device ?? "Desktop";
                    info.BrowserType = browser.Type ?? "unknown";
                    info.OperatingSystem = browser.OS ?? "unknown";
                    info.ScreenWidth = browser.ScreenSize.Width;
                    info.ScreenHeight = browser.ScreenSize.Height;
                }
            }
            catch (Exception ex)
            {
                // No client attached (designer, a background thread outside a request): stay on the defaults.
                _trace.Log(TraceLayer.Device, $"Application.Browser unavailable ({ex.GetType().Name}) — assuming Desktop");
            }

            info.Profile = _simulated ?? ProfileFrom(info.ReportedDevice, info.ScreenWidth);
            info.Simulated = _simulated.HasValue;
            return info;
        }

        /// <summary>Device string first ("Mobile" / "Tablet" / "Desktop"), screen width as the tie-breaker.</summary>
        private static DeviceProfile ProfileFrom(string reported, int screenWidth)
        {
            if (string.Equals(reported, "Mobile", StringComparison.OrdinalIgnoreCase)) return DeviceProfile.Phone;
            if (string.Equals(reported, "Tablet", StringComparison.OrdinalIgnoreCase)) return DeviceProfile.Tablet;
            if (screenWidth > 0 && screenWidth < 560) return DeviceProfile.Phone;
            if (screenWidth > 0 && screenWidth < 1100) return DeviceProfile.Tablet;
            return DeviceProfile.Desktop;
        }

        // ── IDeviceServices ───────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// The question every field action asks first. In a Hybrid shell this is the platform's reachability
        /// API; here it is the simulated switch. Either way the caller gets a Task&lt;bool&gt; and no
        /// platform knowledge.
        /// </summary>
        public Task<bool> IsOnlineAsync()
        {
            _trace.Log(TraceLayer.Device, $"IDeviceServices.IsOnlineAsync() → {_online}");
            return Task.FromResult(_online);
        }

        /// <summary>
        /// The barcode/document scanner. Works offline — it is a device feature, not a server call — but its
        /// output is untrusted input and is validated on the server (immediately when online, at sync time
        /// when the value travelled inside a queued command).
        /// </summary>
        public Task<string> ScanDocumentAsync()
        {
            _scanIndex = (_scanIndex + 1) % SimulatedScans.Length;
            string value = SimulatedScans[_scanIndex];
            _trace.Log(TraceLayer.Device, $"IDeviceServices.ScanDocumentAsync() → \"{value}\" (untrusted until the server validates it)");
            return Task.FromResult(value);
        }

        /// <summary>Haptic feedback: a glove-friendly confirmation that does not need the screen to be read.</summary>
        public Task VibrateAsync()
        {
            _trace.Log(TraceLayer.Device, "IDeviceServices.VibrateAsync() → shell haptic (no-op in the browser)");
            return Task.CompletedTask;
        }
    }
}
