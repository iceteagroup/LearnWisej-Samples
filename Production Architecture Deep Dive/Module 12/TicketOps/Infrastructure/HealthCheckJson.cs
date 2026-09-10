using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using TicketOps.Domain;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// JSON in and out for <see cref="HealthReport"/>: the manifest on disk is parsed with it, and the report the
    /// diagnostics page shows (declared values + live probe results) is written with it — the exact body a
    /// /health endpoint would return with the matching HTTP status.
    /// </summary>
    public static class HealthCheckJson
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public static HealthReport Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new InvalidOperationException("HealthCheck.json is empty.");

            var report = JsonSerializer.Deserialize<HealthReport>(json, Options);
            if (report == null)
                throw new InvalidOperationException("HealthCheck.json could not be parsed.");
            return report;
        }

        public static string Serialize(HealthReport report)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));
            return JsonSerializer.Serialize(report, Options);
        }
    }
}
