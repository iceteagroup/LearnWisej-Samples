using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using EnterpriseOps.Data;
using EnterpriseOps.Services;
using Wisej.Web;

namespace EnterpriseOps.Diagnostics
{
    /// <summary>Live numbers for the "Session &amp; health" card. All safe to show; the session id is shortened.</summary>
    public sealed class SessionStats
    {
        public int SessionCount { get; init; }
        public long ManagedHeapBytes { get; init; }
        public long WorkingSetBytes { get; init; }
        public TimeSpan ProcessUptime { get; init; }
        public TimeSpan SessionAge { get; init; }
        public string SessionIdSuffix { get; init; }
    }

    /// <summary>
    /// Builds what the diagnostics page shows. The snapshot is chosen by type: version, environment, node,
    /// theme, flags, safe recent events — nothing else can get in. Sensitive values from the deployment
    /// config are redacted here, before any view model exists, and the trace says what was redacted and how.
    /// </summary>
    public sealed class DiagnosticsService
    {
        private static readonly DateTime ProcessStartUtc = ReadProcessStartUtc();

        private readonly DeploymentConfig _config;
        private readonly SessionContext _session;
        private readonly StructuredLog _log;
        private readonly Action<string> _trace;

        public DiagnosticsService(DeploymentConfig config, SessionContext session, StructuredLog log, Action<string> trace)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _trace = trace ?? throw new ArgumentNullException(nameof(trace));
        }

        /// <summary>What was kept out of the last snapshot, and how. Shown as a count on the page, in full in the trace.</summary>
        public IReadOnlyList<string> RedactionNotes { get; private set; } = Array.Empty<string>();

        /// <summary>The version support needs: AssemblyInformationalVersion (semver + commit), from Properties/AssemblyInfo.cs.</summary>
        public static string Version
        {
            get
            {
                Assembly assembly = typeof(DiagnosticsService).Assembly;
                string informational = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
                return string.IsNullOrWhiteSpace(informational) ? assembly.GetName().Version?.ToString() ?? "0.0.0" : informational;
            }
        }

        public DiagnosticSnapshot CaptureSnapshot()
        {
            // Copy the flags so the snapshot cannot be used to mutate the configuration.
            var flags = new Dictionary<string, string>(_config.FeatureFlags);

            var snapshot = new DiagnosticSnapshot(
                Version,
                _config.Environment,
                _config.NodeName,
                ThemeName(),
                flags,
                _log.SafeRecentEvents(5));

            RedactionNotes = new[]
            {
                "ConnectionString — excluded: no snapshot field can carry it",
                $"StorageAccountName — masked → {Mask(_config.StorageAccountName)}",
                "ApiKey — excluded",
                "exception messages — never in SafeRecentEvents (server log only)",
                $"session id — shortened to …{SessionIdSuffix()}",
            };

            _trace($"Diagnostics: DiagnosticSnapshot built by type — version {snapshot.Version} · {snapshot.Environment} · {snapshot.NodeName} · theme {snapshot.ActiveTheme} · {flags.Count} flags · {snapshot.SafeRecentEvents.Count} safe events");
            _trace($"Diagnostics: redacted {RedactionNotes.Count} — {string.Join(" · ", RedactionNotes)}");

            return snapshot;
        }

        public SessionStats CaptureStats()
        {
            using Process process = Process.GetCurrentProcess();

            return new SessionStats
            {
                SessionCount = ReadSessionCount(),
                ManagedHeapBytes = GC.GetTotalMemory(forceFullCollection: false),
                WorkingSetBytes = process.WorkingSet64,
                ProcessUptime = DateTime.UtcNow - ProcessStartUtc,
                SessionAge = _session.SessionAge,
                SessionIdSuffix = SessionIdSuffix(),
            };
        }

        /// <summary>"stenterpriseopsprod" → "st•••••prod": enough to recognise, not enough to use.</summary>
        public static string Mask(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            if (value.Length <= 6) return new string('•', value.Length);
            return value.Substring(0, 2) + "•••••" + value.Substring(value.Length - 4);
        }

        private string SessionIdSuffix()
        {
            string id = _session.SessionId ?? "";
            return id.Length <= 6 ? id : id.Substring(id.Length - 6);
        }

        private static string ThemeName()
        {
            try
            {
                return Application.Theme?.Name ?? "(default)";
            }
            catch
            {
                return "(unavailable)";
            }
        }

        private static int ReadSessionCount()
        {
            try
            {
                return Application.SessionCount;
            }
            catch
            {
                return -1;
            }
        }

        private static DateTime ReadProcessStartUtc()
        {
            try
            {
                using Process process = Process.GetCurrentProcess();
                return process.StartTime.ToUniversalTime();
            }
            catch
            {
                return DateTime.UtcNow;
            }
        }
    }
}
