using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Data;
using TicketOps.Domain;
using TicketOps.Infrastructure;

namespace TicketOps.Services
{
    /// <summary>
    /// The ticket workflow, now depending on <see cref="SessionContext"/>. Because the service is handed
    /// THIS session's context (instead of reaching into a static or reading a control), it can be
    /// unit-tested by passing a hand-made context — no browser, no session, no server required — and
    /// it stays correct under concurrent sessions: session A's tenant can never leak into session B's query.
    /// </summary>
    public sealed class TicketService : ITicketService
    {
        private const int MaxTitleLength = 80;

        private readonly SessionContext _ctx;
        private readonly ITicketRepository _repository;
        private readonly ILog _log;

        public TicketService(SessionContext ctx, ITicketRepository repository, ILog log)
        {
            _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<IReadOnlyList<Ticket>> GetOpenTicketsAsync()
        {
            _log.Info(LogLayer.Service, "TicketService.GetOpenTicketsAsync", $"tenant {_ctx.Tenant} (from SessionContext) → ITicketRepository.GetByTenantAsync");
            var rows = await _repository.GetByTenantAsync(_ctx.Tenant);
            _log.Info(LogLayer.Service, "TicketService.GetOpenTicketsAsync", $"{rows.Count} open tickets for {_ctx.Tenant}");
            return rows;
        }

        public async Task<OperationResult<Ticket>> CreateForCurrentUserAsync(string title)
        {
            _log.Info(LogLayer.Service, "TicketService.CreateForCurrentUserAsync", $"validate title \"{title}\"");

            var errors = Validate(title);
            if (errors.Count > 0)
            {
                _log.Warn(LogLayer.Service, "TicketService.CreateForCurrentUserAsync", $"rejected: {string.Join("; ", errors)}");
                return OperationResult<Ticket>.Fail(errors[0], errors.ToArray());
            }

            var ticket = new Ticket
            {
                Title = title.Trim(),
                Tenant = _ctx.Tenant,          // scoped to this session
                Author = _ctx.CurrentUser,     // safe under concurrent sessions
                Priority = TicketPriority.Medium
            };

            _log.Info(LogLayer.Service, "TicketService.CreateForCurrentUserAsync", $"stamped Tenant={ticket.Tenant}, Author=\"{ticket.Author}\" from SessionContext → ITicketRepository.UpsertAsync");
            var saved = await _repository.UpsertAsync(ticket);
            return OperationResult<Ticket>.Ok(saved, $"Ticket #{saved.Id} created for {saved.Tenant}.");
        }

        private static List<string> Validate(string title)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(title))
                errors.Add("Title is required.");
            else if (title.Trim().Length > MaxTitleLength)
                errors.Add($"Title must be {MaxTitleLength} characters or fewer.");
            return errors;
        }
    }
}
