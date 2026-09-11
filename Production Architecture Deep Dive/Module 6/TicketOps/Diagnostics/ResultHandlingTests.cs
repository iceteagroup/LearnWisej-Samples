using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Data;
using TicketOps.Dialogs;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using TicketOps.Services;

namespace TicketOps.Diagnostics
{
    /// <summary>What one test case reported.</summary>
    public sealed class TestOutcome
    {
        public bool Passed { get; init; }
        public string Detail { get; init; }
    }

    /// <summary>One unit-style test of the result handling: a name, what it expects, and how to run it.</summary>
    public sealed class ResultHandlingTestCase
    {
        public string Id { get; init; }
        public string Name { get; init; }
        public string Expectation { get; init; }
        public Func<Task<TestOutcome>> Run { get; init; }
    }

    /// <summary>
    /// The lab's "unit-style test cases for result handling", runnable inside the app (no test runner on the
    /// machine is assumed). Every case builds its own fixture — a fresh seeded repository and an
    /// ApprovalService — and hands the service an <see cref="ApprovalDialogResult"/> exactly as the dialog
    /// would, so the cases prove the service's behaviour without a browser or a dialog. No Wisej.NET type
    /// appears here: this file would move to an xUnit project unchanged.
    ///
    /// The same six cases are written up in docs/TestCases.md.
    /// </summary>
    public sealed class ResultHandlingTests
    {
        private const int PendingOrder = 2002;      // WO-2002 · Repair loading dock pump
        private const int DecidedOrder = 2001;      // WO-2001 · already approved in the seed
        private const int SeedAuditRows = 2;
        private const string Approver = "test-approver";

        private readonly ILog _log;

        public ResultHandlingTests(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public IReadOnlyList<ResultHandlingTestCase> Cases => new[]
        {
            new ResultHandlingTestCase
            {
                Id = "RH-1", Name = "Approve, confirmed",
                Expectation = "Succeeded · status Approved · +1 audit row · +1 notification",
                Run = ApproveConfirmed
            },
            new ResultHandlingTestCase
            {
                Id = "RH-2", Name = "Reject with comments, confirmed",
                Expectation = "Succeeded · status Rejected · comments stored on the work order",
                Run = RejectWithComments
            },
            new ResultHandlingTestCase
            {
                Id = "RH-3", Name = "Reject without comments (forged past the dialog)",
                Expectation = "Fail \"" + Strings.CommentsRequiredToReject + "\" · nothing written",
                Run = RejectWithoutComments
            },
            new ResultHandlingTestCase
            {
                Id = "RH-4", Name = "Not confirmed (Cancel / ✕)",
                Expectation = "Fail \"" + Strings.DecisionNotConfirmed + "\" · work order unchanged",
                Run = NotConfirmed
            },
            new ResultHandlingTestCase
            {
                Id = "RH-5", Name = "Decide an already-decided order",
                Expectation = "Fail with the WorkOrder.CanDecide reason · original decision kept",
                Run = AlreadyDecided
            },
            new ResultHandlingTestCase
            {
                Id = "RH-6", Name = "Repository outage during commit, then recovery",
                Expectation = "the commit throws · 0 of 3 writes applied · retry after recovery succeeds",
                Run = OutageDuringCommit
            }
        };

        private (InMemoryWorkOrderRepository repository, IApprovalService service) Fixture()
        {
            var repository = new InMemoryWorkOrderRepository();
            return (repository, new ApprovalService(repository, _log, Approver));
        }

        private async Task<TestOutcome> ApproveConfirmed()
        {
            var (repo, service) = Fixture();
            var result = await service.ApplyAsync(PendingOrder, ApprovalDialogResult.Confirm(ApprovalAction.Approve, "Quote checked against the vendor contract."));
            var wo = await repo.FindAsync(PendingOrder);

            bool passed = result.Succeeded
                && wo.Status == WorkOrderStatus.Approved
                && wo.DecidedBy == Approver
                && repo.AuditCount == SeedAuditRows + 1
                && repo.NotificationCount == 1;
            return new TestOutcome { Passed = passed, Detail = $"{result} · status {wo.Status} · audit {repo.AuditCount} · notifications {repo.NotificationCount}" };
        }

        private async Task<TestOutcome> RejectWithComments()
        {
            var (repo, service) = Fixture();
            const string comments = "Pump model mismatch — needs a re-quote first.";
            var result = await service.ApplyAsync(PendingOrder, ApprovalDialogResult.Confirm(ApprovalAction.Reject, comments));
            var wo = await repo.FindAsync(PendingOrder);

            bool passed = result.Succeeded
                && wo.Status == WorkOrderStatus.Rejected
                && wo.DecisionComments == comments
                && repo.AuditCount == SeedAuditRows + 1;
            return new TestOutcome { Passed = passed, Detail = $"{result} · status {wo.Status} · comments \"{wo.DecisionComments}\"" };
        }

        private async Task<TestOutcome> RejectWithoutComments()
        {
            var (repo, service) = Fixture();
            // The dialog would never confirm this; the service must refuse it anyway.
            var result = await service.ApplyAsync(PendingOrder, ApprovalDialogResult.Confirm(ApprovalAction.Reject, "   "));
            var wo = await repo.FindAsync(PendingOrder);

            bool passed = !result.Succeeded
                && result.Message == Strings.CommentsRequiredToReject
                && wo.Status == WorkOrderStatus.Pending
                && repo.AuditCount == SeedAuditRows
                && repo.NotificationCount == 0;
            return new TestOutcome { Passed = passed, Detail = $"{result} · status {wo.Status} · audit {repo.AuditCount}" };
        }

        private async Task<TestOutcome> NotConfirmed()
        {
            var (repo, service) = Fixture();
            var result = await service.ApplyAsync(PendingOrder, ApprovalDialogResult.NotConfirmed());
            var wo = await repo.FindAsync(PendingOrder);

            bool passed = !result.Succeeded
                && result.Message == Strings.DecisionNotConfirmed
                && wo.Status == WorkOrderStatus.Pending
                && wo.DecidedBy == null
                && repo.AuditCount == SeedAuditRows;
            return new TestOutcome { Passed = passed, Detail = $"{result} · status {wo.Status} · decidedBy {(wo.DecidedBy ?? "—")}" };
        }

        private async Task<TestOutcome> AlreadyDecided()
        {
            var (repo, service) = Fixture();
            var before = await repo.FindAsync(DecidedOrder);
            var result = await service.ApplyAsync(DecidedOrder, ApprovalDialogResult.Confirm(ApprovalAction.Reject, "Trying to flip a closed decision."));
            var after = await repo.FindAsync(DecidedOrder);

            bool passed = !result.Succeeded
                && result.Message.Contains("already approved")
                && after.Status == before.Status
                && after.DecidedBy == before.DecidedBy
                && after.DecisionComments == before.DecisionComments
                && repo.AuditCount == SeedAuditRows;
            return new TestOutcome { Passed = passed, Detail = $"{result} · still {after.Status} by {after.DecidedBy}" };
        }

        private async Task<TestOutcome> OutageDuringCommit()
        {
            var repo = new InMemoryWorkOrderRepository();
            var failing = new FailingCommitRepository(repo);
            var service = new ApprovalService(failing, _log, Approver);
            var confirmed = ApprovalDialogResult.Confirm(ApprovalAction.Approve, "Approved during the outage drill.");

            failing.FailCommits = true;
            string caught = null;
            try
            {
                await service.ApplyAsync(PendingOrder, confirmed);
            }
            catch (StoreUnavailableException ex)
            {
                caught = ex.GetType().Name;
            }
            failing.FailCommits = false;

            var wo = await repo.FindAsync(PendingOrder);
            bool nothingApplied = wo.Status == WorkOrderStatus.Pending
                && wo.DecidedBy == null
                && repo.AuditCount == SeedAuditRows
                && repo.NotificationCount == 0;

            // Recovery: the same confirmed result applies cleanly once the store is back.
            var retry = await service.ApplyAsync(PendingOrder, confirmed);
            var afterRetry = await repo.FindAsync(PendingOrder);
            bool recovered = retry.Succeeded && afterRetry.Status == WorkOrderStatus.Approved && repo.AuditCount == SeedAuditRows + 1;

            bool passed = caught == nameof(StoreUnavailableException) && nothingApplied && recovered;
            return new TestOutcome
            {
                Passed = passed,
                Detail = $"caught {caught ?? "nothing"} · after outage: status {wo.Status}, audit {repo.AuditCount}, notifications {repo.NotificationCount} · retry {retry}"
            };
        }

        /// <summary>Thrown by the test double when the store refuses a commit.</summary>
        private sealed class StoreUnavailableException : Exception
        {
            public StoreUnavailableException() : base("The store did not accept the commit.") { }
        }

        /// <summary>Test double: delegates to a real repository, but its transactions can be told to fail on commit.</summary>
        private sealed class FailingCommitRepository : IWorkOrderRepository
        {
            private readonly IWorkOrderRepository _inner;

            public FailingCommitRepository(IWorkOrderRepository inner) => _inner = inner;

            public bool FailCommits { get; set; }

            public Task<IReadOnlyList<WorkOrder>> GetAllAsync() => _inner.GetAllAsync();
            public Task<WorkOrder> FindAsync(int id) => _inner.FindAsync(id);
            public Task<IReadOnlyList<ApprovalRecord>> GetAuditTrailAsync() => _inner.GetAuditTrailAsync();
            public IWorkOrderTransaction BeginTransaction() => new Transaction(_inner.BeginTransaction(), this);

            private sealed class Transaction : IWorkOrderTransaction
            {
                private readonly IWorkOrderTransaction _inner;
                private readonly FailingCommitRepository _owner;

                public Transaction(IWorkOrderTransaction inner, FailingCommitRepository owner)
                {
                    _inner = inner;
                    _owner = owner;
                }

                public void Update(WorkOrder workOrder) => _inner.Update(workOrder);
                public void RecordAudit(ApprovalRecord record) => _inner.RecordAudit(record);
                public void QueueNotification(string recipient, string message) => _inner.QueueNotification(recipient, message);

                public Task CommitAsync()
                {
                    if (_owner.FailCommits)
                        throw new StoreUnavailableException();
                    return _inner.CommitAsync();
                }

                public void Dispose() => _inner.Dispose();
            }
        }
    }
}
