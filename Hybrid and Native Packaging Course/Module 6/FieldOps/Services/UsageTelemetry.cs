using System;
using System.IO;
using System.Text.Json;
using Wisej.Hybrid;
using Wisej.Web;

namespace FieldOps.Services
{
    /// <summary>
    /// One usage event per device feature run: what ran, how it ended, which client. Lean on
    /// purpose - no photo, no signature image, no customer or signer name, no raw coordinate,
    /// nothing the privacy declaration does not list. The sink here is a JSON-lines file
    /// (%LOCALAPPDATA%\FieldOps\usage.jsonl); point it at your telemetry service in production.
    /// </summary>
    public static class UsageTelemetry
    {
        private static readonly object _gate = new object();
        private static readonly string _file = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FieldOps", "usage.jsonl");

        /// <param name="feature">photo.capture, photo.upload, signature.save, location.resolve, map.open, reminder.schedule, permission.camera, ...</param>
        /// <param name="outcome">ok, cancelled, denied, denied-for-good, no-fix, failed ...</param>
        public static void Record(string feature, string outcome)
        {
            var e = new
            {
                at = DateTime.UtcNow,
                feature,
                outcome,
                client = Device.Valid ? "hybrid" : "browser",
                platform = Device.Valid ? Device.System.Platform.ToString() : "Browser",
                model = Device.Valid ? Device.System.Model : null,
                osVersion = Device.Valid ? Device.System.Version : null,
                clientVersion = Application.Session["ClientVersion"] as string,
                session = Application.SessionId[..8],
            };
            var line = JsonSerializer.Serialize(e);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_file));
                lock (_gate) File.AppendAllText(_file, line + Environment.NewLine);
            }
            catch (IOException ex)
            {
                OpsLog.Error("usage telemetry", ex);
            }
            OpsLog.Info("usage: " + line);
        }
    }
}
