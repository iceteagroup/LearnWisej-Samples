using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketOps.Data;
using TicketOps.Domain;
using TicketOps.Infrastructure;

namespace TicketOps.Services
{
    /// <summary>
    /// The session-scoped service that owns the rules around <see cref="SessionContext"/>.
    /// It receives THIS session's context through its constructor (no <c>SessionContext.Current</c>,
    /// no static), so a unit test can hand it a hand-made context and a fake directory.
    /// Shared, read-only inputs (<see cref="AppSettings"/>) are injected too — read once per process,
    /// never mutated here.
    /// </summary>
    public sealed class SessionService : ISessionService
    {
        private readonly SessionContext _ctx;
        private readonly IUserDirectory _users;
        private readonly AppSettings _settings;
        private readonly ILog _log;

        public SessionService(SessionContext ctx, IUserDirectory users, AppSettings settings, ILog log)
        {
            _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
            _users = users ?? throw new ArgumentNullException(nameof(users));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public Task<IReadOnlyList<UserAccount>> GetUsersAsync()
        {
            _log.Info(LogLayer.Service, "SessionService.GetUsersAsync", "→ IUserDirectory.GetUsersAsync()");
            return _users.GetUsersAsync();
        }

        public Task<IReadOnlyList<string>> GetTenantsAsync()
        {
            _log.Info(LogLayer.Service, "SessionService.GetTenantsAsync", "→ IUserDirectory.GetTenantsAsync()");
            return _users.GetTenantsAsync();
        }

        public async Task<OperationResult<SessionContext>> SignInAsync(string userName)
        {
            _log.Info(LogLayer.Service, "SessionService.SignInAsync", $"\"{userName}\" → IUserDirectory.FindAsync");
            var user = await _users.FindAsync(userName);
            if (user == null)
            {
                _log.Warn(LogLayer.Service, "SessionService.SignInAsync", $"rejected: '{userName}' is not a TicketOps operator");
                return OperationResult<SessionContext>.Fail("That operator does not exist.");
            }

            // Identity (user scope) becomes part of THIS session's context (session scope).
            _ctx.CurrentUser = user.Name;
            _ctx.Tenant = user.IsMemberOf(_ctx.Tenant) ? _ctx.Tenant : user.DefaultTenant;
            _ctx.SelectedTicketId = null;
            _log.Info(LogLayer.Session, "SessionContext", $"CurrentUser = \"{_ctx.CurrentUser}\", Tenant = {_ctx.Tenant} — session {_ctx.ShortId} only");
            return OperationResult<SessionContext>.Ok(_ctx, $"Signed in as {user.Name} · {_ctx.Tenant}.");
        }

        public async Task<OperationResult<SessionContext>> SwitchTenantAsync(string tenant)
        {
            _log.Info(LogLayer.Service, "SessionService.SwitchTenantAsync", $"{tenant} → IUserDirectory.FindAsync(\"{_ctx.CurrentUser}\")");
            var user = await _users.FindAsync(_ctx.CurrentUser);
            if (user == null)
                return OperationResult<SessionContext>.Fail("Sign in first.");

            // The rule lives on the domain object; the service applies it and records the outcome.
            if (!user.IsMemberOf(tenant))
            {
                _log.Warn(LogLayer.Domain, "UserAccount.IsMemberOf", $"rejected: {user.Name} is not a member of tenant {tenant} (member of {string.Join(", ", user.Tenants)})");
                return OperationResult<SessionContext>.Fail($"{user.Name} is not a member of {tenant}.");
            }

            _ctx.Tenant = tenant;
            _ctx.SelectedTicketId = null;
            _log.Info(LogLayer.Session, "SessionContext", $"Tenant = {tenant}, SelectedTicketId = null — session {_ctx.ShortId} only");
            return OperationResult<SessionContext>.Ok(_ctx, $"Tenant switched to {tenant}.");
        }

        public OperationResult<SessionContext> SelectTheme(string theme)
        {
            _log.Info(LogLayer.Service, "SessionService.SelectTheme", $"\"{theme}\" — allowed: {string.Join(", ", _settings.AvailableThemes)} (AppSettings, read-only)");
            if (!_settings.AvailableThemes.Any(t => string.Equals(t, theme, StringComparison.OrdinalIgnoreCase)))
            {
                _log.Warn(LogLayer.Service, "SessionService.SelectTheme", $"rejected: '{theme}' is not offered by this deployment");
                return OperationResult<SessionContext>.Fail("That theme is not available on this deployment.");
            }

            _ctx.Theme = theme;
            _log.Info(LogLayer.Session, "SessionContext", $"Theme = {theme} — session {_ctx.ShortId} only (the configured default stays {_settings.DefaultTheme ?? "(config)"})");
            return OperationResult<SessionContext>.Ok(_ctx, $"Theme {theme} selected for this session.");
        }

        public void SelectTicket(int? ticketId)
        {
            _ctx.SelectedTicketId = ticketId;
            _log.Info(LogLayer.Session, "SessionContext",
                ticketId.HasValue
                    ? $"SelectedTicketId = {ticketId} — this session only ({_ctx.ShortId})"
                    : $"SelectedTicketId = null — this session only ({_ctx.ShortId})");
        }

        public SessionContext SimulateAnotherSession(int number)
        {
            // What AppComposition does for a real connection, minus the Wisej.NET session behind it:
            // a fresh object with its own id. It shares nothing with _ctx — that is the whole point.
            var other = new SessionContext
            {
                SessionId = Guid.NewGuid().ToString("N"),
                CurrentUser = $"load-user-{number:00}",
                Tenant = _settings.DefaultTenant,
                Theme = _settings.DefaultTheme,
                ClientProfile = "Simulated"
            };
            long total = SharedCounters.ContextCreated();
            _log.Info(LogLayer.Session, "SessionService.SimulateAnotherSession",
                $"#{number:00} → new SessionContext {other.ShortId} for {other.CurrentUser} · contexts on this server: {total} · this session still {_ctx.ShortId}/{_ctx.CurrentUser}");
            return other;
        }
    }
}
