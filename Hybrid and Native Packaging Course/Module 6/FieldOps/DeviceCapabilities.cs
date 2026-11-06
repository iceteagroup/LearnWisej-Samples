using Wisej.Hybrid;
using Wisej.Web;

namespace FieldOps
{
    /// <summary>
    /// The capability matrix of THIS session: read once when the session starts, kept in
    /// Application.Session, obeyed by every screen. Never the user agent - the Hybrid client's
    /// WebView and a PWA both look like a mobile browser; only Device.Valid knows.
    /// </summary>
    public sealed class DeviceCapabilities
    {
        /// <summary>Inside the Wisej.NET Hybrid client.</summary>
        public bool Valid { get; private init; }

        /// <summary>The client can take a photo (Device.Media.CaptureSupported).</summary>
        public bool Camera { get; private init; }

        /// <summary>Location + the platform maps app (any Hybrid client).</summary>
        public bool NativeNavigate { get; private init; }

        /// <summary>Operating-system reminders (Device.LocalNotification, any Hybrid client).</summary>
        public bool LocalReminders { get; private init; }

        /// <summary>Biometric unlock needs the Authentication extension, which FieldOps does not ship.</summary>
        public bool Biometrics => false;

        public string Platform { get; private init; }

        public static DeviceCapabilities Current
        {
            get
            {
                if (Application.Session["Capabilities"] is DeviceCapabilities caps) return caps;
                caps = Read();
                Application.Session["Capabilities"] = caps;
                return caps;
            }
        }

        private static DeviceCapabilities Read()
        {
            bool valid = Device.Valid;
            return new DeviceCapabilities
            {
                Valid = valid,
                Camera = valid && Device.Media.CaptureSupported,
                NativeNavigate = valid,
                LocalReminders = valid,
                Platform = valid ? Device.System.Platform.ToString() : "Browser",
            };
        }

        public override string ToString() =>
            $"Device.Valid={Valid} camera={(Camera ? "CapturePhotoAsync" : "Upload")} " +
            $"navigate={(NativeNavigate ? "Geolocation+Map" : "maps link")} " +
            $"reminders={(LocalReminders ? "LocalNotification" : "job list (server push)")} biometrics=absent platform={Platform}";
    }
}
