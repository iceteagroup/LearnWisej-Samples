using System;
using System.Collections.Generic;
using System.Globalization;
using TicketOps.Data;
using TicketOps.Infrastructure;
using TicketOps.Services;

namespace TicketOps.Diagnostics
{
    /// <summary>
    /// Read-only diagnostics. The <b>Application</b> panel is built from things that are the same for every
    /// session (AppSettings, Default.json via IRuntimeInfo, the shared store, the shared counter); the
    /// <b>This session</b> panel is built from the injected <see cref="SessionContext"/> — never from
    /// <c>Application.Session</c> read ad hoc, which invites the very static-style shortcuts this page exists
    /// to expose. Diagnostics reads; it never decides.
    /// </summary>
    public sealed class DiagnosticsService : IDiagnosticsService
    {
        private readonly AppSettings _settings;
        private readonly IRuntimeInfo _runtime;
        private readonly SessionContext _ctx;
        private readonly SharedTicketStore _store;
        private readonly ILog _log;

        public DiagnosticsService(AppSettings settings, IRuntimeInfo runtime, SessionContext ctx, SharedTicketStore store, ILog log)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
            _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public DiagnosticsSnapshot GetSnapshot()
        {
            _log.Info(LogLayer.Infrastructure, "DiagnosticsService.GetSnapshot",
                $"application rows ← AppSettings ({ShortPath(_settings.LoadedFrom)}) + Application.Configuration + shared store/counter");

            var application = new List<SettingRow>
            {
                new SettingRow("Environment / build", $"{_settings.Environment} · {_settings.BuildVersion}"),
                new SettingRow("Dispatch API base URL", _settings.DispatchApiBaseUrl),
                new SettingRow("Upload limit", $"{_settings.UploadLimitMB} MB"),
                new SettingRow("Logging level", _settings.LoggingLevel),
                new SettingRow("Idle session timeout (Default.json)", _runtime.ConfiguredSessionTimeoutSeconds.HasValue ? $"{_runtime.ConfiguredSessionTimeoutSeconds} s" : "(not read)"),
                new SettingRow("Default theme (Default.json)", _runtime.ConfiguredThemeName ?? "(not read)"),
                new SettingRow("Server", _runtime.Server ?? "(not read)"),
                new SettingRow("Active sessions (Application.SessionCount)", _runtime.ActiveSessionCount.HasValue ? _runtime.ActiveSessionCount.Value.ToString(CultureInfo.InvariantCulture) : "(not read)"),
                new SettingRow("SessionContexts created (this process)", SharedCounters.ContextsCreated.ToString(CultureInfo.InvariantCulture)),
                new SettingRow("Tickets in the shared store", _store.Count.ToString(CultureInfo.InvariantCulture)),
                new SettingRow("⚠ Legacy static probe", StaticLeakProbe.LastWriter)
            };

            _log.Info(LogLayer.Session, "DiagnosticsService.GetSnapshot", $"session rows ← injected SessionContext {_ctx}");

            var age = DateTime.UtcNow - _ctx.StartedUtc;
            bool? bag = _runtime.SessionBagHolds(_ctx);
            var session = new List<SettingRow>
            {
                new SettingRow("Session id", _ctx.SessionId ?? "(none)"),
                new SettingRow("User", _ctx.CurrentUser ?? "(none)"),
                new SettingRow("Tenant", _ctx.Tenant ?? "(none)"),
                new SettingRow("Theme (SessionContext)", _ctx.Theme ?? "(none)"),
                new SettingRow("Theme rendering now (Application.Theme)", _runtime.ActiveThemeName ?? "(not read)"),
                new SettingRow("Client profile", _ctx.ClientProfile ?? "(none)"),
                new SettingRow("Client profile now (Application.ActiveProfile)", _runtime.ActiveClientProfile ?? "(not read)"),
                new SettingRow("Selected ticket", _ctx.SelectedTicketId.HasValue ? "#" + _ctx.SelectedTicketId.Value : "(none)"),
                new SettingRow("Session started", $"{_ctx.StartedUtc.ToLocalTime():HH:mm:ss} · {(int)age.TotalSeconds} s ago"),
                new SettingRow("Same instance in Application.Session?", bag.HasValue ? (bag.Value ? "yes" : "NO — investigate") : "(not read)")
            };

            return new DiagnosticsSnapshot(application, session);
        }

        private static string ShortPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return "(defaults)";
            int i = path.LastIndexOfAny(new[] { '\\', '/' });
            return i >= 0 ? path.Substring(i + 1) : path;
        }
    }
}
