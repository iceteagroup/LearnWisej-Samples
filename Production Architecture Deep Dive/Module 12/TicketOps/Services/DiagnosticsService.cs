using System;
using System.Threading.Tasks;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;

namespace TicketOps.Services
{
    /// <summary>
    /// Collects what a Supervisor needs to answer "what is this node doing right now?": server, port, runtime
    /// mode, product version, session count, WebSocket state, uptime — and the live health report.
    /// The role check lives here (not in a button handler) and the snapshot carries no secret by construction:
    /// IRuntimeInfo simply has no member for a connection string or a key.
    /// </summary>
    public sealed class DiagnosticsService : IDiagnosticsService
    {
        private readonly IRuntimeInfo _runtime;
        private readonly IHealthCheckService _health;
        private readonly IUserContext _user;
        private readonly ILog _log;

        public DiagnosticsService(IRuntimeInfo runtime, IHealthCheckService health, IUserContext user, ILog log)
        {
            _runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
            _health = health ?? throw new ArgumentNullException(nameof(health));
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<OperationResult<DiagnosticsSnapshot>> GetSnapshotAsync()
        {
            if (_user.Role != OperatorRole.Supervisor)
            {
                // Expected outcome, not an exception: the screen shows the sentence, nothing else is revealed.
                _log.Warn(LogLayer.Service, "DiagnosticsService.GetSnapshotAsync", $"access denied for {_user.UserName}: diagnostics are for Supervisors only");
                return OperationResult<DiagnosticsSnapshot>.Fail(Strings.DiagnosticsAccessDenied);
            }

            var snapshot = new DiagnosticsSnapshot
            {
                ServerName = _runtime.ServerName,
                ServerPort = _runtime.ServerPort,
                RuntimeMode = _runtime.RuntimeMode,
                ProductVersion = _runtime.ProductVersion,
                Framework = _runtime.Framework,
                SessionCount = _runtime.SessionCount,
                SessionId = _runtime.SessionId,
                IsWebSocket = _runtime.IsWebSocket,
                Uptime = _runtime.Uptime,
                ServerTimeUtc = _runtime.ServerTimeUtc,
                TimeZoneId = _runtime.TimeZoneId,
                OperatorName = _user.UserName,
                OperatorRole = _user.Role
            };

            snapshot.Health = await _health.CheckAsync();
            snapshot.TakenAt = DateTime.Now;

            return OperationResult<DiagnosticsSnapshot>.Ok(snapshot, $"Diagnostics refreshed · {snapshot.Health.Status} · HTTP {snapshot.Health.HttpStatusCode}");
        }

        public static string FormatUptime(TimeSpan uptime)
            => $"{(int)uptime.TotalDays}d {uptime.Hours:00}:{uptime.Minutes:00}:{uptime.Seconds:00}";
    }
}
