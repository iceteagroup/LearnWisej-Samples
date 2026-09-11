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
    /// <b>This session</b> panel is built from the injected <see cref="SessionContext"/>. Diagnostics reads;
    /// it never decides.
    /// </summary>
    public sealed class DiagnosticsService : IDiagnosticsService
    {
        private readonly AppSettings _settings;
        private readonly IRuntimeInfo _runtime;
        private readonly SessionContext _ctx;
        private readonly SharedTicketStore _store;

        public DiagnosticsService(AppSettings settings, IRuntimeInfo runtime, SessionContext ctx, SharedTicketStore store)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
            _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public DiagnosticsSnapshot GetSnapshot()
        {
            var application = new List<SettingRow>
            {
                new SettingRow("Environment / build", $"{_settings.Environment} · {_settings.BuildVersion}"),
                new SettingRow("Dispatch API base URL", _settings.DispatchApiBaseUrl),
                new SettingRow("Upload limit", $"{_settings.UploadLimitMB} MB"),
                new SettingRow("Logging level", _settings.LoggingLevel),
                new SettingRow("Idle session timeout (Default.json)", _runtime.ConfiguredSessionTimeoutSeconds.HasValue ? $"{_runtime.ConfiguredSessionTimeoutSeconds} s" : "(not read)"),
                new SettingRow("Default theme (Default.json)", _runtime.ConfiguredThemeName ?? "(not read)"),
                new SettingRow("Server", _runtime.Server ?? "(not read)"),
                new SettingRow("Active sessions", _runtime.ActiveSessionCount.HasValue ? _runtime.ActiveSessionCount.Value.ToString(CultureInfo.InvariantCulture) : "(not read)"),
                new SettingRow("Sessions started (this process)", SharedCounters.ContextsCreated.ToString(CultureInfo.InvariantCulture)),
                new SettingRow("Tickets in the store", _store.Count.ToString(CultureInfo.InvariantCulture))
            };

            var age = DateTime.UtcNow - _ctx.StartedUtc;
            var session = new List<SettingRow>
            {
                new SettingRow("Session id", _ctx.SessionId ?? "(none)"),
                new SettingRow("User", _ctx.CurrentUser ?? "(none)"),
                new SettingRow("Tenant", _ctx.Tenant ?? "(none)"),
                new SettingRow("Theme", _ctx.Theme ?? "(none)"),
                new SettingRow("Client profile", _ctx.ClientProfile ?? "(none)"),
                new SettingRow("Selected ticket", _ctx.SelectedTicketId.HasValue ? "#" + _ctx.SelectedTicketId.Value : "(none)"),
                new SettingRow("Session started", $"{_ctx.StartedUtc.ToLocalTime():HH:mm:ss} · {(int)age.TotalSeconds} s ago")
            };

            return new DiagnosticsSnapshot(application, session);
        }
    }
}
