using EnterpriseOps.Data;
using EnterpriseOps.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace EnterpriseOps.Services
{
    /// <summary>One entry of HealthCheck.json "checks" (the contract the balancer and the dashboard share).</summary>
    public class HealthCheckDefinition
    {
        public string Name { get; set; }
        public bool Critical { get; set; }
        public string Description { get; set; }
    }

    /// <summary>The parts of HealthCheck.json the code reads (the rest is documentation for operations).</summary>
    public class HealthCheckFile
    {
        public Dictionary<string, string> Endpoints { get; set; } = new Dictionary<string, string>();
        public HealthProbeSettings Probe { get; set; } = new HealthProbeSettings();
        public HealthLimits Limits { get; set; } = new HealthLimits();
        public List<HealthCheckDefinition> Checks { get; set; } = new List<HealthCheckDefinition>();
    }

    public class HealthProbeSettings
    {
        public int IntervalSeconds { get; set; } = 10;
        public int TimeoutSeconds { get; set; } = 3;
        public int UnhealthyThreshold { get; set; } = 3;
        public int HealthyThreshold { get; set; } = 2;
    }

    public class HealthLimits
    {
        public int MaxSessionsPerNode { get; set; } = 500;
    }

    /// <summary>
    /// Runs the checks HealthCheck.json names, in-process, and answers GET /healthz (readiness) and
    /// GET /healthz/live (liveness). Process-wide by nature — a node has ONE health, whoever asks —
    /// so a static holder is right here. Nothing user- or tenant-specific is stored.
    ///
    /// The same check list is used three times: by the probe endpoint (the balancer asks), by the
    /// release dashboard (operations look), and by the simulated peer node (so one machine can show
    /// a two-node release). safeToExpose in HealthCheck.json is honoured: Detail never carries an
    /// exception message or a connection string.
    /// </summary>
    public static class HealthProbeService
    {
        private static readonly WorkOrderRepository Repository = new WorkOrderRepository();
        private static int _sessionsStarted;

        public static HealthCheckFile Contract { get; private set; } = new HealthCheckFile();
        public static string ContractPath { get; private set; } = "(HealthCheck.json not loaded)";
        public static string LoadError { get; private set; }

        /// <summary>JSON shape of the probe body: public fields included, enums as strings, camelCase — the balancer reads it, not a .NET client.</summary>
        public static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            IncludeFields = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() },
        };

        /// <summary>Startup.cs calls this once. A missing or broken HealthCheck.json is a configuration error the probe reports, not a crash.</summary>
        public static void LoadHealthCheckFile(string contentRootPath)
        {
            ContractPath = Path.Combine(contentRootPath, "HealthCheck.json");
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, ReadCommentHandling = JsonCommentHandling.Skip };
                Contract = JsonSerializer.Deserialize<HealthCheckFile>(File.ReadAllText(ContractPath), options) ?? new HealthCheckFile();
                LoadError = Contract.Checks.Count == 0 ? "HealthCheck.json names no checks" : null;
            }
            catch (Exception ex)
            {
                LoadError = "HealthCheck.json could not be read (" + ex.GetType().Name + ")";
                Contract = new HealthCheckFile();
            }
        }

        /// <summary>Program.Main counts sessions so the sessionStore check has a real number (process-wide, not user state).</summary>
        public static int SessionStarted() => Interlocked.Increment(ref _sessionsStarted);

        public static int SessionsStarted => _sessionsStarted;

        /// <summary>GET /healthz — readiness: every critical check must pass. Runs the real checks against THIS process.</summary>
        public static HealthReport Probe()
        {
            var report = NewReport(HostConfiguration.NodeName, HostConfiguration.ReleaseVersion);
            foreach (var check in Contract.Checks)
                report.Checks.Add(Run(check, databaseFault: false));
            if (LoadError != null)
                report.Checks.Insert(0, new HealthCheckResult { Name = "healthcheck.json", Critical = true, Status = HealthStatus.Fail, Detail = LoadError });
            return report;
        }

        /// <summary>
        /// The same checks for a node this process is NOT (app-node-B on the dashboard). Everything that
        /// is process-independent (configuration, storage, websocket, theme) is checked for real; the
        /// database check honours the simulated migration fault that the release failure path injects.
        /// </summary>
        public static HealthReport ProbeSimulated(string nodeName, string build, bool databaseFault)
        {
            var report = NewReport(nodeName, build);
            foreach (var check in Contract.Checks)
                report.Checks.Add(Run(check, databaseFault));
            return report;
        }

        /// <summary>GET /healthz/live — liveness never inspects dependencies: "the process can run code".</summary>
        public static object Liveness() => new
        {
            status = "Alive",
            node = HostConfiguration.NodeName,
            release = HostConfiguration.ReleaseVersion,
            environment = HostConfiguration.EnvironmentName,
            uptimeSeconds = (int)(DateTime.UtcNow - ProcessStartUtc).TotalSeconds,
            timestampUtc = DateTime.UtcNow,
        };

        private static readonly DateTime ProcessStartUtc = DateTime.UtcNow;

        private static HealthReport NewReport(string node, string release) => new HealthReport
        {
            Node = node,
            Release = release,
            Environment = HostConfiguration.EnvironmentName,
            TimestampUtc = DateTime.UtcNow,
        };

        /// <summary>One check → one result. Names come from HealthCheck.json; an unknown name is "skipped", never a crash.</summary>
        private static HealthCheckResult Run(HealthCheckDefinition check, bool databaseFault)
        {
            var result = new HealthCheckResult { Name = check.Name, Critical = check.Critical, Status = HealthStatus.Skipped, Detail = "no probe implemented for this check name" };
            try
            {
                switch (check.Name)
                {
                    case "configuration":
                        bool ok = HostConfiguration.StartupValidation != null && HostConfiguration.StartupValidation.Succeeded;
                        Set(result, ok, ok ? $"startup validation passed for {HostConfiguration.EnvironmentName}" : "startup validation reported errors");
                        break;

                    case "database":
                        if (databaseFault)
                        {
                            // The simulated 2.4.2 migration: the work-order table is mid-migration, the known query returns nothing.
                            Set(result, false, "known query returned 0 rows (migration 2.4.2 incomplete)");
                            break;
                        }
                        int rows = Repository.CountOpen("contoso");
                        Set(result, rows > 0, $"known query returned {rows} open work orders for the probe tenant");
                        break;

                    case "storage":
                        Set(result, StorageRootWritable(out string storageDetail), storageDetail);
                        break;

                    case "websocket":
                        Set(result, WebSocketEnabled(out string wsDetail), wsDetail);
                        break;

                    case "sessionStore":
                        int limit = Contract.Limits.MaxSessionsPerNode;
                        Set(result, _sessionsStarted < limit, $"{_sessionsStarted} sessions started on this node (limit {limit})");
                        break;

                    case "theme":
                        string theme = ReadDefaultJsonValue("theme");
                        Set(result, !string.IsNullOrWhiteSpace(theme), string.IsNullOrWhiteSpace(theme) ? "Default.json names no theme" : $"Default.json theme = {theme}");
                        break;
                }
            }
            catch (Exception ex)
            {
                // safeToExpose.exceptionMessages = false: the type is enough for the balancer; the log gets the rest.
                Set(result, false, "check threw " + ex.GetType().Name);
            }
            return result;
        }

        private static void Set(HealthCheckResult result, bool ok, string detail)
        {
            result.Status = ok ? HealthStatus.Ok : HealthStatus.Fail;
            result.Detail = detail;
        }

        /// <summary>The upload/storage root must be writable — a probe file is written and deleted (the folder is git-ignored).</summary>
        private static bool StorageRootWritable(out string detail)
        {
            string root = Path.GetFullPath(Path.Combine(HostConfiguration.ContentRootPath, HostConfiguration.StorageRoot));
            Directory.CreateDirectory(root);
            string probe = Path.Combine(root, ".healthz-probe");
            File.WriteAllText(probe, DateTime.UtcNow.ToString("O"));
            File.Delete(probe);
            detail = "storage root writable (" + HostConfiguration.StorageRoot + ")";
            return true;
        }

        /// <summary>Default.json "webSocket" is on unless explicitly false; the proxy must still forward the Upgrade (deployment/nginx.conf).</summary>
        private static bool WebSocketEnabled(out string detail)
        {
            string value = ReadDefaultJsonValue("webSocket");
            bool enabled = !string.Equals(value, "false", StringComparison.OrdinalIgnoreCase);
            detail = enabled ? "WebSocket transport enabled in Default.json — proxy must forward Upgrade/Connection headers" : "Default.json disables the WebSocket transport";
            return enabled;
        }

        private static string ReadDefaultJsonValue(string property)
        {
            string path = Path.Combine(HostConfiguration.ContentRootPath, "Default.json");
            if (!File.Exists(path))
                return null;
            using (var doc = JsonDocument.Parse(File.ReadAllText(path), new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip }))
            {
                return doc.RootElement.TryGetProperty(property, out var element) ? element.ToString() : null;
            }
        }
    }
}
