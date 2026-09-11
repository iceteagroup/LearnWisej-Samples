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

        public Task<IReadOnlyList<UserAccount>> GetUsersAsync() => _users.GetUsersAsync();

        public Task<IReadOnlyList<string>> GetTenantsAsync() => _users.GetTenantsAsync();

        public async Task<OperationResult<SessionContext>> SignInAsync(string userName)
        {
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
            return OperationResult<SessionContext>.Ok(_ctx, $"Signed in as {user.Name} · {_ctx.Tenant}.");
        }

        public async Task<OperationResult<SessionContext>> SwitchTenantAsync(string tenant)
        {
            var user = await _users.FindAsync(_ctx.CurrentUser);
            if (user == null)
                return OperationResult<SessionContext>.Fail("Sign in first.");

            // The rule lives on the domain object; the service applies it and records the outcome.
            if (!user.IsMemberOf(tenant))
            {
                _log.Warn(LogLayer.Domain, "UserAccount.IsMemberOf", $"rejected: {user.Name} is not a member of tenant {tenant}");
                return OperationResult<SessionContext>.Fail($"{user.Name} is not a member of {tenant}.");
            }

            _ctx.Tenant = tenant;
            _ctx.SelectedTicketId = null;
            return OperationResult<SessionContext>.Ok(_ctx, $"Tenant switched to {tenant}.");
        }

        public OperationResult<SessionContext> SelectTheme(string theme)
        {
            if (!_settings.AvailableThemes.Any(t => string.Equals(t, theme, StringComparison.OrdinalIgnoreCase)))
            {
                _log.Warn(LogLayer.Service, "SessionService.SelectTheme", $"rejected: '{theme}' is not offered by this deployment");
                return OperationResult<SessionContext>.Fail("That theme is not available on this deployment.");
            }

            _ctx.Theme = theme;
            return OperationResult<SessionContext>.Ok(_ctx, $"Theme {theme} selected for this session.");
        }

        public void SelectTicket(int? ticketId)
        {
            _ctx.SelectedTicketId = ticketId;
        }
    }
}
