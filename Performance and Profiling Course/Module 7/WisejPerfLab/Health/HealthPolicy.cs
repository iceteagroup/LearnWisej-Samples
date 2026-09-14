using System;
using System.Threading;
using Wisej.Core;

namespace WisejPerfLab.Health
{
    /// <summary>
    /// The health gate: what the load balancer asks, and what this instance answers.
    /// </summary>
    /// <remarks>
    /// Wisej.NET serves <c>healthcheck.wx</c> and reads <c>HealthCheck.json</c> from the project root.
    /// The thresholds in that file come from <see cref="CapacityModel"/>, which comes from the
    /// measurements in Modules 3 to 6.
    /// <para>
    /// <see cref="Apply"/> also installs a custom <c>HealthCheck.IsServerAvailable</c>, for two reasons:
    /// it records <b>why</b> the instance last said no, which is the thing an operator needs at three in
    /// the morning; and it makes the refusal demonstrable — the lab can lower the session limit to the
    /// number of sessions that exist right now and watch the next browser tab be turned away while this
    /// one keeps working.
    /// </para>
    /// </remarks>
    public static class HealthPolicy
    {
        private static int _sessionLimitOverride;
        private static int _configuredMaxSessions = -1;

        /// <summary>The last decision, for the screen: "available — 1 session, 34 % memory, 6 % CPU".</summary>
        public static string LastAnswer { get; private set; } = "not asked yet";

        public static DateTime? LastAskedAt { get; private set; }

        /// <summary>Refusals since the process started.</summary>
        public static int Refusals { get; private set; }

        /// <summary>The session limit in force: the lab override when set, otherwise the configured one.</summary>
        public static int EffectiveMaxSessions
            => Volatile.Read(ref _sessionLimitOverride) > 0
                ? Volatile.Read(ref _sessionLimitOverride)
                : HealthCheck.MaxSessions;

        /// <summary>Installs the custom availability function. Called once, from Startup.</summary>
        public static void Apply()
        {
            // Remember what HealthCheck.json configured, so the lab override can be undone.
            _configuredMaxSessions = HealthCheck.MaxSessions;

            HealthCheck.IsServerAvailable = IsAvailable;
        }

        /// <summary>
        /// The decision itself: is this instance willing to take another session?
        /// </summary>
        /// <remarks>
        /// It is called from two places — the framework hook <c>HealthCheck.IsServerAvailable</c>, and the
        /// <c>/healthcheck.wx</c> endpoint this sample serves in <c>Startup.cs</c>. Both use the same
        /// thresholds, so the load balancer and the screen can never disagree.
        /// </remarks>
        public static bool IsAvailable()
        {
            LastAskedAt = DateTime.Now;

            var sessions = Wisej.Web.Application.SessionCount;
            var memory = ServerLoad.MemoryPercent();
            var cpu = ServerLoad.CpuPercent();
            var limit = EffectiveMaxSessions;

            string refusedBecause = null;
            if (limit > 0 && sessions >= limit)
                refusedBecause = $"sessions {sessions} >= maxSessions {limit}";
            else if (HealthCheck.MaxMemory > 0 && memory >= HealthCheck.MaxMemory)
                refusedBecause = $"memory {memory} % >= maxMemory {HealthCheck.MaxMemory} %";
            else if (HealthCheck.MaxCPU > 0 && cpu >= HealthCheck.MaxCPU)
                refusedBecause = $"CPU {cpu} % >= maxCPU {HealthCheck.MaxCPU} %";

            if (refusedBecause != null)
            {
                Refusals++;
                LastAnswer = $"{HealthCheck.ReturnCode} — {refusedBecause}, retry after {HealthCheck.RetryAfter} s";
                Console.Error.WriteLine("[WisejPerfLab] healthcheck: NOT available — " + refusedBecause);
                return false;
            }

            LastAnswer = $"available — {sessions} session(s), memory {memory} %, CPU {cpu} %";
            return true;
        }

        /// <summary>
        /// The lab control: refuse new sessions from now on, by lowering the limit to the number of
        /// sessions that already exist. Existing sessions are untouched — that is the whole point of a
        /// health check, and the thing that is worth seeing once with your own eyes.
        /// </summary>
        public static void RefuseNewSessions()
        {
            var limit = Math.Max(1, Wisej.Web.Application.SessionCount);
            Volatile.Write(ref _sessionLimitOverride, limit);

            // The framework property, not only our own function: this is what the load balancer's
            // healthcheck.wx request is answered from, and setting it is exactly what the Wisej.NET
            // documentation shows an application doing when it wants to control its own availability.
            HealthCheck.MaxSessions = limit;
        }

        /// <summary>Back to the configured limit.</summary>
        public static void RestoreConfiguredLimit()
        {
            Volatile.Write(ref _sessionLimitOverride, 0);

            if (_configuredMaxSessions >= 0)
                HealthCheck.MaxSessions = _configuredMaxSessions;
        }

        /// <summary>What HealthCheck.json configured, before any lab override.</summary>
        public static int ConfiguredMaxSessions => _configuredMaxSessions;

        /// <summary>The configuration as the file set it, for the screen.</summary>
        public static string DescribeConfiguration()
            => $"enabled {HealthCheck.Enabled}   maxSessions {ConfiguredMaxSessions}   " +
               $"maxMemory {HealthCheck.MaxMemory} %   maxCPU {HealthCheck.MaxCPU} %   " +
               $"returnCode {HealthCheck.ReturnCode}   retryAfter {HealthCheck.RetryAfter} s";
    }
}
