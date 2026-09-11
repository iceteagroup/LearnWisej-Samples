using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketOps.Data;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Validation;

namespace TicketOps.Services
{
    /// <summary>
    /// The safe save pipeline.
    ///
    ///   1. VALIDATE   — the same pure WorkOrderValidator the editor ran; the server never trusts the client's verdict.
    ///   2. RULES      — load the STORED record, apply WorkOrderRules with the SESSION's role (closed-order freeze,
    ///                   transition table against the stored status, cost approval threshold).
    ///   3. PERSIST    — inside one transaction: stage the row + the audit entry, commit once.
    ///   4. CONFIRM    — return Saved only after the commit returned.
    ///
    /// If step 1 or 2 fails the answer is an Invalid result with every error collected and nothing written.
    /// If step 3 throws, the transaction is rolled back, the failure is logged here with its details and the
    /// exception continues to the handler, which shows a safe message. No control appears in this file: an
    /// import job can call it with no form on screen.
    /// </summary>
    public sealed class WorkOrderService : IWorkOrderService
    {
        private readonly IWorkOrderRepository _repository;
        private readonly WorkOrderValidator _validator;
        private readonly ILog _log;

        public WorkOrderService(IWorkOrderRepository repository, WorkOrderValidator validator, ILog log)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<IReadOnlyList<WorkOrder>> GetWorkOrdersAsync()
        {
            var rows = await _repository.GetAllAsync();
            return rows.OrderBy(o => o.Id).ToList();
        }

        public async Task<SaveResult> SaveAsync(SaveWorkOrderCommand command, SessionContext session)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (session == null) throw new ArgumentNullException(nameof(session));

            // 1. VALIDATE — the server is the gate. A crafted request skips the editor's pre-check, not this line.
            var validation = _validator.Validate(command);
            if (!validation.IsValid)
                return SaveResult.Invalid("validate", validation);

            // 2. RULES + AUTHORIZE — against the stored status and the session's role, never the command's claims.
            WorkOrder stored = null;
            if (command.Id.HasValue)
            {
                stored = await _repository.FindAsync(command.Id.Value);
                if (stored == null)
                {
                    var gone = new ValidationResult();
                    gone.AddSummaryError("The work order no longer exists. Refresh the list.");
                    return SaveResult.Invalid("rules", gone);
                }
            }

            var rules = WorkOrderRules.Check(stored, command, session.Role);
            if (!rules.IsValid)
            {
                _log.Warn(LogLayer.Service, "WorkOrderService.SaveAsync", $"rejected by rules for {session.UserName} ({session.Role}): {rules}");
                return SaveResult.Invalid("rules", rules);
            }

            // 3. PERSIST — atomically. Either the row and its audit entry both land, or neither does.
            var order = stored ?? new WorkOrder();
            order.Title = command.Title.Trim();
            order.AssigneeId = command.AssigneeId.Trim();
            order.Priority = command.Priority;
            order.DueDate = command.DueDate.Date;
            order.EstimatedCost = command.EstimatedCost;
            order.EstimatedHours = command.EstimatedHours;
            order.Status = command.ToStatus;

            using (var tx = _repository.BeginTransaction())
            {
                try
                {
                    int id = tx.Upsert(order);
                    tx.Audit(id, stored == null ? "created" : $"saved ({stored.Status} → {command.ToStatus})", session.UserName);
                    var saved = await tx.CommitAsync();                      // the single moment "it happened"

                    // 4. CONFIRM — only after the commit returned.
                    return SaveResult.Ok(saved, $"Work order #{saved.Id} saved.");
                }
                catch (Exception ex)
                {
                    // Unexpected failure: details stay in the log; the transaction's Dispose rolls back; the handler shows a safe message.
                    _log.Error(LogLayer.Service, "WorkOrderService.SaveAsync", ex, $"persist failed — tx#{tx.Number} rolled back, nothing written");
                    throw;
                }
            }
        }
    }
}
