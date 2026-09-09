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
    /// The fake (in-memory) implementation of <see cref="ITicketService"/>. "Fake" describes the storage,
    /// not the rules: validation and the close rule are the real ones, which is why a screen built
    /// against this service keeps working when the repository becomes a database.
    ///
    /// Notice what is NOT here: no control, no Text property, no AlertBox. The service can be called
    /// from a unit test, a bulk job or a second screen.
    /// </summary>
    public sealed class TicketService : ITicketService
    {
        private const int MaxTitleLength = 80;

        private readonly ITicketRepository _repository;
        private readonly ILog _log;

        public TicketService(ITicketRepository repository, ILog log)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<IReadOnlyList<Ticket>> GetOpenTicketsAsync()
        {
            _log.Info(LogLayer.Service, "TicketService.GetOpenTicketsAsync", "→ ITicketRepository.GetAllAsync()");
            var all = await _repository.GetAllAsync();
            var open = all.Where(t => t.Status != TicketStatus.Closed)
                          .OrderByDescending(t => t.Priority)
                          .ThenBy(t => t.Id)
                          .ToList();
            _log.Info(LogLayer.Service, "TicketService.GetOpenTicketsAsync", $"{open.Count} open of {all.Count} tickets");
            return open;
        }

        public async Task<OperationResult<Ticket>> SaveAsync(TicketDraft draft)
        {
            if (draft == null) throw new ArgumentNullException(nameof(draft));

            _log.Info(LogLayer.Service, "TicketService.SaveAsync", $"validate {draft}");

            var errors = Validate(draft);
            if (errors.Count > 0)
            {
                // A validation failure is an expected outcome, not an exception: the screen shows the message.
                _log.Warn(LogLayer.Service, "TicketService.SaveAsync", $"rejected: {string.Join("; ", errors)}");
                return OperationResult<Ticket>.Fail(errors[0], errors.ToArray());
            }

            Ticket ticket;
            if (draft.Id.HasValue)
            {
                ticket = await _repository.FindAsync(draft.Id.Value);
                if (ticket == null)
                {
                    _log.Warn(LogLayer.Service, "TicketService.SaveAsync", $"ticket #{draft.Id} no longer exists");
                    return OperationResult<Ticket>.Fail("The ticket no longer exists. Refresh the list.");
                }
            }
            else
            {
                ticket = new Ticket();
            }

            ticket.Title = draft.Title.Trim();
            ticket.Priority = draft.Priority;
            ticket.HoursLogged = draft.HoursLogged;
            if (ticket.Status == TicketStatus.Open && ticket.HoursLogged > 0)
                ticket.Status = TicketStatus.InProgress;

            _log.Info(LogLayer.Service, "TicketService.SaveAsync", $"valid → ITicketRepository.UpsertAsync({(draft.Id.HasValue ? "#" + draft.Id : "new")})");
            var saved = await _repository.UpsertAsync(ticket);

            return OperationResult<Ticket>.Ok(saved, $"Ticket #{saved.Id} saved.");
        }

        public async Task<OperationResult<Ticket>> CloseAsync(int ticketId)
        {
            _log.Info(LogLayer.Service, "TicketService.CloseAsync", $"#{ticketId} → ITicketRepository.FindAsync");
            var ticket = await _repository.FindAsync(ticketId);
            if (ticket == null)
                return OperationResult<Ticket>.Fail("The ticket no longer exists. Refresh the list.");

            // The rule lives on the domain object; the service applies it and persists the outcome.
            if (!ticket.CanClose(out string reason))
            {
                _log.Warn(LogLayer.Domain, "Ticket.CanClose", $"#{ticketId} rejected: {reason}");
                return OperationResult<Ticket>.Fail(reason);
            }

            ticket.Close();
            _log.Info(LogLayer.Domain, "Ticket.Close", $"#{ticketId} status → Closed");
            await _repository.UpsertAsync(ticket);
            return OperationResult<Ticket>.Ok(ticket, $"Ticket #{ticketId} closed.");
        }

        private static List<string> Validate(TicketDraft draft)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(draft.Title))
                errors.Add("Title is required.");
            else if (draft.Title.Trim().Length > MaxTitleLength)
                errors.Add($"Title must be {MaxTitleLength} characters or fewer.");
            if (draft.HoursLogged < 0 || draft.HoursLogged > 999)
                errors.Add("Hours logged must be between 0 and 999.");
            return errors;
        }
    }
}
