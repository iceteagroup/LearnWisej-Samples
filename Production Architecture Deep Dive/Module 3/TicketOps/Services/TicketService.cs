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
    /// not the rules: validation, the close rule and the meaning of every filter chip are the real ones.
    ///
    /// Notice what is NOT here: no control, no Text property, no profile name. The service does not know
    /// whether the caller is a desktop grid, a tablet tab or a phone view — that is what lets one screen
    /// re-arrange itself without duplicating a single decision.
    /// </summary>
    public sealed class TicketService : ITicketService
    {
        private const int MaxTitleLength = 80;
        private const int MinSearchLength = 2;

        private readonly ITicketRepository _repository;
        private readonly ILog _log;
        private readonly string _currentUser;

        public TicketService(ITicketRepository repository, ILog log, string currentUser)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _currentUser = currentUser ?? "";
        }

        public async Task<IReadOnlyList<Ticket>> GetOpenTicketsAsync()
        {
            _log.Info(LogLayer.Service, "TicketService.GetOpenTicketsAsync", "→ ITicketRepository.GetAllAsync()");
            var all = await _repository.GetAllAsync();
            var open = Sort(all.Where(t => t.Status != TicketStatus.Closed)).ToList();
            _log.Info(LogLayer.Service, "TicketService.GetOpenTicketsAsync", $"{open.Count} open of {all.Count} tickets");
            return open;
        }

        public async Task<OperationResult<IReadOnlyList<Ticket>>> SearchAsync(TicketFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            _log.Info(LogLayer.Service, "TicketService.SearchAsync", $"validate {filter}");

            string text = (filter.Text ?? "").Trim();
            if (text.Length > 0 && text.Length < MinSearchLength)
            {
                // An expected outcome, not an exception: the screen shows the sentence.
                _log.Warn(LogLayer.Service, "TicketService.SearchAsync", $"rejected: query \"{text}\" is shorter than {MinSearchLength} characters");
                return OperationResult<IReadOnlyList<Ticket>>.Fail($"Type at least {MinSearchLength} characters to search.");
            }

            if (!string.IsNullOrEmpty(filter.Chip) && !TicketFilter.Chips.Contains(filter.Chip))
            {
                _log.Warn(LogLayer.Service, "TicketService.SearchAsync", $"rejected: unknown chip \"{filter.Chip}\"");
                return OperationResult<IReadOnlyList<Ticket>>.Fail($"\"{filter.Chip}\" is not a known filter.");
            }

            var all = await _repository.GetAllAsync();
            IEnumerable<Ticket> rows = all.Where(t => t.Status != TicketStatus.Closed);

            if (text.Length > 0)
                rows = rows.Where(t => Matches(t, text));

            rows = ApplyChip(rows, filter.Chip);

            var list = Sort(rows).ToList();
            string what = filter.IsEmpty ? "no filter" : filter.ToString();
            _log.Info(LogLayer.Service, "TicketService.SearchAsync", $"{list.Count} of {all.Count} tickets match {what}");
            return OperationResult<IReadOnlyList<Ticket>>.Ok(list, list.Count == 1 ? "1 ticket" : $"{list.Count} tickets");
        }

        public async Task<OperationResult<Ticket>> SaveAsync(TicketDraft draft)
        {
            if (draft == null) throw new ArgumentNullException(nameof(draft));

            _log.Info(LogLayer.Service, "TicketService.SaveAsync", $"validate {draft}");

            var errors = Validate(draft);
            if (errors.Count > 0)
            {
                _log.Warn(LogLayer.Service, "TicketService.SaveAsync", $"rejected: {string.Join("; ", errors)}");
                return OperationResult<Ticket>.Fail(errors[0], errors.ToArray());
            }

            Ticket ticket;
            bool isNew = !draft.Id.HasValue;
            if (isNew)
            {
                ticket = new Ticket();
            }
            else
            {
                ticket = await _repository.FindAsync(draft.Id.Value);
                if (ticket == null)
                {
                    _log.Warn(LogLayer.Service, "TicketService.SaveAsync", $"ticket #{draft.Id} no longer exists");
                    return OperationResult<Ticket>.Fail("The ticket no longer exists. Refresh the list.");
                }
            }

            ticket.Title = draft.Title.Trim();
            ticket.Priority = draft.Priority;
            ticket.Assignee = (draft.Assignee ?? "").Trim();
            ticket.HoursLogged = draft.HoursLogged;
            ticket.Notes = (draft.Notes ?? "").Trim();
            if (ticket.Status == TicketStatus.Open && ticket.HoursLogged > 0)
                ticket.Status = TicketStatus.InProgress;

            _log.Info(LogLayer.Service, "TicketService.SaveAsync", $"valid → ITicketRepository.UpsertAsync({(isNew ? "new" : "#" + draft.Id)})");
            var saved = await _repository.UpsertAsync(ticket);

            await _repository.AppendEventAsync(new TicketEvent
            {
                TicketId = saved.Id,
                Text = isNew ? $"#{saved.Id} created by {_currentUser}" : $"{_currentUser} updated #{saved.Id}"
            });

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
            await _repository.AppendEventAsync(new TicketEvent { TicketId = ticketId, Text = $"{_currentUser} closed #{ticketId}" });
            return OperationResult<Ticket>.Ok(ticket, $"Ticket #{ticketId} closed.");
        }

        public async Task<OperationResult<IReadOnlyList<TicketEvent>>> GetActivityAsync(string filterText)
        {
            string text = (filterText ?? "").Trim();
            if (text.Length > 0 && text.Length < MinSearchLength)
            {
                _log.Warn(LogLayer.Service, "TicketService.GetActivityAsync", $"rejected: filter \"{text}\" is shorter than {MinSearchLength} characters");
                return OperationResult<IReadOnlyList<TicketEvent>>.Fail($"Type at least {MinSearchLength} characters to filter the activity.");
            }

            _log.Info(LogLayer.Service, "TicketService.GetActivityAsync", text.Length == 0 ? "→ ITicketRepository.GetEventsAsync()" : $"→ GetEventsAsync() filtered by \"{text}\"");
            var events = await _repository.GetEventsAsync();
            IReadOnlyList<TicketEvent> list = text.Length == 0
                ? events
                : events.Where(e => e.Text.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            return OperationResult<IReadOnlyList<TicketEvent>>.Ok(list, $"{list.Count} events");
        }

        #region Decisions (no UI anywhere)

        private IEnumerable<Ticket> ApplyChip(IEnumerable<Ticket> rows, string chip)
        {
            switch (chip)
            {
                case TicketFilter.ChipOpen: return rows.Where(t => t.Status == TicketStatus.Open);
                case TicketFilter.ChipHigh: return rows.Where(t => t.Priority == TicketPriority.High);
                case TicketFilter.ChipMine: return rows.Where(t => string.Equals(t.Assignee, _currentUser, StringComparison.OrdinalIgnoreCase));
                case TicketFilter.ChipToday: return rows.Where(t => t.CreatedAt.Date == DateTime.Today);
                case TicketFilter.ChipUnassigned: return rows.Where(t => !t.IsAssigned);
                case TicketFilter.ChipEscalated: return rows.Where(t => t.IsEscalated);
                default: return rows;
            }
        }

        private static bool Matches(Ticket t, string text)
        {
            return (t.Title ?? "").IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0
                || (t.Assignee ?? "").IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0
                || t.Id.ToString().Contains(text);
        }

        private static IEnumerable<Ticket> Sort(IEnumerable<Ticket> rows)
            => rows.OrderByDescending(t => t.Priority).ThenBy(t => t.Id);

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

        #endregion
    }
}
