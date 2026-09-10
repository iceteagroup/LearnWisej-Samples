using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Data;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Security;

namespace TicketOps.Services
{
    /// <summary>
    /// The ticket service with authorization enforced where the action executes.
    ///
    /// Every sensitive method follows the same three steps:
    ///   1. read the identity from the server session (<see cref="IUserSession"/>) — never from a parameter the client could set;
    ///   2. ask <see cref="IPermissionService"/>; if denied, AUDIT the denial and THROW before any read or write;
    ///   3. do the work, then audit the success.
    ///
    /// Ask of every method: "if someone skipped the UI and called this directly, would they be stopped?"
    /// Here the answer never depends on a button.
    /// </summary>
    public sealed class TicketService : ITicketService
    {
        private const int MaxNoteLength = 500;

        private readonly ITicketRepository _repository;
        private readonly IUserSession _session;
        private readonly IPermissionService _permissions;
        private readonly IAuditService _audit;
        private readonly ILog _log;

        public TicketService(ITicketRepository repository, IUserSession session, IPermissionService permissions, IAuditService audit, ILog log)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
            _audit = audit ?? throw new ArgumentNullException(nameof(audit));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<IReadOnlyList<Ticket>> GetTicketsAsync()
        {
            Authorize(Permission.ViewTickets, "GetTicketsAsync", "all");
            _log.Info(LogLayer.Service, "TicketService.GetTicketsAsync", "→ ITicketRepository.GetAllAsync()");
            return await _repository.GetAllAsync();
        }

        public async Task<OperationResult<Ticket>> AddNoteAsync(int ticketId, string note)
        {
            var user = Authorize(Permission.AddNote, "AddNote", $"#{ticketId}");

            note = (note ?? string.Empty).Trim();
            if (note.Length == 0)
                return OperationResult<Ticket>.Fail("Type a note first.");
            if (note.Length > MaxNoteLength)
                return OperationResult<Ticket>.Fail($"A note must be {MaxNoteLength} characters or fewer.");

            // The note is user content: the trace ListBox escapes item text (verified at runtime), so it is logged as
            // plain text — encoding it here would show literal entities. Its body never enters the audit trail — only its length does.
            _log.Info(LogLayer.Service, "TicketService.AddNoteAsync",
                $"#{ticketId} note ({note.Length} chars{(HtmlPolicy.LooksLikeMarkup(note) ? ", contains markup — stored as text" : string.Empty)}): {note}");

            var ticket = await _repository.FindAsync(ticketId);
            if (ticket == null)
                return OperationResult<Ticket>.Fail("The ticket no longer exists. Refresh the list.");

            ticket.Note = note;
            var saved = await _repository.UpsertAsync(ticket);
            _audit.Success(user, "AddNote", $"#{ticketId}", $"{note.Length} chars");
            return OperationResult<Ticket>.Ok(saved, $"Note saved on #{ticketId}.");
        }

        public async Task<OperationResult<Ticket>> CloseAsync(int ticketId)
        {
            var user = Authorize(Permission.CloseTicket, "CloseTicket", $"#{ticketId}");

            _log.Info(LogLayer.Service, "TicketService.CloseAsync", $"#{ticketId} → ITicketRepository.FindAsync");
            var ticket = await _repository.FindAsync(ticketId);
            if (ticket == null)
                return OperationResult<Ticket>.Fail("The ticket no longer exists. Refresh the list.");

            if (!ticket.CanClose(out string reason))
            {
                // A domain rule saying no is an expected outcome (result), unlike a permission denial (exception).
                _log.Warn(LogLayer.Domain, "Ticket.CanClose", $"#{ticketId} rejected: {reason}");
                return OperationResult<Ticket>.Fail(reason);
            }

            ticket.Close(user.UserName);
            _log.Info(LogLayer.Domain, "Ticket.Close", $"#{ticketId} status → Closed by {user.UserName}");
            await _repository.UpsertAsync(ticket);
            _audit.Success(user, "CloseTicket", $"#{ticketId}");
            return OperationResult<Ticket>.Ok(ticket, $"Ticket #{ticketId} closed.");
        }

        public async Task<OperationResult<int>> DeleteAsync(int ticketId)
        {
            // The check runs HERE, with the session identity, before _repository is touched.
            var user = Authorize(Permission.DeleteTicket, "DeleteTicket", $"#{ticketId}");

            _log.Info(LogLayer.Service, "TicketService.DeleteAsync", $"#{ticketId} allowed → ITicketRepository.DeleteAsync");
            bool removed = await _repository.DeleteAsync(ticketId);
            if (!removed)
                return OperationResult<int>.Fail("The ticket no longer exists. Refresh the list.");

            _audit.Success(user, "DeleteTicket", $"#{ticketId}");
            return OperationResult<int>.Ok(ticketId, $"Ticket #{ticketId} deleted.");
        }

        /// <summary>
        /// The boundary. Identity from the session; permission from the roles; denial → audited + thrown.
        /// A thrown UnauthorizedAccessException cannot be accidentally treated as success by a caller.
        /// </summary>
        private IUserContext Authorize(Permission permission, string action, string target)
        {
            var user = _session.User;
            if (user == null)
            {
                _log.Warn(LogLayer.Service, $"TicketService.{action}", "denied: not signed in → throw UnauthorizedAccessException");
                _audit.Denied(null, action, target, "not signed in");
                throw new UnauthorizedAccessException("Not signed in.");
            }

            _log.Info(LogLayer.Service, $"TicketService.{action}", $"{target} → IPermissionService.Can({user.UserName}, {permission})");
            if (!_permissions.Can(user, permission))
            {
                string needs = string.Join(" or ", _permissions.RolesGranting(permission));
                _audit.Denied(user, action, target, $"needs {needs}");
                _log.Warn(LogLayer.Service, $"TicketService.{action}", "denied before any read or write → throw UnauthorizedAccessException");
                throw new UnauthorizedAccessException($"{permission} requires the {needs} role.");
            }

            return user;
        }
    }
}
