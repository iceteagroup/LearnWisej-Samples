using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketOps.Data;
using TicketOps.Dialogs;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;

namespace TicketOps.Services
{
    /// <summary>
    /// The transaction side of the approval workflow. The dialog decided what the user wants; this class
    /// decides whether it is allowed and makes it happen — as one unit.
    ///
    /// Two things to notice:
    /// 1. It re-checks what the UI already checked (confirmed? comments on a rejection?) and asks the
    ///    domain rule (<see cref="WorkOrder.CanDecide"/>) itself. UI state is a convenience, never the authority.
    /// 2. It supplies the facts the dialog must never supply — who decided, when — from the trusted side
    ///    (the session identity handed in by AppComposition; Module 11 replaces it with the authenticated principal).
    ///
    /// No Wisej.NET type appears here: the same code runs from a unit test, a bulk job or an API endpoint.
    /// </summary>
    public sealed class ApprovalService : IApprovalService
    {
        private readonly IWorkOrderRepository _repository;
        private readonly ILog _log;
        private readonly string _currentUser;

        public ApprovalService(IWorkOrderRepository repository, ILog log, string currentUser)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _currentUser = string.IsNullOrWhiteSpace(currentUser) ? "unknown" : currentUser;
        }

        public async Task<IReadOnlyList<WorkOrder>> GetQueueAsync()
        {
            _log.Info(LogLayer.Service, "ApprovalService.GetQueueAsync", "→ IWorkOrderRepository.GetAllAsync()");
            var all = await _repository.GetAllAsync();
            var queue = all.OrderBy(w => w.IsPending ? 0 : 1).ThenBy(w => w.Id).ToList();
            _log.Info(LogLayer.Service, "ApprovalService.GetQueueAsync", $"{queue.Count(w => w.IsPending)} pending of {queue.Count}");
            return queue;
        }

        public Task<OperationResult<WorkOrder>> ApplyAsync(int workOrderId, ApprovalDialogResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            _log.Info(LogLayer.Service, "ApprovalService.ApplyAsync", $"#{workOrderId} ← {result}");

            // Gate 1 — the caller should never get here with an unconfirmed result; refuse quietly if it does.
            if (!result.Confirmed)
            {
                _log.Warn(LogLayer.Service, "ApprovalService.ApplyAsync", "result not confirmed → nothing applied (Cancel / ✕ must never reach a write)");
                return Task.FromResult(OperationResult<WorkOrder>.Fail(Strings.DecisionNotConfirmed));
            }

            // Translate the UI's decision into the service's command, adding the trusted facts.
            var command = new ApprovalCommand
            {
                WorkOrderId = workOrderId,
                Action = result.Action,
                Comments = result.Comments,
                DecidedBy = _currentUser,
                DecidedAtUtc = DateTime.UtcNow
            };
            _log.Info(LogLayer.Service, "ApprovalService.ApplyAsync", $"ApprovalDialogResult → ApprovalCommand {command}");
            return ExecuteAsync(command);
        }

        public async Task<OperationResult<WorkOrder>> ExecuteAsync(ApprovalCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            // Gate 2 — the comments rule, re-checked here so it holds for callers that had no dialog.
            if (command.Action == ApprovalAction.Reject && string.IsNullOrWhiteSpace(command.Comments))
            {
                _log.Warn(LogLayer.Service, "ApprovalService.ExecuteAsync", $"#{command.WorkOrderId} rejected: comments are required when rejecting — nothing written");
                return OperationResult<WorkOrder>.Fail(Strings.CommentsRequiredToReject);
            }

            _log.Info(LogLayer.Service, "ApprovalService.ExecuteAsync", $"#{command.WorkOrderId} → IWorkOrderRepository.FindAsync");
            var workOrder = await _repository.FindAsync(command.WorkOrderId);
            if (workOrder == null)
            {
                _log.Warn(LogLayer.Service, "ApprovalService.ExecuteAsync", $"#{command.WorkOrderId} no longer exists");
                return OperationResult<WorkOrder>.Fail(Strings.WorkOrderMissing);
            }

            // Gate 3 — the domain rule: a work order is decided once. The dialog never knew this rule.
            if (!workOrder.CanDecide(out string reason))
            {
                _log.Warn(LogLayer.Domain, "WorkOrder.CanDecide", $"{workOrder.Number} rejected: {reason}");
                return OperationResult<WorkOrder>.Fail(reason);
            }

            workOrder.Decide(command.Action, command.Comments, command.DecidedBy, command.DecidedAtUtc);
            _log.Info(LogLayer.Domain, "WorkOrder.Decide", $"{workOrder.Number} status → {workOrder.Status} (in memory only until the commit)");

            // The transaction boundary: status, audit and notification commit together or not at all.
            using (var tx = _repository.BeginTransaction())
            {
                tx.Update(workOrder);
                tx.RecordAudit(new ApprovalRecord
                {
                    WorkOrderId = workOrder.Id,
                    WorkOrderNumber = workOrder.Number,
                    Action = command.Action,
                    Comments = command.Comments,
                    DecidedBy = command.DecidedBy,
                    DecidedAtUtc = command.DecidedAtUtc
                });
                tx.QueueNotification(workOrder.Requester, $"{workOrder.Number} was {Verb(command.Action)} by {command.DecidedBy}");

                _log.Info(LogLayer.Service, "ApprovalService.ExecuteAsync", "3 writes buffered → tx.CommitAsync() (all-or-nothing)");
                await tx.CommitAsync();       // a throw here propagates: nothing was applied, the handler shows the safe message
            }

            string message = string.Format(Strings.DecisionApplied, workOrder.Number, Verb(command.Action));
            _log.Info(LogLayer.Service, "ApprovalService.ExecuteAsync", $"OK · {message}");
            return OperationResult<WorkOrder>.Ok(workOrder, message);
        }

        private static string Verb(ApprovalAction action) => action == ApprovalAction.Approve ? "approved" : "rejected";
    }
}
