using System;
using System.IO;
using System.Text.Json;

namespace FieldOps.Services
{
    /// <summary>
    /// Signatures and arrival fixes, stored like photos: outside the web root, under the work order,
    /// with a server-generated timestamp (the device's clock is not evidence).
    /// </summary>
    public sealed class SignatureService
    {
        private readonly string _root;

        public SignatureService(string root) { _root = root; }

        public static SignatureService Current { get; } = new SignatureService(
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FieldOps", "signatures"));

        /// <summary>Stores the flattened PNG plus a small record: order, signer, server time.</summary>
        public string Save(string orderNumber, byte[] png, string signerName)
        {
            if (png == null || png.Length < 100)
                throw new ArgumentException("The signature is empty.");
            if (string.IsNullOrWhiteSpace(signerName))
                throw new ArgumentException("Type the signer's name.");

            var folder = Path.Combine(_root, orderNumber);
            Directory.CreateDirectory(folder);
            var id = Guid.NewGuid().ToString("N");
            File.WriteAllBytes(Path.Combine(folder, id + ".png"), png);
            File.WriteAllText(Path.Combine(folder, id + ".json"), JsonSerializer.Serialize(new
            {
                workOrder = orderNumber,
                signer = signerName.Trim(),
                signedAtUtc = DateTime.UtcNow,       // server time
                bytes = png.Length,
            }));
            return id;
        }
    }

    /// <summary>Latitude, longitude and accuracy with SERVER time - the smallest, most sensitive payload.</summary>
    public static class LocationLog
    {
        private static readonly string _file = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FieldOps", "arrivals.log");
        private static readonly object _gate = new object();

        public static void Record(string orderNumber, string technicianId, double latitude, double longitude, double? accuracy)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_file));
            lock (_gate)
                File.AppendAllText(_file, $"{DateTime.UtcNow:o}\t{orderNumber}\t{technicianId}\t{latitude:F5}\t{longitude:F5}\t{accuracy?.ToString("F0") ?? "?"}{Environment.NewLine}");
        }
    }
}
