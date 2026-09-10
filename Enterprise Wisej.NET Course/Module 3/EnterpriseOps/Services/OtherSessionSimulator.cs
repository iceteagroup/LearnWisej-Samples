using System;
using System.Threading.Tasks;
using EnterpriseOps.Data;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// "Session B — tab 2" from the walkthrough, without asking the reviewer to open a second browser.
    ///
    /// It builds a <b>second, independent</b> <see cref="SessionContext"/> for another user, gives it its own
    /// <see cref="WorkOrderService"/>, and saves through the same shared store — which is precisely what a
    /// second browser session would do, because the session context is per session and the store is not. The
    /// only thing it borrows from this session is the activity trace, so the reviewer can watch both sessions
    /// on one screen.
    ///
    /// Open two browser tabs on the sample instead and the outcome is identical: two SessionContext objects,
    /// two tokens, one store, one winner.
    /// </summary>
    public sealed class OtherSessionSimulator
    {
        private readonly IWorkOrderStore _store;
        private readonly AuditTrail _audit;
        private readonly ActivityTrace _trace;

        public OtherSessionSimulator(IWorkOrderStore store, AuditTrail audit, ActivityTrace trace)
        {
            _store = store;
            _audit = audit;
            _trace = trace;
        }

        /// <summary>
        /// Signs another user in, opens the same work order in their session and saves it. Their save is not
        /// stale — they read the current version a moment ago — so it commits and moves the version on, which
        /// is what turns this session's pending edit into a stale one.
        /// </summary>
        public async Task<OtherSessionSaveResult> SaveAsync(string userId, string tenantId, int workOrderId,
            Func<WorkOrderEditModel, string> newTitle, WorkOrderStatus newStatus)
        {
            var identity = new IdentityProvider().SignIn(userId);
            var session = SessionContext.Create(identity, Guid.NewGuid().ToString("N"));
            _trace.Session($"session B started — {session} (a second SessionContext object, same server process, same store)");

            // Session B works in the same tenant — and gets there the same way anybody does: by asking its own
            // session context, which checks its own entitlement. There is no back door for a simulated session.
            TenantSwitchResult switched = session.SwitchTenant(tenantId);
            if (!switched.Succeeded)
                return new OtherSessionSaveResult { Succeeded = false, Message = $"Session B ({userId}) is not entitled to '{tenantId}'." };

            var service = new WorkOrderService(_store, new TenantGuard(_trace), _audit, _trace);

            CommandContext open = session.BeginCommand("work-order.open (session B)");
            WorkOrderEditModel theirs = await service.OpenAsync(open, workOrderId);
            if (theirs == null)
                return new OtherSessionSaveResult { Succeeded = false, Message = $"Session B could not find work order #{workOrderId}." };

            CommandContext save = session.BeginCommand("work-order.save (session B)");
            var command = new SaveWorkOrderCommand(theirs.Id, theirs.TenantId, newTitle(theirs), newStatus, theirs.Token);
            SaveWorkOrderResult result = await service.SaveAsync(save, command);

            if (!result.Succeeded)
                return new OtherSessionSaveResult { Succeeded = false, Message = "Session B's save was itself rejected: " + string.Join(" ", result.Errors) };

            _trace.Session($"session B saved and ended — {theirs.Token.ToDisplayText()} → {result.NewVersion.ToDisplayText()} · correlation {save.CorrelationId}. " +
                           $"This session still holds {theirs.Token.ToDisplayText()}.");

            return new OtherSessionSaveResult
            {
                Succeeded = true,
                UserId = session.UserId,
                CorrelationId = save.CorrelationId,
                PreviousVersion = theirs.Token,
                NewVersion = result.NewVersion,
                Title = result.Saved.Title,
                Status = result.Saved.Status,
                Message = $"{session.UserId} saved #{workOrderId} in another session: " +
                          $"{theirs.Token.ToDisplayText()} → {result.NewVersion.ToDisplayText()} · correlation {save.CorrelationId}",
            };
        }
    }

    /// <summary>What the other session did, so this screen can say it in one sentence.</summary>
    public sealed class OtherSessionSaveResult
    {
        public bool Succeeded { get; init; }
        public string UserId { get; init; }
        public string CorrelationId { get; init; }
        public ConcurrencyToken PreviousVersion { get; init; }
        public ConcurrencyToken NewVersion { get; init; }
        public string Title { get; init; }
        public WorkOrderStatus Status { get; init; }
        public string Message { get; init; }
    }
}
